namespace Judo.SchemaRegistryClient.Rest.Entities.Requests
{
    using Newtonsoft.Json;

    public class RegisterSchemaResponse
    {
        public static RegisterSchemaResponse FromJson(string json)
        {
            return JsonConvert.DeserializeObject<RegisterSchemaResponse>(json);
        }

        public int Id {get;set;}

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}