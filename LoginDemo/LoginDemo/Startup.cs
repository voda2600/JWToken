using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace LoginDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
            });
            //Добавляем сессию на 60 минут
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //Для корректной работы с версиями 

            //Секретный ключ для шифрования и дешифрования токена
            var SecretKey = Encoding.ASCII.GetBytes("IlnurNagibator911");
            //Добавляем аунтефикацию
            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                //Заменяем стандартные методы аунтефикации на JWTtoken. (JwtBearer) - библиотека.
            })
            .AddJwtBearer(token =>
            {
                /*В этом разделе мы настраиваем Token с секретным ключом,
                 * сроком годности,
                 * потребителем и т. д. Секретный ключ предназначен для шифрования и дешифрования токена.*/
                token.RequireHttpsMetadata = false;
                token.SaveToken = true;
                token.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(SecretKey),
                    ValidateIssuer = true,
                    //базовый URL-адрес вашего приложения
                    ValidIssuer = "http://localhost:45092/",
                    ValidateAudience = true,
                    //Если JWT создается с помощью веб - службы,
                    //то это будет URL - адрес потребителя.
                    ValidAudience = "http://localhost:45092/",
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

       
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            
            app.UseSession();

            //Добавление токена Jwt ко всем входящим заголовкам HTTP-запросов
            app.Use(async (context, next) =>
            {
                var JWToken = context.Session.GetString("JWToken");
                if (!string.IsNullOrEmpty(JWToken))
                {
                    //Важно, что Authorization заголовок и Bearer
                    context.Request.Headers.Add("Authorization", "Bearer " + JWToken);
                }
                await next();
            });
            //UseAuth будет использовать AddJwtBearer, что мы настроили вышек в Configure
            //он будет искать Authorization заголовок в файле HTTP Request
            //будет оценивать и проверять токен в соответствии с конфигурацией, 
            //которую мы установили для токена.
            //Сразу же подключает Identity.User
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
