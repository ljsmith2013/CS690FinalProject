using System;
using System.IO;

namespace CommunityCalendar {
    public static class EventFunctions {
        const string OrganizerFile = "EventOrganizers.txt";

        public static List<string> GetUserEvents(string username, string fileName = OrganizerFile) {
            var userEvents = new List<string>();

            if(!File.Exists(fileName)) return userEvents;

            foreach(var line in File.ReadAllLines(fileName)) {
                var parts = line.Split("|");
                if(parts.Length < 5) continue;

                if(parts[0] == username) {
                    userEvents.Add(line);
                }
            }

            return userEvents;
        }

            public static void NewEvent(string username, string eventname, string date, string time, string details, string fileName = OrganizerFile) {
                var entry = username + "|" + eventname + "|" + date + "|" + time + "|" + details;
                File.AppendAllText(fileName, Environment.NewLine + entry);
                Console.WriteLine("New event " + eventname + " successfully created!");
            }
            public static List<string> GetEventsforDate (string date, string fileName = OrganizerFile){
                var dateEvents = new List<string>();
                if(!File.Exists(fileName)) return dateEvents;

                foreach(var line in File.ReadAllLines(fileName)) {
                    var parts = line.Split("|");
                    if(parts.Length < 5) continue;

                    if(parts[2] == date)
                    dateEvents.Add(line);
                }

                return dateEvents;
            }
        }
    }