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
                  
                    string updateQuery = @"
                UPDATE ""Whitelist""
                SET email = @Email
                WHERE id = 1;"; 

                    conexion.EjecutarNonQuery(updateQuery,
                        new NpgsqlParameter("@Email", whitelistEmail));

                    MessageBox.Show("WhishList updated.");
                    WhitelistEmailTextBox.Clear();
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
            try
            {
                if (conexion.AbrirConexion())
                {

                    foreach (var record in Types)
                    {
                        string updateQuery = @"
                            UPDATE ""Types""
                            SET ""write"" = @write, 
                                ""read"" = @read, 
                                ""create"" = @create, 
                                ""delete"" = @delete
                            WHERE id = @id;";

                        conexion.EjecutarNonQuery(updateQuery,
                            new NpgsqlParameter("@write", record.write),
                            new NpgsqlParameter("@read", record.read),
                            new NpgsqlParameter("@create", record.create),
                            new NpgsqlParameter("@delete", record.delete),
                            new NpgsqlParameter("@id", record.Id));
                    }


                    int newMessageLimit = int.Parse(MessageLimitTextBox.Text);
                    string userUpdateQuery = @"
                        UPDATE ""Users""
                        SET messages_left = @newMessageLimit
                        WHERE username = 'User';";

                    conexion.EjecutarNonQuery(userUpdateQuery,
                        new NpgsqlParameter("@newMessageLimit", newMessageLimit));

                    MessageBox.Show("Cambios guardados con éxito.");



                    string userIndividualQuery = "SELECT username, messages_left FROM \"Users\";";
                    using (var reader = conexion.EjecutarConsulta(userIndividualQuery))
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                string userName = reader.GetString(0);
                                int currentMessagesLeft = reader.GetInt32(1);

                                int updatedMessagesLeft;

                                if (currentMessagesLeft < messageLimitBase)
                                {

                                    int diference = messageLimitBase - currentMessagesLeft;
                                    updatedMessagesLeft = newMessageLimit - diference;
                                }
                                else
                                {

                                    updatedMessagesLeft = newMessageLimit;
                                }


                                string updateQuery = "UPDATE FROM \"Users\" SET messages_left = " + updatedMessagesLeft + " WHERE username = '" + userName + "';";

                                conexion.EjecutarNonQuery(updateQuery);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Cannot upload the message limit");
                            }
                        }
                    }
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Error: Invalid values inserted.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error when try to save the changes: {ex.Message}");
            }
            finally
            {
                conexion.CerrarConexion();
            }
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
