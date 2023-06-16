using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Superbots.App.Common.Models;
using Superbots.App.Features.Chat.Models;

namespace Superbots.App
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
        {
            // Add services to the container.
            //builder.Services.AddRazorPages();
            //Questo serve per sotituire il path di default per le page di razor perché ho cambiato la struttura delle cartelle nel progetto.
            builder.Services.AddRazorPages(pagesOptions => pagesOptions.RootDirectory = "/AppCoreComponents");
            builder.Services.AddServerSideBlazor();

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //La configurazione CORS ovviamente è pensata per una demo locale
            //Indagare come dovrebbero essere impostate
            builder.Services.AddCors(options =>
            {
                //options.AddDefaultPolicy(builder =>
                //{
                //    builder.WithOrigins("https://localhost:7192")
                //        .AllowAnyHeader()
                //        .AllowAnyMethod();
                //});
                options.AddPolicy("SolutionOneApi", builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });//bisogna dirlo al controller

            });

            builder.Services.AddLogging(logBuilder =>
            {
                logBuilder.AddConsole();
                logBuilder.AddDebug();
            });

            builder.Services.AddMudServices();

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("SqLiteApp");
                options.UseSqlite(connectionString);
            }).AddScoped<IAppSettingsService, AppSettingsService>();

            builder.Services.AddDbContext<ChatDbContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("SqLiteChat");
                options.UseSqlite(connectionString);
            }).AddScoped<IChatService, ChatService>();

            builder.Services.AddScoped<IOpenAiConnector<OpenAiConnector>, OpenAiConnector>();
            return builder;
        }
    }
}
