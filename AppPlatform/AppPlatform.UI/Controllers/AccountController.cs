using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppPlatform.UI.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/

        public ActionResult Login()
        {
            return View();
        }
       
        [HttpPost]
        public ActionResult LoginSuccess()
        {
            ViewBag.Enterprise_ID = Request["Enterprise_ID"];
            return View();
        }

    }
}
