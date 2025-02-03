using Akka.Actor;
using Core.Actors;
using Presentation.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSignalR();

var actorSystem = ActorSystem.Create("ChatSystem");
var chatCoordinator = actorSystem.ActorOf(Props.Create<ChatCoordinatorActor>(), "chatCoordinator");

builder.Services.AddSingleton(actorSystem);
builder.Services.AddSingleton(chatCoordinator);

builder.Services.AddCors(options =>
{
    options.AddPolicy("chatApp", builder =>
    {
        builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors("chatApp");

app.MapHub<ChatHub>("/chat");

app.UseHttpsRedirection();

app.Run();
