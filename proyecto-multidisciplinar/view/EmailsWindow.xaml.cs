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
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;


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
               
                emailListView.Visibility = Visibility.Collapsed;
                loadingImage.Visibility = Visibility.Visible;

              
                var request = gmailService.Users.Messages.List("me");
                request.MaxResults = 50; 
                var response = await request.ExecuteAsync();

                if (response.Messages != null)
                {
                    var emailList = new List<EmailItem>();
                    foreach (var message in response.Messages)
                    {
                        var emailRequest = gmailService.Users.Messages.Get("me", message.Id);
                        var email = await emailRequest.ExecuteAsync();

                        
                        var headers = email.Payload.Headers;
                        string subject = headers.FirstOrDefault(h => h.Name == "Subject")?.Value ?? "Sin asunto";
                        string from = headers.FirstOrDefault(h => h.Name == "From")?.Value ?? "Desconocido";
                        string dateHeader = headers.FirstOrDefault(h => h.Name == "Date")?.Value;

                        string dateTime = "Fecha desconocida";
                        if (DateTime.TryParse(dateHeader, out DateTime date))
                        {
                            dateTime = date.ToString("yyyy-MM-dd HH:mm"); 
                        }

                        emailList.Add(new EmailItem
                        {
                            MessageId = message.Id,
                            From = from,
                            Hour = dateTime,
                            Subject = subject,
                            IsRead = false
                        });
                    }

                    
                    emailListView.ItemsSource = emailList;
                    loadingImage.Visibility = Visibility.Collapsed;
                    emailListView.Visibility = Visibility.Visible;
                }
                else
                {
                   
                    loadingImage.Visibility = Visibility.Collapsed;
                    emailListView.Visibility = Visibility.Visible;
                    MessageBox.Show("No se encontraron correos.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
            
                loadingImage.Visibility = Visibility.Collapsed;
                emailListView.Visibility = Visibility.Visible;

                MessageBox.Show($"Error al cargar correos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void EliminarCorreo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
            
                var button = sender as Button;
                if (button == null) return;

                string messageId = button.CommandParameter as string;
                if (string.IsNullOrEmpty(messageId)) return;

             
                var result = MessageBox.Show("¿Está seguro que desea eliminar este correo?",
                    "Confirmar Eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                  
                    await gmailService.Users.Messages.Delete("me", messageId).ExecuteAsync();

                
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
          
            EnviarCorreoWindow enviarCorreoWindow = new EnviarCorreoWindow(gmailService, this.username);
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
       
            var selectedEmail = emailListView.SelectedItem as EmailItem;
            if (selectedEmail == null) return;

         
            selectedEmail.IsRead = true;
            emailListView.Items.Refresh();

           
            var emailDetailsWindow = new EmailDetailsWindow(gmailService, selectedEmail.MessageId);
            emailDetailsWindow.Show();

           
            var itemContainer = emailListView.ItemContainerGenerator.ContainerFromItem(selectedEmail) as ListViewItem;
            if (itemContainer != null)
            {
              
                var noLeidoButton = FindNoLeidoButtonInRow(itemContainer);
                if (noLeidoButton != null)
                {
                        noLeidoButton.Content = "Leído";
                }
            }
        }

     
        private Button FindNoLeidoButtonInRow(ListViewItem itemContainer)
        {
          
            var stackPanel = FindVisualChild<StackPanel>(itemContainer);
            if (stackPanel != null)
            {
                return stackPanel.Children.OfType<Button>().FirstOrDefault(b => b.Name == "NoLeidoButton");
            }
            return null;
        }


     
        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
{
    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
    {
        var child = VisualTreeHelper.GetChild(parent, i);
        if (child is T)
        {
            return (T)child;
        }

        var result = FindVisualChild<T>(child);
        if (result != null)
        {
            return result;
        }
    }
    return null;
}



        private async Task CargarCorreosAsync()
        {
            try
            {
            
                Dispatcher.Invoke(() =>
                {
                    emailListView.Visibility = Visibility.Collapsed;
                    loadingImage.Visibility = Visibility.Visible;
                });

             
                var request = gmailService.Users.Messages.List("me");
                request.MaxResults = 50;
                var response = await request.ExecuteAsync();

                var emailList = new List<EmailItem>();

                if (response.Messages != null)
                {
                    foreach (var message in response.Messages)
                    {
                        var emailRequest = gmailService.Users.Messages.Get("me", message.Id);
                        var email = await emailRequest.ExecuteAsync();

                       
                        var headers = email.Payload.Headers;
                        string subject = headers.FirstOrDefault(h => h.Name == "Subject")?.Value ?? "Sin asunto";
                        string from = headers.FirstOrDefault(h => h.Name == "From")?.Value ?? "Desconocido";
                        string dateHeader = headers.FirstOrDefault(h => h.Name == "Date")?.Value;

                        string dateTime = "Fecha desconocida";
                        if (DateTime.TryParse(dateHeader, out DateTime date))
                        {
                            dateTime = date.ToString("yyyy-MM-dd HH:mm"); 
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

                
                Dispatcher.Invoke(() =>
                {
                    emailListView.ItemsSource = emailList;

                
                    loadingImage.Visibility = Visibility.Collapsed;
                    emailListView.Visibility = Visibility.Visible;
                });
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                  
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
              
                await credential.RevokeTokenAsync(CancellationToken.None);

             
                string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".credentials/gmail-wpf-quickstart");
                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath, true);
                }
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
        private void MarcarNoLeido_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null)
                return;

            var messageId = button.CommandParameter.ToString();
            MessageBox.Show($"Correo {messageId} marcado como no leído");
      
            if (button.Content.ToString() == "Leído")
            {
                button.Content = "No Leído";
            }
            else
            {
                button.Content = "Leído";
            }
        }


    }
}