using System.Drawing;
using System.Windows.Forms;
using JulyAgent.Constants;
using JulyAgent.Utils;

namespace JulyAgent.Forms
{
    public partial class TextInputPopup : Form
    {
        private TextBox _textBox;
        private Button _okButton;
        private Button _cancelButton;
        private Label _titleLabel;

        public string EnteredText { get; private set; } = string.Empty;

        public TextInputPopup()
        {
            InitializeComponent();
            ApplyTheme();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = "Text Input";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.TopMost = true;

            // Title Label
            _titleLabel = new Label
            {
                Text = "Enter your text:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(350, 100),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // TextBox
            _textBox = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(20, 120),
                Size = new Size(350, 50),
                BorderStyle = BorderStyle.FixedSingle
            };
            _textBox.KeyDown += TextBox_KeyDown;

            // OK Button
            _okButton = new Button
            {
                Text = "OK",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(200, 200),
                Size = new Size(80, 50),
                Cursor = Cursors.Hand
            };
            _okButton.Click += OkButton_Click;

            // Cancel Button
            _cancelButton = new Button
            {
                Text = "Cancel",
                Font = new Font("Segoe UI", 10),
                Location = new Point(290, 200),
                Size = new Size(150, 50),
                Cursor = Cursors.Hand
            };
            _cancelButton.Click += CancelButton_Click;

            // Add controls to form
            this.Controls.AddRange(new Control[]
            {
                _titleLabel,
                _textBox,
                _okButton,
                _cancelButton
            });

            // Set default button
            this.AcceptButton = _okButton;
            this.CancelButton = _cancelButton;

            this.ResumeLayout(false);

            // Focus on textbox
            this.Load += (s, e) => _textBox.Focus();
        }

        private void ApplyTheme()
        {
            ThemeManager.ApplyTheme(this, "dark");
        }

        private void TextBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OkButton_Click(sender, e);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                CancelButton_Click(sender, e);
            }
        }

        private void OkButton_Click(object? sender, EventArgs e)
        {
            EnteredText = _textBox.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _textBox.Focus();
            _textBox.SelectAll();
        }
    }
}
