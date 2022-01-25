using System;
using System.Collections.Generic;
using System.Linq;
using LINQHelpers;
using Microsoft.Extensions.Configuration;
using Spectre.Console;

namespace TerminalChat
{
    internal class MainView
    {
        private readonly IConfiguration _config;
        private List<string> friendsList { get; set; } 
        private List<string> mainOptions { get; set; }
        private string selectedItem { get; set; } 
        private int currIndex { get; set; }

        internal MainView(IConfiguration config)
        {
            _config = config;
            friendsList = _config.GetSection("SavedContacts").AsEnumerable().TakeValues<string>().ToList();
            currIndex = 0;
            ShowMainView();
        }

        internal void ShowMainView()
        {
            mainOptions = new List<string>{ "Contacts", "Configuration", "Start New Chat" };
            selectedItem = mainOptions[currIndex];

        }

        internal void ShowChat()
        {
            var chatViewTable = new Table();
            /* chatViewTable.Add() */
            chatViewTable.AddColumn("Friends");
            foreach (var friend in friendsList)
            {
                if (friend == selectedItem)
                {
                    chatViewTable.AddRow($"[cyan1 bold]{friend}[/]");
                }
                else
                {
                    chatViewTable.AddRow($"{friend}");
                }
            }

            // AnsiConsole.Live(chatViewTable)
            //     .Start(async ctx => 
            //     {
            //         lock(chatViewTable)ctx.Refresh();
            //         await Task.Delay(500);

            //         /* chatViewTable.AddColumn(""); */
            //         /* ctx.Refresh(); */
            //         /* await Task.Delay(500); */

            //     });
            AnsiConsole.Clear();
            AnsiConsole.Write(chatViewTable);
        }

        public void ReadInput(Action Refresh, List<string> options, string selectedOption, int selIndex)
        {
            Refresh();
            var input = Console.ReadKey();

            if (input.Key == ConsoleKey.J)
            {
                selIndex = selIndex + 1 == options.Count ? selIndex : selIndex + 1;
            }
            else if (input.Key == ConsoleKey.K)
            {
                selIndex = selIndex == 0 ? selIndex : selIndex - 1;
            }

            selectedOption = options[selIndex];
        }
    }
}
