namespace CommunityCalendar;
using System;
using Spectre.Console;
using System.IO;
using System.Globalization;

public static class OrganizerFlow {
    public static void Run(string[] args) {

        var user = NewOrExisting();
        if (user == null) return;

        var action = PromptHelper.SelectNextAction();
        switch(action) {
            case "view":    ShowEvents(user);   break;
            case "create":  CreateEventFlow(user);  break;
            case "edit/delete": ManageEvents(user); break;
            case "back": return;
        }
    }

    static string? NewOrExisting() {

        var userStatus = PromptHelper.SelectUserStatus();

        if(userStatus == "back")
        return null;

        string enteredUser;

        if (userStatus == "existing") {
            while (true) {
                Console.Write("Please enter your username:");
                enteredUser = Console.ReadLine()!;

            if (UserFunctions.UserExists(enteredUser)) {
                Console.WriteLine("Welcome Back! " + enteredUser);
                break;
            }
            
            else {
                Console.WriteLine("Sorry, the username " + enteredUser +" does not exist.");
                var userChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("Would you like to create a profile with that username, try again, or go back to the main menu?")
                .PageSize(10)
                .AddChoices("create", "try again", "back"));

                if (userChoice == "create") {
                    UserFunctions.CreateUser(enteredUser);
                    break;
                    }

                if (userChoice == "back") {
                    return null;  
                }
                }
            }
        }

        else {
            while(true) {
                Console.WriteLine("Enter the username you would like to use:");
                enteredUser = Console.ReadLine()!;

                if(UserFunctions.UserExists(enteredUser)) {
                    var againOrback = PromptHelper.YesNoPrompt("Sorry, that username is already in use. Would you like to try again? (no will return you to the main menu)");

                    if(againOrback == "no")
                        return null;
                }

                else {
                    UserFunctions.CreateUser(enteredUser);
                    break;
                }
            }
        }
        return enteredUser;
    }

    static void ShowEvents(string username) {
        var userLines = EventFunctions.GetUserEvents(username);
        foreach(var line in userLines) {
            var eachEvent = line.Split("|");
            string recordedDT = eachEvent[2] + " " + eachEvent[3];
            if(!DateTime.TryParse(recordedDT, out var dt))
                dt = DateTime.MinValue;
            Console.WriteLine("\n" + "Event Name: " + eachEvent[1] + "\n" + "When: " + dt.ToString("MM-dd-yyyy hh:mm tt") + "\n" + "Event Details: "+ eachEvent[4]);
        }
    }
    static void ManageEvents(string username) {
        var userLines = EventFunctions.GetUserEvents(username);

        if (userLines.Count == 0) {
            var noeventsaction = PromptHelper.YesNoPrompt("You currently have no events. Would you like to create a new event? (no will return you to the main menu)");
            if(noeventsaction == "yes")
                CreateEventFlow(username);
                return;
        }

        var displayedChoices = userLines
                .Select((line, idx) => {
                    var parts = line.Split("|");
                    return idx+1 + parts[1];
                })
                .ToList();

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
                }

                else if(editordelete == "modify") {
                    var parts = userLines[selectionIndex].Split("|", 5);
                    var options = parts.ToArray();
                    while(true) {
                        var fieldtoedit = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                            .Title("Select a field to update, or save changes and exit to main menu.")
                            .AddChoices(
                                "Event Name: " + options[1],
                                "Event Date: " + options[2],
                                "Event Time: " + options[3],
                                "Event Details: " + options[4],
                                "save and exit")
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
                }
            }

    static void CreateEventFlow(string username) {
        Console.WriteLine("Add a new event: ");
        Console.Write("Event Name: ");
        var eventName = Console.ReadLine()!;

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
        var eventDetails = Console.ReadLine()!;

        EventFunctions.NewEvent(username, eventName, eventDate, eventTime, eventDetails);
    }
}