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
        private Dictionary<string, bool> emailReadStatus = new Dictionary<string, bool>();
        private Stack<string> pageTokenHistory = new Stack<string>(); 
        private string currentPageToken;  
        private string nextPageToken;
        private const int PAGE_SIZE = 25;

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
        private async void CargarCorreos(string pageToken = null, bool isNextPage = false)
        {
            await CargarCorreosAsync(pageToken, isNextPage);
        }
        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(nextPageToken))
            {
                _ = CargarCorreosAsync(nextPageToken, true);
            }
        }

        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (pageTokenHistory.Count > 0)
            {
                // Tomar el token previo y cargar los correos
                string previousToken = pageTokenHistory.Pop();
                _ = CargarCorreosAsync(previousToken, false);
            }
            else
            {
                // Si no hay tokens previos, simplemente recargamos la primera página
                _ = CargarCorreosAsync(null, false);
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

            emailReadStatus[selectedEmail.MessageId] = true;
            selectedEmail.IsRead = true;

            var itemContainer = emailListView.ItemContainerGenerator.ContainerFromItem(selectedEmail) as ListViewItem;
            if (itemContainer != null)
            {
                var noLeidoButton = FindNoLeidoButtonInRow(itemContainer);
                if (noLeidoButton != null)
                {
                    noLeidoButton.Content = "Leído";
                }
            }

            var emailDetailsWindow = new EmailDetailsWindow(gmailService, selectedEmail.MessageId);
            emailDetailsWindow.Show();
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



        private async Task CargarCorreosAsync(string pageToken = null, bool isNextPage = false)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    emailListView.Visibility = Visibility.Collapsed;
                    loadingImage.Visibility = Visibility.Visible;
                    nextButton.IsEnabled = false;
                    previousButton.IsEnabled = false;
                });

                var request = gmailService.Users.Messages.List("me");
                request.MaxResults = PAGE_SIZE;
                if (pageToken != null)
                {
                    request.PageToken = pageToken;
                }

                var response = await request.ExecuteAsync();

                // Manejar el historial de navegación
                if (isNextPage)
                {
                    if (currentPageToken != null)
                    {
                        pageTokenHistory.Push(currentPageToken);
                    }
                }
                currentPageToken = pageToken;
                nextPageToken = response.NextPageToken;

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

                        bool isRead = emailReadStatus.ContainsKey(message.Id) && emailReadStatus[message.Id];

                        emailList.Add(new EmailItem
                        {
                            MessageId = message.Id,
                            From = from,
                            Hour = dateTime,
                            Subject = subject,
                            IsRead = isRead
                        });
                    }
                }

                Dispatcher.Invoke(() =>
                {
                    emailListView.ItemsSource = emailList;
                    nextButton.IsEnabled = !string.IsNullOrEmpty(nextPageToken);
                    previousButton.IsEnabled = true;

                    foreach (EmailItem item in emailList)
                    {
                        var container = emailListView.ItemContainerGenerator.ContainerFromItem(item) as ListViewItem;
                        if (container != null)
                        {
                            var button = FindNoLeidoButtonInRow(container);
                            if (button != null)
                            {
                                button.Content = item.IsRead ? "Leído" : "No Leído";
                            }
                        }
                    }

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
            var isCurrentlyRead = button.Content.ToString() == "Leído";

            // Update the read status in our dictionary
            emailReadStatus[messageId] = !isCurrentlyRead;

            // Update button text
            button.Content = isCurrentlyRead ? "No Leído" : "Leído";
        }
    }
}