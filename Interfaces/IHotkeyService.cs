namespace JulyAgent.Interfaces
{
    public interface IHotkeyService
    {
        bool RegisterHotkey(string hotkey);
        bool UnregisterHotkey();
        bool IsHotkeyRegistered { get; }
        string CurrentHotkey { get; }
        void SetWindowHandle(IntPtr handle);
        bool ProcessMessage(int msg, IntPtr wParam, IntPtr lParam);
        event EventHandler? HotkeyPressed;
    }
}
