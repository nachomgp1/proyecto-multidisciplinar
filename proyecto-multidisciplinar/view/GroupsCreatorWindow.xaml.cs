using Npgsql;
using proyecto_multidisciplinar.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
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
using proyecto_multidisciplinar.view;

namespace proyecto_multidisciplinar
{
    /// <summary>
    /// Lógica de interacción para GroupsCreatorWindow.xaml
    /// </summary>
    public partial class GroupsCreatorWindow : Window
    {
        Conexion conexion = new Conexion();
        private string? adminUser;
        public GroupsCreatorWindow(string? adminUser)
        {
            InitializeComponent();
            this.adminUser = adminUser;
        }
        public void Exit_Click(object sernder, RoutedEventArgs e)
        {
            UserManagementOptions viewAdmid = new UserManagementOptions(adminUser);
            this.Close();
            viewAdmid.Show();
        }
        private void CreateGroupButton_Click(object sender, RoutedEventArgs e)
        {
            string groupName = GroupNameTextBox.Text.Trim(); 

            if (string.IsNullOrEmpty(groupName))
            {
                MessageBox.Show("Please, insert a valid group name.");
                return;
            }

          
            Conexion conexion = new Conexion();

            if (conexion.AbrirConexion())  
            {
               
                string query = "INSERT INTO \"Groups\" (name) VALUES (@name)";

                NpgsqlParameter nameParameter = new NpgsqlParameter("@name", NpgsqlTypes.NpgsqlDbType.Varchar)
                {
                    Value = groupName
                };

               
                conexion.EjecutarNonQuery(query, nameParameter);

               
                conexion.CerrarConexion();

                
                MessageBox.Show("Grupo creado con éxito.");
                sendAcction("Successfully group creation");
                GroupNameTextBox.Clear();
            }
            else
            {
                MessageBox.Show("No se pudo establecer una conexión con la base de datos.");
            }
        }
        //CAMBIAR ESTO A EL QUE ESTA SITUADO EN EL MAINWINDOWS
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
            return "127.0.0.1"; 
        }
        private void sendAcction(string action)
        {
            try
            {
               
                DateTime currentDate = DateTime.Now;

                
                string userIp = GetLocalIPAddress();

                conexion.AbrirConexion();
                string email=null;
                //add the email selecting with a query
                string queryEmail = "SELECT email FROM \"Users\" WHERE username LIKE '"+adminUser+"'; ";
                NpgsqlDataReader reader = conexion.EjecutarConsulta(queryEmail);
                if (reader.Read())
                {
                   email = reader.GetString(0);
                }
                conexion.CerrarConexion();

                conexion.AbrirConexion();

              
                string queryInsert = "INSERT INTO \"Logs\" (username, action, date, user_ip, user_email) " +
                                     "VALUES (@username, @action, @date, @user_ip, @user_email)";

               
                NpgsqlParameter[] parameters = new NpgsqlParameter[] {
            new NpgsqlParameter("@username", adminUser), 
            new NpgsqlParameter("@action", action),
            new NpgsqlParameter("@date", currentDate),
            new NpgsqlParameter("@user_ip", userIp), 
            new NpgsqlParameter("@user_email", email)
        };

                
                conexion.EjecutarNonQuery(queryInsert, parameters);
                conexion.CerrarConexion();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar la acción: " + ex.Message);
                conexion.CerrarConexion();
            }
        }
    }
}
