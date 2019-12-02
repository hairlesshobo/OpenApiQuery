namespace OpenApiQuery
{
    public class OpenApiQueryApplyResult<T>
    {
        internal OpenApiQueryOptions<T> Options { get; set; }
        /// <summary>
        /// Total number of items avaailable based on the current filter. Only populated when
        /// $count=true option is used in filter.
        /// </summary>
        public long? TotalCount { get; set; }

        /// <summary>
        /// Object containing the matching items, optionally limited by the $top value provided
        /// </summary>
        public T[] ResultItems { get; set; }

        public OpenApiQueryApplyResult()
        {
        }

        internal OpenApiQueryApplyResult(OpenApiQueryOptions<T> options, T[] resultItems, long? totalCount)
        {
            Options = options;
            ResultItems = resultItems;
            TotalCount = totalCount;
        }
    }
}
