using EtcdExample;
using EtcdExample.DI;
using Example.RTCs;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRTC("http://localhost:2379", "test")
    .RegisterAllSettingsAsServiceFromAssembly(typeof(Program).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Map("/", (IRTCStore store) =>
{
    var rtc = store.Get<SimpleRTC>();
    Console.WriteLine(rtc);
});

app.Map("/rtc", ([FromServices]SimpleRTC rtc) =>
{
    Console.WriteLine(rtc);
});


app.Run();
