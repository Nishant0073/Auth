using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDataProtection();

var app = builder.Build();

app.MapGet("/",()=> "Hello World!");

app.MapGet("/username", (HttpContext httpContext,IDataProtectionProvider idp ) =>{

    var protector = idp.CreateProtector("auth-cookie");

    var authCookie = httpContext.Request.Headers.Cookie.FirstOrDefault(x => x.StartsWith("auth="));
    var payload = protector.Unprotect(authCookie.Split("=").Last());
    var username = payload.Split(":").First();
    var password = payload.Split(":").Last();
    return password;
});

app.MapGet("/login", (HttpContext httpContext,IDataProtectionProvider idp) =>{
    var protector = idp.CreateProtector("auth-cookie");
    httpContext.Response.Headers["set-cookie"] = $"auth={protector.Protect("nishant:1234")}" ;
    return "Ok";
});


app.Run();

