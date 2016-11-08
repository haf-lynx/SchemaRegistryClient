namespace Judo.SchemaRegistryClient
{
    public class SchemaMetadata
    {

        public SchemaMetadata(int id, int version, string schema) 
        {
            this.Id = id;
            this.Version = version;
            this.Schema = schema;
        }

        public int Id { get; set; }
        public int Version { get; set; }
        public string Schema { get; set; }
    }
}