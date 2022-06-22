using Microsoft.AspNetCore;

namespace Givt.API
{
    public class Program
    {
        //private const string ConfigKeyCommonPrefix = "Common:";
        //private const string ConfigKeyAppPrefix = "API:";
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost
                .CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var settings = config.Build();
                    config
                        //.AddAzureAppConfiguration(options =>
                        //{
                        //    options.Connect(settings.GetConnectionString("AzureAppConfiguration"))
                        //    // order of .Select() is important, as the key/values selected will be layered. Last one takes precedence.
                        //    .Select(ConfigKeyCommonPrefix + KeyFilter.Any, null)
                        //    .Select(ConfigKeyCommonPrefix + KeyFilter.Any, hostingContext.HostingEnvironment.EnvironmentName)
                        //    .Select(ConfigKeyAppPrefix + KeyFilter.Any, null)
                        //    .Select(ConfigKeyAppPrefix + KeyFilter.Any, hostingContext.HostingEnvironment.EnvironmentName)
                        //    // order of .TrimKeyPrefix() is important too, as existing key names will survive collisions. So first one takes precedence.
                        //    .TrimKeyPrefix(ConfigKeyAppPrefix)
                        //    .TrimKeyPrefix(ConfigKeyCommonPrefix)
                        //    ;
                        //})
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) // I think we dont need this anymore right? Bcus AddAzureAppConfig() ?
                        .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables();
                })
                .UseDefaultServiceProvider(x =>
                {
                    x.ValidateOnBuild = false;
                    x.ValidateScopes = false;
                })
                .UseStartup<Startup>()
                .UseUrls("http://*:5000")
                .Build();
        }

// Asp.Net 6.0 minimal API host setup:
    //public static void Main(string[] args)
    //    {
    //        var builder = WebApplication.CreateBuilder(args);

    //        // Add services to the container.

    //        builder.Services.AddControllers();
    //        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    //        builder.Services.AddEndpointsApiExplorer();
    //        builder.Services.AddSwaggerGen();

    //        var app = builder.Build();

    //        // Configure the HTTP request pipeline.
    //        if (app.Environment.IsDevelopment())
    //        {
    //            app.UseSwagger();
    //            app.UseSwaggerUI();
    //        }

    //        app.UseHttpsRedirection();

    //        app.UseAuthorization();

    //        app.MapControllers();

    //        app.Run();
    //    }
    }
}