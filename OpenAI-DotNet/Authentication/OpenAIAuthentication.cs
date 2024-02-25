// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using System.Text.Json;

namespace OpenAI
{
    /// <summary>
    /// Represents authentication to the OpenAI API endpoint
    /// </summary>
    public sealed class OpenAIAuthentication
    {
        private const string OPENAI_KEY = "OPENAI_KEY";
        private const string OPENAI_API_KEY = "OPENAI_API_KEY";
        private const string OPENAI_SECRET_KEY = "OPENAI_SECRET_KEY";
        private const string TEST_OPENAI_SECRET_KEY = "TEST_OPENAI_SECRET_KEY";
        private const string OPENAI_ORGANIZATION_ID = "OPENAI_ORGANIZATION_ID";
        private const string OPEN_AI_ORGANIZATION_ID = "OPEN_AI_ORGANIZATION_ID";
        private const string ORGANIZATION = "ORGANIZATION";

        private readonly AuthInfo authInfo;

        /// <summary>
        /// The API key, required to access the API endpoint.
        /// </summary>
        public string ApiKey => authInfo.ApiKey;

        /// <summary>
        /// For users who belong to multiple organizations, you can pass a header to specify which organization is used for an API request.
        /// Usage from these API requests will count against the specified organization's subscription quota.
        /// </summary>
        public string OrganizationId => authInfo.OrganizationId;

        /// <summary>
        /// Allows implicit casting from a string, so that a simple string API key can be provided in place of an instance of <see cref="OpenAIAuthentication"/>.
        /// </summary>
        /// <param name="key">The API key to convert into a <see cref="OpenAIAuthentication"/>.</param>
        public static implicit operator OpenAIAuthentication(string key) => new(key);

        private OpenAIAuthentication(AuthInfo authInfo) => this.authInfo = authInfo;

        /// <summary>
        /// Instantiates a new Authentication object with the given <paramref name="apiKey"/>, which may be <see langword="null"/>.
        /// </summary>
        /// <param name="apiKey">The API key, required to access the API endpoint.</param>
        public OpenAIAuthentication(string apiKey) => authInfo = new AuthInfo(apiKey);

        /// <summary>
        /// Instantiates a new Authentication object with the given <paramref name="apiKey"/>, which may be <see langword="null"/>.
        /// </summary>
        /// <param name="apiKey">The API key, required to access the API endpoint.</param>
        /// <param name="organization">
        /// For users who belong to multiple organizations, you can pass a header to specify which organization is used for an API request.
        /// Usage from these API requests will count against the specified organization's subscription quota.
        /// </param>
        public OpenAIAuthentication(string apiKey, string organization) => authInfo = new AuthInfo(apiKey, organization);

        private static OpenAIAuthentication cachedDefault;

        /// <summary>
        /// The default authentication to use when no other auth is specified.
        /// This can be set manually, or automatically loaded via environment variables or a config file.
        /// <seealso cref="LoadFromEnv"/><seealso cref="LoadFromDirectory"/>
        /// </summary>
        public static OpenAIAuthentication Default
        {
            get
            {
                if (cachedDefault != null)
                {
                    return cachedDefault;
                }

                var auth = LoadFromDirectory() ??
                           LoadFromDirectory(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)) ??
                           LoadFromEnv();
                cachedDefault = auth ?? throw new UnauthorizedAccessException("Failed to load a valid API Key!");
                return auth;
            }
            internal set => cachedDefault = value;
        }

