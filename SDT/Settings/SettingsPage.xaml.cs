using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace SDT
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsPage : MetroWindow
    {
        public SettingsPage()
        {
            InitializeComponent();

            List<KeyValuePair<string, Color>> cols = new List<KeyValuePair<string, Color>>
            {
                new KeyValuePair<string, Color>("Blue", Color.Blue),
                new KeyValuePair<string, Color>("Brown", Color.Brown),
                new KeyValuePair<string, Color>("Crimson", Color.Crimson),
                new KeyValuePair<string, Color>("Cyan", Color.Cyan),
                new KeyValuePair<string, Color>("Indigo", Color.Indigo),
                new KeyValuePair<string, Color>("Lime", Color.Lime),
                new KeyValuePair<string, Color>("Magenta", Color.Magenta),
                new KeyValuePair<string, Color>("Olive", Color.Olive),
                new KeyValuePair<string, Color>("Orange", Color.Orange),
                new KeyValuePair<string, Color>("Pink", Color.Pink),
                new KeyValuePair<string, Color>("Purple", Color.Purple),
                new KeyValuePair<string, Color>("Red", Color.Red),
                new KeyValuePair<string, Color>("Sienna", Color.Sienna),
                new KeyValuePair<string, Color>("Teal", Color.Teal),
                new KeyValuePair<string, Color>("Yellow", Color.Yellow),
                new KeyValuePair<string, Color>("Violet", Color.Violet),
            };

            ColorsSelect.ItemsSource = cols;
            ColorsSelect.DisplayMemberPath = "Key";
            ColorsSelect.SelectedValue = "Value";
        }

        private readonly MainWindow _MetroWindow;

        public SettingsPage(MainWindow MetroWindow)
        {
            _MetroWindow = MetroWindow;
        }

        /// <summary>
        /// Change Accent color
        /// </summary>
        private void ColorsSelectOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedColor = ColorsSelect.SelectedItem as KeyValuePair<string, Color>?;
            if (selectedColor.HasValue)
            {
                try
                {
                    var selectedAccent = ColorsSelect.SelectedItem.ToString();

                        var theme = ThemeManager.DetectAppStyle(Application.Current).Item1.Name;
                        var test = selectedColor.Value.ToString();
                        var accentCC = test.Split('[', ',')[1];
                        Properties.Settings.Default.AccentC = accentCC;

                        ThemeManager.ChangeAppStyle(Application.Current,
                            ThemeManager.GetAccent(accentCC),
                            ThemeManager.GetAppTheme(theme));
                }
                catch (System.Exception er)
                {
                    var window = Application.Current.Windows.OfType<MetroWindow>().LastOrDefault();
                    if (window != null)
                        window.ShowMessageAsync("Błąd!", er.Message);
                    return;
                }
            }
        }

        /// <summary>
        /// Change Theme color
        /// </summary>
        private void ChangeAppThemeButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var theme = ThemeManager.DetectAppStyle(Application.Current);
                ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, ThemeManager.GetAppTheme("Base" + ((Button)sender).Name));

                Properties.Settings.Default.ThemeC = ThemeManager.DetectAppStyle(Application.Current).Item1.Name;
            }
            catch (System.Exception er)
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().LastOrDefault();
                if (window != null)
                    window.ShowMessageAsync("Błąd!", er.Message);
                return;
            }
        }


    }
}
