using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SolutionMatchTool.Data
{
    public class ExcelHelper
    {
        public Dictionary<string, IEnumerable> WorkSheets { get; set; }

        public ExcelHelper()
        {
            this.WorkSheets = new Dictionary<string, IEnumerable>();
        }

        public string GetWorkBook()
        {
            return GetWorkBook("default");
        }

        public string GetWorkBook(string name)
        {
            StringBuilder sb = new StringBuilder("<?xml version=\"1.0\"?><?mso-application progid=\"Excel.Sheet\"?>");

            sb.Append("<s:Workbook xmlns:x=\"urn:schemas-microsoft-com:office:excel\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" xmlns:s=\"urn:schemas-microsoft-com:office:spreadsheet\">");

            sb.Append("<s:Styles>");
            sb.Append("<s:Style s:ID=\"s63\"><s:Font s:Bold=\"1\" s:Underline=\"Single\"/></s:Style>");
            sb.Append("</s:Styles>");


            foreach (KeyValuePair<string, IEnumerable> item in this.WorkSheets)
            {
                sb.Append(CreateWorkSheet(item.Key, item.Value));
            }

            sb.Append("</s:Workbook>");

            using (System.IO.StreamWriter writer = new System.IO.StreamWriter("c:/" + name + ".xml"))
            {
                writer.Write(sb.ToString());
            }

            return sb.ToString();
        }

        private string CreateWorkSheet(string name, IEnumerable value)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("<s:Worksheet s:Name=\"{0}\"><s:Table>", name);           

            int i = 0;
            foreach (var a in value)
            {
                if (i == 0)//build headers
                    sb.Append(GetProperties(a, false));


                sb.Append(GetProperties(a));
                i++;
            }

            sb.Append("</s:Table></s:Worksheet>");

            return sb.ToString();
        }

        private string GetProperties(object a)
        {
            return GetProperties(a, true);
        }
        private string GetProperties(object a, bool useValue)
        {
            StringBuilder sb = new StringBuilder();
            object value = null;

            System.Reflection.PropertyInfo[] props = a.GetType().GetProperties();

            if (!useValue)
            {
                //add columns info
                foreach (System.Reflection.PropertyInfo p in props)
                {
                    //autofit doesn't work on text, stupid MS...
                    //So, add five to the length of the column header for the default width
                    sb.Append("<s:Column s:AutoFitWidth=\"1\" s:Width=\"" + p.Name.Length + 5 + "\"></s:Column>");
                }
            }

            sb.Append("<s:Row>");
            foreach (System.Reflection.PropertyInfo p in props)
            {

                string propType = "";
                switch (p.PropertyType.Name)
                {
                    default:
                        propType = "String";
                        break;
                }

                value = p.Name;

                if (useValue)
                {
                    sb.Append("<s:Cell>");
                    try
                    {
                        value = p.GetValue(a, null);
                    }
                    catch
                    { }
                }
                else
                    sb.Append("<s:Cell s:StyleID=\"s63\">");

                sb.AppendFormat("<s:Data s:Type=\"{0}\">{1}</s:Data>", propType, value);

                sb.Append("</s:Cell>");
            }

            sb.Append("</s:Row>");

            return sb.ToString();
        }

    }
}
