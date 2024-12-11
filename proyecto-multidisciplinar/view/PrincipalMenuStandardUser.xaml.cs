using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace proyecto_multidisciplinar.view
{
    
    public partial class PrincipalMenuStandardUser : Window
    {
        private string username;
        public PrincipalMenuStandardUser(string username)
        {
            InitializeComponent();

            this.username = username;

            usernameLabel.Content = "Logged user:" + " " + username;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();
        }
    }
}
