using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement
{
    public class Startup
    {
        private IConfiguration _config;
        
        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //System.Action<MvcOptions> action = (mvcOptions) => { mvcOptions.EnableEndpointRouting = false; };
            //services.AddMvc(action);

            // The name of the connection string (i.e. "EmployeeDBConnection") is defined in appsettings.json
            // Trusted_connection = true
            // Integrated Security = SSPI   |   They all specify using Integrated Windows Authentication over SQL Server authentication
            // Integrated Security = true
            services.AddDbContextPool<AppDBContext>(options => options.UseSqlServer(_config.GetConnectionString("EmployeeDBConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                  {
                     options.Password.RequiredLength = 6;
                     options.Password.RequiredUniqueChars = 3;
                  }).AddEntityFrameworkStores<AppDBContext>();

            /*
             * This allows for the configuration of password-complexity for our application.
             * It can be done as well during AddIdentity service-initialization as seen above.
    
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 10;
                options.Password.RequiredUniqueChars = 3;
            }
            );

            */

            services.AddMvc(options => options.EnableEndpointRouting = false);

            /*
             * This makes it so a user must be logged-in in order to reach any method within the app
            services.AddMvc(options => {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                
                options.Filters.Add(new AuthorizeFilter(policy));
            */

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Administration/AccessDenied");
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminRolePolicy" , policy => policy.RequireRole("Admin")
                                                                      .RequireClaim("Create Role"));

                options.AddPolicy("SuperAdminPolicy", policy => policy.RequireRole("Admin")
                                                                      .RequireClaim("Create Role")
                                                                      .RequireClaim("Edit Role")
                                                                      .RequireClaim("Delete Role"));

                // An example of a policy that accepts a claim-holder that possesses one of the given values
                // options.AddPolicy("AllowedCountryPolicy", policy => policy.RequireClaim("Country", "USA", "India", "UK"));
            });

            services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP-request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            else
            {
                app.UseExceptionHandler("/Error");

                // The '{0}' placeholder automatically receives the HTTP-status code
                // Re-executes the pipeline and then replaces the status code to the original non-success one
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
                // Because it re-executes the pipeline instead of redirecting it, it preserves the original URL
            }

            app.UseAuthentication();

            //app.UseMvcWithDefaultRoute(); // This applies the equivalent to the code below

            //app.UseMvc(routes => {
            //    routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            //});

            app.UseMvc();

            app.UseFileServer();




            // This piece of code creates a middleware that awaits for the Response-object and forward-adds
            // a string to the end of the Response-stream

            //app.Use(async (context, next) =>
            //{
            //    await next();
            //    await context.Response.WriteAsync(". In construction...");
            //});
            //
            //app.MapGet("/", () => System.Diagnostics.Process.GetCurrentProcess().ProcessName);
        }
    }
}
