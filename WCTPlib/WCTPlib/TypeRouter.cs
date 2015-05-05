using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WCTPlib
{
    public class OperationController
    {
        //// Summary:
        ////     Gets the action context.
        ////
        //// Returns:
        ////     The action context.
        //public HttpActionContext ActionContext { get; set; }
        ////
        //// Summary:
        ////     Gets the System.Web.Http.HttpConfiguration of the current System.Web.Http.ApiController.
        ////
        //// Returns:
        ////     The System.Web.Http.HttpConfiguration of the current System.Web.Http.ApiController.
        //public HttpConfiguration Configuration { get; set; }
        ////
        //// Summary:
        ////     Gets the System.Web.Http.HttpConfiguration of the current System.Web.Http.ApiController.
        ////
        //// Returns:
        ////     The System.Web.Http.HttpConfiguration of the current System.Web.Http.ApiController.
        //public HttpControllerContext ControllerContext { get; set; }
        ////
        //// Summary:
        ////     Gets the model state after the model binding process.
        ////
        //// Returns:
        ////     The model state after the model binding process.
        //public ModelStateDictionary ModelState { get; }
        ////
        //// Summary:
        ////     Gets or sets the HttpRequestMessage of the current System.Web.Http.ApiController.
        ////
        //// Returns:
        ////     The HttpRequestMessage of the current System.Web.Http.ApiController.
        //public HttpRequestMessage Request { get; set; }
        ////
        //// Summary:
        ////     Gets the request context.
        ////
        //// Returns:
        ////     The request context.
        //public HttpRequestContext RequestContext { get; set; }
        ////
        //// Summary:
        ////     Gets an instance of a System.Web.Http.Routing.UrlHelper, which is used to
        ////     generate URLs to other APIs.
        ////
        //// Returns:
        ////     A System.Web.Http.Routing.UrlHelper, which is used to generate URLs to other
        ////     APIs.
        //public UrlHelper Url { get; set; }
        ////
        //// Summary:
        ////     Returns the current principal associated with this request.
        ////
        //// Returns:
        ////     The current principal associated with this request.
        //public IPrincipal User { get; set; }
    }

    public class Route
    {
        /// <summary>
        /// Constructs a route container for a WCTPlib.Operation handler method.
        /// </summary>
        /// <param name="controller">The System.Type of the class containing the handler method.</param>
        /// <param name="action">The name of the method handling the WCTPlib.Operation</param>
        internal Route(Type controller, string action)//, params Tuple<string, object>[] parameters)
        {
            Controller = controller;
            Action = action;
            //Parameters = parameters.ToDictionary(p => p.Item1, p => p.Item2);
        }

        public string Action { get; private set; }
        public Type Controller { get; private set; }
        //public IDictionary<string, object> Parameters { get; set; }
    }

    public class RouteTable
    {
        public RouteTable()
        {
            Routes = new Dictionary<Type, Route>();
        }

        private static Dictionary<Type, Route> Routes { get; set; }

        /// <summary>
        /// Map a handler method for a WCTP operation type.
        /// </summary>
        /// <param name="operation">The WCTPlib.Operation type being handled by the method.</param>
        /// <param name="controller">The System.Type of the class containing the handler method.</param>
        /// <param name="action">The name of the method handling the WCTPlib.Operation</param>
        public void MapRoute(Type operation, Type controller, string action)
        {
            //TODO: Better way of doing this, restricted generic?
            if (!operation.IsSubclassOf(typeof(Operation)))
                throw new ArgumentException("The WCTP route handler can only handle WCTPlib.Operation child types.", "type");

            var method = controller.GetMethod(action, BindingFlags.Instance | BindingFlags.ExactBinding | BindingFlags.Public);
            if (method == null)
                throw new ArgumentException("Controller method " + action + " not found in class " + controller.Name + ".", "action");

            var parameters = method.GetParameters();
            if (parameters.Count() != 1)
                throw new Exception("Invalid controller parameter count, must take a single child of WCTPlib.Operation.");
            if (!parameters.First().ParameterType.IsSubclassOf(typeof(Operation)))
                throw new Exception("Invalid controller parameter type, must be a child of WCTPlib.Operation.");
            if (!method.ReturnType.IsAssignableFrom(typeof(Operation)))
                throw new Exception("Invalid controller return type, must be a WCTPlib.Operation or one of it's children.");

            Routes.Add(operation, new Route(controller, action));
        }

        /// <summary>
        /// Tries to retrieve a route specified for handling a specific WCTPlib.Operation type.
        /// </summary>
        /// <param name="type">The WCTPlib.Operation type being handled by the method.</param>
        /// <param name="route">The route of the handler method.</param>
        /// <returns>True if a route was found, otherwise false.</returns>
        public bool TryGetRoute(Type type, out Route route)
        {
            //var type = request.RequestType;
            //var path = (request.PathInfo ?? String.Empty).TrimStart('/');
            return Routes.TryGetValue(type, out route);
        }
    }
}
