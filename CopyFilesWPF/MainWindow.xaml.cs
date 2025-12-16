using CopyFilesWPF.Presenter;
using CopyFilesWPF.View;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace CopyFilesWPF
{
    public partial class MainWindow : Window, IMainWindowView
    {
        private readonly IMainWindowPresenter _mainWindowPresenter;
        public MainWindow MainWindowView => this;

        public MainWindow()
        {
            InitializeComponent();
            _mainWindowPresenter = new MainWindowPresenter(this);
        }

        private void FromButton_Click(object sender, RoutedEventArgs e)
        {
            FromTextBox.Text = OpenFile();
            _mainWindowPresenter.ChooseFileFromButtonClick(FromTextBox.Text);
        }

        private void ToButton_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new FolderBrowserDialog(); // for this type - please turn on WinForms in project (in properties)
            DialogResult result = dialog.ShowDialog();
            ToTextBox.Text = dialog.SelectedPath;
            _mainWindowPresenter.ChooseFileToButtonClick(ToTextBox.Text);
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindowPresenter.CopyButtonClick();
        }

        private void FromTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CopyButton.IsEnabled = CheckFromAndToPaths();
        }

        private void ToTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CopyButton.IsEnabled = CheckFromAndToPaths();
        }

        private bool CheckFromAndToPaths()
        {
            return ToTextBox.Text.Length > 0 && FromTextBox.Text.Length > 0;
        }

        private static string OpenFile()
        {
            var openFile = new OpenFileDialog
            {
                Multiselect = false
            };
            openFile.ShowDialog();

            return openFile.FileName;
        }

        public void AddFilePanel(Grid filePanel)
        {
            MainWindowView.MainPanel.Children.Add(filePanel);
        }

        public void ClearPaths()
        {
            MainWindowView.FromTextBox.Text = "";
            MainWindowView.ToTextBox.Text = "";
        }

        public Grid CreateFilePanel(string filePath, RoutedEventHandler pauseHandler, RoutedEventHandler cancelHandler)
        {
            const double panelHeight = 60;
            const double fileNameColumnWidth = 320;

            var panel = new Grid { Height = panelHeight };

            // Columns
            panel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(fileNameColumnWidth) });
            panel.ColumnDefinitions.Add(new ColumnDefinition());
            panel.ColumnDefinitions.Add(new ColumnDefinition());

            // Rows
            panel.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            panel.RowDefinitions.Add(new RowDefinition());

            // File name
            var fileNameText = new TextBlock
            {
                Text = Path.GetFileName(filePath),
                Margin = new Thickness(5, 0, 5, 0)
            };
            Grid.SetRow(fileNameText, 0);
            Grid.SetColumn(fileNameText, 0);
            panel.Children.Add(fileNameText);

            // Progress bar
            var progressBar = new System.Windows.Controls.ProgressBar { Margin = new Thickness(10) };
            Grid.SetRow(progressBar, 1);
            Grid.SetColumn(progressBar, 0);
            Grid.SetColumnSpan(progressBar, 3);
            panel.Children.Add(progressBar);

            // Pause button
            var pauseButton = new System.Windows.Controls.Button
            {
                Content = "Pause",
                Margin = new Thickness(5)
            };
            pauseButton.Click += pauseHandler;
            Grid.SetRow(pauseButton, 1);
            Grid.SetColumn(pauseButton, 1);
            panel.Children.Add(pauseButton);

            // Cancel button
            var cancelButton = new System.Windows.Controls.Button
            {
                Content = "Cancel",
                Margin = new Thickness(5)
            };
            cancelButton.Click += cancelHandler;
            Grid.SetRow(cancelButton, 1);
            Grid.SetColumn(cancelButton, 2);
            panel.Children.Add(cancelButton);

            return panel;
        }
    }
}
