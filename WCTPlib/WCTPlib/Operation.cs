using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Xml.Linq;

namespace WCTPlib
{
    public abstract class Operation
    {
        public abstract Version Version { get; }
        public abstract XDocument GetDocument();
        public abstract string GetXml(SaveOptions options = SaveOptions.DisableFormatting);
        public abstract StringContent GetContent(SaveOptions options = SaveOptions.DisableFormatting);
        public abstract HttpResponseMessage GetResponseMessage(SaveOptions options = SaveOptions.DisableFormatting);

        public bool IsValid()
        {
            var context = new ValidationContext(this);//, serviceProvider: null, items: null
            return Validator.TryValidateObject(this, context, new List<ValidationResult>(), true);
        }

        public bool IsValid(out IList<ValidationResult> validationResults)
        {
            validationResults = new List<ValidationResult>();
            var context = new ValidationContext(this);//, serviceProvider: null, items: null
            return Validator.TryValidateObject(this, context, validationResults, true);
        }
    }
}
