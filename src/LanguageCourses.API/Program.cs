using System.Reflection;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using LanguageCourses.Infrastructure.EF;
using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<LanguageCoursesDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

// MediatR + FluentValidation (varrendo o assembly da Application)
var appAssembly = Assembly.Load("LanguageCourses.Application");
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(appAssembly));
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(appAssembly);

// MVC + JSON
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Swagger (doc)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Language Courses API",
        Version = "v1",
        Description = "API para gestão de cursos de idiomas (MVC + DDD + CQRS).",
        Contact = new OpenApiContact { Name = "Eduardo Ruan Guimarães Fonseca", Url = new Uri("https://github.com/Eduardo-Ruan-G-Fonseca/language-courses") }
    });

    // XML comments (Controllers e DTOs)
    var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly);
    foreach (var xml in xmlFiles)
        c.IncludeXmlComments(xml, includeControllerXmlComments: true);

    // Evita conflito de nomes de schema
    c.CustomSchemaIds(t => t.FullName);
});

var app = builder.Build();

// ==== Tratamento global de erros (mapeia para 404/400/500) ====
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var feature = context.Features.Get<IExceptionHandlerPathFeature>();
        var ex = feature?.Error;

        var status = StatusCodes.Status500InternalServerError;
        var title = "Erro interno do servidor";

        if (ex is KeyNotFoundException)
        {
            status = StatusCodes.Status404NotFound;
            title = "Recurso não encontrado";
        }
        else if (ex is InvalidOperationException)
        {
            status = StatusCodes.Status400BadRequest;
            title = "Operação inválida";
        }

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Title = title,
            Status = status,
            Detail = ex?.Message
        };

        await context.Response.WriteAsJsonAsync(problem);
    });
});

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Language Courses API v1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.MapControllers();
app.Run();
