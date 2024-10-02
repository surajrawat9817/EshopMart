using Common.Logging.Correlation;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Kubernetes;

var builder = WebApplication.CreateBuilder(args);
// Environment.SetEnvironmentVariable("HostingEnvironment","Local");
Console.WriteLine("env =="+Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
builder.Configuration.AddJsonFile($"ocelot.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true, true);
builder.Services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        policy => { policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin(); });
});
builder.Services.AddOcelot()
    .AddKubernetes()
    .AddCacheManager(o => o.WithDictionaryHandle());

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
    
// app.AddCorrelationIdMiddleware();
app.UseRouting();
app.UseCors("CorsPolicy");
app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello Ocelot"); });
});
await app.UseOcelot();
app.Run();