using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using IAuthenticationService = BLL.Abstractions.Interfaces.UserInterfaces.IAuthenticationService;

namespace ASP.NET_PrL;

public class BasicAuthenticationOptions : AuthenticationSchemeOptions
{
}

public class AuthHandler : AuthenticationHandler<BasicAuthenticationOptions>
{
    private readonly IAuthenticationService authenticationService;

    public AuthHandler(IOptionsMonitor<BasicAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IAuthenticationService authenticationService) : base(options, logger, encoder, clock)
    {
        this.authenticationService = authenticationService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Cookies.ContainsKey("token"))
            return AuthenticateResult.Fail("Unauthorized");

        var token = Request.Cookies["token"];
        if (string.IsNullOrEmpty(token))
        {
            return AuthenticateResult.Fail("Unauthorized");
        }

        try
        {
            return await this.ValidateToken(token);
        }
        catch (Exception ex)
        {
            return AuthenticateResult.Fail(ex.Message);
        }
    }

    private async Task<AuthenticateResult> ValidateToken(string token)
    {
        var authResult = await this.authenticationService.GetUserByToken(token);
        if (!authResult.IsSuccess)
        {
            return AuthenticateResult.Fail("Unauthorized");
        }

        var user = authResult.Value;
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new System.Security.Principal.GenericPrincipal(identity, null);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}
