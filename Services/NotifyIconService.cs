using System.Drawing;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using JulyAgent.Constants;
using JulyAgent.Interfaces;

namespace JulyAgent.Services
{
    public class NotifyIconService : INotifyIconService
    {
        private readonly ILogger<NotifyIconService> _logger;
        private NotifyIcon? _notifyIcon;
        private ContextMenuStrip? _contextMenu;
        private bool _disposed;

        public bool IsVisible => _notifyIcon?.Visible ?? false;

        public event EventHandler? DoubleClick;
        public event EventHandler? SettingsClicked;
        public event EventHandler? ShowClicked;
        public event EventHandler? ExitClicked;
        public event EventHandler? ScreenshotGridClicked;

        public NotifyIconService(ILogger<NotifyIconService> logger)
        {
            _logger = logger;
        }

        public void Initialize()
        {
            try
            {
                CreateContextMenu();
                CreateNotifyIcon();
                _logger.LogInformation("NotifyIcon service initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing NotifyIcon service");
                throw;
            }
        }

        private void CreateContextMenu()
        {
            _contextMenu = new ContextMenuStrip();
            
            var screenshotItem = new ToolStripMenuItem("Screenshot Grid", null, OnScreenshotGridClicked);
            var settingsItem = new ToolStripMenuItem("Settings", null, OnSettingsClicked);
            var separator = new ToolStripSeparator();
            var showItem = new ToolStripMenuItem("Show", null, OnShowClicked);
            var exitItem = new ToolStripMenuItem("Exit", null, OnExitClicked);

            _contextMenu.Items.AddRange(new ToolStripItem[]
            {
                screenshotItem,
                settingsItem,
                separator,
                showItem,
                exitItem
            });
        }

        private void CreateNotifyIcon()
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                Text = AppConstants.AppName,
                ContextMenuStrip = _contextMenu,
                Visible = true
            };

            _notifyIcon.DoubleClick += OnDoubleClick;
        }

        public void Show()
        {
            if (_notifyIcon != null && !_disposed)
            {
                _notifyIcon.Visible = true;
            }
        }

        public void Hide()
        {
            if (_notifyIcon != null && !_disposed)
            {
                _notifyIcon.Visible = false;
            }
        }

        private void OnDoubleClick(object? sender, EventArgs e)
        {
            DoubleClick?.Invoke(this, e);
        }

        private void OnScreenshotGridClicked(object? sender, EventArgs e)
        {
            ScreenshotGridClicked?.Invoke(this, e);
        }

        private void OnSettingsClicked(object? sender, EventArgs e)
        {
            SettingsClicked?.Invoke(this, e);
        }

        private void OnShowClicked(object? sender, EventArgs e)
        {
            ShowClicked?.Invoke(this, e);
        }

        private void OnExitClicked(object? sender, EventArgs e)
        {
            ExitClicked?.Invoke(this, e);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _notifyIcon?.Dispose();
                _contextMenu?.Dispose();
                _disposed = true;
                _logger.LogInformation("NotifyIcon service disposed");
            }
        }
    }
}