        /// <summary>
        /// Attempts to load api keys from environment variables, as "OPENAI_KEY" (or "OPENAI_SECRET_KEY", for backwards compatibility)
        /// </summary>
        /// <param name="organizationId">
        /// For users who belong to multiple organizations, you can pass a header to specify which organization is used for an API request.
        /// Usage from these API requests will count against the specified organization's subscription quota.
        /// </param>
        /// <returns>
        /// Returns the loaded <see cref="OpenAIAuthentication"/> any api keys were found,
        /// or <see langword="null"/> if there were no matching environment vars.
        /// </returns>
        public static OpenAIAuthentication LoadFromEnv(string organizationId = null)
        {
            var apiKey = Environment.GetEnvironmentVariable(OPENAI_KEY);

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                apiKey = Environment.GetEnvironmentVariable(OPENAI_API_KEY);
            }

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                apiKey = Environment.GetEnvironmentVariable(OPENAI_SECRET_KEY);
            }

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                apiKey = Environment.GetEnvironmentVariable(TEST_OPENAI_SECRET_KEY);
            }

            if (string.IsNullOrWhiteSpace(organizationId))
            {
                organizationId = Environment.GetEnvironmentVariable(OPEN_AI_ORGANIZATION_ID);
            }

            if (string.IsNullOrWhiteSpace(organizationId))
            {
                organizationId = Environment.GetEnvironmentVariable(OPENAI_ORGANIZATION_ID);
            }

            if (string.IsNullOrWhiteSpace(organizationId))
            {
                organizationId = Environment.GetEnvironmentVariable(ORGANIZATION);
            }

            return string.IsNullOrEmpty(apiKey) ? null : new OpenAIAuthentication(apiKey, organizationId);
        }

        /// <summary>
        /// Attempts to load api keys from a specified configuration file.
        /// </summary>
        /// <param name="path">The specified path to the configuration file.</param>
        /// <returns>
        /// Returns the loaded <see cref="OpenAIAuthentication"/> any api keys were found,
        /// or <see langword="null"/> if it was not successful in finding a config
        /// (or if the config file didn't contain correctly formatted API keys)
        /// </returns>
        public static OpenAIAuthentication LoadFromPath(string path)
            => LoadFromDirectory(Path.GetDirectoryName(path), Path.GetFileName(path), false);

        /// <summary>
        /// Attempts to load api keys from a configuration file, by default ".openai" in the current directory,
        /// optionally traversing up the directory tree.
        /// </summary>
        /// <param name="directory">
        /// The directory to look in, or <see langword="null"/> for the current directory.
        /// </param>
        /// <param name="filename">
        /// The filename of the config file.
        /// </param>
        /// <param name="searchUp">
        /// Whether to recursively traverse up the directory tree if the <paramref name="filename"/> is not found in the <paramref name="directory"/>.
        /// </param>
        /// <returns>
        /// Returns the loaded <see cref="OpenAIAuthentication"/> any api keys were found,
        /// or <see langword="null"/> if it was not successful in finding a config
        /// (or if the config file didn't contain correctly formatted API keys)
        /// </returns>
        public static OpenAIAuthentication LoadFromDirectory(string directory = null, string filename = ".openai", bool searchUp = true)
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                directory = Environment.CurrentDirectory;
            }

            AuthInfo authInfo = null;

            var currentDirectory = new DirectoryInfo(directory);

            while (authInfo == null && currentDirectory.Parent != null)
            {
                var filePath = Path.Combine(currentDirectory.FullName, filename);

                if (File.Exists(filePath))
                {
                    try
                    {
                        authInfo = JsonSerializer.Deserialize<AuthInfo>(File.ReadAllText(filePath));
                        break;
                    }
                    catch (Exception)
                    {
                        // try to parse the old way for backwards support.
                    }

                    var lines = File.ReadAllLines(filePath);
                    string apiKey = null;
                    string organization = null;

                    foreach (var line in lines)
                    {
                        var parts = line.Split('=', ':');

                        for (var i = 0; i < parts.Length - 1; i++)
                        {
                            var part = parts[i];
                            var nextPart = parts[i + 1];

                            switch (part)
                            {
                                case OPENAI_KEY:
                                case OPENAI_API_KEY:
                                case OPENAI_SECRET_KEY:
                                case TEST_OPENAI_SECRET_KEY:
                                    apiKey = nextPart.Trim();
                                    break;
                                case ORGANIZATION:
                                case OPEN_AI_ORGANIZATION_ID:
                                case OPENAI_ORGANIZATION_ID:
                                    organization = nextPart.Trim();
                                    break;
                            }
                        }
                    }

                    authInfo = new AuthInfo(apiKey, organization);
                }

                if (searchUp)
                {
                    currentDirectory = currentDirectory.Parent;
                }
                else
                {
                    break;
                }
            }

            if (authInfo == null ||
                string.IsNullOrEmpty(authInfo.ApiKey))
            {
                return null;
            }

            return new OpenAIAuthentication(authInfo);
        }
    }
}
