using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace FileSplitor
{
    public class Program
    {
        private static int _splite_size = 32;//mb
        private static string _path = Directory.GetCurrentDirectory();
        private static string _output_path = _path + "/output/";
        private static string _source_path = _path + "/file/";


        public static void Main(string[] args)
        {

            Console.OutputEncoding = System.Text.Encoding.UTF8;

            ParseArgs(args);

            Log("Opening path: " + _path);
            if (!Directory.Exists(_path))
            {
                Log("Source Path does not exist. Exiting.");
                PauseExit();
                return;
            }

            if (!Directory.Exists(_source_path))
            {
                Log("[file] Path does not exist. Exiting.");
                PauseExit();
                return;
            }

            if (!Directory.Exists(_output_path))
            {
                Directory.CreateDirectory(_output_path);
            }

            var files = Directory.GetFiles(_source_path).ToList();

            if (files.Count == 0)
            {
                Log("No files exist. Exiting.");
                PauseExit();
                return;
            }

            string filetosplit = "";
            string extension = "";
            foreach (var file in files)
            {
                filetosplit = Path.GetFileNameWithoutExtension(file);
                extension = Path.GetExtension(file);
                break;
            }
            Log(filetosplit);
            filetosplit = _source_path + filetosplit + extension;
            FileStream fsr = new FileStream(filetosplit, FileMode.Open, FileAccess.Read);
            long FileLength = fsr.Length;
            byte[] btArr = new byte[FileLength];
            fsr.Read(btArr, 0, (int)FileLength);
            fsr.Close();
            long PartLength = _splite_size * 1024 * 1024;
            int nCount = (int)Math.Ceiling((double)FileLength / PartLength);
            string strFileName = Path.GetFileName(filetosplit);
            long byteCount = 0;
            for (int i = 1; i <= nCount; i++, byteCount = (i < nCount ? byteCount + PartLength : FileLength - PartLength))
            {
                FileStream fsw = new FileStream(_output_path + Path.DirectorySeparatorChar +"part_"+ i + "_"+strFileName, FileMode.Create, FileAccess.Write);
                fsw.Write(btArr, (int)byteCount, (int)(i < nCount ? PartLength : FileLength - byteCount));
                fsw.Flush();
                fsw.Close();
            }

            Console.WriteLine("Splite complete.");


            PauseExit();

        }

        static void PauseExit()
        {
            while (!Console.ReadLine().ToUpper().Contains("CLOSE")) continue;
            Environment.Exit(0);
        }


        private static void Log(string message)
        {
            Console.WriteLine(message);
        }

        private static void ParseArgs(string[] args)
        {
            if (args.Length != 0 && !args[0].Contains('-')) _path = args[0];

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "-s":
                    case "-size":
                        _splite_size = int.Parse(args[i + 1]);
                        break;
                    case "-help":
                    case "-h":
                        PrintHelp();
                        Environment.Exit(0);
                        return;
                    default:
                        break;
                }
            }
        }

        private static void PrintHelp()
        {
            Console.WriteLine("File Splitor");
            Console.WriteLine("Place the files to be split into [file] directory,The split file is located in the [out] directory");
            Console.WriteLine("-size or -s, splite file size /MB(default: 32MB).");
        }
    }

}
