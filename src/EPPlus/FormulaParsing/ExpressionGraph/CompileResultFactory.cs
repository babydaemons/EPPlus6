/*************************************************************************************************
  Required Notice: Copyright (C) EPPlus Software AB. 
  This software is licensed under PolyForm Noncommercial License 1.0.0 
  and may only be used for noncommercial purposes 
  https://polyformproject.org/licenses/noncommercial/1.0.0/

  A commercial license to use this software can be purchased at https://epplussoftware.com
 *************************************************************************************************
  Date               Author                       Change
 *************************************************************************************************
  01/27/2020         EPPlus Software AB       Initial release EPPlus 5
 *************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OfficeOpenXml.FormulaParsing;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;

namespace OfficeOpenXml.FormulaParsing.ExpressionGraph
{
    internal static class CompileResultFactory
    {
        public static CompileResult Create(object obj)
        {
            return Create(obj, 0);
        }

        public static CompileResult Create(object obj, int excelAddressReferenceId)
        {
            if ((obj is INameInfo))
            {
                obj = ((INameInfo)obj).Value;
            }
            if (obj is IRangeInfo)
            {
                obj = ((IRangeInfo)obj).GetOffset(0, 0);
            }
            if (obj == null) return new CompileResult(null, DataType.Empty);
            var t = obj.GetType();

            if (t.Equals(typeof(string)))
            {
                return new CompileResult(obj, DataType.String, excelAddressReferenceId);
            }
            if (t.Equals(typeof(double)) || obj is decimal || obj is float)
            {
                return new CompileResult(obj, DataType.Decimal, excelAddressReferenceId);
            }
            if (t.Equals(typeof(int)) || obj is long || obj is short)
            {
                return new CompileResult(obj, DataType.Integer, excelAddressReferenceId);
            }
            if (t.Equals(typeof(bool)))
            {
                return new CompileResult(obj, DataType.Boolean, excelAddressReferenceId);
            }
            if (t.Equals(typeof(ExcelErrorValue)))
            {
                return new CompileResult(obj, DataType.ExcelError, excelAddressReferenceId);
            }
            if (t.Equals(typeof(System.DateTime)))
            {
                return new CompileResult(((System.DateTime)obj).ToOADate(), DataType.Date, excelAddressReferenceId);
            }
            throw new ArgumentException("Non supported type " + t.FullName);
        }
        public static CompileResult Create(object obj, int excelAddressReferenceId, FormulaRangeAddress address)
        {
            if ((obj is INameInfo))
            {
                obj = ((INameInfo)obj).Value;
            }
            if (obj is IRangeInfo)
            {
                obj = ((IRangeInfo)obj).GetOffset(0, 0);
            }
            if (obj == null) return new CompileResult(null, DataType.Empty);
            var t = obj.GetType();
            
            if (t.Equals(typeof(string)))
            {
                return new AddressCompileResult(obj, DataType.String, address);
            }
            if (t.Equals(typeof(double)) || obj is decimal || obj is float)
            {
                return new AddressCompileResult(obj, DataType.Decimal, address);
            }
            if (t.Equals(typeof(int)) || obj is long || obj is short)
            {
                return new AddressCompileResult(obj, DataType.Integer, address);
            }
            if (t.Equals(typeof(bool)))
            {
                return new AddressCompileResult(obj, DataType.Boolean, address);
            }
            if (t.Equals(typeof (ExcelErrorValue)))
            {
                return new AddressCompileResult(obj, DataType.ExcelError, address);
            }
            if (t.Equals(typeof(System.DateTime)))
            {
                return new AddressCompileResult(((System.DateTime)obj).ToOADate(), DataType.Date, address);
            }
            throw new ArgumentException("Non supported type " + t.FullName);
        }
    }
}
