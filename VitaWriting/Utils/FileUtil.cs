using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace VitaWriting.Utils
{
    public static class FileUtil
    {
        public static readonly string RootFolder = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// 获取文件夹内所有文件
        /// </summary>
        public static List<FileInfo> GetFiles(string dir, bool deep = false)
        {
            var result = new List<FileInfo>();
            TraverseFiles(dir, deep, result.Add);
            return result;
        }

        /// <summary>
        /// 获取文件夹内所有文件名称
        /// </summary>
        public static List<string> GetFileNames(string dir, bool deep = false)
        {
            var result = new List<string>();
            TraverseFiles(dir, deep, file => result.Add(file.Name));
            return result;
        }

        /// <summary>
        /// 获取文件夹内所有文件名称（不带扩展名）
        /// </summary>
        public static List<string> GetFileNamesWithoutExtensions(string dir, bool deep = false)
        {
            var result = new List<string>();
            TraverseFiles(dir, deep, file => result.Add(Path.GetFileNameWithoutExtension(file.Name)));
            return result;
        }

        /// <summary>
        /// 获取文件夹内文件 (不存在时返回null)
        /// </summary>
        public static FileInfo GetFileOrNull(string filePath)
        {
            var file = new FileInfo(Path.Combine(RootFolder, filePath));
            return file.Exists ? file : null;
        }

        /// <summary>
        /// 获取文件夹内文件 (不存在时创建文件)
        /// </summary>
        public static FileInfo GetFileOrCreate(string filePath)
        {
            var file = new FileInfo(Path.Combine(RootFolder, filePath));
            if (!file.Exists)
            {
                file.Directory?.Create();
                file.Create().Close();
            }

            file.Refresh();
            return file;
        }

        /// <summary>
        /// 读取文本文件
        /// </summary>
        public static string ReadText(FileInfo file)
        {
            return File.ReadAllText(file.FullName);
        }

        /// <summary>
        /// 写入文本文件
        /// </summary>
        public static void WriteText(FileInfo file, string text)
        {
            File.WriteAllText(file.FullName, text);
        }

        /// <summary>
        /// 保存资源文件
        /// </summary>
        public static void SaveResource(string resourceName, string outPath = "")
        {
            var assembly = Assembly.GetExecutingAssembly();

            var fullResourceName = $"VitaWriting.Resources.{resourceName}";

            using (var stream = assembly.GetManifestResourceStream(fullResourceName))
            {
                if (stream == null) return;

                var filePath = Path.Combine(RootFolder, outPath);

                if (string.IsNullOrEmpty(Path.GetExtension(filePath))) filePath = Path.Combine(filePath, resourceName);

                var directoryPath = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                if (File.Exists(filePath)) return;

                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    stream.CopyTo(fileStream);
                }
            }
        }


        private static void TraverseFiles(string dir, bool deep, Action<FileInfo> action)
        {
            var directory = new DirectoryInfo(Path.Combine(RootFolder, dir));
            if (!directory.Exists) throw new ArgumentException("Directory does not exist.");

            foreach (var file in directory.GetFiles()) action(file);

            if (deep)
                foreach (var subDir in directory.GetDirectories())
                    TraverseFiles(subDir.FullName, true, action);
        }
    }
}