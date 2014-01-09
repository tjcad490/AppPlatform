using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcOSGi.Shell.Models
{
    public class ApplicationViewModel
    {
        public ApplicationViewModel()
        {
            MainMenuItems = new List<MenuItem>();
            SidebarMenuItems = new List<MenuItem>();
        }

        public List<MenuItem> MainMenuItems { get; private set; }
        public List<MenuItem> SidebarMenuItems { get; private set; }
    }
}