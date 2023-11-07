using System;
using System.Net.Http;
using System.Text;

namespace OpenAI.Extensions
{
    internal static class StringExtensions
    {
        /// <summary>
        /// Attempts to get the event data from the string data.
        /// Returns false once the stream is done.
        /// </summary>
        /// <param name="streamData">Raw stream data.</param>
        /// <param name="eventData">Parsed stream data.</param>
        /// <returns>True, if the stream is not done. False if stream is done.</returns>
        public static bool TryGetEventStreamData(this string streamData, out string eventData)
        {
            const string dataTag = "data: ";
            eventData = string.Empty;

            if (streamData.StartsWith(dataTag))
            {
                eventData = streamData[dataTag.Length..].Trim();
            }

            const string doneTag = "[DONE]";
            return eventData != doneTag;
        }

        public static StringContent ToJsonStringContent(this string json)
        {
            const string jsonContent = "application/json";
            return new StringContent(json, Encoding.UTF8, jsonContent);
        }

        internal enum SnakeCaseState
        {
            Start,
            Lower,
            Upper,
            NewWord
        }

        /// <summary>
        /// Shamelessly lifted until proper support w/.net8
        /// https://github.com/JamesNK/Newtonsoft.Json/blob/7b8c3b0ed0380cf76d66894e81bf4d4d5b0bd796/Src/Newtonsoft.Json/Utilities/StringUtils.cs#L200-L276
        /// </summary>
        public static string ToSnakeCase(string @string)
        {
            if (string.IsNullOrEmpty(@string))
            {
                return @string;
            }

            var sb = new StringBuilder();
            var state = SnakeCaseState.Start;

            for (int i = 0; i < @string.Length; i++)
            {
                if (@string[i] == ' ')
                {
                    if (state != SnakeCaseState.Start)
                    {
                        state = SnakeCaseState.NewWord;
                    }
                }
                else if (char.IsUpper(@string[i]))
                {
                    switch (state)
                    {
                        case SnakeCaseState.Upper:
                            bool hasNext = (i + 1 < @string.Length);
                            if (i > 0 && hasNext)
                            {
                                char nextChar = @string[i + 1];
                                if (!char.IsUpper(nextChar) && nextChar != '_')
                                {
                                    sb.Append('_');
                                }
                            }
                            break;
                        case SnakeCaseState.Lower:
                        case SnakeCaseState.NewWord:
                            sb.Append('_');
                            break;
                    }

                    var c = char.ToLowerInvariant(@string[i]);
                    sb.Append(c);

                    state = SnakeCaseState.Upper;
                }
                else if (@string[i] == '_')
                {
                    sb.Append('_');
                    state = SnakeCaseState.Start;
                }
                else
                {
                    if (state == SnakeCaseState.NewWord)
                    {
                        sb.Append('_');
                    }

                    sb.Append(@string[i]);
                    state = SnakeCaseState.Lower;
                }
            }

            return sb.ToString();
        }
    }
}
