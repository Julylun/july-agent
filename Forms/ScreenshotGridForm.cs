using System;
using System.Drawing;
using System.Windows.Forms;
using JulyAgent.Interfaces;
using JulyAgent.Utils;

namespace JulyAgent.Forms
{
    public class ScreenshotGridForm : Form
    {
        private readonly IScreenshotService _screenshotService;

        private PictureBox _pictureBox = null!;
        private NumericUpDown _numCols = null!;
        private NumericUpDown _numRows = null!;
        private Button _saveButton = null!;
        private Button _copyButton = null!;
        private Button _recaptureButton = null!;
        private Button _closeButton = null!;
        private TableLayoutPanel _mainLayout = null!;
        private FlowLayoutPanel _controlsPanel = null!;
        private Bitmap? _originalImage;

        public ScreenshotGridForm(IScreenshotService screenshotService)
        {
            _screenshotService = screenshotService;
            InitializeComponent();
            CaptureAndDisplay();
            ApplyTheme();
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            Text = "Screenshot Grid";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1000, 700);
            Padding = new Padding(12);

            _mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            _controlsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false
            };

            var colsLabel = new Label { Text = "Columns:", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(0, 8, 0, 0) };
            _numCols = new NumericUpDown { Minimum = 1, Maximum = 100, Value = 3, Width = 80 };            
            var rowsLabel = new Label { Text = "Rows:", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(16, 8, 0, 0) };
            _numRows = new NumericUpDown { Minimum = 1, Maximum = 100, Value = 3, Width = 80 };

            _recaptureButton = new Button { Text = "Capture", Width = 100, Height = 32 };
            _saveButton = new Button { Text = "Save Image", Width = 110, Height = 32 };
            _copyButton = new Button { Text = "Copy", Width = 100, Height = 32 };
            _closeButton = new Button { Text = "Close", Width = 100, Height = 32 };

            _controlsPanel.Controls.Add(colsLabel);
            _controlsPanel.Controls.Add(_numCols);
            _controlsPanel.Controls.Add(rowsLabel);
            _controlsPanel.Controls.Add(_numRows);
            _controlsPanel.Controls.Add(_recaptureButton);
            _controlsPanel.Controls.Add(_saveButton);
            _controlsPanel.Controls.Add(_copyButton);
            _controlsPanel.Controls.Add(_closeButton);

