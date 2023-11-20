using System.Collections.Generic;

namespace OpenAI
{
    public interface IListResponse<out TObject>
        where TObject : BaseResponse
    {
        IReadOnlyList<TObject> Items { get; }
    }
}