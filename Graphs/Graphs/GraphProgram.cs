using System;
using GraphProject;
using System.Diagnostics;

namespace Program
{
    public class Program
    {
        public static void Main(string[] args)
        {

            /////////////////////
            //      SETUP      //
            /////////////////////

            Console.ForegroundColor =
                ConsoleColor.DarkGray; // Do this so that way the degrees output is more noticeable.
            string
                sourceFile =
                    ""; //Need to put this here first since otherwise it's not accessible later on in the program.
            Random rand = new Random(); //Use this for picking which type of fact you have for the celebrities

            // Using these two unfortunately slows down the creation of the graph, BUT will help with making more interesting fun facts.
            List<string> celebrities = new List<string>();


            ///<summary>
            /// Has the user type in a filename to use for the program. Keeps asking the user to input a file until it is a valid one.
            ///</summary>
            void FileInput()
            {
                Console.WriteLine("Please enter a filepath for the IMDb databse you want to use");
                Console.Write("> ");
                sourceFile = Console.ReadLine();
                while (!File.Exists(sourceFile))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[-] Error: File '{sourceFile}' does not exist!");
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("Please enter a filename for the IMDb databse you want to use");
                    Console.Write("> ");
                    sourceFile = Console.ReadLine();
                }

                Console.Clear();
            }

            // Command Line Arg Handling (Will automatically use first arg if given, or will ask user for filename if not given!)
            if (args.Length > 0)
            {
                sourceFile = args[0];
                if (!File.Exists(args[0]))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[-] Error: File '{args[0]}' does not exist!\n\n");
                    Console.ForegroundColor = ConsoleColor.DarkGray;

                    while (!File.Exists(sourceFile))
                    {
                        FileInput();
                    }
                }
            }
            else
            {
                sourceFile = "";
                FileInput();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"File '{sourceFile}' is valid. Creating graph, please wait. . .");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            // Create the MathGraph object for the graph
            MathGraph<string> movieGraph = new MathGraph<string>();
            Stopwatch sw = new Stopwatch();

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
                if (!movieGraph.ContainsVertex(name))
                {
                    movieGraph.AddVertex(name);
                    celebrities.Add(name);
                }

                if (!movieGraph.ContainsVertex(parts[1])) movieGraph.AddVertex(parts[1]);

