namespace UnityExtension.Api
{
    public class ApiAuthorization : SingletonBehaviour<ApiAuthorization>
    {
        private string _idSession = "";
        private string _idUser = "";
        private string _password = "";

        public string IdSession { get => _idSession; }
        public string IdUser { get => _idUser; }
        public string Password { get => _password; }
    }
}
