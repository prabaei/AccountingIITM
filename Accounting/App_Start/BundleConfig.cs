using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace Accounting.App_Start
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/autocomplete").Include("~/Scripts/autocomplete/jquery-ui.min.js"));
            ////script dialog javascript
            bundles.Add(new ScriptBundle("~/bundles/Dialog").Include("~/Scripts/Dialog/jquery-ui.js"));

            bundles.Add(new ScriptBundle("~/bundles/DatePicker").Include("~/Scripts/DatePicker/jquery-ui.js"));

            bundles.Add(new ScriptBundle("~/bundles/sortable").Include("~/Scripts/Sortable/jquery-ui.js"));
            bundles.Add(new ScriptBundle("~/bundles/selectable").Include("~/Scripts/selectable/jquery-ui.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/bundles/autocompletecss").Include("~/Content/autocomplete/*.css", new CssRewriteUrlTransform()));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));
            ////css for dialog box
            bundles.Add(new StyleBundle("~/bundles/Dialogcss").Include("~/Content/dialog/jquery-ui.css", new CssRewriteUrlTransform()));

            bundles.Add(new StyleBundle("~/bundles/DatePickercss").Include("~/Content/DatePicker/jquery-ui.css", new CssRewriteUrlTransform()));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/bundles/sortablecss").Include("~/Content/Sortable/jquery-ui.css",new CssRewriteUrlTransform()));
            bundles.Add(new StyleBundle("~/bundles/selectablecss").Include("~/Content/selectable/jquery-ui.css", new CssRewriteUrlTransform()));
        }
    }
}