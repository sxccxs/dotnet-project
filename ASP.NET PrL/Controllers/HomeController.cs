﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ASP.NET_PrL.Models;

namespace ASP.NET_PrL.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return Redirect("/Login/Index");

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
