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
    
    public partial class PrincipalMenuAdmin : Window
    {
        private string username;
       
        public PrincipalMenuAdmin(string username)
        {
            InitializeComponent();

            this.username = username;

            string displayUsername = username.Split('@')[0];

            usernameLabel.Content = "Logged user:" + " " + displayUsername;


        }

        private void Button_Click_Exit(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();
        }

        private void Button_Click_Ftp(object sender, RoutedEventArgs e)
        {
            //ViewFtpAdmin ftpWindow = new ViewFtpAdmin(username);
            //this.Close();
            //ftpWindow.Show();
            

        }
        private void Button_Click_Email(object sender, RoutedEventArgs e)
        {
            //PrincipalEmailWindow emailWindow = new PrincipalEmailWindow(email);
            //this.Close();
            //emailWindow.Show();
            

        }

        private void Button_Click_User_Management(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_Logs(object sender, RoutedEventArgs e)
        {
            LogsView logView = new LogsView(username);
            logView.Show();
            this.Close();

        }
    }
}
