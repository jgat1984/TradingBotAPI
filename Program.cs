using TradingBotAPI.CoreBot;
using TradingBotAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<TradingService>();
builder.Services.AddSingleton<TradeRepository>();
builder.Services.AddSingleton<PnLRepository>();

// ✅ Global CORS setup
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "https://trading-bot-frontend-xi.vercel.app"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

DatabaseHelper.EnsureSchema();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080);
});

var app = builder.Build();

app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TradingBot API v1");
    c.InjectStylesheet("/swagger-custom.css");
});

// ✅ CORS MUST go here before controllers
app.UseCors();

app.UseAuthorization();

// ✅ Controllers after CORS
app.MapControllers();

app.MapGet("/", () => Results.Text("TradingBotAPI is live 🚀", "text/plain"));
app.MapGet("/health", () => Results.Ok(new { status = "Healthy ✅" }));

app.Run();
