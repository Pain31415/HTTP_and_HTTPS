using System;
using System.Net;
using System.Windows;
using HtmlAgilityPack;

namespace HamletApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            // URL тексту твору "Гамлет" на Проекті «Ґутенберґ»
            string url = "https://www.gutenberg.org/files/1524/1524-0.txt";

            using (WebClient client = new WebClient())
            {
                try
                {
                    // Завантаження тексту твору
                    string text = client.DownloadString(url);

                    // Відображення тексту в текстовому полі
                    HamletTextBox.Text = text;
                }
                catch (WebException ex)
                {
                    MessageBox.Show("Помилка при завантаженні тексту: " + ex.Message);
                }
            }
        }
    }
}
