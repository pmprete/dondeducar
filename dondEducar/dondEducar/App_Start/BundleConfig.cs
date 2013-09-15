using System.Web;
using System.Web.Optimization;

namespace dondEducar
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {

            //***************************** CSS *************************************

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.*"));
            
            bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
                       "~/Content/bootstrap.css",
                       "~/Content/bootstrap-theme.css"));

            bundles.Add(new StyleBundle("~/Content/slickgrid").Include(
                       "~/Content/slick.grid.css"));

            bundles.Add(new StyleBundle("~/Content/metro").Include(
                      "~/Content/m-buttons.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

            
            //****************************** JS *************************************
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui-{version}.js",
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

    
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                       "~/Scripts/bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/slickgrid").Include(
                        "~/Scripts/jquery.event.drag.js",
                       "~/Scripts/SlickGrid/slick.*",
                       "~/Scripts/SlickGrid/Controls/slick.*"    
                        ));

            bundles.Add(new ScriptBundle("~/bundles/underscore").Include(
                      "~/Scripts/underscore-min.js"));

            bundles.Add(new ScriptBundle("~/bundles/educar").Include(
                       "~/Scripts/educar.js"));

           
        }
    }
}