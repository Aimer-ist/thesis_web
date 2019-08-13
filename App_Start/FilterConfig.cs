using System.Web;
using System.Web.Mvc;

namespace EyeWeb_Again {
    public class FilterConfig {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
