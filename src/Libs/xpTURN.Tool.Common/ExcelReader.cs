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
            this.column = column;
            this.row = row;

            return this;
        }
    }
    
    public class ExcelReader
    {
        protected System.Data.DataTable SheetData { get; set; }
        public string fileName = string.Empty;

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

            fileName = string.Empty;
        }

        public bool OpenTableSheet(string _fileName, string _sheetName)
        {
            InitOpen();
            fileName = _fileName;
            Logger.Log.Tool.File(_fileName);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            try
            {
                using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
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
                                    SheetData = table;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception error)
            {
                Logger.Log.Tool.Error(DebugInfo.Empty, $"Exception, {fileName}\n{error.Message}");
                return false;
            }

            if (SheetData == null)
            {
                Logger.Log.Tool.Error(DebugInfo.Empty, $"SheetData is null, {fileName}");
                return false;
            }

            //
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
            Logger.Log.Tool.Line(cell.row.ToString());

            return GetCellString(cell.column, cell.row);
        }

        public string GetCellString(int x, int y)
        {
            Logger.Log.Tool.Line(y.ToString());

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
            Logger.Log.Tool.Line(cell.row.ToString());

            return GetTrimCellString(cell.column, cell.row);
        }

        public string GetTrimCellString(int x, int y)
        {
            Logger.Log.Tool.Line(y.ToString());

            string oCell = GetCellString(x, y);
            if (oCell == null)
                return "";

            return oCell.ToString().Trim();
        }

        public object GetCell(DataCell cell)
        {
            Logger.Log.Tool.Line(cell.row.ToString());

            return GetCell(cell.column, cell.row);
        }

        public object GetCell(int x, int y)
        {
            Logger.Log.Tool.Line(y.ToString());

            if (SheetData == null)
            {
                Logger.Log.Tool.Error(DebugInfo.Empty, $"SheetData is null, {fileName}");
                return null;
            }

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
