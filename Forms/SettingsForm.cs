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
        private Label _promptLabel;
        private TextBox _promptTextBox;
        private Button _samplePromptButton;
        private TableLayoutPanel _mainLayout;
        private FlowLayoutPanel _buttonPanel;

        public string ApiKey { get; private set; } = string.Empty;
        public string SelectedModel { get; private set; } = AppConstants.DefaultModel;
        public string Prompt { get; private set; } = string.Empty;

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
            this.Size = new Size(600, 450);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.Padding = new Padding(20);

            // Main layout panel
            _mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 8,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            // Configure columns
            _mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150)); // Label column
            _mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); // Control column

            // Configure rows
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // API Key Label
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35)); // API Key TextBox
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20)); // Spacing
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Model Label
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35)); // Model ComboBox
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20)); // Spacing
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Prompt Label
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // Prompt TextBox
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20)); // Spacing
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); // Button row

            // API Key Label
            _apiKeyLabel = new Label
            {
                Text = "Google Gemini API Key:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                AutoSize = false
            };

            // API Key TextBox
            _apiKeyTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                UseSystemPasswordChar = true,
                Dock = DockStyle.Fill
            };

            // Model Label
            _modelLabel = new Label
            {
                Text = "Gemini Model:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                AutoSize = false
            };

            // Model ComboBox
            _modelComboBox = new ComboBox
            {
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Dock = DockStyle.Fill
            };
            _modelComboBox.Items.AddRange(AppConstants.AvailableModels);

            // Prompt Label
            _promptLabel = new Label
            {
                Text = "System Prompt:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                AutoSize = false
            };

            // Prompt TextBox
            _promptTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Dock = DockStyle.Fill
            };

            // Sample Prompt Button
            _samplePromptButton = new Button
            {
                Text = "Sample Prompts",
                Font = new Font("Segoe UI", 9),
                Size = new Size(100, 25),
                Cursor = Cursors.Hand
            };
            _samplePromptButton.Click += SamplePromptButton_Click;

            // Button panel
            _buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            // Save Button
            _saveButton = new Button
            {
                Text = "Save",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(80, 35),
                Cursor = Cursors.Hand
            };
            _saveButton.Click += SaveButton_Click;

            // Cancel Button
            _cancelButton = new Button
            {
                Text = "Cancel",
                Font = new Font("Segoe UI", 10),
                Size = new Size(80, 35),
                Cursor = Cursors.Hand
            };
            _cancelButton.Click += CancelButton_Click;

            // Add buttons to button panel
            _buttonPanel.Controls.Add(_cancelButton);
            _buttonPanel.Controls.Add(_saveButton);

            // Add controls to main layout
            _mainLayout.Controls.Add(_apiKeyLabel, 0, 0);
            _mainLayout.Controls.Add(_apiKeyTextBox, 1, 0);
            _mainLayout.Controls.Add(new Label { Text = "" }, 0, 1); // Spacing
            _mainLayout.Controls.Add(new Label { Text = "" }, 1, 1); // Spacing
            _mainLayout.Controls.Add(_modelLabel, 0, 2);
            _mainLayout.Controls.Add(_modelComboBox, 1, 2);
            _mainLayout.Controls.Add(new Label { Text = "" }, 0, 3); // Spacing
            _mainLayout.Controls.Add(new Label { Text = "" }, 1, 3); // Spacing
            _mainLayout.Controls.Add(_promptLabel, 0, 4);
            _mainLayout.Controls.Add(_promptTextBox, 1, 4);
            _mainLayout.Controls.Add(new Label { Text = "" }, 0, 5); // Spacing for button
            _mainLayout.Controls.Add(_samplePromptButton, 1, 5);
            _mainLayout.Controls.Add(new Label { Text = "" }, 0, 6); // Spacing
            _mainLayout.Controls.Add(new Label { Text = "" }, 1, 6); // Spacing
            _mainLayout.Controls.Add(new Label { Text = "" }, 0, 7); // Spacing
            _mainLayout.Controls.Add(_buttonPanel, 1, 7);

            // Add main layout to form
            this.Controls.Add(_mainLayout);

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
                _promptTextBox.Text = settings.Prompt ?? "";
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
                Prompt = _promptTextBox.Text.Trim();

                // Save settings
                var settings = new AppSettings
                {
                    ApiKey = ApiKey,
                    Model = SelectedModel,
                    Prompt = _promptTextBox.Text.Trim()
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

        private void SamplePromptButton_Click(object? sender, EventArgs e)
        {
            try
            {
                using var promptForm = new PromptSelectionForm();
                if (promptForm.ShowDialog() == DialogResult.OK)
                {
                    _promptTextBox.Text = promptForm.SelectedPrompt;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error showing prompt selection form");
                MessageBox.Show($"Error selecting prompt: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
