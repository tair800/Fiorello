using Fiorella;
using Fiorella.Hubs;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
builder.Services.Register(config);
var app = builder.Build();

app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
           name: "areas",
           pattern: "{area:exists}/{controller=DashBoard}/{action=Index}/{id?}"
         );
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
    );
app.MapHub<ChatHub>("/testhub");
app.Run();