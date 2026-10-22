using System.Web;
using System.Web.Optimization;

namespace CMS
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                       "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Contenttest/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            //Base css 
            bundles.Add(new StyleBundle("~/bundles/basecss").Include(
                                      "~/Content/assets/plugins/jquery-ui/themes/base/minified/jquery-ui.min.css",
                                      "~/Content/assets/plugins/bootstrap/css/bootstrap.min.css",
                                      "~/Content/assets/plugins/font-awesome/css/font-awesome.min.css",
                                      "~/Content/assets/plugins/ionicons/css/ionicons.min.css",
                                      "~/Content/assets/css/animate.min.css",
                                      "~/Content/assets/css/style.min.css",
                                      "~/Content/assets/css/style-responsive.min.css",
                                      "~/Content/assets/css/theme/default.css",
                                      "~/Content/assets/plugins/bootstrap-sweetalert/sweetalert.css"));
            //Base header js 
            bundles.Add(new ScriptBundle("~/bundles/headerbasejs").Include(
                                      "~/Content/assets/plugins/pace/pace.min.js"
                ));
            //Base footer js
            bundles.Add(new ScriptBundle("~/bundles/footerbasejs").Include(
                                     "~/Content/assets/plugins/jquery/jquery-1.9.1.min.js",
                                     "~/Content/assets/plugins/jquery/jquery-migrate-1.1.0.min.js",
                                     "~/Content/assets/plugins/jquery-ui/ui/minified/jquery-ui.min.js",
                                     "~/Content/assets/plugins/bootstrap/js/bootstrap.min.js",
                                     "~/Content/assets/plugins/slimscroll/jquery.slimscroll.min.js",
                                     "~/Content/assets/plugins/jquery-cookie/jquery.cookie.js", "~/Content/assets/plugins/bootstrap-sweetalert/sweetalert.min.js"
                                     ));

            //page begin js level
            bundles.Add(new ScriptBundle("~/bundle/pagebeginjs").Include(
                                    "~/Content/assets/js/apps.min.js"
                ));

            ////this css only for login page
            //bundles.Add(new StyleBundle("~/Login/css").Include(
            //          "~/Content/bower_components/bootstrap/dist/css/bootstrap.min.css",
            //          "~/Content/bower_components/font-awesome/css/font-awesome.min.css",
            //          "~/Content/bower_components/Ionicons/css/ionicons.min.css",
            //          "~/Content/dist/css/AdminLTE.min.css"
            //          ));
            ////this js only for login page 
            //bundles.Add(new ScriptBundle("~/Login/js").Include(
            //          "~/Content/bower_components/jquery/dist/jquery.min.js",
            //          "~/Content/bower_components/bootstrap/dist/js/bootstrap.min.js"));
            //// base css for admin template
            //bundles.Add(new StyleBundle("~/Admin/BaseCss").Include(
            //           "~/Content/bower_components/bootstrap/dist/css/bootstrap.min.css",
            //           "~/Content/bower_components/font-awesome/css/font-awesome.min.css",
            //           "~/Content/bower_components/Ionicons/css/ionicons.min.css",
            //            "~/Content/plugins/iCheck/all.css", 
            //            "~/Content/bower_components/select2/dist/css/select2.min.css"));
            //// AdminLTE Theme.
            //bundles.Add(new StyleBundle("~/Admin/ThemeCss").Include(
            //           "~/Content/dist/css/AdminLTE.min.css",
            //           "~/Content/dist/css/skins/_all-skins.min.css",
            //            "~/Content/bower_components/bootstrap-datepicker/dist/css/bootstrap-datepicker.min.css"
            //            ));

            ////Base Js for Admin template
            //bundles.Add(new ScriptBundle("~/AdminBaseJS").Include("~/Content/bower_components/jquery/dist/jquery.min.js",
            //    "~/Content/bower_components/bootstrap/dist/js/bootstrap.min.js",
            //    "~/Content/bower_components/PACE/pace.min.js",
            //    "~/Content/bower_components/select2/dist/js/select2.full.min.js",
            //    "~/Content/bower_components/bootstrap-datepicker/dist/js/bootstrap-datepicker.min.js", 
            //    "~/Content/plugins/iCheck/icheck.min.js"));


            ////AdminLTE JS
            //bundles.Add(new ScriptBundle("~/AdminThemeJS").Include(
            //    "~/Content/bower_components/jquery-slimscroll/jquery.slimscroll.min.js",
            //    "~/Content/bower_components/fastclick/lib/fastclick.js",
            //    "~/Content/dist/js/adminlte.min.js"));
            ////"../../dist/js/adminlte.min.js" -- this js for multiple theme.  
            BundleTable.EnableOptimizations = false;
        }
    }
}
