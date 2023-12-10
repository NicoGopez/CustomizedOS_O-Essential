using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using Sys = Cosmos.System;
using Cosmos.Core;
using Cosmos.System.FileSystem;
using System.Text.RegularExpressions;
using System.Management;
using System.Net;



namespace Program
{
    public class FileData
    {
        public string Name { get; set; }
        public string Content { get; set; }
    }

    //--------------------


    //--------------------
    public class FileSystem
    {
        private List<FileData> files;

        public FileSystem()
        {
            files = new List<FileData>();
        }

        public void AddFile(string name, string content)
        {
            files.Add(new FileData { Name = name, Content = content });
        }

        public bool FileExists(string name)
        {
            return files.Exists(file => file.Name == name);
        }

        public string ReadFile(string name)
        {
            var file = files.Find(f => f.Name == name);
            return file != null ? file.Content : null;
        }

        public void WriteFile(string name, string content)
        {
            var file = files.Find(f => f.Name == name);
            if (file != null)
            {
                file.Content = content;
            }
            else
            {
                AddFile(name, content);
            }
        }

        public void DeleteFile(string name)
        {
            files.RemoveAll(file => file.Name == name);
        }

        public List<FileData> GetFiles()
        {
            return files;
        }

        //------------------------------------------------------------------------------

        //---------------------------------------------------------------------------

        public class Kernel : Sys.Kernel

        {

            private bool isLoggedIn = false;
            private bool shouldShutdown = false;
            private bool shouldReboot = false;

            private FileSystem fileSystem;

            private CosmosVFS vfs;

         


            public void login()
            {
                Console.Write("  Enter username: ");
                string username = Console.ReadLine();

                Console.Write("  Enter password: ");
                string password = "";
                ConsoleKeyInfo key;
                do
                {
                    key = Console.ReadKey(true);
                    if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                    {
                        password += key.KeyChar;
                        Console.Write("*");
                    }
                } while (key.Key != ConsoleKey.Enter);

                if (username == "admin" && password == "admin")
                {
                    space();
                    Console.WriteLine("  Login successful!");
                    isLoggedIn = true;
                }
                else
                {
                    space();
                    Console.WriteLine("  Invalid username or password. Please try again.");
                    isLoggedIn = false;
                }
            }



            protected override void BeforeRun()
            {
                fileSystem = new FileSystem();

                Console.BackgroundColor = ConsoleColor.Blue;
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine("               ____        ____                    __   _        __");
                Console.WriteLine("              / __ \\ ____ / __/___  ___ ___  ___  / /_ (_)___ _ / /");
                Console.WriteLine("             / /_/ //___// _/ (_-< (_-</ -_)/ _ \\/ __// // _ `// / ");
                Console.WriteLine("             \\____/     /___//___//___/\\__//_//_/\\__//_/ \\_,_//_/  ");
                space();
                Console.WriteLine("        Welcome to Customized Operating System of Group 1 named O-Essential");
                Console.WriteLine("             Kindly log-in to proceed with using our customized OS.");
                space();
                login();


            }

