using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlogPlugin.Models;

namespace BlogPlugin.Controllers
{
    public class HelloController : Controller
    {
        //
        // GET: /Hello/

        public ActionResult Index()
        {
            var model = new Movies();
            return View(model.GetMovies());
        }

        public ActionResult Other()
        {
            return View();
        }

    }
}
