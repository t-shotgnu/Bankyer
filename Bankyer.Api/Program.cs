using Bankyer.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Bankyer.Application.Commands.CreateAccount;
using Bankyer.Application.Commands.DeleteAccount;
using Bankyer.Application.Commands.Deposit;
using Bankyer.Application.Commands.Withdraw;
using Bankyer.Application.Handlers;
using Bankyer.Application.Queries;
using Bankyer.Application.Services;
using Bankyer.Domain;
using Bankyer.Infrastructure;
using Bankyer.Shared;
using Bankyer.Shared.Events;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
    options.AddPolicy("Frontend", policy => policy
        .WithOrigins("http://localhost:5173", "https://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod()));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=bankyer.db"));

builder.Services.AddSerilog(new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger());
builder.Logging.AddSerilog();

builder.Services
    .AddSingleton<IEventBus, InMemoryEventBus>()
    .AddScoped<IEventStore, DatabaseEventStore>()
    .AddSingleton<IEventTypeResolver, EventTypeResolver>()
    .AddTransient<IEventHandler<AccountOpenedEvent>, AccountOpenedEventHandler>()
    .AddTransient<IEventHandler<MoneyDepositedEvent>, MoneyDepositedEventHandler>()
    .AddTransient<IEventHandler<MoneyWithdrawnEvent>, MoneyWithdrawnEventHandler>();

// Register Command Handlers and Query Handlers
builder.Services.AddTransient<CreateAccountCommandHandler>()
    .AddTransient<DepositCommandHandler>()
    .AddTransient<WithdrawCommandHandler>()
    .AddTransient<DeleteAccountCommandHandler>()
    .AddTransient<GetAccountQueryHandler>()
    .AddTransient<GetAllAccountsQueryHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseCors("Frontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
