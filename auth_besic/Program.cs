using System.Security.Claims;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDataProtection();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthService>();

var app = builder.Build();


app.Use((ctx,next)=>{
    var idp = ctx.RequestServices.GetRequiredService<IDataProtectionProvider>();
    var protector = idp.CreateProtector("auth-cookie");

    var authCookie = ctx.Request.Headers.Cookie.FirstOrDefault(x => x.StartsWith("auth="));
    var payload = protector.Unprotect(authCookie.Split("=").Last());
    var  key = payload.Split(":").First();
    var  value = payload.Split(":").Last();

    var claims = new List<Claim>();
    claims.Add(new Claim(key,value));
    var identity = new ClaimsIdentity(claims);

    ctx.User = new ClaimsPrincipal(identity);
    return next();
});

app.MapGet("/", () => "Hello World!");

app.MapGet("/username", (HttpContext httpContext) =>
{

    return httpContext.User.FindFirst("usr").Value;
});

app.MapGet("/login", (AuthService auth) =>
{
    auth.SignIn();
    return "Ok";
});


app.Run();


public class AuthService
{
    public IHttpContextAccessor Accessor { get; }
    private readonly IDataProtectionProvider idp;

    public AuthService(IHttpContextAccessor accessor, IDataProtectionProvider idp)
    {
        Accessor = accessor;
        this.idp = idp;
    }
    public void SignIn()
    {
        var protector = idp.CreateProtector("auth-cookie");
        Accessor.HttpContext.Response.Headers["set-cookie"] = $"auth={protector.Protect("usr:nishant")}";
    }

}
