using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace SGToolWindow
{
    /// <summary>
    /// Interaction logic for ToolWindow1Control.
    /// </summary>
    public partial class ToolWindow1Control : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolWindow1Control"/> class.
        /// </summary>
        public ToolWindow1Control()
        {
            this.InitializeComponent();
            AddMessage("SGCopilot", "Welcome to Our Company! Here are some important links: \n" +
                "https://en.wikipedia.org/wiki/Soci%C3%A9t%C3%A9_G%C3%A9n%C3%A9rale \n" +
                "https://www.societegenerale.com/en", false);

        }

        private async void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            string userMessage = UserInput.Text;

            AddMessage("User", userMessage, true);

            LoadingIndicator.Visibility = Visibility.Visible;

            // Call your API to get the chatbot response
            string chatbotResponse = await GetChatbotResponse(userMessage);

            LoadingIndicator.Visibility = Visibility.Collapsed;

            // Display the chatbot response in the ChatHistory TextBlock
            AddMessage("Chatbot", chatbotResponse, false);

            //Placeholder chatbot response for test
            //string chatbotResponse = "Placeholder GPT response";
            //AddMessage("Chatbot", chatbotResponse);

            // Clear the user input
            UserInput.Clear();
        }

        private void AddMessage(string sender, string message, bool alignRight)
        {
            // Create a new Border to act as the chat bubble.
            Border chatBubble = new Border();
            chatBubble.Background = (sender == "User") ? Brushes.LightBlue : Brushes.LightGray;
            chatBubble.BorderBrush = Brushes.Gray;
            chatBubble.BorderThickness = new Thickness(1);
            chatBubble.CornerRadius = new CornerRadius(10);
            chatBubble.Padding = new Thickness(8);
            chatBubble.Margin = new Thickness(5, 5, 5, 0); // Add margin for spacing.

            TextBox messageBlock = new TextBox();
            messageBlock.Text = message;
            messageBlock.IsReadOnly = true;
            messageBlock.TextWrapping = TextWrapping.Wrap;
            messageBlock.Background = Brushes.Transparent;
            messageBlock.BorderThickness = new Thickness(0);

            chatBubble.Child = messageBlock;

            if (alignRight)
            {
                chatBubble.HorizontalAlignment = HorizontalAlignment.Right;
            }
            else
            {
                chatBubble.HorizontalAlignment = HorizontalAlignment.Left;
            }


            // Add the TextBlock to the chat history.
            ChatHistory.Children.Add(chatBubble);

            // Scroll to the bottom of the chat history to show the latest message.
            ChatHistoryScrollViewer.ScrollToBottom();
        }

        private readonly string apiUrl = "http://127.0.0.1:5000/chatbot"; // Update with your API URL

        public async Task<string> GetChatbotResponse(string userInput)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // Create the request payload
                    var requestData = new
                    {
                        input_text = userInput
                    };

                    var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    // Send the HTTP POST request to the Flask API
                    var response = await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        dynamic responseData = Newtonsoft.Json.JsonConvert.DeserializeObject(responseContent);
                        string chatbotResponse = responseData.response;

                        return chatbotResponse;
                    }
                    else
                    {
                        return $"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle API call errors
                return $"Error: {ex.Message}";
            }
        }

        private void UserInput_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}