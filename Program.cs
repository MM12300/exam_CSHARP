using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EXAM_CSHARP
{
    internal class Program
    {
        private static readonly Object universeLock = new Object();
        
        public class ProgressViewer
        {
            public int NbPlanetsDeserialized { get; set; }
            public int NbSystemsDeserialized { get; set; }

            public event EventHandler Planet
            {  
                add  
                {  
                    NbPlanetsDeserialized++;
                }  
                remove  
                {  
                }  
            }

            public void PlanetEvent(ProgressViewer progressViewer)
            {
                //ProgressViewer progressViewer = new ProgressViewer();
                progressViewer.Planet += my_PlanetEvent;  
            }  
            
            public void my_PlanetEvent(object sender, EventArgs e)  
            { }

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
        
        public static string planetCount()
        {
            int count = 0;
            // Universe folder relative path
            string dirPath = @"..\..\Universe";

            //Get list of all directories within Universe folder
            string[] dirs = Directory.GetDirectories(dirPath, "*", SearchOption.TopDirectoryOnly);
            foreach (string dir in dirs)
            {
                string[] allfiles = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
                foreach (var file in allfiles)
                {
                    count++;
                }
            }

            return count.ToString();
        }

        // Main for EXERCICE 2
        public static async Task Main()
        {
            // EXERCICE 1 + 3
            //Console.WriteLine("Serialize synchronous timing : ");
            //UniverseDeserialize();
            // EXERCICE 2 + 3 
            Console.WriteLine("Serialize asynchronous timing : ");
            await UniverseDeserializeAsync();
            Console.ReadKey();  
        }

        // EXERCICE 1 
        public static void UniverseDeserialize()
        {
            string planetCountString = planetCount();
            ProgressViewer progressViewer = new ProgressViewer()
                { NbPlanetsDeserialized = 0, NbSystemsDeserialized = 0 };

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
                        
                        // Triggering planet events
                        progressViewer.PlanetEvent(progressViewer);
                        Console.WriteLine(progressViewer.NbPlanetsDeserialized + "/" + planetCountString);

                        system.Planets.Add(planet);
                        
                        //Console.WriteLine("Planet name : {0}, size : {1}, usability : {2}, orbit : {3}", planet.Name, planet.Size.ToString(), planet.Usability.ToString(), planet.Orbit.ToString());
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

        // EXERCICE 2
        public static async Task UniverseDeserializeAsync()
        {
            string planetCountString = planetCount();
            ProgressViewer progressViewer = new ProgressViewer()
                { NbPlanetsDeserialized = 0, NbSystemsDeserialized = 0 };
            
            //New task list
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
                            // Triggering planet events
                            lock (universe)
                            {
                                progressViewer.PlanetEvent(progressViewer);
                                Console.WriteLine(progressViewer.NbPlanetsDeserialized + "/" + planetCountString);
                            }
                            //Console.WriteLine("Planet name : {0}, size : {1}, usability : {2}, orbit : {3}", planet.Name, planet.Size.ToString(), planet.Usability.ToString(), planet.Orbit.ToString());
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