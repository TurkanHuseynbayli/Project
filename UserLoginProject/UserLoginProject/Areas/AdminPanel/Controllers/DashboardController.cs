using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserLoginProject.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Areas.AdminPanel.Controllers
{
    public class DashboardController : Controller
    {
        [Area("AdminPanel")]
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
