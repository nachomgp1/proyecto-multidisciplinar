using proyecto_multidisciplinar.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
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
    public partial class LogsView : Window
    {
        private string? username;
        public LogsView(string? username)
        {

            InitializeComponent();

            this.username = username;

            Logs.CreateTableLogs(mainPanel);

            Logs.CreateButtons(buttonsPanel, OnBackButtonClick);
        }

        public void OnBackButtonClick()
        {
            PrincipalMenuAdmin principalAdminView = new PrincipalMenuAdmin(username);
            principalAdminView.Show();
            this.Close();
            
            
        }

        
    }
}
