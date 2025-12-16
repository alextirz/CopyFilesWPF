using System.Windows;
using System.Windows.Controls;

namespace CopyFilesWPF.View
{
    public interface IMainWindowView
    {
        MainWindow MainWindowView { get; }

        void AddFilePanel(Grid filePanel);
        void ClearPaths();
        public Grid CreateFilePanel(string filePath, RoutedEventHandler pauseHandler, RoutedEventHandler cancelHandler);
    }
}
