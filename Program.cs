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
        // readonly variable used for lock() 
        private static readonly Object universeLock = new Object();
        
        public class ProgressViewer
        {
            public int NbPlanetsDeserialized { get; set; }

            public event EventHandler Planet
            {  
                add  
                {  
                    //Increment the number of planets on event add
                    NbPlanetsDeserialized++;
                }  
                remove  
                {  
                }  
            }

            // EVENT VERSION 1 : the "strange" version
            public void PlanetEvent(ProgressViewer progressViewer)
            {
                progressViewer.Planet += my_PlanetEvent;  
            }  
            
            public void my_PlanetEvent(object sender, EventArgs e)  
            { }

        }
        
        // EVENT VERSION 1 : the "strange" version
        public static void my_PlanetEventGood(object sender, EventArgs e)  
        { }
        
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
        
        // Counts the number of planets 
        public static string planetCount()
        {
            int count = 0;
            // Universe folder relative path
            string dirPath = @"..\..\Universe";
            //Get list of all directories within Universe folder
            string[] dirs = Directory.GetDirectories(dirPath, "*", SearchOption.TopDirectoryOnly);
            //For all directories
            foreach (string dir in dirs)
            {
                string[] allfiles = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
                //For all files
                foreach (var file in allfiles)
                {
                    count++;
                }
            }

            return count.ToString();
        }

        public static async Task Main()
        {
            // EXERCICE 1 + 3 : planet count + synchronous deserialization
            Console.WriteLine("Serialize synchronous timing : ");
            UniverseDeserialize();
            // EXERCICE 2 + 3 : planet count + asynchronous deserialization
            Console.WriteLine("Serialize asynchronous timing : ");
            await UniverseDeserializeAsync();
            Console.ReadKey();  
        }

        // EXERCICE 1 
        public static void UniverseDeserialize()
        {
            // Planet Count
            string planetCountString = planetCount();
            // Progress Viewer
            ProgressViewer progressViewer = new ProgressViewer()
                { NbPlanetsDeserialized = 0 };

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
                        
                        // Triggering planet events :
                        // !!!!!!!!!!!! WARNING !!!!!!!!!!!!!!
                        // TWO VERSION OF EVENTS
                        // VERSION 1 : the strange version
                        //progressViewer.PlanetEvent(progressViewer);
                        // VERSION 2 : the simple version
                        progressViewer.Planet += my_PlanetEventGood;
                        
                        //PROGRESS BAR CONSOLE WRITING
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
            // Planet Count
            string planetCountString = planetCount();
            ProgressViewer progressViewer = new ProgressViewer()
                { NbPlanetsDeserialized = 0 };
            
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
                            lock (universeLock)
                            {
                                // Triggering planet events :
                                // !!!!!!!!!!!! WARNING !!!!!!!!!!!!!!
                                // TWO VERSION OF EVENTS
                                // VERSION 1 : the strange version
                                //progressViewer.PlanetEvent(progressViewer);
                                // VERSION 2 : the simple version
                                progressViewer.Planet += my_PlanetEventGood;
                        
                                //PROGRESS BAR CONSOLE WRITING
                                Console.WriteLine(progressViewer.NbPlanetsDeserialized + "/" + planetCountString);
                            }
                            //Console.WriteLine("Planet name : {0}, size : {1}, usability : {2}, orbit : {3}", planet.Name, planet.Size.ToString(), planet.Usability.ToString(), planet.Orbit.ToString());
                        }
                    }

                    // Add this system to the Universe
                    universe.Systems.Add(system);
                }));
            }
            
            // Await for all tasks completion
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