using System.Web.Optimization;

namespace SistemaSemus
{
    public class BundleConfig
    {
        // Para obter mais informações sobre o agrupamento, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region Scripts
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/popper").Include(
                        "~/Scripts/umd/popper.min.js"));

            // Use a versão em desenvolvimento do Modernizr para desenvolver e aprender. Em seguida, quando estiver
            // pronto para a produção, utilize a ferramenta de build em https://modernizr.com para escolher somente os testes que precisa.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));
            #endregion

            #region Custom
            bundles.Add(new ScriptBundle("~/bundles/semusJS").Include(
                       "~/Scripts/SemusScripts/scripts_do_sistema.js"));

            bundles.Add(new ScriptBundle("~/bundles/stock").Include(
                       "~/Scripts/bootstrap.bundle.js",
                       "~/Scripts/CustomJS/feather.min.js",
                       "~/Scripts/CustomJS/Chart.min.js",
                       "~/Scripts/SemusScripts/dashboard.js"));

            bundles.Add(new ScriptBundle("~/bundles/beagle").Include(
                "~/Scripts/Beagle/perfect-scrollbar.min.js",
                "~/Scripts/bootstrap.bundle.min.js",
                "~/Scripts/Beagle/app.js",
                "~/Scripts/Beagle/jquery.flot.js",
                "~/Scripts/Beagle/jquery.flot.pie.js",
                "~/Scripts/Beagle/jquery.flot.time.js",
                "~/Scripts/Beagle/jquery.flot.resize.js",
                "~/Scripts/Beagle/jquery.flot.orderBars.js",
                "~/Scripts/Beagle/curvedLines.js",
                "~/Scripts/Beagle/jquery.flot.tooltip.js"));
            #endregion

            #region cloudflare
            bundles.Add(new ScriptBundle("~/bundles/selectpicker").Include(
                "~/Scripts/CustomJS/bootstrap-select.min.js",
                "~/Scripts/SemusScripts/seletcpicker.js"));

            bundles.Add(new ScriptBundle("~/bundles/masks").Include(
                "~/Scripts/CustomJS/jquery.mask.min.js",
                "~/Scripts/SemusScripts/Mascaras.js"));
            #endregion

            #region Styles
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/Custom/Site.css",
                      "~/Content/FontAwesome/css/all.css"));

            bundles.Add(new StyleBundle("~/Content/stock").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/Custom/Estoques.css",
                      "~/Content/FontAwesome/css/all.css"));

            bundles.Add(new StyleBundle("~/Content/create").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/Custom/form-validation.css",
                      "~/Content/FontAwesome/css/all.css",
                      "~/Content/Custom/bootstrap-select.min.css"));

            bundles.Add(new StyleBundle("~/Content/delete").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/Custom/signin.css"));

            bundles.Add(new StyleBundle("~/Content/login").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/Custom/floating-labels.css"));

            bundles.Add(new StyleBundle("~/Content/beagle").Include(
                "~/Content/CssBeagle/perfect-scrollbar.css",
                "~/Content/CssBeagle/material-design-ionic-font.css",
                "~/Content/CssBeagle/app.css"));
            #endregion
        }
    }
}
