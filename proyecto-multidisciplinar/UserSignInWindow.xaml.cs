using Npgsql;
using proyecto_multidisciplinar.model;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;

namespace proyecto_multidisciplinar
{
    public partial class UserSignInWindow : Window
    {
        public static string radioButtonSelected;
        public static int userGroup = 0;
        public static string user=null;
        public static string email = null;
        public static string password = null;
        public static string userType = null;

        Conexion conexion = new Conexion();
        Dictionary<string, int> groupDictionary = new Dictionary<string, int>(); 

        public UserSignInWindow()
        {
            InitializeComponent();
        }

        private void SignIn(object sender, RoutedEventArgs e)
        {
             user = userTextBox.Text;
             email = emailTextBox.Text;
             password = passwordTextBox.Password;
             userType = null;

            if (radioButtonSelected.Equals("standardUser"))
            {
                userType = "1";
            }
            else if (radioButtonSelected.Equals("groupUser"))
            {
                userType = "2";
            }
            else if (radioButtonSelected.Equals("administrator"))
            {
                userType = "3";
            }

           checkUserData(user, email, password, userType);
        }

        public void checkUserData(string user, string email, string password, string userType)
        {
            if (conexion.AbrirConexion())
            {
                try
                {
                   
                    string query = "SELECT * FROM \"Users\" WHERE username = @username OR email = @email";

                    NpgsqlParameter paramUser = new NpgsqlParameter("@username", user);
                    NpgsqlParameter paramEmail = new NpgsqlParameter("@email", email);

                    NpgsqlDataReader reader = conexion.EjecutarConsulta(query, paramUser, paramEmail);
                    
                    if (!reader.HasRows)
                    {
                        reader.Close(); 

                        string queryInsert;

                        if (!userType.Equals("2")) 
                        {
                            
                            queryInsert = "INSERT INTO \"Users\" (username, email, password, sent_messages, type, group) " +
                                          "VALUES (@username, @email, @password, @sentMessages, @type, NULL);";

                            NpgsqlParameter[] parameters = new NpgsqlParameter[] {
                                 new NpgsqlParameter("@username", user),
                                new NpgsqlParameter("@email", email),
                                new NpgsqlParameter("@password", password),
                                new NpgsqlParameter("@sentMessages", NpgsqlTypes.NpgsqlDbType.Integer) { Value = 0 },
                                new NpgsqlParameter("@type", userType),
                                };
                            MessageBox.Show($"username: {user}, email: {email}, password: {password}, sentMessages: 0, type: {userType}, group: NULL");

                            conexion.EjecutarNonQuery(queryInsert,parameters);
                            
                        }
                        else // Si es tipo grupo
                        {
                            queryInsert = "INSERT INTO \"Users\" (username, email, password, sent_messages, type, group) " +
                                          "VALUES (@username, @email, @password, @sentMessages, @type, @group);";
                            NpgsqlParameter[] parameters = new NpgsqlParameter[] {
                                 new NpgsqlParameter("@username", user),
                                new NpgsqlParameter("@email", email),
                                new NpgsqlParameter("@password", password),
                                new NpgsqlParameter("@sentMessages", NpgsqlTypes.NpgsqlDbType.Integer) { Value = 0 },
                                new NpgsqlParameter("@type", userType),
                                new NpgsqlParameter("@group", userGroup)

                            };
                            conexion.EjecutarNonQuery(queryInsert, parameters);
                            
                        }

                        conexion.CerrarConexion();
                        sendAcction("Successfully signed-in");
                    }
                    else
                    {
                        MessageBox.Show("The username or the email already exists");
                        reader.Close();
                        conexion.CerrarConexion();
                        sendAcction("sign-in error");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error during user creation: " + ex.Message);
                    conexion.CerrarConexion();
                   
                }
            }
            else
            {
                MessageBox.Show("Could not connect to the database.");
                
            }
        }

        private void sendAcction(string action)
        {
            try
            {
                // Obtener fecha y hora actual
                DateTime currentDate = DateTime.Now;

                // Obtener la IP local
                string userIp = GetLocalIPAddress();

                conexion.AbrirConexion();

                // Query para insertar en Logs
                string queryInsert = "INSERT INTO \"Logs\" (username, action, date, user_ip) " +
                                     "VALUES (@username, @action, @date, @user_ip)";

                // Parámetros para la consulta
                NpgsqlParameter[] parameters = new NpgsqlParameter[] {
            new NpgsqlParameter("@username", user ?? "Unknown"), // Usar "Unknown" si el usuario no está definido
            new NpgsqlParameter("@action", action),
            new NpgsqlParameter("@date", currentDate),
            new NpgsqlParameter("@user_ip", userIp)
        };

                // Ejecutar la consulta
                conexion.EjecutarNonQuery(queryInsert, parameters);
                conexion.CerrarConexion();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar la acción: " + ex.Message);
                conexion.CerrarConexion();
            }
        }

        private void addGroups(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            radioButtonSelected = radioButton.Name;
            ComboBox existingComboBox = null;

            foreach (var child in userSignIngGrid.Children)
            {
                if (child is ComboBox comboBox && comboBox.Name == "groupsComboBox")
                {
                    existingComboBox = comboBox;
                    break;
                }
            }

            if (existingComboBox != null)
            {
                userSignIngGrid.Children.Remove(existingComboBox);
            }

            if (radioButton.Name == "groupUser")
            {
                ComboBox groups = new ComboBox
                {
                    Name = "groupsComboBox",
                    Width = 200,
                    Height = 30,
                    Margin = new Thickness(600, -290, 0, 0)
                };

                groups.SelectionChanged += Groups_SelectionChanged; 

               
                conexion.AbrirConexion();
                string queryGroups = "SELECT id, name FROM \"Groups\"";
                NpgsqlDataReader reader = conexion.EjecutarConsulta(queryGroups);

                groupDictionary.Clear(); 

                while (reader.Read())
                {
                    int id = reader.GetInt32(0); 
                    string name = reader.GetString(1); 
                    groupDictionary[name] = id; 
                    groups.Items.Add(name); 
                }

                reader.Close();
                conexion.CerrarConexion();

                groups.SelectedIndex = 0; 
                if (groups.Items.Count > 0)
                {
                    userGroup = groupDictionary[(string)groups.SelectedItem]; 
                }

                userSignIngGrid.Children.Add(groups);
            }
        }

        private void Groups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox.SelectedItem != null)
            {
                string selectedGroupName = comboBox.SelectedItem.ToString();
                if (groupDictionary.ContainsKey(selectedGroupName))
                {
                    userGroup = groupDictionary[selectedGroupName]; 
                }
            }
        }
        string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "127.0.0.1"; // Retorna localhost si no encuentra una IP válida
        }
    }
}
