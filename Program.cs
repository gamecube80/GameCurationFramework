using GameCurationFramework.Helpers;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddMemoryCache();

builder.Services.AddSingleton(sp => {
    var configuration = sp.GetRequiredService<IConfiguration>();

    var endpoint = configuration["CosmosDb:AccountEndpoint"];
    var key = configuration["CosmosDb:AccountKey"];

    return new CosmosClient(endpoint, key);
});

builder.Services.AddScoped<GameDataService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if(!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
