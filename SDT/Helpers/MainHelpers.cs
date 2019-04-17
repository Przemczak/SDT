using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SDT.Helpers
{
    class MainHelpers
    {
        private readonly MainWindow _mainWindow;

        public MainHelpers(MainWindow MainWindow)
        {
            _mainWindow = MainWindow;
        }

        /// <summary>
        /// Clear textboxes and checkboxes in User
        /// </summary>
        public void ClearUser()
        {
            foreach (TextBox c in _mainWindow.userGrid.Children.OfType<TextBox>())
            {
                if(c.Name == "userLoginTextBox"){}
                else
                { if (c is TextBox && c != null) { c.Text = string.Empty; }}
            }
        }

        /// <summary>
        /// Clear textboxes in Pc
        /// </summary>
        public void ClearPcInfo()
        {
            foreach (TextBox c in _mainWindow.pcGrid.Children.OfType<TextBox>())
            {
                if (c.Name == "pcTextBox") { }
                else
                { if (c is TextBox && c != null) { c.Text = string.Empty; } }
            }
        }

        /// <summary>
        /// Clear textboxes in Printer
        /// </summary>
        public void ClearPrinter()
        {
            foreach (TextBox c in _mainWindow.printerGrid.Children.OfType<TextBox>())
            {
                if (c.Name == "printerTextBox") { }
                else
                { if (c is TextBox && c != null) { c.Text = string.Empty; } }
            }
        }

        /// <summary>
        /// Check textbox for USER
        /// </summary>
        public bool CheckTextBoxUser()
        {
            if (string.IsNullOrWhiteSpace(_mainWindow.userLoginTextBox.Text))
            {
                _mainWindow.popupText.Text = "Podaj login użytkownika.";
                _mainWindow.mainPopupBox.IsPopupOpen = true;
                return false;
            }
            else { return true; }
        }

        /// <summary>
        /// Check textbox for PC
        /// </summary>
        public bool CheckTextBoxPc()
        {
            if (string.IsNullOrWhiteSpace(_mainWindow.pcTextBox.Text))
            {
                _mainWindow.popupText.Text = "Podaj adres stacji.";
                _mainWindow.mainPopupBox.IsPopupOpen = true;
                return false;
            }
            else { return true; }
        }

        /// <summary>
        /// Check textbox for Printer
        /// </summary>
        public bool CheckTextBoxPrinter()
        {
            if (string.IsNullOrWhiteSpace(_mainWindow.printerTextBox.Text))
            {
                _mainWindow.popupText.Text = "Podaj IP/NS drukarki.";
                _mainWindow.mainPopupBox.IsPopupOpen = true;
                return false;
            }
            else { return true; }
        }
    }
}
