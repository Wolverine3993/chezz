using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Chezz.Conventions
{
	/// <summary>
	/// Prepends a global route prefix (e.g. "api") to every controller route.
	/// </summary>
	public class RoutePrefixConvention : IApplicationModelConvention
	{
		private readonly AttributeRouteModel _prefix;

		public RoutePrefixConvention(string prefix)
		{
			_prefix = new AttributeRouteModel(new RouteAttribute(prefix));
		}

		public void Apply(ApplicationModel application)
		{
			foreach (var controller in application.Controllers)
			{
				foreach (var selector in controller.Selectors)
				{
					selector.AttributeRouteModel = selector.AttributeRouteModel is null
						? _prefix
						: AttributeRouteModel.CombineAttributeRouteModel(_prefix, selector.AttributeRouteModel);
				}
			}
		}
	}
}
