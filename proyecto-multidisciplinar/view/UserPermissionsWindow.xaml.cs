using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Npgsql;
using proyecto_multidisciplinar.model;

namespace proyecto_multidisciplinar.view
{
    public partial class UserPermissionsWindow : Window
    {
        private string? username;
        private Conexion conexion;
        private int messageLimitBase = 0;
        public ObservableCollection<TypeRecord> Types { get; set; }

        public UserPermissionsWindow(string? username)
        {
            InitializeComponent();
            this.username = username;
            conexion = new Conexion();
            LoadData();
        }
        public void AddToWhitelist_Click(object sender, RoutedEventArgs e)
        {
            string whitelistEmail = WhitelistEmailTextBox.Text;

            if (string.IsNullOrWhiteSpace(whitelistEmail))
            {
                MessageBox.Show("Please, insert a valid email.");
                return;
            }

            try
            {
                if (conexion.AbrirConexion())
                {
                   
                    string insertQuery = @"
                INSERT INTO ""Whitelist"" (email)
                VALUES (@Email);";

                    conexion.EjecutarNonQuery(insertQuery,
                        new NpgsqlParameter("@Email", whitelistEmail));

                    MessageBox.Show("Email successfully added to Whitelist.");
                    WhitelistEmailTextBox.Clear();
                    Logs.InsertLogs(username, "Succesfull Email Insert to Whitelist", DateTime.Now, MainWindow.GetLocalIpAdress(), MainWindow.GetEmail(username));

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }


        public void Exit_Click(object sernder, RoutedEventArgs e)
        {
            PrincipalMenuAdmin viewAdmid = new PrincipalMenuAdmin(username);
            this.Close();
            viewAdmid.Show();
        }
        private void LoadData()
        {
            Types = new ObservableCollection<TypeRecord>();

            try
            {
                if (conexion.AbrirConexion())
                {

                    string userQuery = "SELECT messages_left FROM \"Users\" WHERE username = 'User'";
                    using (var reader = conexion.EjecutarConsulta(userQuery))
                    {
                        if (reader.Read())
                        {
                            MessageLimitTextBox.Text = reader.GetInt32(0).ToString();
                            messageLimitBase = reader.GetInt32(0);
                        }
                    }


                    string typesQuery = "SELECT id, name, \"write\", \"read\", \"create\", \"delete\" FROM \"Types\";";
                    using (var reader = conexion.EjecutarConsulta(typesQuery))
                    {
                        while (reader.Read())
                        {
                            Types.Add(new TypeRecord
                            {
                                Id = reader.GetInt32(0),
                                name = reader.GetString(1),
                                write = reader.GetBoolean(2),
                                read = reader.GetBoolean(3),
                                create = reader.GetBoolean(4),
                                delete = reader.GetBoolean(5)
                            });
                        }
                    }

                    TypesDataGrid.ItemsSource = Types;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos: {ex.Message}");
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            NpgsqlConnection connection = null;
            try
            {
                // Create a new connection directly instead of using the Conexion wrapper
                connection = new NpgsqlConnection(conexion.ConnectionString); // Assuming you have a connection string property
                connection.Open();

                // First, update Types
                foreach (var record in Types)
                {
                    using (var updateCommand = new NpgsqlCommand(@"
            UPDATE ""Types""
            SET ""write"" = @write, 
                ""read"" = @read, 
                ""create"" = @create, 
                ""delete"" = @delete
            WHERE id = @id;", connection))
                    {
                        updateCommand.Parameters.AddWithValue("@write", record.write);
                        updateCommand.Parameters.AddWithValue("@read", record.read);
                        updateCommand.Parameters.AddWithValue("@create", record.create);
                        updateCommand.Parameters.AddWithValue("@delete", record.delete);
                        updateCommand.Parameters.AddWithValue("@id", record.Id);
                        updateCommand.ExecuteNonQuery();
                    }
                }

                // Update message limit for the main user
                int newMessageLimit = int.Parse(MessageLimitTextBox.Text);
                using (var userUpdateCommand = new NpgsqlCommand(@"
        UPDATE ""Users""
        SET messages_left = @newMessageLimit
        WHERE username = 'User';", connection))
                {
                    userUpdateCommand.Parameters.AddWithValue("@newMessageLimit", newMessageLimit);
                    userUpdateCommand.ExecuteNonQuery();
                }

                // Fetch and update individual users
                using (var userQuery = new NpgsqlCommand("SELECT username, messages_left from \"Users\";", connection))
                using (var reader = userQuery.ExecuteReader())
                {
                    var usersToUpdate = new List<(string Username, int CurrentMessagesLeft)>();

                    // First, collect the data we need
                    while (reader.Read())
                    {
                        usersToUpdate.Add((
                            reader.GetString(0),
                            reader.GetInt32(1)
                        ));
                    }

                    // Close the reader before updating
                    reader.Close();

                    // Now update each user
                    foreach (var user in usersToUpdate)
                    {
                        int updatedMessagesLeft;

                        if (user.CurrentMessagesLeft < messageLimitBase)
                        {
                            int difference = messageLimitBase - user.CurrentMessagesLeft;
                            updatedMessagesLeft = newMessageLimit - difference;
                        }
                        else
                        {
                            updatedMessagesLeft = newMessageLimit;
                        }

                        using (var updateUserCommand = new NpgsqlCommand(@"
                UPDATE ""Users"" 
                SET messages_left = @updatedMessagesLeft 
                WHERE username = @userName;", connection))
                        {
                            updateUserCommand.Parameters.AddWithValue("@updatedMessagesLeft", updatedMessagesLeft);
                            updateUserCommand.Parameters.AddWithValue("@userName", user.Username);
                            updateUserCommand.ExecuteNonQuery();
                        }
                    }
                    Logs.InsertLogs(username, "Saved User Permissions Changes", DateTime.Now, MainWindow.GetLocalIpAdress(), MainWindow.GetEmail(username));
                }

                MessageBox.Show("Changes saved successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving changes: {ex.Message}");
                Logs.InsertLogs(username, "Failed User Permissions Changes", DateTime.Now, MainWindow.GetLocalIpAdress(), MainWindow.GetEmail(username));

            }
            finally
            {
                connection?.Close();
            }
            PrincipalMenuAdmin viewAdmid = new PrincipalMenuAdmin(username);
            this.Close();
            viewAdmid.Show();

        }
}

    public class TypeRecord
    {
        public int Id { get; set; }
        public string name { get; set; }
        public bool write { get; set; }
        public bool read { get; set; }
        public bool create { get; set; }
        public bool delete { get; set; }
    }
}
