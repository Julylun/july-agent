using System.Drawing;
using System.Windows.Forms;
using JulyAgent.Constants;
using JulyAgent.Utils;

namespace JulyAgent.Forms
{
    public partial class ProcessingForm : Form
    {
        private Label _statusLabel;
        private ProgressBar _progressBar;
        private TableLayoutPanel _mainLayout;

        public ProcessingForm()
        {
            InitializeComponent();
            ApplyTheme();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = "Processing";
            this.Size = new Size(450, 180);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.ControlBox = false;
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
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Title row
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20)); // Spacing
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30)); // Progress bar row
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20)); // Bottom spacing

            // Status Label
            _statusLabel = new Label
            {
                Text = "Sending request to Gemini...",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                AutoSize = false
            };

            // Progress Bar
            _progressBar = new ProgressBar
            {
                Dock = DockStyle.Fill,
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 30
            };

            // Add controls to layout
            _mainLayout.Controls.Add(_statusLabel, 0, 0);
            _mainLayout.Controls.Add(new Label { Text = "" }, 0, 1); // Spacing
            _mainLayout.Controls.Add(_progressBar, 0, 2);

            // Add main layout to form
            this.Controls.Add(_mainLayout);

            this.ResumeLayout(false);
        }

        private void ApplyTheme()
        {
            ThemeManager.ApplyTheme(this, "dark");
        }

        public void UpdateStatus(string status)
        {
            if (_statusLabel.InvokeRequired)
            {
                _statusLabel.Invoke(new Action<string>(UpdateStatus), status);
            }
            else
            {
                _statusLabel.Text = status;
            }
        }
    }
}
