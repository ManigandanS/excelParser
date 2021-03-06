﻿using System;
using System.Collections.Generic;
using ExcelParser.Extensions;
using ExcelParser.Model;
using ExcelParser.Model.Extensions;
using ExcelParser.PropertyBinders;
using MsExcel = Microsoft.Office.Interop.Excel;

namespace ExcelParser.Core
{
    public class ExcelProvider : IDisposable
    {
        private readonly string _fileName;

        private readonly MsExcel.Application _excelApp;

        public ExcelProvider(string fileName)
        {
            _fileName = fileName;
            _excelApp = new MsExcel.Application();
        }

        #region Parse Methods

        public Workbook Parse()
        {
            var workbook = _excelApp.Workbooks.Open(_fileName);
            var parsedWorkbook = new Workbook {Sheets = workbook.Sheets.ToModel()};
            return parsedWorkbook;
        }

        #endregion

        #region ParseExact<T> Methods

        public IEnumerable<T> ParseExact<T>() where T : class
        {
            return ParseExact<T>(x => true);
        }

        public IEnumerable<T> ParseExact<T>(MsExcel.Workbook workbook) where T : class
        {
            return ParseExact<T>(workbook, x => true);
        }

        public IEnumerable<T> ParseExact<T>(Predicate<Row> predicate) where T : class
        {
            var workbook = _excelApp.Workbooks.Open(_fileName);
            ExcelSheetManager.Workbook = workbook;
            return ParseExact<T>(workbook, predicate);
        }

        public IEnumerable<T> ParseExact<T>(MsExcel.Workbook workbook, Predicate<Row> predicate) where T : class
        {
            if (workbook != null)
            {
                var type = typeof(T);
                var obj = Activator.CreateInstance(type);
                var propertyBinder = new PropertyBinder();
                propertyBinder.Bind(obj, predicate);
                yield return obj as T;
            }
        }

        #endregion

        #region TryParse Methods

        public bool TryParse(out Workbook workbook, out Exception exception)
        {
            workbook = null;
            exception = null;
            try
            {
                workbook = Parse();
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
            return true;
        }

        #endregion

        #region TryParseExact<T> Methods

        public bool TryParseExact<T>(out IEnumerable<T> objects, out Exception exception) where T : class
        {
            objects = null;
            exception = null;
            try
            {
                objects = ParseExact<T>(x => true);
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
            return true;
        }

        public bool TryParseExact<T>(MsExcel.Workbook workbook, out IEnumerable<T> objects, out Exception exception) where T : class
        {
            objects = null;
            exception = null;
            try
            {
                objects = ParseExact<T>(workbook, x => true);
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
            return true;
        }

        public bool TryParseExact<T>(Predicate<Row> predicate, out IEnumerable<T> objects, out Exception exception) where T : class
        {
            objects = null;
            exception = null;
            try
            {
                objects = ParseExact<T>(x => true);
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
            return true;
        }

        public bool TryParseExact<T>(MsExcel.Workbook workbook, Predicate<Row> predicate, out IEnumerable<T> objects, out Exception exception) where T : class
        {
            objects = null;
            exception = null;
            try
            {
                objects = ParseExact<T>(workbook, predicate);
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
            return true;
        }

        #endregion

        #region IDisposable Methods

        public void Dispose()
        {
            _excelApp.Quit();
        }

        #endregion
    }
}
