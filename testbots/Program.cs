using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using testbots;
using testbots.DAL.MySql;
using testbots.DAL.MySql.Repository;
using testbots.DAL.SqlLite;
using testbots.DAL.SqlLite.Repository;
using testbots.Service;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        
        // Register Bot configuration
        services.Configure<BotConfiguration>(context.Configuration.GetSection("BotConfiguration"));
        services.AddHttpClient("telegram_bot_client").RemoveAllLoggers()
                .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
                {
                    BotConfiguration? botConfiguration = sp.GetService<IOptions<BotConfiguration>>()?.Value;
                    ArgumentNullException.ThrowIfNull(botConfiguration);
                    TelegramBotClientOptions options = new(botConfiguration.BotToken);
                    return new TelegramBotClient(options, httpClient);
                });

        services.AddDbContext<MySqlContext>(options =>
                   options.UseMySql(context.Configuration.GetConnectionString(MySqlContext.ConnectionStrings),
                                    new MySqlServerVersion(new Version(8, 0, 21))));

        services.AddDbContext<SqlLiteContext>(options =>
                 options.UseSqlite(context.Configuration.GetConnectionString(SqlLiteContext.ConnectionStrings)));

        services.AddScoped<IPostRepository, PostRepository>();

        services.AddScoped<IUserMovieBotService, UserMovieBotService>();

        services.AddScoped<UpdateHandler>();

        services.AddScoped<ReceiverService>();

        services.AddHostedService<PollingService>();
    })
    .Build();

await host.RunAsync();