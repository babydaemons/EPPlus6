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
using System.Text.RegularExpressions;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime.Workdays;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Metadata;
using OfficeOpenXml.FormulaParsing.ExpressionGraph;

namespace OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime
{
    [FunctionMetadata(
        Category = ExcelFunctionCategory.DateAndTime,
        EPPlusVersion = "4",
        Description = "Returns the number of whole networkdays (excluding weekends & holidays), between two supplied dates, using parameters to specify weekend days",
        IntroducedInExcelVersion = "2010")]
    internal class NetworkdaysIntl : ExcelFunction
    {
        public override CompileResult Execute(IEnumerable<FunctionArgument> arguments, ParsingContext context)
        {
            var functionArguments = arguments as FunctionArgument[] ?? arguments.ToArray();
            ValidateArguments(functionArguments, 2);
            var startDate = System.DateTime.FromOADate(ArgToInt(functionArguments, 0));
            var endDate = System.DateTime.FromOADate(ArgToInt(functionArguments, 1));
            WorkdayCalculator calculator = new WorkdayCalculator();
            var weekdayFactory = new HolidayWeekdaysFactory();
            if (functionArguments.Length > 2)
            {
                var holidayArg = functionArguments[2].Value;
                if (Regex.IsMatch(holidayArg.ToString(), "^[01]{7}"))
                {
                    calculator = new WorkdayCalculator(weekdayFactory.Create(holidayArg.ToString()));
                }
                else if (IsNumeric(holidayArg))
                {
                    var holidayCode = Convert.ToInt32(holidayArg);
                    calculator = new WorkdayCalculator(weekdayFactory.Create(holidayCode));
                }
                else
                {
                    return new CompileResult(eErrorType.Value);
                }
            }
            var result = calculator.CalculateNumberOfWorkdays(startDate, endDate);
            if (functionArguments.Length > 3)
            {
                result = calculator.ReduceWorkdaysWithHolidays(result, functionArguments[3]);
            }
            return new CompileResult(result.NumberOfWorkdays, DataType.Integer);
        }
    }
}
