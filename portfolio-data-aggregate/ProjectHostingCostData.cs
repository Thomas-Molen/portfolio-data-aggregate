using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using portfolio_data_aggregate.Cache;
using portfolio_data_aggregate.Models;
using portfolio_data_aggregate.Models.Responses;
using portfolio_data_aggregate.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace portfolio_data_aggregate
{
    public class ProjectHostingCostData
    {
        [FunctionName("HostingCosts")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, ILogger log)
        {
            // Get all environment variables loaded on process
            var environmentVariables = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Process);
            
            // Create dictionary of all hosting cost values, by filtering on the variables with designated prefix (HCPM)
            // Number values will be attempted to be parsed as floats according to US standards (e.g. 1,000.20)
            Dictionary<string, float> hostingCost = environmentVariables
                .Cast<DictionaryEntry>()
                .Where(v => v.Key.ToString().StartsWith(HostingCost.VariablePrefix))
                .ToDictionary(
                    v => v.Key.ToString().Substring(HostingCost.VariablePrefix.Length),
                    v =>
                    {
                        if (float.TryParse(v.Value.ToString(), NumberStyles.Any, new CultureInfo("en-us"), out float value)) return value;
                        return 0;
                    });
            
            return new OkObjectResult(hostingCost);
        }
    }
}
