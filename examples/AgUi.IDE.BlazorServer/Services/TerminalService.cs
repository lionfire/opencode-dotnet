namespace AgUi.IDE.BlazorServer.Services;

/// <summary>
/// Terminal output line.
/// </summary>
public class TerminalLine
{
    public string Content { get; set; } = "";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public TerminalOutputType Type { get; set; }
}

public enum TerminalOutputType
{
    Stdout,
    Stderr,
    System,
    Prompt
}

/// <summary>
/// Service for managing terminal output.
/// </summary>
public class TerminalService
{
    private readonly List<TerminalLine> _lines = new();
    private const int MaxBufferSize = 10000;

    public event Action? OutputChanged;

    public IReadOnlyList<TerminalLine> Lines => _lines.AsReadOnly();

    public void AppendLine(string content, TerminalOutputType type = TerminalOutputType.Stdout)
    {
        _lines.Add(new TerminalLine
        {
            Content = content,
            Type = type
        });

        // Trim buffer if too large
        if (_lines.Count > MaxBufferSize)
        {
            _lines.RemoveRange(0, _lines.Count - MaxBufferSize);
        }

        OutputChanged?.Invoke();
    }

    public void AppendOutput(string content)
    {
        AppendLine(content, TerminalOutputType.Stdout);
    }

    public void AppendError(string content)
    {
        AppendLine(content, TerminalOutputType.Stderr);
    }

    public void AppendSystem(string content)
    {
        AppendLine(content, TerminalOutputType.System);
    }

    public void AppendPrompt(string content = "$")
    {
        AppendLine(content, TerminalOutputType.Prompt);
    }

    public void Clear()
    {
        _lines.Clear();
        OutputChanged?.Invoke();
    }

    /// <summary>
    /// Initialize with sample output for demo.
    /// </summary>
    public void InitializeWithSampleOutput()
    {
        AppendPrompt("$ opencode serve");
        AppendSystem("Server started on http://localhost:9123");
        AppendOutput("Ready for connections...");
        AppendPrompt("$");
    }
}
