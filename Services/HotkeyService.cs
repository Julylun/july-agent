using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using JulyAgent.Interfaces;

namespace JulyAgent.Services
{
    public class HotkeyService : IHotkeyService
    {
        private const int WM_HOTKEY = 0x0312;
        private const int HOTKEY_ID = 1;
        
        private readonly ILogger<HotkeyService> _logger;
        private IntPtr _windowHandle;
        private bool _isRegistered;
        private string _currentHotkey = string.Empty;

        public bool IsHotkeyRegistered => _isRegistered;
        public string CurrentHotkey => _currentHotkey;

        public event EventHandler? HotkeyPressed;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public HotkeyService(ILogger<HotkeyService> logger)
        {
            _logger = logger;
        }

        public bool RegisterHotkey(string hotkey)
        {
            try
            {
                if (_isRegistered)
                {
                    UnregisterHotkey();
                }

                var (modifiers, key) = ParseHotkey(hotkey);
                if (key == 0)
                {
                    _logger.LogError("Invalid hotkey format: {Hotkey}", hotkey);
                    return false;
                }

                if (!RegisterHotKey(_windowHandle, HOTKEY_ID, modifiers, key))
                {
                    _logger.LogError("Failed to register hotkey: {Hotkey}", hotkey);
                    return false;
                }

                _isRegistered = true;
                _currentHotkey = hotkey;
                _logger.LogInformation("Hotkey registered successfully: {Hotkey}", hotkey);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering hotkey: {Hotkey}", hotkey);
                return false;
            }
        }

        public bool UnregisterHotkey()
        {
            try
            {
                if (!_isRegistered)
                {
                    return true;
                }

                if (UnregisterHotKey(_windowHandle, HOTKEY_ID))
                {
                    _isRegistered = false;
                    _currentHotkey = string.Empty;
                    _logger.LogInformation("Hotkey unregistered successfully");
                    return true;
                }

                _logger.LogWarning("Failed to unregister hotkey");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unregistering hotkey");
                return false;
            }
        }

        public void SetWindowHandle(IntPtr handle)
        {
            _windowHandle = handle;
        }

        public bool ProcessMessage(int msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == WM_HOTKEY && wParam.ToInt32() == HOTKEY_ID)
            {
                _logger.LogDebug("Hotkey pressed: {Hotkey}", _currentHotkey);
                HotkeyPressed?.Invoke(this, EventArgs.Empty);
                return true;
            }
            return false;
        }

        private (uint modifiers, uint key) ParseHotkey(string hotkey)
        {
            var parts = hotkey.ToUpper().Split('+');
            uint modifiers = 0;
            uint key = 0;

            foreach (var part in parts)
            {
                var trimmed = part.Trim();
                switch (trimmed)
                {
                    case "CTRL":
                    case "CONTROL":
                        modifiers |= 0x0002; // MOD_CONTROL
                        break;
                    case "ALT":
                        modifiers |= 0x0001; // MOD_ALT
                        break;
                    case "SHIFT":
                        modifiers |= 0x0004; // MOD_SHIFT
                        break;
                    case "WIN":
                    case "WINDOWS":
                        modifiers |= 0x0008; // MOD_WIN
                        break;
                    default:
                        if (trimmed.Length == 1)
                        {
                            key = (uint)trimmed[0];
                        }
                        break;
                }
            }

            return (modifiers, key);
        }
    }
}
