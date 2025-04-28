namespace CommunityCalendar;
using System;
using Spectre.Console;
using System.IO;
using System.Net;
using System.Globalization;

public class Program
{
    public static void Main(string[] args) {
        var mode = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
        .Title("Select Mode: event organizer or community member. Select exit to leave the application")
        .PageSize(10)
        .AddChoices(new[] {
            "event organizer", "community member", "exit",
        }));

        if(mode == "exit") {
            return;
        }

        if (mode == "event organizer") {
            var userStatus = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("Are you a new or existing user? Select back to return to the main menu.")
            .PageSize(10)
            .AddChoices(new[] { "new", "existing", "back",
            }));

            if(userStatus == "back") {
                Main(args);
                return;
            }

            string enteredUser;

            if (userStatus == "existing") {
                while (true) {
                    Console.Write("Please enter your username:");
                    enteredUser = Console.ReadLine();

                if (UserFunctions.UserExists(enteredUser)) {
                    Console.WriteLine("Welcome Back! " + enteredUser);
                    break;
                }
                
                else {
                    Console.WriteLine("Sorry, the username" + enteredUser +"does not exist.");
                    var userChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("Would you like to create a profile with that username, try again, or go back to the main menu?")
                    .PageSize(10)
                    .AddChoices(new[] { "create", "try again", "back"
                    }));

                    if (userChoice == "create") {
                        UserFunctions.CreateUser(enteredUser);
                        break;
                        }

                    if (userChoice == "back") {
                        Main(args);
                        return;  
                    }
                    }
                }
            }

            else {
                while(true) {
                    Console.WriteLine("Enter the username you would like to use:");
                    enteredUser = Console.ReadLine();

                    if(UserFunctions.UserExists(enteredUser)) {
                        var againOrback = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                        .Title("Sorry, that username is already in use. Would you like to try again, or go back to the main menu?")
                        .PageSize(10)
                        .AddChoices(new[] { "try again", "back", 
                        }));

                        if(againOrback == "try again") continue;

                        if(againOrback == "back") {
                            Main(args);
                            return;
                        }
                    }
                    else {
                        UserFunctions.CreateUser(enteredUser);
                        break;
                    }
                }
            }

            var nextAction = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("Would you like to view your events, edit/delete your existing events, create a new event, or go back to the main menu?")
            .PageSize(10)
            .AddChoices(new[] { "view", "edit/delete", "create", "back", 
            }));

            if(nextAction == "back") {
                Main(args);
                return;
            }

            if (nextAction == "view") {
                var userLines = EventFunctions.GetUserEvents(enteredUser);
                foreach(var line in userLines) {
                    var eachEvent = line.Split("|");
                        string recordedDT = eachEvent[2] + " " + eachEvent[3];
                        if(!DateTime.TryParse(recordedDT, out var dt))
                            dt = DateTime.MinValue;
                        Console.WriteLine("\n" + "Event Name: " + eachEvent[1] + "\n" + "When: " + dt.ToString("MM-dd-yyyy hh:mm tt") + "\n" + "Event Details: "+ eachEvent[4]);
                    }
                Main(args);
                return;
                }
            if (nextAction == "edit/delete") {
                var userLines = EventFunctions.GetUserEvents(enteredUser);

                var displayedChoices = userLines
                .Select((line, idx) => {
                    var parts = line.Split("|");
                    return idx+1 + parts[1];
                })
                .ToList();

                if(displayedChoices.Count == 0) {
                    var noevents = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                        .Title("You have no events, would you like to create one or return to the main menu?")
                        .AddChoices(new[]{"create an event", "return to main menu", })
                    );

                    if(noevents == "create an event") {
                        Console.WriteLine("Add a new event: ");
                        Console.Write("Event Name: ");
                        var eventName = Console.ReadLine();
                        DateTime eventDateValid;
                        while(true) {
                            Console.Write("Event Date (MM/DD/YYYY): ");
                            var userDate = Console.ReadLine();
                            if(DateTime.TryParseExact(userDate, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out eventDateValid)) {
                                if(eventDateValid.Date >= DateTime.Today) {
                                    break;
                                }

                                Console.WriteLine("Date must be today or a future date. Please enter a differet date.");
                            }

                            else {
                                Console.WriteLine("Invalid format. Please use MM/dd/yyyy format to enter the date you would like to search.");
                            }
                            }
                        var eventDate = eventDateValid.ToString("MM/dd/yyyy");

                        DateTime eventTimeValid;
                        while(true) {
                        Console.Write("Event Time (hh:mm tt - ex: 12:00 PM): ");
                        var userTime = Console.ReadLine();

                        if(DateTime.TryParseExact(userTime, "h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out eventTimeValid)) {
                            break;
                        }

                        Console.WriteLine("Please enter a valid time in the format hh:mm tt(AM/PM)(ex: 4:45 PM)");

                        }
                        var eventTime = eventTimeValid.ToString("hh:mm tt");
                        Console.Write("Event Details: ");
                        var eventDetails = Console.ReadLine();

                        EventFunctions.NewEvent(enteredUser, eventName, eventDate, eventTime, eventDetails);

                        Main(args);
                        return;
                    }

                    else {
                        Main(args);
                        return;
                    }
                }

                var usersChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("Select which event you would like to modify or delete:")
                    .AddChoices(displayedChoices)
                );

                var selectionIndex = displayedChoices.IndexOf(usersChoice);

                var editordelete = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("Would you like to modify or delete" + usersChoice[1])
                    .AddChoices("modify", "delete", "back")
                );

                if(editordelete == "delete") {
                    var allLines = File.ReadAllLines("EventOrganizers.txt").ToList();
                    allLines.Remove(userLines[selectionIndex]);
                    File.WriteAllLines("EventOrganizers.txt", allLines);
                    Console.WriteLine("Event deleted successfully.");

                    Main(args);
                    return;
                }

                else if(editordelete == "modify") {
                    var parts = userLines[selectionIndex].Split("|", 5);
                    var options = parts.ToArray();
                    while(true) {
                        var fieldtoedit = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                            .Title("Select a field to update, or save changes and exit to main menu.")
                            .AddChoices(new[]{
                                "Event Name: " + options[1],
                                "Event Date: " + options[2],
                                "Event Time: " + options[3],
                                "Event Details: " + options[4],
                                "save and exit"})
                        );

                        if(fieldtoedit.StartsWith("Event Name: ")) {
                            Console.Write("New Event Name: ");
                            var newName = Console.ReadLine();
                            if (!string.IsNullOrEmpty(newName)) options[1] = newName;
                        }

                        else if (fieldtoedit.StartsWith("Event Date: ")) {
                            DateTime eventDateValid;
                            while(true) {
                                Console.Write("New Event Date (MM/DD/YYYY): ");
                                var userDate = Console.ReadLine();
                                if(DateTime.TryParseExact(userDate, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out eventDateValid)) {
                                    if(eventDateValid.Date >= DateTime.Today) {
                                        options[2] = eventDateValid.ToString("MM/dd/yyyy");
                                        break;
                                    }
                                    Console.WriteLine("Date must be today or a future date. Please enter a differet date.");
                                }

                                    Console.WriteLine("Invalid format. Please use MM/dd/yyyy format to enter the date you would like to search, and ensure the day is on or after today's date.");
                                }
                        }

                        else if(fieldtoedit.StartsWith("Event Time: ")) {
                            DateTime eventTimeValid;
                            while(true) {
                                Console.Write("New Event Time (hh:mm tt - ex: 12:00 PM): ");
                                var userTime = Console.ReadLine();

                                if(DateTime.TryParseExact(userTime, "h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out eventTimeValid)) {
                                    options[3] = eventTimeValid.ToString("hh:mm tt");
                                    break;
                                }
                                Console.WriteLine("Please enter a valid time in the format hh:mm tt(AM/PM)(ex: 4:45 PM)");
                            }
                        }

                        else if(fieldtoedit.StartsWith("Event Details: ")) {
                            Console.Write("New Event Details: ");
                            var newDetails = Console.ReadLine();
                            if (!string.IsNullOrEmpty(newDetails)) options[4] = newDetails;
                        }

                        else {
                            break;
                        }
                    }

                    var fileLines = File.ReadAllLines("EventOrganizers.txt").ToList();
                    var lineIdx = fileLines.IndexOf(userLines[selectionIndex]);
                    fileLines[lineIdx] = string.Join("|", options);
                    File.WriteAllLines("EventOrganizers.txt", fileLines);

                    Console.WriteLine("Event successfully updated. Returning to main menu.");
                    Main(args);
                    return;
                }
            }
            if (nextAction == "create") {
                Console.WriteLine("Add a new event: ");
                Console.Write("Event Name: ");
                var eventName = Console.ReadLine();

                DateTime eventDateValid;
                while(true) {
                    Console.Write("Event Date (MM/DD/YYYY): ");
                    var userDate = Console.ReadLine();
                    if(DateTime.TryParseExact(userDate, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out eventDateValid)) {
                        if(eventDateValid.Date >= DateTime.Today) {
                            break;
                        }

                        Console.WriteLine("Date must be today or a future date. Please enter a differet date.");
                    }

                    else {
                        Console.WriteLine("Invalid format. Please use MM/dd/yyyy format to enter the date you would like to search.");
                    }
                    }
                var eventDate = eventDateValid.ToString("MM/dd/yyyy");

                DateTime eventTimeValid;
                while(true) {
                Console.Write("Event Time (hh:mm tt - ex: 12:00 PM): ");
                var userTime = Console.ReadLine();

                if(DateTime.TryParseExact(userTime, "h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out eventTimeValid)) {
                    break;
                }

                Console.WriteLine("Please enter a valid time in the format hh:mm tt(AM/PM)(ex: 4:45 PM)");

                }
                var eventTime = eventTimeValid.ToString("hh:mm tt");
                Console.Write("Event Details: ");
                var eventDetails = Console.ReadLine();

                EventFunctions.NewEvent(enteredUser, eventName, eventDate, eventTime, eventDetails);

                Main(args);
                return;
            }
        }
        if (mode == "community member") {
            while(true) {

                DateTime enteredDate;

                while(true) {

                    Console.WriteLine("Please enter a date to see what events are happening on that day. (Enter date as MM/dd/yyyy)");
                    var input = Console.ReadLine();

                    if(DateTime.TryParseExact(input, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out enteredDate)) {
                        if(enteredDate.Date >= DateTime.Today) {
                            break;
                        }

                        Console.WriteLine("Date must be today or a future date. Please enter a differet date.");
                    }

                    else {
                        Console.WriteLine("Invalid format. Please use MM/dd/yyyy format to enter the date you would like to search.");
                    }
                    }

                    var selectedDate = enteredDate.ToString("MM/dd/yyyy");

                    bool eventsFound = false;

                    var dateLines = EventFunctions.GetEventsforDate(selectedDate);
                    foreach(var line in dateLines) {
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
                    
                    var anotherSearch = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("Would you like to search for another event or return to the main menu?")
                    .PageSize(10)
                    .AddChoices(new[] { "yes", "no, return to main menu",
                    }));

                    if (anotherSearch == "yes") {
                        continue;
                    }
                    else{
                        Main(args);
                        return;
                    }   
                }
            }
        }
    }