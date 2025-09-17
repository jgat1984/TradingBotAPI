using TradingBotAPI.CoreBot;      // For DatabaseHelper, Repositories, Models
using TradingBotAPI.Services;     // For TradingService

var builder = WebApplication.CreateBuilder(args);

// Register app services
builder.Services.AddSingleton<TradingService>();
builder.Services.AddSingleton<TradeRepository>();
builder.Services.AddSingleton<PnLRepository>();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Register controllers
builder.Services.AddControllers();

// Register Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Initialize DB
DatabaseHelper.EnsureSchema();

// ✅ Force Kestrel to listen on port 8080 (Render requirement)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080);
});

var app = builder.Build();

// Enable static files (for swagger-custom.css)
app.UseStaticFiles();

// ✅ Enable Swagger UI so routes are visible
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TradingBot API v1");
    c.InjectStylesheet("/swagger-custom.css"); // custom dark theme if you want
});

app.UseCors("AllowReactApp");

// Middleware
app.UseAuthorization();
app.MapControllers();

// ❌ Removed hardcoded localhost ports
// app.Urls.Clear();
// app.Urls.Add("http://localhost:5126");
// app.Urls.Add("https://localhost:7126");

app.Run();
