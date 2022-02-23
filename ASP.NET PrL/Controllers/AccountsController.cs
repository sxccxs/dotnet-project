using BLL.Abstractions.Interfaces.UserInterfaces;
using Core.DataClasses;
using Core.Models.UserModels;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_PrL.Controllers;

public class AccountsController : Controller
{
    private readonly ILoginService loginService;

    private readonly IRegistrationService registrationService;

    private readonly IAccountActivationService activationService;

    public AccountsController(ILoginService loginService, IRegistrationService registrationService, IAccountActivationService activationService)
    {
        this.loginService = loginService;
        this.registrationService = registrationService;
        this.activationService = activationService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View("Login");
        // return (User.Identity?.IsAuthenticated ?? false) ? Ok() : View("Login");
        // replace ok with home account page
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            return Redirect("/Home");
        }
        return View(); // ToDo: redirect to account page
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromForm] UserLoginModel loginData)
    {
        if (!ModelState.IsValid)
        {
            return View(loginData);
        }

        var tokenResult = await this.loginService.Login(loginData);
        if (!tokenResult.IsSuccess)
        {
            ViewData["Message"] = tokenResult.ExceptionMessage;
        }
        else
        {
            Response.Cookies.Append("token", tokenResult.Value);
        }

        return Redirect("/Home"); // ToDo: redirect to account page
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromForm] UserRegistrationModel registrationModel)
    {
        if (!ModelState.IsValid)
        {
            return View(registrationModel);
        }

        var result = await this.registrationService.Register(registrationModel);
        if (!result.IsSuccess)
        {
            ViewData["Message"] = result.ExceptionMessage;
            return View(registrationModel);
        }

        ModelState.Clear();
        ViewData["Message"] = "You were registered successfully. An activation link was sent to your email.";
        return View();
    }

    [HttpGet]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("token");

        return Redirect("/");
    }

    [HttpGet]
    [Route("/Accounts/Activate/{uidb64}/{token}")]
    public async Task<IActionResult> Activate(string uidb64, string token)
    {
        var payload = new AccountActivationPayload()
        {
            Uidb64 = uidb64,
            Token = token,
        };

        var result = await this.activationService.Activate(payload);
        if (!result.IsSuccess)
        {
            ViewData["Message"] = "Invalid link.";
        }

        ViewData["Message"] = "Account activated successfully.";
        return View();
    }
}
