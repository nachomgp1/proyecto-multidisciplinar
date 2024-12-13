using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using MimeKit;
using MimeKit.Utils;
using Npgsql;
using proyecto_multidisciplinar.model;
using static System.Net.Mime.MediaTypeNames;

namespace proyecto_multidisciplinar.view
{
    public partial class EnviarCorreoWindow : Window
    {
        private GmailService gmailService;
        private ObservableCollection<model.ArchivoAdjunto> archivosAdjuntos;

        public EnviarCorreoWindow(GmailService service)
        {
            gmailService = service;
            InitializeComponent();

            // Inicializar la colección de archivos adjuntos
            archivosAdjuntos = new ObservableCollection<model.ArchivoAdjunto>();
            lstArchivosAdj.ItemsSource = archivosAdjuntos;
        }



        private void Adjuntar_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Title = "Seleccionar archivos para adjuntar"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string fileName in openFileDialog.FileNames)
                {
                    AgregarArchivo(fileName);
                }
            }
        }

        private void AgregarArchivo(string rutaArchivo)
        {
            // Verificar tamaño máximo individual y total
            FileInfo fileInfo = new FileInfo(rutaArchivo);
            double tamañoArchivoMB = fileInfo.Length / (1024.0 * 1024.0);
            double tamañoTotalMB = archivosAdjuntos.Sum(a => a.SizeInMB) + tamañoArchivoMB;

            if (tamañoArchivoMB > 10)
            {
                MessageBox.Show($"El archivo {Path.GetFileName(rutaArchivo)} supera el límite de 10MB.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (tamañoTotalMB > 10)
            {
                MessageBox.Show("No se pueden adjuntar más archivos. Se superaría el límite total de 10MB.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Agregar archivo a la lista
            archivosAdjuntos.Add(new model.ArchivoAdjunto
            {
                Name = Path.GetFileName(rutaArchivo),
                Path = rutaArchivo,
                SizeInMB = tamañoArchivoMB
            });
        }

        private void EliminarArchivo_Click(object sender, RoutedEventArgs e)
        {
            var archivoAEliminar = (sender as FrameworkElement)?.DataContext as model.ArchivoAdjunto;
            if (archivoAEliminar != null)
            {
                archivosAdjuntos.Remove(archivoAEliminar);
            }
        }
        public static bool CheckWhilelist(String txtDestinatario)
        {
            Conexion conexion = new Conexion();
            bool resultado = false;
            if (conexion.AbrirConexion())
            {
                string query = "SELECT email FROM \"Whitelist\"";

                NpgsqlDataReader reader = conexion.EjecutarConsulta(query);

                while (reader.Read())
                {
                    if (reader.GetString(0) == txtDestinatario) {
                        resultado = true;
                    }

                }
                conexion.CerrarConexion();
            }
            else
            {
                MessageBox.Show("Error with whitelist");
            }
            return resultado;
        }
        private bool CheckHavemsgleft()
        {
            Conexion conexion = new Conexion();
            bool resultado = false;

            if (conexion.AbrirConexion())
            {
                try
                { 
                    string email = "current_user_email@example.com"; 

                    string query = "SELECT messages_left FROM \"Users\" WHERE email = @email";

                  
                    NpgsqlParameter paramUser = new NpgsqlParameter("@gmail", email);

                    NpgsqlDataReader reader = conexion.EjecutarConsulta(query, paramUser);

                    if (reader != null)
                    {
                            if (reader.GetInt32(5) >0)
                            {
                            string queryUpdate = "UPDATE \"Users\" SET messages_left = messages_left - 1 WHERE email = @email";
                            conexion.EjecutarNonQuery(queryUpdate, paramUser);

                             
                            

                            resultado = true;
                        }
                        
 
                        }
                    }
                
                catch (Exception ex)
                {
                    MessageBox.Show($"Error checking messages left: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    conexion.CerrarConexion();
                }
            }
            else
            {
                MessageBox.Show("Error connecting to database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return resultado;
        }
        private async void EnviarCorreo_Click(object sender, RoutedEventArgs e)
        {
            if (CheckWhilelist(txtDestinatario.Text))
            {
                if (CheckHavemsgleft())
                {
                    try
                    {
                        // Validar campos
                        if (string.IsNullOrWhiteSpace(txtDestinatario.Text) ||
                            string.IsNullOrWhiteSpace(txtAsunto.Text) ||
                            string.IsNullOrWhiteSpace(txtMensaje.Text))
                        {
                            MessageBox.Show("Por favor, complete todos los campos.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        // Crear mensaje MIME
                        var mimeMessage = CrearMensajeCorreo(txtDestinatario.Text, txtAsunto.Text, txtMensaje.Text);

                        // Agregar archivos adjuntos
                        if (archivosAdjuntos.Any())
                        {
                            var multipart = new MimeKit.Multipart("mixed");
                            var textPart = new TextPart("plain")
                            {
                                Text = txtMensaje.Text
                            };
                            multipart.Add(textPart);

                            foreach (var archivo in archivosAdjuntos)
                            {
                                var attachment = new MimePart(MimeTypes.GetMimeType(archivo.Path))
                                {
                                    Content = new MimeContent(File.OpenRead(archivo.Path)),
                                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                                    ContentTransferEncoding = ContentEncoding.Base64,
                                    FileName = Path.GetFileName(archivo.Path)
                                };
                                multipart.Add(attachment);
                            }

                            mimeMessage.Body = multipart;
                        }

                        // Convertir a formato base64 URL
                        var mensaje = new Message
                        {
                            Raw = Base64UrlEncode(mimeMessage.ToString())
                        };

                        // Enviar correo
                        await gmailService.Users.Messages.Send(mensaje, "me").ExecuteAsync();

                        MessageBox.Show("Correo enviado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al enviar correo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }else {
                MessageBox.Show("No te quedan mensajes diarios", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
             }else{
                MessageBox.Show("Destinatario no permitido", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
             }

        }
        

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private MimeMessage CrearMensajeCorreo(string para, string asunto, string cuerpo)
        {
            var mensaje = new MimeMessage();
            mensaje.From.Add(new MailboxAddress("", "me"));
            mensaje.To.Add(new MailboxAddress("", para));
            mensaje.Subject = asunto;

            return mensaje;
        }

        private string Base64UrlEncode(string input)
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(inputBytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");
        }
    }

   
}

