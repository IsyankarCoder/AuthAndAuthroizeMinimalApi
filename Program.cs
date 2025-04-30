using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataProtection(c=>{
   c.ApplicationDiscriminator="Volki Tolki";
});
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/username",(HttpContext ctx,IDataProtectionProvider idp)=>{

    var protector = idp.CreateProtector("auth-cookie");

    var authCookie =  ctx.Request.Headers.Cookie.FirstOrDefault(x=>x.StartsWith("auth="));
    var protectedPayload = authCookie.Split("=").Last();
    var payload = protector.Unprotect(protectedPayload); 
    var parts = payload.Split(":");
    var key =  parts[0];
    var value = parts[1]; 
    return value; 
});

app.MapGet("/login",(HttpContext ctx,IDataProtectionProvider idp)=>{
    var protector  = idp.CreateProtector("auth-cookie");
    ctx.Response.Headers["set-cookie"]=$"auth={protector.Protect("usr:volkan")}";
  return "Ok";
});

app.Run();


