using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EXAM_CSHARP
{
    internal class Program
    {
        public static class ProgressViewer
        {
            public static int NbPlanetsDeserialized { get; set; }
            public static int NbSystemsDeserialized { get; set; }

            public static void showProgressPlanets()
            {
                Console.WriteLine(NbPlanetsDeserialized);
            }
            public static void showProgressSystems()
            {
                Console.WriteLine(NbSystemsDeserialized);
            }
        }
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
        
        //Main for EXERCICE 1
        public static void Main(string[] args)
        {
            UniverseDeserialize();
        }

        // EXERCICE 1 
        public static void UniverseDeserialize()
        {
            // Mesuring time for method execution with stopwatch (start)
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            
            // Universe folder relative path
            string dirPath = @"..\..\Universe";

            Universe universe = new Universe() { Systems = new List<System>() };
            
            //Get list of all directories within Universe folder
            string[] dirs = Directory.GetDirectories(dirPath, "*", SearchOption.TopDirectoryOnly);
            foreach (string dir in dirs)
            {
                //Initialize a new System
                System system = new System() { Name = dir, Planets = new List<Planet>() };
                
                //Console.WriteLine(dir);
                //Get list of all files within each Universe subfolders (systems)
                string[] allfiles = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
                
                foreach (var file in allfiles){
                    // Console.WriteLine(file);
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
            //Stop the stopwatch
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
        }

        // Main for EXERCICE 2
        // public static async Task Main()
        // {
        //     await UniverseDeserializeAsync();
        // }

        // EXERCICE 2
        public static async Task UniverseDeserializeAsync()
        {
            List<Task> tasks = new List<Task>();
            
            // Mesuring time for method execution with stopwatch (start)
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            
            // Universe folder relative path
            string dirPath = @"..\..\Universe";

            Universe universe = new Universe() { Systems = new List<System>() };
            
            //Get list of all directories within Universe folder
            string[] dirs = Directory.GetDirectories(dirPath, "*", SearchOption.TopDirectoryOnly);
            foreach (string dir in dirs)
            {
                tasks.Add(Task.Run(() =>
                {
                    //Initialize a new System
                    System system = new System() { Name = dir, Planets = new List<Planet>() };

                    //Get list of all files within each Universe subfolders (systems)
                    string[] allfiles = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);

                    foreach (var file in allfiles)
                    {
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
                }));
            }
            
            await Task.WhenAll(tasks);
            
            //Stop the stopwatch
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
        }
    }
}