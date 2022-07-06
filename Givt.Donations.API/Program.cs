using AutoMapper;
using Givt.Donations.API.MiddleWare;
using Givt.Donations.Business.Infrastructure.Health;
using Givt.Donations.Persistence.DbContexts;
using Givt.Platform.Common.Filters;
using Givt.Platform.Common.Infrastructure.Behaviors;
using Givt.Platform.Common.Loggers;
using Givt.Platform.JWT.Handlers;
using Givt.Platform.JWT.Options;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog.Sinks.Http.Logger;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;

namespace Givt.Donations.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Build a config object, using env vars and JSON providers.
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // Add services to the container.

            // chunks of configuration
            var jwtSection = config.GetSection(JwtOptions.SectionName);
            builder.Services
                .Configure<JwtOptions>(jwtSection)
                .AddSingleton(sp => sp.GetRequiredService<IOptions<JwtOptions>>().Value);

            // logging
            var logitOptions = new LogitHttpLoggerOptions();
            config.GetSection(LogitHttpLoggerOptions.SectionName).Bind(logitOptions);
            var logger = new LogitHttpLogger(logitOptions);
            builder.Services.AddSingleton<ILog, LogitHttpLogger>(x => logger);

            logger.Information($"Givt.Donations.API started on {builder.Environment.EnvironmentName}");
            Console.WriteLine($"Givt.Donations.API started on {builder.Environment.EnvironmentName}");

            var connectionString = config.GetConnectionString("GivtDonationsDb");
            LogConnectionString(logger, connectionString);
            builder.Services.AddDbContext<DonationsContext>(options => options
                .UseNpgsql(connectionString)
#if DEBUG
                // The following three options help with debugging, but should
                // be changed or removed for production.
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
#endif
            );
            builder.Services.AddHealthChecks()
                .AddCheck<DbHealthCheck>("Database", failureStatus: HealthStatus.Unhealthy, tags: new[] { "ready" });


            builder.Services.AddSingleton(new MapperConfiguration(mc =>
            {
                mc.AddProfiles(new List<Profile>
                {
                    // API
                    //new MediumMappingProfile(),

                    // Business
                    //new CampaignCampaignInfoMappingProfile(),
                });
            }).CreateMapper());

            //builder.Services.Configure<StripeOptions>(Configuration.GetSection(StripeOptions.SectionName))
            //    .AddSingleton(sp => sp.GetRequiredService<IOptions<StripeOptions>>().Value);
            //builder.Services.AddSingleton<ISinglePaymentService, StripeIntegration>();

            //builder.Services.Configure<PostmarkOptions>(Configuration.GetSection(PostmarkOptions.SectionName))
            //    .AddSingleton(sp => sp.GetRequiredService<IOptions<PostmarkOptions>>().Value);

            //builder.Services.Configure<GoogleDocsOptions>(Configuration.GetSection(GoogleDocsOptions.SectionName))
            //    .AddSingleton(sp => sp.GetRequiredService<IOptions<GoogleDocsOptions>>().Value);
            //builder.Services.AddTransient<IPdfService, GooglePdfService>();

            //builder.Services.AddMediatR(
            //    //typeof(UserAuthorisationQuery).Assembly                   // Givt.Core.Business
            ////    typeof(ISinglePaymentNotification).Assembly,                // Givt.OnlineCheckout.Integrations.Interfaces
            ////    typeof(StripeIntegration).Assembly,                         // Givt.OnlineCheckout.Integrations.Stripe
            ////    typeof(PostmarkEmailService<IEmailNotification>).Assembly   // Givt.OnlineCheckout.Integrations.Postmark
            //);
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            //builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CampaignGetPipeline<,>));

            builder.Services.AddTransient(typeof(JwtTokenHandler));

            var jwtOptions = jwtSection.Get<JwtOptions>();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.IssuerSigningKey));
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience, // TODO: decide if we want this
                        IssuerSigningKey = key,
                        ClockSkew = TimeSpan.FromMinutes(1),
                    };
                })
                .AddJwtBearer("Auth0", options =>
                {
                    options.Authority = $"https://{config["Auth0:Domain"]}/";
                    options.Audience = config["Auth0:Audience"]; // e.g. https://api.givtapp.net = Auth0's Api Identifier                     
                });
            builder.Services.AddAuthorization(options =>
            {
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                    JwtBearerDefaults.AuthenticationScheme, "Auth0");
                defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
                options.AddPolicy("Retool", policy => policy.RequireClaim("sub", config["Auth0:ClientId"] + "@clients"));
            });


            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
            builder.Services.AddControllers();
            builder.Services.AddRouting(options => options.LowercaseUrls = true);
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddMvcCore(x => { x.Filters.Add<CustomExceptionFilter>(); })
                .AddControllersAsServices()
                .AddMvcOptions(o => o.EnableEndpointRouting = false)
                .AddCors(o => o.AddPolicy("EnableAll", policyBuilder =>
                {
                    policyBuilder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin();
                }));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(2, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });

            builder.Services.AddVersionedApiExplorer(config =>
            {
                config.GroupNameFormat = "'v'VVV";
                config.SubstituteApiVersionInUrl = true;
            });

            builder.Services.AddSwaggerGen(options =>
            {
                // We need to get the IApiVersionDescriptionProvider buried somewhere in the services. 
                // Through the "app" we build later, this would be easy, but we can't access that now
                // because we need to finish configuration first.
#pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
                using (var serviceProvider = builder.Services.BuildServiceProvider())
                {
                    var descriptionProvider = serviceProvider.GetRequiredService<IApiVersionDescriptionProvider>();
                    var assembly = Assembly.GetExecutingAssembly();
                    String assemblyDescription = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
                    String productName = assembly.GetCustomAttribute<AssemblyProductAttribute>().Product;
                    foreach (var description in descriptionProvider.ApiVersionDescriptions)
                    {
                        options.SwaggerDoc(description.GroupName, new OpenApiInfo()
                        {
                            Title = $"{productName} {description.ApiVersion}",
                            Version = description.ApiVersion.ToString(),
                            Description = assemblyDescription + (description.IsDeprecated ? " - DEPRECATED" : "")
                        });
                    }
                }
#pragma warning restore ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
                // integrate xml comments  
                var currentAssembly = Assembly.GetExecutingAssembly();
                var xmlDocs = currentAssembly.GetReferencedAssemblies()
                    .Union(new AssemblyName[] { currentAssembly.GetName() })
                    .Select(a => Path.Combine(Path.GetDirectoryName(currentAssembly.Location), $"{a.Name}.xml"))
                    .Where(f => File.Exists(f))
                    .ToArray();
                Array.ForEach(xmlDocs, (d) => { options.IncludeXmlComments(d); });

                // Filter out `api-version` parameters globally
                options.OperationFilter<ApiVersionFilter>();
            });

            var app = builder.Build();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                //options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");
            });

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            var supportedCultures = new[] { "en-US", "en-GB", "nl-NL", "en-NL", "nl-BE", "en-BE", "de-DE" };

            app.UseRequestLocalization(options =>
                options.AddSupportedCultures(supportedCultures)
            ); // => This is for localizing the resources from the client

            app.UseAuthentication(); // To support JWT Bearer tokens, and Auth0
            app.UseAuthorization();

            app.UseMiddleware<MultipleSchemaAuthenticationMiddleware>();

            app.MapControllers();
            app.UseCors("EnableAll");
            //app.UseMvc();

            app.Urls.Clear();
            app.Urls.Add("http://*:5000");

            app.Run();
        }

        private static void LogConnectionString(LogitHttpLogger logger, string connectionString)
        {
            const string msg = "Using connection string {0}";
            var connectionString_noPW = RemovePassword(connectionString);
            logger.Information(msg, new[] { connectionString_noPW });
            Console.WriteLine(msg, new[] { connectionString_noPW });
        }

        private static string RemovePassword(string connectionString)
        {
            const string PASSWORD = "Password=";

            if (String.IsNullOrEmpty(connectionString))
                return connectionString;
            var p1 = connectionString.IndexOf(PASSWORD, StringComparison.InvariantCultureIgnoreCase);
            if (p1 >= 0)
            {
                var sb = new StringBuilder();
                sb.Append(connectionString[..(p1 + PASSWORD.Length)]);
                sb.Append("<removed>");
                var p2 = connectionString.IndexOf(';', p1);
                if (p2 >= 0)
                    sb.Append(connectionString[p2..]);
                return sb.ToString();
            }
            else
                return connectionString;
        }
    }
}