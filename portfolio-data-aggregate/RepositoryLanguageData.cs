using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using portfolio_data_aggregate.Cache;
using portfolio_data_aggregate.Models.Responses;
using portfolio_data_aggregate.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace portfolio_data_aggregate
{
    public class RepositoryLanguageData
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILanguageDataCache _languageDataCache;

        public RepositoryLanguageData(IHttpClientFactory httpContextFactory, ILanguageDataCache languageDataCache)
        {
            _httpClientFactory = httpContextFactory;
            _languageDataCache = languageDataCache;
        }

        [FunctionName("RepositoryLanguages")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, ILogger log)
        {
            string user = req.Query["user"];
            string repo = req.Query["repo"];
            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(repo)) return new BadRequestObjectResult("No user or repo provided");
            string repositoryURI = WebPath.Combine(user, repo);

            // Try getting data from cache
            string cacheKey = $"{user}/{repo}";
            if (_languageDataCache.TryGetValue(cacheKey, out Dictionary<string, int> cacheValue))
            {
                return new OkObjectResult(cacheValue);
            }
            
            // If data was not in cache, obtain it from GitHub
            var githubClient = _httpClientFactory.CreateClient("GitHub");
            // Find all connected repositories, e.g. any submodules
            // First get the latest commit sha
            string commitURI = WebPath.Combine(repositoryURI, "/commits?page=1&per_page=1");
            var commit = (await githubClient.GetFromJsonAsync<GetCommitResponse[]>(commitURI)).ElementAtOrDefault(0);
            if (commit is null) return new BadRequestObjectResult("Could not find given repository with any commits");

            // Now get the commit tree
            string treeURI = WebPath.Combine(repositoryURI, $"/git/trees/{commit.commit.tree.sha}");
            var tree = await githubClient.GetFromJsonAsync<GetTreeResponse>(treeURI);
            var submodules = tree.tree.Where(t => t.mode == "160000").Select(m => WebPath.Combine(user, m.path)).ToList();

            // Now get the repository language data of all submodules and projects
            // It will be expected that the path is the same as repository name, this is not necessary but will reduce the amount of calls required to GitHub
            submodules.Add(repositoryURI);
            var languageURIs = submodules.Select(m => WebPath.Combine(m, "/languages"));
            var languageTasks = languageURIs.Select(uri => githubClient.GetFromJsonAsync<Dictionary<string, int>>(uri));
            var languageResults = await Task.WhenAll(languageTasks);

            var LanguageData = languageResults
                // Convert to single key-value pair list
                .SelectMany(dict => dict)
                // Group duplicate keys
                .GroupBy(kvp => kvp.Key)
                // Convert back to a single dictionary with the groups merged
                .ToDictionary(g => g.Key, g => g.Sum(kvp => kvp.Value));

            // Set data into cache to limit calls to GitHub
            _languageDataCache.TrySetValue(cacheKey, LanguageData);

            return new OkObjectResult(LanguageData);
        }
    }
}
