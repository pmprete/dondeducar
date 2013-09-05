using System.Configuration;
using System.Web.Mvc;
using MongoDB.Driver;

namespace dondEducar.Controllers
{
    public abstract class BaseController : Controller
    {
        private readonly MongoUrl _mongoUrl = new MongoUrl(ConnectionString());
        public MongoUrl Url
        {
            get { return _mongoUrl; }
        }

        private readonly MongoClient _mongoClient = new MongoClient(ConnectionString());
        public MongoClient Client
        {
            get { return _mongoClient; }
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
