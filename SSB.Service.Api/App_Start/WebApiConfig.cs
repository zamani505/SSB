using Newtonsoft.Json.Serialization;
using SSB.Service.SSBApi.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SSB.Service.SSBApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //  config.MessageHandlers.Add(new ApiKeyHandler());
            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;

            config.Formatters.Remove(config.Formatters.XmlFormatter);

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
