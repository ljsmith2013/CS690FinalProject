namespace CommunityCalendar;
using System;
using System.IO;
using System.IO.Enumeration;
public static class UserFunctions {
    const string OrganizerFile = "EventOrganizers.txt";

    public static bool UserExists(string username, string fileName = OrganizerFile){
        if(!File.Exists(fileName)) return false;
        return File.ReadAllLines(fileName).Any(line => line.StartsWith(username + "|"));
    }
    public static void CreateUser(string username, string fileName = OrganizerFile) {
        File.AppendAllText(fileName, Environment.NewLine + username + "|");
        Console.WriteLine("Created new user " + username);
    }
}