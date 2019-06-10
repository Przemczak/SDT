using System;
using System.Reflection;
using System.Windows;

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
            patchNotesTextBlock.Text = ""
                + Environment.NewLine + "Wersja 1.4.4 (20.20.2010):"
                + Environment.NewLine + "- Dodano skrypt włączający usługi CcmExec/CmRcService/RemoteRegistry"
                + Environment.NewLine + "- Dodano Listę Zmian"
                + Environment.NewLine + "- Poprawiono sprawdzanie informacji o stacji"
                + Environment.NewLine + "- Drukarki: "
                + Environment.NewLine + "- Dodano pole Fax"
                + Environment.NewLine + "- Poprawiono wyłaczenie się paska postępu"
                + Environment.NewLine + "- Poprawiono wyświetlanie piętra/pokoju/inne"
                + Environment.NewLine + ""
                + Environment.NewLine + "Wersja 1.4.3 (12.04.2019):"
                + Environment.NewLine + "- Dodanie daty aktualizacji systemu"
                + Environment.NewLine + ""
                + Environment.NewLine + "Wersja 1.4.2 (05.04.2019):"
                + Environment.NewLine + "- Poprawiono błąd przy uruchamianiu skryptów"
                + Environment.NewLine + ""
                + Environment.NewLine + "Wersja 1.4.1 (03.04.2019):"
                + Environment.NewLine + "- Zmiana GUI"
                + Environment.NewLine + "- Dodanie możliwości sprawdzania drukarek"
                + Environment.NewLine + "- PC:"
                + Environment.NewLine + "- Dodanie sprawdzanie grup w AD: Comp-Joulex-wyjatki-GD, Komp-Info-Noc-GD, Comp-AlterBrow-GD, ProxyBSTBlokada"
                + Environment.NewLine + "- Dodanie zainstalowanego systemu operacyjnego na stacji"
                + Environment.NewLine + "- Dodanie wersji systemu operacyjnego (Build - Version)"
                + Environment.NewLine + "- Użytkownik:"
                + Environment.NewLine + "- Dodanie daty utworzenie konta w AD";
        }
    }
}
