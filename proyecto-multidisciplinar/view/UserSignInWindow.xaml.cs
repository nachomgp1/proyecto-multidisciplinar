using Npgsql;
using proyecto_multidisciplinar.model;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using proyecto_multidisciplinar.view;

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
        public static int? message_left = 0;
        private string? adminUser;
        Conexion conexion = new Conexion();
        Dictionary<string, int> groupDictionary = new Dictionary<string, int>(); 
        
        public UserSignInWindow(string? adminUser)
        {
            this.adminUser = adminUser;
            InitializeComponent();
            inicializateMessage_leftValue();
        }

        private void inicializateMessage_leftValue()
        {
            if (conexion.AbrirConexion())
            {

                string userQuery = "SELECT messages_left FROM \"Users\" WHERE username = 'User'";
                using (var reader = conexion.EjecutarConsulta(userQuery))
                {
                    if (reader.Read())
                    {
                        message_left = reader.GetInt32(0);
                    }
                }
            }
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
            NpgsqlConnection connection = null;
            try
            {
                
                connection = new NpgsqlConnection(conexion.ConnectionString);
                connection.Open();

              
                using (var checkUserCommand = new NpgsqlCommand(
                    "SELECT * FROM \"Users\" WHERE username = @username OR email = @email", connection))
                {
                    checkUserCommand.Parameters.AddWithValue("@username", user);
                    checkUserCommand.Parameters.AddWithValue("@email", email);

                    using (var reader = checkUserCommand.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            reader.Close();

                            
                            string queryInsert;
                            NpgsqlCommand insertCommand;

                            if (userType != "2")
                            {
                               
                               
                                queryInsert = "INSERT INTO \"Users\" (username, email, password, messages_left, \"group\", type) " +
                                              "VALUES (@username, @email, @password, @sentMessages, NULL, @type);";

                                insertCommand = new NpgsqlCommand(queryInsert, connection);
                                insertCommand.Parameters.AddWithValue("@username", user);
                                insertCommand.Parameters.AddWithValue("@email", email);
                                insertCommand.Parameters.AddWithValue("@password", password);
                                insertCommand.Parameters.AddWithValue("@sentMessages", message_left);
                                insertCommand.Parameters.AddWithValue("@type", int.Parse(userType));

                                try
                                {
                                    MessageBox.Show($"username: {user}, email: {email}, password: {password}, sentMessages: {message_left}, type: {userType}, group: NULL");
                                    insertCommand.ExecuteNonQuery();
                                    MessageBox.Show("Datos insertados correctamente.");
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show($"Error al insertar en la base de datos: {ex.Message}");
                                    return;
                                }
                            }
                            else
                            {
                                
                                queryInsert = "INSERT INTO \"Users\" (username, email, password, messages_left, \"group\", type) " +
                                              "VALUES (@username, @email, @password, @sentMessages, @group, @type);";

                                insertCommand = new NpgsqlCommand(queryInsert, connection);
                                insertCommand.Parameters.AddWithValue("@username", user);
                                insertCommand.Parameters.AddWithValue("@email", email);
                                insertCommand.Parameters.AddWithValue("@password", password);
                                insertCommand.Parameters.AddWithValue("@sentMessages", message_left);
                                insertCommand.Parameters.AddWithValue("@type", int.Parse(userType));
                                insertCommand.Parameters.AddWithValue("@group", userGroup);

                                insertCommand.ExecuteNonQuery();
                            }
                            conexion.CerrarConexion();
                            sendAcction("Successfully sign-in");
                        }
                        else
                        {
                            MessageBox.Show("The username or the email already exists");
                            sendAcction("sign-in error");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during user creation: " + ex.Message);
            }
            finally
            {
                connection?.Close();
            }
            try
            {
                if (conexion.AbrirConexion()) 
                {
                    
                    string queryWhiteList = @"INSERT INTO ""Whitelist"" (email) VALUES (@Email);";
                    conexion.EjecutarNonQuery(queryWhiteList,
                        new NpgsqlParameter("@Email", email));

                    MessageBox.Show("Email added successfully to Whitelist.");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during user creation: " + ex.Message);
            }
            finally
            {
                connection?.Close();
            }
        }

        private void sendAcction(string action)
        {
            try
            {
               
                DateTime currentDate = DateTime.Now;

                //get ip
                string userIp = MainWindow.GetLocalIpAdress();
                String email = MainWindow.GetEmail(adminUser);
                conexion.AbrirConexion();

                Logs.InsertLogs(adminUser, action, currentDate, userIp, email);
               
        
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar la acción: " + ex.Message);
                conexion.CerrarConexion();
            }
        }
        public void Exit_Click(object sernder, RoutedEventArgs e)
        {
            PrincipalMenuAdmin viewAdmid = new PrincipalMenuAdmin(adminUser);
            this.Close();
            viewAdmid.Show();
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
    }
}
