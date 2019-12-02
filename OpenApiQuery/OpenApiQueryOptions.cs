using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenApiQuery.Utils;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using NJsonSchema.Annotations;
using NJsonSchema;

namespace OpenApiQuery
{
    public abstract class OpenApiQueryOptions
    {
        /// <summary>
        /// Limit the returned properties
        /// 
        /// See: https://docs.oasis-open.org/odata/odata/v4.01/csprd06/part2-url-conventions/odata-v4.01-csprd06-part2-url-conventions.html#_Toc21425352
        /// </summary>
        [JsonSchema(JsonObjectType.String)]
        [JsonProperty("$select")]
        public SelectQueryOption Select { get; }

        /// <summary>
        /// Include related navigation properties
        /// 
        /// See: https://docs.oasis-open.org/odata/odata/v4.01/csprd06/part2-url-conventions/odata-v4.01-csprd06-part2-url-conventions.html#_Toc21425351
        /// </summary>
        [JsonSchema(JsonObjectType.String)]
        [JsonProperty("$expand")]
        public ExpandQueryOption Expand { get; }

        /// <summary>
        /// Filter result entities
        /// 
        /// See: https://docs.oasis-open.org/odata/odata/v4.01/csprd06/part2-url-conventions/odata-v4.01-csprd06-part2-url-conventions.html#_Toc21425350
        /// </summary>
        [JsonSchema(JsonObjectType.String)]
        [JsonProperty("$filter")]
        public FilterQueryOption Filter { get; }

        /// <summary>
        /// Specify a custom order the result entities
        /// 
        /// See: https://docs.oasis-open.org/odata/odata/v4.01/csprd06/part2-url-conventions/odata-v4.01-csprd06-part2-url-conventions.html#_Toc21425353
        /// </summary>
        [JsonSchema(JsonObjectType.String)]
        [JsonProperty("$orderby")]
        public OrderByQueryOption OrderBy { get; }

        /// <summary>
        /// Skip N elements in the result set
        /// 
        /// See: https://docs.oasis-open.org/odata/odata/v4.01/csprd06/part2-url-conventions/odata-v4.01-csprd06-part2-url-conventions.html#sec_SystemQueryOptionstopandskip
        /// </summary>
        [JsonSchema(JsonObjectType.String)]
        [JsonProperty("$skip")]
        public SkipQueryOption Skip { get; }


        /// <summary>
        /// Select the top N elements in the result set
        /// 
        /// See: https://docs.oasis-open.org/odata/odata/v4.01/csprd06/part2-url-conventions/odata-v4.01-csprd06-part2-url-conventions.html#sec_SystemQueryOptionstopandskip
        /// </summary>
        [JsonSchema(JsonObjectType.String)]
        [JsonProperty("$top")]
        public TopQueryOption Top { get; }


        /// <summary>
        /// Provide the total count of items in the data source (with filters applied)
        /// 
        /// See: https://docs.oasis-open.org/odata/odata/v4.01/csprd06/part2-url-conventions/odata-v4.01-csprd06-part2-url-conventions.html#sec_SystemQueryOptioncount
        /// </summary>
        [JsonSchema(JsonObjectType.Boolean)]
        [JsonProperty("$count")]
        public CountQueryOption Count { get; }

        [JsonIgnore]
        public Type ElementType { get; }

        internal HttpContext HttpContext { get; set; }
        internal ModelStateDictionary ModelState { get; set; }

        public OpenApiQueryOptions(Type elementType)
        {
            ElementType = elementType;
            SelectExpand = new SelectExpandQueryOption(ElementType);
            Filter = new FilterQueryOption(ElementType);
            OrderBy = new OrderByQueryOption(ElementType);
            Skip = new SkipQueryOption();
            Top = new TopQueryOption();
            Count = new CountQueryOption();
        }

        public void Initialize(HttpContext httpContext, ModelStateDictionary modelState)
        {
            HttpContext = httpContext;
            ModelState = modelState;
            var logger = httpContext.RequestServices.GetRequiredService<ILogger<OpenApiQueryOptions>>();
            SelectExpand.Initialize(httpContext, logger, ModelState);
            Filter.Initialize(httpContext, logger, ModelState);
            OrderBy.Initialize(httpContext, logger, ModelState);
            Skip.Initialize(httpContext, ModelState);
            Top.Initialize(httpContext, ModelState);
            Count.Initialize(httpContext, ModelState);
        }
    }

    [OpenApiQueryParameterBindingAttribute]
    public class OpenApiQueryOptions<T> : OpenApiQueryOptions
    {
        public OpenApiQueryOptions()
            : base(typeof(T))
        {
        }

        public async Task<OpenApiQueryApplyResult<T>> ApplyToAsync(
            IQueryable<T> queryable,
            CancellationToken cancellationToken)
        {
            // The order of applying the items to the queryable is important

            // 1. include all related items for further query options
            queryable = SelectExpand.ApplyTo(queryable);
            // 2. sort to have the correct order for filtering and limiting
            queryable = OrderBy.ApplyTo(queryable);
            // 3. filter the items according to the user input
            queryable = Filter.ApplyTo(queryable);

            long? count = null;
            if (Count.Value == true)
            {
                count = await queryable.LongCountAsync(cancellationToken);
            }

            // 4. apply paging on the sorted and filtered result.
            queryable = Skip.ApplyTo(queryable);
            queryable = Top.ApplyTo(queryable);

            var result = await queryable.ToArrayAsync(cancellationToken);

            return new OpenApiQueryApplyResult<T>(this, result, count);
        }
    }
}
