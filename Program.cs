using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Chromophobe.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel;
using System.IO;
public class Startup
{
    public Startup()
    {
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddSingleton<DatabaseHelper>();

        services.AddCors(options =>
       {
           options.AddDefaultPolicy(builder =>
           {
               builder.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
           });
       });
    }


    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseCors();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet("/articles", GetArticles);
            endpoints.MapPost("/articles", CreateArticle);
            endpoints.MapGet("/articleAuthors", GetArticleAuthors);
            endpoints.MapGet("/institutions", GetAllInstitutions);
            endpoints.MapPost("/drop", DropTables);
            endpoints.MapPost("/createWorld", CreateWorld);

            endpoints.MapControllers();
        });
    }

    public async Task GetAllInstitutions(HttpContext context)
    {
        DatabaseHelper databaseHelper = new DatabaseHelper();
        var institutions = await databaseHelper.GetInstitutions();

        // assuming institutions is a List<Institution> or similar collection type
        var json = JsonSerializer.Serialize(institutions);

        await context.Response.WriteAsync(json);
        Console.WriteLine("Institutions: " + json);
    }


    public async Task GetArticles(HttpContext context)
    {
        DatabaseHelper databaseHelper = new DatabaseHelper();
        var articles = databaseHelper.PrintCustomers();
        await context.Response.WriteAsync(articles);
        Console.WriteLine("Articles: " + articles);
    }

    public async Task GetArticleAuthors(HttpContext context)
    {
        DatabaseHelper databaseHelper = new DatabaseHelper();
        await databaseHelper.GetArticlesAuthors();
    }


    public async Task CreateArticle(HttpContext context)
    {
        DatabaseHelper databaseHelper = new DatabaseHelper();

        var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(), new CustomDateTimeConverter() }
        };

        var newArticle = JsonSerializer.Deserialize<Article>(requestBody, jsonOptions);

        if (newArticle != null)
        {
            databaseHelper.InsertArticle(newArticle);
        }
        else
        {
            Console.WriteLine("Article is null");
        }

        var articles = databaseHelper.PrintCustomers();
        await context.Response.WriteAsync(articles);
        // Console.WriteLine("New Article: " + newArticle.title + " " + newArticle.datePublished + " " + newArticle.link + " " + newArticle.Type);
    }

    public Task DropTables(HttpContext context)
    {
        DatabaseHelper databaseHelper = new DatabaseHelper();
        databaseHelper.DropExistingTables();
        context.Response.WriteAsync("Dropped tables");
        return Task.CompletedTask;
    }

    public Task CreateWorld(HttpContext context)
    {
        DatabaseHelper databaseHelper = new DatabaseHelper();
        databaseHelper.CreateArticlesTable();
        databaseHelper.CreateAuthorsTable();
        databaseHelper.CreateArticleAuthorsTable();
        databaseHelper.CreateProvidersTable();
        databaseHelper.CreateTagsTable();
        databaseHelper.CreateProviderTagsTable();
        databaseHelper.CreateInstitutionsTable();
        databaseHelper.CreateInstitutionsTagsTable();
        databaseHelper.InsertAllInstitutions();
        // databaseHelper.InsertInstitutionTags();

        context.Response.WriteAsync("World Created");
        return Task.CompletedTask;
    }


}

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Register services and configure the app using the Startup class
        var startup = new Startup();
        startup.ConfigureServices(builder.Services);
        var app = builder.Build();
        startup.Configure(app, builder.Environment);

        app.Run();
    }
}