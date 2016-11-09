namespace Judo.SchemaRegistryClient.Rest.Entities.Requests
{
    public class RegisterSchemaResponse
    {
        public static RegisterSchemaResponse FromJson(string json)
        {
            return JsonUtils.Deserialize<RegisterSchemaResponse>(json);
        }

        public int Id {get;set;}

        public string ToJson()
        {
            return JsonUtils.Serialize(this);
        }
    }
}