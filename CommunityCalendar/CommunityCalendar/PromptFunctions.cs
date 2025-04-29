namespace CommunityCalendar;
using Spectre.Console;

public static class PromptHelper {
    public static string UserMode() =>
        AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("Select Mode: event organizer or community member. Select exit to leave the application")
            .PageSize(10)
            .AddChoices("event organizer", "community member", "exit"));

    public static string SelectUserStatus() =>
        AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("Are you a new or existing user? Select back to return to the main menu.")
            .PageSize(10)
            .AddChoices( "new", "existing", "back"));

    public static string SelectNextAction() =>
        AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("Would you like to view your events, edit/delete your existing events, create a new event, or go back to the main menu?")
            .PageSize(10)
            .AddChoices("view", "edit/delete", "create", "back"));

    public static string YesNoPrompt(string question) =>
        AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title(question)
            .PageSize(10)
            .AddChoices("yes", "no"));
}