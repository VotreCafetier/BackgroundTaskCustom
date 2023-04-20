using Microsoft.EntityFrameworkCore;
using StatusAPICore;
using System.Text.Json;
using System.Text;
using StatusAPI.DTO;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();

// contexts
builder.Services.AddDbContext<DBContext>(options => options.UseSqlite(@"Data Source=../NetworkMap.db"));

// background job
builder.Services.AddHostedService<PingService>().AddScoped<PingService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
app.UseHttpsRedirection();


app.MapGet("/", async (IDistributedCache cache) =>
{
    var encoded = await cache.GetAsync(builder.Configuration["Cache"]);
    return JsonSerializer.Deserialize<CardDTO>(Encoding.UTF8.GetString(encoded));
});

app.Run();