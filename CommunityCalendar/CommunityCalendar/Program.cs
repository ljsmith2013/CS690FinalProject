namespace CommunityCalendar;
using System;
using Spectre.Console;
using System.IO;
using System.Net;
using System.Globalization;

public class Program
{
    public static void Main(string[] args) {
        var mode = PromptHelper.UserMode();

        if(mode == "exit") {
            return;
        }

        if (mode == "event organizer")
            OrganizerFlow.Run(args);
        
        else if (mode == "community member")
            CommunityFlow.Run(args);
        }
    }