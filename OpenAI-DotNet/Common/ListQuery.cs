// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenAI
{
    public sealed class ListQuery
    {
        [Obsolete("use new .ctr overload")]
        public ListQuery(int? limit, SortOrder order, string after, string before)
            : this(after, before, null, limit, order)
        {
        }

        /// <summary>
        /// List Query.
        /// </summary>
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
        /// <param name="include">
        /// Additional fields to include in the response.
        /// </param>
        /// <param name="limit">
        /// A limit on the number of objects to be returned.
        /// Limit can range between 1 and 100, and the default is 20.
        /// </param>
        /// <param name="order">
        /// Sort order by the 'created_at' timestamp of the objects.
        /// </param>
        public ListQuery(string after = null, string before = null, IEnumerable<string> include = null, int? limit = null, SortOrder order = SortOrder.Descending)
        {
            After = after;
            Before = before;
            Include = include?.ToList();
            Limit = limit;
            Order = order;
        }

        /// <summary>
        /// An item ID to list items after, used in pagination.
        /// </summary>
        public string After { get; set; }

        /// <summary>
        /// An item ID to list items before, used in pagination.
        /// </summary>
        public string Before { get; set; }

        /// <summary>
        /// Additional fields to include in the response.
        /// </summary>
        public IEnumerable<string> Include { get; set; }

        /// <summary>
        /// Additional fields to include in the response.
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// A limit on the number of objects to be returned. Limit can range between 1 and 100, and the default is 20.
        /// </summary>
        public SortOrder Order { get; set; }


        public static implicit operator Dictionary<string, string>(ListQuery query)
        {
            if (query == null) { return null; }
            var parameters = new Dictionary<string, string>();


            if (!string.IsNullOrEmpty(query.After))
            {
                parameters.Add("after", query.After);
            }

            if (!string.IsNullOrEmpty(query.Before))
            {
                parameters.Add("before", query.Before);
            }

            var includes = query.Include?.ToList() ?? new List<string>();

            if (includes is { Count: > 0 })
            {
                parameters.Add("include", string.Join(",", includes));
            }

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

            return parameters;
        }
    }
}
