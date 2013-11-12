using System;
using System.Web;
using System.Web.Mvc;

namespace dondEducar.Helpers
{
    public static class UrlHelperExtension
    {

        public static DateTime FromUnixTime(string unixTimeString)
        {
            var unixTime = Convert.ToInt64(unixTimeString);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        public static string ToPublicAsboluteUri(this UrlHelper urlHelper)
        {
            
            var httpContext = urlHelper.RequestContext.HttpContext;
            var url = httpContext.Request.Url;

            var uriBuilder = new UriBuilder
            {
                Host = url.Host,
                Path = url.AbsolutePath,
                Port = 80,
                Scheme = "http",
                Query = url.Query.Replace("?","")
            };

            if (httpContext.Request.IsLocal)
            {
                uriBuilder.Port = url.Port;
            }

            return uriBuilder.Uri.AbsoluteUri;
        }


        public static string ToPublicUrl(this HttpRequest request)
        {
            
            var uriBuilder = new UriBuilder
            {
                Host = request.Url.Host,
                Path = request.Url.AbsolutePath,
                Port = 80,
                Scheme = "http",
            };

            if (request.IsLocal)
            {
                uriBuilder.Port = request.Url.Port;
            }

            return uriBuilder.Uri.AbsoluteUri;
        }

        public static string ToPublicUrl(this UrlHelper urlHelper, Uri relativeUri)
        {
            var httpContext = urlHelper.RequestContext.HttpContext;

            var uriBuilder = new UriBuilder
            {
                Host = httpContext.Request.Url.Host,
                Path = "/",
                Port = 80,
                Scheme = "http",
            };

            if (httpContext.Request.IsLocal)
            {
                uriBuilder.Port = httpContext.Request.Url.Port;
            }

            return new Uri(uriBuilder.Uri, relativeUri).AbsoluteUri;
        }


    }
}