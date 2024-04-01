using API.Hub;
using API.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// add service ,SignalR
builder.Services.AddSignalR();


// ID
builder.Services.AddSingleton<IDictionary<string,  UserRoom>>(inst => 
         new Dictionary<string, UserRoom>()); // here we create a new instance

// CORS :
builder.Services.AddCors(policy =>
{
    policy.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseCors();

app.UseEndpoints(endpoint =>
{
    endpoint.MapHub<ChatHub>("/chat");
});



app.MapControllers();

app.Run();
