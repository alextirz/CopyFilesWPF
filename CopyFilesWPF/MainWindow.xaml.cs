using CopyFilesWPF.Presenter;
using CopyFilesWPF.View;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using ProgressBar = System.Windows.Controls.ProgressBar;
using Button = System.Windows.Controls.Button;

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
        public Grid CreateProgressPanel(int height, int gridWidth, int gridHeight)
        {
            MainWindowView.Height = MainWindowView.Height + height;
            var newPanel = new Grid();
            newPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(gridWidth) });
            newPanel.ColumnDefinitions.Add(new ColumnDefinition());
            newPanel.ColumnDefinitions.Add(new ColumnDefinition());
            newPanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(gridHeight) });
            newPanel.RowDefinitions.Add(new RowDefinition());
            newPanel.Height = 60;
            DockPanel.SetDock(newPanel, Dock.Top);
            return newPanel;
        }

        public TextBlock CreateTextBlock(string filePath, Grid panel)
        {
            var fileNameTextBlock = new TextBlock
            {
                Text = Path.GetFileName(filePath),
                Margin = new Thickness(5, 0, 5, 0)
            };
            Grid.SetRow(fileNameTextBlock, 0);
            Grid.SetColumn(fileNameTextBlock, 0);
            panel.Children.Add(fileNameTextBlock);
            return fileNameTextBlock;
        }

        public ProgressBar CreateProgressBar(Grid panel)
        {
            var progressBar = new ProgressBar
            {
                Margin = new Thickness(10)
            };
            Grid.SetRow(progressBar, 1);
            panel.Children.Add(progressBar);
            return progressBar;
        }

        public Button CreateButton(Grid panel, string text, int column)
        {
            var button = new Button
            {
                Content = text,
                Margin = new Thickness(5),
                Tag = panel
            };

            Grid.SetRow(button, 1);
            Grid.SetColumn(button, column);
            panel.Children.Add(button);
            return button;
        }
    }
}
