namespace Judo.SchemaRegistryClient.Rest.Entities.Requests
{

    public class RegisterSchemaRequest
    {
        public static RegisterSchemaRequest FromJson(string json)
        {
            return JsonUtils.Deserialize<RegisterSchemaRequest>(json);
        }

        public string Schema { get; set; }

        public string ToJson()
        {
            return JsonUtils.Serialize(this);
        }

        public override string ToString()
        {
            return $"{{schema={Schema}}}";
        }


        public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || !(o is RegisterSchemaRequest))
            {
                return false;
            }

            var that = (RegisterSchemaRequest)o;

            if (Schema != null ? !Schema.Equals(that.Schema) : that.Schema != null)
            {
                return false;
            }

            return true;
        }


        public override int GetHashCode()
        {
            int result = base.GetHashCode();
            result = 31 * result + (Schema != null ? Schema.GetHashCode() : 0);
            return result;

        }
    }
}