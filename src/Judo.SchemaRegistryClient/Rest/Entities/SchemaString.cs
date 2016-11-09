namespace Judo.SchemaRegistryClient.Rest.Entities
{

    public class SchemaStringModel
    {
        public string Schema{ get; set; }

        public static SchemaStringModel FromJson(string json)
        {
            return JsonUtils.Deserialize<SchemaStringModel>(json);
        }


        public string ToJson()
        {
            return JsonUtils.Serialize(this);
        }

    }
}