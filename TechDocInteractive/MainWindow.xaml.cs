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
        WaitWindow waitWindow;

        ToolInfoPresenter toolInfoPresenter;
        List<ToolInfo> currentTools;

        public MainWindow()
        {
            InitializeComponent();
            xmlFilePath = AppSettings.GetCurrentFilePath("CurrentXmlFilePath");
            toolBaseFilePath = AppSettings.GetCurrentFilePath("CurrentFilePath");
            insertBaseFilePath = AppSettings.GetCurrentFilePath("CurrentInsertBasePath");
            colletBaseFilePath = AppSettings.GetCurrentFilePath("CurrentColletBasePath");
            
            if (toolBaseFilePath != null)
            {
                button_openFile.IsEnabled = true;
            }
            textBox_BaseFilePath.Text = toolBaseFilePath;

            textbox_InsertFilePath.Text = insertBaseFilePath;

            textbox_ColletFilePath.Text = colletBaseFilePath;
        }

        private void Button_openFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = @"C:\";
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
            openFileDialog.Filter = "файл .xls (*.xls) | *.xls|файл .xlsx (*.xlsx) | *.xls";

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
            openFileDialog.Filter = "файл .xls (*.xls) | *.xls|файл .xlsx (*.xlsx) | *.xls";

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

        /*void DisplayToolsInfo()
        {
            ToolInfoPresenter toolInfoPresenter = new ToolInfoPresenter(toolBaseFilePath, xmlFilePath);

            foreach (Tool operationTool in toolInfoPresenter.DistinctProjectTools)
            {
                textBox_output.Text += "Инструмент: " + operationTool.Type + " " + operationTool.Name + "\n";
                textBox_output.Text += "Димаметр: " + operationTool.Diametr + "\n";
                textBox_output.Text += "Длинна инструмента: " + operationTool.Length + "\n";
                textBox_output.Text += "Длинна режущей части: " + operationTool.WorkingLength + "\n";
                textBox_output.Text += "Радиус на кромке: " + operationTool.EdgeRadius + "\n";
                textBox_output.Text += "Угол кромки: " + operationTool.Angle + "\n";
                textBox_output.Text += "Кол-во зубов: " + operationTool.NumberOfTeeth + "\n";
                //textBox_output.Text += "Длинна оправки: " + operationTool.HolderLength + "\n";
                //textBox_output.Text += "Общий вылет с оправкой: " + operationTool.FullOverhang + "\n";
                textBox_output.Text += "Вылет инструмента: " + operationTool.WorkingOverhang + "\n";
                //textBox_output.Text += "Стоикость: " + operationTool.Durability + "\n";
                textBox_output.Text += "Имя оправки: " + operationTool.HolderName + "\n";
                textBox_output.Text += "Пластина: " + operationTool.InsertPattern + "\n";
                textBox_output.Text += "\n";
            }

            ShowToolInfoInExcel(toolInfoPresenter);
        }*/

        void DisplayInfo()
        {
            toolInfoPresenter = new ToolInfoPresenter(toolBaseFilePath, xmlFilePath, insertBaseFilePath, colletBaseFilePath);

            toolInfoPresenter.FlagInsertDataProcessingStart += ShowWaitWindow;
            toolInfoPresenter.FlagInsertDataProcessingStop += CloseWaitWindow;

            toolInfoPresenter.InitializeAndStartDataProcessing();

            DisplayOperationInfo(toolInfoPresenter);
            DisplayToolsInfo(toolInfoPresenter);
            currentTools = toolInfoPresenter.ProjectToolsInfo;

            excelReportButton.IsEnabled = true;
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
                textBox_output.Text += "Тв. (вспомогательное время) = " + setup.SetupAuxiliaryTime.ToString("0.00") +
                       "; " +
                       "Тобр. (время обработки) = " + setup.SetupMachiningTime.ToString("0.00") +
                       "\n";
                textBox_output.Text += "\n";
                int shiftNumber = 0;
                foreach (var shift in setup.Shifts)
                {
                    shiftNumber++;
                    textBox_output.Text += "Переход " + shiftNumber + ":" + "\n";
                    textBox_output.Text += "Тв. (вспомогательное время) = " + shift.AuxiliaryTime.ToString("0.00") +
                                          "; " +
                                          "Тобр. (время обработки) = " + shift.MachiningTime.ToString("0.00") +
                                          "\n";
                    textBox_output.Text += "Инструмент: " + shift.Tool.Type + " " + shift.Tool.Name + "\n";
                    textBox_output.Text += shift.ShiftDescription;
                    textBox_output.Text += "\n"; 
                }
                textBox_output.Text += "\n";
            }
        }

        void ShowToolInfoInExcel()
        {
            ExcelRedactor excelRedactor = new ExcelRedactor();
            excelRedactor.Visible = true;
            int rowIndex = 2;

            foreach (var currentTool in currentTools)
            {
                excelRedactor.SetCellValue("Инструмент:", rowIndex, 1);
                excelRedactor.SetCellValue(currentTool.SourceToolName, rowIndex, 2);
                rowIndex++;
                if (currentTool.CurrentInsert1 != null)
                {
                    excelRedactor.SetCellValue("Пластина:", rowIndex, 1);
                    excelRedactor.SetCellValue(currentTool.CurrentInsert1, rowIndex, 2);
                    rowIndex++;
                }
                if (currentTool.CurrentInsert2 != null) 
                {
                    excelRedactor.SetCellValue("Пластина центр. :", rowIndex, 1);
                    excelRedactor.SetCellValue(currentTool.CurrentInsert2, rowIndex, 2);
                    rowIndex++;
                }
                excelRedactor.SetCellValue("Диаметр:", rowIndex, 1);
                excelRedactor.SetCellValue(currentTool.SourceToolDiametr.ToString(), rowIndex, 2);
                rowIndex++;
                excelRedactor.SetCellValue("Длинна инструмента:", rowIndex, 1);
                excelRedactor.SetCellValue(currentTool.SourceToolLength.ToString(), rowIndex, 2);
                rowIndex++;
                excelRedactor.SetCellValue("Длинна режущей части:", rowIndex, 1);
                excelRedactor.SetCellValue(currentTool.SourceCuttingLength.ToString(), rowIndex, 2);
                rowIndex++;
                excelRedactor.SetCellValue("Радиус на кромке:", rowIndex, 1);
                excelRedactor.SetCellValue(currentTool.SourceCutRadius.ToString(), rowIndex, 2);
                rowIndex++;
                excelRedactor.SetCellValue("Угол кромки:", rowIndex, 1);
                excelRedactor.SetCellValue(currentTool.SourceCutAngle.ToString(), rowIndex, 2);
                rowIndex++;
                excelRedactor.SetCellValue("Кол-во зубов:", rowIndex, 1);
                excelRedactor.SetCellValue(currentTool.SourceNumberOfTeeth.ToString(), rowIndex, 2);
                rowIndex++;
                excelRedactor.SetCellValue("Вылет инструмента:", rowIndex, 1);
                excelRedactor.SetCellValue(currentTool.SourceToolOverhang.ToString(), rowIndex, 2);
                rowIndex++;
                excelRedactor.SetCellValue("Оправка:", rowIndex, 1);
                excelRedactor.SetCellValue(currentTool.SourceHolderName, rowIndex, 2);
                if (currentTool.CurrentCollet != null)
                {
                    rowIndex++;
                    excelRedactor.SetCellValue("Цанга:", rowIndex, 1);
                    excelRedactor.SetCellValue(currentTool.CurrentCollet, rowIndex, 2);
                }
                rowIndex += 2;
            }
            excelRedactor.CellAutoFit();
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
            ShowToolInfoInExcel();
        }


        /*void DisplayInsertsInfo()
        {
            ToolInfoPresenter toolInfoPresenter = new ToolInfoPresenter(toolBaseFilePath, xmlFilePath, insertBaseFilePath);
            int count = 0;
            foreach (var insert in toolInfoPresenter.Inserts)
            {
                textBox_output.Text += count.ToString() + " " + insert.ToolName + " " + insert.ProductionQuantity + " " + insert.StorageQuantity + "\n";
                count++;
            }
        }*/

    }
}