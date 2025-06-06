using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EasyUiBackend.Infrastructure.Persistence;
using EasyUiBackend.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EasyUiBackend.Domain.Interfaces;
using EasyUiBackend.Infrastructure.Services;
using EasyUiBackend.Infrastructure.Seeds;
using EasyUiBackend.Infrastructure.Repositories;
using Microsoft.OpenApi.Models;

using System.Text.Json.Serialization;

using AutoMapper;
using EasyUiBackend.Infrastructure.Mappings;
using System.Text.Encodings.Web;
using EasyUiBackend.Domain.Models.Email;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.WriteIndented = true;
		options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
		options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
	});
// Cấu hình cors
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowReactApp", policy =>
	{
		policy.WithOrigins(
				"https://easy-ui-eight.vercel.app",
				"http://localhost:3000",
				"http://localhost:8080",
				"https://easy-ui-backend.onrender.com"
			)
			  .AllowAnyHeader()
			  .AllowAnyMethod()
			  .AllowCredentials();

	});
});

// Cấu hình Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "EasyUI API",
		Version = "v1",
		Description = "API for EasyUI Backend"
	});

	// Cấu hình JWT Authentication trong Swagger
	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer"
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
		}
	});
});

// Cấu hình DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseNpgsql(connectionString));

// Cấu hình Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
	// Password settings
	options.Password.RequireDigit = true;
	options.Password.RequiredLength = 8;
	options.Password.RequireNonAlphanumeric = true;
	options.Password.RequireUppercase = true;
	options.Password.RequireLowercase = true;

	// Lockout settings
	options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
	options.Lockout.MaxFailedAccessAttempts = 5;
	options.Lockout.AllowedForNewUsers = true;

	// User settings
	options.User.RequireUniqueEmail = true;
	options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Cấu hình Authentication
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.SaveToken = true;
	options.RequireHttpsMetadata = false;
	var jwtSecret = builder.Configuration["JWT:Secret"];
	if (string.IsNullOrEmpty(jwtSecret))
	{
		throw new InvalidOperationException("JWT:Secret is not configured");
	}

	options.TokenValidationParameters = new TokenValidationParameters()
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidAudience = builder.Configuration["JWT:ValidAudience"],
		ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
	};
});

// Thêm vào phần services
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IUIComponentRepository, UIComponentRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IAboutUsRepository, AboutUsRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IPaymentService, MomoPaymentService>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IUserFollowRepository, UserFollowRepository>();
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddAutoMapper(typeof(CategoryMapping).Assembly);

// Configure Mail Settings
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

// Configure App Settings
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

// Add HttpClient
builder.Services.AddHttpClient();

var app = builder.Build();
// Sau phần app.UseRouting() và trước app.UseEndpoints() hoặc app.MapControllers()
app.UseCors("AllowReactApp");
// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "EasyUI API V1");
		//c.RoutePrefix = string.Empty; // Để Swagger UI hiển thị ở root URL
	});
}

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Seed Data
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	try
	{
		var context = services.GetRequiredService<AppDbContext>();
		var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
		var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
		await DefaultData.SeedDataAsync(context, userManager, roleManager);
	}
	catch (Exception ex)
	{
		var logger = services.GetRequiredService<ILogger<Program>>();
		logger.LogError(ex, "An error occurred while seeding the database.");
	}
}

app.Run();