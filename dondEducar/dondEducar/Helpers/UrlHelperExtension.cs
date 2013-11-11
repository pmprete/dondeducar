using System;
using System.Web.Mvc;

namespace dondEducar.Helpers
{
    public static class UrlHelperExtension
    {
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