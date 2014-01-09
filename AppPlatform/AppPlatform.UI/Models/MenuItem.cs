using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcOSGi.Shell.Models
{
    public class MenuItem
    {
        public string Text { get; set; }
        public string URL { get; set; }
        public decimal Order { get; set; }
        public string Owner { get; set; }
    }
}