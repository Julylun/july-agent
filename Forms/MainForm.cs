using System.Drawing;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using JulyAgent.Constants;
using JulyAgent.Interfaces;
using JulyAgent.Services;
using JulyAgent.Utils;

namespace JulyAgent.Forms
{
    public partial class MainForm : Form
    {
        private readonly INotifyIconService _notifyIconService;
        private readonly IHotkeyService _hotkeyService;
        private readonly ISettingsService _settingsService;
        private readonly IGeminiService _geminiService;
        private readonly ILogger<MainForm> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IScreenshotService _screenshotService;

        private TableLayoutPanel _mainLayout;
        private Label _welcomeLabel;
        private Label _statusLabel;

        public MainForm(
            INotifyIconService notifyIconService,
            IHotkeyService hotkeyService,
            ISettingsService settingsService,
            IGeminiService geminiService,
            ILogger<MainForm> logger,
            ILoggerFactory loggerFactory,
            IScreenshotService screenshotService)
        {
            _notifyIconService = notifyIconService;
            _hotkeyService = hotkeyService;
            _settingsService = settingsService;
            _geminiService = geminiService;
            _logger = logger;
            _loggerFactory = loggerFactory;
            _screenshotService = screenshotService;

            InitializeComponent();
            InitializeForm();
            SetupEventHandlers();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // Form properties
            this.Text = AppConstants.AppName;
            this.Size = new Size(AppConstants.DefaultFormWidth, AppConstants.DefaultFormHeight);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
            this.Padding = new Padding(20);

            // Main layout panel
            _mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            // Configure rows
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80)); // Welcome row
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20)); // Spacing
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Status row (fill remaining space)

            // Welcome Label
            _welcomeLabel = new Label
            {
                Text = $"Welcome to {AppConstants.AppName}!\n\nPress {AppConstants.DefaultHotkey} to start using Gemini AI",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                AutoSize = false
            };

            // Status Label
            _statusLabel = new Label
            {
                Text = "Application is running in the background.\nCheck system tray for more options.",
                Font = new Font("Segoe UI", 11),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                AutoSize = false
            };

            // Add controls to main layout
            _mainLayout.Controls.Add(_welcomeLabel, 0, 0);
            _mainLayout.Controls.Add(new Label { Text = "" }, 0, 1); // Spacing
            _mainLayout.Controls.Add(_statusLabel, 0, 2);

            // Add main layout to form
            this.Controls.Add(_mainLayout);
            
            this.ResumeLayout(false);
        }

        private async void InitializeForm()
        {
            try
            {
                // Load settings and apply theme
                var settings = await _settingsService.LoadSettingsAsync();
                ThemeManager.ApplyTheme(this, settings.Theme);

                // Initialize services
                _notifyIconService.Initialize();
                _hotkeyService.SetWindowHandle(this.Handle);
                
                if (!_hotkeyService.RegisterHotkey(settings.Hotkey))
                {
                    _logger.LogWarning("Failed to register hotkey: {Hotkey}", settings.Hotkey);
                }

                _logger.LogInformation("MainForm initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing MainForm");
                MessageBox.Show($"Error initializing application: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupEventHandlers()
        {
            _hotkeyService.HotkeyPressed += OnHotkeyPressed;
            _notifyIconService.SettingsClicked += OnSettingsClicked;
            _notifyIconService.ShowClicked += OnShowClicked;
            _notifyIconService.ExitClicked += OnExitClicked;
            _notifyIconService.DoubleClick += OnShowClicked;
            _notifyIconService.ScreenshotGridClicked += OnScreenshotGridClicked;
            this.Load += MainForm_Load;
            this.FormClosing += MainForm_FormClosing;
        }

        private void MainForm_Load(object? sender, EventArgs e)
        {
            this.Hide();
        }

        private async void OnHotkeyPressed(object? sender, EventArgs e)
        {
            try
            {
                await ShowTextInputPopupAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling hotkey press");
            }
        }

        private async Task ShowTextInputPopupAsync()
        {
            using var popup = new TextInputPopup();
            if (popup.ShowDialog() == DialogResult.OK)
            {
                await ProcessTextWithGeminiAsync(popup.EnteredText);
            }
        }

        private async Task ProcessTextWithGeminiAsync(string text)
        {
            try
            {
                var model = await _settingsService.GetModelAsync();

                // Show processing indicator
                using var processingForm = new ProcessingForm();
                processingForm.Show();
                Application.DoEvents();

                // Call Gemini API
                var result = await _geminiService.GenerateContentAsync(text, model);
                processingForm.Close();

                // Show result
                using var resultForm = new ResultForm(result);
                resultForm.ShowDialog();
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("API key"))
            {
                MessageBox.Show(ApiConstants.ApiKeyRequiredMessage, "API Key Required", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing text with Gemini");
                MessageBox.Show($"Error processing with Gemini: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void OnSettingsClicked(object? sender, EventArgs e)
        {
            try
            {
                using var settingsForm = new SettingsForm(_settingsService, _loggerFactory.CreateLogger<SettingsForm>());
                if (settingsForm.ShowDialog() == DialogResult.OK)
                {
                    // Reload settings and reapply theme
                    var settings = await _settingsService.LoadSettingsAsync();
                    ThemeManager.ApplyTheme(this, settings.Theme);
                    
                    // Re-register hotkey if changed
                    _hotkeyService.UnregisterHotkey();
                    if (!_hotkeyService.RegisterHotkey(settings.Hotkey))
                    {
                        _logger.LogWarning("Failed to register hotkey: {Hotkey}", settings.Hotkey);
                    }

                    _logger.LogInformation("Settings updated - API Key: {HasApiKey}, Model: {Model}, Prompt: {PromptLength} chars", 
                        !string.IsNullOrEmpty(settings.ApiKey), settings.Model, settings.Prompt?.Length ?? 0);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error showing settings form");
            }
        }

        private void OnShowClicked(object? sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void OnExitClicked(object? sender, EventArgs e)
        {
            Application.Exit();
        }

        private void OnScreenshotGridClicked(object? sender, EventArgs e)
        {
            try
            {
                using var grid = new ScreenshotGridForm(_screenshotService);
                grid.ShowDialog();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error opening Screenshot Grid");
                MessageBox.Show($"Failed to open Screenshot Grid: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
            else
            {
                Cleanup();
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (_hotkeyService.ProcessMessage(m.Msg, m.WParam, m.LParam))
            {
                return;
            }
            base.WndProc(ref m);
        }

        private void Cleanup()
        {
            try
            {
                _hotkeyService.UnregisterHotkey();
                _notifyIconService.Dispose();
                _logger.LogInformation("MainForm cleanup completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during MainForm cleanup");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Cleanup();
            }
            base.Dispose(disposing);
        }
    }
}
