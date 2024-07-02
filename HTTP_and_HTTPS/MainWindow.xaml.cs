using System.Net.Http;
using System.Windows;
using System.Windows.Controls;

namespace ImageSearchApp
{
    public partial class MainWindow : Window
    {
        private const string BingImageSearchUrl = "https://www.bing.com/images/search?q=";
        private const string GoogleImageSearchUrl = "https://www.google.com/search?q=";

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchTextBox.Text.Trim();
            string selectedEngine = (SearchEnginesComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (!string.IsNullOrWhiteSpace(query) && !string.IsNullOrWhiteSpace(selectedEngine))
            {
                List<string> imageUrls = new List<string>();

                if (selectedEngine == "Bing")
                {
                    string bingImageSearchUrl = BingImageSearchUrl + Uri.EscapeDataString(query);
                    imageUrls.AddRange(await SearchImages(bingImageSearchUrl));
                }
                else if (selectedEngine == "Google")
                {
                    string googleImageSearchUrl = GoogleImageSearchUrl + Uri.EscapeDataString(query);
                    imageUrls.AddRange(await SearchImages(googleImageSearchUrl, true));
                }

                ImageResultsListBox.Items.Clear();
                foreach (var imageUrl in imageUrls)
                {
                    ImageResultsListBox.Items.Add(imageUrl);
                }
            }
            else
            {
                MessageBox.Show("Please enter a search query and select a search engine.");
            }
        }

        private async Task<List<string>> SearchImages(string url, bool isGoogle = false)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string html = await response.Content.ReadAsStringAsync();
                    List<string> imageUrls = ParseImageUrls(html, isGoogle);
                    return imageUrls;
                }
                else
                {
                    MessageBox.Show($"Error: {response.StatusCode}");
                    return new List<string>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return new List<string>();
            }
        }

        private List<string> ParseImageUrls(string html, bool isGoogle)
        {
            List<string> imageUrls = new List<string>();

            if (isGoogle)
            {
                imageUrls.Add("https://example.com/image1.jpg");
                imageUrls.Add("https://example.com/image2.jpg");
                imageUrls.Add("https://example.com/image3.jpg");
            }
            else
            {
                imageUrls.Add("https://example.com/image4.jpg");
                imageUrls.Add("https://example.com/image5.jpg");
                imageUrls.Add("https://example.com/image6.jpg");
            }

            return imageUrls;
        }
    }
}
