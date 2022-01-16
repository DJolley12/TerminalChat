using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Spectre.Console;
using TerminalChatLib;

namespace TerminalChat
{
    internal class ConfigureView
    {
        private readonly IConfiguration _config;

        public ConfigureView(IConfiguration config)
        {
            _config = config;
        }

        public void ConfigurationSetup()
        {

            var configRule = new Rule("\n\nConfiguration\n\n");
            configRule.Style = Style.Parse("lightsteelblue3 bold");
            configRule.Centered();
            configRule.DoubleBorder();
            AnsiConsole.Write(configRule);

            AnsiConsole.MarkupLine("\n\n[green bold underline]Looks like you haven't configured yet, follow these prompts to setup[/]\n\n");

            /* var types = new string[] { "Whitelist", "Blacklist" }; */
            
            /* SettingsHelpers.AddOrUpdateAppSetting(types, "ListenMethodTypes"); */
            /* SettingsHelpers.AddOrUpdateAppSetting("ListenMethodTypes", types.ToList()); */
            SetPort();
            var listenType = SetListenMethodType();
            if (listenType == ListenMethodType.Blacklist)
            {
                SetIPList("[bold]Enter all the IP Addresses you want to allow:[/]", "IPBlackList");
            }
            else if (listenType == ListenMethodType.Whitelist)
            {
                SetIPList("[bold]Enter all the IP Addresses you want to allow:[/]", "IPWhiteList");
            }
            SetConnectionPassphrase();
            SettingsHelpers.AddOrUpdateAppSetting<bool>("IsConfigured", true);
        }

        private void SetConnectionPassphrase()
        {
            var setPassPhrase = ConsoleComponents.YesNoPrompt("[bold]You additionally have the option to setup a passphrase which will need to be sent with any incoming friend/unfavorited chat requests. Do you wish to set one now?[/]\n[yellow1 dim](If not, all chat requests will prompt you for an answer)[/]");

            if (setPassPhrase)
            {
                var passphrase = ConsoleComponents.TextSecret("Enter passphrase:");

                if (!String.IsNullOrWhiteSpace(passphrase))
                {
                    SettingsHelpers.AddOrUpdateAppSetting<string>("ConnectionPassphrase", passphrase);
                }
            }
        }

        private void SetIPList(string message, string dest)
        {
            var isConfiguring = true;
            var ipAddresses = new List<string>();
            
            while (isConfiguring)
            {
                var ip = ConsoleComponents.PromptIPArray(message);

                if (string.IsNullOrWhiteSpace(ip))
                {
                    isConfiguring = ConsoleComponents.YesNoPrompt("[bold]Are you done adding IP Addresses?[/]");
                }
                else
                {
                    ipAddresses.Add(ip);
                }

            }

            SettingsHelpers.AddOrUpdateAppSetting(ipAddresses, dest);
        }

        private ListenMethodType SetListenMethodType()
        {
            var setWhiteOrBlacklist = ConsoleComponents.YesNoPrompt("[bold]Do you want to set an ip whitelist and/or blacklist?[/]");

            var listenMethodType = "AllowAll";

            if (setWhiteOrBlacklist)
            {
                listenMethodType = ConsoleComponents.SingleReturnSelection("[bold]Select one of the following options:[/]", new string[] { "Whitelist", "Blacklist", "Cancel" });

                listenMethodType = listenMethodType == "Cancel" ? "AllowAll" : listenMethodType;
            }

            SettingsHelpers.AddOrUpdateAppSetting<string>("ListenMethodType", listenMethodType);

            return Helpers.ParseListenMethodType(listenMethodType);
;
        }

        private void SetPort()
        {
            var pickPort = ConsoleComponents.YesNoPrompt("[bold]Do you want to specify a port?[/] [yellow1 dim](Select no to continue with default settings.)[/]");
            var port = 0;

            if (pickPort)
            {
                port = AnsiConsole.Prompt<int>(
                        new TextPrompt<int>("[bold]Enter a port number.[/] [yellow1 dim](If the port is taken, the port number will automatically increment until finding a valid port)[/]")
                            .InvalidChoiceMessage("[red] Invalid port. Must be a number[/]")
                            .DefaultValue(0));
            }

            SettingsHelpers.AddOrUpdateAppSetting<int>("Port", port);
        }
    }
}
