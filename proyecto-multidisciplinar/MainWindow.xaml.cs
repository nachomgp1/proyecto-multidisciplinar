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

            ValidateUser(username, password);

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

                if (reader.Read() && reader.GetInt32(0)>0 )
                {
                    MessageBox.Show("Login exitoso");
                }
                else
                {
                    MessageBox.Show("Usuario o contraseña no válidos");
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
    }
}