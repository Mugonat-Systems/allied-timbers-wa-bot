using System.Web;
using System.Web.Mvc;

namespace AlliedTimbers.Utilities;

public static class ObjectExtensions
{
    public static string Upload(this Controller controller, string name)
    {
        var uploaded = FileHandler.Upload(controller.Request.Files[name]);

        return uploaded
            .Replace(HttpContext.Current.Server.MapPath("/"), "/")
            .Replace("\\", "/");
    }
    
}