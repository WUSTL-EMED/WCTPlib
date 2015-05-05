using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WCTPlib.v1r1
{
    public class VersionQuery : Operation
    {
        #region Constructors

        internal static VersionQuery Parse(XElement operation)
        {
            if (operation == null)
                throw new ArgumentNullException("operation");

            var inquirer = (string)operation.Attribute("inquirer");
            var dateTime = (string)operation.Attribute("dateTime");
            var listDTDs = (string)operation.Attribute("listDTDs");
            var listConfiguration = (string)operation.Attribute("listConfiguration");

            return new VersionQuery(inquirer)
            {
                TimeDate = dateTime == null ? default(DateTime?) : DateTime.Parse(dateTime),
                ListDTDs = listDTDs == "yes" ? true : false,//is everything besides yes/no treated as no since that is the default?
                ListConfiguration = listConfiguration == "yes" ? true : false,//is everything besides yes/no treated as no since that is the default?
            };
        }

        private VersionQuery()
        {
            TimeDate = DateTime.UtcNow;
        }

        public VersionQuery(string inquirer)
            : this()
        {
            if (String.IsNullOrEmpty(inquirer))
                throw new ArgumentNullException("inquirer");

            Inquirer = inquirer;
        }

        #endregion Constructors

        #region Properties

        [Required, MaxLength(255)]
        public string Inquirer { get; set; }
        //[DefaultValue(null)]
        public DateTime? TimeDate { get; set; }//do we ever care for this to not be now?
        //[DefaultValue(false)]
        public bool ListDTDs { get; set; }
        //[DefaultValue(false)]
        public bool ListConfiguration { get; set; }

        #endregion Properties

        #region Overrides

        protected override XElement GetOperation()
        {
            var operation = new XElement("wctp-VersionQuery", new XAttribute("inquirer", Inquirer));
            if (TimeDate.HasValue)
                operation.Add(new XAttribute("dateTime", Timestamp(TimeDate.Value)));
            if (ListDTDs)
                operation.Add(new XAttribute("listDTDs", ListDTDs ? "yes" : "no"));
            if (ListConfiguration)
                operation.Add(new XAttribute("listConfiguration", ListConfiguration ? "yes" : "no"));
            return operation;
        }

        #endregion Overrides
    }
}
