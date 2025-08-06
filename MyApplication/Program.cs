using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyApplication.Configuration;
using MyApplication.Contracts;
using MyApplication.Data;
using MyApplication.Exceptions;
using MyApplication.Middleware;
using MyApplication.Repository;
using Newtonsoft.Json;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var connectionString = builder.Configuration.GetConnectionString("connectionHotelDB");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<HotelDBContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddIdentityCore<ApiUser>()
    .AddRoles<IdentityRole>().AddEntityFrameworkStores<HotelDBContext>();
builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration));
builder.Services.AddControllers();
builder.Services.AddCors(option =>
    option.AddPolicy("AllowAll", b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = ctx =>
        {
            var auth = ctx.Request.Headers["Authorization"].FirstOrDefault();

            if (auth != null && auth.StartsWith("Bearer "))
            {
                ctx.Token = auth.Substring(7);
            }
            else
            {
                var cookie = ctx.Request.Cookies["jwt"];
                if (cookie != null) ctx.Token = cookie;
            }

            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            var errorDetails = new ErrorDetails
            {
                ErrorType = "Unauthorized",
                ErrorMessage = "You do not have permission to access this resource."
            };
            var errors = JsonConvert.SerializeObject(errorDetails);
            return context.Response.WriteAsync(errors);
        },
        OnForbidden = context =>
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            var errorDetails = new ErrorDetails
            {
                ErrorType = "Forbidden",
                ErrorMessage = "You do not have permission to access this resource."
            };
            var errors = JsonConvert.SerializeObject(errorDetails);
            return context.Response.WriteAsync(errors);
        }
    };
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(MapperConfig));
builder.Services.AddScoped<IAuthManager, AuthManager>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IHotelsRepository, HotelsRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.UseSerilogRequestLogging();
app.MapControllers();
app.Run();