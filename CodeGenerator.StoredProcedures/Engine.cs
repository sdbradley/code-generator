using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace CodeGenerator.Generator {
    public class Engine {
        private string connectionString = string.Empty;
        private string path = string.Empty;
        private string nameSpace = string.Empty;

        public Engine(string ConnectionString, string NameSpace, string Path) {
            connectionString = ConnectionString;
            path = Path;
            nameSpace = NameSpace;
        }
        public bool Generate() {
            bool retval = false;
            try {
                CreateLoggingEngine(nameSpace);
                using (SqlConnection cnn = new SqlConnection(this.connectionString)) {
                    cnn.Open();
                    SqlCommand cmd = new SqlCommand("select * from INFORMATION_SCHEMA.TABLES order by TABLE_NAME", cnn);
                    cmd.CommandType = CommandType.Text;
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (reader.Read()) {
                        string schema = reader["TABLE_SCHEMA"].ToString();
                        string tableName = reader["TABLE_NAME"].ToString();
                        CreateEngine(nameSpace, tableName);
                    }
                }
                retval = true;
            }
            catch (Exception ex) {
                throw ex;
            }
            return retval;
        }
        private bool CreateLoggingEngine(string nameSpace) {
            bool retval = false;
            try {
                string className = "LoggingEngine";
                if (!System.IO.Directory.Exists(this.path + @"\Engine")) {
                    System.IO.Directory.CreateDirectory(this.path + @"\Engine");
                }
                System.IO.StreamWriter writer = new System.IO.StreamWriter(this.path + @"\Engine\" + className + ".cs");
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("using System;");
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using System.Text;");
                sb.Append("using ");
                sb.Append(nameSpace);
                sb.AppendLine(".Common;");
                sb.Append("using ");
                sb.Append(nameSpace);
                sb.AppendLine(".Data;");
                sb.AppendLine("");
                sb.Append("namespace ");
                sb.Append(nameSpace + ".Engine {");
                sb.Append("public class ");
                sb.Append(className);
                sb.AppendLine(" { ");

                sb.AppendLine("public static void Log(Log log) {");
                sb.AppendLine("try {");
                sb.AppendLine("LoggingData logger = new LoggingData(Constants.ConnectionString());");
                sb.AppendLine("DataStructures.LogResponse response = logger.Log(log);");
                sb.AppendLine("if (!response.Result) {");
                sb.AppendLine("}");
                sb.AppendLine("}");
                sb.Append("catch (Exception ex) {");
                sb.AppendLine("}");
                sb.Append("}");

                sb.AppendLine("public static void Log(string Message) {");
                sb.AppendLine("try {");
                sb.Append("Common.Log log = new Common.Log();");
                sb.Append("log.Message = Message;");
                sb.Append("log.Level = Constants.LogSeverityEnum.Information;");
                sb.Append("Log(log);");
                sb.AppendLine("}");
                sb.Append("catch (Exception ex) {");
                sb.AppendLine("}");
                sb.Append("}");

                sb.AppendLine("public static void Log(string Message, Constants.LogSeverityEnum Level) {");
                sb.AppendLine("try {");
                sb.Append("Common.Log log = new Common.Log();");
                sb.Append("log.Message = Message;");
                sb.Append("log.Level = Level;");
                sb.Append("Log(log);");
                sb.AppendLine("}");
                sb.Append("catch (Exception ex) {");
                sb.AppendLine("}");
                sb.Append("}");

                sb.AppendLine("public static void Log(string Message, string Method, Constants.LogSeverityEnum Level) {");
                sb.AppendLine("try {");
                sb.Append("Common.Log log = new Common.Log();");
                sb.Append("log.Message = Message;");
                sb.Append("log.Level = Level;");
                sb.Append("log.Method = Method;");
                sb.Append("Log(log);");
                sb.AppendLine("}");
                sb.Append("catch (Exception ex) {");
                sb.AppendLine("}");
                sb.Append("}");

                sb.Append("}");
                sb.Append("}");

                writer.WriteLine(sb.ToString());
                writer.Close();
                retval = true;
            }
            catch (Exception ex) {
                throw ex;
            }
            return retval;
        }
        private bool CreateEngine(string nameSpace, string tableName) {
            bool retval = false;
            try {
                string className = tableName.Replace(" ", string.Empty) + "Engine";
                System.Collections.ArrayList columns = new System.Collections.ArrayList();

                if (!System.IO.Directory.Exists(this.path + @"\Engine")) {
                    System.IO.Directory.CreateDirectory(this.path + @"\Engine");
                }
                System.IO.StreamWriter writer = new System.IO.StreamWriter(this.path + @"\Engine\" + className + ".cs");
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("using System;");
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using System.Text;");
                sb.Append("using ");
                sb.Append(nameSpace);
                sb.AppendLine(".Common;");
                sb.Append("using ");
                sb.Append(nameSpace);
                sb.AppendLine(".Common.Entities;");
                sb.Append("using ");
                sb.Append(nameSpace);
                sb.AppendLine(".Data;");
                
                sb.AppendLine("");

                sb.Append("namespace ");
                sb.Append(nameSpace + ".Engine {");

                sb.AppendLine("");

                sb.Append("public class ");
                sb.Append(className);
                sb.AppendLine(" { ");

                sb.AppendLine(CreateGetEngine(nameSpace, tableName));
                sb.AppendLine(CreateGetEntityEngine(nameSpace, tableName));
                sb.AppendLine(CreateSetEntityEngine(nameSpace, tableName));
                sb.AppendLine(CreateDeleteEntityEngine(nameSpace, tableName));

                sb.AppendLine("}");
                sb.AppendLine("}");

                writer.WriteLine(sb.ToString());
                writer.Close();
                retval = true;
            }
            catch (Exception ex) {
                throw ex;
            }
            return retval;
        }
        private string CreateGetEngine(string nameSpace, string tableName) {
            string retval = string.Empty;
            try {
                StringBuilder sb = new StringBuilder();

                sb.Append("public DataStructures.");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.Append("Response ");
                sb.Append("Get");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.AppendLine("(string searchTerm, string sortOrder, int pageIndex, int pageSize) {");

                sb.Append("DataStructures.");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.Append("Response response = new DataStructures.");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.AppendLine("Response();");
                sb.AppendLine("response.Result = false;");
                sb.AppendLine("response.Error = new DataStructures.Error();");
                sb.Append("response.");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.AppendLine(" = null;");

                sb.AppendLine("try {");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.Append("Data data = new ");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.AppendLine("Data(Constants.ConnectionString());");
                sb.Append("response = data.");
                sb.Append("Get");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.AppendLine("(searchTerm, sortOrder, pageIndex, pageSize);");
                sb.AppendLine("if (!response.Result) {");
                sb.Append(nameSpace);
                sb.Append(".Common.Log log = new ");
                sb.Append(nameSpace);
                sb.AppendLine(".Common.Log();");
                sb.AppendLine("log.Level = Constants.LogSeverityEnum.Error;");
                sb.AppendLine("log.Message = response.Error.Message;");
                sb.Append("log.Method = \"");
                sb.Append("Get");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.AppendLine("\";");
                sb.AppendLine("LoggingEngine.Log(log);");
                sb.AppendLine("}");
                sb.AppendLine("}");

                sb.AppendLine("catch (Exception ex) {");
                sb.AppendLine("response.Result = false;");
                sb.AppendLine("response.Error = new DataStructures.Error { ");
                sb.Append("Code = 5000, ");
                sb.Append("Message = ex.Message + \", \" + ex.StackTrace, ");
                sb.Append("UserMessage = \"An unspecified error occurred\"");
                sb.Append("};");
                sb.AppendLine("");
                sb.Append(nameSpace);
                sb.Append(".Common.Log log = new ");
                sb.Append(nameSpace);
                sb.AppendLine(".Common.Log();");
                sb.AppendLine("log.Level = Constants.LogSeverityEnum.Error;");
                sb.AppendLine("log.Message = ex.Message;");
                sb.AppendLine("log.Trace = ex.StackTrace;");
                sb.AppendLine("log.Method = ex.Source;");
                sb.AppendLine("LoggingEngine.Log(log);");
                sb.AppendLine("}");
                sb.AppendLine("return response;");
                sb.AppendLine("}");

                retval = sb.ToString();
            }
            catch (Exception ex) {
                throw ex;
            }
            return retval;
        }
        private string CreateGetEntityEngine(string nameSpace, string tableName) {
            string retval = string.Empty;
            try {
                StringBuilder sb = new StringBuilder();

                sb.Append("public DataStructures.");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.Append("EntityResponse ");
                sb.Append("Get");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.AppendLine("Entity(Guid key) {");

                sb.Append("DataStructures.");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.Append("EntityResponse response = new DataStructures.");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.AppendLine("EntityResponse();");
                sb.AppendLine("response.Result = false;");
                sb.AppendLine("response.Error = new DataStructures.Error();");
                sb.Append("response.Entity = null;");

                sb.AppendLine("try {");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.Append("Data data = new ");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.AppendLine("Data(Constants.ConnectionString());");
                sb.Append("response = data.");
                sb.Append("Get");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.AppendLine("Entity(key);");
                sb.AppendLine("if (!response.Result) {");
                sb.Append(nameSpace);
                sb.Append(".Common.Log log = new ");
                sb.Append(nameSpace);
                sb.AppendLine(".Common.Log();");
                sb.AppendLine("log.Level = Constants.LogSeverityEnum.Error;");
                sb.AppendLine("log.Message = response.Error.Message;");
                sb.Append("log.Method = \"");
                sb.Append("Get");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.AppendLine("Entity\";");
                sb.AppendLine("LoggingEngine.Log(log);");
                sb.AppendLine("}");
                sb.AppendLine("}");

                sb.AppendLine("catch (Exception ex) {");
                sb.AppendLine("response.Result = false;");
                sb.AppendLine("response.Error = new DataStructures.Error { ");
                sb.Append("Code = 5000, ");
                sb.Append("Message = ex.Message + \", \" + ex.StackTrace, ");
                sb.Append("UserMessage = \"An unspecified error occurred\"");
                sb.Append("};");
                sb.AppendLine("");
                sb.Append(nameSpace);
                sb.Append(".Common.Log log = new ");
                sb.Append(nameSpace);
                sb.AppendLine(".Common.Log();");
                sb.AppendLine("log.Level = Constants.LogSeverityEnum.Error;");
                sb.AppendLine("log.Message = ex.Message;");
                sb.AppendLine("log.Trace = ex.StackTrace;");
                sb.AppendLine("log.Method = ex.Source;");
                sb.AppendLine("LoggingEngine.Log(log);");
                sb.AppendLine("}");
                sb.AppendLine("return response;");
                sb.AppendLine("}");

                retval = sb.ToString();
            }
            catch (Exception ex) {
                throw ex;
            }
            return retval;
        }
        private string CreateSetEntityEngine(string nameSpace, string tableName) {
            string retval = string.Empty;
            try {
                StringBuilder sb = new StringBuilder();

                sb.Append("public DataStructures.BooleanResponse ");
                sb.Append("Set");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.Append("Entity(");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.AppendLine("Entity entity) {");

                sb.Append("DataStructures.BooleanResponse response = new DataStructures.BooleanResponse();");
                sb.AppendLine("response.Result = false;");
                sb.AppendLine("response.Error = new DataStructures.Error();");

                sb.AppendLine("try {");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.Append("Data data = new ");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.AppendLine("Data(Constants.ConnectionString());");
                sb.Append("response = data.");
                sb.Append("Set");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.AppendLine("Entity(entity);");
                sb.AppendLine("if (!response.Result) {");
                sb.Append(nameSpace);
                sb.Append(".Common.Log log = new ");
                sb.Append(nameSpace);
                sb.AppendLine(".Common.Log();");
                sb.AppendLine("log.Level = Constants.LogSeverityEnum.Error;");
                sb.AppendLine("log.Message = response.Error.Message;");
                sb.Append("log.Method = \"");
                sb.Append("Set");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.AppendLine("Entity\";");
                sb.AppendLine("LoggingEngine.Log(log);");
                sb.AppendLine("}");
                sb.AppendLine("}");

                sb.AppendLine("catch (Exception ex) {");
                sb.AppendLine("response.Result = false;");
                sb.AppendLine("response.Error = new DataStructures.Error { ");
                sb.Append("Code = 5000, ");
                sb.Append("Message = ex.Message + \", \" + ex.StackTrace, ");
                sb.Append("UserMessage = \"An unspecified error occurred\"");
                sb.Append("};");
                sb.AppendLine("");
                sb.Append(nameSpace);
                sb.Append(".Common.Log log = new ");
                sb.Append(nameSpace);
                sb.AppendLine(".Common.Log();");
                sb.AppendLine("log.Level = Constants.LogSeverityEnum.Error;");
                sb.AppendLine("log.Message = ex.Message;");
                sb.AppendLine("log.Trace = ex.StackTrace;");
                sb.AppendLine("log.Method = ex.Source;");
                sb.AppendLine("LoggingEngine.Log(log);");
                sb.AppendLine("}");
                sb.AppendLine("return response;");
                sb.AppendLine("}");

                retval = sb.ToString();
            }
            catch (Exception ex) {
                throw ex;
            }
            return retval;
        }
        private string CreateDeleteEntityEngine(string nameSpace, string tableName) {
            string retval = string.Empty;
            try {
                StringBuilder sb = new StringBuilder();

                sb.Append("public DataStructures.BooleanResponse ");
                sb.Append("Delete");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.AppendLine("Entity(Guid key) {");

                sb.Append("DataStructures.BooleanResponse response = new DataStructures.BooleanResponse();");
                sb.AppendLine("response.Result = false;");
                sb.AppendLine("response.Error = new DataStructures.Error();");

                sb.AppendLine("try {");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.Append("Data data = new ");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.AppendLine("Data(Constants.ConnectionString());");
                sb.Append("response = data.");
                sb.Append("Delete");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.AppendLine("Entity(key);");
                sb.AppendLine("if (!response.Result) {");
                sb.Append(nameSpace);
                sb.Append(".Common.Log log = new ");
                sb.Append(nameSpace);
                sb.AppendLine(".Common.Log();");
                sb.AppendLine("log.Level = Constants.LogSeverityEnum.Error;");
                sb.AppendLine("log.Message = response.Error.Message;");
                sb.Append("log.Method = \"");
                sb.Append("Delete");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.AppendLine("Entity\";");
                sb.AppendLine("LoggingEngine.Log(log);");
                sb.AppendLine("}");
                sb.AppendLine("}");

                sb.AppendLine("catch (Exception ex) {");
                sb.AppendLine("response.Result = false;");
                sb.AppendLine("response.Error = new DataStructures.Error { ");
                sb.Append("Code = 5000, ");
                sb.Append("Message = ex.Message + \", \" + ex.StackTrace, ");
                sb.Append("UserMessage = \"An unspecified error occurred\"");
                sb.Append("};");
                sb.AppendLine("");
                sb.Append(nameSpace);
                sb.Append(".Common.Log log = new ");
                sb.Append(nameSpace);
                sb.AppendLine(".Common.Log();");
                sb.AppendLine("log.Level = Constants.LogSeverityEnum.Error;");
                sb.AppendLine("log.Message = ex.Message;");
                sb.AppendLine("log.Trace = ex.StackTrace;");
                sb.AppendLine("log.Method = ex.Source;");
                sb.AppendLine("LoggingEngine.Log(log);");
                sb.AppendLine("}");
                sb.AppendLine("return response;");
                sb.AppendLine("}");

                retval = sb.ToString();
            }
            catch (Exception ex) {
                throw ex;
            }
            return retval;
        }
    }
}
