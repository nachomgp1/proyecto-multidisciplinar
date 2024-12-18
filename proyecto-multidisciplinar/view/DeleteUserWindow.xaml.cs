using Npgsql;
using proyecto_multidisciplinar.model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace proyecto_multidisciplinar.view
{
    public partial class DeleteUserWindow : Window
    {
        Dictionary<string, int> userDictionary = new Dictionary<string, int>();
        private Conexion conexion = new Conexion();
        private string username;
        ViewFtpAdmin viewFtpAdmin;
        public DeleteUserWindow(string? username)
        {
            this.username = username;
            InitializeComponent();
            LoadHUD();
           viewFtpAdmin = new ViewFtpAdmin(username);
        }
        public void Exit_Click(object sernder, RoutedEventArgs e)
        {
            UserManagementOptions viewAdmid = new UserManagementOptions(username);
            this.Close();
            viewAdmid.Show();
        }
        private void LoadHUD()
        {
            try
            {
               
                conexion.AbrirConexion();
                string queryUsers = "SELECT id, username FROM \"Users\" WHERE username NOT LIKE 'User';";
                using (NpgsqlDataReader reader = conexion.EjecutarConsulta(queryUsers))
                {
                    userDictionary.Clear();
                    usersComboBox.Items.Clear(); 

                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        userDictionary[name] = id;
                        usersComboBox.Items.Add(name);
                    }
                }
                conexion.CerrarConexion();

               //select the first user
                if (usersComboBox.Items.Count > 0)
                {
                    usersComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}");
            }
        }

        private void deleteUserAction(object sender, RoutedEventArgs e)
        {
            try
            {
                
                string selectedUser = usersComboBox.SelectedItem as string;
                if (selectedUser == null)
                {
                    MessageBox.Show("Please select a user to delete.");
                    return;
                }

                int userId = userDictionary[selectedUser];

                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete user '{selectedUser}'?",
                                                          "Confirm Delete",
                                                          MessageBoxButton.YesNo,
                                                          MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    string selectEmail = "SELECT email, username FROM \"Users\" WHERE id = @id";
                    conexion.AbrirConexion();
                    NpgsqlDataReader reader = conexion.EjecutarConsulta(selectEmail, new NpgsqlParameter("@id", userId));
                    string email="";
                    string sqlUser = "";
                    while(reader.Read())
                    {
                        email = reader.GetString(0);
                        sqlUser = reader.GetString(1);

                    }
                    conexion.CerrarConexion();

                   

                    viewFtpAdmin.deleteDirectory("/", sqlUser);

                    string deleteFromFolders = "DELETE FROM \"Folders\" WHERE name = @name";
                    conexion.AbrirConexion();
                    conexion.EjecutarNonQuery(deleteFromFolders, new NpgsqlParameter("@name", sqlUser));
                    conexion.CerrarConexion();

                    string deleteFromWhitelist = "DELETE FROM \"Whitelist\" WHERE email = @email";
                    conexion.AbrirConexion();
                    conexion.EjecutarNonQuery(deleteFromWhitelist, new NpgsqlParameter("@email", email));
                    conexion.CerrarConexion();

                    string deleteQuery = "DELETE FROM \"Users\" WHERE id = @id";
                    conexion.AbrirConexion();
                    conexion.EjecutarNonQuery(deleteQuery, new NpgsqlParameter("@id", userId));
                    conexion.CerrarConexion();

                    usersComboBox.Items.Remove(selectedUser);
                    userDictionary.Remove(selectedUser);

                    MessageBox.Show($"User '{selectedUser}' deleted successfully.");
                    Logs.InsertLogs(username, "Deleted User", DateTime.Now, MainWindow.GetLocalIpAdress(), MainWindow.GetEmail(username));

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting user: {ex.Message}");
                Logs.InsertLogs(username, "Failed Deleted User", DateTime.Now, MainWindow.GetLocalIpAdress(), MainWindow.GetEmail(username));

            }
            finally
            {
                // Asegurar cierre de conexión
                conexion.CerrarConexion();
            }
        }
    }
}
