﻿using OfficeOpenXml.Export.HtmlExport;
using OfficeOpenXml.Utils;
using System;
using System.IO;
using System.Text;

namespace OfficeOpenXml
{
    internal abstract partial class JsonExport
    {
        private JsonExportSettings _settings;
        protected string _indent = "";
        protected bool _minify;
        public JsonExport(JsonExportSettings settings)
        {
            _settings = settings;
            _minify = settings.Minify;
        }
        internal protected void WriteCellData(StreamWriter sw, ExcelRangeBase dr, int headerRows)
        {
            bool dtOnCell = _settings.AddDataTypesOn == eDataTypeOn.OnCell;
            ExcelWorksheet ws = dr.Worksheet;
            Uri uri = null;
            int commentIx = 0;
            WriteItem(sw, $"\"{_settings.RowsElementName}\":[", true);
            var fromRow = dr._fromRow + headerRows;
            for (int r = fromRow; r <= dr._toRow; r++)
            {
                WriteStart(sw);
                WriteItem(sw, $"\"{_settings.CellsElementName}\":[", true);
                for (int c = dr._fromCol; c <= dr._toCol; c++)
                {
                    var cv = ws.GetCoreValueInner(r, c);
                    var t = JsonEscape(ValueToTextHandler.GetFormattedText(cv._value, ws.Workbook, cv._styleId, false));
                    WriteStart(sw);
                    var hasHyperlink = _settings.WriteHyperlinks && ws._hyperLinks.Exists(r, c, ref uri);
                    var hasComment = _settings.WriteComments && ws._commentsStore.Exists(r, c, ref commentIx);
                    if (cv._value == null)
                    {
                        WriteItem(sw, $"\"t\":\"{t}\"");
                    }
                    else
                    {
                        var v = JsonEscape(HtmlRawDataProvider.GetRawValue(cv._value));
                        WriteItem(sw, $"\"v\":\"{v}\",");
                        WriteItem(sw, $"\"t\":\"{t}\"", false, dtOnCell || hasHyperlink || hasComment);
                        if (dtOnCell)
                        {
                            var dt = HtmlRawDataProvider.GetHtmlDataTypeFromValue(cv._value);
                            WriteItem(sw, $"\"dt\":\"{dt}\"", false, hasHyperlink  || hasComment);
                        }
                    }

                    if (hasHyperlink)
                    {
                        WriteItem(sw, $"\"uri\":\"{JsonEscape(uri?.OriginalString)}\"", false, hasComment);
                    }

                    if(hasComment)
                    {
                        var comment = ws.Comments[commentIx];
                        WriteItem(sw, $"\"comment\":\"{comment.Text}\"");
                    }

                    if(c == dr._toCol)
                    {
                        WriteEnd(sw, "}");
                    }
                    else
                    {
                        WriteEnd(sw,"},");
                    }
                }
                WriteEnd(sw,"]");
                if (r == dr._toRow)
                {
                    WriteEnd(sw);
                }
                else
                {
                    WriteEnd(sw, "},");
                }
            }
            WriteEnd(sw, "]");
            WriteEnd(sw);
        }
        internal static string JsonEscape(string s)
        {
            if (s == null) return "";
            var sb = new StringBuilder();
            foreach (var c in s)
            {
                switch (c)
                {
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '"':
                        sb.Append("\\\"");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    default:
                        if (c < 0x20)
                        {
                            sb.Append($"\\u{((short)c):X4}");
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }
            return sb.ToString();
        }
        internal protected void WriteItem(StreamWriter sw, string v, bool indent=false, bool addComma=false)
        {
            if (addComma) v += ",";
            if (_minify)
            {
                sw.Write(v);
            }
            else
            {
                sw.WriteLine(_indent + v);
                if (indent)
                {
                    _indent += "  ";
                }
            }
        }

        internal protected void WriteStart(StreamWriter sw)
        {
            if (_minify)
            {
            
                sw.Write("{");
            }
            else
            {
                sw.WriteLine($"{_indent}{{");
                _indent += "  ";
            }
        }
        internal protected void WriteEnd(StreamWriter sw, string bracket="}")
        {
            if (_minify)
            {
                sw.Write(bracket);
            }
            else
            {
                _indent = _indent.Substring(0, _indent.Length - 2);
                sw.WriteLine($"{_indent}{bracket}");
            }
        }
    }
}
