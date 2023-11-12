using System.Text.Json.Serialization;

namespace OpenAI;

public interface IUseRateLimits
{
    [JsonIgnore]
    public RateLimits RateLimits { get; set; }
}