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
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(500, 400);

            // Title Label
            _titleLabel = new Label
            {
                Text = "Response from Gemini:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(300, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Result RichTextBox
            _resultTextBox = new RichTextBox
            {
                Font = new Font("Consolas", 10),
                Location = new Point(20, 55),
                Size = new Size(540, 350),
                BorderStyle = BorderStyle.FixedSingle,
                ReadOnly = true,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                WordWrap = true
            };

            // Copy Button
            _copyButton = new Button
            {
                Text = "Copy to Clipboard",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(350, 420),
                Size = new Size(150, 35),
                Cursor = Cursors.Hand
            };
            _copyButton.Click += CopyButton_Click;

            // Close Button
            _closeButton = new Button
            {
                Text = "Close",
                Font = new Font("Segoe UI", 10),
                Location = new Point(510, 420),
                Size = new Size(80, 35),
                Cursor = Cursors.Hand
            };
            _closeButton.Click += CloseButton_Click;

            // Add controls to form
            this.Controls.AddRange(new Control[]
            {
                _titleLabel,
                _resultTextBox,
                _copyButton,
                _closeButton
            });

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
