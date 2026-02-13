using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using ExcelDataReader;

using xpTURN.Common;

namespace xpTURN.Tool.Common
{
    public struct DataCell
    {
        public int column;
        public int row;

        public DataCell(int column, int row)
        {
            this.column = column;
            this.row = row;
        }

        public DataCell Cell(int column, int row)
        {
            return new DataCell(column, row);
        }
    }
    
    public class ExcelReader
    {
        protected System.Data.DataTable SheetData { get; set; }
        public string FileName { get; set; } = string.Empty;

        public int LastX { get; set; }
        public int LastY { get; set; }

        public static string CellName(DataCell cell)
        {
            return CellName(cell.column, cell.row);
        }

        public static string CellName(int x)
        {
            int div = x;
            string result = string.Empty;
            int mod = 0;

            int numeral = 'Z' - 'A' + 1; //진수(26)

            while (div > 0)
            {
                mod = (div - 1) % numeral;
                result = (char)('A' + mod) + result;
                div = (div - (mod + 1)) / numeral;
            }

            return result;
        }

        public static string CellName(int x, int y)
        {
            return CellName(x) + y;
        }

        public void InitOpen()
        {
            Close();

            FileName = string.Empty;
        }

        public bool OpenTableSheet(string _fileName, string _sheetName)
        {
            InitOpen();
            FileName = _fileName;
            Logger.Log.Tool.File(_fileName);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            try
            {
                using (var stream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var reader = ExcelReaderFactory.CreateOpenXmlReader(stream))
                    {
                        if (reader == null)
                            return false;

                        var dataSetConfig = new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = false
                            }
                        };

                        using (var result = reader.AsDataSet(dataSetConfig))
                        {
                            for (int i = 0; i < result.Tables.Count; ++i)
                            {
                                var table = result.Tables[i];
                                if (table.TableName == _sheetName)
                                {
                                    SheetData = table.Copy();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception error)
            {
                Logger.Log.Tool.Error(DebugInfo.Empty, $"Exception, {FileName}\n{error.Message}");
                return false;
            }

            if (SheetData == null)
            {
                Logger.Log.Tool.Error(DebugInfo.Empty, $"SheetData is null, {FileName}");
                return false;
            }

            // Update sheet dimensions.
            LastX = SheetData.Columns.Count;
            LastY = SheetData.Rows.Count;

            return true;
        }

        //Excel Error values
        static readonly HashSet<string> ExcelErrors = new HashSet<string> {
            "-2146826281", // #Div/0!
            "-2146826246", // #N/A
            "-2146826259", // #Name?
            "-2146826288", // #Null!
            "-2146826252", // #Num!
            "-2146826265", // #Ref!
            "-2146826273"  // #Value!
        };

        public string GetCellString(DataCell cell)
        {
            return GetCellString(cell.column, cell.row);
        }

        public string GetCellString(int x, int y)
        {
            object oCell = GetCell(x, y);
            if (oCell == null)
                return "";

            string strValue = oCell.ToString();
            if (ExcelErrors.Contains(strValue))
            {
                Logger.Log.Tool.Error($"Invalid Value : {strValue}");
            }

            return strValue;
        }

        public string GetTrimCellString(DataCell cell)
        {
            return GetTrimCellString(cell.column, cell.row);
        }

        public string GetTrimCellString(int x, int y)
        {
            return (GetCellString(x, y) ?? "").Trim();
        }

        public object GetCell(DataCell cell)
        {
            return GetCell(cell.column, cell.row);
        }

        public object GetCell(int x, int y)
        {
            if (SheetData == null)
            {
                Logger.Log.Tool.Error(DebugInfo.Empty, $"SheetData is null, {FileName}");
                return null;
            }

            if (x < 1 || y < 1 || x > LastX || y > LastY)
                return null;

            return SheetData.Rows[y - 1][x - 1];
        }

        public void Close()
        {
            Logger.Log.Tool.File(string.Empty);

            var temp = SheetData;
            SheetData = null;

            if (temp != null)
            {
                temp.Clear();
                temp.Dispose();
            }
        }
    }
}
