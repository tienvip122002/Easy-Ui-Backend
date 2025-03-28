using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace EasyUiBackend.Api.Configurations
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Cấu hình dịch vụ, bao gồm DbContext, Repositories, Services
            services.AddControllers();
            // ... thêm các dịch vụ khác nếu cần
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            // ... cấu hình middleware khác nếu cần
        }
    }
} 