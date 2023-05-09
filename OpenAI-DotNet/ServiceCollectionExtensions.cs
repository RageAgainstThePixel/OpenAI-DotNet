using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace OpenAI;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the <see cref="IOpenAIClient"/> as a singleton in the <see cref="IServiceCollection"/>
    /// with the supplied authentication, settings, and HTTP client.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="openAIAuthentication"></param>
    /// <param name="clientSettings"></param>
    /// <param name="client"></param>
    /// <returns>A reference to this instance after the operation has completed</returns>
    public static IServiceCollection AddOpenAI(this IServiceCollection services,
        OpenAIAuthentication openAIAuthentication,
        OpenAIClientSettings clientSettings,
        HttpClient client)
    {
        return services.AddSingleton<IOpenAIClient>(new OpenAIClient(openAIAuthentication, clientSettings, client));
    }

    /// <summary>
    /// Registers the <see cref="IOpenAIClient"/> as a singleton in the <see cref="IServiceCollection"/>
    /// with the supplied authentication and settings.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="openAIAuthentication"></param>
    /// <param name="clientSettings"></param>
    /// <returns>A reference to this instance after the operation has completed</returns>
    public static IServiceCollection AddOpenAI(this IServiceCollection services,
        OpenAIAuthentication openAIAuthentication,
        OpenAIClientSettings clientSettings) => AddOpenAI(services, openAIAuthentication, clientSettings, null);
    
    /// <summary>
    /// Registers the <see cref="IOpenAIClient"/> as a singleton in the <see cref="IServiceCollection"/>
    /// using the default authentication and settings.
    /// </summary>
    /// <param name="services"></param>
    /// <returns>A reference to this instance after the operation has completed</returns>
    public static IServiceCollection AddOpenAI(this IServiceCollection services) => AddOpenAI(services,
        openAIAuthentication: null,
        clientSettings: null,
        client: null);
}