using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace TechDocInteractive
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        string toolBaseFilePath;

        List<string> insertBaseFilePaths;
        List<string> colletBaseFilePaths;
        List<string> millBaseFilePaths;
        List<string> drillBaseFilePaths;

        AppSettings appSettings;

        public SettingsWindow()
        {
            InitializeComponent();

            appSettings = new AppSettings();
            
            toolBaseFilePath = appSettings.GetSingleFilePath("spCamToolBase");
            insertBaseFilePaths = appSettings.GetToolBaseFilePathList("inserts");
            colletBaseFilePaths = appSettings.GetToolBaseFilePathList("collets");
            millBaseFilePaths = appSettings.GetToolBaseFilePathList("mills");
            drillBaseFilePaths = appSettings.GetToolBaseFilePathList("drills");

            textBox_BaseFilePath.Text = toolBaseFilePath;

            DisplayInfo();
        }

        void DisplayInfo()
        {
            UpdatePathDataGrid(insertPathsList, insertBaseFilePaths);
            UpdatePathDataGrid(millPathsList, millBaseFilePaths);
            UpdatePathDataGrid(colletPathsList, colletBaseFilePaths);
            UpdatePathDataGrid(drillPathsList, drillBaseFilePaths);
        }

        void UpdatePathDataGrid(DataGrid dataGrid, List<string> pathList)
        {
            if (pathList == null) 
            {
                dataGrid.ItemsSource = null;
            }
            else
            {
                List<SettingsContainer> newPathsList = new List<SettingsContainer>();
                foreach (var item in pathList)
                {
                    SettingsContainer container = new SettingsContainer();
                    container.SourcePath = item;
                    newPathsList.Add(container);
                }
                dataGrid.ItemsSource = newPathsList;
            }
        }

        private void OpenToolBaseButton_Click(object sender, RoutedEventArgs e)
        {
            toolBaseFilePath = GetFilePath("csv", toolBaseFilePath);
            textBox_BaseFilePath.Text = toolBaseFilePath;
        }

        string GetFilePath(string extension, string initialDirectory)
        {
            string path = @"C:\";

            OpenFileDialog openFileDialog = new OpenFileDialog();
            try
            {
                path = System.IO.Path.GetDirectoryName(initialDirectory);
            }
            catch (Exception)
            {
                path = @"C:\";
            }
            openFileDialog.InitialDirectory = path;
            openFileDialog.Filter = "файл ." + extension + " (*." + extension + ") | *." + extension;

            if (openFileDialog.ShowDialog() == true)
            {
                if (openFileDialog.CheckFileExists)
                {
                    return openFileDialog.FileName;
                }
                else
                {
                    MessageBox.Show("Файл не найден");
                    return initialDirectory;
                }
            }
            else
            {
                return initialDirectory;
            }
        }

        private void OpenInsertBaseButton_Click(object sender, RoutedEventArgs e)
        {
            AddPathToList(ref insertBaseFilePaths);
            DisplayInfo();
        }

        void AddPathToList(ref List<string> pathList)
        {
            string initialDirectory;
            if (pathList.Count == 0) 
            {
                initialDirectory = @"C:\";
            }
            else
            {
                initialDirectory = pathList.Last();
            }

            string filePath = GetFilePath("xlsx", initialDirectory);
            pathList.Add(filePath);
        }

        private void OpenColletBaseButton_Click(object sender, RoutedEventArgs e)
        {
            AddPathToList(ref colletBaseFilePaths);
            DisplayInfo();
        }

        private void OpenMillBaseButton_Click(object sender, RoutedEventArgs e)
        {
            AddPathToList(ref millBaseFilePaths);
            DisplayInfo();
        }

        private void OpenDrillBaseButton_Click(object sender, RoutedEventArgs e)
        {
            AddPathToList(ref drillBaseFilePaths);
            DisplayInfo();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            InsertToSettingsFile();
            this.Close();
        }

        void InsertToSettingsFile()
        {
            appSettings.SetSingleFilePath(toolBaseFilePath, "spCamToolBase");
            appSettings.RewriteToolBaseFilePathList(insertBaseFilePaths, "inserts");
            appSettings.RewriteToolBaseFilePathList(millBaseFilePaths, "mills");
            appSettings.RewriteToolBaseFilePathList(colletBaseFilePaths, "collets");
            appSettings.RewriteToolBaseFilePathList(drillBaseFilePaths, "drills");
        }

        private void CanselButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RemoveInsertBaseButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveBaseNote(sender, insertBaseFilePaths);
        }

        void RemoveBaseNote(object sender, List<string> pathList)
        {
            Button button = sender as Button;
            SettingsContainer pathContainer = button.DataContext as SettingsContainer;
            string path = pathContainer.SourcePath;
            pathList.Remove(path);
            DisplayInfo();
        }

        private void RemoveMillBaseButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveBaseNote(sender, millBaseFilePaths);
        }

        private void RemoveColletBaseButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveBaseNote(sender, colletBaseFilePaths);
        }

        private void RemoveDrillBaseButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveBaseNote(sender, drillBaseFilePaths);
        }
    }
}
