using System;
using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.Annotations;

namespace OpenApiQuery.NSwag
{
    public class OpenApiQueryOptionsDoc
    {
        /// <summary>
        /// Limit the returned properties
        ///
        /// See: https://docs.oasis-open.org/odata/odata/v4.01/csprd06/part2-url-conventions/odata-v4.01-csprd06-part2-url-conventions.html#_Toc21425352
        /// </summary>
        [JsonProperty("$select")]
        public string Select { get; }

        /// <summary>
        /// Include related navigation properties
        ///
        /// See: https://docs.oasis-open.org/odata/odata/v4.01/csprd06/part2-url-conventions/odata-v4.01-csprd06-part2-url-conventions.html#_Toc21425351
        /// </summary>
        [JsonProperty("$expand")]
        public string Expand { get; }

        /// <summary>
        /// Filter result entities
        ///
        /// See: https://docs.oasis-open.org/odata/odata/v4.01/csprd06/part2-url-conventions/odata-v4.01-csprd06-part2-url-conventions.html#_Toc21425350
        /// </summary>
        [JsonProperty("$filter")]
        public string Filter { get; }

        /// <summary>
        /// Specify a custom order the result entities
        ///
        /// See: https://docs.oasis-open.org/odata/odata/v4.01/csprd06/part2-url-conventions/odata-v4.01-csprd06-part2-url-conventions.html#_Toc21425353
        /// </summary>
        [JsonProperty("$orderby")]
        public string OrderBy { get; }

        /// <summary>
        /// Skip N elements in the result set
        ///
        /// See: https://docs.oasis-open.org/odata/odata/v4.01/csprd06/part2-url-conventions/odata-v4.01-csprd06-part2-url-conventions.html#sec_SystemQueryOptionstopandskip
        /// </summary>
        [JsonProperty("$skip")]
        public int Skip { get; }


        /// <summary>
        /// Select the top N elements in the result set
        ///
        /// See: https://docs.oasis-open.org/odata/odata/v4.01/csprd06/part2-url-conventions/odata-v4.01-csprd06-part2-url-conventions.html#sec_SystemQueryOptionstopandskip
        /// </summary>
        [JsonProperty("$top")]
        public int Top { get; }


        /// <summary>
        /// Provide the total count of items in the data source (with filters applied)
        ///
        /// See: https://docs.oasis-open.org/odata/odata/v4.01/csprd06/part2-url-conventions/odata-v4.01-csprd06-part2-url-conventions.html#sec_SystemQueryOptioncount
        /// </summary>
        [JsonProperty("$count")]
        public bool Count { get; }
    }
}