            protected override void Run()
            {
                if (!isLoggedIn)
                {
                    login();
                }

                if (isLoggedIn)
                {
                    Console.WriteLine("  Type 'help' to see the list of available commands in this OS.");
                    while (true)
                    {
                        Console.Write("  Input: ");
                        string command = Console.ReadLine();

                        switch (command)
                        {
                            //BASIC REQS.
                            case "help":
                                space();
                                commandlist();
                                space();
                                break;

                            case "about":
                                space();
                                about();
                                space();
                                break;

                            case "shutdown":
                                space();
                                Cosmos.System.Power.Shutdown();
                                space();
                                break;

                            case "restart":
                                space();
                                Cosmos.System.Power.Reboot();
                                space();
                                break;

                            case "signout":
                                space();
                                SignOut();
                                space();
                                break;

                            case "datetime":
                                space();
                                datetime();
                                space();
                                break;

                            case "kernel":
                                space();
                                kernelvr();
                                space();
                                break;

                            case "theme":
                                space();
                                SetConsoleColors();
                                space();
                                break;

                            case "clear":
                                space();
                                ClearCommands();
                                space();
                                break;

                            case "cpu":
                                space();
                                Console.WriteLine($"  CPU: {CPU.GetCPUBrandString()}");
                                Console.WriteLine($"  Processors: {Environment.ProcessorCount}");
                                space();
                                break;

                            case "calcu":
                                space();
                                Console.WriteLine("Simple Calculator");

                                while (true)
                                {
                                    Console.WriteLine("  \nEnter an expression (e.g., 5 + 3) or 'exit' to end:");

                                    string input = Console.ReadLine();

                                    if (input.ToLower() == "exit")
                                        break;

                                    try
                                    {
                                        double result = EvaluateExpression(input);
                                        Console.WriteLine($"  Result: {result}");
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Error: {ex.Message}");
                                    }
                                }
                                space();
                                break;


                            //FILE SYSTEM

                            case "cfile":
                                space();
                                CreateFile();
                                space();
                                break;

                            case "rfile":
                                space();
                                ShowFile();
                                ReadFile();
                                space();
                                break;

                            case "efile":
                                space();
                                ShowFile();
                                EditFile();
                                space();
                                break;

                            case "showfile":
                                space();
                                ShowMemoryFile();
                                break;

                            case "dfile":
                                space();
                                ShowFile();
                                DeleteFile();
                                space();
                                break;

                            //GAMES

                            case "guessnum":
                                space();
                                PlayGuessTheNumber();
                                space();
                                break;

                            case "rpsgame":
                                space();
                                RockPaperScissorsGame();
                                space();
                                break;



                            default:
                                space();
                                Console.WriteLine("Invalid command. Type 'help' for a list of commands.");
                                space();
                                break;
                        }
                    }
                }
            }

            public void space()
            {
                Console.WriteLine();
            }

            static double EvaluateExpression(string expression)
            {
                // Split the input into operands and operator
                string[] elements = expression.Split(' ');

                if (elements.Length != 3)
                    throw new ArgumentException("Invalid expression format. Please enter an expression like '5 + 3'.");

                // Parse the operands
                if (!double.TryParse(elements[0], out double operand1) || !double.TryParse(elements[2], out double operand2))
                    throw new ArgumentException("Invalid operands. Please enter valid numbers.");

                // Perform the operation based on the operator
                switch (elements[1])
                {
                    case "+":
                        return operand1 + operand2;
                    case "-":
                        return operand1 - operand2;
                    case "*":
                        return operand1 * operand2;
                    case "/":
                        if (operand2 == 0)
                            throw new DivideByZeroException("Cannot divide by zero.");
                        return operand1 / operand2;
                    default:
                        throw new ArgumentException("Invalid operator. Please use +, -, *, or /.");
                }
            }

            public void commandlist()
            {
                Console.WriteLine("      ------------------------- BASIC FEATURES -------------------------");
                Console.WriteLine("       help - list of commands                shutdown - shutdown the OS");
                Console.WriteLine("       about - detail about the OS            restart - restart the OS");
                Console.WriteLine("       kernel - show the kernel version       cpu - shows cpu and its count");    
                space();
                Console.WriteLine("      ------------------------- OTHER FEATURES -------------------------");
                Console.WriteLine("       datetime - show current date and time");
                Console.WriteLine("       theme - change the colors of the console and its font");
                Console.WriteLine("       clear - clear old commands");
                Console.WriteLine("       calcu - basic calculator");
                space();
 
                Console.WriteLine("      ====================== FILE SYSTEM COMMANDS ======================");
                Console.WriteLine("       cfile - create new file                rfile - read file");
                Console.WriteLine("       efile - edit and save file             dfile - deletes file");
                Console.WriteLine("       showfile - show the list of file");
                space();
                Console.WriteLine("      ============================== GAMES =============================");
                Console.WriteLine("       guessnum     |   play number guessing game");
                Console.WriteLine("       rpsgame      |   play rock paper scissors game ");
                space();
                Console.WriteLine("      ------------------------------------------------------------------");
                Console.WriteLine("                     TYPE 'signout' TO LOG-OUT IN THE OS.               ");
            }

            public void datetime()
            {
                DateTime currentDateTime = DateTime.Now;
                Console.WriteLine("  Current Date: " + currentDateTime.ToShortDateString());
                Console.WriteLine("  Current Time: " + currentDateTime.ToString("h:mm tt"));
            }

            public void kernelvr()
            {
                string KernelVersion = "O-Essential Kernel 1.0";
                Console.WriteLine("  Kernel Version: " + KernelVersion);
            }

