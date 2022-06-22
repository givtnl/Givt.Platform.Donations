//using Auth0.AspNetCore.Authentication;
using AutoMapper;
using Givt.API.Filters;
using Givt.API.Handlers;
using Givt.API.MiddleWare;
using Givt.API.Options;
using Givt.Donations.Domain.Mappings;
using Givt.Donations.Infrastructure.Behaviors;
using Givt.Donations.Infrastructure.Loggers;
using Givt.Donations.Persistence.DbContexts;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
//using Pomelo.EntityFrameworkCore.MySql;
using Serilog.Sinks.Http.Logger;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;

namespace Givt.API;

public class Startup
{
    public IHostEnvironment HostEnvironment { get; }
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
    {
        Configuration = configuration;
        HostEnvironment = hostEnvironment;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        ConfigureOptions(services);

        services.AddSingleton<ILog, LogitHttpLogger>(x => new LogitHttpLogger(Configuration["LogitConfiguration:Tag"], Configuration["LogitConfiguration:Key"]));

        services.AddSingleton(new MapperConfiguration(mc =>
        {
            mc.AddProfiles(new List<Profile>
            {
                new DonationHistoryMappingProfile(),
                //new MediumMappingProfile(),
                //new OrganisationMappingProfile(),
                //new ReportMappingProfile(),

                //new DataMediumMappingProfile(),
                //new DataOrganisationMappingProfile(),
                //new DonationReportMappingProfile(),
            });
        }).CreateMapper());

        //services.AddSingleton<ISinglePaymentService, StripeIntegration>();
        //services.AddTransient<IPdfService, GooglePdfService>();
        //services.AddSingleton<IFileStorage, AzureFileStorage>();

        var jwtSection = Configuration.GetSection(JwtOptions.SectionName);
        services.Configure<JwtOptions>(jwtSection)
            .AddSingleton(sp => sp.GetRequiredService<IOptions<JwtOptions>>().Value);

        //services.Configure<StripeOptions>(Configuration.GetSection(StripeOptions.SectionName))
        //    .AddSingleton(sp => sp.GetRequiredService<IOptions<StripeOptions>>().Value);

        //services.Configure<PostmarkOptions>(Configuration.GetSection(PostmarkOptions.SectionName))
        //    .AddSingleton(sp => sp.GetRequiredService<IOptions<PostmarkOptions>>().Value);

        //services.Configure<GoogleDocsOptions>(Configuration.GetSection(GoogleDocsOptions.SectionName))
        //    .AddSingleton(sp => sp.GetRequiredService<IOptions<GoogleDocsOptions>>().Value);

        //services.Configure<AzureBlobStorageOptions>(Configuration.GetSection(AzureBlobStorageOptions.SectionName))
        //    .AddSingleton(sp => sp.GetRequiredService<IOptions<AzureBlobStorageOptions>>().Value);

        //services.AddMediatR(
        //    typeof(GetOrganisationByMediumIdQuery).Assembly,            // Givt.OnlineCheckout.Business
        //    typeof(ISinglePaymentNotification).Assembly,                // Givt.OnlineCheckout.Integrations.Interfaces
        //    typeof(StripeIntegration).Assembly,                         // Givt.OnlineCheckout.Integrations.Stripe
        //    typeof(PostmarkEmailService<IEmailNotification>).Assembly   // Givt.OnlineCheckout.Integrations.Postmark
        //);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        services.AddTransient(typeof(JwtTokenHandler));

        var jwtOptions = jwtSection.Get<JwtOptions>();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.IssuerSigningKey));
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
        services
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
                 options.Authority = $"https://{Configuration["Auth0:Domain"]}/";
                 options.Audience = Configuration["Auth0:Audience"]; // e.g. https://api.givtapp.net = Auth0's Api Identifier                     
             });
        services.AddAuthorization(options =>
        {
            var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                JwtBearerDefaults.AuthenticationScheme, "Auth0");
            defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
            options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            options.AddPolicy("Retool", policy => policy.RequireClaim("sub", Configuration["Auth0:ClientId"] + "@clients"));
        });

        var connectionString = Configuration.GetConnectionString("GivtDonationsDb");
        services.AddDbContext<DonationsContext>(
            options => options
            // MySQL/MariaDB:
            //.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            // PostgreSQL / CockroachDB
            .UseNpgsql(connectionString)
#if DEBUG
            // The following three options help with debugging, but should
            // be changed or removed for production.
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
#endif
        );

        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen((options) =>
        {
            options.SwaggerDoc("v2", new OpenApiInfo
            {
                Version = "v2",
                Title = "Givt Platform API",
                Description = "The Givt platform API microservice."
            });
        });
        services.AddControllers();
        services.AddMvcCore(x => { x.Filters.Add<CustomExceptionFilter>(); })
            .AddControllersAsServices()
            .AddMvcOptions(o => o.EnableEndpointRouting = false)
            .AddCors(o => o.AddPolicy("EnableAll", builder =>
            {
                builder.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin();
            }));
    }

    public void Configure(IApplicationBuilder app, IHostEnvironment env, ILog logger)
    {
        logger.Information($"Givt.API started on {env.EnvironmentName}");

        // Configure the HTTP request pipeline.
        if (!env.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }
        var supportedCultures = new[] { "en-US", "en-GB", "nl-NL", "en-NL", "nl-BE", "en-BE", "de-DE" };

        app.UseRequestLocalization(options =>
            options
                .AddSupportedCultures(supportedCultures)
        ); // => This is for localizing the resources from the client

        app.UseSwagger();
        app.UseSwaggerUI((options) =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        });

        app.UseAuthentication(); // To support JWT Bearer tokens, and Auth0
        app.UseAuthorization(); // Auth0
        app.UseMiddleware<MultipleSchemaAuthenticationMiddleware>();

        app.UseCors("EnableAll")
            .UseMvc();
    }

    public void ConfigureOptions(IServiceCollection services)
    {
        //services.AddAzureAppConfiguration();
        services.AddSwaggerGen(options =>
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
    }
}