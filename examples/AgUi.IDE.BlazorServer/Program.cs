using AgUi.IDE.BlazorServer.Components;
using AgUi.IDE.BlazorServer.Services;
using LionFire.OpenCode.Blazor;
using LionFire.OpenCode.Serve;
using LionFire.OpenCode.Serve.Extensions;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Load settings from configuration
var ideSettings = builder.Configuration.GetSection("IDE").Get<IdeSettings>() ?? new IdeSettings();

// Add services to the container
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add MudBlazor services
builder.Services.AddMudServices();

// Add OpenCode Blazor services (theme, utilities)
builder.Services.AddOpenCodeBlazor();

// Add SignalR for real-time updates
builder.Services.AddSignalR();

// Register configuration
builder.Services.Configure<IdeSettings>(builder.Configuration.GetSection("IDE"));

// Register OpenCode client
builder.Services.AddOpenCodeClient(options =>
{
    options.BaseUrl = builder.Configuration["OpenCode:BaseUrl"] ?? "http://localhost:9123";
});

// Register IDE services
builder.Services.AddScoped<IdeStateService>();
builder.Services.AddScoped<FileTreeService>();
builder.Services.AddScoped<OpenCodeLauncher>();
builder.Services.AddScoped<PermissionService>(sp =>
{
    var client = sp.GetService<IOpenCodeClient>();
    var logger = sp.GetService<ILogger<PermissionService>>();
    return new PermissionService(client, logger);
});
builder.Services.AddScoped<ChatService>(sp =>
{
    var client = sp.GetService<IOpenCodeClient>();
    var permissionService = sp.GetService<PermissionService>();
    var ideState = sp.GetService<IdeStateService>();
    var logger = sp.GetService<ILogger<ChatService>>();
    return new ChatService(client, permissionService, ideState, logger);
});
builder.Services.AddScoped<DiffService>();
builder.Services.AddScoped<TerminalService>();
builder.Services.AddScoped<OpenCodeIdeService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