            _pictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Black
            };
            _pictureBox.Paint += PictureBox_Paint;

            _mainLayout.Controls.Add(_controlsPanel, 0, 0);
            _mainLayout.Controls.Add(_pictureBox, 0, 1);

            Controls.Add(_mainLayout);

            _numCols.ValueChanged += (_, __) => _pictureBox.Invalidate();
            _numRows.ValueChanged += (_, __) => _pictureBox.Invalidate();
            _recaptureButton.Click += (_, __) => { CaptureAndDisplay(); };
            _saveButton.Click += (_, __) => SaveWithOverlay();
            _copyButton.Click += (_, __) => CopyWithOverlay();
            _closeButton.Click += (_, __) => Close();

            ResumeLayout(false);
        }

        private void ApplyTheme()
        {
            ThemeManager.ApplyTheme(this, "dark");
        }

        private void CaptureAndDisplay()
        {
            _originalImage?.Dispose();
            _originalImage = _screenshotService.CapturePrimaryScreen();
            _pictureBox.Image = _originalImage;
            _pictureBox.Invalidate();
        }

        private void PictureBox_Paint(object? sender, PaintEventArgs e)
        {
            if (_originalImage == null) return;
            if (_numCols.Value < 1 || _numRows.Value < 1) return;

            var rect = GetImageDisplayRectangle(_pictureBox);
            if (rect.Width <= 0 || rect.Height <= 0) return;

            using var pen = new Pen(Color.FromArgb(200, Color.Lime), 1f);
            pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Center;

            int cols = (int)_numCols.Value;
            int rows = (int)_numRows.Value;

            // Draw vertical lines
            for (int c = 1; c < cols; c++)
            {
                float x = rect.Left + (rect.Width * c) / (float)cols;
                e.Graphics.DrawLine(pen, x, rect.Top, x, rect.Bottom);
            }
            // Draw horizontal lines
            for (int r = 1; r < rows; r++)
            {
                float y = rect.Top + (rect.Height * r) / (float)rows;
                e.Graphics.DrawLine(pen, rect.Left, y, rect.Right, y);
            }

            // Draw border
            using var borderPen = new Pen(Color.FromArgb(220, Color.DeepSkyBlue), 2f);
            e.Graphics.DrawRectangle(borderPen, rect);
        }

        private Rectangle GetImageDisplayRectangle(PictureBox pb)
        {
            if (pb.Image == null) return Rectangle.Empty;
            var img = pb.Image;
            var pbWidth = pb.ClientSize.Width;
            var pbHeight = pb.ClientSize.Height;
            var imgWidth = img.Width;
            var imgHeight = img.Height;

            float ratio = Math.Min(pbWidth / (float)imgWidth, pbHeight / (float)imgHeight);
            int displayWidth = (int)(imgWidth * ratio);
            int displayHeight = (int)(imgHeight * ratio);
            int posX = (pbWidth - displayWidth) / 2;
            int posY = (pbHeight - displayHeight) / 2;
            return new Rectangle(posX, posY, displayWidth, displayHeight);
        }

        private void SaveWithOverlay()
        {
            if (_originalImage == null) return;
            using var bmp = RenderOverlayOnOriginal();
            using var sfd = new SaveFileDialog
            {
                Filter = "PNG Image|*.png|JPEG Image|*.jpg;*.jpeg|Bitmap Image|*.bmp",
                FileName = $"screenshot-grid-{DateTime.Now:yyyyMMdd-HHmmss}.png"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                var ext = System.IO.Path.GetExtension(sfd.FileName).ToLowerInvariant();
                var format = ext switch
                {
                    ".jpg" or ".jpeg" => System.Drawing.Imaging.ImageFormat.Jpeg,
                    ".bmp" => System.Drawing.Imaging.ImageFormat.Bmp,
                    _ => System.Drawing.Imaging.ImageFormat.Png,
                };
                bmp.Save(sfd.FileName, format);
            }
        }

        private void CopyWithOverlay()
        {
            if (_originalImage == null) return;
            using var bmp = RenderOverlayOnOriginal();
            Clipboard.SetImage((Image)bmp.Clone());
        }

        private Bitmap RenderOverlayOnOriginal()
        {
            if (_originalImage == null) throw new InvalidOperationException("No image captured");
            var cols = Math.Max(1, (int)_numCols.Value);
            var rows = Math.Max(1, (int)_numRows.Value);
            var w = _originalImage.Width;
            var h = _originalImage.Height;
            var result = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using var g = Graphics.FromImage(result);
            g.DrawImageUnscaled(_originalImage, 0, 0);

            using var pen = new Pen(Color.Lime, 1f);
            // Vertical lines
            for (int c = 1; c < cols; c++)
            {
                float x = (w * c) / (float)cols;
                g.DrawLine(pen, x, 0, x, h);
            }
            // Horizontal lines
            for (int r = 1; r < rows; r++)
            {
                float y = (h * r) / (float)rows;
                g.DrawLine(pen, 0, y, w, y);
            }

            using var borderPen = new Pen(Color.DeepSkyBlue, 2f);
            g.DrawRectangle(borderPen, 0, 0, w - 1, h - 1);
            return result;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _originalImage?.Dispose();
                _pictureBox?.Dispose();
                _numCols?.Dispose();
                _numRows?.Dispose();
                _saveButton?.Dispose();
                _copyButton?.Dispose();
                _recaptureButton?.Dispose();
                _closeButton?.Dispose();
                _mainLayout?.Dispose();
                _controlsPanel?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
