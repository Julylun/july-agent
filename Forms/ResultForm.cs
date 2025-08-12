using System.Drawing;
using System.Windows.Forms;
using JulyAgent.Constants;
using JulyAgent.Utils;

namespace JulyAgent.Forms
{
    public partial class ResultForm : Form
    {
        private RichTextBox _resultTextBox;
        private Button _copyButton;
        private Button _closeButton;
        private Label _titleLabel;
        private TableLayoutPanel _mainLayout;
        private FlowLayoutPanel _buttonPanel;

        public ResultForm(string result)
        {
            InitializeComponent();
            _resultTextBox.Text = result;
            ApplyTheme();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = "Gemini Response";
            this.Size = new Size(700, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(600, 500);
            this.Padding = new Padding(20);

            // Main layout panel
            _mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            // Configure rows
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Title row
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20)); // Spacing
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Content row (fill remaining space)
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); // Button row

            // Title Label
            _titleLabel = new Label
            {
                Text = "Response from Gemini:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                AutoSize = false
            };

            // Result RichTextBox
            _resultTextBox = new RichTextBox
            {
                Font = new Font("Consolas", 10),
                BorderStyle = BorderStyle.FixedSingle,
                ReadOnly = true,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                WordWrap = true,
                Dock = DockStyle.Fill
            };

            // Button panel
            _buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            // Copy Button
            _copyButton = new Button
            {
                Text = "Copy to Clipboard",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(150, 35),
                Cursor = Cursors.Hand
            };
            _copyButton.Click += CopyButton_Click;

            // Close Button
            _closeButton = new Button
            {
                Text = "Close",
                Font = new Font("Segoe UI", 10),
                Size = new Size(80, 35),
                Cursor = Cursors.Hand
            };
            _closeButton.Click += CloseButton_Click;

            // Add buttons to button panel
            _buttonPanel.Controls.Add(_closeButton);
            _buttonPanel.Controls.Add(_copyButton);

            // Add controls to main layout
            _mainLayout.Controls.Add(_titleLabel, 0, 0);
            _mainLayout.Controls.Add(new Label { Text = "" }, 0, 1); // Spacing
            _mainLayout.Controls.Add(_resultTextBox, 0, 2);
            _mainLayout.Controls.Add(_buttonPanel, 0, 3);

            // Add main layout to form
            this.Controls.Add(_mainLayout);

            // Set default button
            this.AcceptButton = _closeButton;

            this.ResumeLayout(false);
        }

        private void ApplyTheme()
        {
            ThemeManager.ApplyTheme(this, "dark");
        }

        private void CopyButton_Click(object? sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(_resultTextBox.Text);
                _copyButton.Text = "Copied!";
                _copyButton.BackColor = Color.FromArgb(AppConstants.Colors.SuccessGreen);
                
                // Reset button after 2 seconds
                var timer = new System.Windows.Forms.Timer
                {
                    Interval = 2000
                };
                timer.Tick += (s, args) =>
                {
                    _copyButton.Text = "Copy to Clipboard";
                    ThemeManager.ApplyThemeToControl(_copyButton, ThemeManager.GetThemeColors("dark"));
                    timer.Stop();
                    timer.Dispose();
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error copying to clipboard: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CloseButton_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _resultTextBox.Focus();
        }
    }
}
