using System;

using System.Windows;
using System.Windows.Controls;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


namespace proyecto_multidisciplinar.view
{
    public partial class EmailDetailsWindow : Window
    {
        private GmailService gmailService;
        private string messageId;

        public EmailDetailsWindow(GmailService service, string messageId)
        {
            InitializeComponent();
            this.gmailService = service;
            this.messageId = messageId;
            LoadEmailDetails();
        }

        private async void LoadEmailDetails()
        {
            try
            {
                // Show loading indicator
                loadingIndicator.Visibility = Visibility.Visible;
                emailDetailsPanel.Visibility = Visibility.Collapsed;

                // Fetch the full message details
                var message = await gmailService.Users.Messages.Get("me", messageId).ExecuteAsync();

                // Extract headers
                var headers = message.Payload.Headers;
                string subject = headers.FirstOrDefault(h => h.Name == "Subject")?.Value ?? "Sin asunto";
                string from = headers.FirstOrDefault(h => h.Name == "From")?.Value ?? "Desconocido";
                string to = headers.FirstOrDefault(h => h.Name == "To")?.Value ?? "Desconocido";
                string date = headers.FirstOrDefault(h => h.Name == "Date")?.Value ?? "Fecha desconocida";

                // Parse the body
                string body = GetEmailBody(message);

                // Update UI
                Dispatcher.Invoke(() =>
                {
                    subjectTextBlock.Text = subject;
                    fromTextBlock.Text = $"From: {from}";
                    toTextBlock.Text = $"To: {to}";
                    dateTextBlock.Text = $"Date: {date}";
                    bodyTextBox.Text = body;

                    // Load attachments
                    LoadAttachments(message);

                    // Hide loading indicator
                    loadingIndicator.Visibility = Visibility.Collapsed;
                    emailDetailsPanel.Visibility = Visibility.Visible;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading email details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                Dispatcher.Invoke(() =>
                {
                    loadingIndicator.Visibility = Visibility.Collapsed;
                });
            }
        }

        private string GetEmailBody(Message message)
        {
            // Handle different payload types
            if (message.Payload.Body != null && !string.IsNullOrEmpty(message.Payload.Body.Data))
            {
                return DecodeBase64String(message.Payload.Body.Data);
            }

            // Check for multipart message
            if (message.Payload.Parts != null)
            {
                foreach (var part in message.Payload.Parts)
                {
                    if (part.MimeType == "text/plain" || part.MimeType == "text/html")
                    {
                        return DecodeBase64String(part.Body.Data);
                    }
                }
            }

            return "No body content";
        }

        private void LoadAttachments(Message message)
        {
            // Clear previous attachments
            attachmentsStackPanel.Children.Clear();

            if (message.Payload.Parts != null)
            {
                foreach (var part in message.Payload.Parts)
                {
                    if (part.Filename != null)
                    {
                        // Create a button for each attachment
                        Button attachmentButton = new Button
                        {
                            Content = part.Filename,
                            Margin = new Thickness(5),
                            Tag = part
                        };
                        attachmentButton.Click += AttachmentButton_Click;
                        attachmentsStackPanel.Children.Add(attachmentButton);
                    }
                }

                // Show/hide attachments section
                attachmentsSection.Visibility = attachmentsStackPanel.Children.Count > 0
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
            else
            {
                attachmentsSection.Visibility = Visibility.Collapsed;
            }
        }

        private async void AttachmentButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var part = button.Tag as MessagePart;

                if (part == null || string.IsNullOrEmpty(part.Body.AttachmentId))
                {
                    MessageBox.Show("Cannot download attachment.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Get the attachment
                var attachmentResponse = await gmailService.Users.Messages.Attachments.Get("me", messageId, part.Body.AttachmentId).ExecuteAsync();

                // Decode the attachment data
                byte[] attachmentData = Convert.FromBase64String(attachmentResponse.Data.Replace('-', '+').Replace('_', '/'));

                // Prompt user to save the file
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = part.Filename,
                    Filter = "All files (*.*)|*.*"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    File.WriteAllBytes(saveFileDialog.FileName, attachmentData);
                    MessageBox.Show("Attachment saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error downloading attachment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string DecodeBase64String(string base64String)
        {
            try
            {
                // Validar si la entrada es nula o vacía
                if (string.IsNullOrEmpty(base64String))
                {
                    return "Error:La cadena es imposible de leer o esta vacia.";
                }

           
                base64String = base64String.Replace('-', '+').Replace('_', '/');

             
                while (base64String.Length % 4 != 0)
                {
                    base64String += '=';
                }

             
                if (!IsBase64String(base64String))
                {
                    return "La cadena no está codificada en Base64.";
                }

                byte[] data = Convert.FromBase64String(base64String);
                return Encoding.UTF8.GetString(data);
            }
            catch (Exception ex)
            {
                return $"Error al decodificar la cadena : {ex.Message}";
            }
        }

      
        private bool IsBase64String(string base64String)
        {
            base64String = base64String.Trim();
            return base64String.Length % 4 == 0 && Regex.IsMatch(base64String, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
