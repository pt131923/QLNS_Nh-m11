using API.Data;
using API.Middleware;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Cấu hình CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.WithOrigins()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
            });
        });
        builder.Services.AddCors(options =>
       {
        options.AddPolicy("AllowOrigin", builder =>
            builder.WithOrigins("http://localhost:4300")
                   .AllowAnyHeader()
                   .AllowAnyMethod());
       });

    // Other service configurations...

        // Cấu hình JsonOptions
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressMapClientErrors = true;
            });

        // Cấu hình AutoMapper
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new AutoMapperProfiles());
        });
        var mapper = mapperConfig.CreateMapper();
        builder.Services.AddSingleton(mapper);

        // Thêm các dịch vụ khác
        builder.Services.AddControllers();
        builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        // Cấu hình DbContext
        builder.Services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Xây dựng ứng dụng
        var app = builder.Build();

        // Cấu hình middleware
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHttpsRedirection();
        }

        app.UseMiddleware<ExceptionMiddleware>();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors("AllowOrigin");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        // Thực hiện migration và seed data
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<DataContext>();
                await context.Database.MigrateAsync();
                await Seed.SeedEmployees(context);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred during migration");
            }
        }

        // Chạy ứng dụng
        app.Run();
    }
}