            public void about()
            {
                string About = "This OS is made by Group 1 (December 2023)";
                Console.WriteLine("  About: " + About);
            }

            public void SetConsoleColors()
            {
                Console.WriteLine("  Select a theme:");

                Console.WriteLine("    1. Theme 1 [Black - White]");
                Console.WriteLine("    2. Theme 2 [DarkBlue - White]");
                Console.WriteLine("    3. Theme 3 [DarkRed - White]");
                Console.WriteLine("    4. Theme 4 [White - Black]");

                Console.Write("  Enter theme option: ");
                if (int.TryParse(Console.ReadLine(), out int themeOption))
                {
                    switch (themeOption)
                    {
                        case 1:
                            ApplyTheme(ConsoleColor.Black, ConsoleColor.White);
                            break;
                        case 2:
                            ApplyTheme(ConsoleColor.DarkBlue, ConsoleColor.White);
                            break;
                        case 3:
                            ApplyTheme(ConsoleColor.DarkRed, ConsoleColor.White);
                            break;
                        case 4:
                            ApplyTheme(ConsoleColor.White, ConsoleColor.Black);
                            break;

                        default:
                            Console.WriteLine("  Invalid theme option!");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("  Invalid input format!");
                }
            }

            private void ApplyTheme(ConsoleColor backgroundColor, ConsoleColor foregroundColor)
            {
                Console.BackgroundColor = backgroundColor;
                Console.ForegroundColor = foregroundColor;
                Console.Clear();
                Console.WriteLine("  Colors updated successfully!");
            }

            // GAMES CODE
            public void PlayGuessTheNumber()
            {
                Console.WriteLine("  Welcome to Guess the Number Game!");
                Console.WriteLine("  I'm thinking of a number between 1 and 50. Try to guess it!");

                Random rand = new Random();
                int secretNumber = rand.Next(1, 51);
                int attempts = 0;
                int guess = 0;

                while (guess != secretNumber)
                {
                    Console.Write("  Enter your guess [1-50]: ");
                    if (!int.TryParse(Console.ReadLine(), out guess))
                    {
                        Console.WriteLine("  Invalid input. Please enter a number.");
                        continue;
                    }

                    attempts++;

                    if (guess < secretNumber)
                    {
                        Console.WriteLine("  Too low! Try a higher number.");
                    }
                    else if (guess > secretNumber)
                    {
                        Console.WriteLine("  Too high! Try a lower number.");
                    }
                    else
                    {
                        Console.WriteLine($"  Congratulations! You guessed the number {secretNumber} in {attempts} attempts.");
                    }
                }
            }

            public void RockPaperScissorsGame()
            {
                Console.WriteLine("  Welcome to Rock-Paper-Scissors! Play up to 3 rounds!");
                int userWins = 0;
                int computerWins = 0;

                for (int round = 1; round <= 3; round++)
                {
                    Console.WriteLine($"  \nRound {round}");
                    Console.WriteLine("  Choose your move:");
                    Console.WriteLine("      1. Rock");
                    Console.WriteLine("      2. Paper");
                    Console.WriteLine("      3. Scissors");
                    Console.Write("  Enter your choice (1-3): ");

                    if (int.TryParse(Console.ReadLine(), out int playerChoice) && playerChoice >= 1 && playerChoice <= 3)
                    {
                        Random random = new Random();
                        int computerChoice = random.Next(1, 4);

                        space();
                        Console.WriteLine($"  Your choice: {GetChoiceName(playerChoice)}");
                        Console.WriteLine($"  Computer's choice: {GetChoiceName(computerChoice)}");

                        DetermineWinner(playerChoice, computerChoice, ref userWins, ref computerWins);
                    }
                    else
                    {
                        Console.WriteLine("  Invalid input. Please choose a number between 1 and 3.");
                        round--;
                    }
                }

                Console.WriteLine("  \nGame Over!");

                if (userWins > computerWins)
                {
                    space();
                    Console.WriteLine("  Congratulations! You win the match!");
                }
                else if (userWins < computerWins)
                {
                    space();
                    Console.WriteLine("  Computer wins the match!");
                }
                else
                {
                    space();
                    Console.WriteLine("  There's no winner in this match!");
                }
            }

            private string GetChoiceName(int choice)
            {
                return choice switch
                {
                    1 => "Rock",
                    2 => "Paper",
                    3 => "Scissors",
                    _ => "Unknown"
                };
            }

            private void DetermineWinner(int userChoice, int computerChoice, ref int userWins, ref int computerWins)
            {
                if ((userChoice == 1 && computerChoice == 3) ||
                    (userChoice == 2 && computerChoice == 1) ||
                    (userChoice == 3 && computerChoice == 2))
                {
                    space();
                    Console.WriteLine("  Congratulations! You win this round!");
                    userWins++;
                }
                else if ((computerChoice == 1 && userChoice == 3) ||
                         (computerChoice == 2 && userChoice == 1) ||
                         (computerChoice == 3 && userChoice == 2))
                {
                    space();
                    Console.WriteLine("  Computer wins this round!");
                    computerWins++;
                }
                else
                {
                    space();
                    Console.WriteLine("  It's a tie for this round!");
                }
            }

            private void SignOut()
            {
                isLoggedIn = false;
                Cosmos.System.Power.Reboot();
            }

            private void ClearCommands()
            {
                Console.Clear();
                Console.WriteLine("  Commands Cleared");
            }

     
            //FILE SYSTEM 

            private void ShowFile()
            {
                var savedFiles = fileSystem.GetFiles();
                if (savedFiles.Count > 0)
                {
                    Console.WriteLine("  \nSaved Files:");
                    foreach (var file in savedFiles)
                    {
                        Console.WriteLine($"  File: {file.Name}");
                    }
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("  \nNo files saved.\n");
                }
            }

            private void CreateFile()
            {
                Console.Write("  Enter the file name: ");
                var fileName = Console.ReadLine();

                if (fileSystem.FileExists(fileName))
                {
                    Console.WriteLine($"  File '{fileName}' already exists. Choose a different name.");
                    return;
                }

                Console.WriteLine("  Enter the content of the file (type 'SAVE' on a new line to save):");
                var content = new StringBuilder();
                string line;
                while (true)
                {
                    line = Console.ReadLine();
                    if (line.ToUpper() == "SAVE")
                    {
                        break; // Exit the loop when "SAVE" is entered
                    }

                    content.AppendLine(line);
                }

                fileSystem.WriteFile(fileName, content.ToString().TrimEnd());
                Console.WriteLine($"  File '{fileName}' created and saved.");
            }

            private void ReadFile()
            {
                Console.Write("  Enter the file name to read: ");
                var fileName = Console.ReadLine();

                var content = fileSystem.ReadFile(fileName);
                if (content != null)
                {
                    Console.WriteLine($"  Content of '{fileName}':");
                    Console.WriteLine(content);
                }
                else
                {
                    Console.WriteLine($"  File '{fileName}' not found.");
                }
            }

            private void ShowMemoryFile()
            {
                Console.WriteLine("  \nFiles in Memory:");

                var savedFiles = fileSystem.GetFiles();
                if (savedFiles.Count > 0)
                {
                    foreach (var file in savedFiles)
                    {
                        Console.WriteLine($"  File: {file.Name}, Content: {file.Content}");
                    }
                }
                else
                {
                    Console.WriteLine("  No files saved in memory.");
                }
            }
            private void EditFile()
            {
                Console.Write("  Enter the file name to edit: ");
                var fileName = Console.ReadLine();

                var content = fileSystem.ReadFile(fileName);
                if (content != null)
                {
                    Console.WriteLine($"  Current content of '{fileName}':");
                    Console.WriteLine(content);

                    Console.WriteLine("  Enter the new content (type 'SAVE' on a new line to save and exit):");
                    var newContent = new StringBuilder();
                    string line;
                    while (true)
                    {
                        line = Console.ReadLine();
                        if (line.ToUpper() == "SAVE")
                            break;

                        newContent.AppendLine(line);
                    }

                    fileSystem.WriteFile(fileName, newContent.ToString().TrimEnd());
                    Console.WriteLine($"  File '{fileName}' edited and saved.");
                }
                else
                {
                    Console.WriteLine($"  File '{fileName}' not found.");
                }
            }

            private void DeleteFile()
            {
                Console.Write("  Enter the file name to delete: ");
                var fileName = Console.ReadLine();

                if (fileSystem.FileExists(fileName))
                {
                    fileSystem.DeleteFile(fileName);
                    Console.WriteLine($"  File '{fileName}' deleted.");
                }
                else
                {
                    Console.WriteLine($"File '{fileName}' not found.");
                }
            }

            // PC INFO

        
        }

    }

     

}