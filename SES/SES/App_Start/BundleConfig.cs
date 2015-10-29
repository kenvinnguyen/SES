using System.Web;
using System.Web.Optimization;

namespace SES
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            //================================================ Scripts ==========================================

            bundles.Add(new ScriptBundle("~/bundles/Home").Include(
             "~/Scripts/app/app.js"));
           
            //nguoi dung
            bundles.Add(new ScriptBundle("~/bundles/appUser").Include(
                "~/Scripts/app/app.js",
                "~/Scripts/app/HOAdminAuthUser.js"));
           
            //phan quyen nguoi dung
            bundles.Add(new ScriptBundle("~/bundles/appHOAdminAuthRole").Include(
                "~/Scripts/app/app.js",
                "~/Scripts/app/HOAdminAuthRole.js"));
           
            //thong bao
            bundles.Add(new ScriptBundle("~/bundles/appHOAdminAuthAnnouncement").Include(
           "~/Scripts/app/app.js",
           "~/Scripts/app/HOAdminAuthAnnouncement.js"));

            //cac doan script duoc su dung lai
            bundles.Add(new ScriptBundle("~/bundles/appHOAdminAuthAnnouncement").Include(
          "~/Scripts/app/app.js",
          "~/Scripts/app/HOAdminAuthAnnouncement.js"));
            //Phan cap vung mien
            bundles.Add(new ScriptBundle("~/bundles/appAdminMasterTerritory").Include(
                "~/Scripts/app/app.js",
                "~/Scripts/app/AdminMasterTerritory.js"));
            //Quản lý lịch nghỉ
            bundles.Add(new ScriptBundle("~/bundles/appAdminMasterHoliday").Include(
                "~/Scripts/app/app.js",
                "~/Scripts/app/AdminMasterHoliday.js"));
            //Quản lý lịch nghỉ
            bundles.Add(new ScriptBundle("~/bundles/appDelivery").Include(
                "~/Scripts/app/app.js",
                "~/Scripts/app/DeliveryManagement.js"));
            //================================================ Scripts ==========================================

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
        }
    }
}
