using Library_Webapi.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Unity;

namespace Library_Webapi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

			// Web API 設定和服務
			// Web API 路由
			config.EnableCors();

			config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
