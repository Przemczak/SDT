using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Linq;
using System.Windows;


namespace SDT
{
    /// <summary>
    /// Interaction logic for PC_Installer.xaml
    /// </summary>
    public partial class PC_Installer : MetroWindow
    {
        public PC_Installer()
        {
            InitializeComponent();
        }

        private void Button_Stacja_Copy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (Combo_Instal.Text)
                {
                    case "OPSCCM2012BY1":

                        break;
                    case "OPSCCM2012GD1":

                        break;
                    case "OPSCCM2012KA1":

                        break;
                    case "OPSCCM2012KR1":

                        break;
                    case "OPSCCM2012OL1":

                        break;
                    case "OPSCCM2012PO1":

                        break;
                    case "OPSCCM2012WA1":

                        break;
                    case "OPSCCM2012WA2":

                        break;
                    case "OPSCCM2012WR1":

                        break;
                    case "OPSCCM2012SZ1":

                        break;
                }
            }
            catch (Exception er)
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                if (window != null)
                    window.ShowMessageAsync("Błąd!", er.Message);
                return;
            }
        }

        private void Button_InstaExplo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Insta_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_InstaCopy_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_InstaTemp_Click(object sender, RoutedEventArgs e)
        {

        }
    }


//OPSCCM2012BY1 Bydgoszcz
//OPSCCM2012GD1 Gdańsk
//OPSCCM2012KA1 Katowice
//OPSCCM2012KR1 Kraków
//OPSCCM2012OL1 Olsztyn
//OPSCCM2012PO1 Poznań
//OPSCCM2012WA1 Warszawa
//OPSCCM2012WA2 Warszawa
//OPSCCM2012WR1 Wrocław
//OPSCCM2012SZ1 Szczecin

}
