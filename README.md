# OpenAI-DotNet

[![Discord](https://img.shields.io/discord/855294214065487932.svg?label=&logo=discord&logoColor=ffffff&color=7389D8&labelColor=6A7EC2)](https://discord.gg/xQgMW9ufN4)
[![NuGet version (OpenAI-DotNet)](https://img.shields.io/nuget/v/OpenAI-DotNet.svg?label=OpenAI-DotNet&logo=nuget)](https://www.nuget.org/packages/OpenAI-DotNet/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/OpenAI-DotNet)](https://www.nuget.org/packages/OpenAI-DotNet/)
[![NuGet version (OpenAI-DotNet-Proxy)](https://img.shields.io/nuget/v/OpenAI-DotNet-Proxy.svg?label=OpenAI-DotNet-Proxy&logo=nuget)](https://www.nuget.org/packages/OpenAI-DotNet-Proxy/)
[![Nuget Publish](https://github.com/RageAgainstThePixel/OpenAI-DotNet/actions/workflows/Publish-Nuget.yml/badge.svg)](https://github.com/RageAgainstThePixel/OpenAI-DotNet/actions/workflows/Publish-Nuget.yml)

A simple C# .NET client library for [OpenAI](https://openai.com/) to use though their RESTful API.
Independently developed, this is not an official library and I am not affiliated with OpenAI.
An OpenAI API account is required.

Originally Forked from [OpenAI-API-dotnet](https://github.com/OkGoDoIt/OpenAI-API-dotnet).
More context [on Roger Pincombe's blog](https://rogerpincombe.com/openai-dotnet-api).

## Requirements

- This library targets .NET 8.0 and above.
- It should work across console apps, winforms, wpf, asp.net, etc.
- It should also work across Windows, Linux, and Mac.

## Getting started

### Install from NuGet

Install package [`OpenAI-DotNet` from Nuget](https://www.nuget.org/packages/OpenAI-DotNet/).  Here's how via command line:

powershell:

```terminal
Install-Package OpenAI-DotNet
```

dotnet:

```terminal
dotnet add package OpenAI-DotNet
```

> Looking to [use OpenAI-DotNet in the Unity Game Engine](https://github.com/RageAgainstThePixel/com.openai.unity)? Check out our unity package on OpenUPM:
>
>[![openupm](https://img.shields.io/npm/v/com.openai.unity?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.openai.unity/)

## [Documentation](https://rageagainstthepixel.github.io/OpenAI-DotNet)

> Check out our new api docs!

<https://rageagainstthepixel.github.io/OpenAI-DotNet>

### Table of Contents

- [Authentication](#authentication)
- [Azure OpenAI](#azure-openai)
  - [Azure Active Directory Authentication](#azure-active-directory-authentication)
- [OpenAI API Proxy](#openai-api-proxy)
- [Models](#models)
  - [List Models](#list-models)
  - [Retrieve Models](#retrieve-model)
  - [Delete Fine Tuned Model](#delete-fine-tuned-model)
- [Responses](#responses)
  - [Create Response](#create-response)
    - [Simple Text Response](#simple-response-with-text)
    - [Streaming Response with Function Calling](#streaming-response-with-function-calling)
  - [Get Response](#get-response)
  - [List Input Items](#list-input-items)
  - [Cancel Response](#cancel-response)
  - [Delete Response](#delete-response)
- [Conversations](#conversations)
  - [Create Conversation](#create-conversation)
  - [Retrieve Conversation](#retrieve-conversation)
  - [Update Conversation](#update-conversation)
  - [Delete Conversation](#delete-conversation)
  - [List Conversation Items](#list-conversation-items)
  - [Create Conversation Item](#create-conversation-item)
  - [Retrieve Conversation Item](#retrieve-conversation-item)
  - [Delete Conversation Item](#delete-conversation-item)
- [Realtime](#realtime)
  - [Create Realtime Session](#create-realtime-session)
  - [Client Events](#client-events)
    - [Sending Client Events](#sending-client-events)
  - [Server Events](#server-events)
    - [Receiving Server Events](#receiving-server-events)
- [Assistants](#assistants)
  - [List Assistants](#list-assistants)
  - [Create Assistant](#create-assistant)
  - [Retrieve Assistant](#retrieve-assistant)
  - [Modify Assistant](#modify-assistant)
  - [Delete Assistant](#delete-assistant)
  - [Assistant Streaming](#assistant-streaming)
  - [Threads](#threads)
    - [Create Thread](#create-thread)
    - [Create Thread and Run](#create-thread-and-run)
      - [Streaming](#create-thread-and-run-streaming)
    - [Retrieve Thread](#retrieve-thread)
    - [Modify Thread](#modify-thread)
    - [Delete Thread](#delete-thread)
    - [Thread Messages](#thread-messages)
      - [List Messages](#list-thread-messages)
      - [Create Message](#create-thread-message)
      - [Retrieve Message](#retrieve-thread-message)
      - [Modify Message](#modify-thread-message)
    - [Thread Runs](#thread-runs)
      - [List Runs](#list-thread-runs)
      - [Create Run](#create-thread-run)
        - [Streaming](#create-thread-run-streaming)
      - [Retrieve Run](#retrieve-thread-run)
      - [Modify Run](#modify-thread-run)
      - [Submit Tool Outputs to Run](#thread-submit-tool-outputs-to-run)
      - [Structured Outputs](#thread-structured-outputs)
      - [List Run Steps](#list-thread-run-steps)
      - [Retrieve Run Step](#retrieve-thread-run-step)
      - [Cancel Run](#cancel-thread-run)
  - [Vector Stores](#vector-stores)
    - [List Vector Stores](#list-vector-stores)
    - [Create Vector Store](#create-vector-store)
    - [Retrieve Vector Store](#retrieve-vector-store)
    - [Modify Vector Store](#modify-vector-store)
    - [Delete Vector Store](#delete-vector-store)
    - [Vector Store Files](#vector-store-files)
      - [List Vector Store Files](#list-vector-store-files)
      - [Create Vector Store File](#create-vector-store-file)
      - [Retrieve Vector Store File](#retrieve-vector-store-file)
      - [Delete Vector Store File](#delete-vector-store-file)
    - [Vector Store File Batches](#vector-store-file-batches)
      - [Create Vector Store File Batch](#create-vector-store-file-batch)
      - [Retrieve Vector Store File Batch](#retrieve-vector-store-file-batch)
      - [List Files In Vector Store Batch](#list-files-in-vector-store-batch)
      - [Cancel Vector Store File Batch](#cancel-vector-store-file-batch)
- [Chat](#chat)
  - [Chat Completions](#chat-completions)
  - [Streaming](#chat-streaming)
  - [Tools](#chat-tools)
  - [Vision](#chat-vision)
  - [Audio](#chat-audio)
  - [Structured Outputs](#chat-structured-outputs)
  - [Json Mode](#chat-json-mode)
- [Audio](#audio)
  - [Create Speech](#create-speech)
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
  - [Retrieve File](#retrieve-file-info)
  - [Download File Content](#download-file-content)
- [Fine Tuning](#fine-tuning)
  - [Create Fine Tune Job](#create-fine-tune-job)
  - [List Fine Tune Jobs](#list-fine-tune-jobs)
  - [Retrieve Fine Tune Job Info](#retrieve-fine-tune-job-info)
  - [Cancel Fine Tune Job](#cancel-fine-tune-job)
  - [List Fine Tune Job Events](#list-fine-tune-job-events)
- [Batches](#batches)
  - [List Batches](#list-batches)
  - [Create Batch](#create-batch)
  - [Retrieve Batch](#retrieve-batch)
  - [Cancel Batch](#cancel-batch)
- [Embeddings](#embeddings)
  - [Create Embedding](#create-embeddings)
- [Moderations](#moderations)
  - [Create Moderation](#create-moderation)

### [Authentication](https://platform.openai.com/docs/api-reference/authentication)

There are 3 ways to provide your API keys, in order of precedence:

> [!WARNING]
> We recommended using the environment variables to load the API key instead of having it hard coded in your source. It is not recommended use this method in production, but only for accepting user credentials, local testing and quick start scenarios.

1. [Pass keys directly with constructor](#pass-keys-directly-with-constructor) :warning:
2. [Load key from configuration file](#load-key-from-configuration-file)
3. [Use System Environment Variables](#use-system-environment-variables)

#### Pass keys directly with constructor

> [!WARNING]
> We recommended using the environment variables to load the API key instead of having it hard coded in your source. It is not recommended use this method in production, but only for accepting user credentials, local testing and quick start scenarios.

```csharp
using var api = new OpenAIClient("sk-apiKey");
```

Or create a `OpenAIAuthentication` object manually

```csharp
using var api = new OpenAIClient(new OpenAIAuthentication("sk-apiKey", "org-yourOrganizationId", "proj_yourProjectId"));
```

#### Load key from configuration file

Attempts to load api keys from a configuration file, by default `.openai` in the current directory, optionally traversing up the directory tree or in the user's home directory.

To create a configuration file, create a new text file named `.openai` and containing the line:

> [!NOTE]
> Organization and project id entries are optional.

##### Json format

```json
{
  "apiKey": "sk-aaaabbbbbccccddddd",
  "organizationId": "org-yourOrganizationId",
  "projectId": "proj_yourProjectId"
}
```

##### Deprecated format

```shell
OPENAI_API_KEY=sk-aaaabbbbbccccddddd
OPENAI_ORGANIZATION_ID=org-yourOrganizationId
OPENAI_PROJECT_ID=proj_yourProjectId
```

You can also load the configuration file directly with known path by calling static methods in `OpenAIAuthentication`:

- Loads the default `.openai` config in the specified directory:

```csharp
using var api = new OpenAIClient(OpenAIAuthentication.LoadFromDirectory("path/to/your/directory"));
```

- Loads the configuration file from a specific path. File does not need to be named `.openai` as long as it conforms to the json format:

```csharp
using var api = new OpenAIClient(OpenAIAuthentication.LoadFromPath("path/to/your/file.json"));
```

#### Use System Environment Variables

Use your system's environment variables specify an api key and organization to use.

- Use `OPENAI_API_KEY` for your api key.
- Use `OPENAI_ORGANIZATION_ID` to specify an organization.
- Use `OPENAI_PROJECT_ID` to specify a project.

```csharp
using var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnvironment());
```

### Handling OpenAIClient and HttpClient Lifecycle

`OpenAIClient` implements `IDisposable` to manage the lifecycle of the resources it uses, including `HttpClient`. When you initialize `OpenAIClient`, it will create an internal `HttpClient` instance if one is not provided. This internal `HttpClient` is disposed of when `OpenAIClient` is disposed of. If you provide an external `HttpClient` instance to `OpenAIClient`, you are responsible for managing its disposal.

- If `OpenAIClient` creates its own `HttpClient`, it will also take care of disposing it when you dispose `OpenAIClient`.
- If an external `HttpClient` is passed to `OpenAIClient`, it will not be disposed of by `OpenAIClient`. You must manage the disposal of the `HttpClient` yourself.

Please ensure to appropriately dispose of `OpenAIClient` to release resources timely and to prevent any potential memory or resource leaks in your application.

Typical usage with an internal `HttpClient`:

```csharp
using var api = new OpenAIClient();
```

Custom `HttpClient` (which you must dispose of yourself):

```csharp
using var customHttpClient = new HttpClient();
// set custom http client properties here
var api = new OpenAIClient(client: customHttpClient);
```

### [Azure OpenAI](https://learn.microsoft.com/en-us/azure/cognitive-services/openai)

You can also choose to use Microsoft's Azure OpenAI deployments as well.

You can find the required information in the Azure Playground by clicking the `View Code` button and view a URL like this:

```markdown
https://{your-resource-name}.openai.azure.com/openai/deployments/{deployment-id}/chat/completions?api-version={api-version}
```

- `your-resource-name` The name of your Azure OpenAI Resource.
- `deployment-id` The deployment name you chose when you deployed the model.
- `api-version` The API version to use for this operation. This follows the YYYY-MM-DD format.

To setup the client to use your deployment, you'll need to pass in `OpenAISettings` into the client constructor.

```csharp
var auth = new OpenAIAuthentication("sk-apiKey");
var settings = new OpenAISettings(resourceName: "your-resource-name", deploymentId: "deployment-id", apiVersion: "api-version");
using var api = new OpenAIClient(auth, settings);
```

#### [Azure Active Directory Authentication](https://learn.microsoft.com/en-us/azure/cognitive-services/openai/reference#authentication)

[Authenticate with MSAL](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet) as usual and get access token, then use the access token when creating your `OpenAIAuthentication`. Then be sure to set useAzureActiveDirectory to true when creating your `OpenAISettings`.

[Tutorial: Desktop app that calls web APIs: Acquire a token](https://learn.microsoft.com/en-us/azure/active-directory/develop/scenario-desktop-acquire-token?tabs=dotnet)

```csharp
// get your access token using any of the MSAL methods
var accessToken = result.AccessToken;
var auth = new OpenAIAuthentication(accessToken);
var settings = new OpenAISettings(resourceName: "your-resource", deploymentId: "deployment-id", apiVersion: "api-version", useActiveDirectoryAuthentication: true);
using var api = new OpenAIClient(auth, settings);
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
4. Create a new `OpenAISettings` object and specify the domain where your intermediate API is located.
5. Pass your new `auth` and `settings` objects to the `OpenAIClient` constructor when you create the client instance.

Here's an example of how to set up the front end:

```csharp
var authToken = await LoginAsync();
var auth = new OpenAIAuthentication($"sess-{authToken}");
var settings = new OpenAISettings(domain: "api.your-custom-domain.com");
using var api = new OpenAIClient(auth, settings);
```

This setup allows your front end application to securely communicate with your backend that will be using the OpenAI-DotNet-Proxy, which then forwards requests to the OpenAI API. This ensures that your OpenAI API keys and other sensitive information remain secure throughout the process.

#### Back End Example

In this example, we demonstrate how to set up and use `OpenAIProxy` in a new ASP.NET Core web app. The proxy server will handle authentication and forward requests to the OpenAI API, ensuring that your API keys and other sensitive information remain secure.

1. Create a new [ASP.NET Core minimal web API](https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-6.0) project.
2. Add the OpenAI-DotNet nuget package to your project.
    - Powershell install: `Install-Package OpenAI-DotNet-Proxy`
    - Dotnet install: `dotnet add package OpenAI-DotNet-Proxy`
    - Manually editing .csproj: `<PackageReference Include="OpenAI-DotNet-Proxy" />`
3. Create a new class that inherits from `AbstractAuthenticationFilter` and override the `ValidateAuthentication` method. This will implement the `IAuthenticationFilter` that you will use to check user session token against your internal server.
4. In `Program.cs`, create a new proxy web application by calling `OpenAIProxy.CreateWebApplication` method, passing your custom `AuthenticationFilter` as a type argument.
5. Create `OpenAIAuthentication` and `OpenAISettings` as you would normally with your API keys, org id, or Azure settings.

```csharp
public partial class Program
{
    private class AuthenticationFilter : AbstractAuthenticationFilter
    {
        public override async Task ValidateAuthenticationAsync(IHeaderDictionary request)
        {
            await Task.CompletedTask; // remote resource call to verify token

            // You will need to implement your own class to properly test
            // custom issued tokens you've setup for your end users.
            if (!request.Authorization.ToString().Contains(TestUserToken))
            {
                throw new AuthenticationException("User is not authorized");
            }
        }
    }

    public static void Main(string[] args)
    {
        var auth = OpenAIAuthentication.LoadFromEnvironment();
        var settings = new OpenAISettings(/* your custom settings if using Azure OpenAI */);
        using var openAIClient = new OpenAIClient(auth, settings);
        OpenAIProxy.CreateWebApplication<AuthenticationFilter>(args, openAIClient).Run();
    }
}
```

Once you have set up your proxy server, your end users can now make authenticated requests to your proxy api instead of directly to the OpenAI API. The proxy server will handle authentication and forward requests to the OpenAI API, ensuring that your API keys and other sensitive information remain secure.

---

### [Models](https://platform.openai.com/docs/api-reference/models)

List and describe the various models available in the API. You can refer to the [Models documentation](https://platform.openai.com/docs/models) to understand what models are available and the differences between them.

Also checkout [model endpoint compatibility](https://platform.openai.com/docs/models/model-endpoint-compatibility) to understand which models work with which endpoints.

To specify a custom model not pre-defined in this library:

```csharp
var model = new Model("model-id");
```

The Models API is accessed via `OpenAIClient.ModelsEndpoint`

#### [List models](https://platform.openai.com/docs/api-reference/models/list)

Lists the currently available models, and provides basic information about each one such as the owner and availability.

```csharp
using var api = new OpenAIClient();
var models = await api.ModelsEndpoint.GetModelsAsync();

foreach (var model in models)
{
    Console.WriteLine(model.ToString());
}
```

#### [Retrieve model](https://platform.openai.com/docs/api-reference/models/retrieve)

Retrieves a model instance, providing basic information about the model such as the owner and permissions.

```csharp
using var api = new OpenAIClient();
var model = await api.ModelsEndpoint.GetModelDetailsAsync("gpt-4o");
Console.WriteLine(model.ToString());
```

#### [Delete Fine Tuned Model](https://platform.openai.com/docs/api-reference/fine-tunes/delete-model)

Delete a fine-tuned model. You must have the Owner role in your organization.

```csharp
using var api = new OpenAIClient();
var isDeleted = await api.ModelsEndpoint.DeleteFineTuneModelAsync("your-fine-tuned-model");
Assert.IsTrue(isDeleted);
```

---

### [Responses](https://platform.openai.com/docs/api-reference/responses)

OpenAI's most advanced interface for generating model responses. Supports text and image inputs, and text outputs. Create stateful interactions with the model, using the output of previous responses as input. Extend the model's capabilities with built-in tools for file search, web search, computer use, and more. Allow the model access to external systems and data using function calling.

- Related Guides:
  - [QuickStart](https://platform.openai.com/docs/quickstart?api-mode=responses)
  - [Text Inputs and Outputs](https://platform.openai.com/docs/guides/text?api-mode=responses)
  - [Image Inputs](https://platform.openai.com/docs/guides/images?api-mode=responses)
  - [Structured Outputs](https://platform.openai.com/docs/guides/structured-outputs?api-mode=responses)
  - [Conversation State](https://platform.openai.com/docs/guides/conversation-state?api-mode=responses)
  - [Extend the model with tools](https://platform.openai.com/docs/guides/tools?api-mode=responses)

The Response API is accessed via `OpenAIClient.ResponsesEndpoint`

#### [Create Response](https://platform.openai.com/docs/api-reference/responses/create)

Creates a model response. Provide text or image inputs to generate text or JSON outputs. Have the model call your own custom code or use built-in tools like web search or file search to use your own data as input for the model's response.

##### Simple Response With Text

```csharp
var api = new OpenAIClient();
var response = await api.ResponsesEndpoint.CreateModelResponseAsync("Tell me a three sentence bedtime story about a unicorn.");
var responseItem = response.Output.LastOrDefault();
Console.WriteLine($"{messageItem.Role}:{textContent.Text}");
response.PrintUsage();
```

##### Streaming Response with Function Calling

```csharp
var api = new OpenAIClient();
var conversation = new List<IResponseItem>
{
    new Message(Role.System, "You are a helpful assistant."),
    new Message(Role.User, "What time is it?"),
};
var tools = new List<Tool>
{
    Tool.GetOrCreateTool(typeof(DateTimeUtility), nameof(DateTimeUtility.GetDateTime))
};
var request = new CreateResponseRequest(conversation, Model.GPT4_1_Nano, tools: tools);

async Task StreamCallback(string @event, IServerSentEvent sseEvent)
{
    switch (sseEvent)
    {
        case Message messageItem:
            conversation.Add(messageItem);
            break;
        case FunctionToolCall functionToolCall:
            conversation.Add(functionToolCall);
            var output = await functionToolCall.InvokeFunctionAsync();
            conversation.Add(output);
            await api.ResponsesEndpoint.CreateModelResponseAsync(new(conversation, Model.GPT4_1_Nano, tools: tools, toolChoice: "none"), StreamCallback);
            break;
    }
}

var response = await api.ResponsesEndpoint.CreateModelResponseAsync(request, StreamCallback);
var responseItem = response.Output.LastOrDefault();
Console.WriteLine($"{responseItem.Role}: {responseItem}");
response.PrintUsage();
```

#### [Get Response](https://platform.openai.com/docs/api-reference/responses/get)

Retrieves a model response with the given ID.

```csharp
var api = new OpenAIClient();
var response = await api.ResponsesEndpoint.GetModelResponseAsync("response-id");
Console.WriteLine(response.ToString());
```

#### [List Input Items](https://platform.openai.com/docs/api-reference/responses/input-items)

Returns a list of input items for a given response.

```csharp
var api = new OpenAIClient();
var responseInputItems = await api.ResponsesEndpoint.ListInputItemsAsync("response-id");
foreach (var item in responseInputItems)
{
    Console.WriteLine(item.ToJsonString());
}
```

#### [Cancel Response](https://platform.openai.com/docs/api-reference/responses/cancel)

Cancels a model response with the given ID.

> [!NOTE]
> Only responses created with the background parameter set to true can be cancelled.

```csharp
var api = new OpenAIClient();
var isCancelled = await api.ResponsesEndpoint.CancelModelResponseAsync("response-id");
Assert.IsTrue(isCancelled);
```

#### [Delete Response](https://platform.openai.com/docs/api-reference/responses/delete)

Deletes a model response with the given ID.

```csharp
var api = new OpenAIClient();
var isDeleted = await api.ResponsesEndpoint.DeleteModelResponseAsync("response-id");
Assert.IsTrue(isDeleted);
```

---

### [Conversations](https://platform.openai.com/docs/api-reference/conversations)

Create and manage conversations to store and retrieve conversation state across Response API calls.

The Conversations API is accessed via `OpenAIClient.ConversationsEndpoint`

#### [Create Conversation](https://platform.openai.com/docs/api-reference/conversations/create)

Create a conversation.

```csharp
var api = new OpenAIClient();
conversation = await api.ConversationsEndpoint.CreateConversationAsync(
    new CreateConversationRequest(new Message(Role.Developer, systemPrompt)));
Console.WriteLine(conversation.ToString());
// use the conversation object when creating responses.
var request = await api.ResponsesEndpoint.CreateResponseAsync(
    new CreateResponseRequest(textInput: "Hello!", conversationId: conversation, model: Model.GPT5_Nano));
var response = await openAI.ResponsesEndpoint.CreateModelResponseAsync(request);
var responseItem = response.Output.LastOrDefault();
Console.WriteLine($"{responseItem.Role}:{responseItem}");
response.PrintUsage();
```

#### [Retrieve Conversation](https://platform.openai.com/docs/api-reference/conversations/retrieve)

Get a conversation by id.

```csharp
var api = new OpenAIClient();
var conversation = await api.ConversationsEndpoint.GetConversationAsync("conversation-id");
Console.WriteLine(conversation.ToString());
```

#### [Update Conversation](https://platform.openai.com/docs/api-reference/conversations/update)

Update a conversation with custom metadata.

```csharp
var api = new OpenAIClient();
var metadata = new Dictionary<string, object>
{
    { "favorite_color", "blue" },
    { "favorite_food", "pizza" }
};
var updatedConversation = await api.ConversationsEndpoint.UpdateConversationAsync("conversation-id", metadata);
```

#### [Delete Conversation](https://platform.openai.com/docs/api-reference/conversations/delete)

Delete a conversation by id.

```csharp
var api = new OpenAIClient();
var isDeleted = await api.ConversationsEndpoint.DeleteConversationAsync("conversation-id");
Assert.IsTrue(isDeleted);
```

#### [List Conversation Items](https://platform.openai.com/docs/api-reference/conversations/list-items)

List all items for a conversation with the given ID.

```csharp
var api = new OpenAIClient();
var query = new ListQuery(limit: 10);
var items = await api.ConversationsEndpoint.ListConversationItemsAsync("conversation-id", query);

foreach (var item in items)
{
    Console.WriteLine(item.ToJsonString());
}
```

#### [Create Conversation Item](https://platform.openai.com/docs/api-reference/conversations/create-item)

Create a new conversation item for a conversation with the given ID.

```csharp
var api = new OpenAIClient();
var items = new List<IResponseItem>
{
    new Message(Role.User, "Hello!"),
    new Message(Role.Assistant, "Hi! How can I help you?")
}
var addedItems = await api.ConversationsEndpoint.CreateConversationItemsAsync("conversation-id", items);

foreach (var item in addedItems)
{
    Console.WriteLine(item.ToJsonString());
}
```

#### [Retrieve Conversation Item](https://platform.openai.com/docs/api-reference/conversations/retrieve-item)

Get a conversation item by id.

```csharp
var api = new OpenAIClient();
var item = await api.ConversationsEndpoint.GetConversationItemAsync("conversation-id", "item-id");
Console.WriteLine(item.ToJsonString());
```

#### [Delete Conversation Item](https://platform.openai.com/docs/api-reference/conversations/delete-item)

Delete a conversation item by id.

```csharp
var api = new OpenAIClient();
var isDeleted = await api.ConversationsEndpoint.DeleteConversationItemAsync("conversation-id", "item-id");
Assert.IsTrue(isDeleted);
```

---

### [Realtime](https://platform.openai.com/docs/api-reference/realtime)

> [!WARNING]
> Beta Feature. API subject to breaking changes.

- [Realtime Guide](https://platform.openai.com/docs/guides/realtime)

The Realtime API enables you to build low-latency, multi-modal conversational experiences. It currently supports text and audio as both input and output, as well as function calling.

The Assistants API is accessed via `OpenAIClient.RealtimeEndpoint`

#### Create Realtime Session

Here is a simple example of how to create a realtime session and to send and receive messages from the model.

```csharp
using var api = new OpenAIClient();
var cancellationTokenSource = new CancellationTokenSource();
var tools = new List<Tool>
{
    Tool.FromFunc("goodbye", () =>
    {
        cancellationTokenSource.Cancel();
        return "Goodbye!";
    })
};
var configuration = new SessionConfiguration(Model.GPT4oRealtime, tools: tools);
using var session = await api.RealtimeEndpoint.CreateSessionAsync(configuration);
var responseTask = session.ReceiveUpdatesAsync<IServerEvent>(ServerEvents, cancellationTokenSource.Token);
await session.SendAsync(new ConversationItemCreateRequest("Hello!"));
await session.SendAsync(new CreateResponseRequest());
await session.SendAsync(new InputAudioBufferAppendRequest(new ReadOnlyMemory<byte>(new byte[1024 * 4])), cancellationTokenSource.Token);
await session.SendAsync(new ConversationItemCreateRequest("GoodBye!"));
await session.SendAsync(new CreateResponseRequest());
await responseTask;

void ServerEvents(IServerEvent @event)
{
    switch (@event)
    {
        case ResponseAudioTranscriptResponse transcriptResponse:
            Console.WriteLine(transcriptResponse.ToString());
            break;
        case ResponseFunctionCallArgumentsResponse functionCallResponse:
            if (functionCallResponse.IsDone)
            {
                ToolCall toolCall = functionCallResponse;
                toolCall.InvokeFunction();
            }

            break;
    }
}
```

#### Client Events

The library implements `IClientEvent` interface for outgoing client sent events.

- [`UpdateSessionRequest`](https://platform.openai.com/docs/api-reference/realtime-client-events/session/update): Update the session with new session options.
- [`InputAudioBufferAppendRequest`](https://platform.openai.com/docs/api-reference/realtime-client-events/input-audio-buffer/append): Append audio to the input audio buffer. (Unlike made other client events, the server will not send a confirmation response to this event).
- [`InputAudioBufferCommitRequest`](https://platform.openai.com/docs/api-reference/realtime-client-events/input-audio-buffer/commit): Commit the input audio buffer. (When in Server VAD mode, the client does not need to send this event).
- [`InputAudioBufferClearRequest`](https://platform.openai.com/docs/api-reference/realtime-client-events/input-audio-buffer/clear): Clear the input audio buffer.
- [`ConversationItemCreateRequest`](https://platform.openai.com/docs/api-reference/realtime-client-events/conversation/item/create): Create a new conversation item. This is the main way to send user content to the model.
- [`ConversationItemTruncateRequest`](https://platform.openai.com/docs/api-reference/realtime-client-events/conversation/item/truncate): Send this event to truncate a previous assistant message’s audio.
- [`ConversationItemDeleteRequest`](https://platform.openai.com/docs/api-reference/realtime-client-events/conversation/item/delete): Delete a conversation item. This is useful when you want to remove a message from the conversation history.
- [`CreateResponseRequest`](https://platform.openai.com/docs/api-reference/realtime-client-events/response/create): Create a response from the model. Send this event after creating new conversation items or invoking tool calls. This will trigger the model to generate a response.
- [`ResponseCancelRequest`](https://platform.openai.com/docs/api-reference/realtime-client-events/response/cancel) -Send this event to cancel an in-progress response.

##### Sending Client Events

You can send client events at any time to the server by calling the `RealtimeSession.SendAsync` method on the session object. The send call will return a `IServerEvent` handle that best represents the appropriate response from the server for that event. This is useful if you want to handle server responses in a more granular way.

Ideally though, you may want to handle all server responses with [`RealtimeSession.ReceiveUpdatesAsync`](#receiving-server-events).

> [!NOTE]
> The server will not send a confirmation response to the `InputAudioBufferAppendRequest` event.

> [!IMPORTANT]
> You will also need to send `CreateResponseRequest` to trigger the model to generate a response.

```csharp
var serverEvent = await session.SendAsync(new ConversationItemCreateRequest("Hello!"));
Console.WriteLine(serverEvent.ToJsonString());
serverEvent = await session.SendAsync(new CreateResponseRequest());
Console.WriteLine(serverEvent.ToJsonString());
```

#### Server Events

The library implements `IServerEvent` interface for incoming server sent events.

- [`RealtimeEventError`](https://platform.openai.com/docs/api-reference/realtime-server-events/error): Returned when an error occurs, which could be a client problem or a server problem.
- [`SessionResponse`](https://platform.openai.com/docs/api-reference/realtime-server-events/session): Returned for both a `session.created` and `session.updated` event.
- [`RealtimeConversationResponse`](https://platform.openai.com/docs/api-reference/realtime-server-events/conversation/created): Returned when a new conversation item is created.
- [`ConversationItemCreatedResponse`](https://platform.openai.com/docs/api-reference/realtime-server-events/conversation/item/created): Returned when a new conversation item is created.
- [`ConversationItemInputAudioTranscriptionResponse`](https://platform.openai.com/docs/api-reference/realtime-server-events/conversation): Returned when the input audio transcription is completed or failed.
- [`ConversationItemTruncatedResponse`](https://platform.openai.com/docs/api-reference/realtime-server-events/conversation/item/truncated): Returned when a conversation item is truncated.
- [`ConversationItemDeletedResponse`](https://platform.openai.com/docs/api-reference/realtime-server-events/conversation/item/deleted): Returned when a conversation item is deleted.
- [`InputAudioBufferCommittedResponse`](https://platform.openai.com/docs/api-reference/realtime-server-events/input_audio_buffer/committed): Returned when an input audio buffer is committed, either by the client or automatically in server VAD mode.
- [`InputAudioBufferClearedResponse`](https://platform.openai.com/docs/api-reference/realtime-server-events/input_audio_buffer/cleared): Returned when an input audio buffer is cleared.
- [`InputAudioBufferStartedResponse`](https://platform.openai.com/docs/api-reference/realtime-server-events/input_audio_buffer/speech_started): Sent by the server when in server_vad mode to indicate that speech has been detected in the audio buffer. This can happen any time audio is added to the buffer (unless speech is already detected). The client may want to use this event to interrupt audio playback or provide visual feedback to the user.
- [`InputAudioBufferStoppedResponse`](https://platform.openai.com/docs/api-reference/realtime-server-events/input_audio_buffer/speech_stopped): Returned in server_vad mode when the server detects the end of speech in the audio buffer.
- [`RealtimeResponse`](https://platform.openai.com/docs/api-reference/realtime-server-events/response): Returned when a response is created or done.
- [`ResponseOutputItemResponse`](https://platform.openai.com/docs/api-reference/realtime-server-events/response/output_item): Returned when a response output item is added or done.
- [`ResponseContentPartResponse`](https://platform.openai.com/docs/api-reference/realtime-server-events/response/content_part): Returned when a response content part is added or done.
- [`ResponseTextResponse`](https://platform.openai.com/docs/api-reference/realtime-server-events/response/text): Returned when a response text is updated or done.
- [`ResponseAudioTranscriptResponse`](https://platform.openai.com/docs/api-reference/realtime-server-events/response/audio_transcript): Returned when a response audio transcript is updated or done.
- [`ResponseAudioResponse`](https://platform.openai.com/docs/api-reference/realtime-server-events/response/audio): Returned when a response audio is updated or done.
- [`ResponseFunctionCallArgumentsResponse`](https://platform.openai.com/docs/api-reference/realtime-server-events/response/function_call_arguments): Returned when a response function call arguments are updated or done.
- [`RateLimitsResponse`](https://platform.openai.com/docs/api-reference/realtime-server-events/rate_limits): Returned when rate limits are updated.

##### Receiving Server Events

To receive server events, you will need to call the `RealtimeSession.ReceiveUpdatesAsync` method on the session object. This method will return a `Task` or `IAsyncEnumerable<T>` that will complete when the session is closed or when the cancellation token is triggered. Ideally this method should be called once and runs for the duration of the session.

> [!NOTE]
> You can also get sent `IClientEvent` callbacks as well by using the `IRealtimeEvent` interface instead of `IServerEvent`.

```csharp
await foreach (var @event in session.ReceiveUpdatesAsync<IServerEvent>(cts.Token))
{
    switch (@event)
    {
        case RealtimeEventError error:
            // raised anytime an error occurs
            break;
        case SessionResponse sessionResponse:
            // raised when a session is created or updated
            break;
        case RealtimeConversationResponse conversationResponse:
            // raised when a new conversation is created
            break;
        case ConversationItemCreatedResponse conversationItemCreated:
            // raised when a new conversation item is created
            break;
        case ConversationItemInputAudioTranscriptionResponse conversationItemTranscription:
            // raised when the input audio transcription is completed or failed
            break;
        case ConversationItemTruncatedResponse conversationItemTruncated:
            // raised when a conversation item is truncated
            break;
        case ConversationItemDeletedResponse conversationItemDeleted:
            // raised when a conversation item is deleted
            break;
        case InputAudioBufferCommittedResponse committedResponse:
            // raised when an input audio buffer is committed
            break;
        case InputAudioBufferClearedResponse clearedResponse:
            // raised when an input audio buffer is cleared
            break;
        case InputAudioBufferStartedResponse startedResponse:
            // raised when speech is detected in the audio buffer
            break;
        case InputAudioBufferStoppedResponse stoppedResponse:
            // raised when speech stops in the audio buffer
            break;
        case RealtimeResponse realtimeResponse:
            // raised when a response is created or done
            break;
        case ResponseOutputItemResponse outputItemResponse:
            // raised when a response output item is added or done
            break;
        case ResponseContentPartResponse contentPartResponse:
            // raised when a response content part is added or done
            break;
        case ResponseTextResponse textResponse:
            // raised when a response text is updated or done
            break;
        case ResponseAudioTranscriptResponse transcriptResponse:
            // raised when a response audio transcript is updated or done
            break;
        case ResponseFunctionCallArgumentsResponse functionCallResponse:
            // raised when a response function call arguments are updated or done
            break;
        case RateLimitsResponse rateLimitsResponse:
            // raised when rate limits are updated
            break;
    }
}
```

---

### [Assistants](https://platform.openai.com/docs/api-reference/assistants)

> [!WARNING]
> Beta Feature. API subject to breaking changes.

Build assistants that can call models and use tools to perform tasks.

- [Assistants Guide](https://platform.openai.com/docs/assistants)
- [OpenAI Assistants Cookbook](https://github.com/openai/openai-cookbook/blob/main/examples/Assistants_API_overview_python.ipynb)

The Assistants API is accessed via `OpenAIClient.AssistantsEndpoint`

#### [List Assistants](https://platform.openai.com/docs/api-reference/assistants/listAssistants)

Returns a list of assistants.

```csharp
using var api = new OpenAIClient();
var assistantsList = await api.AssistantsEndpoint.ListAssistantsAsync();

foreach (var assistant in assistantsList.Items)
{
    Console.WriteLine($"{assistant} -> {assistant.CreatedAt}");
}
```

#### [Create Assistant](https://platform.openai.com/docs/api-reference/assistants/createAssistant)

Create an assistant with a model and instructions.

```csharp
using var api = new OpenAIClient();
var request = new CreateAssistantRequest(Model.GPT4o);
var assistant = await api.AssistantsEndpoint.CreateAssistantAsync(request);
```

#### [Retrieve Assistant](https://platform.openai.com/docs/api-reference/assistants/getAssistant)

Retrieves an assistant.

```csharp
using var api = new OpenAIClient();
var assistant = await api.AssistantsEndpoint.RetrieveAssistantAsync("assistant-id");
Console.WriteLine($"{assistant} -> {assistant.CreatedAt}");
```

#### [Modify Assistant](https://platform.openai.com/docs/api-reference/assistants/modifyAssistant)

Modifies an assistant.

```csharp
using var api = new OpenAIClient();
var createRequest = new CreateAssistantRequest(Model.GPT4_Turbo);
var assistant = await api.AssistantsEndpoint.CreateAssistantAsync(createRequest);
var modifyRequest = new CreateAssistantRequest(Model.GPT4o);
var modifiedAssistant = await api.AssistantsEndpoint.ModifyAssistantAsync(assistant.Id, modifyRequest);
// OR AssistantExtension for easier use!
var modifiedAssistantEx = await assistant.ModifyAsync(modifyRequest);
```

#### [Delete Assistant](https://platform.openai.com/docs/api-reference/assistants/deleteAssistant)

Delete an assistant.

```csharp
using var api = new OpenAIClient();
var isDeleted = await api.AssistantsEndpoint.DeleteAssistantAsync("assistant-id");
// OR AssistantExtension for easier use!
var isDeleted = await assistant.DeleteAsync();
Assert.IsTrue(isDeleted);
```

#### [Assistant Streaming](https://platform.openai.com/docs/api-reference/assistants-streaming)

> [!NOTE]
> Assistant stream events can be easily added to existing thread calls by passing `Func<IServerSentEvent, Task> streamEventHandler` callback to any existing method that supports streaming.

#### [Threads](https://platform.openai.com/docs/api-reference/threads)

Create Threads that [Assistants](#assistants) can interact with.

The Threads API is accessed via `OpenAIClient.ThreadsEndpoint`

##### [Create Thread](https://platform.openai.com/docs/api-reference/threads/createThread)

Create a thread.

```csharp
using var api = new OpenAIClient();
var thread = await api.ThreadsEndpoint.CreateThreadAsync();
Console.WriteLine($"Create thread {thread.Id} -> {thread.CreatedAt}");
```

##### [Create Thread and Run](https://platform.openai.com/docs/api-reference/runs/createThreadAndRun)

Create a thread and run it in one request.

> See also: [Thread Runs](#thread-runs)

```csharp
using var api = new OpenAIClient();
var assistant = await api.AssistantsEndpoint.CreateAssistantAsync(
    new CreateAssistantRequest(
        name: "Math Tutor",
        instructions: "You are a personal math tutor. Answer questions briefly, in a sentence or less.",
        model: Model.GPT4o));
var messages = new List<Message> { "I need to solve the equation `3x + 11 = 14`. Can you help me?" };
var threadRequest = new CreateThreadRequest(messages);
var run = await assistant.CreateThreadAndRunAsync(threadRequest);
Console.WriteLine($"Created thread and run: {run.ThreadId} -> {run.Id} -> {run.CreatedAt}");
```

###### Create Thread and Run Streaming

Create a thread and run it in one request while streaming events.

```csharp
using var api = new OpenAIClient();
var tools = new List<Tool>
{
    Tool.GetOrCreateTool(typeof(WeatherService), nameof(WeatherService.GetCurrentWeatherAsync))
};
var assistantRequest = new CreateAssistantRequest(tools: tools, instructions: "You are a helpful weather assistant. Use the appropriate unit based on geographical location.");
var assistant = await api.AssistantsEndpoint.CreateAssistantAsync(assistantRequest);
ThreadResponse thread = null;
async Task StreamEventHandler(IServerSentEvent streamEvent)
{
    switch (streamEvent)
    {
        case ThreadResponse threadResponse:
            thread = threadResponse;
            break;
        case RunResponse runResponse:
            if (runResponse.Status == RunStatus.RequiresAction)
            {
                var toolOutputs = await assistant.GetToolOutputsAsync(runResponse);

                foreach (var toolOutput in toolOutputs)
                {
                    Console.WriteLine($"Tool Output: {toolOutput}");
                }

                await runResponse.SubmitToolOutputsAsync(toolOutputs, StreamEventHandler);
            }
            break;
        default:
            Console.WriteLine(streamEvent.ToJsonString());
            break;
    }
}

var run = await assistant.CreateThreadAndRunAsync("I'm in Kuala-Lumpur, please tell me what's the temperature now?", StreamEventHandler);
run = await run.WaitForStatusChangeAsync();
var messages = await thread.ListMessagesAsync();
foreach (var response in messages.Items.Reverse())
{
    Console.WriteLine($"{response.Role}: {response.PrintContent()}");
}
```

##### [Retrieve Thread](https://platform.openai.com/docs/api-reference/threads/getThread)

Retrieves a thread.

```csharp
using var api = new OpenAIClient();
var thread = await api.ThreadsEndpoint.RetrieveThreadAsync("thread-id");
// OR if you simply wish to get the latest state of a thread
thread = await thread.UpdateAsync();
Console.WriteLine($"Retrieve thread {thread.Id} -> {thread.CreatedAt}");
```

##### [Modify Thread](https://platform.openai.com/docs/api-reference/threads/modifyThread)

Modifies a thread.

> [!NOTE]
> Only the metadata can be modified.

```csharp
using var api = new OpenAIClient();
var thread = await api.ThreadsEndpoint.CreateThreadAsync();
var metadata = new Dictionary<string, string>
{
    { "key", "custom thread metadata" }
}
thread = await api.ThreadsEndpoint.ModifyThreadAsync(thread.Id, metadata);
// OR use extension method for convenience!
thread = await thread.ModifyAsync(metadata);
Console.WriteLine($"Modify thread {thread.Id} -> {thread.Metadata["key"]}");
```

##### [Delete Thread](https://platform.openai.com/docs/api-reference/threads/deleteThread)

Delete a thread.

```csharp
using var api = new OpenAIClient();
var isDeleted = await api.ThreadsEndpoint.DeleteThreadAsync("thread-id");
// OR use extension method for convenience!
var isDeleted = await thread.DeleteAsync();
Assert.IsTrue(isDeleted);
```

##### [Thread Messages](https://platform.openai.com/docs/api-reference/messages)

Create messages within threads.

###### [List Thread Messages](https://platform.openai.com/docs/api-reference/messages/listMessages)

Returns a list of messages for a given thread.

```csharp
using var api = new OpenAIClient();
var messageList = await api.ThreadsEndpoint.ListMessagesAsync("thread-id");
// OR use extension method for convenience!
var messageList = await thread.ListMessagesAsync();

foreach (var message in messageList.Items)
{
    Console.WriteLine($"{message.Id}: {message.Role}: {message.PrintContent()}");
}
```

###### [Create Thread Message](https://platform.openai.com/docs/api-reference/messages/createMessage)

Create a message.

```csharp
using var api = new OpenAIClient();
var thread = await api.ThreadsEndpoint.CreateThreadAsync();
var request = new CreateMessageRequest("Hello world!");
var message = await api.ThreadsEndpoint.CreateMessageAsync(thread.Id, request);
// OR use extension method for convenience!
var message = await thread.CreateMessageAsync("Hello World!");
Console.WriteLine($"{message.Id}: {message.Role}: {message.PrintContent()}");
```

###### [Retrieve Thread Message](https://platform.openai.com/docs/api-reference/messages/getMessage)

Retrieve a message.

```csharp
using var api = new OpenAIClient();
var message = await api.ThreadsEndpoint.RetrieveMessageAsync("thread-id", "message-id");
// OR use extension methods for convenience!
var message = await thread.RetrieveMessageAsync("message-id");
var message = await message.UpdateAsync();
Console.WriteLine($"{message.Id}: {message.Role}: {message.PrintContent()}");
```

###### [Modify Thread Message](https://platform.openai.com/docs/api-reference/messages/modifyMessage)

Modify a message.

> [!NOTE]
> Only the metadata can be modified.

```csharp
using var api = new OpenAIClient();
var metadata = new Dictionary<string, string>
{
    { "key", "custom message metadata" }
};
var message = await api.ThreadsEndpoint.ModifyMessageAsync("thread-id", "message-id", metadata);
// OR use extension method for convenience!
var message = await message.ModifyAsync(metadata);
Console.WriteLine($"Modify message metadata: {message.Id} -> {message.Metadata["key"]}");
```

##### [Thread Runs](https://platform.openai.com/docs/api-reference/runs)

Represents an execution run on a thread.

###### [List Thread Runs](https://platform.openai.com/docs/api-reference/runs/listRuns)

Returns a list of runs belonging to a thread.

```csharp
using var api = new OpenAIClient();
var runList = await api.ThreadsEndpoint.ListRunsAsync("thread-id");
// OR use extension method for convenience!
var runList = await thread.ListRunsAsync();

foreach (var run in runList.Items)
{
    Console.WriteLine($"[{run.Id}] {run.Status} | {run.CreatedAt}");
}
```

###### [Create Thread Run](https://platform.openai.com/docs/api-reference/runs/createRun)

Create a run.

```csharp
using var api = new OpenAIClient();
var assistant = await api.AssistantsEndpoint.CreateAssistantAsync(
    new CreateAssistantRequest(
        name: "Math Tutor",
        instructions: "You are a personal math tutor. Answer questions briefly, in a sentence or less.",
        model: Model.GPT4o));
var thread = await api.ThreadsEndpoint.CreateThreadAsync();
var message = await thread.CreateMessageAsync("I need to solve the equation `3x + 11 = 14`. Can you help me?");
var run = await thread.CreateRunAsync(assistant);
Console.WriteLine($"[{run.Id}] {run.Status} | {run.CreatedAt}");
```

###### Create Thread Run Streaming

Create a run and stream the events.

```csharp
using var api = new OpenAIClient();
var assistant = await api.AssistantsEndpoint.CreateAssistantAsync(
    new CreateAssistantRequest(
        name: "Math Tutor",
        instructions: "You are a personal math tutor. Answer questions briefly, in a sentence or less. Your responses should be formatted in JSON.",
        model: Model.GPT4o,
        responseFormat: ChatResponseFormat.Json));
var thread = await api.ThreadsEndpoint.CreateThreadAsync();
var message = await thread.CreateMessageAsync("I need to solve the equation `3x + 11 = 14`. Can you help me?");
var run = await thread.CreateRunAsync(assistant, async streamEvent =>
{
    Console.WriteLine(streamEvent.ToJsonString());
    await Task.CompletedTask;
});
var messages = await thread.ListMessagesAsync();

foreach (var response in messages.Items.Reverse())
{
    Console.WriteLine($"{response.Role}: {response.PrintContent()}");
}
```

###### [Retrieve Thread Run](https://platform.openai.com/docs/api-reference/runs/getRun)

Retrieves a run.

```csharp
using var api = new OpenAIClient();
var run = await api.ThreadsEndpoint.RetrieveRunAsync("thread-id", "run-id");
// OR use extension method for convenience!
var run = await thread.RetrieveRunAsync("run-id");
var run = await run.UpdateAsync();
Console.WriteLine($"[{run.Id}] {run.Status} | {run.CreatedAt}");
```

###### [Modify Thread Run](https://platform.openai.com/docs/api-reference/runs/modifyRun)

Modifies a run.

> [!NOTE]
> Only the metadata can be modified.

```csharp
using var api = new OpenAIClient();
var metadata = new Dictionary<string, string>
{
    { "key", "custom run metadata" }
};
var run = await api.ThreadsEndpoint.ModifyRunAsync("thread-id", "run-id", metadata);
// OR use extension method for convenience!
var run = await run.ModifyAsync(metadata);
Console.WriteLine($"Modify run {run.Id} -> {run.Metadata["key"]}");
```

###### [Thread Submit Tool Outputs to Run](https://platform.openai.com/docs/api-reference/runs/submitToolOutputs)

When a run has the status: `requires_action` and `required_action.type` is `submit_tool_outputs`, this endpoint can be used to submit the outputs from the tool calls once they're all completed.
All outputs must be submitted in a single request.

> [!NOTE]
> See [Create Thread and Run Streaming](#create-thread-and-run-streaming) example on how to stream tool output events.

```csharp
using var api = new OpenAIClient();
var tools = new List<Tool>
{
    // Use a predefined tool
    Tool.Retrieval, Tool.CodeInterpreter,
    // Or create a tool from a type and the name of the method you want to use for function calling
    Tool.GetOrCreateTool(typeof(WeatherService), nameof(WeatherService.GetCurrentWeatherAsync)),
    // Pass in an instance of an object to call a method on it
    Tool.GetOrCreateTool(api.ImagesEndPoint, nameof(ImagesEndpoint.GenerateImageAsync)),
    // Define func<,> callbacks
    Tool.FromFunc("name_of_func", () => { /* callback function */ }),
    Tool.FromFunc<T1,T2,TResult>("func_with_multiple_params", (t1, t2) => { /* logic that calculates return value */ return tResult; })
};
var assistantRequest = new CreateAssistantRequest(tools: tools, instructions: "You are a helpful weather assistant. Use the appropriate unit based on geographical location.");
var testAssistant = await api.AssistantsEndpoint.CreateAssistantAsync(assistantRequest);
var run = await testAssistant.CreateThreadAndRunAsync("I'm in Kuala-Lumpur, please tell me what's the temperature now?");
// waiting while run is Queued and InProgress
run = await run.WaitForStatusChangeAsync();

// Invoke all of the tool call functions and return the tool outputs.
var toolOutputs = await testAssistant.GetToolOutputsAsync(run.RequiredAction.SubmitToolOutputs.ToolCalls);

foreach (var toolOutput in toolOutputs)
{
    Console.WriteLine($"tool call output: {toolOutput.Output}");
}
// submit the tool outputs
run = await run.SubmitToolOutputsAsync(toolOutputs);
// waiting while run in Queued and InProgress
run = await run.WaitForStatusChangeAsync();
var messages = await run.ListMessagesAsync();

foreach (var message in messages.Items.OrderBy(response => response.CreatedAt))
{
    Console.WriteLine($"{message.Role}: {message.PrintContent()}");
}
```

##### [Thread Structured Outputs](https://platform.openai.com/docs/guides/structured-outputs)

Structured Outputs is the evolution of JSON mode. While both ensure valid JSON is produced, only Structured Outputs ensure schema adherence.

> [!IMPORTANT]
>
> - When using JSON mode, always instruct the model to produce JSON via some message in the conversation, for example via your system message. If you don't include an explicit instruction to generate JSON, the model may generate an unending stream of whitespace and the request may run continually until it reaches the token limit. To help ensure you don't forget, the API will throw an error if the string "JSON" does not appear somewhere in the context.
> - The JSON in the message the model returns may be partial (i.e. cut off) if `finish_reason` is length, which indicates the generation exceeded max_tokens or the conversation exceeded the token limit. To guard against this, check `finish_reason` before parsing the response.

First define the structure of your responses. These will be used as your schema.
These are the objects you'll deserialize to, so be sure to use standard Json object models.

```csharp
public class MathResponse
{
    [JsonInclude]
    [JsonPropertyName("steps")]
    public IReadOnlyList<MathStep> Steps { get; private set; }

    [JsonInclude]
    [JsonPropertyName("final_answer")]
    public string FinalAnswer { get; private set; }
}

public class MathStep
{
    [JsonInclude]
    [JsonPropertyName("explanation")]
    public string Explanation { get; private set; }

    [JsonInclude]
    [JsonPropertyName("output")]
    public string Output { get; private set; }
}
```

To use, simply specify the `MathResponse` type as a generic constraint in either `CreateAssistantAsync`, `CreateRunAsync`, or `CreateThreadAndRunAsync`.

```csharp
using var api = new OpenAIClient();
var assistant = await api.AssistantsEndpoint.CreateAssistantAsync<MathResponse>(
    new CreateAssistantRequest(
        name: "Math Tutor",
        instructions: "You are a helpful math tutor. Guide the user through the solution step by step.",
        model: "gpt-4o-2024-08-06"));
ThreadResponse thread = null;

try
{
    async Task StreamEventHandler(IServerSentEvent @event)
    {
        try
        {
            switch (@event)
            {
                case MessageResponse message:
                    if (message.Status != MessageStatus.Completed)
                    {
                        Console.WriteLine(@event.ToJsonString());
                        break;
                    }

                    var mathResponse = message.FromSchema<MathResponse>();

                    for (var i = 0; i < mathResponse.Steps.Count; i++)
                    {
                        var step = mathResponse.Steps[i];
                        Console.WriteLine($"Step {i}: {step.Explanation}");
                        Console.WriteLine($"Result: {step.Output}");
                    }

                    Console.WriteLine($"Final Answer: {mathResponse.FinalAnswer}");
                    break;
                default:
                    Console.WriteLine(@event.ToJsonString());
                    break;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        await Task.CompletedTask;
    }

    var run = await assistant.CreateThreadAndRunAsync("how can I solve 8x + 7 = -23", StreamEventHandler);
    thread = await run.GetThreadAsync();
    run = await run.WaitForStatusChangeAsync();
    Console.WriteLine($"Created thread and run: {run.ThreadId} -> {run.Id} -> {run.CreatedAt}");
    var messages = await thread.ListMessagesAsync();

    foreach (var response in messages.Items.OrderBy(response => response.CreatedAt))
    {
        Console.WriteLine($"{response.Role}: {response.PrintContent()}");
    }
}
finally
{
    await assistant.DeleteAsync(deleteToolResources: thread == null);

    if (thread != null)
    {
        var isDeleted = await thread.DeleteAsync(deleteToolResources: true);
    }
}
```

You can also manually create json schema json string as well, but you will be responsible for deserializing your response data:

```csharp
using var api = new OpenAIClient();
var mathSchema = new JsonSchema("math_response", @"
{
  ""type"": ""object"",
  ""properties"": {
    ""steps"": {
      ""type"": ""array"",
      ""items"": {
        ""type"": ""object"",
        ""properties"": {
          ""explanation"": {
            ""type"": ""string""
          },
          ""output"": {
            ""type"": ""string""
          }
        },
        ""required"": [
          ""explanation"",
          ""output""
        ],
        ""additionalProperties"": false
      }
    },
    ""final_answer"": {
      ""type"": ""string""
    }
  },
  ""required"": [
    ""steps"",
    ""final_answer""
  ],
  ""additionalProperties"": false
}");
var assistant = await api.AssistantsEndpoint.CreateAssistantAsync(
    new CreateAssistantRequest(
        name: "Math Tutor",
        instructions: "You are a helpful math tutor. Guide the user through the solution step by step.",
        model: "gpt-4o-2024-08-06",
        jsonSchema: mathSchema));
ThreadResponse thread = null;

try
{
    var run = await assistant.CreateThreadAndRunAsync("how can I solve 8x + 7 = -23",
        async @event =>
        {
            Console.WriteLine(@event.ToJsonString());
            await Task.CompletedTask;
        });
    thread = await run.GetThreadAsync();
    run = await run.WaitForStatusChangeAsync();
    Console.WriteLine($"Created thread and run: {run.ThreadId} -> {run.Id} -> {run.CreatedAt}");
    var messages = await thread.ListMessagesAsync();

    foreach (var response in messages.Items)
    {
        Console.WriteLine($"{response.Role}: {response.PrintContent()}");
    }
}
finally
{
    await assistant.DeleteAsync(deleteToolResources: thread == null);

    if (thread != null)
    {
        var isDeleted = await thread.DeleteAsync(deleteToolResources: true);
        Assert.IsTrue(isDeleted);
    }
}
```

###### [List Thread Run Steps](https://platform.openai.com/docs/api-reference/runs/listRunSteps)

Returns a list of run steps belonging to a run.

```csharp
using var api = new OpenAIClient();
var runStepList = await api.ThreadsEndpoint.ListRunStepsAsync("thread-id", "run-id");
// OR use extension method for convenience!
var runStepList = await run.ListRunStepsAsync();

foreach (var runStep in runStepList.Items)
{
    Console.WriteLine($"[{runStep.Id}] {runStep.Status} {runStep.CreatedAt} -> {runStep.ExpiresAt}");
}
```

###### [Retrieve Thread Run Step](https://platform.openai.com/docs/api-reference/runs/getRunStep)

Retrieves a run step.

```csharp
using var api = new OpenAIClient();
var runStep = await api.ThreadsEndpoint.RetrieveRunStepAsync("thread-id", "run-id", "step-id");
// OR use extension method for convenience!
var runStep = await run.RetrieveRunStepAsync("step-id");
var runStep = await runStep.UpdateAsync();
Console.WriteLine($"[{runStep.Id}] {runStep.Status} {runStep.CreatedAt} -> {runStep.ExpiresAt}");
```

###### [Cancel Thread Run](https://platform.openai.com/docs/api-reference/runs/cancelRun)

Cancels a run that is `in_progress`.

```csharp
using var api = new OpenAIClient();
var isCancelled = await api.ThreadsEndpoint.CancelRunAsync("thread-id", "run-id");
// OR use extension method for convenience!
var isCancelled = await run.CancelAsync();
Assert.IsTrue(isCancelled);
```

#### [Vector Stores](https://platform.openai.com/docs/api-reference/vector-stores)

Vector stores are used to store files for use by the `file_search` tool.

- [File Search Guide](https://platform.openai.com/docs/assistants/tools/file-search)

The Vector Stores API is accessed via `OpenAIClient.VectorStoresEndpoint`

##### [List Vector Stores](https://platform.openai.com/docs/api-reference/vector-stores/list)

Returns a list of vector stores.

```csharp
using var api = new OpenAIClient();
var vectorStores = await api.VectorStoresEndpoint.ListVectorStoresAsync();

foreach (var vectorStore in vectorStores.Items)
{
    Console.WriteLine(vectorStore);
}
```

##### [Create Vector Store](https://platform.openai.com/docs/api-reference/vector-stores/create)

Create a vector store.

```csharp
using var api = new OpenAIClient();
var createVectorStoreRequest = new CreateVectorStoreRequest("test-vector-store");
var vectorStore = await api.VectorStoresEndpoint.CreateVectorStoreAsync(createVectorStoreRequest);
Console.WriteLine(vectorStore);
```

##### [Retrieve Vector Store](https://platform.openai.com/docs/api-reference/vector-stores/retrieve)

Retrieves a vector store.

```csharp
using var api = new OpenAIClient();
var vectorStore = await api.VectorStoresEndpoint.GetVectorStoreAsync("vector-store-id");
Console.WriteLine(vectorStore);
```

##### [Modify Vector Store](https://platform.openai.com/docs/api-reference/vector-stores/modify)

Modifies a vector store.

```csharp
using var api = new OpenAIClient();
var metadata = new Dictionary<string, object> { { "Test", DateTime.UtcNow } };
var vectorStore = await api.VectorStoresEndpoint.ModifyVectorStoreAsync("vector-store-id", metadata: metadata);
Console.WriteLine(vectorStore);
```

##### [Delete Vector Store](https://platform.openai.com/docs/api-reference/vector-stores/delete)

Delete a vector store.

```csharp
using var api = new OpenAIClient();
var isDeleted = await api.VectorStoresEndpoint.DeleteVectorStoreAsync("vector-store-id");
Assert.IsTrue(isDeleted);
```

##### [Vector Store Files](https://platform.openai.com/docs/api-reference/vector-stores-files)

Vector store files represent files inside a vector store.

- [File Search Guide](https://platform.openai.com/docs/assistants/tools/file-search)

###### [List Vector Store Files](https://platform.openai.com/docs/api-reference/vector-stores-files/listFiles)

Returns a list of vector store files.

```csharp
using var api = new OpenAIClient();
var files = await api.VectorStoresEndpoint.ListVectorStoreFilesAsync("vector-store-id");

foreach (var file in vectorStoreFiles.Items)
{
    Console.WriteLine(file);
}
```

###### [Create Vector Store File](https://platform.openai.com/docs/api-reference/vector-stores-files/createFile)

Create a vector store file by attaching a file to a vector store.

```csharp
using var api = new OpenAIClient();
var file = await api.VectorStoresEndpoint.CreateVectorStoreFileAsync("vector-store-id", "file-id", new ChunkingStrategy(ChunkingStrategyType.Static));
Console.WriteLine(file);
```

###### [Retrieve Vector Store File](https://platform.openai.com/docs/api-reference/vector-stores-files/getFile)

Retrieves a vector store file.

```csharp
using var api = new OpenAIClient();
var file = await api.VectorStoresEndpoint.GetVectorStoreFileAsync("vector-store-id", "vector-store-file-id");
Console.WriteLine(file);
```

###### [Delete Vector Store File](https://platform.openai.com/docs/api-reference/vector-stores-files/deleteFile)

Delete a vector store file. This will remove the file from the vector store but the file itself will not be deleted. To delete the file, use the delete file endpoint.

```csharp
using var api = new OpenAIClient();
var isDeleted = await api.VectorStoresEndpoint.DeleteVectorStoreFileAsync("vector-store-id", vectorStoreFile);
Assert.IsTrue(isDeleted);
```

##### [Vector Store File Batches](https://platform.openai.com/docs/api-reference/vector-stores-file-batches)

Vector store files represent files inside a vector store.

- [File Search Guide](https://platform.openai.com/docs/assistants/tools/file-search)

###### [Create Vector Store File Batch](https://platform.openai.com/docs/api-reference/vector-stores-file-batches/createBatch)

Create a vector store file batch.

```csharp
using var api = new OpenAIClient();
var files = new List<string> { "file_id_1","file_id_2" };
var vectorStoreFileBatch = await api.VectorStoresEndpoint.CreateVectorStoreFileBatchAsync("vector-store-id", files);
Console.WriteLine(vectorStoreFileBatch);
```

###### [Retrieve Vector Store File Batch](https://platform.openai.com/docs/api-reference/vector-stores-file-batches/getBatch)

Retrieves a vector store file batch.

```csharp
using var api = new OpenAIClient();
var vectorStoreFileBatch = await api.VectorStoresEndpoint.GetVectorStoreFileBatchAsync("vector-store-id", "vector-store-file-batch-id");
// you can also use convenience methods!
vectorStoreFileBatch = await vectorStoreFileBatch.UpdateAsync();
vectorStoreFileBatch = await vectorStoreFileBatch.WaitForStatusChangeAsync();
```

###### [List Files In Vector Store Batch](https://platform.openai.com/docs/api-reference/vector-stores-file-batches/listBatchFiles)

Returns a list of vector store files in a batch.

```csharp
using var api = new OpenAIClient();
var files = await api.VectorStoresEndpoint.ListVectorStoreBatchFilesAsync("vector-store-id", "vector-store-file-batch-id");

foreach (var file in files.Items)
{
    Console.WriteLine(file);
}
```

###### [Cancel Vector Store File Batch](https://platform.openai.com/docs/api-reference/vector-stores-file-batches/cancelBatch)

Cancel a vector store file batch. This attempts to cancel the processing of files in this batch as soon as possible.

```csharp
using var api = new OpenAIClient();
var isCancelled = await api.VectorStoresEndpoint.CancelVectorStoreFileBatchAsync("vector-store-id", "vector-store-file-batch-id");
```

---

### [Chat](https://platform.openai.com/docs/api-reference/chat)

Given a chat conversation, the model will return a chat completion response.

The Chat API is accessed via `OpenAIClient.ChatEndpoint`

#### [Chat Completions](https://platform.openai.com/docs/api-reference/chat/create)

Creates a completion for the chat message

```csharp
using var api = new OpenAIClient();
var messages = new List<Message>
{
    new Message(Role.System, "You are a helpful assistant."),
    new Message(Role.User, "Who won the world series in 2020?"),
    new Message(Role.Assistant, "The Los Angeles Dodgers won the World Series in 2020."),
    new Message(Role.User, "Where was it played?"),
};
var chatRequest = new ChatRequest(messages, Model.GPT4o);
var response = await api.ChatEndpoint.GetCompletionAsync(chatRequest);
var choice = response.FirstChoice;
Console.WriteLine($"[{choice.Index}] {choice.Message.Role}: {choice.Message} | Finish Reason: {choice.FinishReason}");
```

#### [Chat Streaming](https://platform.openai.com/docs/api-reference/chat/create#chat/create-stream)

```csharp
using var api = new OpenAIClient();
var messages = new List<Message>
{
    new Message(Role.System, "You are a helpful assistant."),
    new Message(Role.User, "Who won the world series in 2020?"),
    new Message(Role.Assistant, "The Los Angeles Dodgers won the World Series in 2020."),
    new Message(Role.User, "Where was it played?"),
};
var chatRequest = new ChatRequest(messages);
var response = await api.ChatEndpoint.StreamCompletionAsync(chatRequest, async partialResponse =>
{
    Console.Write(partialResponse.FirstChoice.Delta.ToString());
    await Task.CompletedTask;
});
var choice = response.FirstChoice;
Console.WriteLine($"[{choice.Index}] {choice.Message.Role}: {choice.Message} | Finish Reason: {choice.FinishReason}");
```

Or if using [`IAsyncEnumerable{T}`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1?view=net-5.0) ([C# 8.0+](https://docs.microsoft.com/en-us/archive/msdn-magazine/2019/november/csharp-iterating-with-async-enumerables-in-csharp-8))

```csharp
using var api = new OpenAIClient();
var messages = new List<Message>
{
    new Message(Role.System, "You are a helpful assistant."),
    new Message(Role.User, "Who won the world series in 2020?"),
    new Message(Role.Assistant, "The Los Angeles Dodgers won the World Series in 2020."),
    new Message(Role.User, "Where was it played?"),
};
var cumulativeDelta = string.Empty;
var chatRequest = new ChatRequest(messages);
await foreach (var partialResponse in api.ChatEndpoint.StreamCompletionEnumerableAsync(chatRequest))
{
    foreach (var choice in partialResponse.Choices.Where(choice => choice.Delta?.Content != null))
    {
        cumulativeDelta += choice.Delta.Content;
    }
}

Console.WriteLine(cumulativeDelta);
```

#### [Chat Tools](https://platform.openai.com/docs/guides/function-calling)

```csharp
using var api = new OpenAIClient();
var messages = new List<Message>
{
    new(Role.System, "You are a helpful weather assistant. Always prompt the user for their location."),
    new Message(Role.User, "What's the weather like today?"),
};

foreach (var message in messages)
{
    Console.WriteLine($"{message.Role}: {message}");
}

// Define the tools that the assistant is able to use:
// 1. Get a list of all the static methods decorated with FunctionAttribute
var tools = Tool.GetAllAvailableTools(includeDefaults: false, forceUpdate: true, clearCache: true);
// 2. Define a custom list of tools:
var tools = new List<Tool>
{
    Tool.GetOrCreateTool(objectInstance, "TheNameOfTheMethodToCall"),
    Tool.FromFunc("a_custom_name_for_your_function", ()=> { /* Some logic to run */ })
};
var chatRequest = new ChatRequest(messages, tools: tools, toolChoice: "auto");
var response = await api.ChatEndpoint.GetCompletionAsync(chatRequest);
messages.Add(response.FirstChoice.Message);

Console.WriteLine($"{response.FirstChoice.Message.Role}: {response.FirstChoice} | Finish Reason: {response.FirstChoice.FinishReason}");

var locationMessage = new Message(Role.User, "I'm in Glasgow, Scotland");
messages.Add(locationMessage);
Console.WriteLine($"{locationMessage.Role}: {locationMessage.Content}");
chatRequest = new ChatRequest(messages, tools: tools, toolChoice: "auto");
response = await api.ChatEndpoint.GetCompletionAsync(chatRequest);

messages.Add(response.FirstChoice.Message);

if (response.FirstChoice.FinishReason == "stop")
{
    Console.WriteLine($"{response.FirstChoice.Message.Role}: {response.FirstChoice} | Finish Reason: {response.FirstChoice.FinishReason}");

    var unitMessage = new Message(Role.User, "Fahrenheit");
    messages.Add(unitMessage);
    Console.WriteLine($"{unitMessage.Role}: {unitMessage.Content}");
    chatRequest = new ChatRequest(messages, tools: tools, toolChoice: "auto");
    response = await api.ChatEndpoint.GetCompletionAsync(chatRequest);
}

// iterate over all tool calls and invoke them
foreach (var toolCall in response.FirstChoice.Message.ToolCalls)
{
    Console.WriteLine($"{response.FirstChoice.Message.Role}: {toolCall.Function.Name} | Finish Reason: {response.FirstChoice.FinishReason}");
    Console.WriteLine($"{toolCall.Function.Arguments}");
    // Invokes function to get a generic json result to return for tool call.
    var functionResult = await toolCall.InvokeFunctionAsync();
    // If you know the return type and do additional processing you can use generic overload
    var functionResult = await toolCall.InvokeFunctionAsync<string>();
    messages.Add(new Message(toolCall, functionResult));
    Console.WriteLine($"{Role.Tool}: {functionResult}");
}
// System: You are a helpful weather assistant.
// User: What's the weather like today?
// Assistant: Sure, may I know your current location? | Finish Reason: stop
// User: I'm in Glasgow, Scotland
// Assistant: GetCurrentWeather | Finish Reason: tool_calls
// {
//   "location": "Glasgow, Scotland",
//   "unit": "celsius"
// }
// Tool: The current weather in Glasgow, Scotland is 39°C.
```

#### [Chat Vision](https://platform.openai.com/docs/guides/vision)

> [!WARNING]
> Beta Feature. API subject to breaking changes.

```csharp
using var api = new OpenAIClient();
var messages = new List<Message>
{
    new Message(Role.System, "You are a helpful assistant."),
    new Message(Role.User, new List<Content>
    {
        "What's in this image?",
        new ImageUrl("https://upload.wikimedia.org/wikipedia/commons/thumb/d/dd/Gfp-wisconsin-madison-the-nature-boardwalk.jpg/2560px-Gfp-wisconsin-madison-the-nature-boardwalk.jpg", ImageDetail.Low)
    })
};
var chatRequest = new ChatRequest(messages, model: Model.GPT4o);
var response = await api.ChatEndpoint.GetCompletionAsync(chatRequest);
Console.WriteLine($"{response.FirstChoice.Message.Role}: {response.FirstChoice.Message.Content} | Finish Reason: {response.FirstChoice.FinishDetails}");
```

#### [Chat Audio](https://platform.openai.com/docs/guides/audio)

```csharp
using var api = new OpenAIClient();
var messages = new List<Message>
{
    new Message(Role.System, "You are a helpful assistant."),
    new Message(Role.User, "Is a golden retriever a good family dog?")
};
var chatRequest = new ChatRequest(messages, Model.GPT4oAudio, audioConfig: Voice.Alloy);
var response = await api.ChatEndpoint.GetCompletionAsync(chatRequest);
Console.WriteLine($"{response.FirstChoice.Message.Role}: {response.FirstChoice} | Finish Reason: {response.FirstChoice.FinishDetails}");
// todo play response.FirstChoice.Message.AudioOutput.Data
```

#### [Chat Structured Outputs](https://platform.openai.com/docs/guides/structured-outputs)

The evolution of  [Json Mode](#chat-json-mode). While both ensure valid JSON is produced, only Structured Outputs ensure schema adherence.

> [!IMPORTANT]
>
> - When using JSON mode, always instruct the model to produce JSON via some message in the conversation, for example via your system message. If you don't include an explicit instruction to generate JSON, the model may generate an unending stream of whitespace and the request may run continually until it reaches the token limit. To help ensure you don't forget, the API will throw an error if the string "JSON" does not appear somewhere in the context.
> - The JSON in the message the model returns may be partial (i.e. cut off) if `finish_reason` is length, which indicates the generation exceeded max_tokens or the conversation exceeded the token limit. To guard against this, check `finish_reason` before parsing the response.

First define the structure of your responses. These will be used as your schema.
These are the objects you'll deserialize to, so be sure to use standard Json object models.

```csharp
public class MathResponse
{
    [JsonInclude]
    [JsonPropertyName("steps")]
    public IReadOnlyList<MathStep> Steps { get; private set; }

    [JsonInclude]
    [JsonPropertyName("final_answer")]
    public string FinalAnswer { get; private set; }
}

public class MathStep
{
    [JsonInclude]
    [JsonPropertyName("explanation")]
    public string Explanation { get; private set; }

    [JsonInclude]
    [JsonPropertyName("output")]
    public string Output { get; private set; }
}
```

To use, simply specify the `MathResponse` type as a generic constraint when requesting a completion.

```csharp
using var api = new OpenAIClient();
var messages = new List<Message>
{
    new(Role.System, "You are a helpful math tutor. Guide the user through the solution step by step."),
    new(Role.User, "how can I solve 8x + 7 = -23")
};

var chatRequest = new ChatRequest(messages, model: "gpt-4o-2024-08-06");
var (mathResponse, chatResponse) = await api.ChatEndpoint.GetCompletionAsync<MathResponse>(chatRequest);

for (var i = 0; i < mathResponse.Steps.Count; i++)
{
    var step = mathResponse.Steps[i];
    Console.WriteLine($"Step {i}: {step.Explanation}");
    Console.WriteLine($"Result: {step.Output}");
}

Console.WriteLine($"Final Answer: {mathResponse.FinalAnswer}");
chatResponse.GetUsage();
```

#### [Chat Json Mode](https://platform.openai.com/docs/guides/text-generation/json-mode)

> [!IMPORTANT]
>
> - When using JSON mode, always instruct the model to produce JSON via some message in the conversation, for example via your system message. If you don't include an explicit instruction to generate JSON, the model may generate an unending stream of whitespace and the request may run continually until it reaches the token limit. To help ensure you don't forget, the API will throw an error if the string "JSON" does not appear somewhere in the context.
> - The JSON in the message the model returns may be partial (i.e. cut off) if `finish_reason` is length, which indicates the generation exceeded max_tokens or the conversation exceeded the token limit. To guard against this, check `finish_reason` before parsing the response.
> - JSON mode will not guarantee the output matches any specific schema, only that it is valid and parses without errors.

```csharp
var messages = new List<Message>
{
    new Message(Role.System, "You are a helpful assistant designed to output JSON."),
    new Message(Role.User, "Who won the world series in 2020?"),
};
var chatRequest = new ChatRequest(messages, Model.GPT4o, responseFormat: ChatResponseFormat.Json);
var response = await api.ChatEndpoint.GetCompletionAsync(chatRequest);

foreach (var choice in response.Choices)
{
    Console.WriteLine($"[{choice.Index}] {choice.Message.Role}: {choice} | Finish Reason: {choice.FinishReason}");
}

response.GetUsage();
```

---

### [Audio](https://platform.openai.com/docs/api-reference/audio)

Converts audio into text.

The Audio API is accessed via `OpenAIClient.AudioEndpoint`

#### [Create Speech](https://platform.openai.com/docs/api-reference/audio/createSpeech)

Generates audio from the input text.

```csharp
using var api = new OpenAIClient();
var request = new SpeechRequest("Hello World!");
async Task ChunkCallback(ReadOnlyMemory<byte> chunkCallback)
{
    // TODO Implement audio playback as chunks arrive
    await Task.CompletedTask;
}

var response = await api.AudioEndpoint.CreateSpeechAsync(request, ChunkCallback);
await File.WriteAllBytesAsync("../../../Assets/HelloWorld.mp3", response.ToArray());
```

#### [Create Transcription](https://platform.openai.com/docs/api-reference/audio/createTranscription)

Transcribes audio into the input language.

```csharp
using var api = new OpenAIClient();
using var request = new AudioTranscriptionRequest(Path.GetFullPath(audioAssetPath), language: "en");
var response = await api.AudioEndpoint.CreateTranscriptionTextAsync(request);
Console.WriteLine(response);
```

You can also get detailed information using `verbose_json` to get timestamp granularities:

```csharp
using var api = new OpenAIClient();
using var request = new AudioTranscriptionRequest(transcriptionAudio, responseFormat: AudioResponseFormat.Verbose_Json, timestampGranularity: TimestampGranularity.Word, temperature: 0.1f, language: "en");
var response = await api.AudioEndpoint.CreateTranscriptionTextAsync(request);

foreach (var word in response.Words)
{
    Console.WriteLine($"[{word.Start}-{word.End}] \"{word.Word}\"");
}
```

#### [Create Translation](https://platform.openai.com/docs/api-reference/audio/createTranslation)

Translates audio into into English.

```csharp
using var api = new OpenAIClient();
using var request = new AudioTranslationRequest(Path.GetFullPath(audioAssetPath));
var response = await api.AudioEndpoint.CreateTranslationTextAsync(request);
Console.WriteLine(response);
```

---

### [Images](https://platform.openai.com/docs/api-reference/images)

Given a prompt and/or an input image, the model will generate a new image.

The Images API is accessed via `OpenAIClient.ImagesEndpoint`

#### [Create Image](https://platform.openai.com/docs/api-reference/images/create)

Creates an image given a prompt.

```csharp
using var api = new OpenAIClient();
var request = new ImageGenerationRequest("A house riding a velociraptor", Models.Model.DallE_3);
var imageResults = await api.ImagesEndPoint.GenerateImageAsync(request);

foreach (var image in imageResults)
{
    Console.WriteLine(image);
    // image == url or b64_string
}
```

#### [Edit Image](https://platform.openai.com/docs/api-reference/images/create-edit)

Creates an edited or extended image given an original image and a prompt.

```csharp
using var api = new OpenAIClient();
var request = new ImageEditRequest(imageAssetPath, maskAssetPath, "A sunlit indoor lounge area with a pool containing a flamingo", size: ImageSize.Small);
var imageResults = await api.ImagesEndPoint.CreateImageEditAsync(request);

foreach (var image in imageResults)
{
    Console.WriteLine(image);
    // image == url or b64_string
}
```

#### [Create Image Variation](https://platform.openai.com/docs/api-reference/images/create-variation)

Creates a variation of a given image.

```csharp
using var api = new OpenAIClient();
var request = new ImageVariationRequest(imageAssetPath, size: ImageSize.Small);
var imageResults = await api.ImagesEndPoint.CreateImageVariationAsync(request);

foreach (var image in imageResults)
{
    Console.WriteLine(image);
    // image == url or b64_string
}
```

---

### [Files](https://platform.openai.com/docs/api-reference/files)

Files are used to upload documents that can be used with features like [Fine-tuning](#fine-tuning).

The Files API is accessed via `OpenAIClient.FilesEndpoint`

#### [List Files](https://platform.openai.com/docs/api-reference/files/list)

Returns a list of files that belong to the user's organization.

```csharp
using var api = new OpenAIClient();
var fileList = await api.FilesEndpoint.ListFilesAsync();

foreach (var file in fileList)
{
    Console.WriteLine($"{file.Id} -> {file.Object}: {file.FileName} | {file.Size} bytes");
}
```

#### [Upload File](https://platform.openai.com/docs/api-reference/files/create)

Upload a file that can be used across various endpoints. The size of all the files uploaded by one organization can be up to 100 GB.

The size of individual files can be a maximum of 512 MB. See the Assistants Tools guide to learn more about the types of files supported. The Fine-tuning API only supports .jsonl files.

```csharp
using var api = new OpenAIClient();
var file = await api.FilesEndpoint.UploadFileAsync("path/to/your/file.jsonl", FilePurpose.FineTune);
Console.WriteLine(file.Id);
```

#### [Delete File](https://platform.openai.com/docs/api-reference/files/delete)

Delete a file.

```csharp
using var api = new OpenAIClient();
var isDeleted = await api.FilesEndpoint.DeleteFileAsync(fileId);
Assert.IsTrue(isDeleted);
```

#### [Retrieve File Info](https://platform.openai.com/docs/api-reference/files/retrieve)

Returns information about a specific file.

```csharp
using var api = new OpenAIClient();
var file = await api.FilesEndpoint.GetFileInfoAsync(fileId);
Console.WriteLine($"{file.Id} -> {file.Object}: {file.FileName} | {file.Size} bytes");
```

#### [Download File Content](https://platform.openai.com/docs/api-reference/files/retrieve-content)

Downloads the file content to the specified directory.

```csharp
using var api = new OpenAIClient();
var downloadedFilePath = await api.FilesEndpoint.DownloadFileAsync(fileId, "path/to/your/save/directory");
Console.WriteLine(downloadedFilePath);
Assert.IsTrue(File.Exists(downloadedFilePath));
```

---

### [Fine Tuning](https://platform.openai.com/docs/api-reference/fine-tuning)

Manage fine-tuning jobs to tailor a model to your specific training data.

Related guide: [Fine-tune models](https://platform.openai.com/docs/guides/fine-tuning)

The Files API is accessed via `OpenAIClient.FineTuningEndpoint`

#### [Create Fine Tune Job](https://platform.openai.com/docs/api-reference/fine-tuning/create)

Creates a job that fine-tunes a specified model from a given dataset.

Response includes details of the enqueued job including job status and the name of the fine-tuned models once complete.

```csharp
using var api = new OpenAIClient();
var fileId = "file-abc123";
var request = new CreateFineTuneRequest(fileId);
var job = await api.FineTuningEndpoint.CreateJobAsync(Model.GPT3_5_Turbo, request);
Console.WriteLine($"Started {job.Id} | Status: {job.Status}");
```

#### [List Fine Tune Jobs](https://platform.openai.com/docs/api-reference/fine-tuning/list)

List your organization's fine-tuning jobs.

```csharp
using var api = new OpenAIClient();
var jobList = await api.FineTuningEndpoint.ListJobsAsync();

foreach (var job in jobList.Items.OrderByDescending(job => job.CreatedAt))
{
    Console.WriteLine($"{job.Id} -> {job.CreatedAt} | {job.Status}");
}
```

#### [Retrieve Fine Tune Job Info](https://platform.openai.com/docs/api-reference/fine-tuning/retrieve)

Gets info about the fine-tune job.

```csharp
using var api = new OpenAIClient();
var job = await api.FineTuningEndpoint.GetJobInfoAsync(fineTuneJob);
Console.WriteLine($"{job.Id} -> {job.CreatedAt} | {job.Status}");
```

#### [Cancel Fine Tune Job](https://platform.openai.com/docs/api-reference/fine-tuning/cancel)

Immediately cancel a fine-tune job.

```csharp
using var api = new OpenAIClient();
var isCancelled = await api.FineTuningEndpoint.CancelFineTuneJobAsync(fineTuneJob);
Assert.IsTrue(isCancelled);
```

#### [List Fine Tune Job Events](https://platform.openai.com/docs/api-reference/fine-tuning/list-events)

Get status updates for a fine-tuning job.

```csharp
using var api = new OpenAIClient();
var eventList = await api.FineTuningEndpoint.ListJobEventsAsync(fineTuneJob);
Console.WriteLine($"{fineTuneJob.Id} -> status: {fineTuneJob.Status} | event count: {eventList.Events.Count}");

foreach (var @event in eventList.Items.OrderByDescending(@event => @event.CreatedAt))
{
    Console.WriteLine($"  {@event.CreatedAt} [{@event.Level}] {@event.Message}");
}
```

---

### [Batches](https://platform.openai.com/docs/api-reference/batch)

Create large batches of API requests for asynchronous processing. The Batch API returns completions within 24 hours for a 50% discount.

- [Batch Guide](https://platform.openai.com/docs/guides/batch)

The Batches API is accessed via `OpenAIClient.BatchesEndpoint`

#### [List Batches](https://platform.openai.com/docs/api-reference/batch/list)

List your organization's batches.

```csharp
using var api = new OpenAIClient();
var batches = await api.BatchEndpoint.ListBatchesAsync();

foreach (var batch in listResponse.Items)
{
    Console.WriteLine(batch);
}
```

#### [Create Batch](https://platform.openai.com/docs/api-reference/batch/create)

Creates and executes a batch from an uploaded file of requests

```csharp
using var api = new OpenAIClient();
var batchRequest = new CreateBatchRequest("file-id", Endpoint.ChatCompletions);
var batch = await api.BatchEndpoint.CreateBatchAsync(batchRequest);
```

#### [Retrieve Batch](https://platform.openai.com/docs/api-reference/batch/retrieve)

Retrieves a batch.

```csharp
using var api = new OpenAIClient();
var batch = await api.BatchEndpoint.RetrieveBatchAsync("batch-id");
// you can also use convenience methods!
batch = await batch.UpdateAsync();
batch = await batch.WaitForStatusChangeAsync();
```

#### [Cancel Batch](https://platform.openai.com/docs/api-reference/batch/cancel)

Cancels an in-progress batch. The batch will be in status cancelling for up to 10 minutes, before changing to cancelled, where it will have partial results (if any) available in the output file.

```csharp
using var api = new OpenAIClient();
var isCancelled = await api.BatchEndpoint.CancelBatchAsync(batch);
Assert.IsTrue(isCancelled);
```

---

### [Embeddings](https://platform.openai.com/docs/api-reference/embeddings)

Get a vector representation of a given input that can be easily consumed by machine learning models and algorithms.

Related guide: [Embeddings](https://platform.openai.com/docs/guides/embeddings)

The Edits API is accessed via `OpenAIClient.EmbeddingsEndpoint`

#### [Create Embeddings](https://platform.openai.com/docs/api-reference/embeddings/create)

Creates an embedding vector representing the input text.

```csharp
using var api = new OpenAIClient();
var response = await api.EmbeddingsEndpoint.CreateEmbeddingAsync("The food was delicious and the waiter...", Models.Embedding_Ada_002);
Console.WriteLine(response);
```

---

### [Moderations](https://platform.openai.com/docs/api-reference/moderations)

Given a input text, outputs if the model classifies it as violating OpenAI's content policy.

Related guide: [Moderations](https://platform.openai.com/docs/guides/moderation)

The Moderations API can be accessed via `OpenAIClient.ModerationsEndpoint`

#### [Create Moderation](https://platform.openai.com/docs/api-reference/moderations/create)

Classifies if text violates OpenAI's Content Policy.

```csharp
using var api = new OpenAIClient();
var isViolation = await api.ModerationsEndpoint.GetModerationAsync("I want to kill them.");
Assert.IsTrue(isViolation);
```

Additionally you can also get the scores of a given input.

```csharp
using var api = new OpenAIClient();
var response = await api.ModerationsEndpoint.CreateModerationAsync(new ModerationsRequest("I love you"));
Assert.IsNotNull(response);
Console.WriteLine(response.Results?[0]?.Scores?.ToString());
```

---
