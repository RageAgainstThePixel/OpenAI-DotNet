# C#/.NET SDK for accessing the OpenAI GPT-3 API

A simple C# .NET wrapper library to use with [OpenAI](https://openai.com/)'s GPT-3 API.  More context [on Roger Pincombe's blog](https://rogerpincombe.com/openai-dotnet-api).

## Requirements

This library is based on .NET Standard 2.0, so it should work across .NET Framework >=4.7.2 and .NET Core >= 3.0.  It should work across console apps, winforms, wpf, asp.net, etc (although I have not yet tested with asp.net).  It should work across Windows, Linux, and Mac, although I have only tested on Windows and Linux so far.

## Getting started

### Install from NuGet

Install package [`OpenAI` from Nuget](https://www.nuget.org/packages/OpenAI-DotNet/).  Here's how via commandline:

```powershell
Install-Package OpenAI-DotNet
```

### Quick Start

Uses the default authentication from the current directory, the default user directory or system environment variables

```csharp
OpenAI api = new OpenAI(Engine.Davinci);
```

### Authentication

There are 3 ways to provide your API keys, in order of precedence:

1. Pass keys directly to `Authentication(string key)` constructor
2. Set environment variables
3. Include a config file in the local directory or in your user directory named `.openai` and containing the line:

```shell
OPENAI_KEY=sk-aaaabbbbbccccddddd
```

You use the `Authentication` when you initialize the API as shown:

#### If you want to provide a key manually

```csharp
OpenAI api = new OpenAI("sk-mykeyhere");
```

#### Create a `Authentication` object manually

```chsarp
OpenAI api = new OpenAI(new Authentication("sk-secretkey"));
```

#### Use System Environment Variables

> Use `OPENAI_KEY` or `OPENAI_SECRET_KEY` specify a key defined in the system's local environment:

```chsarp
OpenAI api = new OpenAI(Authentication LoadFromEnv());
```

#### Load key from specified directory

> Attempts to load api keys from a configuration file, by default `.openai` in the current directory, optionally traversing up the directory tree.

```chsarp
OpenAI api = new OpenAI(Authentication.LoadFromDirectory("C:\\MyProject"));;
```

### Completions

The Completion API is accessed via `OpenAI.Completions`:

```csharp
var result = await api.Completions.CreateCompletionAsync("One Two Three One Two", temperature: 0.1, engine: Engine.Davinci);
Console.WriteLine(result);
```

 Get the `CompletionResult` (which is mostly metadata), use its implicit string operator to get the text if all you want is the completion choice.

#### Streaming

Streaming allows you to get results are they are generated, which can help your application feel more responsive, especially on slow models like Davinci.

```csharp
var api = new OpenAI();
await api.Completions.StreamCompletionAsync(result =>
{
    foreach (var choice in result.Completions)
    {
        Console.WriteLine(choice);
    }
}, "My name is Roger and I am a principal software engineer at Salesforce.  This is my resume:", max_tokens: 200, temperature: 0.5, presencePenalty: 0.1, frequencyPenalty: 0.1, engine: Engine.Davinci);
```

The result.Completions

Or if using [`IAsyncEnumerable{T}`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1?view=net-5.0) ([C# 8.0+](https://docs.microsoft.com/en-us/archive/msdn-magazine/2019/november/csharp-iterating-with-async-enumerables-in-csharp-8))

```csharp
var api = new OpenAI();
await foreach (var token in api.Completions.StreamCompletionEnumerableAsync("My name is Roger and I am a principal software engineer at Salesforce.  This is my resume:", max_tokens: 200, temperature: 0.5, presencePenalty: 0.1, frequencyPenalty: 0.1, engine: Engine.Davinci))
{
  Console.Write(token);
}
```

### Document Search

The Search API is accessed via `OpenAI.Search`:

#### You can get all results as a dictionary using

```csharp
var api = new OpenAI();
string query = "Washington DC";
string[] documents = { "Canada", "China", "USA", "Spain" };

Dictionary<string, double> results = await api.Search.GetSearchResultsAsync(query, documents, Engine.Curie);
// result["USA"] == 294.22
// result["Spain"] == 73.81
```

> The returned dictionary maps documents to scores.

#### You can get only the best match using

```csharp
var api = new OpenAI();
string query = "Washington DC";
string[] documents = { "Canada", "China", "USA", "Spain" };
string result = await api.Search.GetBestMatchAsync(query, documents, Engine.Curie);
// result == "USA"
```

> The returned document result string.

#### And if you only want the best match but still want to know the score, use

```csharp
var api = new OpenAI();
string query = "Washington DC";
string[] documents = { "Canada", "China", "USA", "Spain" };
Tuple<string, double> result = await GetBestMatchWithScoreAsync(query, documents, Engine.Curie);
// (result, score) == "USA", 294.22
```

## License

![CC-0 Public Domain](https://licensebuttons.net/p/zero/1.0/88x31.png)

This library is licensed CC-0, in the public domain.  You can use it for whatever you want, publicly or privately, without worrying about permission or licensing or whatever.  It's just a wrapper around the OpenAI API, so you still need to get access to OpenAI from them directly.  I am not affiliated with OpenAI and this library is not endorsed by them, I just have beta access and wanted to make a C# library to access it more easily.  Hopefully others find this useful as well.  Feel free to open a PR if there's anything you want to contribute.
