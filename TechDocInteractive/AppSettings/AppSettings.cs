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

        public void SetSingleFilePath(string filePath, string keyName)
        {
            XPathNavigator navigator = CreateAndSetNavigator();

            if (navigator.MoveToChild(keyName, ""))
            {
                navigator.SetValue(filePath);
                settingsDoc.Save(@".\Settings.xml");
            }
            else
            {
                navigator.AppendChild("<" + keyName + ">" + filePath + "</" + keyName + ">");
                settingsDoc.Save(@".\Settings.xml");
            }
        }

        public string GetSingleFilePath(string keyName)
        {
            XPathNavigator navigator = CreateAndSetNavigator();

            if (navigator.MoveToChild(keyName, ""))
            {
                return navigator.Value;
            }

            return "";
        }

        XPathNavigator CreateAndSetNavigator()
        {
            XPathNavigator navigator = settingsDoc.CreateNavigator();
            navigator.MoveToRoot();
            navigator.MoveToFirstChild();
            return navigator;
        }

        public void AddToolBaseToFilePathList(string toolBaseFilePath, string keyName)
        {
            XPathNavigator navigator = CreateAndSetNavigator();

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

        public void RewriteToolBaseFilePathList(List<string> filePathList, string keyName)
        {
            XPathNavigator navigator = CreateAndSetNavigator();

            if (navigator.MoveToChild(keyName + "BaseList", "")) 
            {
                navigator.DeleteSelf();
                navigator.AppendChild("<" + keyName + "BaseList" + "></" + keyName + "BaseList" + ">");
                navigator.MoveToChild(keyName + "BaseList", "");

                foreach (string filePath in filePathList)
                {
                    navigator.AppendChild("<" + keyName + ">" + filePath + "</" + keyName + ">");
                    settingsDoc.Save(@".\Settings.xml");
                }
            }
            else
            {
                navigator.AppendChild("<" + keyName + "BaseList" + "></" + keyName + "BaseList" + ">");
                navigator.MoveToChild(keyName + "BaseList", "");

                foreach (string filePath in filePathList)
                {
                    navigator.AppendChild("<" + keyName + ">" + filePath + "</" + keyName + ">");
                    settingsDoc.Save(@".\Settings.xml");
                }
            }
        }

        public List<string> GetToolBaseFilePathList(string keyName)
        {
            List<string> filePathsList = new List<string>();
            XPathNavigator navigator = CreateAndSetNavigator();

            if (navigator.MoveToChild(keyName + "BaseList", ""))
            {
                XPathNodeIterator iterator = navigator.SelectChildren(navigator.NodeType);

                while (iterator.MoveNext())
                {
                    filePathsList.Add(iterator.Current.Value);
                }

                return filePathsList;
            }

            return new List<string>();
        }

        public void RemoveToolBaseFilePathNote(string toolBaseFilePath, string keyName)
        {
            XPathNavigator navigator = CreateAndSetNavigator();

            if (navigator.MoveToChild(keyName + "BaseList", "")) 
            {
                XPathNodeIterator iterator = navigator.SelectChildren(navigator.NodeType);

                while (iterator.MoveNext())
                {
                    if (iterator.Current.Value.Equals(toolBaseFilePath))
                    {
                        iterator.Current.DeleteSelf();
                        settingsDoc.Save(@".\Settings.xml");
                    }
                }
            }
        }

        /*public static string GetCurrentFilePath(string key)
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
        }*/
    }
}
