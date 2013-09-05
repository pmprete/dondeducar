using System.Configuration;
using System.Web.Mvc;
using MongoDB.Driver;

namespace dondEducar.Controllers
{
    public abstract class BaseController : Controller
    {
        private readonly MongoClient _mongoClient = new MongoClient(GetMongoDbConnectionString());
        public MongoClient Client
        {
            get { return _mongoClient; }
        }

        private static string GetMongoDbConnectionString()
        {
            var connectionString = ConfigurationManager.AppSettings.Get("MONGOHQ_URL") ??
                ConfigurationManager.AppSettings.Get("MONGOLAB_URI") ??
                "mongodb://localhost";
            return connectionString;
        }
    }
}
