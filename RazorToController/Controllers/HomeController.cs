using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorToController
{
    public class HomeController : Controller
    {
        // GET: HomeController
        [Route("~/home")]
        //[Authorize()]
        public ActionResult Index()
        {
            return View(new { Hello="World" });
        }

        
    }
}
