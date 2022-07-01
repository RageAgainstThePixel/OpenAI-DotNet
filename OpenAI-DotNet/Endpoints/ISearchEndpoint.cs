using OpenAI;

namespace OpenAI.Endpoints
{
    public interface ISearchEndpoint
    {
        /// <summary>
        /// This endpoint lets you do semantic search over documents. This means that you can provide a query,
        /// such as a natural language question or a statement, and find documents that answer the question
        /// or are semantically related to the statement. The “documents” can be words, sentences, paragraphs
        /// or even longer documents. For example, if you provide documents "White House", "hospital", "school"
        /// and query "the president", you’ll get a different similarity score for each document.
        /// The higher the similarity score, the more semantically similar the document is to the query
        /// (in this example, “White House” will be most similar to “the president”).
        /// </summary>
        SearchEndpoint SearchEndpoint { get; }
    }
}
