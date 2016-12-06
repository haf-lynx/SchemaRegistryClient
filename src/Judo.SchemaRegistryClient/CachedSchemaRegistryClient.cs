using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Judo.SchemaRegistryClient.Rest;
using Judo.SchemaRegistryClient.Rest.Entities;
using Microsoft.Hadoop.Avro.Schema;

namespace Judo.SchemaRegistryClient
{
    public class CachedSchemaRegistryClient : ISchemaRegistryClient
    {
        private const string DefaultKey = "___DEFAULT___KEY___";
        private readonly RestService _restService;
        private readonly int _identityDictionaryCapacity;
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, int>> _schemaCache;
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<int, Schema>> _idCache;
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, int>> _versionCache;

        public CachedSchemaRegistryClient(String baseUrl, int identityMapCapacity) : this(new RestService(baseUrl), identityMapCapacity)
        {
        }

        public CachedSchemaRegistryClient(String[] baseUrls, int identityMapCapacity) : this(new RestService(baseUrls), identityMapCapacity)
        {
        }

        public CachedSchemaRegistryClient(RestService restService, int identityMapCapacity)
        {
            this._identityDictionaryCapacity = identityMapCapacity;
            this._schemaCache = new ConcurrentDictionary<String, ConcurrentDictionary<string, int>>();
            this._idCache = new ConcurrentDictionary<String, ConcurrentDictionary<int, Schema>>();
            this._versionCache = new ConcurrentDictionary<String, ConcurrentDictionary<string, int>>();
            this._restService = restService;
            this._idCache.GetOrAdd(DefaultKey,k => new ConcurrentDictionary<int, Schema>());
        }

        private Task<int> RegisterAndGetIdAsync(String subject, Schema schema)
        {
            return _restService.RegisterSchemaAsync(schema.ToString(), subject);
        }

        private async Task<Schema> GetSchemaByIdFromRegistryAsync(int id)
        {
            var restSchema = await _restService.GetIdAsync(id);
            return new JsonSchemaBuilder().BuildSchema(restSchema.Schema);
        }

        private async Task<int> GetVersionFromRegistryAsync(String subject, Schema schema)
        {
            var response = await _restService.LookupSubjectVersionAsync(schema.ToString(), subject);
            return response.Version;
        }

        public async Task<int> RegisterAsync(string subject, Schema schema)
        {
            ConcurrentDictionary<string, int> schemaIdDictionary;
            if (_schemaCache.ContainsKey(subject))
            {
                schemaIdDictionary = _schemaCache[subject];
            }
            else
            {
                schemaIdDictionary = new ConcurrentDictionary<string, int>();
                _schemaCache.GetOrAdd(subject, schemaIdDictionary);
            }

            if (schemaIdDictionary.ContainsKey(schema.ToString()))
            {
                return schemaIdDictionary[schema.ToString()];
            }
            else
            {
                if (schemaIdDictionary.Count >= _identityDictionaryCapacity)
                {
                    throw new Exception("Too many schema objects created for " + subject + "!");
                }
                var id = await RegisterAndGetIdAsync(subject, schema);
                schemaIdDictionary.GetOrAdd(schema.ToString(), id);
                _idCache[DefaultKey].GetOrAdd(id, schema);
                return id;
            }
        }

        public Task<Schema> GetByIDAsync(int id)
        {
            return GetBySubjectAndIDAsync(null, id);
        }

        public async Task<Schema> GetBySubjectAndIDAsync(string subject, int id)
        {

            ConcurrentDictionary<int, Schema> idSchemaDictionary;
            if (_idCache.ContainsKey(subject ?? DefaultKey))
            {
                idSchemaDictionary = _idCache[subject ?? DefaultKey];
            }
            else
            {
                idSchemaDictionary = new ConcurrentDictionary<int, Schema>();
                _idCache.GetOrAdd(subject, idSchemaDictionary);
            }

            if (idSchemaDictionary.ContainsKey(id))
            {
                return idSchemaDictionary[id];
            }
            else
            {
                var schema = await GetSchemaByIdFromRegistryAsync(id);
                idSchemaDictionary.GetOrAdd(id, schema);
                return schema;
            }
        }

        public async Task<SchemaMetadata> GetLatestSchemaMetadataAsync(string subject)
        {
            var response = await _restService.GetLatestVersionAsync(subject);
            return new SchemaMetadata(response.Id, response.Version, response.Schema);
        }

        public async Task<SchemaMetadata> GetSchemaMetadataAsync(string subject, int version)
        {
            var response = await _restService.GetVersionAsync(subject, version);
            return new SchemaMetadata(response.Id, response.Version, response.Schema);
        }

        public async Task<int> GetVersionAsync(string subject, Schema schema)
        {
            ConcurrentDictionary<string, int> schemaVersionDictionary;
            if (_versionCache.ContainsKey(subject))
            {
                schemaVersionDictionary = _versionCache[subject];
            }
            else
            {
                schemaVersionDictionary = new ConcurrentDictionary<string, int>();
                _versionCache.GetOrAdd(subject, schemaVersionDictionary);
            }

            if (schemaVersionDictionary.ContainsKey(schema.ToString()))
            {
                return schemaVersionDictionary[schema.ToString()];
            }
            else
            {
                if (schemaVersionDictionary.Count >= _identityDictionaryCapacity)
                {
                    throw new Exception("Too many schema objects created for " + subject + "!");
                }
                var version = await GetVersionFromRegistryAsync(subject, schema);
                schemaVersionDictionary.GetOrAdd(schema.ToString(), version);
                return version;
            }
        }

        public Task<bool> TestCompatibilityAsync(string subject, Schema schema)
        {
            return _restService.TestCompatibilityAsync(schema.ToString(), subject, "latest");
        }

        public async Task<string> UpdateCompatibilityAsync(string subject, string compatibility)
        {
            var response = await _restService.UpdateCompatibilityAsync(compatibility, subject);
            return response.Compatibility;
        }

        public async Task<string> GetCompatibilityAsync(string subject)
        {
            var response = await _restService.GetConfigAsync(subject);
            return response.Compatibility;
        }

        public Task<string[]> GetAllSubjectsAsync()
        {
            return _restService.GetAllSubjectsAsync();
        }
    }
}