using InventoryPortfolio.Data;
using InventoryPortfolio.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString =
    builder.Configuration.GetConnectionString(
        "DefaultConnection")
    ?? throw new InvalidOperationException(
        "接続文字列が設定されていません。");

// DB接続
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(connectionString));

// サービスの登録
builder.Services.AddScoped<IProductService,ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(o => o.SwaggerEndpoint("/openapi/v1.json", "v1")); // 追加
}

app.UseAuthorization();

app.MapControllers();

app.Run();
