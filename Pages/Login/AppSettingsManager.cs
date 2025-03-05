using Newtonsoft.Json.Linq;

namespace mecanico_plus.Pages.Login
{
    public class AppSettingsManager
    {
        private readonly string _filePath;
        private readonly JObject _json;
        private string _connectionString = "";

        public AppSettingsManager()
        {
            _filePath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            _json = JObject.Parse(File.ReadAllText(_filePath));
        }

        public void definirCadenaConexion(string cadena)
        {
            _connectionString = cadena;
        }

        public void ConcatenarCadenaConexion(string server, string port, string database, string user, string password, string pooling, string searchPath)
        {
            _connectionString = "Server=" + server + ";Port=" + port + ";Database=" + database + ";User Id=" + user + ";Password=" + password + ";Pooling=" + pooling + ";SearchPath=" + searchPath + ";";
        }

        public string GetConnectionString(string name)
        {
            return _json["ConnectionStrings"][name].ToString();
        }

        public string GetAmbiente(string name)
        {
            return _json["Ambiente"][name].ToString();
        }

        public void SetConnectionString(string name)
        {
            _json["ConnectionStrings"][name] = _connectionString;
            Save();
        }

        private void Save()
        {
            File.WriteAllText(_filePath, _json.ToString());
        }
    }
}
