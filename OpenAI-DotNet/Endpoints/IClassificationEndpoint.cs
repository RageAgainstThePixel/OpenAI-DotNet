using OpenAI;

namespace OpenAI.Endpoints
{
    public interface IClassificationEndpoint
    {
        /// <summary>
        /// This endpoint provides the ability to leverage a labeled set of examples without fine-tuning and can be
        /// used for any text-to-label task. By avoiding fine-tuning, it eliminates the need for hyper-parameter tuning.
        /// The endpoint serves as an "autoML" solution that is easy to configure, and adapt to changing label schema.
        /// Up to 200 labeled examples can be provided at query time. Given a query and a set of labeled examples,
        /// the model will predict the most likely label for the query. Useful as a drop-in replacement for any ML
        /// classification or text-to-label task.
        /// </summary>
        ClassificationEndpoint ClassificationEndpoint { get; }
    }
}
