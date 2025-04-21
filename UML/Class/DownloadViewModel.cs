namespace UML.Class.ViewModels
{
    public class DownloadViewModel
    {
        private static DownloadViewModel _instance;
        public static DownloadViewModel Instance => _instance ?? (_instance = new DownloadViewModel());

        public string DownloadProgress { get; set; }
        public string StatusMessage { get; set; }
        public bool IsDownloading { get; set; }

        public double DownloadPercentage { get; set; }
    }
}