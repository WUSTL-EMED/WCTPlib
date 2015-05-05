using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace WCTPlib
{
    public static class WCTPConfig
    {
        internal static RouteTable RouteTable = new RouteTable();

        public static void RegisterRoutes(Action<RouteTable> registerCallback)
        {
            if (registerCallback == null)
                throw new ArgumentNullException("registerCallback");
            registerCallback(RouteTable);//Not sure we want to do this.
        }
    }

    public class WCTPHandler : IHttpHandler
    {
        /// <summary>
        /// Indicates that an instance of WCTPlib.WCTPHandler is reusable.
        /// </summary>
        public bool IsReusable
        {
            get
            {
                return true;//This handler _should_ be stateless outside of the request context and thus reusable.
            }
        }

        /// <summary>
        /// Process a request as a WCTP operation.
        /// </summary>
        /// <param name="context">The System.Web.HttpContext of the request.</param>
        public void ProcessRequest(HttpContext context)
        {
            var request = context.Request;
            var response = context.Response;
            var requestType = (request.RequestType ?? String.Empty);
            var contentType = (request.ContentType ?? String.Empty).Split(';').First();
            //WCTP only uses POST requests with a text/xml content type.
            if (!requestType.Equals("POST", StringComparison.OrdinalIgnoreCase))//Method verbs are supposed to be case sensitive: RFC 2616 § 5.1.1 Method
            {
                BadRequestType(ref response);
                return;
            }
            if (!contentType.Equals("text/xml", StringComparison.OrdinalIgnoreCase))
            {
                BadContentType(ref response);
                return;
            }

            var wctp = new WCTPlib.WCTP();

            XDocument xml;
            if (!wctp.TryParse(request, out xml))
            {
                XMLValidationError(ref response);//301 vs 302, separate parsing from validation error, do we care?
                return;
            }

            Operation operation;
            if (!wctp.TryParse(xml, out operation))
            {
                XMLValidationError(ref response);//301 vs 302, separate parsing from validation error, do we care?
                return;
            }

            Route handler;
            if (operation == null || !WCTPConfig.RouteTable.TryGetRoute(operation.GetType(), out handler))
            {
                OperationNotSupported(ref response);
                return;
            }

            var controller = Activator.CreateInstance(handler.Controller);
            var method = handler.Controller.GetMethod(handler.Action, BindingFlags.Instance | BindingFlags.ExactBinding | BindingFlags.Public);

            if (controller == null || method == null)
            {
                OperationNotSupported(ref response);
                return;
            }

            try
            {
                //Pass context?
                //Right now the Route mapper does all of the type checking, so we will assume everything is proper here.
                var content = method.Invoke(controller, new object[] { operation }) as Operation;
                response.Clear();
                response.StatusCode = (int)HttpStatusCode.OK;
                response.ContentType = "text/xml";
                response.ContentEncoding = Encoding.UTF8;
                response.Write(content.GetXml());
                response.End();
                return;
            }
            catch (ArgumentException)
            {
                //log?
                //bad arguments, other exceptions?
            }

            InternalServerError(ref response);
            return;
        }

        #region Error Responses

        //Output xml on errors?
        private void BadContentType(ref HttpResponse response)
        {
            response.Clear();
            response.StatusCode = (int)HttpStatusCode.UnsupportedMediaType;
            response.ContentType = "text/plain";
            response.ContentEncoding = Encoding.UTF8;
            response.Write("415 Unsupported Media Type");
            response.End();
            return;
        }

        private void BadRequestType(ref HttpResponse response)
        {
            response.Clear();
            response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
            response.ContentType = "text/plain";
            response.ContentEncoding = Encoding.UTF8;
            response.AddHeader("Allow", "POST");
            response.Write("405 Method Not Allowed");
            response.End();
            return;
        }

        private void InternalServerError(ref HttpResponse response)
        {
            var content = new WCTPlib.v1r1.Failure((int)Status.InternalServerError) { ErrorText = "Internal Server Error" };
            response.Clear();
            response.StatusCode = (int)HttpStatusCode.OK;
            response.ContentType = "text/xml";
            response.ContentEncoding = Encoding.UTF8;
            response.Write(content.GetXml());
            response.End();
            return;
        }

        private void OperationNotSupported(ref HttpResponse response)
        {
            var content = new WCTPlib.v1r1.Failure((int)Status.OperationNotSupported) { ErrorText = "Operation not supported" };
            response.Clear();
            response.StatusCode = (int)HttpStatusCode.OK;
            response.ContentType = "text/xml";
            response.ContentEncoding = Encoding.UTF8;
            response.Write(content.GetXml());
            response.End();
            return;
        }

        //WCTP version for errors?
        private void XMLValidationError(ref HttpResponse response)
        {
            var content = new WCTPlib.v1r1.Failure((int)Status.XMLValidationError) { ErrorText = "XML validation error" };
            response.Clear();
            response.StatusCode = (int)HttpStatusCode.OK;
            response.ContentType = "text/xml";
            response.ContentEncoding = Encoding.UTF8;
            response.Write(content.GetXml());
            response.End();
            return;
        }

        #endregion Error Responses
    }
}
