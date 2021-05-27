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
        string insertBaseFilePath;
        string colletBaseFilePath;
        string millBaseFilePath;

        AppSettings appSettings;

        public SettingsWindow()
        {
            InitializeComponent();

            appSettings = new AppSettings();

            toolBaseFilePath = AppSettings.GetCurrentFilePath("CurrentToolBaseFilePath");
            insertBaseFilePath = AppSettings.GetCurrentFilePath("CurrentInsertBasePath");
            colletBaseFilePath = AppSettings.GetCurrentFilePath("CurrentColletBasePath");
            millBaseFilePath = AppSettings.GetCurrentFilePath("CurrentMillBasePath");

            textBox_BaseFilePath.Text = toolBaseFilePath;

            textbox_InsertFilePath.Text = insertBaseFilePath;

            textbox_ColletFilePath.Text = colletBaseFilePath;

            textbox_MillFilePath.Text = millBaseFilePath;
        }

        private void OpenToolBaseButton_Click(object sender, RoutedEventArgs e)
        {
            toolBaseFilePath = GetFilePath("csv", toolBaseFilePath);
            textBox_BaseFilePath.Text = toolBaseFilePath;
            appSettings.SetToolBaseFilePath(toolBaseFilePath, "spCamToolBase");
        }

        string GetFilePath(string extension, string initialDirectory)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string path = System.IO.Path.GetDirectoryName(initialDirectory);
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
            insertBaseFilePath = GetFilePath("xlsx", insertBaseFilePath);
            textbox_InsertFilePath.Text = insertBaseFilePath;
        }

        private void OpenColletBaseButton_Click(object sender, RoutedEventArgs e)
        {
            colletBaseFilePath = GetFilePath("xlsx", colletBaseFilePath);
            textbox_ColletFilePath.Text = colletBaseFilePath;
        }

        private void OpenMillBaseButton_Click(object sender, RoutedEventArgs e)
        {
            millBaseFilePath = GetFilePath("xlsx", millBaseFilePath);
            textbox_MillFilePath.Text = millBaseFilePath;
        }

        private void OpenDrillBaseButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            AppSettings.SetCurrentFilePath("CurrentToolBaseFilePath", toolBaseFilePath);
            AppSettings.SetCurrentFilePath("CurrentInsertBasePath", insertBaseFilePath);
            AppSettings.SetCurrentFilePath("CurrentColletBasePath", colletBaseFilePath);
            AppSettings.SetCurrentFilePath("CurrentMillBasePath", millBaseFilePath);
            this.Close();
        }

        private void CanselButton_Click(object sender, RoutedEventArgs e)
        {
            toolBaseFilePath = AppSettings.GetCurrentFilePath("CurrentToolBaseFilePath");
            insertBaseFilePath = AppSettings.GetCurrentFilePath("CurrentInsertBasePath");
            colletBaseFilePath = AppSettings.GetCurrentFilePath("CurrentColletBasePath");
            millBaseFilePath = AppSettings.GetCurrentFilePath("CurrentMillBasePath");
            this.Close();
        }
    }
}
