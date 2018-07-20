using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipIt
{
    class Program
    {
        private static string directoryToWorkOn = AppDomain.CurrentDomain.BaseDirectory;
        private static string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        static void Main(string[] args)
        {
            bool doContinue = true;
            bool doCompress = false;
            while (doContinue)
            {
                Console.Clear();
                Console.WriteLine("Do you wish to compress or decompress? (C/D)");
                string input = Console.ReadKey().Key.ToString().ToUpperInvariant();

                switch (input)
                {
                    case "C":
                        doCompress = true;
                        doContinue = false;
                        break;
                    case "D":
                        doContinue = false;
                        break;
                    default:
                        break;
                }

                Console.Clear();

                if (doCompress)
                {
                    Compress();
                }
                else
                {
                    Decompress();
                }
            }
            Console.WriteLine("All done!");
            Console.ReadKey();

        }

        public static void Compress()
        {         
            // Create a new folder for the zipped files
            string zipDirectory = desktopPath + @"\Zips";
            Directory.CreateDirectory(zipDirectory);

            // Get files
            DirectoryInfo directorySelected = new DirectoryInfo(directoryToWorkOn);
            var files = directorySelected.GetFiles().Where((f) => f.Extension != ".exe");

            Parallel.ForEach(files, (fileToCompress) =>
            {
                using (FileStream originalFileStream = fileToCompress.OpenRead())
                {
                    if ((File.GetAttributes(fileToCompress.FullName) &
                       FileAttributes.Hidden) != FileAttributes.Hidden & fileToCompress.Extension != ".gz")
                    {
                        using (FileStream compressedFileStream = File.Create(zipDirectory + @"\" + fileToCompress.Name + ".gz"))
                        {
                            using (GZipStream compressionStream = new GZipStream(compressedFileStream,
                               CompressionMode.Compress))
                            {
                                originalFileStream.CopyTo(compressionStream);

                            }
                        }
                        FileInfo info = new FileInfo(zipDirectory + "\\" + fileToCompress.Name + ".gz");
                        Console.WriteLine("Compressed {0} from {1} to {2} bytes.",
                        fileToCompress.Name, fileToCompress.Length.ToString(), info.Length.ToString());
                    }
                }
            });            
        }

        public static void Decompress()
        {
            // Create a new folder for the unzipped files
            string unzipDirectory = directoryToWorkOn + @"\Unzips";
            Directory.CreateDirectory(unzipDirectory);

            // Get files
            DirectoryInfo directorySelected = new DirectoryInfo(directoryToWorkOn);
            var files = directorySelected.GetFiles().Where((f) => f.Extension != ".exe");

            Parallel.ForEach(files, (fileToDecompress) =>
            {
                using (FileStream originalFileStream = fileToDecompress.OpenRead())
                {
                    string currentFileName = fileToDecompress.Name;
                    string newFileName = unzipDirectory + @"\" + currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);

                    using (FileStream decompressedFileStream = File.Create(newFileName))
                    {
                        using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                        {
                            decompressionStream.CopyTo(decompressedFileStream);
                            Console.WriteLine("Decompressed: {0}", fileToDecompress.Name);
                        }
                    }
                }
            });

        }


    }
}
