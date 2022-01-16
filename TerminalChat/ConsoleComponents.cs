using System.Collections.Generic;
using System.Net;
using Spectre.Console;

namespace TerminalChat
{
    internal static class ConsoleComponents
    {
        internal static bool YesNoPrompt(string promptMessage) =>
            AnsiConsole.Prompt(new SelectionPrompt<string>()
                        .Title($"{promptMessage}")
                        .PageSize(10)
                        .AddChoices(new[] {
                            "Yes", "No"
                    })) == "Yes" ? true : false;

        internal static string SingleReturnSelection(string promptMessage, string[] options) => 
            AnsiConsole.Prompt(new SelectionPrompt<string>()
                        .Title($"{promptMessage}")
                        .PageSize(10)
                        .AddChoices(options));

        internal static List<string> MultiSelection(string promptMessage, string[] options) => 
            AnsiConsole.Prompt(new MultiSelectionPrompt<string>()
                    .Title($"[bold]{promptMessage}")
                    .PageSize(10)
                    .InstructionsText(
                        "[yellow1 dim](Press [blue]<space>[/] to toggle a choice, " + 
                        "[green]<enter>[/] to accept)[/]")
                    .AddChoices(options));

        internal static string TextSecret(string promptMessage) => 
            AnsiConsole.Prompt(new TextPrompt<string>(promptMessage)
                    .PromptStyle("cyan1")
                    .Secret());

        internal static string PromptIPArray(string promptMessage) => 
            AnsiConsole.Prompt(new TextPrompt<string>(promptMessage)
                    .PromptStyle("bold")
                    .Validate(ip => {
                            var parsed = IPAddress.TryParse(ip, out IPAddress parsedIP) == true;
                            if (!parsed)
                            {
                                return false;
                            }

                            return true;
                        })
                    );
    }
}
