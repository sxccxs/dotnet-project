using BLL.Abstractions.Interfaces.UserInterfaces;
using Core.Models.UserModels;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_PrL.Controllers;

public class LoginController : Controller
{
    private readonly ILoginService loginService;

    public LoginController(ILoginService loginService)
    {
        this.loginService = loginService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromForm] UserLoginModel userLoginModel)
    {
        var token = await loginService.Login(userLoginModel);
        return Ok(token);
    }
}