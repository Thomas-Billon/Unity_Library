using UnityEngine;

namespace UnityExtension.Api
{
    public class ApiConfig : SingletonBehaviour<ApiConfig>
    {
        [SerializeField]
        private string _urlBase = "";

        [SerializeField]
        private string _version = "";

        public string UrlBase { get => _urlBase; }
        public string Version { get => _version; }
    }
}
