using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using HtmlAgilityPack;

namespace SearchApp
{
    public partial class MainWindow : Window
    {
        private const string BingSearchUrl = "https://www.bing.com/search?q=";

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchTextBox.Text.Trim();

            if (!string.IsNullOrWhiteSpace(query))
            {
                string bingSearchUrl = BingSearchUrl + Uri.EscapeDataString(query);
                string searchResults = await SearchBing(bingSearchUrl);

                SearchResultsTextBox.Text = searchResults;
            }
            else
            {
                MessageBox.Show("Please enter a search query.");
            }
        }

        private async Task<string> SearchBing(string url)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string html = await response.Content.ReadAsStringAsync();
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(html);

                    var resultNodes = doc.DocumentNode.SelectNodes("//li[@class='b_algo']");

                    if (resultNodes != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (var node in resultNodes)
                        {
                            var titleNode = node.SelectSingleNode(".//h2");
                            var snippetNode = node.SelectSingleNode(".//div[@class='b_caption']/p");

                            if (titleNode != null && snippetNode != null)
                            {
                                string title = titleNode.InnerText.Trim();
                                string snippet = snippetNode.InnerText.Trim();

                                sb.AppendLine($"Title: {title}");
                                sb.AppendLine($"Snippet: {snippet}");
                                sb.AppendLine();
                            }
                        }
                        return sb.ToString();
                    }
                    else
                    {
                        return "No results found.";
                    }
                }
                else
                {
                    return $"Error: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
