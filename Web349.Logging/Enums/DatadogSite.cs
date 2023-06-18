using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Web349.Logging.Enums
{
    public static class DatadogSite
    {
        public static readonly string US1 = "https://http-intake.logs.datadoghq.com/api/v2/logs";
        public static readonly string US3 = "https://http-intake.logs.us3.datadoghq.com/api/v2/logs";
        public static readonly string US5 = "https://http-intake.logs.us5.datadoghq.com/api/v2/logs";
        public static readonly string EU = "https://http-intake.logs.datadoghq.eu/api/v2/logs";
        public static readonly string AP1 = "https://http-intake.logs.ap1.datadoghq.com/api/v2/logs";
        public static readonly string US1_FED = "https://http-intake.logs.ddog-gov.com/api/v2/logs";

        public static string GetByString(string site)
        {
            Type t = typeof(DatadogSite);
            var fi = t.GetFields().Where(x => x.Name.Equals(site, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (fi == null)
            {
                return null;
            }
            return fi.GetValue(null) as string;
        }
    }
}
