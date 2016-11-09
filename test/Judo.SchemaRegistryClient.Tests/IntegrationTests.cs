using Xunit;
using Microsoft.Hadoop.Avro;

namespace Judo.SchemaRegistryClient.Tests
{
    public class IntegrationTests
    {
        [Fact]
        public async void CanRegisterSchema() 
        {
            var client = new CachedSchemaRegistryClient("http://localhost:8081", 200);
            var schema = AvroSerializer.Create<TestMessageModel>(new AvroSerializerSettings(){Resolver = new AvroPublicMemberContractResolver() }).ReaderSchema;
            var response = await client.RegisterAsync("mytopic-value", schema);
            
            var returnedSchema = await client.GetByIDAsync(response);
            Assert.NotNull(returnedSchema);
            Assert.True(response > 0);
        }
    }

    public class TestMessageModel
    {
        public int Id {get;set;}

        public string SomeData {get;set;}
    }
}
