
namespace Judo.SchemaRegistryClient.Rest.Utils
{
    using System;
    public class UrlList
    {  
        private int _index = 0;

        private readonly object _lock = new object();
        private readonly string[] _urls = null;

        public UrlList (params string[] urls)
        {
            if(urls == null || urls.Length == 0)
            {
                throw new ArgumentException("UrlList must be passed at least one url");
            }
            _urls = urls;
        }

        public string Current 
        {
            get
            {
                return _urls[_index];
            }
        }

        public void Fail()
        {
            lock(_lock)
            {
                _index = (_index+1) % _urls.Length;
            }
        }

        public int Size 
        {
            get
            {
                return _urls.Length;
            }
        }
    }
}