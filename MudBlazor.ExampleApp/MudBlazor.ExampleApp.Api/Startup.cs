using Blazr.RenderState;
using Blazr.RenderState.Server;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Options;
using MudBlazor.ExampleApp.Api.Components;
using MudBlazor.ExampleApp.Api.Configuration;
using MudBlazor.ExampleApp.Api.Filters;
using MudBlazor.ExampleApp.Application;
using MudBlazor.ExampleApp.Application.Account;
using MudBlazor.ExampleApp.Client;
using MudBlazor.ExampleApp.Client.Common.Auth;
using MudBlazor.ExampleApp.Infrastructure;
using MudBlazor.Services;
using Serilog;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Startup", Version = "1.0")]

namespace MudBlazor.ExampleApp.Api
{
    [IntentManaged(Mode.Merge)]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Issues with auth throwing 401:
        // See: https://learn.microsoft.com/en-us/aspnet/core/blazor/security/?view=aspnetcore-9.0&tabs=visual-studio#manage-authentication-state-in-blazor-web-apps
        [IntentManaged(Mode.Fully, Comments = Mode.Ignore)]
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(
                opt =>
                {
                    opt.Filters.Add<ExceptionFilter>();
                });
            services.AddApplication(Configuration);
            services.ConfigureApplicationSecurity(Configuration);
            services.ConfigureHealthChecks(Configuration);
            services.ConfigureIdentity();
            services.ConfigureProblemDetails();
            services.ConfigureApiVersioning();
            services.AddInfrastructure(Configuration);
            services.ConfigureSwagger(Configuration);
            services.AddClientServices(Configuration);
            services.AddTransient<IAccountEmailSender, AccountEmailSender>();
            //services.AddScoped<IAuthService, PlaceholderAuthService>();
            services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents();

            // [IntentIgnore]
            services.AddHttpContextAccessor();
            // [IntentIgnore]
            services.AddScoped<IBlazrRenderStateService, ServerRenderStateService>();

            services.AddMudServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSerilogRequestLogging();
            app.UseExceptionHandler();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();
            app.UseAntiforgery();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultHealthChecks();
                endpoints.MapControllers();

                endpoints.MapRazorComponents<App>()
                    .AddInteractiveServerRenderMode()
                    .AddInteractiveWebAssemblyRenderMode()
                    .AddAdditionalAssemblies(typeof(Client._Imports).Assembly)
                    ;
            });
            app.UseSwashbuckle(Configuration);
        }
    }
}