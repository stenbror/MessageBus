
using System.Linq;

var data_storage = new List<MessageStorage>();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// POST: publish endpoint /////////////////////////////////////////////////////
app.MapPost("/publish", (Message msg) => {
    try {
        var element = new MessageStorage(Guid.NewGuid(), DateTime.Now, msg.data);
        data_storage.Add(element);

        return Results.Ok<String>(element.Guid.ToString());
    }
    catch {
        return Results.BadRequest();
    }
})
.WithName("Publish")
.WithOpenApi();;

// GET: subscribe endpoint ////////////////////////////////////////////////////
app.MapGet("/subscribe", (DateTime from) => {
    try {
        return Results.Ok(data_storage.ToArray().Where<MessageStorage>(el => el.timestamp >= from) );
    }
    catch {
        return Results.BadRequest();
    }
})
.WithName("Subscribe")
.WithOpenApi();

app.Run();

// Data structure used in messagebus //////////////////////////////////////////
record Message(string data);
record MessageStorage(Guid Guid, DateTime timestamp, string data);