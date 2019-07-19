using System.Windows;

namespace SDT.Pages
{
    /// <summary>
    /// Interaction logic for PasswordWindow.xaml
    /// </summary>
    public partial class PasswordWindow : Window
    {
        private readonly Helpers.PasswordGenerator _passwordGenerator;

        public PasswordWindow()
        {
            InitializeComponent();
            _passwordGenerator = new Helpers.PasswordGenerator(this);
        }

        private void generateButton_Click(object sender, RoutedEventArgs e)
        {
            _passwordGenerator.Generate();

            passwordTextBox.Focus();
            passwordTextBox.SelectAll();
        }
    }
}
