using InventoryMg.BLL.Extensions;
using InventoryMg.DAL.Configurations;
using InventoryMg.DAL.Entities;
using InventoryMg.DAL.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

namespace InventoryMg.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "InventoryManager", Version = "v1" });


                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description =
                        "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\""
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            },
    });
            });

           builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 104857600; // 100 MB
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Open", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            builder.Services.AddDbContext<ApplicationDbContext>(opts =>
            {
                var defaultConn = builder.Configuration.GetSection("ConnectionStrings")["DefaultConn"];

                opts.UseSqlServer(defaultConn, x => x.MigrationsAssembly("InventoryMg.DAL")
                );

            });


            builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));
            var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtConfig:Secret").Value);

            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false, //for development, true when u get to prod
                ValidateAudience = false, //for development, true when u get to prod
                RequireExpirationTime = false,//for development,  when u get to prod implement refresh token 
                ValidateLifetime = true
            };


            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })

               .AddJwtBearer(jwt =>
               {
                   jwt.SaveToken = true;
                   jwt.TokenValidationParameters = tokenValidationParameters;
               });

            builder.Services.AddSingleton(tokenValidationParameters);

            builder.Services.Configure<FormOptions>(options =>
              {
                  options.MultipartBodyLengthLimit = 104857600; // 100 MB
              });

            builder.Services.AddIdentity<UserProfile, AppRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.RegisterServices();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddAutoMapper(Assembly.Load("InventoryMg.BLL"));
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();
            app.AddGlobalErrorHandler();


            await SeedData.EnsurePopulatedAsync(app);
            app.Run();

           
        }
    }
}