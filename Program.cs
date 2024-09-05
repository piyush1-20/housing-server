using API.Data;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using API.Middlewares;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;
using dotenv.net;
using CloudinaryDotNet;
DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
var cloudinaryUrl = Environment.GetEnvironmentVariable("CLOUDINARY_URL");
Cloudinary cloudinary = new Cloudinary(cloudinaryUrl);

cloudinary.Api.Secure = true;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


//db related code
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddCors();

//adding automapper as service to use it in application
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

//adding newtonsoftjson to use patch 
builder.Services.AddControllers().AddNewtonsoftJson();

//mapping 
builder.Services.AddScoped<IUnitofWork, UnitofWork>();
builder.Services.AddScoped<IPhotoService, PhotoService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
 
var secretKey = builder.Configuration.GetSection("AppSettings:Key").Value;
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(op =>
{
    op.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = key,
    };

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.ConfigureExceptionHandler(app.Environment);
app.UseMiddleware<ExceptionMiddleware>();


app.UseCors(m => { m.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
