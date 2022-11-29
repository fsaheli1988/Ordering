using Microsoft.EntityFrameworkCore;
using Ordering.Infrastructure;
using System.Reflection;
using FluentValidation.AspNetCore;
using MediatR;
using Ordering.API.Application.Behaviors;
using Ordering.API.Application.Models;
using Ordering.API.Infrastructure;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

builder.Services.AddDbContext<OrderingContext>(options =>
{

    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString"));
});


builder.Services.AddScoped<DbContext, OrderingContext>();


builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IBuyerRepository, BuyerRepository>();
builder.Services.AddTransient<OrderingContextSeed>();
//var connectionString = new ConnectionString(builder.Configuration["ConnectionString"]);
var connectionString = new ConnectionString(builder.Configuration.GetConnectionString("ConnectionString"));
builder.Services.AddSingleton(connectionString);
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
//builder.Services.AddControllers().AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssemblyContaining<Startup>());
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddControllersWithViews();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

using (var scope = app.Services.CreateScope()) { scope.ServiceProvider.GetService<OrderingContextSeed>().SeedAsync().Wait(); }
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
