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
using System.Text.RegularExpressions;
using System.Windows.Media;

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
                conexion.CerrarConexion();
            }

        }
        private bool IsValidEmail(string email)
        {
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            bool isValid = Regex.IsMatch(email, emailPattern);
            return isValid;
        }

        private void SignIn(object sender, RoutedEventArgs e)
            {
            user = userTextBox?.Text;
            password = passwordTextBox?.Password;
            email = emailTextBox.Text;
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Invalid email format. Please enter a valid email.");
                sendAcction("sign-in error");
                return;
            }

           
            

            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Username and Password cannot be empty.");
                sendAcction("sign-in error");
                return;
            }

          
            

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
            Conexion conexion = new Conexion();

            try
            {
               
                if (!conexion.AbrirConexion())
                {
                    MessageBox.Show("Error al abrir la conexión.");
                    return;
                }

                string checkUserQuery = "SELECT * FROM \"Users\" WHERE username = @username OR email = @email;";
                using (var reader = conexion.EjecutarConsulta(checkUserQuery,
                            new NpgsqlParameter("@username", user),
                            new NpgsqlParameter("@email", email)))
                {
                    if (reader.HasRows) 
                    {
                        MessageBox.Show("The username or the email already exists.");
                        sendAcction("sign-in error");
                        return; 
                    }
                    reader.Close(); 
                }

               
                string insertUserQuery;
                List<NpgsqlParameter> parametros = new List<NpgsqlParameter>
        {
            new NpgsqlParameter("@username", user),
            new NpgsqlParameter("@email", email),
            new NpgsqlParameter("@password", password),
            new NpgsqlParameter("@sentMessages", message_left),
            new NpgsqlParameter("@type", int.Parse(userType))
        };

                if (userType != "2") 
                {
                    insertUserQuery = @"INSERT INTO ""Users"" (username, email, password, messages_left, ""group"", type) 
                                VALUES (@username, @email, @password, @sentMessages, NULL, @type);";
                }
                else 
                {
                    insertUserQuery = @"INSERT INTO ""Users"" (username, email, password, messages_left, ""group"", type) 
                                VALUES (@username, @email, @password, @sentMessages, @group, @type);";
                    parametros.Add(new NpgsqlParameter("@group", userGroup));
                }

                conexion.EjecutarNonQuery(insertUserQuery, parametros.ToArray());
                MessageBox.Show($"{user} was created successfully.");

                
                string whitelistQuery = @"INSERT INTO ""Whitelist"" (email) VALUES (@Email);";
                conexion.EjecutarNonQuery(whitelistQuery, new NpgsqlParameter("@Email", email));
                MessageBox.Show("Email added successfully to Whitelist.");

                sendAcction("Successfully sign-in");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during user creation: {ex.Message}");
            }
            finally
            {
                
                conexion.CerrarConexion();
                PrincipalMenuAdmin viewAdmid = new PrincipalMenuAdmin(adminUser);
                this.Close();
                viewAdmid.Show();
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
            UserManagementOptions viewAdmid = new UserManagementOptions(adminUser);
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
        private void EmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(emailTextBox.Text, emailPattern))
            {
                emailTextBox.BorderBrush = new SolidColorBrush(Colors.Red); 
            }
            else
            {
                emailTextBox.BorderBrush = new SolidColorBrush(Colors.Green); 
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
