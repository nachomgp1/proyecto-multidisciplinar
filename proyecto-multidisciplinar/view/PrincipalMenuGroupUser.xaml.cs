using Npgsql;
using proyecto_multidisciplinar.model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection.Metadata;
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

    public partial class PrincipalMenuGroupUser : Window
    {
        private string username;
        public PrincipalMenuGroupUser(string username)
        {
            InitializeComponent();

            this.username = username;

            string displayUsername = username.Split('@')[0];

            usernameLabel.Content = "Logged user:" + " " + displayUsername;
            

            FillGroupComboBox(username,userGroupComboBox);
        }



        private static void FillGroupComboBox(string username, ComboBox userGroupComboBox)
        {

            Conexion conexion = new Conexion();
            int groupUser = ObtainGroupUser(username);

            if (conexion.AbrirConexion())
            {
                string query = "SELECT username FROM \"Users\" where \"group\" = @groupUser";

                NpgsqlParameter paramGroup = new NpgsqlParameter("@groupUser", groupUser);

                NpgsqlDataReader reader = conexion.EjecutarConsulta(query, paramGroup);

                List<string> names = new List<string>();

                while (reader.Read())
                {
                    names.Add(reader.GetString(0));
                }

                userGroupComboBox.ItemsSource = names;

                reader.Close();
                conexion.CerrarConexion();
            }
        }

        private static int ObtainGroupUser(string username) {
            Conexion conexion = new Conexion();
            int groupUser = 0;

            if (conexion.AbrirConexion())
            {

                string query = "SELECT \"group\" FROM \"Users\" where username = @username";

                NpgsqlParameter paramUser = new NpgsqlParameter("@username", username);

                NpgsqlDataReader reader = conexion.EjecutarConsulta(query, paramUser);

                if (reader.Read())
                {
                    groupUser = reader.GetInt32(0);
                }

                reader.Close();
                conexion.CerrarConexion();
            }
            return groupUser;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();

        }

        private void Button_Click_Ftp(object sender, RoutedEventArgs e)
        {
            ViewFtpUser viewFtpUser = new ViewFtpUser(username);
            this.Close();
            viewFtpUser.Show();

        }

        private void Button_Click_Email(object sender, RoutedEventArgs e)
        {
            MainWindowMail viewMainMail = new MainWindowMail(username);
            this.Close();
            viewMainMail.Show();

        }

        private void Button_Click_Ftp_Group(object sender, RoutedEventArgs e)
        {

        }
    }
}
