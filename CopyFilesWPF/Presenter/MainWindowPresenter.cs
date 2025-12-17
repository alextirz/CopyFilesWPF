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
        private const int PanelHeight = 60;
        private const int FileNameColumnWidth = 320;
        private const int FileNameColumnHeight = 20;
        public const string PauseButtonName = "Pause"; 
        public const string CancelButtonName = "Pause";
        public const string ResumeButtonName = "Resume";

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
            var filePath = Path.GetFileName(_mainWindowModel.FilePath.PathFrom);
            _mainWindowView.ClearPaths();

            Grid panel = _mainWindowView.CreateProgressPanel(PanelHeight, FileNameColumnWidth, FileNameColumnHeight);
            _mainWindowView.CreateTextBlock(filePath, panel);
            _mainWindowView.CreateProgressBar(panel);
            var pauseButton = _mainWindowView.CreateButton(panel, PauseButtonName, 1);
            var cancelButton = _mainWindowView.CreateButton(panel, CancelButtonName, 2);

            pauseButton.Click += PauseCancelClick;
            cancelButton.Click += PauseCancelClick;

            _mainWindowView.AddFilePanel(panel);
            _mainWindowModel.CopyFile(ProgressChanged, OnCopyComplete, panel);
        }


        // порефакторить этот метод, убрать хардкод, и переделать его по SOLID (тут несколько ответсвенностей)
        private void PauseCancelClick(object sender, RoutedEventArgs routedEventArgs)
        {
            ((Button)sender).IsEnabled = false;
            if(((Button)sender)!.Content.ToString()!.Equals(CancelButtonName)) {
                ((((Button)sender).Tag as Grid)!.Tag as FileCopier)!.CancelFlag = true;
            }
            else if (((Button)sender)!.Content.ToString()!.Equals(PauseButtonName))
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
                    _mainWindowView.MainWindowView.Height = _mainWindowView.MainWindowView.Height - PanelHeight;
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
                        if (el is Button button1 && button1!.Content.ToString()!.Equals(ResumeButtonName) && button1!.IsEnabled == false)
                        {
                            button1.Content = PauseButtonName;
                            button1.IsEnabled = true;
                        }
                        else if (el is Button button && button!.Content.ToString()!.Equals(PauseButtonName) && button.IsEnabled == false)
                        {
                            button.Content = ResumeButtonName;
                            button.IsEnabled = true;
                        }
                    }
                }
            );
        }
    }
}
