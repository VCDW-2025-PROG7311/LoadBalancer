var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "wwwroot"
});

builder.WebHost.UseUrls("http://0.0.0.0:80");

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/", async () =>
{
    var hostname = Environment.MachineName;

    if (hostname == "api3" || hostname == "api6" || hostname == "api9")
    {
        await Task.Delay(500);
    }

    return $"Hello from {hostname}!";
});

app.Run();