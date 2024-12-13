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

            string displayUsername = username.Split('@')[0];    

            usernameLabel.Content = "Logged user:" + " " + displayUsername;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();
        }

        private void Button_Click_Ftp(object sender, RoutedEventArgs e)
        {


        }

        private void Button_Click_Email(object sender, RoutedEventArgs e)
        {

        }

        
    }
}
