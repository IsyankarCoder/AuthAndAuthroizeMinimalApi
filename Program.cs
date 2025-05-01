using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication("cookie").
                 AddCookie("cookie");

/*builder.Services.AddAuthentication("volki")
.AddCookie("volki");   */   
//builder.Services.AddScoped<AuthService>();
builder.Services.AddDataProtection(c=>{
   c.ApplicationDiscriminator="Volki Tolki";
});


var app = builder.Build();
app.UseAuthentication();
/*app.Use((ctx,next)=>{
    var idp = ctx.RequestServices.GetRequiredService<IDataProtectionProvider>();
    var protector = idp.CreateProtector("auth-cookie");

    var authCookie =  ctx.Request.Headers.Cookie.FirstOrDefault(x=>x.StartsWith("auth="));
    var protectedPayload = authCookie?.Split("=").Last();

    if(protectedPayload is not null)
    { 
       var payload = protector.Unprotect(protectedPayload); 
       var parts = payload.Split(":");
       var key =  parts[0];
       var value = parts[1];  
       var claims = new List<Claim>();
    claims.Add(new Claim(key,value));

    var identity = new ClaimsIdentity(claims);
    ctx.User = new ClaimsPrincipal(identity);

    }
 return next();
});*/

app.MapGet("/", () => "Hello World!");

app.MapGet("/username",(HttpContext ctx)=>{ 
    return ctx.User.FindFirst("usr").Value;
  
});

app.MapGet("/login",async (HttpContext ctx)=>{


   var claims = new List<Claim>();
   claims.Add(new Claim("usr","volkan"));
   var identity =  new ClaimsIdentity(claims,"cookie");
   var identity2 = new ClaimsIdentity(claims,"volki");

  /* var  identityList = new List<ClaimsIdentity>{
     identity,identity2
   };*/
   var user = new ClaimsPrincipal(identity); 
   
   await ctx.SignInAsync("cookie",user);
  return "Ok";
});

if(!app.Environment.IsDevelopment()){
    app.UseSwagger();
    app.UseSwaggerUI(); 
}

app.Run();


/*public class AuthService{
   private readonly IDataProtectionProvider _idp;
   private readonly IHttpContextAccessor _accessor;

   public AuthService(IDataProtectionProvider Idp, IHttpContextAccessor Accessor){
       _idp=Idp;
       _accessor=Accessor;
   }

   public void SignIn(){
    var protector = _idp.CreateProtector("auth-cookie");
    _accessor.HttpContext.Response.Headers["set-cookie"] = $"auth={protector.Protect("usr:volkan")}";
   }
}*/
