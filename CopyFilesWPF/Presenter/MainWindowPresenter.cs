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
        public const string CancelButtonName = "Cancel";
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
        public void CopyButtonClick()
        {
            var fileName = Path.GetFileName(_mainWindowModel.FilePath.PathFrom);

            _mainWindowView.ClearPaths();

            var panel = CreatePanelWithControls(fileName);

            _mainWindowView.AddFilePanel(panel);

            _mainWindowModel.CopyFile(ProgressChanged, OnCopyComplete, panel);
        }

        private Grid CreatePanelWithControls(string fileName)
        {
            var panel = _mainWindowView.CreateProgressPanel(
                PanelHeight,
                FileNameColumnWidth,
                FileNameColumnHeight);

            _mainWindowView.CreateTextBlock(fileName, panel);
            _mainWindowView.CreateProgressBar(panel);

            var pauseButton = _mainWindowView.CreateButton(panel, PauseButtonName, 1);
            var cancelButton = _mainWindowView.CreateButton(panel, CancelButtonName, 2);

            pauseButton.Click += PauseCancelClick;
            cancelButton.Click += PauseCancelClick;

            return panel;
        }


        private void PauseCancelClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            button.IsEnabled = false;

            var panel = (Grid)button.Tag;
            var copier = (FileCopier)panel.Tag;

            if (button.Content.Equals(CancelButtonName))
            {
                copier.CancelFlag = true;
                return;
            }

            if (button.Content.Equals(PauseButtonName))
            {
                copier.PauseFlag.Reset();
                return;
            }

            copier.PauseFlag.Set();
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

        private void ProgressChanged(double percentage, ref bool cancelFlag, Grid panel)
        {
            _mainWindowView.MainWindowView.Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                (ThreadStart)(() =>
                {
                    UpdateProgress(panel, percentage);
                    UpdatePauseButton(panel);
                }));
        }
        private static void UpdateProgress(Grid panel, double percentage)
        {
            foreach (var element in panel.Children)
            {
                if (element is ProgressBar bar)
                {
                    bar.Value = percentage;
                    return;
                }
            }
        }

        private void UpdatePauseButton(Grid panel)
        {
            foreach (var element in panel.Children)
            {
                if (element is Button button && !button.IsEnabled)
                {
                    if (button.Content.Equals(PauseButtonName))
                    {
                        button.Content = ResumeButtonName;
                        button.IsEnabled = true;
                    }
                    else if (button.Content.Equals(ResumeButtonName))
                    {
                        button.Content = PauseButtonName;
                        button.IsEnabled = true;
                    }
                }
            }
        }


    }
}
