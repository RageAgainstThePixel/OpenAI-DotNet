namespace OpenAI.FileTunes
{
    public class FineTunesEndpoint : BaseEndPoint
    {
        /// <inheritdoc />
        public FineTunesEndpoint(OpenAIClient api) : base(api) { }

        /// <inheritdoc />
        protected override string GetEndpoint()
        {
            throw new System.NotImplementedException();
        }
    }
}
