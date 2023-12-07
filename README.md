# OpenAI-DotNet

[![Discord](https://img.shields.io/discord/855294214065487932.svg?label=&logo=discord&logoColor=ffffff&color=7389D8&labelColor=6A7EC2)](https://discord.gg/xQgMW9ufN4)
[![NuGet version (OpenAI-DotNet)](https://img.shields.io/nuget/v/OpenAI-DotNet.svg?label=OpenAI-DotNet&logo=nuget)](https://www.nuget.org/packages/OpenAI-DotNet/)
[![NuGet version (OpenAI-DotNet-Proxy)](https://img.shields.io/nuget/v/OpenAI-DotNet-Proxy.svg?label=OpenAI-DotNet-Proxy&logo=nuget)](https://www.nuget.org/packages/OpenAI-DotNet-Proxy/)
[![Nuget Publish](https://github.com/RageAgainstThePixel/OpenAI-DotNet/actions/workflows/Publish-Nuget.yml/badge.svg)](https://github.com/RageAgainstThePixel/OpenAI-DotNet/actions/workflows/Publish-Nuget.yml)

A simple C# .NET client library for [OpenAI](https://openai.com/) to use though their RESTful API.
Independently developed, this is not an official library and I am not affiliated with OpenAI.
An OpenAI API account is required.

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
  - [Azure Active Directory Authentication](#azure-active-directory-authentication)
- [OpenAI API Proxy](#openai-api-proxy)
- [Models](#models)
  - [List Models](#list-models)
  - [Retrieve Models](#retrieve-model)
  - [Delete Fine Tuned Model](#delete-fine-tuned-model)
- [Assistants](#assistants) :new:
  - [List Assistants](#list-assistants) :new:
  - [Create Assistant](#create-assistant) :new:
  - [Retrieve Assistant](#retrieve-assistant) :new:
  - [Modify Assistant](#modify-assistant) :new:
  - [Delete Assistant](#delete-assistant) :new:
  - [List Assistant Files](#list-assistant-files) :new:
  - [Attach File to Assistant](#attach-file-to-assistant) :new:
  - [Upload File to Assistant](#upload-file-to-assistant) :new:
  - [Retrieve File from Assistant](#retrieve-file-from-assistant) :new:
  - [Remove File from Assistant](#remove-file-from-assistant) :new:
  - [Delete File from Assistant](#delete-file-from-assistant) :new:
- [Threads](#threads) :new:
  - [Create Thread](#create-thread) :new:
  - [Create Thread and Run](#create-thread-and-run) :new:
  - [Retrieve Thread](#retrieve-thread) :new:
  - [Modify Thread](#modify-thread) :new:
  - [Delete Thread](#delete-thread) :new:
  - [Thread Messages](#thread-messages) :new:
    - [List Messages](#list-thread-messages) :new:
    - [Create Message](#create-thread-message) :new:
    - [Retrieve Message](#retrieve-thread-message) :new:
    - [Modify Message](#modify-thread-message) :new:
    - [Thread Message Files](#thread-message-files) :new:
      - [List Message Files](#list-thread-message-files) :new:
      - [Retrieve Message File](#retrieve-thread-message-file) :new:
  - [Thread Runs](#thread-runs) :new:
    - [List Runs](#list-thread-runs) :new:
    - [Create Run](#create-thread-run) :new:
    - [Retrieve Run](#retrieve-thread-run) :new:
    - [Modify Run](#modify-thread-run) :new:
    - [Submit Tool Outputs to Run](#thread-submit-tool-outputs-to-run) :new:
    - [List Run Steps](#list-thread-run-steps) :new:
    - [Retrieve Run Step](#retrieve-thread-run-step) :new:
    - [Cancel Run](#cancel-thread-run) :new:
- [Chat](#chat)
  - [Chat Completions](#chat-completions)
  - [Streaming](#chat-streaming)
  - [Tools](#chat-tools) :new:
  - [Vision](#chat-vision) :new:
  - [Json Mode](#chat-json-mode) :new:
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
- [Embeddings](#embeddings)
  - [Create Embedding](#create-embeddings)
- [Moderations](#moderations)
  - [Create Moderation](#create-moderation)
- ~~[Completions](#completions)~~ :warning: Deprecated
  - ~~[Streaming](#completion-streaming)~~ :warning: Deprecated
- ~~[Edits](#edits)~~ :warning: Deprecated
  - ~~[Create Edit](#create-edit)~~  :warning: Deprecated

### [Authentication](https://platform.openai.com/docs/api-reference/authentication)

There are 3 ways to provide your API keys, in order of precedence:

1. [Pass keys directly with constructor](#pass-keys-directly-with-constructor)
2. [Load key from configuration file](#load-key-from-configuration-file)
3. [Use System Environment Variables](#use-system-environment-variables)

You use the `OpenAIAuthentication` when you initialize the API as shown:

#### Pass keys directly with constructor

:warning: We recommended using the environment variables to load the API key instead of having it hard coded in your source. It is not recommended use this method in production, but only for accepting user credentials, local testing and quick start scenarios.

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

You can also load the configuration file directly with known path by calling static methods in `OpenAIAuthentication`:

- Loads the default `.openai` config in the specified directory:

```csharp
var api = new OpenAIClient(OpenAIAuthentication.LoadFromDirectory("path/to/your/directory"));
```

- Loads the configuration file from a specific path. File does not need to be named `.openai` as long as it conforms to the json format:

```csharp
var api = new OpenAIClient(OpenAIAuthentication.LoadFromPath("path/to/your/file.json"));
```

#### Use System Environment Variables

Use your system's environment variables specify an api key and organization to use.

- Use `OPENAI_API_KEY` for your api key.
- Use `OPENAI_ORGANIZATION_ID` to specify an organization.

```csharp
var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
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
var api = new OpenAIClient();
var models = await api.ModelsEndpoint.GetModelsAsync();

foreach (var model in models)
{
    Console.WriteLine(model.ToString());
}
```

#### [Retrieve model](https://platform.openai.com/docs/api-reference/models/retrieve)

Retrieves a model instance, providing basic information about the model such as the owner and permissions.

```csharp
var api = new OpenAIClient();
var model = await api.ModelsEndpoint.GetModelDetailsAsync("text-davinci-003");
Console.WriteLine(model.ToString());
```

#### [Delete Fine Tuned Model](https://platform.openai.com/docs/api-reference/fine-tunes/delete-model)

Delete a fine-tuned model. You must have the Owner role in your organization.

```csharp
var api = new OpenAIClient();
var isDeleted = await api.ModelsEndpoint.DeleteFineTuneModelAsync("your-fine-tuned-model");
Assert.IsTrue(isDeleted);
```

### [Assistants](https://platform.openai.com/docs/api-reference/assistants)

> :warning: Beta Feature

Build assistants that can call models and use tools to perform tasks.

- [Assistants Guide](https://platform.openai.com/docs/assistants)
- [OpenAI Assistants Cookbook](https://github.com/openai/openai-cookbook/blob/main/examples/Assistants_API_overview_python.ipynb)

The Assistants API is accessed via `OpenAIClient.AssistantsEndpoint`

#### [List Assistants](https://platform.openai.com/docs/api-reference/assistants/listAssistants)

Returns a list of assistants.

```csharp
var api = new OpenAIClient();
var assistantsList = await OpenAIClient.AssistantsEndpoint.ListAssistantsAsync();

foreach (var assistant in assistantsList.Items)
{
    Console.WriteLine($"{assistant} -> {assistant.CreatedAt}");
}
```

#### [Create Assistant](https://platform.openai.com/docs/api-reference/assistants/createAssistant)

Create an assistant with a model and instructions.

```csharp
var api = new OpenAIClient();
var request = new CreateAssistantRequest("gpt-3.5-turbo-1106");
var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(request);
```

#### [Retrieve Assistant](https://platform.openai.com/docs/api-reference/assistants/getAssistant)

Retrieves an assistant.

```csharp
var api = new OpenAIClient();
var assistant = await OpenAIClient.AssistantsEndpoint.RetrieveAssistantAsync("assistant-id");
Console.WriteLine($"{assistant} -> {assistant.CreatedAt}");
```

#### [Modify Assistant](https://platform.openai.com/docs/api-reference/assistants/modifyAssistant)

Modifies an assistant.

```csharp
var api = new OpenAIClient();
var createRequest = new CreateAssistantRequest("gpt-3.5-turbo-1106");
var assistant = await api.AssistantsEndpoint.CreateAssistantAsync(createRequest);
var modifyRequest = new CreateAssistantRequest("gpt-4-1106-preview");
var modifiedAssistant = await api.AssistantsEndpoint.ModifyAsync(assistant.Id, modifyRequest);
// OR AssistantExtension for easier use!
var modifiedAssistantEx = await assistant.ModifyAsync(modifyRequest);
```

#### [Delete Assistant](https://platform.openai.com/docs/api-reference/assistants/deleteAssistant)

Delete an assistant.

```csharp
var api = new OpenAIClient();
var isDeleted = await api.AssistantsEndpoint.DeleteAssistantAsync("assistant-id");
// OR AssistantExtension for easier use!
var isDeleted = await assistant.DeleteAsync();
Assert.IsTrue(isDeleted);
```

#### [List Assistant Files](https://platform.openai.com/docs/api-reference/assistants/listAssistantFiles)

Returns a list of assistant files.

```csharp
var api = new OpenAIClient();
var filesList = await api.AssistantsEndpoint.ListFilesAsync("assistant-id");
// OR AssistantExtension for easier use!
var filesList = await assistant.ListFilesAsync();

foreach (var file in filesList.Items)
{
    Console.WriteLine($"{file.AssistantId}'s file -> {file.Id}");
}
```

#### [Attach File to Assistant](https://platform.openai.com/docs/api-reference/assistants/createAssistantFile)

Create an assistant file by attaching a File to an assistant.

```csharp
var api = new OpenAIClient();
var filePath = "assistant_test_2.txt";
await File.WriteAllTextAsync(filePath, "Knowledge is power!");
var fileUploadRequest = new FileUploadRequest(filePath, "assistant");
var file = await api.FilesEndpoint.UploadFileAsync(fileUploadRequest);
var assistantFile = await api.AssistantsEndpoint.AttachFileAsync("assistant-id", file.Id);
// OR use extension method for convenience!
var assistantFIle = await assistant.AttachFileAsync(file);
```

#### [Upload File to Assistant](#upload-file)

Uploads ***and*** attaches a file to an assistant.

> Assistant extension method, for extra convenience!

```csharp
var api = new OpenAIClient();
var filePath = "assistant_test_2.txt";
await File.WriteAllTextAsync(filePath, "Knowledge is power!");
var assistantFile = await assistant.UploadFileAsync(filePath);
```

#### [Retrieve File from Assistant](https://platform.openai.com/docs/api-reference/assistants/getAssistantFile)

Retrieves an AssistantFile.

```csharp
var api = new OpenAIClient();
var assistantFile = await api.AssistantsEndpoint.RetrieveFileAsync("assistant-id", "file-id");
// OR AssistantExtension for easier use!
var assistantFile = await assistant.RetrieveFileAsync(fileId);
Console.WriteLine($"{assistantFile.AssistantId}'s file -> {assistantFile.Id}");
```

#### [Remove File from Assistant](https://platform.openai.com/docs/api-reference/assistants/deleteAssistantFile)

Remove a file from an assistant.

> Note: The file will remain in your organization until [deleted with FileEndpoint](#delete-file).

```csharp
var api = new OpenAIClient();
var isRemoved = await api.AssistantsEndpoint.RemoveFileAsync("assistant-id", "file-id");
// OR use extension method for convenience!
var isRemoved = await assistant.RemoveFileAsync("file-id");
Assert.IsTrue(isRemoved);
```

#### [Delete File from Assistant](#delete-file)

Removes a file from the assistant and then deletes the file from the organization.

> Assistant extension method, for extra convenience!

```csharp
var api = new OpenAIClient();
var isDeleted = await assistant.DeleteFileAsync("file-id");
Assert.IsTrue(isDeleted);
```

### [Threads](https://platform.openai.com/docs/api-reference/threads)

> :warning: Beta Feature

Create Threads that [Assistants](#assistants) can interact with.

The Threads API is accessed via `OpenAIClient.ThreadsEndpoint`

#### [Create Thread](https://platform.openai.com/docs/api-reference/threads/createThread)

Create a thread.

```csharp
var api = new OpenAIClient();
var thread = await api.ThreadsEndpoint.CreateThreadAsync();
Console.WriteLine($"Create thread {thread.Id} -> {thread.CreatedAt}");
```

#### [Create Thread and Run](https://platform.openai.com/docs/api-reference/runs/createThreadAndRun)

Create a thread and run it in one request.

> See also: [Thread Runs](#thread-runs)

```csharp
var api = new OpenAIClient();
var assistant = await api.AssistantsEndpoint.CreateAssistantAsync(
    new CreateAssistantRequest(
        name: "Math Tutor",
        instructions: "You are a personal math tutor. Answer questions briefly, in a sentence or less.",
        model: "gpt-4-1106-preview"));
var messages = new List<Message> { "I need to solve the equation `3x + 11 = 14`. Can you help me?" };
var threadRequest = new CreateThreadRequest(messages);
var run = await assistant.CreateThreadAndRunAsync(threadRequest);
Console.WriteLine($"Created thread and run: {run.ThreadId} -> {run.Id} -> {run.CreatedAt}");
```

#### [Retrieve Thread](https://platform.openai.com/docs/api-reference/threads/getThread)

Retrieves a thread.

```csharp
var api = new OpenAIClient();
var thread = await api.ThreadsEndpoint.RetrieveThreadAsync("thread-id");
// OR if you simply wish to get the latest state of a thread
thread = await thread.UpdateAsync();
Console.WriteLine($"Retrieve thread {thread.Id} -> {thread.CreatedAt}");
```

#### [Modify Thread](https://platform.openai.com/docs/api-reference/threads/modifyThread)

Modifies a thread.

> Note: Only the metadata can be modified.

```csharp
var api = new OpenAIClient();
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

#### [Delete Thread](https://platform.openai.com/docs/api-reference/threads/deleteThread)

Delete a thread.

```csharp
var api = new OpenAIClient();
var isDeleted = await api.ThreadsEndpoint.DeleteThreadAsync("thread-id");
// OR use extension method for convenience!
var isDeleted = await thread.DeleteAsync();
Assert.IsTrue(isDeleted);
```

#### [Thread Messages](https://platform.openai.com/docs/api-reference/messages)

Create messages within threads.

##### [List Thread Messages](https://platform.openai.com/docs/api-reference/messages/listMessages)

Returns a list of messages for a given thread.

```csharp
var api = new OpenAIClient();
var messageList = await api.ThreadsEndpoint.ListMessagesAsync("thread-id");
// OR use extension method for convenience!
var messageList = await thread.ListMessagesAsync();

foreach (var message in messageList.Items)
{
    Console.WriteLine($"{message.Id}: {message.Role}: {message.PrintContent()}");
}
```

##### [Create Thread Message](https://platform.openai.com/docs/api-reference/messages/createMessage)

Create a message.

```csharp
var api = new OpenAIClient();
var thread = await api.ThreadsEndpoint.CreateThreadAsync();
var request = new CreateMessageRequest("Hello world!");
var message = await api.ThreadsEndpoint.CreateMessageAsync(thread.Id, request);
// OR use extension method for convenience!
var message = await thread.CreateMessageAsync("Hello World!");
Console.WriteLine($"{message.Id}: {message.Role}: {message.PrintContent()}");
```

##### [Retrieve Thread Message](https://platform.openai.com/docs/api-reference/messages/getMessage)

Retrieve a message.

```csharp
var api = new OpenAIClient();
var message = await api.ThreadsEndpoint.RetrieveMessageAsync("thread-id", "message-id");
// OR use extension methods for convenience!
var message = await thread.RetrieveMessageAsync("message-id");
var message = await message.UpdateAsync();
Console.WriteLine($"{message.Id}: {message.Role}: {message.PrintContent()}");
```

##### [Modify Thread Message](https://platform.openai.com/docs/api-reference/messages/modifyMessage)

Modify a message.

> Note: Only the message metadata can be modified.

```csharp
var api = new OpenAIClient();
var metadata = new Dictionary<string, string>
{
    { "key", "custom message metadata" }
};
var message = await api.ThreadsEndpoint.ModifyMessageAsync("thread-id", "message-id", metadata);
// OR use extension method for convenience!
var message = await message.ModifyAsync(metadata);
Console.WriteLine($"Modify message metadata: {message.Id} -> {message.Metadata["key"]}");
```

##### Thread Message Files

###### [List Thread Message Files](https://platform.openai.com/docs/api-reference/messages/listMessageFiles)

Returns a list of message files.

```csharp
var api = new OpenAIClient();
var fileList = await api.ThreadsEndpoint.ListFilesAsync("thread-id", "message-Id");
// OR use extension method for convenience!
var fileList = await thread.ListFilesAsync("message-id");
var fileList = await message.ListFilesAsync();

foreach (var file in fileList.Items)
{
    Console.WriteLine(file.Id);
}
```

###### [Retrieve Thread Message File](https://platform.openai.com/docs/api-reference/messages/getMessageFile)

Retrieves a message file.

```csharp
var api = new OpenAIClient();
var file = await api.ThreadsEndpoint.RetrieveFileAsync("thread-id", "message-id", "file-id");
// OR use extension method for convenience!
var file = await message.RetrieveFileAsync();
Console.WriteLine(file.Id);
```

#### [Thread Runs](https://platform.openai.com/docs/api-reference/runs)

Represents an execution run on a thread.

##### [List Thread Runs](https://platform.openai.com/docs/api-reference/runs/listRuns)

Returns a list of runs belonging to a thread.

```csharp
var api = new OpenAIClient();
var runList = await api.ThreadsEndpoint.ListRunsAsync("thread-id");
// OR use extension method for convenience!
var runList = await thread.ListRunsAsync();

foreach (var run in runList.Items)
{
    Console.WriteLine($"[{run.Id}] {run.Status} | {run.CreatedAt}");
}
```

##### [Create Thread Run](https://platform.openai.com/docs/api-reference/runs/createRun)

Create a run.

```csharp
var api = new OpenAIClient();
var assistant = await api.AssistantsEndpoint.CreateAssistantAsync(
    new CreateAssistantRequest(
        name: "Math Tutor",
        instructions: "You are a personal math tutor. Answer questions briefly, in a sentence or less.",
        model: "gpt-4-1106-preview"));
var thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync();
var message = await thread.CreateMessageAsync("I need to solve the equation `3x + 11 = 14`. Can you help me?");
var run = await thread.CreateRunAsync(assistant);
Console.WriteLine($"[{run.Id}] {run.Status} | {run.CreatedAt}");
```

##### [Retrieve Thread Run](https://platform.openai.com/docs/api-reference/runs/getRun)

Retrieves a run.

```csharp
var api = new OpenAIClient();
var run = await api.ThreadsEndpoint.RetrieveRunAsync("thread-id", "run-id");
// OR use extension method for convenience!
var run = await thread.RetrieveRunAsync("run-id");
var run = await run.UpdateAsync();
Console.WriteLine($"[{run.Id}] {run.Status} | {run.CreatedAt}");
```

##### [Modify Thread Run](https://platform.openai.com/docs/api-reference/runs/modifyRun)

Modifies a run.

> Note: Only the metadata can be modified.

```csharp
var api = new OpenAIClient();
var metadata = new Dictionary<string, string>
{
    { "key", "custom run metadata" }
};
var run = await api.ThreadsEndpoint.ModifyRunAsync("thread-id", "run-id", metadata);
// OR use extension method for convenience!
var run = await run.ModifyAsync(metadata);
Console.WriteLine($"Modify run {run.Id} -> {run.Metadata["key"]}");
```

##### [Thread Submit Tool Outputs to Run](https://platform.openai.com/docs/api-reference/runs/submitToolOutputs)

When a run has the status: `requires_action` and `required_action.type` is `submit_tool_outputs`, this endpoint can be used to submit the outputs from the tool calls once they're all completed. All outputs must be submitted in a single request.

```csharp
var api = new OpenAIClient();
var function = new Function(
    nameof(WeatherService.GetCurrentWeather),
    "Get the current weather in a given location",
    new JsonObject
    {
        ["type"] = "object",
        ["properties"] = new JsonObject
        {
            ["location"] = new JsonObject
            {
                ["type"] = "string",
                ["description"] = "The city and state, e.g. San Francisco, CA"
            },
            ["unit"] = new JsonObject
            {
                ["type"] = "string",
                ["enum"] = new JsonArray { "celsius", "fahrenheit" }
            }
        },
        ["required"] = new JsonArray { "location", "unit" }
    });
testAssistant = await api.AssistantsEndpoint.CreateAssistantAsync(new CreateAssistantRequest(tools: new Tool[] { function }));
var run = await testAssistant.CreateThreadAndRunAsync("I'm in Kuala-Lumpur, please tell me what's the temperature in celsius now?");
// waiting while run is Queued and InProgress
run = await run.WaitForStatusChangeAsync();
var toolCall = run.RequiredAction.SubmitToolOutputs.ToolCalls[0];
Console.WriteLine($"tool call arguments: {toolCall.FunctionCall.Arguments}");
var functionArgs = JsonSerializer.Deserialize<WeatherArgs>(toolCall.FunctionCall.Arguments);
var functionResult = WeatherService.GetCurrentWeather(functionArgs);
var toolOutput = new ToolOutput(toolCall.Id, functionResult);
run = await run.SubmitToolOutputsAsync(toolOutput);
// waiting while run in Queued and InProgress
run = await run.WaitForStatusChangeAsync();
var messages = await run.ListMessagesAsync();

foreach (var message in messages.Items.OrderBy(response => response.CreatedAt))
{
    Console.WriteLine($"{message.Role}: {message.PrintContent()}");
}
```

##### [List Thread Run Steps](https://platform.openai.com/docs/api-reference/runs/listRunSteps)

Returns a list of run steps belonging to a run.

```csharp
var api = new OpenAIClient();
var runStepList = await api.ThreadsEndpoint.ListRunStepsAsync("thread-id", "run-id");
// OR use extension method for convenience!
var runStepList = await run.ListRunStepsAsync();

foreach (var runStep in runStepList.Items)
{
    Console.WriteLine($"[{runStep.Id}] {runStep.Status} {runStep.CreatedAt} -> {runStep.ExpiresAt}");
}
```

##### [Retrieve Thread Run Step](https://platform.openai.com/docs/api-reference/runs/getRunStep)

Retrieves a run step.

```csharp
var api = new OpenAIClient();
var runStep = await api.ThreadsEndpoint.RetrieveRunStepAsync("thread-id", "run-id", "step-id");
// OR use extension method for convenience!
var runStep = await run.RetrieveRunStepAsync("step-id");
var runStep = await runStep.UpdateAsync();
Console.WriteLine($"[{runStep.Id}] {runStep.Status} {runStep.CreatedAt} -> {runStep.ExpiresAt}");
```

##### [Cancel Thread Run](https://platform.openai.com/docs/api-reference/runs/cancelRun)

Cancels a run that is `in_progress`.

```csharp
var api = new OpenAIClient();
var isCancelled = await api.ThreadsEndpoint.CancelRunAsync("thread-id", "run-id");
// OR use extension method for convenience!
var isCancelled = await run.CancelAsync();
Assert.IsTrue(isCancelled);
```

### [Chat](https://platform.openai.com/docs/api-reference/chat)

Given a chat conversation, the model will return a chat completion response.

The Chat API is accessed via `OpenAIClient.ChatEndpoint`

#### [Chat Completions](https://platform.openai.com/docs/api-reference/chat/create)

Creates a completion for the chat message

```csharp
var api = new OpenAIClient();
var messages = new List<Message>
{
    new Message(Role.System, "You are a helpful assistant."),
    new Message(Role.User, "Who won the world series in 2020?"),
    new Message(Role.Assistant, "The Los Angeles Dodgers won the World Series in 2020."),
    new Message(Role.User, "Where was it played?"),
};
var chatRequest = new ChatRequest(messages, Model.GPT4);
var response = await api.ChatEndpoint.GetCompletionAsync(chatRequest);
var choice = response.FirstChoice;
Console.WriteLine($"[{choice.Index}] {choice.Message.Role}: {choice.Message} | Finish Reason: {choice.FinishReason}");
```

#### [Chat Streaming](https://platform.openai.com/docs/api-reference/chat/create#chat/create-stream)

```csharp
var api = new OpenAIClient();
var messages = new List<Message>
{
    new Message(Role.System, "You are a helpful assistant."),
    new Message(Role.User, "Who won the world series in 2020?"),
    new Message(Role.Assistant, "The Los Angeles Dodgers won the World Series in 2020."),
    new Message(Role.User, "Where was it played?"),
};
var chatRequest = new ChatRequest(messages);
var response = await api.ChatEndpoint.StreamCompletionAsync(chatRequest, partialResponse =>
{
    Console.Write(partialResponse.FirstChoice.Delta.ToString());
});
var choice = response.FirstChoice;
Console.WriteLine($"[{choice.Index}] {choice.Message.Role}: {choice.Message} | Finish Reason: {choice.FinishReason}");
```

Or if using [`IAsyncEnumerable{T}`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1?view=net-5.0) ([C# 8.0+](https://docs.microsoft.com/en-us/archive/msdn-magazine/2019/november/csharp-iterating-with-async-enumerables-in-csharp-8))

```csharp
var api = new OpenAIClient();
var messages = new List<Message>
{
    new Message(Role.System, "You are a helpful assistant."),
    new Message(Role.User, "Who won the world series in 2020?"),
    new Message(Role.Assistant, "The Los Angeles Dodgers won the World Series in 2020."),
    new Message(Role.User, "Where was it played?"),
};
var cumulativeDelta = string.Empty;
var chatRequest = new ChatRequest(messages);
await foreach (var partialResponse in OpenAIClient.ChatEndpoint.StreamCompletionEnumerableAsync(chatRequest))
{
    foreach (var choice in partialResponse.Choices.Where(choice => choice.Delta?.Content != null))
    {
        cumulativeDelta += choice.Delta.Content;
    }
}

Console.WriteLine(cumulativeDelta);
```

#### [Chat Tools](https://platform.openai.com/docs/guides/function-calling)

> Only available with the latest 0613 model series!

```csharp
var api = new OpenAIClient();
var messages = new List<Message>
{
    new Message(Role.System, "You are a helpful weather assistant."),
    new Message(Role.User, "What's the weather like today?"),
};

foreach (var message in messages)
{
    Console.WriteLine($"{message.Role}: {message}");
}

// Define the tools that the assistant is able to use:
var tools = new List<Tool>
{
    new Function(
        nameof(WeatherService.GetCurrentWeather),
        "Get the current weather in a given location",
            new JsonObject
            {
                ["type"] = "object",
                ["properties"] = new JsonObject
                {
                    ["location"] = new JsonObject
                    {
                        ["type"] = "string",
                        ["description"] = "The city and state, e.g. San Francisco, CA"
                    },
                    ["unit"] = new JsonObject
                    {
                        ["type"] = "string",
                        ["enum"] = new JsonArray {"celsius", "fahrenheit"}
                    }
                },
                ["required"] = new JsonArray { "location", "unit" }
            })
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

if (!string.IsNullOrEmpty(response.ToString()))
{
    Console.WriteLine($"{response.FirstChoice.Message.Role}: {response.FirstChoice} | Finish Reason: {response.FirstChoice.FinishReason}");

    var unitMessage = new Message(Role.User, "celsius");
    messages.Add(unitMessage);
    Console.WriteLine($"{unitMessage.Role}: {unitMessage.Content}");
    chatRequest = new ChatRequest(messages, tools: tools, toolChoice: "auto");
    response = await api.ChatEndpoint.GetCompletionAsync(chatRequest);
}

var usedTool = response.FirstChoice.Message.ToolCalls[0];
Console.WriteLine($"{response.FirstChoice.Message.Role}: {usedTool.Function.Name} | Finish Reason: {response.FirstChoice.FinishReason}");
Console.WriteLine($"{usedTool.Function.Arguments}");
var functionArgs = JsonSerializer.Deserialize<WeatherArgs>(usedTool.Function.Arguments.ToString());
var functionResult = WeatherService.GetCurrentWeather(functionArgs);
messages.Add(new Message(usedTool, functionResult));
Console.WriteLine($"{Role.Tool}: {functionResult}");
// System: You are a helpful weather assistant.
// User: What's the weather like today?
// Assistant: Sure, may I know your current location? | Finish Reason: stop
// User: I'm in Glasgow, Scotland
// Assistant: GetCurrentWeather | Finish Reason: tool_calls
// {
//   "location": "Glasgow, Scotland",
//   "unit": "celsius"
// }
// Tool: The current weather in Glasgow, Scotland is 20 celsius
```

#### [Chat Vision](https://platform.openai.com/docs/guides/vision)

> :warning: Beta Feature
> Currently, GPT-4 with vision does not support the `message.name` parameter, functions/tools, nor the `response_format` parameter.

```csharp
var api = new OpenAIClient();
var messages = new List<Message>
{
    new Message(Role.System, "You are a helpful assistant."),
    new Message(Role.User, new List<Content>
    {
        "What's in this image?",
        new ImageUrl("https://upload.wikimedia.org/wikipedia/commons/thumb/d/dd/Gfp-wisconsin-madison-the-nature-boardwalk.jpg/2560px-Gfp-wisconsin-madison-the-nature-boardwalk.jpg", ImageDetail.Low)
    })
};
var chatRequest = new ChatRequest(messages, model: "gpt-4-vision-preview");
var response = await api.ChatEndpoint.GetCompletionAsync(chatRequest);
Console.WriteLine($"{response.FirstChoice.Message.Role}: {response.FirstChoice.Message.Content} | Finish Reason: {response.FirstChoice.FinishDetails}");
```

#### [Chat Json Mode](https://platform.openai.com/docs/guides/text-generation/json-mode)

> :warning: Beta Feature

Important notes:

- When using JSON mode, always instruct the model to produce JSON via some message in the conversation, for example via your system message. If you don't include an explicit instruction to generate JSON, the model may generate an unending stream of whitespace and the request may run continually until it reaches the token limit. To help ensure you don't forget, the API will throw an error if the string "JSON" does not appear somewhere in the context.
- The JSON in the message the model returns may be partial (i.e. cut off) if `finish_reason` is length, which indicates the generation exceeded max_tokens or the conversation exceeded the token limit. To guard against this, check `finish_reason` before parsing the response.
- JSON mode will not guarantee the output matches any specific schema, only that it is valid and parses without errors.

```csharp
var messages = new List<Message>
{
    new Message(Role.System, "You are a helpful assistant designed to output JSON."),
    new Message(Role.User, "Who won the world series in 2020?"),
};
var chatRequest = new ChatRequest(messages, "gpt-4-1106-preview", responseFormat: ChatResponseFormat.Json);
var response = await OpenAIClient.ChatEndpoint.GetCompletionAsync(chatRequest);

foreach (var choice in response.Choices)
{
    Console.WriteLine($"[{choice.Index}] {choice.Message.Role}: {choice} | Finish Reason: {choice.FinishReason}");
}

response.GetUsage();
```

### [Audio](https://platform.openai.com/docs/api-reference/audio)

Converts audio into text.

The Audio API is accessed via `OpenAIClient.AudioEndpoint`

#### [Create Speech](https://platform.openai.com/docs/api-reference/audio/createSpeech)

Generates audio from the input text.

```csharp
var api = new OpenAIClient();
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
var api = new OpenAIClient();
var request = new AudioTranscriptionRequest(Path.GetFullPath(audioAssetPath), language: "en");
var response = await api.AudioEndpoint.CreateTranscriptionAsync(request);
Console.WriteLine(response);
```

#### [Create Translation](https://platform.openai.com/docs/api-reference/audio/createTranslation)

Translates audio into into English.

```csharp
var api = new OpenAIClient();
var request = new AudioTranslationRequest(Path.GetFullPath(audioAssetPath));
var response = await api.AudioEndpoint.CreateTranslationAsync(request);
Console.WriteLine(response);
```

### [Images](https://platform.openai.com/docs/api-reference/images)

Given a prompt and/or an input image, the model will generate a new image.

The Images API is accessed via `OpenAIClient.ImagesEndpoint`

#### [Create Image](https://platform.openai.com/docs/api-reference/images/create)

Creates an image given a prompt.

```csharp
var api = new OpenAIClient();
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
var api = new OpenAIClient();
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
var api = new OpenAIClient();
var request = new ImageVariationRequest(imageAssetPath, size: ImageSize.Small);
var imageResults = await api.ImagesEndPoint.CreateImageVariationAsync(request);

foreach (var image in imageResults)
{
    Console.WriteLine(image);
    // image == url or b64_string
}
```

### [Files](https://platform.openai.com/docs/api-reference/files)

Files are used to upload documents that can be used with features like [Fine-tuning](#fine-tuning).

The Files API is accessed via `OpenAIClient.FilesEndpoint`

#### [List Files](https://platform.openai.com/docs/api-reference/files/list)

Returns a list of files that belong to the user's organization.

```csharp
var api = new OpenAIClient();
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
var api = new OpenAIClient();
var file = await api.FilesEndpoint.UploadFileAsync("path/to/your/file.jsonl", "fine-tune");
Console.WriteLine(file.Id);
```

#### [Delete File](https://platform.openai.com/docs/api-reference/files/delete)

Delete a file.

```csharp
var api = new OpenAIClient();
var isDeleted = await api.FilesEndpoint.DeleteFileAsync(fileId);
Assert.IsTrue(isDeleted);
```

#### [Retrieve File Info](https://platform.openai.com/docs/api-reference/files/retrieve)

Returns information about a specific file.

```csharp
var api = new OpenAIClient();
var file = await GetFileInfoAsync(fileId);
Console.WriteLine($"{file.Id} -> {file.Object}: {file.FileName} | {file.Size} bytes");
```

#### [Download File Content](https://platform.openai.com/docs/api-reference/files/retrieve-content)

Downloads the file content to the specified directory.

```csharp
var api = new OpenAIClient();
var downloadedFilePath = await api.FilesEndpoint.DownloadFileAsync(fileId, "path/to/your/save/directory");
Console.WriteLine(downloadedFilePath);
Assert.IsTrue(File.Exists(downloadedFilePath));
```

### [Fine Tuning](https://platform.openai.com/docs/api-reference/fine-tuning)

Manage fine-tuning jobs to tailor a model to your specific training data.

Related guide: [Fine-tune models](https://platform.openai.com/docs/guides/fine-tuning)

The Files API is accessed via `OpenAIClient.FineTuningEndpoint`

#### [Create Fine Tune Job](https://platform.openai.com/docs/api-reference/fine-tuning/create)

Creates a job that fine-tunes a specified model from a given dataset.

Response includes details of the enqueued job including job status and the name of the fine-tuned models once complete.

```csharp
var api = new OpenAIClient();
var fileId = "file-abc123";
var request = new CreateFineTuneRequest(fileId);
var job = await api.FineTuningEndpoint.CreateJobAsync(Model.GPT3_5_Turbo, request);
Console.WriteLine($"Started {job.Id} | Status: {job.Status}");
```

#### [List Fine Tune Jobs](https://platform.openai.com/docs/api-reference/fine-tuning/list)

List your organization's fine-tuning jobs.

```csharp
var api = new OpenAIClient();
var jobList = await api.FineTuningEndpoint.ListJobsAsync();

foreach (var job in jobList.Items.OrderByDescending(job => job.CreatedAt)))
{
    Console.WriteLine($"{job.Id} -> {job.CreatedAt} | {job.Status}");
}
```

#### [Retrieve Fine Tune Job Info](https://platform.openai.com/docs/api-reference/fine-tuning/retrieve)

Gets info about the fine-tune job.

```csharp
var api = new OpenAIClient();
var job = await api.FineTuningEndpoint.GetJobInfoAsync(fineTuneJob);
Console.WriteLine($"{job.Id} -> {job.CreatedAt} | {job.Status}");
```

#### [Cancel Fine Tune Job](https://platform.openai.com/docs/api-reference/fine-tuning/cancel)

Immediately cancel a fine-tune job.

```csharp
var api = new OpenAIClient();
var isCancelled = await api.FineTuningEndpoint.CancelFineTuneJobAsync(fineTuneJob);
Assert.IsTrue(isCancelled);
```

#### [List Fine Tune Job Events](https://platform.openai.com/docs/api-reference/fine-tuning/list-events)

Get status updates for a fine-tuning job.

```csharp
var api = new OpenAIClient();
var eventList = await api.FineTuningEndpoint.ListJobEventsAsync(fineTuneJob);
Console.WriteLine($"{fineTuneJob.Id} -> status: {fineTuneJob.Status} | event count: {eventList.Events.Count}");

foreach (var @event in eventList.Items.OrderByDescending(@event => @event.CreatedAt))
{
    Console.WriteLine($"  {@event.CreatedAt} [{@event.Level}] {@event.Message}");
}
```

### [Embeddings](https://platform.openai.com/docs/api-reference/embeddings)

Get a vector representation of a given input that can be easily consumed by machine learning models and algorithms.

Related guide: [Embeddings](https://platform.openai.com/docs/guides/embeddings)

The Edits API is accessed via `OpenAIClient.EmbeddingsEndpoint`

#### [Create Embeddings](https://platform.openai.com/docs/api-reference/embeddings/create)

Creates an embedding vector representing the input text.

```csharp
var api = new OpenAIClient();
var response = await api.EmbeddingsEndpoint.CreateEmbeddingAsync("The food was delicious and the waiter...", Models.Embedding_Ada_002);
Console.WriteLine(response);
```

### [Moderations](https://platform.openai.com/docs/api-reference/moderations)

Given a input text, outputs if the model classifies it as violating OpenAI's content policy.

Related guide: [Moderations](https://platform.openai.com/docs/guides/moderation)

The Moderations API can be accessed via `OpenAIClient.ModerationsEndpoint`

#### [Create Moderation](https://platform.openai.com/docs/api-reference/moderations/create)

Classifies if text violates OpenAI's Content Policy.

```csharp
var api = new OpenAIClient();
var isViolation = await api.ModerationsEndpoint.GetModerationAsync("I want to kill them.");
Assert.IsTrue(isViolation);
```

Additionally you can also get the scores of a given input.

```csharp
var response = await OpenAIClient.ModerationsEndpoint.CreateModerationAsync(new ModerationsRequest("I love you"));
Assert.IsNotNull(response);
Console.WriteLine(response.Results?[0]?.Scores?.ToString());
```

---

### [Completions](https://platform.openai.com/docs/api-reference/completions)

> :warning: Deprecated, and soon to be removed.

Given a prompt, the model will return one or more predicted completions, and can also return the probabilities of alternative tokens at each position.

The Completions API is accessed via `OpenAIClient.CompletionsEndpoint`

```csharp
var api = new OpenAIClient();
var response = await api.CompletionsEndpoint.CreateCompletionAsync("One Two Three One Two", temperature: 0.1, model: Model.Davinci);
Console.WriteLine(response);
```

> To get the `CompletionResponse` (which is mostly metadata), use its implicit string operator to get the text if all you want is the completion choice.

#### Completion Streaming

> :warning: Deprecated, and soon to be removed.

Streaming allows you to get results are they are generated, which can help your application feel more responsive, especially on slow models like Davinci.

```csharp
var api = new OpenAIClient();

await api.CompletionsEndpoint.StreamCompletionAsync(response =>
{
    foreach (var choice in response.Completions)
    {
        Console.WriteLine(choice);
    }
}, "My name is Roger and I am a principal software engineer at Salesforce.  This is my resume:", maxTokens: 200, temperature: 0.5, presencePenalty: 0.1, frequencyPenalty: 0.1, model: Model.Davinci);
```

Or if using [`IAsyncEnumerable{T}`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1?view=net-5.0) ([C# 8.0+](https://docs.microsoft.com/en-us/archive/msdn-magazine/2019/november/csharp-iterating-with-async-enumerables-in-csharp-8))

```csharp
var api = new OpenAIClient();
await foreach (var partialResponse in api.CompletionsEndpoint.StreamCompletionEnumerableAsync("My name is Roger and I am a principal software engineer at Salesforce.  This is my resume:", maxTokens: 200, temperature: 0.5, presencePenalty: 0.1, frequencyPenalty: 0.1, model: Model.Davinci))
{
  Console.WriteLine(partialResponse);
}
```

### [Edits](https://platform.openai.com/docs/api-reference/edits)

> :warning: Deprecated, and soon to be removed.

Given a prompt and an instruction, the model will return an edited version of the prompt.

The Edits API is accessed via `OpenAIClient.EditsEndpoint`

#### [Create Edit](https://platform.openai.com/docs/api-reference/edits/create)

Creates a new edit for the provided input, instruction, and parameters using the provided input and instruction.

```csharp
var api = new OpenAIClient();
var request = new EditRequest("What day of the wek is it?", "Fix the spelling mistakes");
var response = await api.EditsEndpoint.CreateEditAsync(request);
Console.WriteLine(response);
```

## License

![CC-0 Public Domain](https://licensebuttons.net/p/zero/1.0/88x31.png)

This library is licensed CC-0, in the public domain.  You can use it for whatever you want, publicly or privately, without worrying about permission or licensing or whatever.  It's just a wrapper around the OpenAI API, so you still need to get access to OpenAI from them directly.  I am not affiliated with OpenAI and this library is not endorsed by them, I just have beta access and wanted to make a C# library to access it more easily.  Hopefully others find this useful as well.  Feel free to open a PR if there's anything you want to contribute.
