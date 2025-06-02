using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Forms;
using DeviceArchiving.Service;
using DeviceArchiving.Data.Contexts;
using DeviceArchiving.Data.Dto;
using DeviceArchiving.Data;
namespace DeviceArchiving.WindowsForm.Forms;
static class Program
{
    public static IServiceProvider Services { get; private set; }

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        services.AddSingleton<UserIdInterceptor>(sp => new UserIdInterceptor(() => AppSession.CurrentUserId));
        services.AddDbContextFactory<DeviceArchivingContext>(options =>
                        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                               .AddInterceptors(services.BuildServiceProvider().GetRequiredService<UserIdInterceptor>()));


        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IDeviceService, DeviceService>();
        services.AddScoped<IOperationTypeService, OperationTypeService>();
        services.AddScoped<IOperationService, OperationService>();

        Services = services.BuildServiceProvider();


        using (var scope = Services.CreateScope())
        {
            var accountService = scope.ServiceProvider.GetService<IAccountService>();
            Application.Run(new LoginForm(accountService));
        }
    }
}
