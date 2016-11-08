namespace Judo.SchemaRegistryClient.Rest.Entities
{

    public class Config
    {

        public string Compatibility { get; set; }

        public Config(string compatibility)
        {
            this.Compatibility = compatibility;
        }

        public Config()
        {
        }


        public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || !(o is Config))
            {
                return false;
            }

            Config that = (Config)o;

            if (!this.Compatibility.Equals(that.Compatibility))
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return 31 * Compatibility.GetHashCode();
        }


        public override string ToString()
        {
            return $"{{compatibilityLevel={this.Compatibility}}}";
        }
    }
}