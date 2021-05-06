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
        string insertBaseFilePath;
        string colletBaseFilePath;
        string millBaseFilePath;
        WaitWindow waitWindow;

        ToolInfoPresenter toolInfoPresenter;
        List<ToolInfo> currentTools;

        public MainWindow()
        {
            InitializeComponent();
            xmlFilePath = @"C:\"; //AppSettings.GetCurrentFilePath("CurrentXmlFilePath");
            toolBaseFilePath = AppSettings.GetCurrentFilePath("CurrentFilePath");
            insertBaseFilePath = AppSettings.GetCurrentFilePath("CurrentInsertBasePath");
            colletBaseFilePath = AppSettings.GetCurrentFilePath("CurrentColletBasePath");
            millBaseFilePath = AppSettings.GetCurrentFilePath("CurrentMillBasePath");
            
            if (toolBaseFilePath != null)
            {
                button_openFile.IsEnabled = true;
            }
            textBox_BaseFilePath.Text = toolBaseFilePath;

            textbox_InsertFilePath.Text = insertBaseFilePath;

            textbox_ColletFilePath.Text = colletBaseFilePath;

            textbox_MillFilePath.Text = millBaseFilePath;
        }

        private void Button_openFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = xmlFilePath;// @"C:\";
            openFileDialog.Filter = "файл проекта .xml (*.xml) | *.xml";

            if (openFileDialog.ShowDialog() == true)
            {
                xmlFilePath = openFileDialog.FileName;
                textBox_output.Clear();
                AppSettings.SetCurrentFilePath("CurrentXmlFilePath", xmlFilePath);
                try
                {
                    //DisplayOperationInfo();
                    //DisplayToolsInfo();
                    toolInfoGrid.Visibility = Visibility.Visible;
                    textBox_output.Visibility = Visibility.Visible;
                    excelReportButton.IsEnabled = true;
                    refreshButton.IsEnabled = true;
                    DisplayInfo();
                }
                catch (AppXmlAnalyzerExceptions ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Файл не найден");
            }
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

                button_openFile.IsEnabled = true;
            } 
            else
            {
                if (!button_openFile.IsEnabled)
                {
                    MessageBox.Show("Файл не найден");
                }
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

                openInsertBaseButton.IsEnabled = true;
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

                openColletBaseButton.IsEnabled = true;
            }
            else
            {
                if (!openColletBaseButton.IsEnabled)
                {
                    MessageBox.Show("Файл не найден");
                }
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
                if (!openColletBaseButton.IsEnabled)
                {
                    MessageBox.Show("Файл не найден");
                }
            }
        }

        void DisplayInfo()
        {
            toolInfoPresenter = new ToolInfoPresenter(toolBaseFilePath, xmlFilePath, insertBaseFilePath, colletBaseFilePath, millBaseFilePath);

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
            toolInfoGrid.CanUserAddRows = false;
        }

        void DisplayOperationInfo(ToolInfoPresenter toolInfoPresenter)
        {
            //ToolInfoPresenter toolInfoPresenter = new ToolInfoPresenter(toolBaseFilePath, xmlFilePath, insertBaseFilePath);

            Operation operationInfo = toolInfoPresenter.OperationDescription;
            textBox_output.Text += "Деталь: " + operationInfo.DetailName + "\n";
            textBox_output.Text += "Станок: " + operationInfo.MachinetoolName + "\n";
            textBox_output.Text += "\n";

            foreach (var setup in operationInfo.Setups)
            {
                textBox_output.Text += "Установ: " + setup.SetupName + "\n";
                textBox_output.Text += "Тпрог. (время обработки) = " + setup.SetupMachiningTime.ToString("0.00");
                textBox_output.Text += "\n";
                int shiftNumber = 0;
                foreach (var shift in setup.Shifts)
                {
                    shiftNumber++;
                    textBox_output.Text += "Переход " + shiftNumber + ":" + "\n";
                    textBox_output.Text += "Тпрог. (время обработки) = " + shift.GetMachiningTime().ToString("0.00") + "\n";
                    textBox_output.Text += "Инструмент: " + shift.Tool.Type + " " + shift.Tool.Name + "\n";
                    textBox_output.Text += shift.ShiftDescription;
                    textBox_output.Text += "\n"; 
                }
                textBox_output.Text += "\n";
            }
        }

        void OutputToolInfoIntoExcel(ExcelRedactor excelRedactor)
        {
            Operation operationInfo = toolInfoPresenter.OperationDescription;

            excelRedactor.SetCellValue("Инструмент для обработки детали " +
                                        operationInfo.DetailName +
                                        " на станке " +
                                        operationInfo.MachinetoolName,
                                      1, 1);
            excelRedactor.MergeCells(1, 1, 1, 2);
            excelRedactor.SetBorderStyle(1, 1, 1, 2, ReportBorderStyle.Medium);

            int rowIndex = 2;

            foreach (var currentTool in currentTools)
            {
                int blockStartRowIndex = rowIndex;

                excelRedactor.SetCellValue("Инструмент:", rowIndex, 1);
                excelRedactor.SetBorderStyle(rowIndex, 1, ReportBorderStyle.Thin);
                excelRedactor.SetCellValue(currentTool.SourceToolName, rowIndex, 2);
                excelRedactor.SetBorderStyle(rowIndex, 2, ReportBorderStyle.Thin);
                rowIndex++;
                if (currentTool.CurrentInsert1 != null)
                {
                    excelRedactor.SetCellValue("Пластина:", rowIndex, 1);
                    excelRedactor.SetBorderStyle(rowIndex, 1, ReportBorderStyle.Thin);
                    excelRedactor.SetCellValue(currentTool.CurrentInsert1, rowIndex, 2);
                    excelRedactor.SetBorderStyle(rowIndex, 2, ReportBorderStyle.Thin);
                    rowIndex++;
                }
                if (currentTool.CurrentInsert2 != null) 
                {
                    excelRedactor.SetCellValue("Пластина центр. :", rowIndex, 1);
                    excelRedactor.SetBorderStyle(rowIndex, 1, ReportBorderStyle.Thin);
                    excelRedactor.SetCellValue(currentTool.CurrentInsert2, rowIndex, 2);
                    excelRedactor.SetBorderStyle(rowIndex, 2, ReportBorderStyle.Thin);
                    rowIndex++;
                }
                excelRedactor.SetCellValue("Диаметр:", rowIndex, 1);
                excelRedactor.SetBorderStyle(rowIndex, 1, ReportBorderStyle.Thin);
                excelRedactor.SetCellValue(currentTool.SourceToolDiametr.ToString(), rowIndex, 2);
                excelRedactor.SetBorderStyle(rowIndex, 2, ReportBorderStyle.Thin);
                rowIndex++;
                excelRedactor.SetCellValue("Длина инструмента:", rowIndex, 1);
                excelRedactor.SetBorderStyle(rowIndex, 1, ReportBorderStyle.Thin);
                excelRedactor.SetCellValue(currentTool.SourceToolLength.ToString(), rowIndex, 2);
                excelRedactor.SetBorderStyle(rowIndex, 2, ReportBorderStyle.Thin);
                rowIndex++;
                excelRedactor.SetCellValue("Длина режущей части:", rowIndex, 1);
                excelRedactor.SetBorderStyle(rowIndex, 1, ReportBorderStyle.Thin);
                excelRedactor.SetCellValue(currentTool.SourceCuttingLength.ToString(), rowIndex, 2);
                excelRedactor.SetBorderStyle(rowIndex, 2, ReportBorderStyle.Thin);
                rowIndex++;
                if (currentTool.SourceCutRadius != 0)
                {
                    excelRedactor.SetCellValue("Радиус на кромке:", rowIndex, 1);
                    excelRedactor.SetBorderStyle(rowIndex, 1, ReportBorderStyle.Thin);
                    excelRedactor.SetCellValue(currentTool.SourceCutRadius.ToString(), rowIndex, 2);
                    excelRedactor.SetBorderStyle(rowIndex, 2, ReportBorderStyle.Thin);
                    rowIndex++;
                }
                if (currentTool.SourceCutAngle != 0) 
                {
                    excelRedactor.SetCellValue("Угол кромки:", rowIndex, 1);
                    excelRedactor.SetBorderStyle(rowIndex, 1, ReportBorderStyle.Thin);
                    excelRedactor.SetCellValue(currentTool.SourceCutAngle.ToString(), rowIndex, 2);
                    excelRedactor.SetBorderStyle(rowIndex, 2, ReportBorderStyle.Thin);
                    rowIndex++;
                }
                excelRedactor.SetCellValue("Кол-во зубов/пластин:", rowIndex, 1);
                excelRedactor.SetBorderStyle(rowIndex, 1, ReportBorderStyle.Thin);
                excelRedactor.SetCellValue(currentTool.SourceNumberOfTeeth.ToString(), rowIndex, 2);
                excelRedactor.SetBorderStyle(rowIndex, 2, ReportBorderStyle.Thin);
                rowIndex++;
                excelRedactor.SetCellValue("Вылет инструмента:", rowIndex, 1);
                excelRedactor.SetBorderStyle(rowIndex, 1, ReportBorderStyle.Thin);
                excelRedactor.SetCellValue(currentTool.SourceToolOverhang.ToString(), rowIndex, 2);
                excelRedactor.SetBorderStyle(rowIndex, 2, ReportBorderStyle.Thin);
                rowIndex++;
                excelRedactor.SetCellValue("Оправка:", rowIndex, 1);
                excelRedactor.SetBorderStyle(rowIndex, 1, ReportBorderStyle.Thin);
                excelRedactor.SetCellValue(currentTool.SourceHolderName, rowIndex, 2);
                excelRedactor.SetBorderStyle(rowIndex, 2, ReportBorderStyle.Thin);
                if (currentTool.CurrentCollet != null)
                {
                    rowIndex++;
                    excelRedactor.SetCellValue("Цанга:", rowIndex, 1);
                    excelRedactor.SetBorderStyle(rowIndex, 1, ReportBorderStyle.Thin);
                    excelRedactor.SetCellValue(currentTool.CurrentCollet, rowIndex, 2);
                    excelRedactor.SetBorderStyle(rowIndex, 2, ReportBorderStyle.Thin);
                }

                excelRedactor.SetBorderStyle(rowIndex + 1, 1, ReportBorderStyle.Thin);
                excelRedactor.SetBorderStyle(rowIndex + 1, 2, ReportBorderStyle.Thin);
                excelRedactor.SetBorderStyle(blockStartRowIndex, 1, rowIndex + 1, 2, ReportBorderStyle.Medium);
                rowIndex += 2;
            }
            excelRedactor.CellAutoFormat();
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

        private void ColletComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox insertComboBox = sender as ComboBox;
            ToolInfo toolInfo = insertComboBox.DataContext as ToolInfo;
            int toolId = toolInfo.ToolPosition;
            foreach (var tool in currentTools)
            {
                if (tool.ToolPosition.Equals(toolId))
                {
                    tool.CurrentCollet = insertComboBox.SelectedItem as string;
                }
            }
        }

        private void ExcelReportButton_Click(object sender, RoutedEventArgs e)
        {
            ExcelRedactor excelRedactor = new ExcelRedactor();
            OutputToolInfoIntoExcel(excelRedactor);

            byte[] bin = excelRedactor.CloseAndPackAsByteArray();

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "файл .xlsx (*.xlsx) | *.xlsx";

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllBytes(saveFileDialog.FileName, bin);
                openExcelReportFile(saveFileDialog.FileName, true);
            }
        }

        void openExcelReportFile(string filePath, bool visible)
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
                textBox_output.Visibility = Visibility.Visible;
                DisplayInfo();
            }
            catch (AppXmlAnalyzerExceptions ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}