using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeGenerator.Generator {
    public class Log {
        private string path = string.Empty;
        private string nameSpace = string.Empty;

        public Log(string NameSpace, string Path) {
            path = Path;
            nameSpace = NameSpace;
        }
        public bool Generate() {
            bool retval = false;
            try {
                if (!System.IO.Directory.Exists(this.path + @"\Common")) {
                    System.IO.Directory.CreateDirectory(this.path + @"\Common");
                }
                System.IO.StreamWriter writer = new System.IO.StreamWriter(this.path + @"\Common\Log.cs");

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("using System;");
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using System.Text;");
                sb.AppendLine("");
                sb.Append("namespace ");
                sb.Append(nameSpace);
                sb.Append(".Common");
                sb.AppendLine(" {");
                sb.AppendLine("public class Log {");
                sb.AppendLine("private string userName;");
                sb.AppendLine("private int errorNumber;");
                sb.AppendLine("private Constants.LogSeverityEnum level;");
                sb.AppendLine("private string method;");
                sb.AppendLine("private string message;");
                sb.AppendLine("private string trace;");
                sb.AppendLine("public string UserName {");
                sb.AppendLine("get { return this.userName; }");
                sb.AppendLine("set { this.userName = value; }");
                sb.AppendLine("}");
                sb.AppendLine("public int ErrorNumber {");
                sb.AppendLine("get { return this.errorNumber; }");
                sb.AppendLine("set { this.errorNumber = value; }");
                sb.AppendLine("}");
                sb.AppendLine("public Constants.LogSeverityEnum Level {");
                sb.AppendLine("get { return this.level; }");
                sb.AppendLine("set { this.level = value; }");
                sb.AppendLine("}");
                sb.AppendLine("public string Method {");
                sb.AppendLine("get { return this.method; }");
                sb.AppendLine("set { this.method = value; }");
                sb.AppendLine("}");
                sb.AppendLine("public string Message {");
                sb.AppendLine("get { return this.message; }");
                sb.AppendLine("set { this.message = value; }");
                sb.AppendLine("}");
                sb.AppendLine("public string Trace {");
                sb.AppendLine("get { return this.trace; }");
                sb.AppendLine("set { this.trace = value; }");
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
