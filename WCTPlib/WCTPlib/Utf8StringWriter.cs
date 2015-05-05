using System.IO;
using System.Text;

namespace WCTPlib
{
    internal class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding
        {
            get
            {
                return Encoding.UTF8;//WCTP only supports UTF-8.
            }
        }
    }
}
