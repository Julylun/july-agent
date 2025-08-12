using System.Drawing;
using JulyAgent.Constants;

namespace JulyAgent.Utils
{
    public static class ThemeManager
    {
        public static class DarkTheme
        {
            public static Color Background => Color.FromArgb(AppConstants.Colors.DarkBackground, AppConstants.Colors.DarkBackground, AppConstants.Colors.DarkBackground);
            public static Color Foreground => Color.FromArgb(AppConstants.Colors.DarkForeground, AppConstants.Colors.DarkForeground, AppConstants.Colors.DarkForeground);
            public static Color Primary => Color.FromArgb(AppConstants.Colors.PrimaryBlue);
            public static Color Secondary => Color.FromArgb(AppConstants.Colors.SecondaryGray, AppConstants.Colors.SecondaryGray, AppConstants.Colors.SecondaryGray);
            public static Color Success => Color.FromArgb(AppConstants.Colors.SuccessGreen);
            public static Color Warning => Color.FromArgb(AppConstants.Colors.WarningOrange);
            public static Color Error => Color.FromArgb(AppConstants.Colors.ErrorRed);
            public static Color Text => Color.White;
            public static Color Border => Color.FromArgb(60, 60, 60);
        }

        public static class LightTheme
        {
            public static Color Background => Color.White;
            public static Color Foreground => Color.FromArgb(240, 240, 240);
            public static Color Primary => Color.FromArgb(AppConstants.Colors.PrimaryBlue);
            public static Color Secondary => Color.FromArgb(200, 200, 200);
            public static Color Success => Color.FromArgb(AppConstants.Colors.SuccessGreen);
            public static Color Warning => Color.FromArgb(AppConstants.Colors.WarningOrange);
            public static Color Error => Color.FromArgb(AppConstants.Colors.ErrorRed);
            public static Color Text => Color.Black;
            public static Color Border => Color.FromArgb(180, 180, 180);
        }

        public static void ApplyTheme(Form form, string theme)
        {
            var colors = GetThemeColors(theme);
            
            form.BackColor = colors.Background;
            form.ForeColor = colors.Text;

            foreach (Control control in form.Controls)
            {
                ApplyThemeToControl(control, colors);
            }
        }

        public static void ApplyThemeToControl(Control control, ThemeColors colors)
        {
            if (control is Button button)
            {
                ApplyButtonTheme(button, colors);
            }
            else if (control is TextBox textBox)
            {
                ApplyTextBoxTheme(textBox, colors);
            }
            else if (control is RichTextBox richTextBox)
            {
                ApplyRichTextBoxTheme(richTextBox, colors);
            }
            else if (control is ComboBox comboBox)
            {
                ApplyComboBoxTheme(comboBox, colors);
            }
            else if (control is Label label)
            {
                label.ForeColor = colors.Text;
            }

            // Recursively apply to child controls
            foreach (Control child in control.Controls)
            {
                ApplyThemeToControl(child, colors);
            }
        }

        private static void ApplyButtonTheme(Button button, ThemeColors colors)
        {
            if (button.Text.Contains("OK") || button.Text.Contains("Save") || button.Text.Contains("Copy"))
            {
                button.BackColor = colors.Primary;
                button.ForeColor = Color.White;
            }
            else
            {
                button.BackColor = colors.Secondary;
                button.ForeColor = colors.Text;
            }

            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Cursor = Cursors.Hand;
        }

        private static void ApplyTextBoxTheme(TextBox textBox, ThemeColors colors)
        {
            textBox.BackColor = colors.Foreground;
            textBox.ForeColor = colors.Text;
            textBox.BorderStyle = BorderStyle.FixedSingle;
        }

        private static void ApplyRichTextBoxTheme(RichTextBox richTextBox, ThemeColors colors)
        {
            richTextBox.BackColor = colors.Foreground;
            richTextBox.ForeColor = colors.Text;
            richTextBox.BorderStyle = BorderStyle.FixedSingle;
        }

        private static void ApplyComboBoxTheme(ComboBox comboBox, ThemeColors colors)
        {
            comboBox.BackColor = colors.Foreground;
            comboBox.ForeColor = colors.Text;
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        public static ThemeColors GetThemeColors(string theme)
        {
            return theme.ToLower() switch
            {
                "light" => new ThemeColors
                {
                    Background = LightTheme.Background,
                    Foreground = LightTheme.Foreground,
                    Primary = LightTheme.Primary,
                    Secondary = LightTheme.Secondary,
                    Text = LightTheme.Text,
                    Border = LightTheme.Border
                },
                _ => new ThemeColors
                {
                    Background = DarkTheme.Background,
                    Foreground = DarkTheme.Foreground,
                    Primary = DarkTheme.Primary,
                    Secondary = DarkTheme.Secondary,
                    Text = DarkTheme.Text,
                    Border = DarkTheme.Border
                }
            };
        }

        public struct ThemeColors
        {
            public Color Background { get; set; }
            public Color Foreground { get; set; }
            public Color Primary { get; set; }
            public Color Secondary { get; set; }
            public Color Text { get; set; }
            public Color Border { get; set; }
        }
    }
}
