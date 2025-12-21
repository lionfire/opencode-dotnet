namespace AgUi.Chat.BlazorServer.Services;

/// <summary>
/// Available chat backend modes.
/// </summary>
public enum ChatMode
{
    /// <summary>
    /// Mode not yet selected - show the selector dialog.
    /// </summary>
    NotSelected,

    /// <summary>
    /// Use mock backend with simulated responses.
    /// </summary>
    Mock,

    /// <summary>
    /// Use real OpenCode serve backend.
    /// </summary>
    OpenCode
}

/// <summary>
/// Manages the current chat mode state.
/// </summary>
public class ChatModeState
{
    private ChatMode _mode = ChatMode.NotSelected;

    /// <summary>
    /// Event raised when the mode changes.
    /// </summary>
    public event Action? OnModeChanged;

    /// <summary>
    /// Gets or sets the current chat mode.
    /// </summary>
    public ChatMode Mode
    {
        get => _mode;
        set
        {
            if (_mode != value)
            {
                _mode = value;
                OnModeChanged?.Invoke();
            }
        }
    }

    /// <summary>
    /// Returns true if a mode has been selected.
    /// </summary>
    public bool IsModeSelected => _mode != ChatMode.NotSelected;
}
