using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Reflection.Metadata;
using UserService.Data;
using UserService.Profiles;
using UserService.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("UserConnection");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    document => {
        document.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "UserService API",
            Version = "v1",
            Description = "API de Login e Cadastro de Usuários",
        });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    document.IncludeXmlComments(xmlPath); }
    );
builder.Services.AddDbContext<UserContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
builder.Services.AddAutoMapper(typeof(UserProfile));
builder.Services.AddScoped<PasswordHandler>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<TokenHandler>();
builder.Services.AddScoped<UserMainService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
