// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace OpenAI
{
    public interface IListResponse<out TObject>
        where TObject : IListItem
    {
        IReadOnlyList<TObject> Items { get; }
    }

    public interface IListItem { }
}
