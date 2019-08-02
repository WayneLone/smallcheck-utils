using NPOI.HSSF.UserModel;
using NPOI.SS.Converter;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Smallcheck.Utils.Excel
{
    /// <summary>
    /// Excel帮助类
    /// </summary>
    public class ExcelHelper
    {
        #region 将List转换成excel +static byte[] ExportDataListToExcel<T>(List<T> list, List<ExcelTitle> tableHeaders, string sheetPrefix) where T : class, new()
        /// <summary>
        /// 将List转换成excel
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="list">数据集合</param>
        /// <param name="tableHeaders">表头 k--属性 v--属性显示名称</param>
        /// <param name="sheet">sheet名称</param>
        /// <returns>文件流</returns>
        public static byte[] ExportDataListToExcel<T>(List<T> list, List<ExcelTitle> tableHeaders, string sheetPrefix) where T : class, new()
        {
            HSSFWorkbook hssWorkBook = new HSSFWorkbook();  // 创建工作薄
            int sheetNums = (int)Math.Ceiling(list.Count / 50000.00);
            ICellStyle cellStyle = hssWorkBook.CreateCellStyle();
            Dictionary<string, ICellStyle> dataFormatMap = new Dictionary<string, ICellStyle>();
            for (int i = 0; i < sheetNums; i++)
            {
                ISheet sheet = hssWorkBook.CreateSheet(sheetPrefix + (i + 1));    // 创建sheet表
                // 创建表头
                IRow rowHead = sheet.CreateRow(0);
                int rowId = 0;
                foreach (var title in tableHeaders)
                {
                    ICell cell = rowHead.CreateCell(rowId);
                    cell.SetCellValue(title.Title);
                    // 设置样式 加粗居中
                    HSSFFont font = (HSSFFont)hssWorkBook.CreateFont();
                    font.Boldweight = 600;
                    cellStyle.SetFont(font);
                    cellStyle.Alignment = HorizontalAlignment.Center;
                    cell.CellStyle = cellStyle;
                    sheet.AutoSizeColumn(rowId);
                    rowId++;
                }
                rowId = 1;
                // 创建数据行
                PropertyInfo[] properties = typeof(T).GetProperties();
                // 计算每个sheet中记录数
                var pagedList = list.Skip(i * 50000).Take(50000).ToList();
                foreach (T item in pagedList)
                {
                    IRow rowBody = sheet.CreateRow(rowId);
                    int cellIndex = 0;
                    foreach (var title in tableHeaders)
                    {
                        foreach (PropertyInfo property in properties)
                        {
                            if (string.Equals(property.Name, title.PropertyName, StringComparison.CurrentCultureIgnoreCase))
                            {
                                ICell cell = rowBody.CreateCell(cellIndex);
                                FillCellValue(cell, property.GetValue(item, null));
                                if (!string.IsNullOrWhiteSpace(title.DataFormat))
                                {
                                    if (dataFormatMap.ContainsKey(title.DataFormat))
                                    {
                                        SetCellDataFormat(dataFormatMap[title.DataFormat], cell, title.DataFormat);
                                    }
                                    else
                                    {
                                        ICellStyle cellFormat = hssWorkBook.CreateCellStyle();
                                        dataFormatMap.Add(title.DataFormat, cellFormat);
                                        SetCellDataFormat(cellFormat, cell, title.DataFormat);
                                    }
                                }
                                break;
                            }
                        }
                        cellIndex++;
                    }
                    rowId++;
                }
                // 冻结首行
                sheet.CreateFreezePane(0, 1);
            }
            MemoryStream file = new MemoryStream();
            hssWorkBook.Write(file);
            return file.GetBuffer();
        }
        #endregion

        #region 将List转换成excel +static byte[] ExportDataListToExcel<T>(List<T> list, Dictionary<string, string> tableHeaders, string sheetPrefix) where T : class, new()
        /// <summary>
        /// 将List转换成excel
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="list">数据集合</param>
        /// <param name="tableHeaders">表头 k--属性 v--属性显示名称</param>
        /// <param name="sheet">sheet名称</param>
        /// <returns>文件流</returns>
        public static byte[] ExportDataListToExcel<T>(List<T> list, Dictionary<string, string> tableHeaders, string sheetPrefix) where T : class, new()
        {
            HSSFWorkbook hssWorkBook = new HSSFWorkbook();  // 创建工作薄
            int sheetNums = (int)Math.Ceiling(list.Count / 50000.00);
            for (int i = 0; i < sheetNums; i++)
            {
                ISheet sheet = hssWorkBook.CreateSheet(sheetPrefix + (i + 1));    // 创建sheet表
                // 创建表头
                IRow rowHead = sheet.CreateRow(0);
                int rowId = 0;
                foreach (KeyValuePair<string, string> kv in tableHeaders)
                {
                    ICell cell = rowHead.CreateCell(rowId);
                    cell.SetCellValue(kv.Value);
                    // 设置样式 加粗居中
                    HSSFCellStyle cellStyle = (HSSFCellStyle)hssWorkBook.CreateCellStyle();
                    HSSFFont font = (HSSFFont)hssWorkBook.CreateFont();
                    font.Boldweight = 600;
                    cellStyle.SetFont(font);
                    cellStyle.Alignment = HorizontalAlignment.Center;
                    cell.CellStyle = cellStyle;
                    rowId++;
                }
                rowId = 1;
                // 创建数据行
                PropertyInfo[] properties = typeof(T).GetProperties();
                // 计算每个sheet中记录数
                var pagedList = list.Skip(i * 50000).Take(50000).ToList();
                foreach (T item in pagedList)
                {
                    IRow rowBody = sheet.CreateRow(rowId);
                    int cellIndex = 0;
                    foreach (KeyValuePair<string, string> kv in tableHeaders)
                    {
                        foreach (PropertyInfo property in properties)
                        {
                            if (string.Equals(property.Name, kv.Key, StringComparison.CurrentCultureIgnoreCase))
                            {
                                ICell cell = rowBody.CreateCell(cellIndex);
                                FillCellValue(cell, property.GetValue(item, null));
                                break;
                            }
                        }
                        cellIndex++;
                    }
                    rowId++;
                }
                // 冻结首行
                sheet.CreateFreezePane(0, 1);
            }
            MemoryStream file = new MemoryStream();
            hssWorkBook.Write(file);
            return file.GetBuffer();
        }
        #endregion

        #region 将excel转换html table +static string ExcelToHtml(string excelPath)
        /// <summary>
        /// 将excel转换html table
        /// </summary>
        /// <param name="excelPath">excel文件物理路径</param>
        /// <returns></returns>
        public static string ExcelToHtml(string excelPath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(excelPath))
                {
                    return null;
                }
                string fileExt = Path.GetExtension(excelPath);
                if (!string.Equals(".xls", fileExt, StringComparison.CurrentCultureIgnoreCase))
                {
                    return null;
                }
                using (FileStream fs = new FileStream(excelPath, FileMode.Open, FileAccess.ReadWrite))
                {
                    HSSFWorkbook workbook = new HSSFWorkbook(fs);
                    ExcelToHtmlConverter excelToHtmlConverter = new ExcelToHtmlConverter();
                    excelToHtmlConverter.OutputColumnHeaders = false;
                    excelToHtmlConverter.OutputHiddenColumns = false;
                    excelToHtmlConverter.OutputHiddenRows = false;
                    excelToHtmlConverter.OutputLeadingSpacesAsNonBreaking = false;
                    excelToHtmlConverter.OutputRowNumbers = false;
                    excelToHtmlConverter.OutputRowNumbers = false;
                    excelToHtmlConverter.ProcessWorkbook(workbook);
                    return excelToHtmlConverter.Document.InnerXml;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region 将excel转换成DataTable +static DataTable ExcelToDataTable(byte[] excelBytes)
        /// <summary>
        /// 将excel转换成DataTable
        /// </summary>
        /// <param name="excelBytes">excel文件流字节</param>
        /// <returns></returns>
        public static DataTable ExcelToDataTable(byte[] excelBytes, string fileExt)
        {
            if (excelBytes.Length == 0 || string.IsNullOrWhiteSpace(fileExt))
            {
                return null;
            }
            List<string> allowFileExt = new List<string> { ".xls", ".xlsx" };
            if (!allowFileExt.Contains(fileExt.ToLower()))
            {
                return null;
            }
            using (MemoryStream ms = new MemoryStream(excelBytes))
            {
                // ms.Position = 0;
                IWorkbook workbook = null;
                if (string.Equals(".xls", fileExt, StringComparison.CurrentCultureIgnoreCase))
                {
                    workbook = new HSSFWorkbook(ms);
                }
                else
                {
                    workbook = new XSSFWorkbook(ms);
                }
                ISheet sheet = workbook.GetSheetAt(0);  // 取第一个表
                DataTable table = new DataTable();
                IRow headerRow = sheet.GetRow(0);   // 第一行为标题行
                int cellCount = headerRow.LastCellNum;  // LastCellNum= PhysicalNumberOfCells;
                int rowCount = sheet.LastRowNum;    // LastRowNum = PhysicalNumberOfRows - 1
                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                {
                    DataColumn column = new DataColumn(headerRow.GetCell(i) == null ? "Column" + i : headerRow.GetCell(i).StringCellValue);
                    table.Columns.Add(column);
                }
                bool flag;
                for (int i = (sheet.FirstRowNum + 1); i <= rowCount; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row != null && row.GetCell(0) != null)
                    {
                        DataRow dataRow = table.NewRow();
                        flag = false;
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {
                            if (row.GetCell(j) != null && row.GetCell(j).ToString() != "")
                            {
                                flag = true;
                                dataRow[j] = GetCellValue(row.GetCell(j));
                            }
                        }
                        //行中有数据加到table中
                        if (flag)
                            table.Rows.Add(dataRow);
                    }
                }
                return table;
            }
        }
        #endregion

        #region 获得单元格的值 -static object GetCellValue(ICell cell)
        /// <summary>
        /// 获得单元格的值
        /// </summary>
        /// <param name="cell">单元格</param>
        /// <returns></returns>
        private static object GetCellValue(ICell cell)
        {
            if (cell == null)
                return string.Empty;
            switch (cell.CellType)
            {
                case CellType.Blank:
                    return string.Empty;
                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();
                case CellType.Error:
                    return cell.ErrorCellValue.ToString();
                case CellType.Numeric:
                    return cell.NumericCellValue;
                case CellType.Unknown:

                case CellType.String:
                    return cell.StringCellValue.Trim();
                case CellType.Formula:
                    try
                    {
                        HSSFFormulaEvaluator e = new HSSFFormulaEvaluator(cell.Sheet.Workbook);
                        e.EvaluateInCell(cell);
                        return cell.ToString();
                    }
                    catch
                    {
                        return cell.NumericCellValue.ToString();
                    }
                default:
                    return cell.ToString().Trim();
                    // This is a trick to get the correct value of the cell. NumericCellValue will return a numeric value no matter the cell value is a date or a number
            }
        }
        #endregion

        #region 填充单元格值 -static void FillCellValue(ICell cell, object value)
        /// <summary>
        /// 填充单元格值
        /// </summary>
        /// <param name="cell">单元格</param>
        /// <param name="value">要填充的值</param>
        private static void FillCellValue(ICell cell, object value)
        {
            if (value == null)
            {
                cell.SetCellValue("");
                return;
            }
            switch (value.GetType().ToString())
            {
                case "System.String"://字符串类型
                    cell.SetCellValue((string)value);
                    break;
                case "System.DateTime"://日期类型
                    DateTime dateV;
                    DateTime.TryParse(value.ToString(), out dateV);
                    cell.SetCellValue(dateV.ToString("yyyy-MM-dd HH:mm:ss"));
                    break;
                case "System.Boolean"://布尔型
                    bool boolV = false;
                    bool.TryParse(value.ToString(), out boolV);
                    cell.SetCellValue(boolV);
                    break;
                case "System.Int16"://整型
                case "System.Int32":
                case "System.Int64":
                case "System.Byte":
                    int intV = 0;
                    int.TryParse(value.ToString(), out intV);
                    cell.SetCellValue(intV);
                    break;
                case "System.Decimal"://浮点型
                case "System.Double":
                    double doubV = 0;
                    double.TryParse(value.ToString(), out doubV);
                    cell.SetCellValue(doubV);
                    break;
                case "System.DBNull"://空值处理
                    cell.SetCellValue("");
                    break;
                default:
                    cell.SetCellValue("");
                    break;
            }
        }
        #endregion

        #region DataTable转Excel +static byte[] TableToExcel(DataTable dt)
        /// <summary>
        /// DataTable转Excel 读取DataTable Columns做为表头
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <returns></returns>
        public static byte[] TableToExcel(DataTable dt)
        {
            IWorkbook wk = new HSSFWorkbook();
            ISheet sheet = wk.CreateSheet();
            IRow headerRow = sheet.CreateRow(0);    // 表头
            // 处理标题
            foreach (DataColumn column in dt.Columns)
            {
                ICell cell = headerRow.CreateCell(column.Ordinal);
                cell.SetCellValue(column.ColumnName);
                // 设置样式 加粗居中
                HSSFCellStyle cellStyle = (HSSFCellStyle)wk.CreateCellStyle();
                HSSFFont font = (HSSFFont)wk.CreateFont();
                font.Boldweight = 600;
                cellStyle.SetFont(font);
                cellStyle.Alignment = HorizontalAlignment.Center;
                cell.CellStyle = cellStyle;
            }
            // 处理body部分
            int rowIndex = 1;
            foreach (DataRow row in dt.Rows)
            {
                IRow dataRow = sheet.CreateRow(rowIndex);
                foreach (DataColumn column in dt.Columns)
                {
                    dataRow.CreateCell(column.Ordinal).SetCellValue(row[column.ColumnName].ToString());
                }
                rowIndex++;
            }
            // 冻结首行
            sheet.CreateFreezePane(0, 1);
            MemoryStream ms = new MemoryStream();
            wk.Write(ms);
            sheet = null;
            headerRow = null;
            return ms.GetBuffer();
        }
        #endregion

        #region DataTable转换成Excel文件 +static byte[] TableToExcel(DataTable dt, object[,] titles, string exprotName, int max = int.MaxValue)
        /// <summary>
        /// DataTable转换成Excel文件
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="titles">标题</param>
        /// <param name="exprotName">文件名</param>
        /// <param name="max">导出最多行数</param>
        /// <returns></returns>
        public static byte[] TableToExcel(DataTable dt, object[,] titles, string exprotName, int max = int.MaxValue)
        {
            /**
             * titles demo
             * public string SayHello(string msg)
             * object[,] titles = new object[,] {
             *   { "OrderCode", "订单编号", "" },
             *   { "OrderState", "订单状态", new Func<string, string>(SayHello) },
             *   { "SalePrice", "订单金额", new object[]{ new Func<string, string>(getjine), "SalePrice", "DeliveryFee" } }
             * };
             */
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();
            // 新建一个Excel页签
            ISheet sheet = hssfworkbook.CreateSheet("Sheet1");
            IRow rowhead = sheet.CreateRow(0);
            Dictionary<string, object[]> titDic = new Dictionary<string, object[]>();   // key-属性名称 value-调用方法或空
            string strTemp = string.Empty;
            // 设置表头
            for (int i = 0; i < titles.GetLength(0); i++)
            {
                ICell cell = rowhead.CreateCell(i);
                cell.SetCellValue((string)titles[i, 1]);
                // 设置样式 加粗居中
                HSSFCellStyle cellStyle = (HSSFCellStyle)hssfworkbook.CreateCellStyle();
                HSSFFont font = (HSSFFont)hssfworkbook.CreateFont();
                font.Boldweight = 600;
                cellStyle.SetFont(font);
                cellStyle.Alignment = HorizontalAlignment.Center;
                cell.CellStyle = cellStyle;
                if (!titDic.ContainsKey(strTemp = ((string)titles[i, 0]).ToLower())) // 执行if判断之后，把值赋给变量
                {
                    titDic.Add(strTemp, new object[2] { titles[i, 1], titles[i, 2] });  // key=>字段 value=>字段中文名 字段值获取方法
                }
            }
            int index;
            for (int i = 0; i < dt.Rows.Count && i < max; i++)
            {
                IRow rowbody = sheet.CreateRow(i + 1);
                index = 0;
                foreach (KeyValuePair<string, object[]> item in titDic)
                {
                    object[] val = titDic[dt.Columns[item.Key].ToString().ToLower()];
                    object fun = val[1];
                    ICell cell = rowbody.CreateCell(index++);
                    if (fun is Func<string, string>)
                    {
                        cell.SetCellValue((fun as Func<string, string>)(dt.Rows[i][item.Key].ToString()));
                    }
                    else if (fun is object[])
                    {
                        cell.SetCellValue((((object[])fun)[0] as Func<string, string, string>)(dt.Rows[i][(string)((object[])fun)[1]].ToString(), dt.Rows[i][(string)((object[])fun)[2]].ToString()));
                    }
                    else
                        FillCellValue(cell, dt.Rows[i][item.Key]);
                }
            }
            // 冻结首行
            sheet.CreateFreezePane(0, 1);
            MemoryStream file = new MemoryStream();
            hssfworkbook.Write(file);
            sheet = null;
            return file.GetBuffer();
        }
        #endregion

        #region 设置单元格数据格式 -static void SetCellDataFormat(ICellStyle cellStyle, ICell cell, string dataFormat)
        /// <summary>
        /// 设置单元格数据格式
        /// </summary>
        /// <param name="cellStyle">单元格样式</param>
        /// <param name="cell">单元格</param>
        /// <param name="dataFormat">数据格式</param>
        private static void SetCellDataFormat(ICellStyle cellStyle, ICell cell, string dataFormat)
        {
            cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat(dataFormat);
            cell.CellStyle = cellStyle;
        }
        #endregion

        /// <summary>
        /// Excel标题
        /// </summary>
        public class ExcelTitle
        {
            /// <summary>
            /// 实体属性
            /// </summary>
            public string PropertyName { get; set; }

            /// <summary>
            /// 标题
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// 数据格式 仅支持Excel内嵌格式
            /// ps: 通过Excel软件 任选一单元格右击 "选择设置单元格格式" 查看 "数字" 选项卡中 "自定义" 选项
            /// </summary>
            public string DataFormat { get; set; }

            /// <summary>
            /// 百分数
            /// </summary>
            public const string PERCENTAGE = "0.00%";

            /// <summary>
            /// 小数 保留两位
            /// </summary>
            public const string DECIMAL = "0.00";

            /// <summary>
            /// 科学计数法
            /// </summary>
            public const string SCIENTIFIC_COUNTING_METHOD = "0.00E+00";

            /// <summary>
            /// 日期格式
            /// </summary>
            public const string DATE = "yyyy/m/d";

            /// <summary>
            /// 日期时间格式
            /// </summary>
            public const string DATE_TIME = "yyyy/m/d h:mm";
        }
    }
}
