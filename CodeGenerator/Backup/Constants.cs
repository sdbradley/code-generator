using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeGenerator.Generator {
    public class Constants {
        private string path = string.Empty;
        private string nameSpace = string.Empty;

        public Constants(string NameSpace, string Path) {
            path = Path;
            nameSpace = NameSpace;
        }
        public bool Generate() {
            bool retval = false;
            try {
                if (!System.IO.Directory.Exists(this.path + @"\Common")) {
                    System.IO.Directory.CreateDirectory(this.path + @"\Common");
                }
                System.IO.StreamWriter writer = new System.IO.StreamWriter(this.path + @"\Common\Constants.cs");

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("using System;");
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using System.Text;");
                sb.AppendLine("");
                sb.Append("namespace ");
                sb.Append(nameSpace);
                sb.Append(".Common");
                sb.AppendLine(" {");
                sb.AppendLine("public class Constants {");
                sb.AppendLine("public enum LogSeverityEnum {");
                sb.AppendLine("None = 0,");
                sb.AppendLine("Critical = 1,");
                sb.AppendLine("Error = 2,");
                sb.AppendLine("Warning = 3,");
                sb.AppendLine("Information = 4,");
                sb.AppendLine("Verbose = 5");
                sb.AppendLine("}");
                sb.AppendLine("public static string ConnectionString() {");
                sb.AppendLine("if (System.Configuration.ConfigurationManager.ConnectionStrings[\"ConnectionStringName\"] != null) {");
                sb.AppendLine("string val = System.Configuration.ConfigurationManager.ConnectionStrings[\"ConnectionStringName\"].ToString();");
                sb.AppendLine("return val;");
                sb.AppendLine("}");
                sb.AppendLine("else {");
                sb.AppendLine("return string.Empty;");
                sb.AppendLine("}");
                sb.AppendLine("}");
                sb.AppendLine(" }");
                sb.AppendLine(" }");
                writer.WriteLine(sb.ToString());

                writer.Close();

                retval = true;
            }
            catch (Exception ex) {
                throw ex;
            }
            return retval;
        }
    }
}
