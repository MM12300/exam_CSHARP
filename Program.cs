using System;
using System.IO;

namespace EXAM_CSHARP
{
    internal class Program
    {
        public class Planet
        {
            public int Size { get; set; }
            public int Usability { get; set; }
            public int Orbit { get; set; }
            public string Name { get; set; }
        }

        public static void Main(string[] args)
        {
            string dirPath = @"..\..\Universe";
            //Get list of all directories within Universe folder
            string[] dirs = Directory.GetDirectories(dirPath, "*", SearchOption.TopDirectoryOnly);
            foreach (string dir in dirs)
            {
                Console.WriteLine(dir);
                //Get list of all files within each Universe subfolders (systems)
                string[] allfiles = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
                foreach (var file in allfiles){
                    Console.WriteLine(file);
                }
            }
        }
    }
}