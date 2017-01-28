using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace CodeGenerator.Generator {
    public class DataStructures {
        private string connectionString = string.Empty;
        private string path = string.Empty;
        private string nameSpace = string.Empty;

        public DataStructures(string ConnectionString, string NameSpace, string Path) {
            connectionString = ConnectionString;
            path = Path;
            nameSpace = NameSpace;
        }
        public bool Generate() {
            bool retval = false;
            try {
                if (!System.IO.Directory.Exists(this.path)) {
                    System.IO.Directory.CreateDirectory(this.path);
                }
                System.IO.StreamWriter writer = new System.IO.StreamWriter(this.path + @"\Common\DataStructures.cs");

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("using System;");
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using System.Text;");
                sb.AppendLine("using System.Data;");
                sb.AppendLine("using System.IO;");
                sb.AppendLine("");
                sb.Append("namespace ");
                sb.Append(nameSpace);
                sb.AppendLine(".Common");
                sb.AppendLine(" {");
                sb.AppendLine("public class DataStructures {");
                writer.WriteLine(sb.ToString());

                writer.WriteLine(CreateBooleanDataStructure());
                writer.WriteLine(CreateLogDataStructure());
                writer.WriteLine(CreateErrorDataStructure());
                
                using (SqlConnection cnn = new SqlConnection(this.connectionString)) {
                    cnn.Open();
                    SqlCommand cmd = new SqlCommand("select * from INFORMATION_SCHEMA.TABLES order by TABLE_NAME", cnn);
                    cmd.CommandType = CommandType.Text;
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (reader.Read()) {
                        string schema = reader["TABLE_SCHEMA"].ToString();
                        string tableName = reader["TABLE_NAME"].ToString();
                        writer.WriteLine(CreateDataStructures(nameSpace, tableName));
                        writer.WriteLine(CreateEntityDataStructure(nameSpace, tableName));
                    }
                }

                sb = new StringBuilder();
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
        private string CreateDataStructures(string nameSpace, string tableName) {
            string retval = string.Empty;
            try {
                string className = tableName.Replace(" ", string.Empty) + "EntityCollection";
                string entityName = tableName.Replace(" ", string.Empty) + "Entity";
                System.Collections.ArrayList columns = new System.Collections.ArrayList();

                StringBuilder sb = new StringBuilder();

                sb.Append("public struct ");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.AppendLine("Response {");
                sb.AppendLine("public bool Result;");
                sb.Append("public List<");
                sb.Append(nameSpace);
                sb.Append(".Common.Entities.");
                sb.Append(entityName);
                sb.Append("> ");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.AppendLine(";");
                sb.AppendLine("public Error Error;");
                sb.AppendLine("public int TotalCount;");
                sb.AppendLine("public int PageIndex;");
                sb.AppendLine("public int PageSize;");
                sb.AppendLine("}");

                retval = sb.ToString();
            }
            catch (Exception ex) {
                throw ex;
            }
            return retval;
        }
        private string CreateEntityDataStructure(string nameSpace, string tableName) {
            string retval = string.Empty;
            try {
                string className = tableName.Replace(" ", string.Empty) + "EntityCollection";
                string entityName = tableName.Replace(" ", string.Empty) + "Entity";
                System.Collections.ArrayList columns = new System.Collections.ArrayList();

                StringBuilder sb = new StringBuilder();

                sb.Append("public struct ");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.AppendLine("EntityResponse {");
                sb.AppendLine("public bool Result;");
                sb.Append("public ");
                sb.Append(nameSpace);
                sb.Append(".Common.Entities.");
                sb.Append(entityName);
                sb.Append(" Entity");
                //sb.Append(tableName.Replace(" ", string.Empty));
                sb.AppendLine(";");
                sb.AppendLine("public Error Error;");
                sb.AppendLine("}");

                retval = sb.ToString();
            }
            catch (Exception ex) {
                throw ex;
            }
            return retval;
        }
        private string CreateLogDataStructure() {
            string retval = string.Empty;
            try {
                StringBuilder sb = new StringBuilder();
                sb.Append("public struct LogResponse {");
                sb.AppendLine("public bool Result;");
                sb.AppendLine("public Error Error;");
                sb.AppendLine("}");
                retval = sb.ToString();
            }
            catch (Exception ex) {
                throw ex;
            }
            return retval;
        }
        private string CreateBooleanDataStructure() {
            string retval = string.Empty;
            try {
                StringBuilder sb = new StringBuilder();
                sb.Append("public struct BooleanResponse {");
                sb.AppendLine("public bool Result;");
                sb.AppendLine("public Error Error;");
                sb.AppendLine("}");
                retval = sb.ToString();
            }
            catch (Exception ex) {
                throw ex;
            }
            return retval;
        }
        private string CreateErrorDataStructure() {
            string retval = string.Empty;
            try {
                StringBuilder sb = new StringBuilder();
                sb.Append("public struct Error {");
                sb.AppendLine("public int Code;");
                sb.AppendLine("public string Message;");
                sb.AppendLine("public string UserMessage;");
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
