using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Spectre.Console;

namespace TerminalChat
{
    internal class MainView
    {
        private readonly IConfiguration _config;
        private List<string> friendsList = new List<string>{ "Jerry", "David", "CommanderYeetBus69" }; 
        private string selectedFriend = "Jerry"; 
        private int selectedIndex = 0;

        internal MainView(IConfiguration config)
        {
            _config = config;
        }



        internal void ShowChat()
        {
            var chatViewTable = new Table();
            /* chatViewTable.Add() */
            chatViewTable.AddColumn("Friends");
            foreach (var friend in friendsList)
            {
                if (friend == selectedFriend)
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

        public void ReadInput()
        {
            while (true)
            {
                ShowChat();
                var input = Console.ReadKey();

                if (input.Key == ConsoleKey.J)
                {
                    selectedIndex = selectedIndex + 1 == friendsList.Count ? selectedIndex : selectedIndex + 1;
                }
                else if (input.Key == ConsoleKey.K)
                {
                    selectedIndex = selectedIndex == 0 ? selectedIndex : selectedIndex - 1;
                }

                selectedFriend = friendsList[selectedIndex];
            }
        }
    }
}
