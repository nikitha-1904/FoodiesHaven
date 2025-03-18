using FoodiesHaven.Models;
using Microsoft.EntityFrameworkCore;
using FoodiesHaven.Data;
using FoodiesHaven.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer; //Namespace for JWT (JSON Web Token) authentication.
using Microsoft.IdentityModel.Tokens; //Namespace for token validation.
using Microsoft.OpenApi.Models; //Namespace for Swagger/OpenAPI models.
using System.Text; 
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args); //creates instance to configure and build the web application.

// Add services to the container.
builder.Services.AddControllers() // Adds services for controllers to the dependency injection container.
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddScoped<CartController>();
builder.Services.AddEndpointsApiExplorer(); //connecting endpoint to controller
builder.Services.AddSwaggerGen(); //Adds services for generating Swagger documentation.

builder.Services.AddDbContext<EFCoreDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("EFCoreDBConnection"));
});

// Add services for authentication and authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["jwtSettings:issuer"],
        ValidAudience = builder.Configuration["jwtSettings:audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwtSettings:key"]))
    };
});

builder.Services.AddAuthorization();

// Add Swagger services
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Foodie's Haven API", Version = "v1" });
    //Adds a security definition for JWT bearer tokens.
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    //Adds a security requirement for the Swagger documentation.
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
            new string[] { }
        }
    });
});

var app = builder.Build();
//to build web appilication

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); //redirects http to https

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
//maps controller routes

app.Run();
//to run the application 


//DI - where services are injected into the application, where we register and configure the service
//Builder -To configure and build the web application
//Singleton Pattern: Used in builder.Services.AddSwaggerGen() to ensure a single instance of Swagger services.
//builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
