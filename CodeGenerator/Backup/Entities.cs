using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace CodeGenerator.Generator {
    public class Entities {
        private string connectionString = string.Empty;
        private string path = string.Empty;
        private string nameSpace = string.Empty;

        public Entities(string ConnectionString, string NameSpace, string Path) {
            connectionString = ConnectionString;
            path = Path;
            nameSpace = NameSpace;
        }
        public bool Generate() {
            bool retval = false;
            try {
                using (SqlConnection cnn = new SqlConnection(this.connectionString)) {
                    cnn.Open();
                    SqlCommand cmd = new SqlCommand("select * from INFORMATION_SCHEMA.TABLES order by TABLE_NAME", cnn);
                    cmd.CommandType = CommandType.Text;
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (reader.Read()) {
                        string schema = reader["TABLE_SCHEMA"].ToString();
                        string tableName = reader["TABLE_NAME"].ToString();
                        CreateEntity(nameSpace, tableName);
                    }
                }
                retval = true;
            }
            catch (Exception ex) {
                throw ex;
            }
            return retval;
        }
        private struct Column {
            public string Name;
            public string DataType;
        }
        private bool CreateEntity(string nameSpace, string tableName) {
            bool retval = false;
            try {
                string className = tableName.Replace(" ", string.Empty) + "Entity";
                System.Collections.ArrayList columns = new System.Collections.ArrayList();

                if (!System.IO.Directory.Exists(this.path + @"\Common\Entities")) {
                    System.IO.Directory.CreateDirectory(this.path + @"\Common\Entities");
                }
                System.IO.StreamWriter writer = new System.IO.StreamWriter(this.path + @"\Common\Entities\" + className + ".cs");
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("using System;");
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using System.Text;");
                sb.AppendLine("");
                sb.Append("namespace ");
                sb.Append(nameSpace);
                sb.Append(".Common.Entities");
                sb.AppendLine(" {");
                sb.Append("public class ");
                sb.Append(className);
                sb.AppendLine(" {");

                //get columns
                using (SqlConnection cnn = new SqlConnection(this.connectionString)) {
                    cnn.Open();
                    SqlCommand cmd = new SqlCommand("select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME=@tableName", cnn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@tableName", tableName));
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (reader.Read()) {
                        string columnName = reader["COLUMN_NAME"].ToString();
                        string dataType = reader["DATA_TYPE"].ToString();
                        string type = string.Empty;

                        Column col = new Column();
                        col.Name = columnName;
                        col.DataType = dataType;
                        columns.Add(col);

                        switch (dataType) {
                            case "uniqueidentifier":
                                type = "Guid";
                                break;
                            case "nvarchar":
                                type = "string";
                                break;
                            case "varchar":
                                type = "string";
                                break;
                            case "text":
                                type = "string";
                                break;
                            case "float":
                                type = "double";
                                break;
                            case "int":
                                type = "int";
                                break;
                            case "bigint":
                                type = "long";
                                break;
                            case "bit":
                                type = "bool";
                                break;
                            case "datetime":
                                type = "DateTime";
                                break;
                        }

                        //public properties
                        sb.Append("public ");
                        sb.Append(type);
                        sb.Append(" ");
                        sb.Append(columnName);
                        sb.AppendLine(" {");
                        sb.Append("get;");
                        sb.Append("set;");
                        sb.AppendLine("}");
                    }
                }

                //new entity method
                //public static ClassEntity NewClassEntity() {
                //    ClassEntity entity = new ClassEntity();
                //    entity.ClassKey = Guid.NewGuid();
                //    entity.Title = string.Empty;
                //    entity.Description = string.Empty;
                //    entity.CreatedDate = DateTime.Now;
                //    entity.UpdatedDate = DateTime.Now;
                //    return entity;
                //}
                sb.Append("public static ");
                sb.Append(className);
                sb.Append(" New");
                sb.Append(className);
                sb.AppendLine("() {");
                sb.Append(className);
                sb.Append(" entity = new ");
                sb.Append(className);
                sb.AppendLine("();");
                foreach (object ocol in columns) {
                    Column col = (Column)ocol;
                    switch (col.DataType) {
                        case "uniqueidentifier":
                            sb.Append("entity.");
                            sb.Append(col.Name);
                            sb.Append(" = ");
                            sb.AppendLine("Guid.NewGuid();");
                            break;
                        case "nvarchar":
                            sb.Append("entity.");
                            sb.Append(col.Name);
                            sb.Append(" = ");
                            sb.AppendLine("string.Empty;");
                            break;
                        case "varchar":
                            sb.Append("entity.");
                            sb.Append(col.Name);
                            sb.Append(" = ");
                            sb.AppendLine("string.Empty;");
                            break;
                        case "text":
                            sb.Append("entity.");
                            sb.Append(col.Name);
                            sb.Append(" = ");
                            sb.AppendLine("string.Empty;");
                            break;
                        case "datetime":
                            sb.Append("entity.");
                            sb.Append(col.Name);
                            sb.Append(" = ");
                            sb.AppendLine("DateTime.Now;");
                            break;
                    }
                }
                sb.AppendLine("return entity;");
                sb.AppendLine("}");

                // merge method
                sb.Append("public void Merge(");
                sb.Append(className);
                sb.AppendLine(" obj) {");

                foreach (object ocol in columns) {
                    Column col = (Column)ocol;
                    switch (col.DataType) {
                        case "uniqueidentifier":
                            sb.AppendLine("if (obj." + col.Name + " != Guid.Empty) { this." + col.Name + " = obj." + col.Name + "; }");
                            break;
                        case "nvarchar":                            
                        case "varchar":
                        case "text":
                            sb.AppendLine("if (!string.IsNullOrEmpty(obj." + col.Name + ")) { this." + col.Name + " = obj." + col.Name + "; }");
                            break;
                        case "datetime":
                            sb.AppendLine("if (obj." + col.Name + " > DateTime.MinValue) { this." + col.Name + " = obj." + col.Name + "; }");
                            break;
                        case "int":
                        case "bigint":
                        case "decimal":
                            sb.AppendLine("if (obj." + col.Name + " > 0) { this." + col.Name + " = obj." + col.Name + "; }");
                            break;
                    }
                }
/*
                public void Merge(InvoicesEntity obj) {
                    if (obj.InvoiceKey != Guid.Empty) { this.InvoiceKey = obj.InvoiceKey; }
                }
*/
                sb.AppendLine("}");

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
    }
}
