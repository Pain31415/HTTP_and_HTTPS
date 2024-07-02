using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using HtmlAgilityPack;

namespace GutenbergApp
{
    public partial class MainWindow : Window
    {
        private List<Book> top100Books = new List<Book>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void LoadTop100Button_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://www.gutenberg.org/browse/scores/top";
            WebClient client = new WebClient();
            string html = client.DownloadString(url);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            var bookNodes = doc.DocumentNode.SelectNodes("//ol[1]/li/a");

            top100Books.Clear();
            foreach (var node in bookNodes)
            {
                string title = WebUtility.HtmlDecode(node.InnerText.Trim());
                string bookUrl = "https://www.gutenberg.org" + node.GetAttributeValue("href", string.Empty);
                top100Books.Add(new Book { Title = title, Url = bookUrl });
            }

            BooksListBox.ItemsSource = top100Books;
            BooksListBox.DisplayMemberPath = "Title";
        }

        private void BooksListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BooksListBox.SelectedItem is Book selectedBook)
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = selectedBook.Url,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка при відкритті книги: " + ex.Message);
                }
            }
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchTextBox.Text;
            string url = $"https://www.gutenberg.org/ebooks/search/?query={Uri.EscapeDataString(query)}";
            WebClient client = new WebClient();
            string html = client.DownloadString(url);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            var bookNodes = doc.DocumentNode.SelectNodes("//li[contains(@class, 'booklink')]/a");

            if (bookNodes != null)
            {
                top100Books.Clear();
                foreach (var node in bookNodes)
                {
                    var titleNode = node.SelectSingleNode(".//span[@class='title']");
                    if (titleNode != null)
                    {
                        string title = WebUtility.HtmlDecode(titleNode.InnerText.Trim());
                        string bookUrl = "https://www.gutenberg.org" + node.GetAttributeValue("href", string.Empty);
                        top100Books.Add(new Book { Title = title, Url = bookUrl });
                    }
                }

                BooksListBox.ItemsSource = top100Books;
                BooksListBox.DisplayMemberPath = "Title";
            }
            else
            {
                MessageBox.Show("Нічого не знайдено за вашим запитом.");
            }
        }

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            string author = AuthorTextBox.Text;
            string url = $"https://www.gutenberg.org/ebooks/author/{Uri.EscapeDataString(author)}";
            WebClient client = new WebClient();
            string html = client.DownloadString(url);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            var bookNodes = doc.DocumentNode.SelectNodes("//li[contains(@class, 'booklink')]/a");

            if (bookNodes != null)
            {
                foreach (var node in bookNodes)
                {
                    string bookUrl = "https://www.gutenberg.org" + node.GetAttributeValue("href", string.Empty);
                    string bookHtml = client.DownloadString(bookUrl);

                    HtmlDocument bookDoc = new HtmlDocument();
                    bookDoc.LoadHtml(bookHtml);

                    var textNode = bookDoc.DocumentNode.SelectSingleNode("//a[contains(@href, '.txt')]");
                    if (textNode != null)
                    {
                        string textUrl = "https:" + textNode.GetAttributeValue("href", string.Empty);
                        string bookText = client.DownloadString(textUrl);
                        string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"{node.InnerText}.txt");
                        File.WriteAllText(fileName, bookText);
                    }
                }

                MessageBox.Show("Книги завантажено");
            }
            else
            {
                MessageBox.Show("Не вдалося знайти книги автора.");
            }
        }
    }

    public class Book
    {
        public string Title { get; set; }
        public string Url { get; set; }
    }
}
