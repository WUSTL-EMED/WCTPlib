using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace WCTPlib
{
    internal static class Resources
    {
        private static Assembly Assembly { get; set; }
        private static IList<string> ResourceNames { get; set; }

        static Resources()
        {
            Assembly = typeof(Resources).Assembly;
            ResourceNames = Assembly.GetManifestResourceNames().ToList();
        }

        internal static bool ResourceExists(string name)
        {
            return ResourceNames.Any(n => n.EndsWith(name));
        }

        internal static Stream GetResourceStream(string name)
        {
            var resource = ResourceNames.FirstOrDefault(n => n.EndsWith(name, StringComparison.OrdinalIgnoreCase));
            if (String.IsNullOrEmpty(resource))
                throw new ArgumentException(String.Format("Embedded resource '{0}' not found.", name), "resource");
            return Assembly.GetManifestResourceStream(resource);
        }

        internal static bool TryGetResourceStream(string name, out Stream stream)
        {
            stream = null;
            var resource = ResourceNames.FirstOrDefault(n => n.EndsWith(name, StringComparison.OrdinalIgnoreCase));
            if (String.IsNullOrEmpty(resource))
                return false;
            stream = Assembly.GetManifestResourceStream(resource);
            return true;
        }
    }
}
