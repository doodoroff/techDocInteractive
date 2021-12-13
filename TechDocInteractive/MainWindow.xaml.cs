using System;
using System.Collections;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.IO;
using System.Xml;
using Microsoft.Win32;
using System.Xml.XPath;
using Excel = Microsoft.Office.Interop.Excel;

namespace TechDocInteractive
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string xmlFilePath;
        string toolBaseFilePath;
        List<string> insertBaseFilePaths;
        List<string> colletBaseFilePaths;
        List<string> millBaseFilePaths;
        List<string> drillBaseFilePaths;
        WaitWindow waitWindow;
        ToolInfoPresenter toolInfoPresenter;
        List<ToolInfo> currentTools;
        AppSettings appSettings;

        public MainWindow()
        {
            InitializeComponent();

            appSettings = new AppSettings();

            //xmlFilePath = AppSettings.GetCurrentFilePath("CurrentXmlFilePath");
            xmlFilePath = appSettings.GetSingleFilePath("CurrentXmlFilePath");
            toolBaseFilePath = appSettings.GetSingleFilePath("spCamToolBase");
            insertBaseFilePaths = appSettings.GetToolBaseFilePathList("inserts");
            colletBaseFilePaths = appSettings.GetToolBaseFilePathList("collets");
            millBaseFilePaths = appSettings.GetToolBaseFilePathList("mills");
            drillBaseFilePaths = appSettings.GetToolBaseFilePathList("drills");
            /*toolBaseFilePath = AppSettings.GetCurrentFilePath("CurrentToolBaseFilePath");
            insertBaseFilePath = AppSettings.GetCurrentFilePath("CurrentInsertBasePath");
            colletBaseFilePath = AppSettings.GetCurrentFilePath("CurrentColletBasePath");
            millBaseFilePath = AppSettings.GetCurrentFilePath("CurrentMillBasePath");*/
            
            if (toolBaseFilePath != null)
            {
                button_openFile.IsEnabled = true;
            }
        }

        private void Button_openFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string path = System.IO.Path.GetDirectoryName(xmlFilePath);
            openFileDialog.InitialDirectory = path;
            openFileDialog.Filter = "файл проекта .xml (*.xml) | *.xml";

            if (openFileDialog.ShowDialog() == true)
            {
                xmlFilePath = openFileDialog.FileName;
                //textBox_output.Clear();
                //AppSettings.SetCurrentFilePath("CurrentXmlFilePath", xmlFilePath);
                try
                {
                    //DisplayOperationInfo();
                    //DisplayToolsInfo();
                    toolInfoGrid.Visibility = Visibility.Visible;
                    //textBox_output.Visibility = Visibility.Visible;
                    excelReportButton.IsEnabled = true;
                    refreshButton.IsEnabled = true;
                    spCamDoc_Tabs.Visibility = Visibility.Visible;
                    DisplayInfo();
                }
                catch (AppXmlAnalyzerExceptions ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                //MessageBox.Show("Файл не найден");
            }
        }

        void DisplayInfo()
        {
            toolInfoPresenter = new ToolInfoPresenter(toolBaseFilePath, 
                                                      xmlFilePath, 
                                                      insertBaseFilePaths, 
                                                      colletBaseFilePaths, 
                                                      millBaseFilePaths, 
                                                      drillBaseFilePaths);

            toolInfoPresenter.FlagInsertDataProcessingStart += ShowWaitWindow;
            toolInfoPresenter.FlagInsertDataProcessingStop += CloseWaitWindow;

            toolInfoPresenter.InitializeAndStartDataProcessing();

            DisplayOperationInfo(toolInfoPresenter);
            DisplayToolsInfo(toolInfoPresenter);
            currentTools = toolInfoPresenter.ProjectToolsInfo;
        }

        void ShowWaitWindow(object sender, EventArgs e)
        {
            waitWindow = new WaitWindow();
            waitWindow.Show();
        }

        void CloseWaitWindow(object sender, EventArgs e)
        {
            waitWindow.Close();
        }

        void DisplayToolsInfo(ToolInfoPresenter toolInfoPresenter)
        {
            //ToolInfoPresenter toolInfoPresenter = new ToolInfoPresenter(toolBaseFilePath, xmlFilePath, insertBaseFilePath);

            toolInfoGrid.ItemsSource = toolInfoPresenter.ProjectToolsInfo;
            
            //auxToolInfoGrid.ItemsSource=toolInfoPresenter.ProjectToolsInfo
            toolInfoGrid.CanUserAddRows = false;

        }

        void DisplayOperationInfo(ToolInfoPresenter toolInfoPresenter)
        {
            Operation operationInfo = toolInfoPresenter.OperationDescription;
            detailName_textBox.Text = operationInfo.DetailName;
            detailMachinetool_textBlock.Text = operationInfo.MachinetoolName;
            detailMachineTime_textBox.Text = operationInfo.OperationMachiningTime;
            detailAuxTime_textBlock.Text = operationInfo.OperationAuxiliaryTime;
            detailOpTime_textBlock.Text = operationInfo.OperationCommonTime;
            technologyInfoGrid.ItemsSource = operationInfo.Setups;
        }

        private void InsertComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox insertComboBox = sender as ComboBox;

            ToolInfo toolInfo = insertComboBox.DataContext as ToolInfo;
            int toolId = toolInfo.ToolPosition;
            foreach (var tool in currentTools)
            {
                if (tool.ToolPosition.Equals(toolId))
                {
                    tool.CurrentInsert1 = insertComboBox.SelectedItem as string;
                }
            }
        }

        private void InsertComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox insertComboBox = sender as ComboBox;
            ToolInfo toolInfo = insertComboBox.DataContext as ToolInfo;
            int toolId = toolInfo.ToolPosition;
            foreach (var tool in currentTools)
            {
                if (tool.ToolPosition.Equals(toolId))
                {
                    tool.CurrentInsert2 = insertComboBox.SelectedItem as string;
                }
            }
        }

        private void HubBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox hubComboBox = sender as ComboBox;
            AuxToolInfo auxToolInfo = hubComboBox.DataContext as AuxToolInfo;
            int auxToolHashCode = auxToolInfo.GetHashCode();
            foreach (ToolInfo tool in currentTools)
            {
                List<AuxToolInfo> currentAuxToolInfoList = tool.SourceAuxToolsSpecification;
                if (currentAuxToolInfoList != null) 
                {
                    foreach (AuxToolInfo auxTool in currentAuxToolInfoList)
                    {
                        if (auxTool.GetHashCode() == auxToolHashCode)
                        {
                            auxTool.CurrentHub = hubComboBox.SelectedItem as string;
                        }
                    }
                }
            }
        }

        private void ExcelReportButton_Click(object sender, RoutedEventArgs e)
        {
            ExcelReport excelReport = new ExcelReport(toolInfoPresenter.OperationDescription, currentTools);
            ExcelRedactor reportToOutput = excelReport.GenerateExcelReport();

            byte[] bin = reportToOutput.CloseAndPackAsByteArray();

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "файл .xlsx (*.xlsx) | *.xlsx";

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllBytes(saveFileDialog.FileName, bin);
                OpenExcelReportFile(saveFileDialog.FileName, true);
            }
        }

        void OpenExcelReportFile(string filePath, bool visible)
        {
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook workbook = excelApp.Workbooks.Open(Filename: filePath);
            excelApp.Visible = visible;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                toolInfoGrid.Visibility = Visibility.Visible;
                //textBox_output.Visibility = Visibility.Visible;
                DisplayInfo();
            }
            catch (AppXmlAnalyzerExceptions ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }

        private void LeftMenuTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl currentTab = sender as TabControl;
            TabItem currentTabItem = currentTab.SelectedItem as TabItem;
            string selectedTabName = currentTabItem.Header as string;

            switch (selectedTabName)
            {
                case " SprutCAM Док. ":
                    {
                        if (toolInfoPresenter != null)
                        {
                            spCamDoc_Tabs.Visibility = Visibility.Visible;
                        }
                        break;
                    }
                case " Инструмент + ":
                    {
                        spCamDoc_Tabs.Visibility = Visibility.Collapsed;
                        break;
                    }
                default:
                    break;
            }
        }
    }
}