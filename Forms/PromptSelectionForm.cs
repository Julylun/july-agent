using System.Drawing;
using System.Windows.Forms;
using JulyAgent.Constants;
using JulyAgent.Utils;

namespace JulyAgent.Forms
{
    public partial class PromptSelectionForm : Form
    {
        private ListBox _promptListBox;
        private Button _selectButton;
        private Button _cancelButton;
        private TableLayoutPanel _mainLayout;
        private FlowLayoutPanel _buttonPanel;

        public string SelectedPrompt { get; private set; } = string.Empty;

        public PromptSelectionForm()
        {
            InitializeComponent();
            ApplyTheme();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = "Select Sample Prompt";
            this.Size = new Size(700, 500);
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
                ColumnCount = 1,
                RowCount = 3,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            // Configure rows
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Title row
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // ListBox row (fill remaining space)
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); // Button row

            // Title Label
            var titleLabel = new Label
            {
                Text = "Choose a sample prompt to use as your system prompt:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                AutoSize = false
            };

            // Prompt ListBox
            _promptListBox = new ListBox
            {
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill,
                SelectionMode = SelectionMode.One
            };

            // Add sample prompts to ListBox
            foreach (var prompt in AppConstants.SamplePrompts)
            {
                _promptListBox.Items.Add(prompt);
            }

            // Select first item by default
            if (_promptListBox.Items.Count > 0)
            {
                _promptListBox.SelectedIndex = 0;
            }

            // Button panel
            _buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            // Select Button
            _selectButton = new Button
            {
                Text = "Select",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(80, 35),
                Cursor = Cursors.Hand
            };
            _selectButton.Click += SelectButton_Click;

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
            _buttonPanel.Controls.Add(_selectButton);

            // Add controls to main layout
            _mainLayout.Controls.Add(titleLabel, 0, 0);
            _mainLayout.Controls.Add(_promptListBox, 0, 1);
            _mainLayout.Controls.Add(_buttonPanel, 0, 2);

            // Add main layout to form
            this.Controls.Add(_mainLayout);

            // Set default button
            this.AcceptButton = _selectButton;
            this.CancelButton = _cancelButton;

            this.ResumeLayout(false);
        }

        private void ApplyTheme()
        {
            ThemeManager.ApplyTheme(this, "dark");
        }

        private void SelectButton_Click(object? sender, EventArgs e)
        {
            if (_promptListBox.SelectedItem != null)
            {
                SelectedPrompt = _promptListBox.SelectedItem.ToString() ?? string.Empty;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select a prompt first!", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
