using TaskService.Controllers;
using TaskService.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<TodoDb>(serviceProvider =>
{
    var config = serviceProvider.GetRequiredService<IConfiguration>();
    var uri = config["TodoDbSettings:Uri"];
    var username = config["TodoDbSettings:Username"];
    var password = config["TodoDbSettings:Password"];
    return new TodoDb(uri, username, password);
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
