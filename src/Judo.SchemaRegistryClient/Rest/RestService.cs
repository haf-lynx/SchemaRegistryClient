
namespace Judo.SchemaRegistryClient.Rest
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Entities;
    using Entities.Requests;
    using Exceptions;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Utils;

    public class RestService
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly UrlList _baseUrls;
        public RestService(UrlList baseUrls)
        {
            this._baseUrls = baseUrls;
        }

        public RestService(params string[] baseUrls) : this(new UrlList(baseUrls))
        {
        }

        public UrlList BaseUrls => _baseUrls;

        public ILogger<RestService> Logger { get; set; }

        public Task<SchemaModel> LookupSubjectVersionAsync(string schemaString, string subject)
        {
            var request = new RegisterSchemaRequest();
            request.Schema = schemaString;
            return LookupSubjectVersionAsync(request, subject);
        }

        public Task<SchemaModel> LookupSubjectVersionAsync(RegisterSchemaRequest registerSchemaRequest, string subject)
        {
            var path = $"/subjects/{subject}";
            return DoRequestAsync<SchemaModel>(path, HttpMethod.Post, new StringContent(registerSchemaRequest.ToJson()));
        }

        public Task<int> RegisterSchemaAsync(String schemaString, String subject)
        {
            var request = new RegisterSchemaRequest();
            request.Schema = schemaString;
            return RegisterSchemaAsync(request, subject);
        }

        public async Task<int> RegisterSchemaAsync(RegisterSchemaRequest registerSchemaRequest, String subject)
        {
            var path = $"/subjects/{subject}/versions";

            var response = await DoRequestAsync<RegisterSchemaResponse>(path, HttpMethod.Post, new StringContent(registerSchemaRequest.ToJson()));
            return response.Id;
        }

        public Task<bool> TestCompatibilityAsync(String schemaString, String subject, String version)
        {
            RegisterSchemaRequest request = new RegisterSchemaRequest();
            request.Schema = schemaString;
            return TestCompatibilityAsync(request, subject, version);
        }


        public async Task<bool> TestCompatibilityAsync(RegisterSchemaRequest registerSchemaRequest,
                                         String subject,
                                         String version)
        {
            var path = $"/compatibility/subjects/{subject}/versions/{version}";

            var response = await DoRequestAsync<CompatibilityCheckResponse>(path, HttpMethod.Post, new StringContent(registerSchemaRequest.ToJson()));

            return response.IsCompatible;
        }

        public Task<ConfigUpdateRequest> UpdateCompatibilityAsync(String compatibility, String subject)
        {
            ConfigUpdateRequest request = new ConfigUpdateRequest();
            request.Compatibility = compatibility;
            return UpdateConfigAsync(request, subject);
        }

        public Task<ConfigUpdateRequest> UpdateConfigAsync(
                                                ConfigUpdateRequest configUpdateRequest,
                                                String subject)
        {
            var path = subject != null ? "/config/{subject}" : "/config";
            return DoRequestAsync<ConfigUpdateRequest>(path, HttpMethod.Put, new StringContent(configUpdateRequest.ToJson()));
        }

        public Task<Config> GetConfigAsync(String subject)
        {
            var path = subject != null ? "/config/{subject}" : "/config";
            return DoRequestAsync<Config>(path, HttpMethod.Get);
        }

        public Task<SchemaStringModel> GetIdAsync(int id)
        {
            var path = "/schemas/ids/{id}";
            return DoRequestAsync<SchemaStringModel>(path, HttpMethod.Get);
        }

        public Task<SchemaModel> GetVersionAsync(String subject, int version)
        {
            var path = $"/subjects/{subject}/versions/{version}";
            return DoRequestAsync<SchemaModel>(path, HttpMethod.Get);
        }

        public Task<SchemaModel> GetLatestVersionAsync(String subject)
        {
            var path = $"/subjects/{subject}/versions/latest";

            return DoRequestAsync<SchemaModel>(path, HttpMethod.Get);
        }

        public Task<int[]> GetAllVersionsAsync(String subject)
        {
            var path = $"/subjects/{subject}/versions";

            return DoRequestAsync<int[]>(path, HttpMethod.Get);
        }

        public Task<string[]> GetAllSubjectsAsync()
        {
            return DoRequestAsync<string[]>("/subjects", HttpMethod.Get);
        }

        private async Task<T> DoRequestAsync<T>(string path, HttpMethod method, HttpContent content = null)
        {

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(_baseUrls.Current + path),
                Method = method,
                Content = content
            };
            content.Headers.ContentType = new MediaTypeHeaderValue(Versions.SCHEMA_REGISTRY_V1_JSON_WEIGHTED);

            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return default(T);
                }

                var body = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(body, new JsonSerializerSettings()
                {
                    ContractResolver = new DefaultContractResolver()
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    }
                });
            }
            else
            {
                ErrorMessage errorMessage;
                try
                {
                    errorMessage = JsonConvert.DeserializeObject<ErrorMessage>(response.ReasonPhrase);
                }
                catch (Exception ex)
                {
                    errorMessage = new ErrorMessage(50005, ex.Message);
                }

                throw new RestClientException(errorMessage.Message, (int)response.StatusCode, errorMessage.ErrorCode);
            }
        }
    }
}