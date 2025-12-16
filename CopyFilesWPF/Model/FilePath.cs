using System.IO;

namespace CopyFilesWPF.Model
{
    public sealed record FilePath
    {
        public string PathToFolder { get; set; } = string.Empty;

        public string PathFrom { get; set; } = string.Empty;

        public string PathTo => Path.Combine(PathToFolder, Path.GetFileName(PathFrom));
    }
}
