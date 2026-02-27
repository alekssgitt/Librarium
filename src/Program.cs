using Librarium;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.RegisterCustomServices();

var app = builder.Build();

app.MapControllers();
app.UseSwagger();
app.Run();