using System.Configuration;
using System.Web.Mvc;
using MongoDB.Driver;

namespace dondEducar.Controllers
{
    public abstract class BaseController : Controller
    {
        protected BaseController()
        {
            var url = new MongoUrl(ConnectionString());
            var client = new MongoClient(url);
            var server = client.GetServer();
            _mongoDatabase = server.GetDatabase(url.DatabaseName);
        }

        private readonly MongoDatabase _mongoDatabase;
        public MongoDatabase Database
        {
            get { return _mongoDatabase; }
        }

        private static string ConnectionString()
        {
            var connectionString = ConfigurationManager.AppSettings.Get("MONGOHQ_URL") ??
                ConfigurationManager.AppSettings.Get("MONGOLAB_URI") ??
                "mongodb://localhost/test";
            return connectionString;
        }
    }
}
