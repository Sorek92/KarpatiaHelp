using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace KarpatiaHelp
{
    internal class File
    {
        public string FileName { get; set; }
        public string FolderName { get; set; }
        public string Path { get; set; }

        public File(string fileName)
        {
            this.FileName = fileName;
            this.FolderName = Directory.GetCurrentDirectory() + "\\";
            this.Path = this.FolderName + this.FileName;
        }

        public string LoadFile()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "dane";
            dialog.DefaultExt = ".txt";
            dialog.Filter = "Text documents (.txt)|*.txt";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                this.Path = dialog.FileName;
                this.FolderName = dialog.FileName.Substring(0, dialog.FileName.LastIndexOf('\\')) + "\\";
                int l = this.FolderName.Length - 1;
                this.FileName = dialog.FileName.Substring(l + 1, dialog.FileName.Length - l - 1);

            }

            return FileName;
        }

        public void ChangePath(string file)
        {
            if (file != FileName)
            {
                FileName = file;
                Path = FolderName + FileName;
            }
        }

    }
}
