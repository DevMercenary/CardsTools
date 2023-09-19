namespace CardsTools.Data.Tools
{
    internal static class FileHelper
    {
        private static string _rootPath = Directory.GetCurrentDirectory();
        private static string _storageFolderName = "DeskCardSave";
        private static string _storageBackupFolderName = "Backup";
        public static string GetStoragePath()
        {
            string fullPath = Path.Combine(_rootPath, _storageFolderName);
            Directory.CreateDirectory(fullPath);
            return fullPath;
        }
        public static string GetStorageBackupPath(string nameBackup)
        {
            string fullPath = Path.Combine(GetStoragePath(), Path.Combine(_storageBackupFolderName, nameBackup));
            Directory.CreateDirectory(fullPath);
            return fullPath;
        }
        public static List<string> ReadFiles()
        {
            var path = GetStoragePath();

            var fileType = "*.json";
            var allFiles = new List<string>();

            var jsonFiles = Directory.GetFiles(path, fileType, SearchOption.AllDirectories).ToList();
            foreach (var item in jsonFiles)
            {
                allFiles.Add(item);
            }

            return allFiles;
        }
    }
}
