using System.Text.Json.Serialization;
using OpenAI.Chat;
using OpenAI.Extensions;

namespace OpenAI.ThreadMessages;

public sealed class ThreadMessageContent
{
    public ThreadMessageContent() { }

    public ThreadMessageContent(ContentType type, string input)
    {
        Type = type;

        switch (Type)
        {
            case ContentType.Text:
                Text = new ThreadMessageContentText
                {
                    Value = input
                };
                break;

            case ContentType.ImageUrl:
                ImageUrl = new ImageUrl(input);
                break;
        }
    }

    [JsonInclude]
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter<ContentType>))]
    public ContentType Type { get; private set; }

    [JsonInclude]
    [JsonPropertyName("text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ThreadMessageContentText Text { get; private set; }

    [JsonInclude]
    [JsonPropertyName("image_url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ImageUrl ImageUrl { get; private set; }
}

public class ThreadMessageContentText
{
    /// <summary>
    /// The data that makes up the text.
    /// </summary>
    /// <returns></returns>
    [JsonPropertyName("value")]
    public string Value { get; set; }
    
    /// <summary>
    /// Annotations
    /// </summary>
    [JsonPropertyName("annotations")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Annotation[] Annotations { get; set; }
}