using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;


namespace Model.Data
{
    public static class FilePaths
    {
        private static string _filesDirectory;

       
        /// Возвращает путь к папке Files в проекте Model
      
        //public static string GetFilesDirectory()
        //{
        //    if (_filesDirectory == null)
        //    {
        //        // Получаем путь к DLL библиотеки Model
        //        string modelDllPath = Assembly.GetExecutingAssembly().Location;
        //        string modelProjectDir = Directory.GetParent(modelDllPath).Parent.Parent.FullName;
        //        _filesDirectory = Path.Combine(modelProjectDir, "Files");

        //        // Создаём папку, если её нет
        //        if (!Directory.Exists(_filesDirectory))
        //            Directory.CreateDirectory(_filesDirectory);
        //    }
        //    return _filesDirectory;
        //}
        public static string GetFilesDirectory()
        {
            // ВРЕМЕННО: просто папка рядом с EXE
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            string filesDir = Path.Combine(exeDir, "Files");

            if (!Directory.Exists(filesDir))
                Directory.CreateDirectory(filesDir);

            return filesDir;
        }

        /// Возвращает полный путь к файлу (с учётом формата)
        public static string GetEstablishmentPath(string fileName, string format)
        {
            string extension = format.ToLower() == "json" ? "json" : "xml";
            string fullFileName = Path.ChangeExtension(fileName, extension);
            return Path.Combine(GetFilesDirectory(), fullFileName);
        }

        /// Возвращает полный путь к существующему файлу по имени (без расширения)
        public static string GetExistingFilePath(string fileNameWithoutExtension)
        {
            string filesDir = GetFilesDirectory();

            // Ищем JSON
            string jsonPath = Path.Combine(filesDir, fileNameWithoutExtension + ".json");
            if (File.Exists(jsonPath))
                return jsonPath;

            // Ищем XML
            string xmlPath = Path.Combine(filesDir, fileNameWithoutExtension + ".xml");
            if (File.Exists(xmlPath))
                return xmlPath;

            return null;
        }

    }
}