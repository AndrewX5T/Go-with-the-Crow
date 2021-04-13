using System.Web;
using System.Web.Mvc;

namespace Go_with_the_Crow
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
