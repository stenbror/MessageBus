
using System.Linq;

var channel_storage = new Dictionary<string, List<MessageStorage>>();

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
        var element = new MessageStorage(Guid.NewGuid(), DateTime.Now, msg.version, msg.data);

        if (channel_storage.ContainsKey(msg.channel)) {
            channel_storage[msg.channel].Add(element);
        }
        else {
            var data_storage = new List<MessageStorage>();
            data_storage.Add(element);
            channel_storage[msg.channel] = data_storage;
        }

        return Results.Ok<ResultMessage>(new ResultMessage(element.id, DateTime.Now, element.version));
    }
    catch {
        return Results.BadRequest();
    }
})
.WithName("Publish")
.WithOpenApi();;

// GET: subscribe endpoint ////////////////////////////////////////////////////
app.MapGet("/subscribe", (string channel, DateTime from) => {
    try {

        if (!channel_storage.ContainsKey(channel)) return Results.NotFound();

        var element_list = channel_storage[channel];

        return Results.Ok(element_list.ToArray().Where<MessageStorage>(el => el.timestamp >= from) );
    }
    catch {
        return Results.BadRequest();
    }
})
.WithName("Subscribe")
.WithOpenApi();

app.Run();

// Data structure used in messagebus //////////////////////////////////////////
record Message(string channel, string version, string data);
record MessageStorage(Guid id, DateTime timestamp, string version, string data);
record ResultMessage(Guid id, DateTime timestamp, string version);