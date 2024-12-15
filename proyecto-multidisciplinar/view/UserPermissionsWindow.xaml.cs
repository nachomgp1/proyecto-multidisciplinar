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
    /// Lógica de interacción para UserPermissionsWindow.xaml
    /// </summary>
    public partial class UserPermissionsWindow : Window
    {
        private string? username;
        public UserPermissionsWindow(string? username)
        {
            InitializeComponent();
            this.username = username;
        }
    }
}
