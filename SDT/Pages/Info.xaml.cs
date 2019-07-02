using System;
using System.Reflection;
using System.Windows;
using System.IO;

namespace SDT.Pages
{
    /// <summary>
    /// Interaction logic for Info.xaml
    /// </summary>
    public partial class Info : Window
    {
        public Info()
        {
            InitializeComponent();

            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            versionTextBox.Text = version.Major + "." + version.Minor + "." + version.Build;

            LoadPatchNotes();
        }

        private void SettingsCloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void LoadPatchNotes()
        {
            try
            {
                patchNotesTextBlock.Text = File.ReadAllText(@"\\Dsb192\sdt_resources$\Patch_Notes.txt");
            }
            catch (Exception)
            {
                patchNotesTextBlock.Text = "Błąd danych";
            }
        }
    }
}
