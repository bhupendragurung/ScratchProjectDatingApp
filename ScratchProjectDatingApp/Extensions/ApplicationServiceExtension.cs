using Microsoft.EntityFrameworkCore;
using ScratchProjectDatingApp.Data;
using ScratchProjectDatingApp.Helper;
using ScratchProjectDatingApp.Interfaces;
using ScratchProjectDatingApp.Services;

namespace ScratchProjectDatingApp.Extensions
{
    public  static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services,IConfiguration config)
        {


            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseSqlServer(config.GetConnectionString("DefaultConnection"));
            });
            services.AddCors();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.Configure<ClodinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<ILikesRepository, LIkesRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<LogUserActivity>();
            return services;
        }
    }
}
