using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Spectre.Console;

namespace TerminalChat
{
    internal class MainView
    {
        private readonly IConfiguration _config;

        internal MainView(IConfiguration config)
        {
            _config = config;
        }

        internal void ShowChat()
        {
            var chatViewTable = new Table();
            /* chatViewTable.Add() */

            AnsiConsole.Live(chatViewTable)
                .Start(async ctx => 
                {
                    chatViewTable.AddColumn("Friends");
                    ctx.Refresh();
                    await Task.Delay(500);

                    chatViewTable.AddColumn("");
                    ctx.Refresh();
                    await Task.Delay(500);
                });
        }
    }
}
