using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Westwind.Utilities.Configuration;

namespace PBSCAnalyzer
{
    public class App
    {

        static XmlFileConfigurationProvider<ApplicationConfiguration> provider = new XmlFileConfigurationProvider<ApplicationConfiguration>()
                                                                                 {
                                                                                     XmlConfigurationFile = "config.xml",              
                                                                                 };
        static XmlFileConfigurationProvider<UserCommandsConfiguration> userCommnadsProvider = new XmlFileConfigurationProvider<UserCommandsConfiguration>()
                                                                                 {
                                                                                     XmlConfigurationFile = @"UserCommandsConfig.xml",
                                                                                 };

        static XmlFileConfigurationProvider<UserSnippetConfiguration> userSqlSnippetsProvider = new XmlFileConfigurationProvider<UserSnippetConfiguration>()
                                                                                 {
                                                                                     XmlConfigurationFile = @"UserTextSnippetsConfig.xml",
                                                                                 };
        public static ApplicationConfiguration Configuration { get; set; }
        public static UserCommandsConfiguration UserCommandsConfiguration { get; set; }
        public static UserSnippetConfiguration UserSnippetConfiguration { get; set; }
        public static int TotalUsersnippets { get; set; }

        static App()
        {
            //Load the properties from the Config store
            Configuration = new ApplicationConfiguration();
            Configuration.Initialize(provider);

            UserCommandsConfiguration = new UserCommandsConfiguration();
            UserCommandsConfiguration.Initialize(userCommnadsProvider);

            UserSnippetConfiguration = new UserSnippetConfiguration();
            UserSnippetConfiguration.Initialize(userSqlSnippetsProvider);

            if (UserCommandsConfiguration.UserCommands.Count == 0)
            {
                UserCommandsConfiguration.UserCommands.Add(new UserCommand() { ComandCaption = "SVN Log", ComandScript = "TortoiseProc.exe /command:log /path:\"%FilePathName%\"" });
                UserCommandsConfiguration.UserCommands.Add(new UserCommand() { ComandCaption = "SVN Blame", ComandScript = "TortoiseProc.exe /command:blame /path:\" % FilePathName % \" /line:%LineNum%" });
                UserCommandsConfiguration.UserCommands.Add(new UserCommand() { ComandCaption = "Edit", ComandScript = "\"%FilePathName%\"" });
            }

            if (UserSnippetConfiguration.UserSnippets.Count == 0)
            {
                UserSnippetConfiguration.UserSnippets.Add(
                    new UserTextSnippet()
                    {
                        PlaceName = "SQL",
                        Caption = "SQL Templates",
                        IsMenu = true,
                        Text = "",
                        SubSnippets = new List<UserTextSnippet>()
                        {
                            new UserTextSnippet() {IsMenu = false, Caption = "Select * From", Text = "Select * From {1}",HotKey = "test"},
                            new UserTextSnippet()
                            {
                                IsMenu = true,
                                Caption = "vw_case_number",
                                Text = "",
                                SubSnippets = new List<UserTextSnippet>()
                                {
                                    new UserTextSnippet() {IsMenu = false, Caption = "", Text = "SELECT * FROM vw_case_number WHERE CASE_NUMBER like '{1}' "},
                                    new UserTextSnippet() {IsMenu = false, Caption = "", Text = "SELECT * FROM vw_case_number WHERE case_id = {1} "},
                                    new UserTextSnippet() {IsMenu = false, Caption = "", Text = " WHERE Case_id in (SELECT case_id FROM vw_case_number WHERE CASE_NUMBER like '{1}') "}
                                }
                            }
                        }
                    });
                UserSnippetConfiguration.UserSnippets.Add(
                    new UserTextSnippet()
                    {
                        PlaceName = "Any",
                        Caption = "PB Templates",
                        IsMenu = true,
                        Text = "",
                        SubSnippets = new List<UserTextSnippet>()
                        {
                            new UserTextSnippet() {IsMenu = false, Caption = "Item 1", Text = "Item 1"},
                            new UserTextSnippet()
                            {
                                IsMenu = true,
                                Caption = "Items 1",
                                Text = "",
                                SubSnippets = new List<UserTextSnippet>()
                                {
                                    new UserTextSnippet() {IsMenu = false, Caption = "", Text = "Sub Item 1"},                                    
                                }
                            }
                        }
                    }
                );
            }
        }

        public static void Save()
        {
            Configuration.Write();
            UserCommandsConfiguration.Write();
            UserSnippetConfiguration.Write();
        }
    }
}