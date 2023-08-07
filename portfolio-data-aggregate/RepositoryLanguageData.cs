using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Http.Json;
using portfolio_data_aggregate.Models.Responses;
using portfolio_data_aggregate.Utility;
using System.Linq;
using System.Collections.Generic;

namespace portfolio_data_aggregate
{
    public static class RepositoryLanguageData
    {
        private static HttpClient _githubClient = new HttpClient()
        {
            BaseAddress = new Uri("https://api.github.com/repos/"),
        };

        [FunctionName("RepositoryLanguages")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, ILogger log)
        {
            string user = req.Query["user"];
            string repo = req.Query["repo"];
            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(repo)) return new BadRequestObjectResult("No user or repo provided");
            string repositoryURI = WebPath.Combine(user, repo);

            // TODO: Add cache for the result to reduce subsequent requests

            // Add user agent, as GitHub requires it
            _githubClient.DefaultRequestHeaders.UserAgent.ParseAdd("Azure Function");

            // Find all connected repositories, e.g. any submodules
            // First get the latest commit sha
            string commitURI = WebPath.Combine(repositoryURI, "/commits?page=1&per_page=1");
            var commit = (await _githubClient.GetFromJsonAsync<GetCommitResponse[]>(commitURI)).ElementAtOrDefault(0);
            if (commit is null) return new BadRequestObjectResult("Could not find given repository with any commits");
            // Now get the commit tree
            string treeURI = WebPath.Combine(repositoryURI, $"/git/trees/{commit.commit.tree.sha}");
            var tree = await _githubClient.GetFromJsonAsync<GetTreeResponse>(treeURI);
            var submodules = tree.tree.Where(t => t.mode == "160000").Select(m => WebPath.Combine(user, m.path)).ToList();

            // Now get the repository language data of all submodules and projects
            // It will be expected that the path is the same as repository name, this is not necessary but will reduce the amount of calls required to GitHub
            submodules.Add(repositoryURI);
            var languageURIs = submodules.Select(m => WebPath.Combine(m, "/languages"));
            var languageTasks = languageURIs.Select(uri => _githubClient.GetFromJsonAsync<Dictionary<string, int>>(uri));
            var languageResults = await Task.WhenAll(languageTasks);

            var LanguageData = languageResults
                // Convert to single key-value pair list
                .SelectMany(dict => dict)
                // Group duplicate keys
                .GroupBy(kvp => kvp.Key)
                // Convert back to a single dictionary with the groups merged
                .ToDictionary(g => g.Key, g => g.Sum(kvp => kvp.Value));

            return new OkObjectResult(LanguageData);
        }
    }
}
