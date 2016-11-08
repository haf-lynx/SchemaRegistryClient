namespace Judo.SchemaRegistryClient.Rest.Entities
{

    using System;
    public class SchemaModel : IComparable<SchemaModel>
    {

        public string Subject {get;set;}
        public int Id {get;set;}
        public int Version {get;set;}
        public string Schema {get;set;}

        public override bool Equals (object obj)
        {
            if(Object.ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            var other = obj as SchemaModel;
            return this.Id == other.Id && this.Version == other.Version && this.Subject == other.Subject && this.Schema == other.Schema;
        }
        
        // override object.GetHashCode
        public override int GetHashCode()
        {
            var result = Subject.GetHashCode();
            result = 31 * result + Version;
            result = 31 * result + Id;
            result = 31 * result + Schema.GetHashCode();
            return result;
        }
        public int CompareTo(SchemaModel other)
        {
            var result = this.Subject.CompareTo(other.Subject);
            if (result != 0) {
            return result;
            }
            result = this.Version - other.Version;
            return result;
        }
    }
}