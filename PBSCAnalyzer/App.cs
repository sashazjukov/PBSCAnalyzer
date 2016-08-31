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
        public static ApplicationConfiguration Configuration { get; set; }
        public static UserCommandsConfiguration UserCommandsConfiguration { get; set; }

        static App()
        {
            //Load the properties from the Config store
            Configuration = new ApplicationConfiguration();
            Configuration.Initialize(provider);

            UserCommandsConfiguration = new UserCommandsConfiguration();
            UserCommandsConfiguration.Initialize(userCommnadsProvider);

            if (UserCommandsConfiguration.UserCommands.Count == 0)
            {
                UserCommandsConfiguration.UserCommands.Add(new UserCommand() { ComandCaption = "SVN Log", ComandScript = "TortoiseProc.exe /command:log /path:\"%FilePathName%\"" });
                UserCommandsConfiguration.UserCommands.Add(new UserCommand() { ComandCaption = "SVN Blame", ComandScript = "TortoiseProc.exe /command:blame /path:\" % FilePathName % \" /line:%LineNum%" });
                UserCommandsConfiguration.UserCommands.Add(new UserCommand() { ComandCaption = "Edit", ComandScript = "\"%FilePathName%\"" });
            }
        }

        public static void Save()
        {
            Configuration.Write();
            UserCommandsConfiguration.Write();
        }
    }
}