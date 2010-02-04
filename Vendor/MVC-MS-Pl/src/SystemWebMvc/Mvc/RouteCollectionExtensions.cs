namespace System.Web.Mvc {
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Routing;

    public static class RouteCollectionExtensions {

        private static RouteCollection FilterRouteCollectionByArea(RouteCollection routes, string areaName) {
            if (areaName == null) {
                areaName = String.Empty;
            }

            RouteCollection filteredRoutes = new RouteCollection();

            using (routes.GetReadLock()) {
                foreach (RouteBase route in routes) {
                    string thisAreaName = AreaHelpers.GetAreaName(route) ?? String.Empty;
                    if (String.Equals(thisAreaName, areaName, StringComparison.OrdinalIgnoreCase)) {
                        filteredRoutes.Add(route);
                    }
                }
            }

            return filteredRoutes;
        }

        public static VirtualPathData GetVirtualPathForArea(this RouteCollection routes, RequestContext requestContext, RouteValueDictionary values) {
            return GetVirtualPathForArea(routes, requestContext, null /* name */, values);
        }

        public static VirtualPathData GetVirtualPathForArea(this RouteCollection routes, RequestContext requestContext, string name, RouteValueDictionary values) {
            if (routes == null) {
                throw new ArgumentNullException("routes");
            }

            if (!String.IsNullOrEmpty(name)) {
                // the route name is a stronger qualifier than the area name, so just pipe it through
                return routes.GetVirtualPath(requestContext, name, values);
            }

            RouteValueDictionary valuesWithoutArea = values;

            string targetArea = null;
            if (values != null) {
                object targetAreaRawValue;
                if (values.TryGetValue("area", out targetAreaRawValue)) {
                    targetArea = targetAreaRawValue as string;

                    // replace the original RVD so that we don't end up with ?area=targetArea in the generated URL
                    valuesWithoutArea = new RouteValueDictionary(values);
                    valuesWithoutArea.Remove("area");
                }
                else {
                    // set target area to current area
                    if (requestContext != null) {
                        targetArea = AreaHelpers.GetAreaName(requestContext.RouteData);
                    }
                }
            }

            RouteCollection filteredRoutes = FilterRouteCollectionByArea(routes, targetArea);
            VirtualPathData vpd = filteredRoutes.GetVirtualPath(requestContext, valuesWithoutArea);
            return vpd;
        }

        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#",
            Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static void IgnoreRoute(this RouteCollection routes, string url) {
            IgnoreRoute(routes, url, null /* constraints */);
        }

        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#",
            Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static void IgnoreRoute(this RouteCollection routes, string url, object constraints) {
            if (routes == null) {
                throw new ArgumentNullException("routes");
            }
            if (url == null) {
                throw new ArgumentNullException("url");
            }

            IgnoreRouteInternal route = new IgnoreRouteInternal(url) {
                Constraints = new RouteValueDictionary(constraints)
            };

            routes.Add(route);
        }

        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#",
            Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapRoute(this RouteCollection routes, string name, string url) {
            return MapRoute(routes, name, url, null /* defaults */, (object)null /* constraints */);
        }

        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#",
            Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapRoute(this RouteCollection routes, string name, string url, object defaults) {
            return MapRoute(routes, name, url, defaults, (object)null /* constraints */);
        }

        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#",
            Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapRoute(this RouteCollection routes, string name, string url, object defaults, object constraints) {
            return MapRoute(routes, name, url, defaults, constraints, null /* namespaces */);
        }

        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#",
            Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapRoute(this RouteCollection routes, string name, string url, string[] namespaces) {
            return MapRoute(routes, name, url, null /* defaults */, null /* constraints */, namespaces);
        }

        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#",
            Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapRoute(this RouteCollection routes, string name, string url, object defaults, string[] namespaces) {
            return MapRoute(routes, name, url, defaults, null /* constraints */, namespaces);
        }

        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#",
            Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapRoute(this RouteCollection routes, string name, string url, object defaults, object constraints, string[] namespaces) {
            if (routes == null) {
                throw new ArgumentNullException("routes");
            }
            if (url == null) {
                throw new ArgumentNullException("url");
            }

            Route route = new Route(url, new MvcRouteHandler()) {
                Defaults = new RouteValueDictionary(defaults),
                Constraints = new RouteValueDictionary(constraints),
                DataTokens = new RouteValueDictionary()
            };

            if ((namespaces != null) && (namespaces.Length > 0)) {
                route.DataTokens["Namespaces"] = namespaces;
            }

            routes.Add(name, route);

            return route;
        }

        private sealed class IgnoreRouteInternal : Route {
            public IgnoreRouteInternal(string url)
                : base(url, new StopRoutingHandler()) {
            }

            public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary routeValues) {
                // Never match during route generation. This avoids the scenario where an IgnoreRoute with
                // fairly relaxed constraints ends up eagerly matching all generated URLs.
                return null;
            }
        }
    }
}
