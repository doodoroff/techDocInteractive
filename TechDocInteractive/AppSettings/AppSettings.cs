using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace TechDocInteractive
{
    class AppSettings
    {
        FileInfo settingsFile;
        XmlDocument settingsDoc;

        public AppSettings()
        {
            settingsFile  = new FileInfo(@".\Settings.xml");
            if (settingsFile.Exists == false)
            {
                CreateSettingsDoc();
                settingsDoc = new XmlDocument();
                settingsDoc.Load(@".\Settings.xml");
            }
            else
            {
                settingsDoc = new XmlDocument();
                settingsDoc.Load(@".\Settings.xml");
                if (XmlDocumentHaveRightFormat() == false)
                {
                    throw new AppXmlAnalyzerExceptions("Неправильный формат файла настроек");
                }
            }
        }

        void CreateSettingsDoc()
        {
            XmlTextWriter xmlTextWriter = new XmlTextWriter(@".\Settings.xml", Encoding.UTF8)
            {
                Formatting = Formatting.Indented,
                IndentChar = '\t',
                Indentation = 1,
                QuoteChar = '\''
            };
            xmlTextWriter.WriteStartDocument();
            xmlTextWriter.WriteStartElement("TechDocInteractiveSettings");
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.Close();
        }

        bool XmlDocumentHaveRightFormat()
        {
            if (settingsDoc.DocumentElement.Name.Equals("TechDocInteractiveSettings"))
            {
                return true;
            }
            return false;
        }

        public void SetCurrentXmlFilePath(string xmlFilePath)
        {
            XPathNavigator navigator = settingsDoc.CreateNavigator();
            navigator.MoveToRoot();
            navigator.MoveToFirstChild();
            if (navigator.MoveToChild("CurrentXmlFilePath", ""))
            {
                navigator.SetValue(xmlFilePath);
                settingsDoc.Save(@".\Settings.xml");
            }
            else
            {
                navigator.AppendChild("<CurrentXmlFilePath>" + xmlFilePath + "</CurrentXmlFilePath>");
                settingsDoc.Save(@".\Settings.xml");
            }
        }

        public void SetToolBaseFilePath(string toolBaseFilePath, string keyName)
        {
            XPathNavigator navigator = settingsDoc.CreateNavigator();
            navigator.MoveToRoot();
            navigator.MoveToFirstChild();
            if (navigator.MoveToChild(keyName + "BaseList", ""))
            {
                navigator.AppendChild("<" + keyName + ">" + toolBaseFilePath + "</" + keyName + ">");
                settingsDoc.Save(@".\Settings.xml");
            }
            else
            {
                navigator.AppendChild("<" + keyName + "BaseList" + "></" + keyName + "BaseList" + ">");
                navigator.MoveToChild(keyName + "BaseList", "");
                navigator.AppendChild("<" + keyName + ">" + toolBaseFilePath + "</" + keyName + ">");
                settingsDoc.Save(@".\Settings.xml");
            }
        }

        public List<string> GetToolBaseFilePathList(string keyName)
        {
            return new List<string>();
        }

        public static string GetCurrentFilePath(string key)
        {
            var appSettings = ConfigurationManager.AppSettings;

            return appSettings[key] ?? "Not Found";
        }

        public static void SetCurrentFilePath(string key, string value)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;

            if (settings[key] == null)
            {
                settings.Add(key, value);
            }
            else
            {
                settings[key].Value = value;
            }
            configFile.Save();
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }
    }
}
