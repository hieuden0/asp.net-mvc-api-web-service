using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MrBill_MVC.Controllers;

namespace MrBill_MVC
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "Index",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);
            routes.IgnoreRoute("services/{*path}");
            routes.MapRoute("Index", "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                  new { controller = "^(?!MrBillMobileServices).*" }
              );
            routes.Add(new ServiceRoute("MrBillMobileServices", new WebServiceHostFactory(), typeof(AppWebService))); 

        }
    }
}