using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

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

        public class System
        {
            public string Name { get; set; }
            public List<Planet> Planets { get; set; }
        }

        public class Universe
        {
            public List<System> Systems { get; set; }
        }
        

        public static void Main(string[] args)
        {
            UniverseDeserialize();
        }

        public static void UniverseDeserialize()
        {
            // Universe folder relative path
            string dirPath = @"..\..\Universe";

            Universe universe = new Universe() { Systems = new List<System>() };
            
            //Get list of all directories within Universe folder
            string[] dirs = Directory.GetDirectories(dirPath, "*", SearchOption.TopDirectoryOnly);
            foreach (string dir in dirs)
            {
                //Initialize a new System
                System system = new System() { Name = dir, Planets = new List<Planet>() };
                
                Console.WriteLine(dir);
                //Get list of all files within each Universe subfolders (systems)
                string[] allfiles = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
                
                foreach (var file in allfiles){
                    Console.WriteLine(file);
                    //Check if file exists
                    if (File.Exists(file))
                    {
                        string planetFile = File.ReadAllText(file);
                        Planet planet = JsonConvert.DeserializeObject<Planet>(planetFile);
                        system.Planets.Add(planet);
                    }
                }
                // Add this system to the Universe
                universe.Systems.Add(system);
            }
        }
    }
}