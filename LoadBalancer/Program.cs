using Application;
using Domain.Services;
using Domain.Settings;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<BalancerSettings>(builder.Configuration.GetSection("BalancerSettings"));
builder.Services.Configure<BackgroundJobsSettings>(builder.Configuration.GetSection("BackgroundJobsSettings"));
builder.Services.AddLoadBalancerApplication(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/*builder.Services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(builder.Configuration.GetSection("ConnectionStrings:LoadBalancer").Value.ToString(), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));*/

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHangfireDashboard();
//RunBackgroundJobs();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


void RunBackgroundJobs()
{
    var backgroundJobsSettings = new BackgroundJobsSettings();
    builder.Configuration.GetSection("BackgroundJobsSettings").Bind(backgroundJobsSettings);
    var pingServerOptions = backgroundJobsSettings.JobSettings.Where(x => x.Name == "PingServers").First();
    //RecurringJob.AddOrUpdate<IBackgroundJobsService>(pingServerOptions.Name, service => service.ReevaluateServers(), Cron.Minutely(), TimeZoneInfo.Local);
    RecurringJob.AddOrUpdate<IBackgroundJobsService>(service => service.ReevaluateServers(), "*/5 * * * * *");
}