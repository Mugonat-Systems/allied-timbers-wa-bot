using Mugonat.Utils.Logging;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AlliedTimbers;

public class MvcApplication : HttpApplication
{
    public static Logger Logger
    {
        get
        {
            const string logsFolder = "wabotlogs";
            var rootPath = HttpContext.Current.Server.MapPath("~/");
            var logsPath = Path.Combine(rootPath, logsFolder);

            if (!Directory.Exists(logsPath)) Directory.CreateDirectory(logsPath);

            return new Logger(rootPath, logsFolder);
        }
    }

    protected void Application_Start()
    {
        AreaRegistration.RegisterAllAreas();
        GlobalConfiguration.Configure(WebApiConfig.Register);
        GlobalConfiguration.Configure(BotConfig.Register);
        FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        RouteConfig.RegisterRoutes(RouteTable.Routes);
        BundleConfig.RegisterBundles(BundleTable.Bundles);

        GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
               new CamelCasePropertyNamesContractResolver();
    }
}