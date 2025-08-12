namespace JulyAgent.Interfaces
{
    public interface INotifyIconService : IDisposable
    {
        void Initialize();
        void Show();
        void Hide();
        bool IsVisible { get; }
        event EventHandler? DoubleClick;
        event EventHandler? SettingsClicked;
        event EventHandler? ShowClicked;
        event EventHandler? ExitClicked;
    }
}
