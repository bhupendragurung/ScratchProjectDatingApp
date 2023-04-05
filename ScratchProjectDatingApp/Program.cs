using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ScratchProjectDatingApp.Data;
using ScratchProjectDatingApp.Entity;
using ScratchProjectDatingApp.Extensions;
using ScratchProjectDatingApp.Interfaces;
using ScratchProjectDatingApp.Middleware;
using ScratchProjectDatingApp.Services;
using ScratchProjectDatingApp.SignalR;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationService( builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
var connString="";
if (builder.Environment.IsDevelopment()) 
    connString = builder.Configuration.GetConnectionString("DefaultConnection");
else 
{
// Use connection string provided at runtime by Heroku.
        var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

        // Parse connection URL to connection string for Npgsql
        connUrl = connUrl.Replace("postgres://", string.Empty);
        var pgUserPass = connUrl.Split("@")[0];
        var pgHostPortDb = connUrl.Split("@")[1];
        var pgHostPort = pgHostPortDb.Split("/")[0];
        var pgDb = pgHostPortDb.Split("/")[1];
        var pgUser = pgUserPass.Split(":")[0];
        var pgPass = pgUserPass.Split(":")[1];
        var pgHost = pgHostPort.Split(":")[0];
        var pgPort = pgHostPort.Split(":")[1];
	    var updatedHost = pgHost.Replace("flycast", "internal");

        connString = $"Server={updatedHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb};";
}
builder.Services.AddDbContext<DataContext>(opt =>
{
    opt.UseNpgsql(connString);
});
var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("https://localhost:4200"));
app.UseAuthentication();

app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();   
app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");
app.MapFallbackToController("Index", "Fallback");
var scope = app.Services.CreateScope();
var services=scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    await context.Database.MigrateAsync();
    await Seed.ClearConnections(context);
    await Seed.SeedUser(userManager,roleManager);
}
catch (Exception ex)
{

	var logger = services.GetService<ILogger<Program>>();
    logger.LogError(ex, "An error occured during migration");
}
app.Run(); 

