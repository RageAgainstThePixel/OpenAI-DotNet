using System.Net.Http;
using System.Text;

namespace OpenAI_DotNet
{
    internal static class StringExtensions
    {
        public static StringContent ToJsonStringContent(this string s)
        {
            return new StringContent(s, Encoding.UTF8, "application/json");
        }
    }
}
