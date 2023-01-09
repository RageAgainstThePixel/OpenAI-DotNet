# C#/.NET SDK for accessing the OpenAI GPT-3 API

A simple C# .NET wrapper library to use with [OpenAI](https://openai.com/)'s GPT-3 API.  More context [on Roger Pincombe's blog](https://rogerpincombe.com/openai-dotnet-api) and forked from [OpenAI-API-dotnet](https://github.com/OkGoDoIt/OpenAI-API-dotnet).

## Requirements

This library is based on .NET Standard 6.0.
It should work across console apps, winforms, wpf, asp.net, etc. It should also work across Windows, Linux, and Mac.

## Getting started

### Install from NuGet

[![NuGet version (OpenAI-DotNet)](https://img.shields.io/nuget/v/OpenAI-DotNet.svg)](https://www.nuget.org/packages/OpenAI-DotNet/)
[![Nuget Publish](https://github.com/RageAgainstThePixel/OpenAI-DotNet/actions/workflows/Publish-Nuget.yml/badge.svg)](https://github.com/RageAgainstThePixel/OpenAI-DotNet/actions/workflows/Publish-Nuget.yml)
[![Discord](https://img.shields.io/discord/855294214065487932.svg?label=&logo=discord&logoColor=ffffff&color=7389D8&labelColor=6A7EC2)](https://discord.gg/xQgMW9ufN4)

Install package [`OpenAI` from Nuget](https://www.nuget.org/packages/OpenAI-DotNet/).  Here's how via command line:

```powershell
Install-Package OpenAI-DotNet
```

> Looking to use OpenAI in the Unity Game Engine? Check out our unity package on OpenUPM:
>
>[![openupm](https://img.shields.io/npm/v/com.openai.unity?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.openai.unity/)

### Quick Start

Uses the default authentication from the current directory, the default user directory or system environment variables:

```csharp
var api = new OpenAIClient();
```

- [Authentication](#authentication)
- [Models](#models)
  - [List Models](#list-models)
  - [Retrieve Models](#retrieve-model)
- [Completions](#completions)
  - [Streaming](#streaming)
- [Edits](#edits)
  - [Create Edit](#create-edit)
- [Embeddings](#embeddings)
  - [Create Embedding](#create-embeddings)
- [Images](#images)
  - [Create Image](#create-image)
  - [Edit Image](#edit-image)
  - [Create Image Variation](#create-image-variation)
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
var api = new OpenAIClient("sk-mykeyhere");
```

Or create a `OpenAIAuthentication` object manually

```csharp
var api = new OpenAIClient(new OpenAIAuthentication("sk-secretkey"));
```

#### Load key from configuration file

Attempts to load api keys from a configuration file, by default `.openai` in the current directory, optionally traversing up the directory tree or in the user's home directory.

To create a configuration file, create a new text file named `.openai` and containing the line:

```shell
OPENAI_KEY=sk-aaaabbbbbccccddddd
```

You can also load the file directly with known path by calling a static method in Authentication:

```csharp
var api = new OpenAIClient(OpenAIAuthentication.LoadFromDirectory("C:\\Path\\To\\.openai"));;
```

#### Use System Environment Variables

> Use `OPENAI_KEY` or `OPENAI_SECRET_KEY` specify a key defined in the system's local environment:

```csharp
var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
```

### [Models](https://beta.openai.com/docs/api-reference/models)

List and describe the various models available in the API. You can refer to the Models documentation to understand what models are available and the differences between them.

The Models API is accessed via `OpenAIClient.ModelsEndpoint`.

#### [List models](https://beta.openai.com/docs/api-reference/models/list)

Lists the currently available models, and provides basic information about each one such as the owner and availability.

```csharp
var api = new OpenAIClient();
var models = await api.ModelsEndpoint.GetModelsAsync();
```

#### [Retrieve model](https://beta.openai.com/docs/api-reference/models/retrieve)

Retrieves a model instance, providing basic information about the model such as the owner and permissioning.

```csharp
var api = new OpenAIClient();
var model = await api.ModelsEndpoint.GetModelDetailsAsync("text-davinci-003");
```

### [Completions](https://beta.openai.com/docs/api-reference/completions)

The Completion API is accessed via `OpenAIClient.CompletionEndpoint`:

```csharp
var api = new OpenAIClient();
var result = await api.CompletionEndpoint.CreateCompletionAsync("One Two Three One Two", temperature: 0.1, model: Model.Davinci);
Debug.Log(result);
```

 Get the `CompletionResult` (which is mostly metadata), use its implicit string operator to get the text if all you want is the completion choice.

#### Streaming

Streaming allows you to get results are they are generated, which can help your application feel more responsive, especially on slow models like Davinci.

```csharp
var api = new OpenAIClient();

await api.CompletionEndpoint.StreamCompletionAsync(result =>
{
    foreach (var choice in result.Completions)
    {
        Debug.Log(choice);
    }
}, "My name is Roger and I am a principal software engineer at Salesforce.  This is my resume:", max_tokens: 200, temperature: 0.5, presencePenalty: 0.1, frequencyPenalty: 0.1, model: Model.Davinci);
```

The result.Completions

Or if using [`IAsyncEnumerable{T}`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1?view=net-5.0) ([C# 8.0+](https://docs.microsoft.com/en-us/archive/msdn-magazine/2019/november/csharp-iterating-with-async-enumerables-in-csharp-8))

```csharp
var api = new OpenAIClient();
await foreach (var token in api.CompletionEndpoint.StreamCompletionEnumerableAsync("My name is Roger and I am a principal software engineer at Salesforce.  This is my resume:", max_tokens: 200, temperature: 0.5, presencePenalty: 0.1, frequencyPenalty: 0.1, model: Model.Davinci))
{
  Debug.Log(token);
}
```

### [Edits](https://beta.openai.com/docs/api-reference/edits)

Given a prompt and an instruction, the model will return an edited version of the prompt.

The Edits API is accessed via `OpenAIClient.EditsEndpoint`.

#### [Create Edit](https://beta.openai.com/docs/api-reference/edits/create)

Creates a new edit for the provided input, instruction, and parameters using the provided input and instruction.

The Create Edit API is accessed via `OpenAIClient.ImagesEndpoint.CreateEditAsync()`.

```csharp
var api = new OpenAIClient();
var request = new EditRequest("What day of the wek is it?", "Fix the spelling mistakes");
var result = await api.EditsEndpoint.CreateEditAsync(request);
```

### [Embeddings](https://beta.openai.com/docs/api-reference/embeddings)

Get a vector representation of a given input that can be easily consumed by machine learning models and algorithms.

Related guide: [Embeddings](https://beta.openai.com/docs/guides/embeddings)

The Edits API is accessed via `OpenAIClient.EmbeddingsEndpoint`.

#### [Create Embeddings](https://beta.openai.com/docs/api-reference/embeddings/create)

Creates an embedding vector representing the input text.

The Create Embedding API is accessed via `OpenAIClient.EmbeddingsEndpoint.CreateEmbeddingAsync()`.

```csharp
var api = new OpenAIClient();
var result = await api.EmbeddingsEndpoint.CreateEmbeddingAsync("The food was delicious and the waiter...");
```

### [Images](https://beta.openai.com/docs/api-reference/images)

Given a prompt and/or an input image, the model will generate a new image.

The Images API is accessed via `OpenAIClient.ImagesEndpoint`.

#### [Create Image](https://beta.openai.com/docs/api-reference/images/create)

Creates an image given a prompt.

The Create Image API is accessed via `OpenAIClient.ImagesEndpoint.GenerateImageAsync()`.

```csharp
var api = new OpenAIClient();
var results = await api.ImageGenerationEndPoint.GenerateImageAsync("A house riding a velociraptor", 1, ImageSize.Small);
var image = results[0];
// result == Texture2D generated image
```

#### [Edit Image](https://beta.openai.com/docs/api-reference/images/create-edit)

Creates an edited or extended image given an original image and a prompt.

The Edit Image API is accessed via `OpenAIClient.ImagesEndPoint.CreateImageEditAsync()`:

```csharp
var api = new OpenAIClient();
var results = await api.ImagesEndPoint.CreateImageEditAsync(Path.GetFullPath(imageAssetPath), Path.GetFullPath(maskAssetPath), "A sunlit indoor lounge area with a pool containing a flamingo", 1, ImageSize.Small);
// results == file://path/to/image.png | Texture2D
```

#### [Create Image Variation](https://beta.openai.com/docs/api-reference/images/create-variation)

Creates a variation of a given image.

The Edit Image API is accessed via `OpenAIClient.ImagesEndPoint.CreateImageVariationAsync()`:

```csharp
var api = new OpenAIClient();
var results = await api.ImagesEndPoint.CreateImageVariationAsync(Path.GetFullPath(imageAssetPath), 1, ImageSize.Small);
// results == file://path/to/image.png | Texture2D
```

### [Moderations](https://beta.openai.com/docs/api-reference/moderations)

Given a input text, outputs if the model classifies it as violating OpenAI's content policy.

Related guide: [Moderations](https://beta.openai.com/docs/guides/moderation)

#### [Create Moderation](https://beta.openai.com/docs/api-reference/moderations/create)

Classifies if text violates OpenAI's Content Policy.

The Moderations endpoint can be accessed via `OpenAIClient.ModerationsEndpoint.GetModerationAsync()`:

```csharp
var api = new OpenAIClient();
var response = await api.ModerationsEndpoint.GetModerationAsync("I want to kill them.");
// response == true
```

## License

![CC-0 Public Domain](https://licensebuttons.net/p/zero/1.0/88x31.png)

This library is licensed CC-0, in the public domain.  You can use it for whatever you want, publicly or privately, without worrying about permission or licensing or whatever.  It's just a wrapper around the OpenAI API, so you still need to get access to OpenAI from them directly.  I am not affiliated with OpenAI and this library is not endorsed by them, I just have beta access and wanted to make a C# library to access it more easily.  Hopefully others find this useful as well.  Feel free to open a PR if there's anything you want to contribute.
