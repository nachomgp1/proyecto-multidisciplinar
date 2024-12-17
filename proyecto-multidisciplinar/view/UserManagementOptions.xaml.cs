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
    /// <summary>
    /// Lógica de interacción para UserManagementOptions.xaml
    /// </summary>
    
    public partial class UserManagementOptions : Window
    {
        private string? username;

        public UserManagementOptions(string? username)
        {
            this.username = username;
            InitializeComponent();
        }
        public void UserPermissions_Click(object senedr,  RoutedEventArgs e)
        {
            UserPermissionsWindow userPermissionsWindow = new UserPermissionsWindow(username);
            this.Close();
            userPermissionsWindow.Show();

        }
        public void UserCreation_Click(object sernder, RoutedEventArgs e)
        {
            UserSignInWindow signIngWindow = new UserSignInWindow(username);
            this.Close();
            signIngWindow.Show();
        }
        public void Exit_Click(object sernder, RoutedEventArgs e)
        {
            PrincipalMenuAdmin viewAdmid = new PrincipalMenuAdmin(username);
            this.Close();
            viewAdmid.Show();
        }
       public void CreateGroup_Click(object sernder, RoutedEventArgs e)
        {
            GroupsCreatorWindow groupsCreatorWindow = new GroupsCreatorWindow(username);
            this.Close();
            groupsCreatorWindow.Show();
        }
    }
}
