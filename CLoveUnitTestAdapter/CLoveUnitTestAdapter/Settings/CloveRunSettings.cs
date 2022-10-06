using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace CLoveUnitTestAdapter.Core
{
    public class CloveRunSettings
    {
        public static CloveRunSettings FromDiscovery(IDiscoveryContext discoveryContext, IXLogger logger)
        {
            return From(discoveryContext.RunSettings.SettingsXml, null, logger);
        }

        public static CloveRunSettings FromExecution(IRunContext runContext, IXLogger logger)
        {
            return From(runContext.RunSettings.SettingsXml, runContext.TestRunDirectory, logger);
        }

        public static CloveRunSettings From(string settingsXml, string testResultDirOrNull, IXLogger logger)
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.LoadXml(settingsXml);
            }
            catch (XmlException e) {
                logger.Erro("RunSettings: Adapter disabled because failed parsing 'runsettings' xml!");
                logger.Trace(e.Message);
                logger.Trace(settingsXml);
                return Default();
            }

            XmlNode cloveNode = xml.SelectSingleNode("RunSettings/CLoveUnit"); //This tag make the adapter enabled by default
            if (cloveNode == null)
            {
                logger.Debug("Missing <CLoveUnit> tag. Adapter will be disabled!");
                return Default();
            }

            bool adapterEnabled = GetBoolFromNode(cloveNode, "AdapterEnabled", true); //Optional
            if (!adapterEnabled)
            {
                logger.Warn("RunSettings: Adapter disabled because 'RunSettings/CLoveUnit/AdapterEnabled' is set to 'false'!");
                return Default();
            }

            string testResultPath = testResultDirOrNull;
            if (testResultPath == null) { 
                //Scenario: DiscoverTests called by UI TestExplorer 
                //          where ResultsDirectory is automatically provided by test framework
                XmlNode resNode = xml.SelectSingleNode("RunSettings/RunConfiguration/ResultsDirectory");
                if (resNode == null)
                {
                    logger.Erro("RunSettings: Something went wrong...missing 'ResultsDirectory'.it should be provided by MSTest framework!");
                    logger.Info("To bypass this issue, you could try to configure a directory path for this 'runsettings' node: RunSettings/RunConfiguration/ResultsDirectory");
                    return Default();
                }
                testResultPath = resNode.InnerText;
            }

            bool debugEnabled = GetBoolFromNode(cloveNode, "DebugEnabled", false); //optional
            string ExecNameRegexStr = GetStringFromNode(cloveNode, "ExecNameRegex", ""); //mandatory
            if (string.IsNullOrEmpty(ExecNameRegexStr))
            {
                logger.Erro("RunSettings: Adapter disabled because 'RunSettings/CLoveUnit/ExecNameRegex' is empty or missing!");
                return Default();
            }

            //Regex validation
            Regex ExecNameRegex;
            try {
                ExecNameRegex = new Regex(ExecNameRegexStr);
            } catch (Exception e)
            {
                logger.Erro("RunSettings: Adapter disabled because 'ExecNameRegex' is not a valid regex! Regex must follow C# regex rules!");
                logger.Trace(e.Message);
                return Default();
            }

            CloveRunSettings settings = new CloveRunSettings();
            settings.AdapterEnabled = adapterEnabled;
            settings.TestResultPath = testResultPath;
            settings.DebugEnabled = debugEnabled;
            //settings.DotVsPath = Path.Combine(settings.TestResultPath, CloveConfig.DotVsDirName);
            settings.CachePath = Path.Combine(settings.TestResultPath, CloveConfig.CacheBaseDirName);
            settings.ExecNameRegex = ExecNameRegex;
            return settings;
        }

        public static CloveRunSettings Default() {
            CloveRunSettings settings = new CloveRunSettings();
            settings.AdapterEnabled = false;
            settings.CachePath = null;
            settings.DebugEnabled = false;
            //settings.DotVsPath = null;
            settings.TestResultPath = null;
            settings.ExecNameRegex = null;
            return settings;
        }

        private static bool GetBoolFromNode(XmlNode node, string xpath, bool defaultValue)
        {
            string boolStr = node.SelectSingleNode(xpath)?.InnerText;
            if (boolStr == null) return defaultValue;
            if(!bool.TryParse(boolStr, out bool boolValue)) return defaultValue;
            return boolValue;
        }

        private static string GetStringFromNode(XmlNode node, string xpath, string defaultValue)
        {
            string str = node.SelectSingleNode(xpath)?.InnerText;
            if (str == null) return defaultValue;
            return str;
        }

        public string TestResultPath { get; internal set; }
        public bool DebugEnabled { get; internal set; }
        //public string DotVsPath { get; internal set; }
        public string CachePath { get; internal set; }
        public bool AdapterEnabled { get; set; }
        public Regex ExecNameRegex { get; set; }
    }
}
