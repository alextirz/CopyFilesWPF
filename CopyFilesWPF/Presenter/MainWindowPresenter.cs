using CopyFilesWPF.Model;
using CopyFilesWPF.View;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Button = System.Windows.Controls.Button;
using ProgressBar = System.Windows.Controls.ProgressBar;

namespace CopyFilesWPF.Presenter
{
    public class MainWindowPresenter : IMainWindowPresenter
    {
        private readonly IMainWindowView _mainWindowView;
        private readonly MainWindowModel _mainWindowModel;
        private const double PanelHeight = 60;
        private const double FileNameColumnWidth = 320;

        public MainWindowPresenter(IMainWindowView mainWindowView) {
            _mainWindowView = mainWindowView;
            _mainWindowModel = new MainWindowModel();
        }

        public void ChooseFileFromButtonClick(string path)
        {
            _mainWindowModel.FilePath.PathFrom = path;
        }

        public void ChooseFileToButtonClick(string path)
        {
            _mainWindowModel.FilePath.PathToFolder = path;
        }

        // порефакторить этот метод, убрать хардкод, разделить на более мелкие методы
        public void CopyButtonClick()
        {
          //  _mainWindowModel.FilePath.PathFrom = _mainWindowView.MainWindowView.FromTextBox.Text;
            //_mainWindowModel.FilePath.PathToFolder = _mainWindowView.MainWindowView.ToTextBox.Text;
           
            var fromPath = _mainWindowModel.FilePath.PathFrom;
            var toPath = _mainWindowModel.FilePath.PathToFolder;
            var filePath = Path.GetFileName(fromPath);

            _mainWindowView.ClearPaths();

            Grid panel = CreateProgressPanel(_mainWindowView);
            CreateTextBlock(filePath, panel);
            CreateProgressBar(panel);
            var pauseButton = CreateButton(panel, "Pause", 1);
            var cancelButton = CreateButton(panel, "Cancel", 2);
            pauseButton.Click += PauseCancelClick;
            cancelButton.Click += PauseCancelClick;

            _mainWindowView.AddFilePanel(panel);
            _mainWindowModel.CopyFile(ProgressChanged, OnCopyComplete, panel);
        }


        private static Grid CreateProgressPanel(IMainWindowView _mainWindowView)
        {
            _mainWindowView.MainWindowView.Height = _mainWindowView.MainWindowView.Height + 60;
            var newPanel = new Grid();
            newPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(320) });
            newPanel.ColumnDefinitions.Add(new ColumnDefinition());
            newPanel.ColumnDefinitions.Add(new ColumnDefinition());
            newPanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
            newPanel.RowDefinitions.Add(new RowDefinition());
            newPanel.Height = 60;
            DockPanel.SetDock(newPanel, Dock.Top);
            return newPanel;
        }

        private static TextBlock CreateTextBlock(string filePath, Grid panel)
        {
            // File name TextBlock
            var fileNameText = new TextBlock
            {
                Text = Path.GetFileName(filePath),
                Margin = new Thickness(5, 0, 5, 0)
            };
            Grid.SetRow(fileNameText, 0);
            Grid.SetColumn(fileNameText, 0);
            panel.Children.Add(fileNameText);
            return fileNameText;
        }

        private static ProgressBar CreateProgressBar(Grid panel)
        {
            var progressBar = new ProgressBar
            {
                Margin = new Thickness(10)
            };
            Grid.SetRow(progressBar, 1);
            panel.Children.Add(progressBar);
            return progressBar;
        }

        private Button CreateButton(Grid panel, string text, int column)
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

        // порефакторить этот метод, убрать хардкод, и переделать его по SOLID (тут несколько ответсвенностей)
        private void PauseCancelClick(object sender, RoutedEventArgs routedEventArgs)
        {
            ((Button)sender).IsEnabled = false;
            if(((System.Windows.Controls.Button)sender)!.Content.ToString()!.Equals("Cancel")) {
                ((((Button)sender).Tag as Grid)!.Tag as FileCopier)!.CancelFlag = true;
            }
            else if (((Button)sender)!.Content.ToString()!.Equals("Pause"))
            {
                ((((Button)sender).Tag as Grid)!.Tag as FileCopier)!.PauseFlag.Reset();
            }
            else
            {
                ((((Button)sender).Tag as Grid)!.Tag as FileCopier)!.PauseFlag.Set();
            }
        }

        private void OnCopyComplete(Grid panel)
        {
            _mainWindowView.MainWindowView.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    _mainWindowView.MainWindowView.Height = _mainWindowView.MainWindowView.Height - 60;
                    _mainWindowView.MainWindowView.MainPanel.Children.Remove(panel);
                    _mainWindowView.MainWindowView.CopyButton.IsEnabled = true;
                }
            );
        }

        // порефакторить этот метод, убрать хардкод, и переделать его по SOLID (тут несколько ответсвенностей)
        private void ProgressChanged(double persentage, ref bool cancelFlag, Grid panel)
        {
            _mainWindowView.MainWindowView.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    foreach (var el in panel.Children)
                    {
                        if (el is ProgressBar bar)
                        {
                            bar.Value = persentage;
                        }
                        if (el is Button button1 && button1!.Content.ToString()!.Equals("Resume") && button1!.IsEnabled == false)
                        {
                            button1.Content = "Pause";
                            button1.IsEnabled = true;
                        }
                        else if (el is Button button && button!.Content.ToString()!.Equals("Pause") && button.IsEnabled == false)
                        {
                            button.Content = "Resume";
                            button.IsEnabled = true;
                        }
                    }
                }
            );
        }
    }
}
