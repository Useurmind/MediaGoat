using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MediaGoat.Utility
{
    public static class FileHelper
    {
        public static void GetFilesRecursive(string path, IList<string> filesFound)
        {
            var filesInCurrentDirectory = Directory.GetFiles(path);
            foreach (var file in filesInCurrentDirectory)
            {
                filesFound.Add(file);
            }

            var subdirectories = Directory.GetDirectories(path);
            foreach (var directory in subdirectories)
            {
                GetFilesRecursive(directory, filesFound);
            }
        }
    }
}
