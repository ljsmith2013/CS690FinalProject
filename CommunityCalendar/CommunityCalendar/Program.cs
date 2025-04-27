namespace CommunityCalendar;
using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Please Select Mode: Event Organizer or Community Member (Type event organizer or community member, or type exit to leave the application.) ");
        string mode = Console.ReadLine();

        if(mode == "exit") {
            return;
        }

        if (mode == "event organizer") {
            Console.WriteLine("Are you a new or existing user? (type new or existing, or type back to return to the main menu)");
            var userStatus = Console.ReadLine();

            if(userStatus == "back") {
                Main(args);
                return;
            }

            string enteredUser;

            if (userStatus == "existing") {
                while (true) {
                    Console.Write("Please enter your username, or type back to return to the main menu: ");
                    enteredUser = Console.ReadLine();

                    if(enteredUser == "back") {
                        Main(args);
                        return;
                    }

                    bool userExists = false;
                    foreach(var line in File.ReadAllLines("EventOrganizers.txt")) {
                        if(line.StartsWith(enteredUser + "|")) {
                            var splitLines = line.Split("|");
                            if (splitLines[0] == enteredUser) {
                                userExists = true;
                                break;
                            }
                        } 
                    }

                if (userExists) {
                    Console.WriteLine("Welcome Back! " + enteredUser);
                    break;
                }
                
                else {
                    Console.WriteLine("Sorry, the username" + enteredUser +"does not exist.");
                    Console.WriteLine("Would you like to create a profile with that username or try again? (Type create or try again) ");
                    string userChoice = Console.ReadLine();
                    if (userChoice == "create") {
                        File.AppendAllText("EventOrganizers.txt", Environment.NewLine + enteredUser + "|");
                        Console.WriteLine("Created new user " + enteredUser);
                        break;
                        }
                    }
                }
            }

            else {
                while(true) {
                    Console.WriteLine("Enter the username you would like to use, or type back to return to the main menu: ");
                    enteredUser = Console.ReadLine();

                    if(enteredUser == "back") {
                        Main(args);
                        return;
                    }

                    bool exists = false;
                    foreach(var line in File.ReadAllLines("EventOrganizers.txt")) {
                        var splitLines = line.Split("|");
                        if (splitLines[0] == enteredUser){
                            exists = true;
                            break;
                        }
                    }
                    if(exists) {
                        Console.WriteLine(enteredUser + " already exists. Please try a different username." + "\n");
                    }
                    else {
                        File.AppendAllText("EventOrganizers.txt", Environment.NewLine + enteredUser);
                        break;
                    }
                }
            }

            Console.WriteLine("Would you like to view your existing events, or create a new one? (Type view or create, or type back to return to the main menu) ");
            string nextAction = Console.ReadLine();

            if(nextAction == "back") {
                Main(args);
                return;
            }

            if (nextAction == "view") {
                foreach (var line in File.ReadAllLines("EventOrganizers.txt")) {
                    var eachEvent = line.Split("|");
                    if (eachEvent.Length < 5) continue;

                    if (eachEvent[0] == enteredUser) {
                        string recordedDT = eachEvent[2] + " " + eachEvent[3];
                        if(!DateTime.TryParse(recordedDT, out var dt))
                            dt = DateTime.MinValue;
                        Console.WriteLine("\n" + "Event Name: " + eachEvent[1] + "\n" + "When: " + dt.ToString("MM-dd-yyyy hh:mm tt") + "\n" + "Event Details: "+ eachEvent[4]);
                    }
                }
            }
            if (nextAction == "create") {
                Console.WriteLine("Add a new event: ");
                Console.Write("Event Name: ");
                var eventName = Console.ReadLine();
                Console.Write("Event Date (MM/DD/YYYY): ");
                var eventDate = Console.ReadLine();
                Console.Write("Event Time (hh:mm tt - ex: 12:00 PM): ");
                var eventTime = Console.ReadLine();
                Console.Write("Event Details: ");
                var eventDetails = Console.ReadLine();

                var newEntry = enteredUser + "|" + eventName + "|" + eventDate + "|" + eventTime + "|" + eventDetails;

                File.AppendAllText("EventOrganizers.txt", Environment.NewLine + newEntry);
                Console.WriteLine("New event " + eventName + " successfully created!");
            }
        }
        if (mode == "community member") {
            Console.WriteLine("Please enter a date to see what events are happening on that day? (Enter date as MM/DD/YYYY, or type back to return to the main menu)");
            var selectedDate = Console.ReadLine();

            if(selectedDate == "back") {
                Main(args);
                return;
            }

            bool eventsFound = false;

            foreach(var line in File.ReadAllLines("EventOrganizers.txt")) {
                var eachEvent = line.Split('|');

                if(eachEvent.Length < 5) continue;

                if(eachEvent[2] == selectedDate)  {
                    string recordedDT = eachEvent[2] + " " + eachEvent[3];
                        if(!DateTime.TryParse(recordedDT, out var dt))
                            dt = DateTime.MinValue;
                    Console.WriteLine("Event Organizer: " + eachEvent[0] + "\n" + "Event Name: " + eachEvent[1] + "\n" + "When: " + dt.ToString("MM-dd-yyyy hh:mm tt") + "\n" + "Event Details: "+ eachEvent[4]);

                    eventsFound = true;
                }
            }
            if(!eventsFound)
                Console.WriteLine("No Events found on " + selectedDate + ".");
        }
    }
}
