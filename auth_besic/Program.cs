using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);
// builder.Services.AddDataProtection();
// builder.Services.AddHttpContextAccessor();
// builder.Services.AddScoped<AuthService>();
//this will work of our middleware
builder.Services.AddAuthentication("cookie").AddCookie("cookie");

var app = builder.Build();

//using middleware
app.UseAuthentication() ;
// app.Use((ctx,next)=>{
//     var idp = ctx.RequestServices.GetRequiredService<IDataProtectionProvider>();
//     var protector = idp.CreateProtector("auth-cookie");

//     var authCookie = ctx.Request.Headers.Cookie.FirstOrDefault(x => x.StartsWith("auth="));
//     var payload = protector.Unprotect(authCookie.Split("=").Last());
//     var  key = payload.Split(":").First();
//     var  value = payload.Split(":").Last();

//     var claims = new List<Claim>();
//     claims.Add(new Claim(key,value));
//     var identity = new ClaimsIdentity(claims);

//     ctx.User = new ClaimsPrincipal(identity);
//     return next();
// });

app.MapGet("/", () => "Hello World!");

app.MapGet("/username", (HttpContext httpContext) =>
{
    return httpContext.User.FindFirst("usr").Value;
});

app.MapGet("/login", async (HttpContext httpContext) =>
{
    // In this part we can call the database and validate username password
    var claims = new List<Claim>();
    claims.Add(new Claim("usr","nishant"));
    var identity = new ClaimsIdentity(claims,"cookie");

    var  user = new ClaimsPrincipal(identity);
    //this will do work of our auth service
    await  httpContext.SignInAsync("cookie", user );
    // auth.SignIn();
    return "Ok";
});


app.Run();


// public class AuthService
// {
//     public IHttpContextAccessor Accessor { get; }
//     private readonly IDataProtectionProvider idp;

//     public AuthService(IHttpContextAccessor accessor, IDataProtectionProvider idp)
//     {
//         Accessor = accessor;
//         this.idp = idp;
//     }
//     public void SignIn()
//     {
//         var protector = idp.CreateProtector("auth-cookie");
//         Accessor.HttpContext.Response.Headers["set-cookie"] = $"auth={protector.Protect("usr:nishant")}";
//     }

// }
