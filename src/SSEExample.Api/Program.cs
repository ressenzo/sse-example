var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Host.ConfigureHostOptions(opt =>
{
    opt.ShutdownTimeout = TimeSpan.FromSeconds(0); 
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowAll",
        policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();
app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/items", async (HttpContext httpContext, CancellationToken ct) =>
{
    httpContext.Response.Headers.Append("Content-Type", "text/event-stream");
    httpContext.Response.Headers.Append("Cache-Control", "no-cache");
    httpContext.Response.Headers.Append("Connection", "keep-alive");

    await httpContext.Response.Body.FlushAsync(ct);

    var count = 1;
    while (!ct.IsCancellationRequested)
    {
        using HttpClient httpClient = new();
        var result = await httpClient.GetAsync($"https://jsonplaceholder.typicode.com/todos/{count}", ct);
        var resultData = await result.Content.ReadFromJsonAsync<Todo>(ct);

        var @event = count % 5 == 0 ?
            "special" :
            "normal";
        string data = 
            $"id: {Guid.NewGuid()}\n" +
            $"event: {@event}\n" +
            $"data: {resultData!.Title}\n\n";
        await httpContext.Response.WriteAsync(data, ct);
        await httpContext.Response.Body.FlushAsync(ct);
        count++;
        
        await Task.Delay(TimeSpan.FromSeconds(2), ct);
    }
})
.WithName("GetItems")
.WithOpenApi();

app.Run();

record Todo
{
    public string Title { get; set; } = default!;
}
