using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace WCTPlib
{
    public class WCTP
    {
        public static HttpClient Client { get; set; }
        public Uri Endpoint { get; set; }
        //public string DTD { get; set; }

        #region Constructors

        //public static WCTP Init(WCTPVersion version)
        //{
        //    switch (version)
        //    {
        //        case WCTPVersion.v1r0:
        //            return new WCTPlib.v1r0.WCTP();
        //        case WCTPVersion.v1r1:
        //            return new WCTPlib.v1r1.WCTP();
        //        case WCTPVersion.v1r2:
        //            return new WCTPlib.v1r2.WCTP();
        //        case WCTPVersion.v1r3:
        //        default:
        //            return new WCTPlib.v1r3.WCTP();
        //    }
        //}

        public WCTP()
        {
        }

        public WCTP(Uri wctpEndpoint)
            : this()
        {
            Endpoint = wctpEndpoint;
        }

        public WCTP(string wctpEndpoint)
            : this(new Uri(wctpEndpoint, UriKind.RelativeOrAbsolute))
        {
        }

        #endregion Constructors

        public XDocument Parse(string xml)
        {
            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Parse,
                ValidationType = ValidationType.DTD,
                XmlResolver = new XmlResourceResolver()//This assumes properly named files on remote DTD urls, do we want to try to detect the proper DTD if it fails?
            };

            if (String.IsNullOrEmpty(xml))
                throw new ArgumentNullException("xml");

            using (var text = new StringReader(xml))
            using (var reader = XmlReader.Create(text, settings))
            {
                //var doc = XDocument.Load(reader, LoadOptions.None);
                //var test1 = doc.Root.Attribute("wctpVersion");
                //var test2 = doc.DocumentType;
                //Check for DTD/wctpVersion mismatch?

                return XDocument.Load(reader, LoadOptions.None);//Output dtd version?
            }
        }

        public bool TryParse(string xml, out XDocument document)
        {
            try
            {
                document = Parse(xml);
                return true;
            }
            catch (Exception)
            {
                document = null;
                return false;
            }
        }

        public XDocument Parse(HttpRequest request)
        {
            //WCTP requests should all be UTF8, so we can probably make this assumption, not sure how safe it is though.
            //This should hopefully keep the underlying InputStream open so we can use it later in the handler if we want to.
            using (var reader = new StreamReader(request.InputStream, Encoding.UTF8, false, request.ContentLength, true))
            {
                var body = reader.ReadToEndAsync();
                body.Wait();
                return Parse(body.Result);
            }
        }

        public bool TryParse(HttpRequest request, out XDocument document)
        {
            try
            {
                document = Parse(request);
                return true;
            }
            catch (Exception)
            {
                //Log? More specific exceptions?
                document = null;
                return false;
            }
        }

        public XDocument Parse(HttpRequestMessage request)
        {
            var body = request.Content.ReadAsStringAsync();
            body.Wait();
            return Parse(body.Result);
        }

        public bool TryParse(HttpRequestMessage request, out XDocument document)
        {
            try
            {
                document = Parse(request);
                return true;
            }
            catch (Exception)
            {
                //Log? More specific exceptions?
                document = null;
                return false;
            }
        }

        public XDocument Parse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)//All WCTP responses should be 200, it isn't RESTful.
                return null;
            var body = response.Content.ReadAsStringAsync();
            body.Wait();
            return Parse(body.Result);
        }

        public bool TryParse(HttpResponseMessage response, out XDocument document)
        {
            try
            {
                document = Parse(response);
                return true;
            }
            catch (Exception)
            {
                //Log? More specific exceptions?
                document = null;
                return false;
            }
        }

        public Operation Parse(XDocument xml)
        {
            var root = xml.Root;
            if (root == null ||
                root.NodeType != XmlNodeType.Element ||
                root.Name.LocalName != "wctp-Operation")
                return null;

            var version = (string)root.Attribute("wctpVersion");
            switch (version)
            {
                //case WCTPlib.v1r0.Operation.VersionString:
                //    return new WCTPlib.v1r0.WCTP().Parse(xml);
                case WCTPlib.v1r1.Operation.VersionString:
                    return new WCTPlib.v1r1.WCTP().Parse(xml);
                //case WCTPlib.v1r2.Operation.VersionString:
                //    return new WCTPlib.v1r2.WCTP().Parse(xml);
                //case WCTPlib.v1r3.Operation.VersionString:
                //    return new WCTPlib.v1r3.WCTP().Parse(xml);
                default:
                    return null;//throw?
            }
        }

        public bool TryParse(XDocument xml, out Operation operation)
        {
            try
            {
                operation = Parse(xml);
                return true;
            }
            catch (Exception)
            {
                //Log? More specific exceptions?
                operation = null;
                return false;
            }
        }

        public async Task<HttpResponseMessage> Request(Operation operation, CancellationToken? token = null)
        {
            if (Client == null)
            {
                //If we don't already have a HttpClient spin one up, it can be used indefinitely.
                var handler = new HttpClientHandler
                {
                    AllowAutoRedirect = true,
                    AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                };
                Client = new HttpClient(handler);
            }

            using (var content = operation.GetContent())
            {
                return await Client.PostAsync(Endpoint, content, token ?? CancellationToken.None);
            }
        }
    }
}
