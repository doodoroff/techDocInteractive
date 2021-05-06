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

        public SettingsWindow()
        {
            InitializeComponent();
            toolBaseFilePath = AppSettings.GetCurrentFilePath("CurrentFilePath");
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
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = toolBaseFilePath;
            openFileDialog.Filter = "файл .csv (*.csv) | *.csv";

            if (openFileDialog.ShowDialog() == true)
            {
                toolBaseFilePath = openFileDialog.FileName;
                textBox_BaseFilePath.Text = toolBaseFilePath;

                AppSettings.SetCurrentFilePath("CurrentFilePath", toolBaseFilePath);
            }
            else
            {
                MessageBox.Show("Файл не найден");
            }
        }

        private void OpenInsertBaseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = toolBaseFilePath;
            openFileDialog.Filter = "файл .xlsx (*.xlsx) | *.xlsx";

            if (openFileDialog.ShowDialog() == true)
            {
                insertBaseFilePath = openFileDialog.FileName;
                textbox_InsertFilePath.Text = insertBaseFilePath;

                AppSettings.SetCurrentFilePath("CurrentInsertBasePath", insertBaseFilePath);
            }
            else
            {
                if (!openInsertBaseButton.IsEnabled)
                {
                    MessageBox.Show("Файл не найден");
                }
            }
        }

        private void OpenColletBaseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = colletBaseFilePath;
            openFileDialog.Filter = "файл .xlsx (*.xlsx) | *.xlsx";

            if (openFileDialog.ShowDialog() == true)
            {
                colletBaseFilePath = openFileDialog.FileName;
                textbox_ColletFilePath.Text = colletBaseFilePath;

                AppSettings.SetCurrentFilePath("CurrentColletBasePath", colletBaseFilePath);

            }
            else
            {
                MessageBox.Show("Файл не найден");
            }
        }

        private void OpenMillBaseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = colletBaseFilePath;
            openFileDialog.Filter = "файл .xlsx (*.xlsx) | *.xlsx";

            if (openFileDialog.ShowDialog() == true)
            {
                millBaseFilePath = openFileDialog.FileName;
                textbox_MillFilePath.Text = millBaseFilePath;

                AppSettings.SetCurrentFilePath("CurrentMillBasePath", millBaseFilePath);

                openMillBaseButton.IsEnabled = true;
            }
            else
            {
                 MessageBox.Show("Файл не найден");
            }
        }
    }
}
