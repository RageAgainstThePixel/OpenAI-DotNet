# OpenAI-DotNet

[![Discord](https://img.shields.io/discord/855294214065487932.svg?label=&logo=discord&logoColor=ffffff&color=7389D8&labelColor=6A7EC2)](https://discord.gg/xQgMW9ufN4)
[![NuGet version (OpenAI-DotNet)](https://img.shields.io/nuget/v/OpenAI-DotNet.svg?label=OpenAI-DotNet&logo=nuget)](https://www.nuget.org/packages/OpenAI-DotNet/)
[![NuGet version (OpenAI-DotNet-Proxy)](https://img.shields.io/nuget/v/OpenAI-DotNet-Proxy.svg?label=OpenAI-DotNet-Proxy&logo=nuget)](https://www.nuget.org/packages/OpenAI-DotNet-Proxy/)
[![Nuget Publish](https://github.com/RageAgainstThePixel/OpenAI-DotNet/actions/workflows/Publish-Nuget.yml/badge.svg)](https://github.com/RageAgainstThePixel/OpenAI-DotNet/actions/workflows/Publish-Nuget.yml)

A simple C# .NET client library for [OpenAI](https://openai.com/) to use use chat-gpt, GPT-4, GPT-3.5-Turbo and Dall-E though their RESTful API (currently in beta). Independently developed, this is not an official library and I am not affiliated with OpenAI. An OpenAI API account is required.

Forked from [OpenAI-API-dotnet](https://github.com/OkGoDoIt/OpenAI-API-dotnet).

More context [on Roger Pincombe's blog](https://rogerpincombe.com/openai-dotnet-api).

> This repository is available to transfer to the OpenAI organization if they so choose to accept it.

## Requirements

- This library targets .NET 6.0 and above.
- It should work across console apps, winforms, wpf, asp.net, etc.
- It should also work across Windows, Linux, and Mac.

## Getting started

### Install from NuGet

Install package [`OpenAI-DotNet` from Nuget](https://www.nuget.org/packages/OpenAI-DotNet/).  Here's how via command line:

```powershell
Install-Package OpenAI-DotNet
```

> Looking to [use OpenAI-DotNet in the Unity Game Engine](https://github.com/RageAgainstThePixel/com.openai.unity)? Check out our unity package on OpenUPM:
>
>[![openupm](https://img.shields.io/npm/v/com.openai.unity?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.openai.unity/)

## Documentation

### Table of Contents

- [Authentication](#authentication)
- [Azure OpenAI](#azure-openai)
  - [Azure Active Directory Authentication](#azure-active-directory-authentication) :new:
- [OpenAI API Proxy](#openai-api-proxy) :new:
- [Models](#models)
  - [List Models](#list-models)
  - [Retrieve Models](#retrieve-model)
  - [Delete Fine Tuned Model](#delete-fine-tuned-model)
- [Completions](#completions)
  - [Streaming](#completion-streaming)
- [Chat](#chat)
  - [Chat Completions](#chat-completions)
  - [Streaming](#chat-streaming)
- [Edits](#edits)
  - [Create Edit](#create-edit)
- [Embeddings](#embeddings)
  - [Create Embedding](#create-embeddings)
- [Audio](#audio)
  - [Create Transcription](#create-transcription)
  - [Create Translation](#create-translation)
- [Images](#images)
  - [Create Image](#create-image)
  - [Edit Image](#edit-image)
  - [Create Image Variation](#create-image-variation)
- [Files](#files)
  - [List Files](#list-files)
  - [Upload File](#upload-file)
  - [Delete File](#delete-file)
  - [Retrieve File Info](#retrieve-file-info)
  - [Download File Content](#download-file-content)
- [Fine Tuning](#fine-tuning)
  - [Create Fine Tune Job](#create-fine-tune-job)
  - [List Fine Tune Jobs](#list-fine-tune-jobs)
  - [Retrieve Fine Tune Job Info](#retrieve-fine-tune-job-info)
  - [Cancel Fine Tune Job](#cancel-fine-tune-job)
  - [List Fine Tune Events](#list-fine-tune-events)
  - [Stream Fine Tune Events](#stream-fine-tune-events)
- [Moderations](#moderations)
  - [Create Moderation](#create-moderation)

### Authentication

There are 3 ways to provide your API keys, in order of precedence:

1. [Pass keys directly with constructor](#pass-keys-directly-with-constructor)
2. [Load key from configuration file](#load-key-from-configuration-file)
3. [Use System Environment Variables](#use-system-environment-variables)

You use the `OpenAIAuthentication` when you initialize the API as shown:

#### Pass keys directly with constructor

```csharp
var api = new OpenAIClient("sk-apiKey");
```

Or create a `OpenAIAuthentication` object manually

```csharp
var api = new OpenAIClient(new OpenAIAuthentication("sk-apiKey", "org-yourOrganizationId"));
```

#### Load key from configuration file

Attempts to load api keys from a configuration file, by default `.openai` in the current directory, optionally traversing up the directory tree or in the user's home directory.

To create a configuration file, create a new text file named `.openai` and containing the line:

> Organization entry is optional.

##### Json format

```json
{
  "apiKey": "sk-aaaabbbbbccccddddd",
  "organization": "org-yourOrganizationId"
}
```

##### Deprecated format

```shell
OPENAI_KEY=sk-aaaabbbbbccccddddd
ORGANIZATION=org-yourOrganizationId
```

You can also load the file directly with known path by calling a static method in Authentication:

```csharp
var api = new OpenAIClient(OpenAIAuthentication.LoadFromDirectory("your/path/to/.openai"));;
```

#### Use System Environment Variables

Use your system's environment variables specify an api key and organization to use.

- Use `OPENAI_API_KEY` for your api key.
- Use `OPENAI_ORGANIZATION_ID` to specify an organization.

```csharp
var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
```

### [Azure OpenAI](https://learn.microsoft.com/en-us/azure/cognitive-services/openai/)

You can also choose to use Microsoft's Azure OpenAI deployments as well.

You can find the required information in the Azure Playground by clicking the `View Code` button and view a URL like this:

```markdown
https://{your-resource-name}.openai.azure.com/openai/deployments/{deployment-id}/chat/completions?api-version={api-version}
```

- `your-resource-name` The name of your Azure OpenAI Resource.
- `deployment-id` The deployment name you chose when you deployed the model.
- `api-version` The API version to use for this operation. This follows the YYYY-MM-DD format.

To setup the client to use your deployment, you'll need to pass in `OpenAIClientSettings` into the client constructor.

```csharp
var auth = new OpenAIAuthentication("sk-apiKey");
var settings = new OpenAIClientSettings(resourceName: "your-resource-name", deploymentId: "deployment-id", apiVersion: "api-version");
var api = new OpenAIClient(auth, settings);
```

#### [Azure Active Directory Authentication](https://learn.microsoft.com/en-us/azure/cognitive-services/openai/reference#authentication)

[Authenticate with MSAL](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet) as usual and get access token, then use the access token when creating your `OpenAIAuthentication`. Then be sure to set useAzureActiveDirectory to true when creating your `OpenAIClientSettings`.

[Tutorial: Desktop app that calls web APIs: Acquire a token](https://learn.microsoft.com/en-us/azure/active-directory/develop/scenario-desktop-acquire-token?tabs=dotnet)

```csharp
// get your access token using any of the MSAL methods
var accessToken = result.AccessToken;
var auth = new OpenAIAuthentication(accessToken);
var settings = new OpenAIClientSettings(resourceName: "your-resource", deploymentId: "deployment-id", apiVersion: "api-version", useActiveDirectoryAuthentication: true);
var api = new OpenAIClient(auth, settings);
```

### [OpenAI API Proxy](OpenAI-DotNet-Proxy/Readme.md)

[![NuGet version (OpenAI-DotNet-Proxy)](https://img.shields.io/nuget/v/OpenAI-DotNet-Proxy.svg?label=OpenAI-DotNet-Proxy&logo=nuget)](https://www.nuget.org/packages/OpenAI-DotNet-Proxy/)

Using either the [OpenAI-DotNet](https://github.com/RageAgainstThePixel/OpenAI-DotNet) or [com.openai.unity](https://github.com/RageAgainstThePixel/com.openai.unity) packages directly in your front-end app may expose your API keys and other sensitive information. To mitigate this risk, it is recommended to set up an intermediate API that makes requests to OpenAI on behalf of your front-end app. This library can be utilized for both front-end and intermediary host configurations, ensuring secure communication with the OpenAI API.

#### Front End Example

In the front end example, you will need to securely authenticate your users using your preferred OAuth provider. Once the user is authenticated, exchange your custom auth token with your API key on the backend.

Follow these steps:

1. Setup a new project using either the [OpenAI-DotNet](https://github.com/RageAgainstThePixel/OpenAI-DotNet) or [com.openai.unity](https://github.com/RageAgainstThePixel/com.openai.unity) packages.
2. Authenticate users with your OAuth provider.
3. After successful authentication, create a new `OpenAIAuthentication` object and pass in the custom token with the prefix `sess-`.
4. Create a new `OpenAIClientSettings` object and specify the domain where your intermediate API is located.
5. Pass your new `auth` and `settings` objects to the `OpenAIClient` constructor when you create the client instance.

Here's an example of how to set up the front end:

```csharp
var authToken = await LoginAsync();
var auth = new OpenAIAuthentication($"sess-{authToken}");
var settings = new OpenAIClientSettings(domain: "api.your-custom-domain.com");
var api = new OpenAIClient(auth, settings);
```

This setup allows your front end application to securely communicate with your backend that will be using the OpenAI-DotNet-Proxy, which then forwards requests to the OpenAI API. This ensures that your OpenAI API keys and other sensitive information remain secure throughout the process.

#### Back End Example

In this example, we demonstrate how to set up and use `OpenAIProxyStartup` in a new ASP.NET Core web app. The proxy server will handle authentication and forward requests to the OpenAI API, ensuring that your API keys and other sensitive information remain secure.

1. Create a new [ASP.NET Core minimal web API](https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-6.0) project.
2. Add the OpenAI-DotNet nuget package to your project.
    - Powershell install: `Install-Package OpenAI-DotNet-Proxy`
    - Manually editing .csproj: `<PackageReference Include="OpenAI-DotNet-Proxy" />`
3. Create a new class that inherits from `AbstractAuthenticationFilter` and override the `ValidateAuthentication` method. This will implement the `IAuthenticationFilter` that you will use to check user session token against your internal server.
4. In `Program.cs`, create a new proxy web application by calling `OpenAIProxyStartup.CreateDefaultHost` method, passing your custom `AuthenticationFilter` as a type argument.
5. Create `OpenAIAuthentication` and `OpenAIClientSettings` as you would normally with your API keys, org id, or Azure settings.

```csharp
public partial class Program
{
    private class AuthenticationFilter : AbstractAuthenticationFilter
    {
        public override void ValidateAuthentication(IHeaderDictionary request)
        {
            // You will need to implement your own class to properly test
            // custom issued tokens you've setup for your end users.
            if (!request.Authorization.ToString().Contains(userToken))
            {
                throw new AuthenticationException("User is not authorized");
            }
        }
    }

    public static void Main(string[] args)
    {
        var auth = OpenAIAuthentication.LoadFromEnv();
        var settings = new OpenAIClientSettings(/* your custom settings if using Azure OpenAI */);
        var openAIClient = new OpenAIClient(auth, settings);
        var proxy = OpenAIProxyStartup.CreateDefaultHost<AuthenticationFilter>(args, openAIClient);
        proxy.Run();
    }
}
```

Once you have set up your proxy server, your end users can now make authenticated requests to your proxy api instead of directly to the OpenAI API. The proxy server will handle authentication and forward requests to the OpenAI API, ensuring that your API keys and other sensitive information remain secure.

### [Models](https://beta.openai.com/docs/api-reference/models)

List and describe the various models available in the API. You can refer to the [Models documentation](https://beta.openai.com/docs/models) to understand what models are available and the differences between them.

Also checkout [model endpoint compatibility](https://platform.openai.com/docs/models/model-endpoint-compatibility) to understand which models work with which endpoints.

The Models API is accessed via `OpenAIClient.ModelsEndpoint`

#### [List models](https://beta.openai.com/docs/api-reference/models/list)

Lists the currently available models, and provides basic information about each one such as the owner and availability.

```csharp
var api = new OpenAIClient();
var models = await api.ModelsEndpoint.GetModelsAsync();

foreach (var model in models)
{
    Console.WriteLine(model.ToString());
}
```

#### [Retrieve model](https://beta.openai.com/docs/api-reference/models/retrieve)

Retrieves a model instance, providing basic information about the model such as the owner and permissions.

```csharp
var api = new OpenAIClient();
var model = await api.ModelsEndpoint.GetModelDetailsAsync("text-davinci-003");
Console.WriteLine(model.ToString());
```

#### [Delete Fine Tuned Model](https://beta.openai.com/docs/api-reference/fine-tunes/delete-model)

Delete a fine-tuned model. You must have the Owner role in your organization.

```csharp
var api = new OpenAIClient();
var result = await api.ModelsEndpoint.DeleteFineTuneModelAsync("your-fine-tuned-model");
Assert.IsTrue(result);
```

### [Completions](https://beta.openai.com/docs/api-reference/completions)

Given a prompt, the model will return one or more predicted completions, and can also return the probabilities of alternative tokens at each position.

The Completions API is accessed via `OpenAIClient.CompletionsEndpoint`

```csharp
var api = new OpenAIClient();
var result = await api.CompletionsEndpoint.CreateCompletionAsync("One Two Three One Two", temperature: 0.1, model: Model.Davinci);
Console.WriteLine(result);
```

> To get the `CompletionResult` (which is mostly metadata), use its implicit string operator to get the text if all you want is the completion choice.

#### Completion Streaming

Streaming allows you to get results are they are generated, which can help your application feel more responsive, especially on slow models like Davinci.

```csharp
var api = new OpenAIClient();

await api.CompletionsEndpoint.StreamCompletionAsync(result =>
{
    foreach (var choice in result.Completions)
    {
        Console.WriteLine(choice);
    }
}, "My name is Roger and I am a principal software engineer at Salesforce.  This is my resume:", maxTokens: 200, temperature: 0.5, presencePenalty: 0.1, frequencyPenalty: 0.1, model: Model.Davinci);
```

Or if using [`IAsyncEnumerable{T}`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1?view=net-5.0) ([C# 8.0+](https://docs.microsoft.com/en-us/archive/msdn-magazine/2019/november/csharp-iterating-with-async-enumerables-in-csharp-8))

```csharp
var api = new OpenAIClient();
await foreach (var token in api.CompletionsEndpoint.StreamCompletionEnumerableAsync("My name is Roger and I am a principal software engineer at Salesforce.  This is my resume:", maxTokens: 200, temperature: 0.5, presencePenalty: 0.1, frequencyPenalty: 0.1, model: Model.Davinci))
{
  Console.WriteLine(token);
}
```

### [Chat](https://platform.openai.com/docs/api-reference/chat)

Given a chat conversation, the model will return a chat completion response.

The Chat API is accessed via `OpenAIClient.ChatEndpoint`

#### [Chat Completions](https://platform.openai.com/docs/api-reference/chat/create)

Creates a completion for the chat message

```csharp
var api = new OpenAIClient();
var chatPrompts = new List<ChatPrompt>
{
    new ChatPrompt("system", "You are a helpful assistant."),
    new ChatPrompt("user", "Who won the world series in 2020?"),
    new ChatPrompt("assistant", "The Los Angeles Dodgers won the World Series in 2020."),
    new ChatPrompt("user", "Where was it played?"),
};
var chatRequest = new ChatRequest(chatPrompts);
var result = await api.ChatEndpoint.GetCompletionAsync(chatRequest);
Console.WriteLine(result.FirstChoice);
```

##### [Chat Streaming](https://platform.openai.com/docs/api-reference/chat/create#chat/create-stream)

```csharp
var api = new OpenAIClient();
var chatPrompts = new List<ChatPrompt>
{
    new ChatPrompt("system", "You are a helpful assistant."),
    new ChatPrompt("user", "Who won the world series in 2020?"),
    new ChatPrompt("assistant", "The Los Angeles Dodgers won the World Series in 2020."),
    new ChatPrompt("user", "Where was it played?"),
};
var chatRequest = new ChatRequest(chatPrompts, Model.GPT3_5_Turbo);

await api.ChatEndpoint.StreamCompletionAsync(chatRequest, result =>
{
    Console.WriteLine(result.FirstChoice);
});
```

Or if using [`IAsyncEnumerable{T}`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1?view=net-5.0) ([C# 8.0+](https://docs.microsoft.com/en-us/archive/msdn-magazine/2019/november/csharp-iterating-with-async-enumerables-in-csharp-8))

```csharp
var api = new OpenAIClient();
var chatPrompts = new List<ChatPrompt>
{
    new ChatPrompt("system", "You are a helpful assistant."),
    new ChatPrompt("user", "Who won the world series in 2020?"),
    new ChatPrompt("assistant", "The Los Angeles Dodgers won the World Series in 2020."),
    new ChatPrompt("user", "Where was it played?"),
};
var chatRequest = new ChatRequest(chatPrompts, Model.GPT3_5_Turbo);

await foreach (var result in api.ChatEndpoint.StreamCompletionEnumerableAsync(chatRequest))
{
    Console.WriteLine(result.FirstChoice);
}
```

### [Edits](https://beta.openai.com/docs/api-reference/edits)

Given a prompt and an instruction, the model will return an edited version of the prompt.

The Edits API is accessed via `OpenAIClient.EditsEndpoint`

#### [Create Edit](https://beta.openai.com/docs/api-reference/edits/create)

Creates a new edit for the provided input, instruction, and parameters using the provided input and instruction.

```csharp
var api = new OpenAIClient();
var request = new EditRequest("What day of the wek is it?", "Fix the spelling mistakes");
var result = await api.EditsEndpoint.CreateEditAsync(request);
Console.WriteLine(result);
```

### [Embeddings](https://beta.openai.com/docs/api-reference/embeddings)

Get a vector representation of a given input that can be easily consumed by machine learning models and algorithms.

Related guide: [Embeddings](https://beta.openai.com/docs/guides/embeddings)

The Edits API is accessed via `OpenAIClient.EmbeddingsEndpoint`

#### [Create Embeddings](https://beta.openai.com/docs/api-reference/embeddings/create)

Creates an embedding vector representing the input text.

```csharp
var api = new OpenAIClient();
var model = await api.ModelsEndpoint.GetModelDetailsAsync("text-embedding-ada-002");
var result = await api.EmbeddingsEndpoint.CreateEmbeddingAsync("The food was delicious and the waiter...", model);
Console.WriteLine(result);
```

### [Audio](https://beta.openai.com/docs/api-reference/audio)

Converts audio into text.

The Audio API is accessed via `OpenAIClient.AudioEndpoint`

#### [Create Transcription](https://platform.openai.com/docs/api-reference/audio/create)

Transcribes audio into the input language.

```csharp
var api = new OpenAIClient();
var request = new AudioTranscriptionRequest(Path.GetFullPath(audioAssetPath), language: "en");
var result = await api.AudioEndpoint.CreateTranscriptionAsync(request);
Console.WriteLine(result);
```

#### [Create Translation](https://platform.openai.com/docs/api-reference/audio/create)

Translates audio into into English.

```csharp
var api = new OpenAIClient();
var request = new AudioTranslationRequest(Path.GetFullPath(audioAssetPath));
var result = await api.AudioEndpoint.CreateTranslationAsync(request);
Console.WriteLine(result);
```

### [Images](https://beta.openai.com/docs/api-reference/images)

Given a prompt and/or an input image, the model will generate a new image.

The Images API is accessed via `OpenAIClient.ImagesEndpoint`

#### [Create Image](https://beta.openai.com/docs/api-reference/images/create)

Creates an image given a prompt.

```csharp
var api = new OpenAIClient();
var results = await api.ImagesEndPoint.GenerateImageAsync("A house riding a velociraptor", 1, ImageSize.Small);

foreach (var result in results)
{
    Console.WriteLine(result);
    // result == file://path/to/image.png
}
```

#### [Edit Image](https://beta.openai.com/docs/api-reference/images/create-edit)

Creates an edited or extended image given an original image and a prompt.

```csharp
var api = new OpenAIClient();
var results = await api.ImagesEndPoint.CreateImageEditAsync(Path.GetFullPath(imageAssetPath), Path.GetFullPath(maskAssetPath), "A sunlit indoor lounge area with a pool containing a flamingo", 1, ImageSize.Small);

foreach (var result in results)
{
    Console.WriteLine(result);
    // result == file://path/to/image.png
}
```

#### [Create Image Variation](https://beta.openai.com/docs/api-reference/images/create-variation)

Creates a variation of a given image.

```csharp
var api = new OpenAIClient();
var results = await api.ImagesEndPoint.CreateImageVariationAsync(Path.GetFullPath(imageAssetPath), 1, ImageSize.Small);

foreach (var result in results)
{
    Console.WriteLine(result);
    // result == file://path/to/image.png
}
```

### [Files](https://beta.openai.com/docs/api-reference/files)

Files are used to upload documents that can be used with features like [Fine-tuning](#fine-tuning).

The Files API is accessed via `OpenAIClient.FilesEndpoint`

#### [List Files](https://beta.openai.com/docs/api-reference/files/list)

Returns a list of files that belong to the user's organization.

```csharp
var api = new OpenAIClient();
var files = await api.FilesEndpoint.ListFilesAsync();

foreach (var file in files)
{
    Console.WriteLine($"{file.Id} -> {file.Object}: {file.FileName} | {file.Size} bytes");
}
```

#### [Upload File](https://beta.openai.com/docs/api-reference/files/upload)

Upload a file that contains document(s) to be used across various endpoints/features. Currently, the size of all the files uploaded by one organization can be up to 1 GB. Please contact us if you need to increase the storage limit.

```csharp
var api = new OpenAIClient();
var fileData = await api.FilesEndpoint.UploadFileAsync("path/to/your/file.jsonl", "fine-tune");
Console.WriteLine(fileData.Id);
```

#### [Delete File](https://beta.openai.com/docs/api-reference/files/delete)

Delete a file.

```csharp
var api = new OpenAIClient();
var result = await api.FilesEndpoint.DeleteFileAsync(fileData);
Assert.IsTrue(result);
```

#### [Retrieve File Info](https://beta.openai.com/docs/api-reference/files/retrieve)

Returns information about a specific file.

```csharp
var api = new OpenAIClient();
var fileData = await GetFileInfoAsync(fileId);
Console.WriteLine($"{fileData.Id} -> {fileData.Object}: {fileData.FileName} | {fileData.Size} bytes");
```

#### [Download File Content](https://beta.openai.com/docs/api-reference/files/retrieve-content)

Downloads the specified file.

```csharp
var api = new OpenAIClient();
var downloadedFilePath = await api.FilesEndpoint.DownloadFileAsync(fileId, "path/to/your/save/directory");
Console.WriteLine(downloadedFilePath);
Assert.IsTrue(File.Exists(downloadedFilePath));
```

### [Fine Tuning](https://beta.openai.com/docs/api-reference/fine-tunes)

Manage fine-tuning jobs to tailor a model to your specific training data.

Related guide: [Fine-tune models](https://beta.openai.com/docs/guides/fine-tuning)

The Files API is accessed via `OpenAIClient.FineTuningEndpoint`

#### [Create Fine Tune Job](https://beta.openai.com/docs/api-reference/fine-tunes/create)

Creates a job that fine-tunes a specified model from a given dataset.

Response includes details of the enqueued job including job status and the name of the fine-tuned models once complete.

```csharp
var api = new OpenAIClient();
var request = new CreateFineTuneRequest(fileData);
var fineTuneJob = await api.FineTuningEndpoint.CreateFineTuneJobAsync(request);
Console.WriteLine(fineTuneJob.Id);
```

#### [List Fine Tune Jobs](https://beta.openai.com/docs/api-reference/fine-tunes/list)

List your organization's fine-tuning jobs.

```csharp
var api = new OpenAIClient();
var fineTuneJobs = await api.FineTuningEndpoint.ListFineTuneJobsAsync();

foreach (var job in fineTuneJobs)
{
    Console.WriteLine($"{job.Id} -> {job.Status}");
}
```

#### [Retrieve Fine Tune Job Info](https://beta.openai.com/docs/api-reference/fine-tunes/retrieve)

Gets info about the fine-tune job.

```csharp
var api = new OpenAIClient();
var result = await api.FineTuningEndpoint.RetrieveFineTuneJobInfoAsync(fineTuneJob);
Console.WriteLine($"{result.Id} -> {result.Status}");
```

#### [Cancel Fine Tune Job](https://beta.openai.com/docs/api-reference/fine-tunes/cancel)

Immediately cancel a fine-tune job.

```csharp
var api = new OpenAIClient();
var result = await api.FineTuningEndpoint.CancelFineTuneJobAsync(fineTuneJob);
Assert.IsTrue(result);
```

#### [List Fine Tune Events](https://beta.openai.com/docs/api-reference/fine-tunes/events)

Get fine-grained status updates for a fine-tune job.

```csharp
var api = new OpenAIClient();
var fineTuneEvents = await api.FineTuningEndpoint.ListFineTuneEventsAsync(fineTuneJob);
Console.WriteLine($"{fineTuneJob.Id} -> status: {fineTuneJob.Status} | event count: {fineTuneEvents.Count}");
```

#### [Stream Fine Tune Events](https://beta.openai.com/docs/api-reference/fine-tunes/events#fine-tunes/events-stream)

```csharp
var api = new OpenAIClient();
await api.FineTuningEndpoint.StreamFineTuneEventsAsync(fineTuneJob, fineTuneEvent =>
{
    Console.WriteLine($"  {fineTuneEvent.CreatedAt} [{fineTuneEvent.Level}] {fineTuneEvent.Message}");
});
```

Or if using [`IAsyncEnumerable{T}`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1?view=net-5.0) ([C# 8.0+](https://docs.microsoft.com/en-us/archive/msdn-magazine/2019/november/csharp-iterating-with-async-enumerables-in-csharp-8))

```csharp
var api = new OpenAIClient();
await foreach (var fineTuneEvent in api.FineTuningEndpoint.StreamFineTuneEventsEnumerableAsync(fineTuneJob))
{
    Console.WriteLine($"  {fineTuneEvent.CreatedAt} [{fineTuneEvent.Level}] {fineTuneEvent.Message}");
}
```

### [Moderations](https://beta.openai.com/docs/api-reference/moderations)

Given a input text, outputs if the model classifies it as violating OpenAI's content policy.

Related guide: [Moderations](https://beta.openai.com/docs/guides/moderation)

The Moderations API can be accessed via `OpenAIClient.ModerationsEndpoint`

#### [Create Moderation](https://beta.openai.com/docs/api-reference/moderations/create)

Classifies if text violates OpenAI's Content Policy.

```csharp
var api = new OpenAIClient();
var response = await api.ModerationsEndpoint.GetModerationAsync("I want to kill them.");
Assert.IsTrue(response);
```

---

## License

![CC-0 Public Domain](https://licensebuttons.net/p/zero/1.0/88x31.png)

This library is licensed CC-0, in the public domain.  You can use it for whatever you want, publicly or privately, without worrying about permission or licensing or whatever.  It's just a wrapper around the OpenAI API, so you still need to get access to OpenAI from them directly.  I am not affiliated with OpenAI and this library is not endorsed by them, I just have beta access and wanted to make a C# library to access it more easily.  Hopefully others find this useful as well.  Feel free to open a PR if there's anything you want to contribute.
