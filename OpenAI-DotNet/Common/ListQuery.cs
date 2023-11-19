using System.Collections.Generic;

namespace OpenAI
{
    public sealed class ListQuery
    {
        /// <summary>
        /// List Query.
        /// </summary>
        /// <param name="limit">
        /// A limit on the number of objects to be returned.
        /// Limit can range between 1 and 100, and the default is 20.
        /// </param>
        /// <param name="order">
        /// Sort order by the 'created_at' timestamp of the objects.
        /// </param>
        /// <param name="after">
        /// A cursor for use in pagination.
        /// after is an object ID that defines your place in the list.
        /// For instance, if you make a list request and receive 100 objects, ending with obj_foo,
        /// your subsequent call can include after=obj_foo in order to fetch the next page of the list.
        /// </param>
        /// <param name="before">
        /// A cursor for use in pagination. before is an object ID that defines your place in the list.
        /// For instance, if you make a list request and receive 100 objects, ending with obj_foo,
        /// your subsequent call can include before=obj_foo in order to fetch the previous page of the list.
        /// </param>
        public ListQuery(int? limit = null, SortOrder order = SortOrder.Descending, string after = null, string before = null)
        {
            Limit = limit;
            Order = order;
            After = after;
            Before = before;
        }

        public int? Limit { get; set; }

        public SortOrder Order { get; set; }

        public string After { get; set; }

        public string Before { get; set; }

        public static implicit operator Dictionary<string, string>(ListQuery query)
        {
            if (query == null) { return null; }
            var parameters = new Dictionary<string, string>();

            if (query.Limit.HasValue)
            {
                parameters.Add("limit", query.Limit.ToString());
            }

            switch (query.Order)
            {
                case SortOrder.Descending:
                    parameters.Add("order", "desc");
                    break;
                case SortOrder.Ascending:
                    parameters.Add("order", "asc");
                    break;
            }

            if (!string.IsNullOrEmpty(query.After))
            {
                parameters.Add("after", query.After);
            }

            if (!string.IsNullOrEmpty(query.Before))
            {
                parameters.Add("before", query.Before);
            }

            return parameters;
        }
    }
}