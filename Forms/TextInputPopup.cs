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
        private TableLayoutPanel _mainLayout;
        private FlowLayoutPanel _buttonPanel;

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
            this.Size = new Size(500, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.TopMost = true;
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
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // Title row
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20)); // Spacing
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); // TextBox row
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20)); // Spacing
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); // Button row

            // Title Label
            _titleLabel = new Label
            {
                Text = "Enter your text:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                AutoSize = false
            };

            // TextBox
            _textBox = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };
            _textBox.KeyDown += TextBox_KeyDown;

            // Button panel
            _buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            // OK Button
            _okButton = new Button
            {
                Text = "OK",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(80, 35),
                Cursor = Cursors.Hand
            };
            _okButton.Click += OkButton_Click;

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
            _buttonPanel.Controls.Add(_okButton);

            // Add controls to main layout
            _mainLayout.Controls.Add(_titleLabel, 0, 0);
            _mainLayout.Controls.Add(new Label { Text = "" }, 0, 1); // Spacing
            _mainLayout.Controls.Add(_textBox, 0, 2);
            _mainLayout.Controls.Add(new Label { Text = "" }, 0, 3); // Spacing
            _mainLayout.Controls.Add(_buttonPanel, 0, 4);

            // Add main layout to form
            this.Controls.Add(_mainLayout);

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
