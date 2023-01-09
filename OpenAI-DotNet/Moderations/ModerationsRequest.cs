using OpenAI.Models;
using System.Text.Json.Serialization;

namespace OpenAI.Moderations
{
    public sealed class ModerationsRequest
    {
        public ModerationsRequest(string input, Model model = null)
        {
            Input = input;
            Model = model;
        }

        [JsonPropertyName("input")]
        public string Input { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }
    }
}