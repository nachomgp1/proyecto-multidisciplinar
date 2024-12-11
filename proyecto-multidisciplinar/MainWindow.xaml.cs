using Npgsql;
using proyecto_multidisciplinar.model;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace proyecto_multidisciplinar
{

    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {

            string username = userTextBox.Text;
            string password = passwordTextBox.Password;

            if (ValidateUser(username, password))
            {
                int typePermission = ObtainUserPermissions(username);

                //Its an administrator
                if (typePermission == 3)
                {
                    view.PrincipalMenuAdmin viewPrincipalMenuAdmin = new view.PrincipalMenuAdmin(username);
                    viewPrincipalMenuAdmin.Show();
                    this.Close();
                }

                //Its an standard user
                else if (typePermission == 1)
                {
                    view.PrincipalMenuStandardUser viewPrincipalMenuUser = new view.PrincipalMenuStandardUser(username);
                    viewPrincipalMenuUser.Show();
                    this.Close();
                }
            }
        }

        private static bool ValidateUser(string username, string password)
        {
            Conexion conexion = new Conexion();
            if (conexion.AbrirConexion())
            {
                string query = "SELECT COUNT(*) FROM \"Users\" where name = @username AND password = @password";

                NpgsqlParameter paramUser = new NpgsqlParameter("@username", username);
                NpgsqlParameter paramPassword = new NpgsqlParameter("@password", password);

                NpgsqlDataReader reader = conexion.EjecutarConsulta(query, paramUser, paramPassword);

                if (reader.Read() && reader.GetInt32(0) > 0)
                {
                    MessageBox.Show("Successful log in");
                }
                else
                {
                    MessageBox.Show("Invalid username or password");
                    return false;
                }

                reader.Close();
                conexion.CerrarConexion();

            }


            return true;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();

        }

        private static int ObtainUserPermissions(string username)
        {
            Conexion conexion = new Conexion();
            int typePermission = 0;

            if (conexion.AbrirConexion())
            {
                string query = "SELECT type FROM \"Users\" where name = @username";

                NpgsqlParameter paramUser = new NpgsqlParameter("@username", username);

                NpgsqlDataReader reader = conexion.EjecutarConsulta(query, paramUser);

                if (reader.Read()){
                    typePermission = reader.GetInt32(0);
                }
                else
                {
                    MessageBox.Show("The user is not assigned permissions");
                    

                }

                reader.Close();
                conexion.CerrarConexion();

            }
            else
            {
                MessageBox.Show("Failed to connect to database");
            }
            return typePermission;
            
            
        }
    }
}