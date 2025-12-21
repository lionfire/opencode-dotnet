using AgUi.Chat.BlazorServer.Components;
using AgUi.Chat.BlazorServer.Hubs;
using AgUi.Chat.BlazorServer.Services;
using LionFire.OpenCode.Blazor;
using LionFire.OpenCode.Serve;
using LionFire.OpenCode.Serve.Extensions;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Load demo settings from configuration
var demoSettings = builder.Configuration.GetSection("Demo").Get<DemoSettings>() ?? new DemoSettings();

// Parse command-line arguments for mode selection
var modeState = new ChatModeState();

if (args.Contains("--mock"))
{
    if (demoSettings.MockBackendEnabled)
    {
        modeState.Mode = ChatMode.Mock;
        Console.WriteLine("Starting in Mock mode (simulated responses)");
    }
    else
    {
        Console.WriteLine("Warning: --mock specified but Demo:MockBackendEnabled is false. Ignoring.");
        Console.WriteLine("Will detect OpenCode automatically.");
    }
}
else if (args.Contains("--opencode"))
{
    modeState.Mode = ChatMode.OpenCode;
    Console.WriteLine("Starting in OpenCode mode (real backend)");
}
else
{
    Console.WriteLine("Will detect OpenCode at startup.");
    if (demoSettings.MockBackendEnabled)
    {
        Console.WriteLine("  Use --mock for simulated responses (demo mode enabled)");
    }
    Console.WriteLine("  Use --opencode to skip detection");
}

// Add services to the container
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add MudBlazor services
builder.Services.AddMudServices();

// Add OpenCode Blazor services
builder.Services.AddOpenCodeBlazor();

// Add SignalR
builder.Services.AddSignalR();

// Register configuration
builder.Services.Configure<DemoSettings>(builder.Configuration.GetSection("Demo"));

// Register chat mode state as singleton
builder.Services.AddSingleton(modeState);

// Register OpenCode launcher
builder.Services.AddSingleton<OpenCodeLauncher>();

// Register chat services
if (demoSettings.MockBackendEnabled)
{
    builder.Services.AddScoped<MockChatService>();
}
builder.Services.AddScoped<OpenCodeChatService>();

// Register OpenCode client for OpenCode mode
builder.Services.AddOpenCodeClient(options =>
{
    options.BaseUrl = builder.Configuration["OpenCode:BaseUrl"] ?? "http://localhost:9876";
});

// Register IChatService factory that returns the appropriate service based on mode
builder.Services.AddScoped<IChatService>(sp =>
{
    var state = sp.GetRequiredService<ChatModeState>();
    var settings = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<DemoSettings>>().Value;

    return state.Mode switch
    {
        ChatMode.Mock when settings.MockBackendEnabled => sp.GetRequiredService<MockChatService>(),
        ChatMode.OpenCode => sp.GetRequiredService<OpenCodeChatService>(),
        _ => sp.GetRequiredService<OpenCodeChatService>() // Default to OpenCode
    };
});

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

// Map SignalR hub
app.MapHub<ChatHub>("/chathub");

app.Run();
