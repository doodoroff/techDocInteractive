using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechDocInteractive
{
    class ExcelReport
    {
        ExcelRedactor excelReport;
        Operation operationDescription;
        List<ToolInfo> tools;

        public ExcelReport(Operation operationDescription, List<ToolInfo> tools)
        {
            this.excelReport = new ExcelRedactor();
            this.operationDescription = operationDescription;
            this.tools = tools;
        }

        void PrepareExcelReport()
        {
            Operation operationInfo = operationDescription;

            excelReport.SetCellValue("Инструмент для обработки детали " +
                                        operationInfo.DetailName +
                                        " на станке " +
                                        operationInfo.MachinetoolName,
                                      1, 1);
            excelReport.MergeCells(1, 1, 1, 2);
            excelReport.SetBorderStyle(1, 1, 1, 2, ReportBorderStyle.Medium);

            int rowIndex = 2;

            foreach (var currentTool in tools)
            {
                int blockStartRowIndex = rowIndex;

                SetToolInfoCellString("Инструмент:", currentTool.SourceToolName, rowIndex, 1);
                rowIndex++;

                if (currentTool.CurrentInsert1 != null)
                {
                    SetToolInfoCellString("Пластина:", currentTool.CurrentInsert1, rowIndex, 1);
                    rowIndex++;
                }
                if (currentTool.CurrentInsert2 != null)
                {
                    SetToolInfoCellString("Пластина центр. :", currentTool.CurrentInsert2, rowIndex, 1);
                    rowIndex++;
                }

                SetToolInfoCellString("Диаметр:", currentTool.SourceToolDiametr.ToString(), rowIndex, 1);
                rowIndex++;

                SetToolInfoCellString("Длина инструмента:", currentTool.SourceToolLength.ToString(), rowIndex, 1);
                rowIndex++;

                SetToolInfoCellString("Длина режущей части:", currentTool.SourceCuttingLength.ToString(), rowIndex, 1);
                rowIndex++;

                if (currentTool.SourceCutRadius != 0)
                {
                    SetToolInfoCellString("Радиус на кромке:", currentTool.SourceCutRadius.ToString(), rowIndex, 1);
                    rowIndex++;
                }
                if (currentTool.SourceCutAngle != 0)
                {
                    SetToolInfoCellString("Угол кромки:", currentTool.SourceCutAngle.ToString(), rowIndex, 1);
                    rowIndex++;
                }

                SetToolInfoCellString("Кол-во зубов/пластин:", currentTool.SourceNumberOfTeeth.ToString(), rowIndex, 1);
                rowIndex++;

                SetToolInfoCellString("Вылет инструмента:", currentTool.SourceToolOverhang.ToString(), rowIndex, 1);

                if (currentTool.SourceAuxToolsSpecification != null)
                {
                    int holderIndex;
                    if (currentTool.SourceAuxToolsSpecification.Count > 1)
                    {
                        holderIndex = 1;
                    }
                    else
                    {
                        holderIndex = 0;
                    }

                    foreach (AuxToolInfo auxTool in currentTool.SourceAuxToolsSpecification)
                    {
                        rowIndex++;
                        SetToolInfoCellString("Держатель " + ConvertHolderInderx(holderIndex) + ":", auxTool.AuxToolName, rowIndex, 1);

                        if (auxTool.AuxToolOverhang != 0)
                        {
                            rowIndex++;
                            SetToolInfoCellString("Вылет держателя " + ConvertHolderInderx(holderIndex) + ":", auxTool.AuxToolOverhang.ToString(), rowIndex, 1);
                        }

                        if (auxTool.CurrentHub != null) 
                        {
                            rowIndex++;
                            SetToolInfoCellString("Зажимная вставка держателя " + ConvertHolderInderx(holderIndex) + ":", auxTool.CurrentHub, rowIndex, 1);
                        }

                        holderIndex++;
                    }
                }

                excelReport.SetBorderStyle(rowIndex + 1, 1, ReportBorderStyle.Thin);
                excelReport.SetBorderStyle(rowIndex + 1, 2, ReportBorderStyle.Thin);
                excelReport.SetBorderStyle(blockStartRowIndex, 1, rowIndex + 1, 2, ReportBorderStyle.Medium);
                rowIndex += 2;
            }
            excelReport.CellAutoFormat();
        }

        void SetToolInfoCellString(string header, string text, int row, int column)
        {
            excelReport.SetCellValue(header, row, column);
            excelReport.SetBorderStyle(row, column, ReportBorderStyle.Thin);
            excelReport.SetCellValue(text, row, column + 1);
            excelReport.SetBorderStyle(row, column + 1, ReportBorderStyle.Thin);
        }

        string ConvertHolderInderx(int index)
        {
            if (index != 0) 
            {
                return index.ToString();
            }
            else
            {
                return "";
            }
        }

        public ExcelRedactor GenerateExcelReport()
        {
            PrepareExcelReport();
            return excelReport;
        }
    }
}
