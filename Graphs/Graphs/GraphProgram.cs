using System;
using GraphProject;
using System.Diagnostics;

namespace Program
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("[-] Error: No arguments were given. There must be at least ONE argument (a filename). Exiting program. . .");
                return;
            }
            if (File.Exists(args[0]))
            {
                Console.WriteLine($"DEBUG: Filepath '{args[0]}' exists!");
            }
            else
            {
                Console.WriteLine($"[-] Error: File '{args[0]}' does not exist!");
                return;
            }
            if (args.Length >= 3)
            {
                // args[1] and args[2] are going to be the initial celebrity names, I believe.
                Console.WriteLine("[+] DEBUG: Two optional args detected! Printing inputs. . .");
                Console.WriteLine(args[1]);
                Console.WriteLine(args[2]);
            }


            // Create the MathGraph object for the graph
            MathGraph<string> movieGraph = new MathGraph<string>();
            Stopwatch sw = new Stopwatch();

            string sourceFile = args[0];
            
            // Use Stopwatch when reading file to see how long it takes (and assign a variable to call this later)
            sw.Start();
            string[] lines = File.ReadAllLines(sourceFile);
            sw.Stop();
            long fileTime = sw.ElapsedMilliseconds;
            sw.Reset();

            // Start to go through the file and make a graph from it (use the stopwatch to time it and store time after loop)
            sw.Start();
            string name = "";
            foreach (string line in lines)
            {
                // Split each line into celebrity & movie
                string[] parts = line.Split('|');
                // If celebrity has a  suffix, cut it
                if (parts[0].Contains('('))
                {
                    int chop = parts[0].IndexOf('(');
                    chop--;
                    name = parts[0].Substring(0, chop);
                    
                }
                else name = parts[0];

                // Check to see if celebrity or movie is already in the graph. If it isn't, make new vertex. If it is, no new vertex
                if (!movieGraph.ContainsVertex(name)) movieGraph.AddVertex(name);
                if (!movieGraph.ContainsVertex(parts[1])) movieGraph.AddVertex(parts[1]);

                // Make an edge between the current celebrity and movie
                movieGraph.AddEdge(name, parts[1]);
            }
            sw.Stop();
            long graphTime = sw.ElapsedMilliseconds;
            sw.Reset();

            // Print out the times it took to begin the program
            Console.WriteLine($"[+] DEBUG INFO\nRead the file '{sourceFile.Substring(2)}' in {fileTime} ms");
            Console.WriteLine($"Successfully made a graph from the data in {graphTime} ms\n");


            // This function will 
            public void CalcDegree()
            {
                Console.WriteLine("Running the CalcDegree() function!");
            }



            string[] celebs = new string[2];
            if(args.Length == 3)
            {
                celebs[0] = args[1];
                celebs[1] = args[2];
            }
            while(true)
            {

                // Prompt for user input to begin the program
                Console.WriteLine("Enter two celebrities names (following the format below)\nExample Input: Kevin Bacon and Chris Pratt");
                Console.Write("> ");
                string inp = Console.ReadLine();
                string[] celebs = inp.Split(" and ");
                Console.WriteLine($"Searching for the Degrees of separation between '{celebs[0]}' and '{celebs[1]}'");

            }
        }
    }
}