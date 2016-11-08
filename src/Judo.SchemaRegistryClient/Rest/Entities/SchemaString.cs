namespace Judo.SchemaRegistryClient.Rest.Entities
{
    using Newtonsoft.Json;

    public class SchemaStringModel
    {
        public string SchemaString{ get; set; }

        public static SchemaStringModel FromJson(string json)
        {
            return JsonConvert.DeserializeObject<SchemaStringModel>(json);
        }


        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}