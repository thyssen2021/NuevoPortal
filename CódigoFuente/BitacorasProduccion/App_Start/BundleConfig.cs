using System.Web.Optimization;

namespace IdentitySample
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

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/dataTables_css").Include(
                      "~/Content/vendors/datatables.net-bs/css/dataTables.bootstrap.min.css",
                      "~/Content/vendors/datatables.net-buttons-bs/css/buttons.bootstrap.min.css",
                      "~/Content/vendors/datatables.net-fixedheader-bs/css/fixedHeader.bootstrap.min.css",
                      "~/Content/vendors/datatables.net-responsive-bs/css/responsive.bootstrap.min.css",
                      "~/Content/vendors/datatables.net-scroller-bs/css/scroller.bootstrap.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/dataTables_js").Include(
                        "~/Content/vendors/datatables.net/js/jquery.dataTables.min.js",
                         "~/Content/vendors/datatables.net-bs/js/dataTables.bootstrap.min.js",
                         "~/Content/vendors/datatables.net-buttons/js/dataTables.buttons.min.js",
                          "~/Content/vendors/datatables.net-buttons-bs/js/buttons.bootstrap.min.js",
                          "~/Content/vendors/datatables.net-buttons/js/buttons.flash.min.js",
                          "~/Content/vendors/datatables.net-buttons/js/buttons.html5.min.js",
                          "~/Content/vendors/datatables.net-buttons/js/buttons.print.min.js",
                          "~/Content/vendors/datatables.net-fixedheader/js/dataTables.fixedHeader.min.js",
                          "~/Content/vendors/datatables.net-keytable/js/dataTables.keyTable.min.js",
                          "~/Content/vendors/datatables.net-responsive/js/dataTables.responsive.min.js",
                          "~/Content/vendors/datatables.net-responsive-bs/js/responsive.bootstrap.js",
                          "~/Content/vendors/datatables.net-scroller/js/dataTables.scroller.min.js",
                          "~/Content/vendors/datatables.net-fixedcolumns/dataTables.fixedColumns.min.js",
                          "~/Content/vendors/jszip/dist/jszip.min.js",
                          "~/Content/vendors/pdfmake/build/pdfmake.min.js",
                          "~/Content/vendors/pdfmake/build/vfs_fonts.js"
                      ));
        }
    }
}
