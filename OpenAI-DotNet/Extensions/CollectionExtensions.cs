// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace OpenAI.Extensions
{
    internal static class CollectionExtensions
    {
        public static void AppendFrom<T>(this List<T> self, IReadOnlyList<T> other)
            where T : IAppendable<T>, new()
        {
            if (other == null)
            {
                return;
            }

            foreach (var otherItem in other)
            {
                if (otherItem == null) { continue; }

                if (otherItem.Index.HasValue)
                {
                    if (otherItem.Index + 1 > self.Count)
                    {
                        var newItem = new T();
                        newItem.AppendFrom(otherItem);
                        self.Insert(otherItem.Index.Value, newItem);
                    }
                    else
                    {
                        self[otherItem.Index.Value].AppendFrom(otherItem);
                    }
                }
                else
                {
                    var newItem = new T();
                    newItem.AppendFrom(otherItem);
                    self.Add(newItem);
                }
            }
        }
    }
}
