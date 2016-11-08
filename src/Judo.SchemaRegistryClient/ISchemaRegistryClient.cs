

using System.Threading.Tasks;
using Microsoft.Hadoop.Avro.Schema;

namespace Judo.SchemaRegistryClient
{
    interface ISchemaRegistryClient
    {
        Task<int> RegisterAsync(string subject, Schema schema);

        Task<Schema> GetByIDAsync(int id);

        Task<Schema> GetBySubjectAndIDAsync(string subject, int id);

        Task<SchemaMetadata> GetLatestSchemaMetadataAsync(string subject);

        Task<SchemaMetadata> GetSchemaMetadataAsync(string subject, int version);

        Task<int> GetVersionAsync(string subject, Schema schema);

        Task<bool> TestCompatibilityAsync(string subject, Schema schema);

        Task<string> UpdateCompatibilityAsync(string subject, string compatibility);

        Task<string> GetCompatibilityAsync(string subject);

        Task<string[]> GetAllSubjectsAsync();
    }
}