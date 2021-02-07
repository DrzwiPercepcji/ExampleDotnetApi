using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using GameLobbyApi.Services;
using GameLobbyApi.Contexts;
using GameLobbyApi.Utils;
using System.Text.Json.Serialization;
using System.Text;

namespace GameLobbyApi
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
			services.AddControllers().AddJsonOptions(options =>
			{
				options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
				options.JsonSerializerOptions.IgnoreNullValues = true;
			});

			services.AddDbContext<GameLobbyContext>(options =>
				options.UseInMemoryDatabase("GameLobby")
			);

			IConfigurationSection appSettingsSection = Configuration.GetSection("AppSettings");
			services.Configure<AppSettings>(appSettingsSection);

			AppSettings appSettings = appSettingsSection.Get<AppSettings>();
			byte[] key = Encoding.ASCII.GetBytes(appSettings.Secret);

			AuthenticationBuilder builder = services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			});

			builder.AddJwtBearer(options =>
			{
				options.RequireHttpsMetadata = false;
				options.SaveToken = true;
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidateIssuer = false,
					ValidateAudience = false
				};
			});

			services.AddScoped<SecurityService>();
			services.AddScoped<PlayerService>();

			services.AddApiVersioning(options =>
			{
				options.ReportApiVersions = true;
				options.AssumeDefaultVersionWhenUnspecified = true;
				options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
				options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
			});

			services.AddSwaggerGen(options =>
			{
				options.CustomSchemaIds(type => type.ToString());

				OpenApiSecurityScheme securityScheme = new OpenApiSecurityScheme
				{
					Description = "JWT Authorization header using the Bearer scheme.",
					Name = "Authorization",
					In = ParameterLocation.Header,
					Type = SecuritySchemeType.Http,
					Scheme = "bearer",
					BearerFormat = "JWT",
					Reference = new OpenApiReference
					{
						Id = JwtBearerDefaults.AuthenticationScheme,
						Type = ReferenceType.SecurityScheme
					}
				};

				options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);
				options.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{securityScheme, new string[] { }}
				});
			});
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, GameLobbyContext context)
		{
			context.Database.EnsureCreated();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});

			app.UseSwagger();
			app.UseSwaggerUI(options =>
			{
				options.SwaggerEndpoint("/swagger/v1/swagger.json", "GameLobby API V1");
			});
		}
	}
}
