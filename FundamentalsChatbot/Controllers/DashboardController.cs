﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FundamentalsChatbot.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Dashboard()
        {
            ViewData["Message"] = "A user's dashboard for seeing their projects, creating new projects, etc.";

            return View();
        }
    }
}