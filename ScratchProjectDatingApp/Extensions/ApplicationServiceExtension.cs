using Microsoft.EntityFrameworkCore;
using ScratchProjectDatingApp.Data;
using ScratchProjectDatingApp.Helper;
using ScratchProjectDatingApp.Interfaces;
using ScratchProjectDatingApp.Services;
using ScratchProjectDatingApp.SignalR;

namespace ScratchProjectDatingApp.Extensions
{
    public  static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services,IConfiguration config)
        {


            // services.AddDbContext<DataContext>(opt =>
            // {
            //     opt.UseNpgsql(config.GetConnectionString("DefaultConnection"));
            // });
            services.AddCors();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.Configure<ClodinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddScoped<IPhotoService, PhotoService>();
          
            services.AddScoped<LogUserActivity>();
            services.AddSignalR();
            services.AddSingleton<PresenceTracker>();
            return services;
        }
    }
}
