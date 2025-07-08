using OfficeOpenXml; // EPPlus kütüphanesini kullanır
using SAP2000.API.models.excel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SAP2000.API.services.excel
{
    public class ExcelDataReaderService : IExcelDataReaderService
    {
        public ExcelDataReaderService()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public List<ExcelColumnData> readColumnData(string filePath)
        {
            List<ExcelColumnData> columns = new List<ExcelColumnData>();

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Error: Excel file not found at {filePath}");
                return columns;
            }

            try
            {
                using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Columns"];
                    if (worksheet == null)
                    {
                        Console.WriteLine("Error: 'Columns' worksheet not found in the Excel file.");
                        return columns;
                    }

                    int startRow = worksheet.Dimension.Start.Row;
                    int endRow = worksheet.Dimension.End.Row;

                    for (int row = startRow + 1; row <= endRow; row++) 
                    {
                        try
                        {
                            string name = worksheet.Cells[row, 1].GetValue<string>();
                            string xStr = worksheet.Cells[row, 2].GetValue<string>();
                            string yStr = worksheet.Cells[row, 3].GetValue<string>();
                            string sectionName = worksheet.Cells[row, 4].GetValue<string>();

                            if (string.IsNullOrWhiteSpace(xStr) && string.IsNullOrWhiteSpace(yStr) && string.IsNullOrWhiteSpace(sectionName))
                            {
                                continue; // Boş satırı atla
                            }

                            double x, y;

                            if (!double.TryParse(xStr, NumberStyles.Any, CultureInfo.InvariantCulture, out x) &&
                                !double.TryParse(xStr, NumberStyles.Any, CultureInfo.CurrentCulture, out x))
                            {
                                Console.WriteLine($"Uyarı: Satır {row} için geçersiz X koordinatı '{xStr}'. Satır atlanıyor.");
                                continue;
                            }

                            if (!double.TryParse(yStr, NumberStyles.Any, CultureInfo.InvariantCulture, out y) &&
                                !double.TryParse(yStr, NumberStyles.Any, CultureInfo.CurrentCulture, out y))
                            {
                                Console.WriteLine($"Uyarı: Satır {row} için geçersiz Y koordinatı '{yStr}'. Satır atlanıyor.");
                                continue;
                            }

                            if (string.IsNullOrWhiteSpace(sectionName))
                            {
                                Console.WriteLine($"Uyarı: Satır {row} için Kesit Adı boş. Satır atlanıyor.");
                                continue;
                            }

                            columns.Add(new ExcelColumnData
                            {
                                Name = name, // İsim boş olabilir, Form1'de otomatik atanacak
                                X = x,
                                Y = y,
                                SectionName = sectionName
                            });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error reading column data from row {row}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing Excel file for columns: {ex.Message}");
            }

            return columns;
        }

        public List<ExcelBeamData> readBeamData(string filePath)
        {
            List<ExcelBeamData> beams = new List<ExcelBeamData>();

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Error: Excel file not found at {filePath}");
                return beams;
            }

            try
            {
                using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Beams"];
                    if (worksheet == null)
                    {
                        Console.WriteLine("Error: 'Beams' worksheet not found in the Excel file.");
                        return beams;
                    }

                    int startRow = worksheet.Dimension.Start.Row;
                    int endRow = worksheet.Dimension.End.Row;

                    for (int row = startRow + 1; row <= endRow; row++) 
                    {
                        try
                        {
                            string name = worksheet.Cells[row, 1].GetValue<string>();
                            string startColumnName = worksheet.Cells[row, 2].GetValue<string>();
                            string endColumnName = worksheet.Cells[row, 3].GetValue<string>();
                            string sectionName = worksheet.Cells[row, 4].GetValue<string>();

                            if (string.IsNullOrWhiteSpace(startColumnName) && string.IsNullOrWhiteSpace(endColumnName) && string.IsNullOrWhiteSpace(sectionName))
                            {
                                continue; // Boş satırı atla
                            }

                            if (string.IsNullOrWhiteSpace(startColumnName) || string.IsNullOrWhiteSpace(endColumnName) || string.IsNullOrWhiteSpace(sectionName))
                            {
                                Console.WriteLine($"Uyarı: Satır {row} için Başlangıç/Bitiş Kolon Adı veya Kesit Adı boş. Satır atlanıyor.");
                                continue;
                            }

                            beams.Add(new ExcelBeamData
                            {
                                Name = name, // İsim boş olabilir, Form1'de otomatik atanacak
                                StartColumnName = startColumnName,
                                EndColumnName = endColumnName,
                                SectionName = sectionName
                            });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error reading beam data from row {row}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing Excel file for beams: {ex.Message}");
            }

            return beams;
        }
    }
}

