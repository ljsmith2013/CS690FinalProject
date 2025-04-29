namespace CommunityCalendar;
using System;
using Spectre.Console;
using System.IO;
using System.Globalization;

public static class CommunityFlow {
    public static void Run(string[] args) {
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
                    
                    var anotherSearch = PromptHelper.YesNoPrompt("Would you like to search for another event?");

                    if (anotherSearch == "yes") {
                        continue;
                    }
                    else{
                        Program.Main(args);
                        return;
                    }   
                }
    }
}