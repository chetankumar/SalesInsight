using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace RedHill.SalesInsight.DAL.Utilities
{
    public enum ImportDataFileFormat { Xlsx, Csv, Txt }
    /// <summary>
    /// Utility to import data
    /// </summary>
    public class ImportData<T> where T : class
    {
        public ImportData(string[] columnNames)
            : this(columnNames, null)
        {
        }

        /// <summary>
        /// Instantiates the import data object
        /// </summary>
        /// <param name="columnNames">Property names of the object</param>
        /// <param name="extraValues">Pass parameters that will be returned as it is, on the RowImporting and other events as event arguments</param>
        public ImportData(string[] columnNames, Dictionary<string, object> extraValues)
        {
            this.ColumnNames = columnNames;
            this.ExtraValues = extraValues;
        }

        public delegate void OnRowImportingListCallBack(object sender, ImportDataRowListEventArgs<T> e);
        public delegate void OnRowImporting(object sender, ImportDataRowImportingEventArgs<T> e);
        public delegate void OnCellImporting(object sender, ImportDataRowImportingEventArgs<T> e);
        public delegate void OnImportCompleted(object sender, ImportDataCompleteEventArgs e);

        public event OnRowImportingListCallBack RowImportingListCallBack;
        public event OnRowImporting RowImporting;
        public event OnCellImporting CellImporting;
        public event OnImportCompleted ImportCompleted;

        private Dictionary<string, object> ExtraValues { get; set; }
        public string[] ColumnNames { get; set; }

        public HttpPostedFileBase PostedFile { get; set; }

        public string FilePath { get; set; }
        public bool IncludeHeader { get; set; }
        public int SheetNumber { get; set; }
        public int RowsToCallBackList { get; set; }

        public List<string> Messages { get; set; }

        public void AddMessages(string message)
        {
            if (Messages == null)
                Messages = new List<string>();
            Messages.Add(string.Format("ERROR : {0}", message));
        }
        public bool Success
        {
            get { return Messages == null || Messages.Count == 0; }
        }
        public ImportDataFileFormat Extension { get; set; }

        private void PrepareImport()
        {
            if (this.ColumnNames == null || this.ColumnNames.Count() <= 0)
                throw new ArgumentNullException("Columns");

            if (string.IsNullOrWhiteSpace(this.FilePath))
            {
                if (this.PostedFile == null || string.IsNullOrWhiteSpace(this.PostedFile.FileName))
                {
                    throw new ArgumentException("FilePath");
                }
                else
                {
                    string tempDir = HttpContext.Current.Server.MapPath("~/Temp/");

                    if (!Directory.Exists(tempDir))
                        Directory.CreateDirectory(tempDir);

                    this.FilePath = Path.Combine(tempDir, PostedFile.FileName);

                    this.PostedFile.SaveAs(this.FilePath);
                }
            }
        }
        public List<T> ItemList { get; set; }
        public List<T> ImportedItemList { get; set; }
        public long RowsCount { get; set; }
        private void ImportFromExcel()
        {
            try
            {
                int rowsSucceeded = 0, rowsFailed = 0;
                object value = null;
                using (var package = new ExcelPackage(new FileInfo(this.FilePath)))
                {
                    ExcelWorkbook workbook = package.Workbook;

                    if (workbook != null)
                    {
                        if (workbook.Worksheets.Count > 0)
                        {
                            Console.WriteLine("Worksheets found for file : " + FilePath);
                            var workSheet = package.Workbook.Worksheets[SheetNumber > 0 ? SheetNumber : 1];

                            int rowCount = workSheet.Dimension.End.Row;
                            int colCount = workSheet.Dimension.End.Column;

                            int startRow = this.IncludeHeader ? 1 : 2;
                            this.RowsCount = rowCount + (this.IncludeHeader ? 0 : -1);
                            string columnName = string.Empty;

                            ImportDataRowImportingEventArgs<T> eventArgs = new ImportDataRowImportingEventArgs<T>();

                            eventArgs.ExtraValues = this.ExtraValues;
                            ItemList = new List<T>();
                            Stopwatch watch = new Stopwatch();
                            Stopwatch excelwatch = new Stopwatch();
                            Stopwatch dbwatch = new Stopwatch();
                            for (int j = startRow; j <= rowCount; j++)
                            {
                                if (RowsToCallBackList > 0 && ItemList.Count == 0)
                                {
                                    watch.Start();
                                    excelwatch.Start();
                                }

                                try
                                {
                                    //Create the object of passed type
                                    var obj = Activator.CreateInstance(typeof(T));
                                    try
                                    {
                                        eventArgs.RowProcessedOnce = false;

                                        for (int i = 1; i <= this.ColumnNames.Count(); i++)
                                        {
                                            columnName = this.ColumnNames[i - 1];

                                            //Get the cell value
                                            value = workSheet.Cells[j, i].Value;

                                            eventArgs.ColumnName = columnName;
                                            eventArgs.Value = value;
                                            eventArgs.Item = (T)obj;

                                            if (this.CellImporting != null)
                                                this.CellImporting.Invoke(this, eventArgs);

                                            if (eventArgs.IgnoreRow)
                                                continue;

                                            //Swap the value to ensure the final value if modified by outside code
                                            value = eventArgs.Value;

                                            //Set the property of the object
                                            SetPropertyValue(obj, columnName, value);

                                            //Mark the row as processed
                                            //eventArgs.RowProcessedOnce = true;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        rowsFailed++;
                                        AddMessages(ex.ToString());
                                    }
                                    if (this.RowImporting != null)
                                        this.RowImporting.Invoke(this, eventArgs);

                                    ItemList.Add((T)obj);
                                    rowsSucceeded++;

                                    if (RowsToCallBackList > 0 && ItemList.Count == RowsToCallBackList)
                                    {
                                        excelwatch.Stop();
                                        Console.WriteLine($"Time Taken to this excel read instance : Minutes : {excelwatch.Elapsed.Minutes}, Seconds : {excelwatch.Elapsed.Seconds}, total second : {excelwatch.Elapsed.TotalSeconds}, Rows Processed this execution : {ItemList.Count}");
                                        excelwatch.Reset();

                                        dbwatch.Start();
                                        ImportDataRowListEventArgs<T> args = new ImportDataRowListEventArgs<T>();
                                        args.Items = ItemList;
                                        args.RowsSucceeded = rowsSucceeded;
                                        args.RowsFailed = rowsFailed;
                                        if (this.RowImportingListCallBack != null)
                                            this.RowImportingListCallBack.Invoke(this, args);
                                        dbwatch.Stop();
                                        Console.WriteLine($"Time Taken to this db import instance : Minutes : {dbwatch.Elapsed.Minutes}, Seconds : {dbwatch.Elapsed.Seconds}, total second : {dbwatch.Elapsed.TotalSeconds}, Rows Processed this execution : {ItemList.Count}");
                                        dbwatch.Reset();

                                        watch.Stop();
                                        Console.WriteLine($"Time Taken to this instance : Minutes : {watch.Elapsed.Minutes}, Seconds : {watch.Elapsed.Seconds}, Rows Processed this execution : {ItemList.Count}");
                                        watch.Reset();
                                        ItemList = new List<T>();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    rowsFailed++;
                                    AddMessages(ex.ToString());
                                }
                            }

                            if (RowsToCallBackList > 0 && ItemList.Count > 0)
                            {
                                ImportDataRowListEventArgs<T> args = new ImportDataRowListEventArgs<T>();
                                args.Items = ItemList;
                                args.RowsSucceeded = rowsSucceeded;
                                args.RowsFailed = rowsFailed;
                                if (this.RowImportingListCallBack != null)
                                    this.RowImportingListCallBack.Invoke(this, args);
                                watch.Stop();
                                Console.WriteLine($"Time Taken to this instance : Minutes : {watch.Elapsed.Minutes}, Seconds : {watch.Elapsed.Seconds}, Rows Processed this execution : {ItemList.Count}");
                                watch.Reset();
                                ItemList = new List<T>();
                            }
                            ImportDataCompleteEventArgs evArgs = new ImportDataCompleteEventArgs();
                            evArgs.RowsSucceeded = rowsSucceeded;
                            evArgs.RowsFailed = rowsFailed;

                            if (this.ImportCompleted != null)
                                this.ImportCompleted.Invoke(this, evArgs);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception  : " + ex.ToString());
                AddMessages(ex.ToString());
            }
        }

        public void StartImport()
        {
            this.PrepareImport();

            switch (this.Extension)
            {
                case ImportDataFileFormat.Xlsx:
                    this.ImportFromExcel();
                    break;
                case ImportDataFileFormat.Csv:
                    this.ImportFromCsv();
                    break;
                case ImportDataFileFormat.Txt:
                    this.ImportFromTextFile();
                    break;
            }
        }

        private void ImportFromCsv()
        {
            AddMessages("Not Implemented.");
            //TODO: Add code to import from CSV
        }

        private void ImportFromTextFile()
        {
            AddMessages("Not Implemented.");
            //TODO: Add code to import from text file
        }

        /// <summary>
        /// Sets the value to the property
        /// </summary>
        /// <param name="obj">The object whose property is to be set</param>
        /// <param name="propertyName">The name of the property</param>
        /// <param name="value">The value to be set</param>
        private void SetPropertyValue(object obj, string propertyName, object value)
        {
            PropertyInfo pInfo = GetPropertyInfo(propertyName);
            if (pInfo != null)
            {
                Type type = pInfo.PropertyType;

                //Check for Nullable Generic types properties
                if (pInfo.PropertyType.IsGenericType && pInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    type = Nullable.GetUnderlyingType(pInfo.PropertyType);

                object finalValue = (value == null) ? value : Convert.ChangeType(value, type);
                if (type == typeof(string) && finalValue != null)
                {
                    finalValue = UTF8Decode((string)finalValue);
                }
                pInfo.SetValue(obj, finalValue, null);
            }
        }

        public string UTF8Decode(string value)
        {
            value = HttpUtility.JavaScriptStringEncode(value);
            //byte[] bytes = Encoding.Default.GetBytes(value);
            //value = Encoding.UTF8.GetString(bytes);
            return value;
        }
        private PropertyInfo GetPropertyInfo(string propertyName)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            PropertyInfo propInfo = null;

            foreach (PropertyInfo info in properties)
            {
                if (info.Name.Equals(propertyName))
                {
                    propInfo = info;
                    break;
                }
            }
            return propInfo;
        }
    }
    public class ImportDataRowImportingEventArgs<T> : EventArgs where T : class
    {
        public bool IgnoreRow { get; set; }
        public string ColumnName { get; set; }
        public bool RowProcessedOnce { get; set; }

        /// <summary>
        /// Gets or Sets the final value that will be assigned to the item
        /// </summary>
        public object Value { get; set; }
        public T Item { get; set; }
        public Dictionary<string, object> Values { get; set; }
        public Dictionary<string, object> ExtraValues { get; set; }
    }

    public class ImportDataRowListEventArgs<T> : EventArgs where T : class
    {
        public int RowsSucceeded { get; set; }
        public int RowsFailed { get; set; }
        public List<T> Items { get; set; }
    }

    public class ImportDataCompleteEventArgs : EventArgs
    {
        public int RowsSucceeded { get; set; }
        public int RowsFailed { get; set; }
    }

    public class ImportDataEventArgs : EventArgs
    {
        public bool Cancel { get; set; }
        public string FilePath { get; set; }
    }

    public static class EDI_ExtensionMethods
    {
        public static void AddThis<T>(this List<T> enumerable, T value)
        {
            if (enumerable == null)
            {
                enumerable = new List<T>();
            }
            enumerable.Add(value);
        }
    }
}
