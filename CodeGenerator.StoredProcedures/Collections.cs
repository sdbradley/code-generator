using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace CodeGenerator.Generator {
    public class Collections {
        private string connectionString = string.Empty;
        private string path = string.Empty;
        private string nameSpace = string.Empty;

        public Collections(string ConnectionString, string NameSpace, string Path) {
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
                        CreateEntityCollection(nameSpace, tableName);
                    }
                }
                retval = true;
            }
            catch (Exception ex) {
                throw ex;
            }
            return retval;
        }
        private bool CreateEntityCollection(string nameSpace, string tableName) {
            bool retval = false;
            try {
                string className = tableName.Replace(" ", string.Empty) + "EntityCollection";
                string entityName = tableName.Replace(" ", string.Empty) + "Entity";
                System.Collections.ArrayList columns = new System.Collections.ArrayList();

                if (!System.IO.Directory.Exists(this.path + @"\Common\Collections")) {
                    System.IO.Directory.CreateDirectory(this.path + @"\Common\Collections");
                }
                System.IO.StreamWriter writer = new System.IO.StreamWriter(this.path + @"\Common\Collections\" + className + ".cs");
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("using System;");
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using System.Text;");
                sb.Append("using ");
                sb.Append(nameSpace);
                sb.AppendLine(".Common.Entities;");
                sb.AppendLine("");
                sb.Append("namespace ");
                sb.Append(nameSpace + ".Common.Collections");
                sb.AppendLine(" {");
                sb.Append("public class ");
                sb.Append(className);
                sb.AppendLine(" : System.Collections.CollectionBase { ");
                sb.Append("public void Add(");
                sb.Append(entityName);
                sb.AppendLine(" entity) {");
                sb.AppendLine("InnerList.Add(entity);");
                sb.AppendLine("}");
                sb.AppendLine("public void Remove(int index) {");
                sb.AppendLine("InnerList.RemoveAt(index);");
                sb.AppendLine("}");
                sb.Append("public ");
                sb.Append(entityName);
                sb.AppendLine(" Item(int index) {");
                sb.Append("return (");
                sb.Append(entityName);
                sb.AppendLine(")InnerList[index];");
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
