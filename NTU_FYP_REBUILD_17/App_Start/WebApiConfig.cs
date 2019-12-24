using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;

namespace NTU_FYP_REBUILD_17
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			config.MapHttpAttributeRoutes();
			//config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);

			config.Routes.MapHttpRoute(
				name: "GetLogin",
				routeTemplate: "api/{controller}/{action}/{username}/{password}",
				defaults: new
				{
					username = RouteParameter.Optional,
					password = RouteParameter.Optional
				}
			);

			config.Routes.MapHttpRoute(
				name: "GetPatientBasedOnCareGiverID_XML",
				routeTemplate: "api/{controller}/{action}/{token}/{id}/{timeDate}/{mask}"
			);

            config.Routes.MapHttpRoute(
				name: "GetPatientBasedOnCareGiverID_JSON",
				routeTemplate: "api/{controller}/{action}/{token}/{id}/{timeDate}/{mask}"
			);

			config.Routes.MapHttpRoute(
				name: "GetPatients_XML",
				routeTemplate: "api/{controller}/{action}/{token}"
			);

			config.Routes.MapHttpRoute(
				name: "GetPatients_JSON",
				routeTemplate: "api/{controller}/{action}/{token}"
			);

			// IN getTableEntryController class
			config.Routes.MapHttpRoute(
				name: "GetHolidayExperienceEntryXML",
				routeTemplate: "api/{controller}/{action}/{token}"
			);

			config.Routes.MapHttpRoute(
				name: "GetHolidayExperienceEntry_JSON",
				routeTemplate: "api/{controller}/{action}/{token}"
			);

			config.Routes.MapHttpRoute(
				name: "patientalbumdoctornote_XML",
				routeTemplate: "api/{controller}/{action}/{token}"
			);

			config.Routes.MapHttpRoute(
				name: "patientalbumdoctornote_JSON",
				routeTemplate: "api/{controller}/{action}/{token}"
			);

			config.Routes.MapHttpRoute(
				name: "likeitem_XML",
				routeTemplate: "api/{controller}/{action}/{token}"
			);

			config.Routes.MapHttpRoute(
				name: "likeitem_JSON",
				routeTemplate: "api/{controller}/{action}/{token}"
			);

			config.Routes.MapHttpRoute(
				name: "dislikeitem_JSON",
				routeTemplate: "api/{controller}/{action}/{token}"
			);

			config.Routes.MapHttpRoute(
				name: "dislikeitem_XML",
				routeTemplate: "api/{controller}/{action}/{token}"
			);

			config.Routes.MapHttpRoute(
				name: "problemlog_XML",
				routeTemplate: "api/{controller}/{action}/{token}"
			);

			config.Routes.MapHttpRoute(
				name: "problemlog_JSON",
				routeTemplate: "api/{controller}/{action}/{token}"
			);

			config.Routes.MapHttpRoute(
	name: "GetAlbumDoctorNoteFromPatient_XML",
	routeTemplate: "api/{controller}/{action}/{token}"
);
		}
	}
}