                // Make an edge between the current celebrity and movie
                movieGraph.AddEdge(name, parts[1]);
            }

            sw.Stop();
            long graphTime = sw.ElapsedMilliseconds; // Save in a long var how long it takes in ms to create the graph
            sw.Reset();

            // Print out the times it took to begin the program
            Console.WriteLine($"\n\nRead the file '{sourceFile}' in {fileTime} ms");
            Console.WriteLine($"Successfully made a graph from the data in {graphTime} ms\n");

        ////////////////////////
        //       ACTUAL       //
        //      PROGRAM       //
        ////////////////////////

        string[] celebs = new string[2];


        // ***** Functions *****

        ///<summary>
        /// Prints out how two celebrities are linked via a common movie, but in special coloring (that mimics what was on the example output)
        ///</summary>
        void ShinyText(string actor1, string movie, string actor2)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(actor1);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" was in ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(movie);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" with ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{actor2}\n");
            Console.ForegroundColor = ConsoleColor.DarkGray;
        }

        ///<summary>
        /// First checks if 2 celebrities are actually linked or not. If they are, then it will search through the whole graph
        /// and print out the stats of that connection via the shortest path. 
        ///</summary>
        void SearchGraph()
        {
            if (!movieGraph.TestConnectedTo(celebs[0], celebs[1]))
            {
                Console.WriteLine($"It looks like '{celebs[0]}' & '{celebs[1]}' aren't connected at all!\n\n");
                return;
            }
            else
            {
                sw.Reset();
                sw.Start();
                List<string> separations = movieGraph.FindShortestPath(celebs[0], celebs[1]);
                int sep = 0;
                foreach (string separation in separations)
                {
                    if (separation.Contains('(')) sep++;
                }

                sw.Stop();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{sep} degrees of separation between '{celebs[0]}' and '{celebs[1]}':");
                for (int i = 0; i < sep * 2; i += 2)
                {
                    ShinyText(separations[i], separations[i + 1], separations[i + 2]);
                }

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(
                    $"Searched over {movieGraph.CountVertices()} verticies and {movieGraph.CountEdges()} edges. Took {sw.ElapsedMilliseconds} ms\n\n");
            }
        }

        void GenerateFactoid()
        {
            int prev = -1;
            foreach (string cel in celebs)
            {
                // Set up all the needed data for each celebrity you need a fun fact for
                List<string> celFilms = new List<string>();
                foreach (string connection in movieGraph.EnumAdjacent(cel))
                {
                    if (connection.Contains('(')) celFilms.Add(connection);
                }

                int choice = rand.Next(0, 6); // This helps pick a random fun fact to display
                while (choice == prev)
                {
                    choice = rand.Next(0, 6);
                }
                prev = choice;
                string factoid = ""; // This is the fact that is output
                switch (choice) // This switches which fact is output based off of the 'choice' variable
                {
                    case 0: // # of movies the celebrity is in
                        factoid = $"Did you know {cel} is in {movieGraph.CountAdjacent(cel)} movie(s)?";
                        break;
                    
                    case 1: // Movie with the most # of celebrities in it that the celebrity stars in
                        int mostCelebs = 0;
                        string biggestMov = "";
                        foreach (string con in celFilms)
                        {
                            if (movieGraph.CountAdjacent(con) > mostCelebs)
                            {
                                mostCelebs = movieGraph.CountAdjacent(con);
                                biggestMov = con;
                            }
                        }

                        factoid = $"Surprisingly, the largest movie {cel} starred in was {biggestMov}!";
                        break;
                    case 2: // Random Movie the celeb is in
                        int randMov = rand.Next(0, celFilms.Count);
                        factoid = $"Fun fact: {cel} starred in {celFilms[randMov]}!";
                        break;
 
                    case 3: // Smallest movie
                        int leastCelebs = 100;
                        string smallestMov = "";
                        foreach (string con in celFilms)
                        {
                            if (movieGraph.CountAdjacent(con) < leastCelebs)
                            {
                                leastCelebs = movieGraph.CountAdjacent(con);
                                smallestMov = con;
                            }
                        }

                        factoid = $"The smallest movie {cel} starred in was {smallestMov}";
                        break;
                    
                    case 4: // Choose a random celebrity and see if the current celeb is connected to them
                        int randChoice = rand.Next(0, celebrities.Count);
                        if (movieGraph.TestConnectedTo(cel, celebrities[randChoice]))
                        {
                            factoid =
                                $"{cel} and {celebrities[randChoice]} are connected somehow! Try that combination up next to find out how!";
                        }
                        else
                        {

                            factoid =
                                $"{cel} and {celebrities[randChoice]} are not connected! So you can strike that off your list of combinations to try!";
                        }

                        break;
                    
                    case 5: // Check potential of relativity to Kevin Bacon
                        if (movieGraph.TestConnectedTo(cel, "Kevin Bacon"))
                        {
                            factoid =
                                $"Apparently {cel} and Kevin Bacon are linked. Maybe try seeing what their degree of separation is next time!";
                        }
                        else
                        {
                            factoid =
                                $"Unfortunately, unlike most celebrities, {cel} and Kevin Bacon are not linked. :(";
                        }
                        break;
                }

                Console.WriteLine(factoid);
            }
        }

        // ***** Main Loop *****
            
        // Run the program once if two celebrities were given as command line args BEFORE entering the loop
            if(args.Length == 3)
            {
                celebs[0] = args[1];
                celebs[1] = args[2];
                Console.WriteLine($"Searching for the Degrees of separation between '{celebs[0]}' and '{celebs[1]}'");
                SearchGraph();
            }
            while(true)
            {
                // ***** Start Loop *****
                Console.WriteLine("Enter two celebrities names, or 'quit' to end the program! (following the format below)\nExample Input: Kevin Bacon and Chris Pratt");
                Console.Write("> ");
                string inp = Console.ReadLine();


                // ****** Input Handling *****
                
                // See if user is trying to quit program
                if ((inp.ToLower().Equals("quit")) || (inp.ToLower().Equals("q")))
                {
                    Console.WriteLine("Quiting Program . . .");
                    break;
                }

                // See if user is following the input format
                if(!inp.Contains(" and "))
                {
                    Console.WriteLine("Oops! Looks like your input didn't follow the format!\n\n");
                    continue;
                }
                
                // See if the celebrities that the user input are in the text file
                celebs = inp.Split(" and ");
                if (celebs[0] == celebs[1])
                {
                    Console.WriteLine($"Slow down there, we already know that '{celebs[0]}' is connected to '{celebs[1]}' because they're the same person!\n\n");
                    continue;
                }
                if(!movieGraph.ContainsVertex(celebs[0]))
                {
                    Console.WriteLine($"Oops! Looks like '{celebs[0]}' isn't in this database.\n\n");
                    continue;
                }
                else if(!movieGraph.ContainsVertex(celebs[1]))
                {
                    Console.WriteLine($"Oops! Looks like '{celebs[1]}' isn't in this database.\n\n");
                    continue;
                }

                Console.WriteLine($"Searching for the Degrees of separation between '{celebs[0]}' and '{celebs[1]}'");

                // ***** Actually Searching *****
                GenerateFactoid();
                SearchGraph();
            }
        }
    }
}