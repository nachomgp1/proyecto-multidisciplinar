using Npgsql;
using proyecto_multidisciplinar.model;
using System.Net;
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
            string email = GetEmail(username);

            //Obtaining date and local ip and insert into logs
            DateTime currentDate = DateTime.Now;
            string userIp = GetLocalIpAdress();

            if (ValidateUser(username, password))
            {
                //Inserting log when user is validated
                Logs.InsertLogs(username, "Successfully logged in", currentDate, userIp);

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

                //Its a group user
                else if (typePermission == 2)
                {
                    view.PrincipalMenuGroupUser viewPrincipalGroupUser = new view.PrincipalMenuGroupUser(username);
                    viewPrincipalGroupUser.Show();
                    this.Close();
                }
            }
            else
            {
                //Inserting log when user or password are invalid
                Logs.InsertLogs(username, "Error when logging in", currentDate, userIp);
            }
        }

        private static bool ValidateUser(string username, string password)
        {
            Conexion conexion = new Conexion();
            if (conexion.AbrirConexion())
            {
                string query = "SELECT COUNT(*) FROM \"Users\" where username = @username AND password = @password";

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

        public static int ObtainUserPermissions(string username)
        {
            Conexion conexion = new Conexion();
            int typePermission = 0;

            if (conexion.AbrirConexion())
            {
                string query = "SELECT type FROM \"Users\" where username = @username";

                NpgsqlParameter paramUser = new NpgsqlParameter("@username", username);

                NpgsqlDataReader reader = conexion.EjecutarConsulta(query, paramUser);

                if (reader.Read())
                {
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

        public static string GetLocalIpAdress()
        {
            string localIp = string.Empty;
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        localIp = ip.ToString();
                        break;
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return localIp;
        }

        public static string GetEmail(string? username)
        {
            Conexion conexion = new Conexion();
            string email= string.Empty;
            if (conexion.AbrirConexion())
            {
                string query = "SELECT email FROM \"Users\" WHERE username = @username";

                NpgsqlParameter paramUser = new NpgsqlParameter("@username", username);

                NpgsqlDataReader reader = conexion.EjecutarConsulta(query, paramUser);

                if (reader.Read())
                {
                    int emailIndex = reader.GetOrdinal("email");

                    email = reader.GetString(emailIndex);
                    
                }
                conexion.CerrarConexion();
            }
            else
            {
                MessageBox.Show("Error with email");
            }
            return email;
        }
    }
}