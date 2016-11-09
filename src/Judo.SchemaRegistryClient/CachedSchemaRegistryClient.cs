using System;
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
        private readonly IDictionary<string, Dictionary<Schema, int>> _schemaCache;
        private readonly IDictionary<string, Dictionary<int, Schema>> _idCache;
        private readonly IDictionary<string, Dictionary<Schema, int>> _versionCache;

        public CachedSchemaRegistryClient(String baseUrl, int identityMapCapacity) : this(new RestService(baseUrl), identityMapCapacity)
        {
        }

        public CachedSchemaRegistryClient(String[] baseUrls, int identityMapCapacity) : this(new RestService(baseUrls), identityMapCapacity)
        {
        }

        public CachedSchemaRegistryClient(RestService restService, int identityMapCapacity)
        {
            this._identityDictionaryCapacity = identityMapCapacity;
            this._schemaCache = new Dictionary<String, Dictionary<Schema, int>>();
            this._idCache = new Dictionary<String, Dictionary<int, Schema>>();
            this._versionCache = new Dictionary<String, Dictionary<Schema, int>>();
            this._restService = restService;
            this._idCache.Add(DefaultKey, new Dictionary<int, Schema>());
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
            Dictionary<Schema, int> schemaIdDictionary;
            if (_schemaCache.ContainsKey(subject))
            {
                schemaIdDictionary = _schemaCache[subject];
            }
            else
            {
                schemaIdDictionary = new Dictionary<Schema, int>();
                _schemaCache.Add(subject, schemaIdDictionary);
            }

            if (schemaIdDictionary.ContainsKey(schema))
            {
                return schemaIdDictionary[schema];
            }
            else
            {
                if (schemaIdDictionary.Count >= _identityDictionaryCapacity)
                {
                    throw new Exception("Too many schema objects created for " + subject + "!");
                }
                var id = await RegisterAndGetIdAsync(subject, schema);
                schemaIdDictionary.Add(schema, id);
                _idCache[DefaultKey].Add(id, schema);
                return id;
            }
        }

        public Task<Schema> GetByIDAsync(int id)
        {
            return GetBySubjectAndIDAsync(null, id);
        }

        public async Task<Schema> GetBySubjectAndIDAsync(string subject, int id)
        {

            Dictionary<int, Schema> idSchemaDictionary;
            if (_idCache.ContainsKey(subject ?? DefaultKey))
            {
                idSchemaDictionary = _idCache[subject ?? DefaultKey];
            }
            else
            {
                idSchemaDictionary = new Dictionary<int, Schema>();
                _idCache.Add(subject, idSchemaDictionary);
            }

            if (idSchemaDictionary.ContainsKey(id))
            {
                return idSchemaDictionary[id];
            }
            else
            {
                var schema = await GetSchemaByIdFromRegistryAsync(id);
                idSchemaDictionary.Add(id, schema);
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
            Dictionary<Schema, int> schemaVersionDictionary;
            if (_versionCache.ContainsKey(subject))
            {
                schemaVersionDictionary = _versionCache[subject];
            }
            else
            {
                schemaVersionDictionary = new Dictionary<Schema, int>();
                _versionCache.Add(subject, schemaVersionDictionary);
            }

            if (schemaVersionDictionary.ContainsKey(schema))
            {
                return schemaVersionDictionary[schema];
            }
            else
            {
                if (schemaVersionDictionary.Count >= _identityDictionaryCapacity)
                {
                    throw new Exception("Too many schema objects created for " + subject + "!");
                }
                var version = await GetVersionFromRegistryAsync(subject, schema);
                schemaVersionDictionary.Add(schema, version);
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