using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace portfolio_data_aggregate.Utility
{
    public static class WebPath
    {
        public static string Combine(params string[] args)
        {
            var prefixAdjusted = args.Select(x => x.Trim('/'));
            return string.Join("/", prefixAdjusted);
        }
    }
}
