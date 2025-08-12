using System.Drawing;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using JulyAgent.Constants;
using JulyAgent.Interfaces;
using JulyAgent.Models;
using JulyAgent.Utils;

namespace JulyAgent.Forms
{
    public partial class SettingsForm : Form
    {
        private readonly ISettingsService _settingsService;
        private readonly ILogger<SettingsForm> _logger;

        private TextBox _apiKeyTextBox;
        private ComboBox _modelComboBox;
        private Button _saveButton;
        private Button _cancelButton;
        private Label _apiKeyLabel;
        private Label _modelLabel;

        public string ApiKey { get; private set; } = string.Empty;
        public string SelectedModel { get; private set; } = AppConstants.DefaultModel;

        public SettingsForm(ISettingsService settingsService, ILogger<SettingsForm> logger)
        {
            _settingsService = settingsService;
            _logger = logger;

            InitializeComponent();
            LoadSettingsAsync();
            ApplyTheme();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = $"Settings - {AppConstants.AppName}";
            this.Size = new Size(450, 250);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;

            // API Key Label
            _apiKeyLabel = new Label
            {
                Text = "Google Gemini API Key:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(200, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // API Key TextBox
            _apiKeyTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 50),
                Size = new Size(400, 30),
                UseSystemPasswordChar = true
            };

            // Model Label
            _modelLabel = new Label
            {
                Text = "Gemini Model:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 90),
                Size = new Size(200, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Model ComboBox
            _modelComboBox = new ComboBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 120),
                Size = new Size(400, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _modelComboBox.Items.AddRange(AppConstants.AvailableModels);

            // Save Button
            _saveButton = new Button
            {
                Text = "Save",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(250, 170),
                Size = new Size(80, 35),
                Cursor = Cursors.Hand
            };
            _saveButton.Click += SaveButton_Click;

            // Cancel Button
            _cancelButton = new Button
            {
                Text = "Cancel",
                Font = new Font("Segoe UI", 10),
                Location = new Point(340, 170),
                Size = new Size(80, 35),
                Cursor = Cursors.Hand
            };
            _cancelButton.Click += CancelButton_Click;

            // Add controls to form
            this.Controls.AddRange(new Control[]
            {
                _apiKeyLabel,
                _apiKeyTextBox,
                _modelLabel,
                _modelComboBox,
                _saveButton,
                _cancelButton
            });

            // Set default button
            this.AcceptButton = _saveButton;
            this.CancelButton = _cancelButton;

            this.ResumeLayout(false);
        }

        private void ApplyTheme()
        {
            ThemeManager.ApplyTheme(this, "dark");
        }

        private async void LoadSettingsAsync()
        {
            try
            {
                var settings = await _settingsService.LoadSettingsAsync();
                _apiKeyTextBox.Text = settings.ApiKey ?? "";
                _modelComboBox.SelectedItem = settings.Model ?? AppConstants.DefaultModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading settings");
                _modelComboBox.SelectedIndex = 0; // Default to first item
            }
        }

        private async void SaveButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_apiKeyTextBox.Text))
            {
                MessageBox.Show("Please enter your Gemini API Key!", "API Key Required", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ApiKey = _apiKeyTextBox.Text.Trim();
                SelectedModel = _modelComboBox.SelectedItem?.ToString() ?? AppConstants.DefaultModel;

                // Save settings
                var settings = new AppSettings
                {
                    ApiKey = ApiKey,
                    Model = SelectedModel
                };

                await _settingsService.SaveSettingsAsync(settings);

                MessageBox.Show("Settings saved successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving settings");
                MessageBox.Show($"Error saving settings: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
