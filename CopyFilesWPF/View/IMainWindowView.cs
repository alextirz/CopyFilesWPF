using System.Windows;
using System.Windows.Controls;

namespace CopyFilesWPF.View
{
    public interface IMainWindowView
    {
        MainWindow MainWindowView { get; }

        void AddFilePanel(Grid filePanel);
        void ClearPaths();
        public Grid CreateProgressPanel(int height, int gridWidth, int gridHeight);
        TextBlock CreateTextBlock(string filePath, Grid panel);
        ProgressBar CreateProgressBar(Grid panel);
        Button CreateButton(Grid panel, string v1, int v2);
    }
}
