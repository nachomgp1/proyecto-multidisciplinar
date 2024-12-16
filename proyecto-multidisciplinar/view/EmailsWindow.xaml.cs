using Google.Apis.Gmail.v1;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows;
using System;
using MAIL;
using XamlAnimatedGif;
using System.Linq;
using Google.Apis.Auth.OAuth2;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Media.Imaging;


namespace proyecto_multidisciplinar.view
{

    public partial class EmailsWindow : Window
    {
        private GmailService gmailService;
        private UserCredential credential;
        private Window ventanaAnterior;
        private string? username;

        public EmailsWindow(GmailService service, UserCredential userCredential, Window ventanaAnterior, string? username)
        {
            this.username = username;
            gmailService = service;
            credential = userCredential;
            this.ventanaAnterior = ventanaAnterior;
            InitializeComponent();
            emailListView.MouseDoubleClick += EmailListView_MouseDoubleClick;
            CargarCorreos();
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (ventanaAnterior != null)
            {
                ventanaAnterior.Show();
            }
            this.Close();
        }
        private async void CargarCorreos()
        {
            try
            {
                // Show loading gif and hide ListView
                emailListView.Visibility = Visibility.Collapsed;
                loadingImage.Visibility = Visibility.Visible;

                // Obtener lista de correos
                var request = gmailService.Users.Messages.List("me");
                request.MaxResults = 50; // Limitar a 50 correos
                var response = await request.ExecuteAsync();

                if (response.Messages != null)
                {
                    var emailList = new List<EmailItem>();
                    foreach (var message in response.Messages)
                    {
                        var emailRequest = gmailService.Users.Messages.Get("me", message.Id);
                        var email = await emailRequest.ExecuteAsync();

                        // Obtener asunto, remitente y fecha
                        var headers = email.Payload.Headers;
                        string subject = headers.FirstOrDefault(h => h.Name == "Subject")?.Value ?? "Sin asunto";
                        string from = headers.FirstOrDefault(h => h.Name == "From")?.Value ?? "Desconocido";
                        string dateHeader = headers.FirstOrDefault(h => h.Name == "Date")?.Value;

                        string dateTime = "Fecha desconocida";
                        if (DateTime.TryParse(dateHeader, out DateTime date))
                        {
                            dateTime = date.ToString("yyyy-MM-dd HH:mm"); // Fecha completa y hora
                        }

                        emailList.Add(new EmailItem
                        {
                            MessageId = message.Id,
                            From = from,
                            Hour = dateTime, // Ahora incluye fecha y hora
                            Subject = subject
                        });
                    }

                    // Hide loading gif and show ListView
                    emailListView.ItemsSource = emailList;
                    loadingImage.Visibility = Visibility.Collapsed;
                    emailListView.Visibility = Visibility.Visible;
                }
                else
                {
                    // No messages found
                    loadingImage.Visibility = Visibility.Collapsed;
                    emailListView.Visibility = Visibility.Visible;
                    MessageBox.Show("No se encontraron correos.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                // Ensure loading gif is hidden and ListView is visible in case of error
                loadingImage.Visibility = Visibility.Collapsed;
                emailListView.Visibility = Visibility.Visible;

                MessageBox.Show($"Error al cargar correos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void EliminarCorreo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Obtener el ID del mensaje del botón
                var button = sender as Button;
                if (button == null) return;

                string messageId = button.CommandParameter as string;
                if (string.IsNullOrEmpty(messageId)) return;

                // Confirmar eliminación
                var result = MessageBox.Show("¿Está seguro que desea eliminar este correo?",
                    "Confirmar Eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Eliminar el correo
                    await gmailService.Users.Messages.Delete("me", messageId).ExecuteAsync();

                    // Recargar la lista de correos
                    await CargarCorreosAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar correo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EnviarCorreo_Click(object sender, RoutedEventArgs e)
        {
            // Abrir ventana de envío de correo
            EnviarCorreoWindow enviarCorreoWindow = new EnviarCorreoWindow(gmailService,this.username);
            enviarCorreoWindow.Show();
        }
        private async void ActualizarCorreos_Click(object sender, RoutedEventArgs e)
        {
            await CargarCorreosAsync();
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            if (ventanaAnterior != null)
            {
                ventanaAnterior.Show();
            }
        }
        private void EmailListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Get the selected email item
            var selectedEmail = emailListView.SelectedItem as EmailItem;
            if (selectedEmail == null) return;

            // Open email details window
            var emailDetailsWindow = new EmailDetailsWindow(gmailService, selectedEmail.MessageId);
            emailDetailsWindow.Show();
        }

        
        private async Task CargarCorreosAsync()
        {
            try
            {
                // Show loading gif and hide ListView
                Dispatcher.Invoke(() =>
                {
                    emailListView.Visibility = Visibility.Collapsed;
                    loadingImage.Visibility = Visibility.Visible;
                });

                // Obtener lista de correos
                var request = gmailService.Users.Messages.List("me");
                request.MaxResults = 50; // Limitar a 50 correos
                var response = await request.ExecuteAsync();

                var emailList = new List<EmailItem>();

                if (response.Messages != null)
                {
                    foreach (var message in response.Messages)
                    {
                        var emailRequest = gmailService.Users.Messages.Get("me", message.Id);
                        var email = await emailRequest.ExecuteAsync();

                        // Obtener asunto, remitente y fecha
                        var headers = email.Payload.Headers;
                        string subject = headers.FirstOrDefault(h => h.Name == "Subject")?.Value ?? "Sin asunto";
                        string from = headers.FirstOrDefault(h => h.Name == "From")?.Value ?? "Desconocido";
                        string dateHeader = headers.FirstOrDefault(h => h.Name == "Date")?.Value;

                        string dateTime = "Fecha desconocida";
                        if (DateTime.TryParse(dateHeader, out DateTime date))
                        {
                            dateTime = date.ToString("yyyy-MM-dd HH:mm"); // Fecha completa y hora
                        }

                        emailList.Add(new EmailItem
                        {
                            MessageId = message.Id,
                            From = from,
                            Hour = dateTime,
                            Subject = subject
                        });
                    }
                }

                // Update UI on main thread
                Dispatcher.Invoke(() =>
                {
                    emailListView.ItemsSource = emailList;

                    // Hide loading gif and show ListView
                    loadingImage.Visibility = Visibility.Collapsed;
                    emailListView.Visibility = Visibility.Visible;
                });
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    // Hide loading gif in case of error
                    loadingImage.Visibility = Visibility.Collapsed;
                    emailListView.Visibility = Visibility.Visible;

                    MessageBox.Show($"Error al actualizar correos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        private async void CerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Revocar tokens de autorización
                await credential.RevokeTokenAsync(CancellationToken.None);

                // Eliminar credenciales almacenadas
                string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".credentials/gmail-wpf-quickstart");
                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath, true);
                }
                
                // Cerrar ventana actual
                this.Close();
                if (ventanaAnterior != null)
                {
                    ventanaAnterior.Show();
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cerrar sesión: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}