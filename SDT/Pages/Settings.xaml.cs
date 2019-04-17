using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SDT.Pages
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();

            List<KeyValuePair<string, string>> accents = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Amber", "amber"),
                new KeyValuePair<string, string>("Blue", "blue"),
                new KeyValuePair<string, string>("Bluegrey", "bluegrey"),
                new KeyValuePair<string, string>("Brown", "brown"),
                new KeyValuePair<string, string>("Cyan", "cyan"),
                new KeyValuePair<string, string>("Deeporange", "deeporange"),
                new KeyValuePair<string, string>("Deeppurple", "deeppurple"),
                new KeyValuePair<string, string>("Green", "green"),
                new KeyValuePair<string, string>("Indigo", "indigo"),
                new KeyValuePair<string, string>("Lightblue", "lightblue"),
                new KeyValuePair<string, string>("Lightgreen", "lightgreen"),
                new KeyValuePair<string, string>("Lime", "lime"),
                new KeyValuePair<string, string>("Orange", "orange"),
                new KeyValuePair<string, string>("Pink", "pink"),
                new KeyValuePair<string, string>("Purple", "purple"),
                new KeyValuePair<string, string>("Red", "red"),
                new KeyValuePair<string, string>("Teal", "teal"),
                new KeyValuePair<string, string>("Yellow", "yellow")
            };

            accentSelectComboBox.ItemsSource = accents;
            accentSelectComboBox.DisplayMemberPath = "Key";
            accentSelectComboBox.SelectedValue = "Value";

            bool isDark = Properties.Settings.Default.Theme;

            if (isDark) { themeToggleButton.IsChecked = true; }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        /// <summary>
        /// Theme Settings
        /// </summary>
        private void ThemeToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            bool isDark = true;
            new PaletteHelper().SetLightDark(isDark);

            Properties.Settings.Default.Theme = isDark;
        }

        private void ThemeToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            bool isDark = false;
            new PaletteHelper().SetLightDark(isDark);

            Properties.Settings.Default.Theme = isDark;
        }

        /// <summary>
        /// Accents Settings
        /// </summary>
        private void AccentSelectComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selectedAccent = accentSelectComboBox.SelectedItem as KeyValuePair<string, string>?;
            string accent = selectedAccent.Value.Value.ToString();

            var swatchesProvider = new SwatchesProvider();
            var changeSwatch = swatchesProvider.Swatches.Single(
                swatch => swatch.Name == accent);

            new PaletteHelper().ReplacePrimaryColor(changeSwatch);

            Properties.Settings.Default.Accent = accent;
        }

        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
            Close();
        }

        private void SettingsCloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
