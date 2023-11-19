using Application;
using Domain.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<BalancerSettings>(builder.Configuration.GetSection("BalancerSettings"));
builder.Services.Configure<BackgroundJobsSettings>(builder.Configuration.GetSection("BackgroundJobsSettings"));
builder.Services.AddLoadBalancerApplication(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseLoadBalancingMiddleware();

app.UseAuthorization();

app.MapControllers();

app.Run();