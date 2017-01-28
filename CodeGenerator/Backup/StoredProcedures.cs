using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace CodeGenerator.Generator {
    public class StoredProcedures {
        private string connectionString = string.Empty;
        private string path = string.Empty;

        public StoredProcedures(string ConnectionString, string Path) {
            connectionString = ConnectionString;
            path = Path;
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
                        CreateGetProcedure(schema, tableName);
                        //CreatePagedSearchProcedure(schema, tableName);
                        CreateGetEntityProcedure(schema, tableName);
                        CreateSetProcedure(schema, tableName);
                        CreateDeleteProcedure(schema, tableName);
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
        private bool CreateGetProcedure(string schema, string tableName) {
            bool retval = false;
            try {
                if(!System.IO.Directory.Exists(this.path + @"\Database\StoredProcs")) {
                    System.IO.Directory.CreateDirectory(this.path + @"\Database\StoredProcs");
                }
                System.IO.StreamWriter writer = new System.IO.StreamWriter(this.path + @"\Database\StoredProcs\" + schema + ".Get" + tableName + ".proc.sql");
                StringBuilder sb = new StringBuilder();
                sb.Append("CREATE PROC ");
                sb.Append("[");
                sb.Append(schema);
                sb.Append("]");
                sb.Append(".[Get");
                sb.Append(tableName);
                sb.AppendLine("] ");
                sb.AppendLine("AS ");
                sb.AppendLine("SET XACT_ABORT ON ");
                sb.AppendLine("BEGIN TRAN ");

                sb.AppendLine("SELECT ");

                //get columns
                using (SqlConnection cnn = new SqlConnection(this.connectionString)) {
                    cnn.Open();
                    SqlCommand cmd = new SqlCommand("select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME=@tableName", cnn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@tableName", tableName));
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    int counter = 0;
                    while (reader.Read()) {
                        string columnName = reader["COLUMN_NAME"].ToString();
                        string dataType = reader["DATA_TYPE"].ToString();
                        if (counter > 0) sb.Append(", ");
                        sb.AppendLine(columnName);
                        counter++;
                    }
                }

                sb.Append(" FROM ");
                sb.Append("[");
                sb.Append(schema);
                sb.Append("]");
                sb.Append(".[");
                sb.Append(tableName);
                sb.AppendLine("] ");

                sb.AppendLine("COMMIT ");

                writer.WriteLine(sb.ToString());
                writer.Close();
                retval = true;
            }
            catch (Exception ex) {
                throw ex;
            }
            return retval;
        }
        /*
        private bool CreatePagedSearchProcedure(string schema, string tableName) {
            bool retval = false;
            try {
                if (!System.IO.Directory.Exists(this.path + @"\Database\StoredProcs")) {
                    System.IO.Directory.CreateDirectory(this.path + @"\Database\StoredProcs");
                }
                System.IO.StreamWriter writer = new System.IO.StreamWriter(this.path + @"\Database\StoredProcs\" + schema + ".Get" + tableName + ".proc.sql");
                StringBuilder sb = new StringBuilder();
                sb.Append("CREATE PROC ");
                sb.Append("[");
                sb.Append(schema);
                sb.Append("]");
                sb.Append(".[PagedSearch");
                sb.Append(tableName);
                sb.AppendLine("] ");
                sb.AppendLine("AS ");
                sb.AppendLine("SET XACT_ABORT ON ");
                sb.AppendLine("BEGIN TRAN ");

                sb.AppendLine("SELECT ");

                //get columns
                using (SqlConnection cnn = new SqlConnection(this.connectionString)) {
                    cnn.Open();
                    SqlCommand cmd = new SqlCommand("select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME=@tableName", cnn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@tableName", tableName));
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    int counter = 0;
                    while (reader.Read()) {
                        string columnName = reader["COLUMN_NAME"].ToString();
                        string dataType = reader["DATA_TYPE"].ToString();
                        if (counter > 0) sb.Append(", ");
                        sb.AppendLine(columnName);
                        counter++;
                    }
                }

                sb.Append(" FROM ");
                sb.Append("[");
                sb.Append(schema);
                sb.Append("]");
                sb.Append(".[");
                sb.Append(tableName);
                sb.AppendLine("] ");

                sb.AppendLine("COMMIT ");

                writer.WriteLine(sb.ToString());
                writer.Close();
                retval = true;
            }
            catch (Exception ex) {
                throw ex;
            }
            return retval;
        }
         * */
        private bool CreateGetEntityProcedure(string schema, string tableName) {
            bool retval = false;
            try {
                System.Collections.ArrayList columns = new System.Collections.ArrayList();
                string entityName = tableName.Replace(" ", string.Empty) + "Entity";
                string firstOrdinal = string.Empty;

                if (!System.IO.Directory.Exists(this.path + @"\Database\StoredProcs")) {
                    System.IO.Directory.CreateDirectory(this.path + @"\Database\StoredProcs");
                }
                System.IO.StreamWriter writer = new System.IO.StreamWriter(this.path + @"\Database\StoredProcs\" + schema + ".Get" + entityName + ".proc.sql");

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
                        Column col = new Column();
                        col.Name = columnName;
                        col.DataType = dataType;
                        columns.Add(col);
                    }
                }

                if (columns.Count > 0) {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("CREATE PROC ");
                    sb.Append("[");
                    sb.Append(schema);
                    sb.Append("]");
                    sb.Append(".[Get");
                    sb.Append(entityName);
                    sb.AppendLine("] ");
                    sb.AppendLine("AS ");

                    sb.Append("@");
                    sb.Append(((Column)columns[0]).Name);
                    sb.Append(" ");
                    sb.AppendLine(((Column)columns[0]).DataType);

                    sb.AppendLine("SET XACT_ABORT ON ");
                    sb.AppendLine("BEGIN TRAN ");

                    sb.AppendLine("SELECT ");

                    int counter = 0;
                    foreach (Column col in columns) {
                        if (counter > 0) sb.Append(", ");
                        sb.Append("[");
                        sb.Append(col.Name);
                        sb.AppendLine("]");
                        counter++;
                    }

                    sb.Append(" FROM ");
                    sb.Append("[");
                    sb.Append(schema);
                    sb.Append("]");
                    sb.Append(".[");
                    sb.Append(tableName);
                    sb.AppendLine("] ");
                    sb.Append("WHERE ");
                    sb.Append("[");
                    sb.Append(((Column)columns[0]).Name);
                    sb.Append("]");
                    sb.Append(" = ");
                    sb.Append("@");
                    sb.AppendLine(((Column)columns[0]).Name);

                    sb.AppendLine("COMMIT ");

                    writer.WriteLine(sb.ToString());
                }

                writer.Close();
                retval = true;
            }
            catch (Exception ex) {
                throw ex;
            }
            return retval;
        }
        private bool CreateSetProcedure(string schema, string tableName) {
            bool retval = false;
            try {
                if (!System.IO.Directory.Exists(this.path + @"\Database\StoredProcs")) {
                    System.IO.Directory.CreateDirectory(this.path + @"\Database\StoredProcs");
                }
                System.IO.StreamWriter writer = new System.IO.StreamWriter(this.path + @"\Database\StoredProcs\" + schema + ".Set" + tableName + ".proc.sql");
                StringBuilder sb = new StringBuilder();
                sb.Append("CREATE PROC ");
                sb.Append("[");
                sb.Append(schema);
                sb.Append("]");
                sb.Append(".[Set");
                sb.Append(tableName);
                sb.Append("] ");

                System.Collections.ArrayList columns = new System.Collections.ArrayList();

                //set parameters
                using (SqlConnection cnn = new SqlConnection(this.connectionString)) {
                    cnn.Open();
                    SqlCommand cmd = new SqlCommand("select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME=@tableName", cnn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@tableName", tableName));
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    int counter = 0;
                    while (reader.Read()) {
                        string columnName = reader["COLUMN_NAME"].ToString();
                        string dataType = reader["DATA_TYPE"].ToString();
                        string isNullable = reader["IS_NULLABLE"].ToString();
                        string columnDefault = ((reader["COLUMN_DEFAULT"] != System.DBNull.Value) ? reader["COLUMN_DEFAULT"].ToString() : "");
                        string maxLength = ((reader["CHARACTER_MAXIMUM_LENGTH"] != System.DBNull.Value) ? reader["CHARACTER_MAXIMUM_LENGTH"].ToString() : "");
                        if (counter > 0) sb.Append(", ");
                        sb.Append("@");
                        sb.Append(columnName);
                        sb.Append(" ");
                        sb.Append(dataType);
                        if (!string.IsNullOrEmpty(maxLength)) {
                            sb.Append(" (");
                            sb.Append(maxLength);
                            sb.Append(") ");
                        }
                        if (!string.IsNullOrEmpty(columnDefault)) {
                            sb.Append(" = ");
                            sb.Append(columnDefault.Replace("(",string.Empty).Replace(")", string.Empty));
                        }
                        else if (isNullable != "NO") {
                            sb.Append(" = NULL ");
                        }
                        columns.Add(columnName);
                        counter++;
                    }
                }

                sb.Append(" AS ");
                sb.Append("SET XACT_ABORT ON ");
                sb.Append("BEGIN TRAN ");

                sb.Append("IF EXISTS ( ");
                sb.Append("SELECT [");
                sb.Append(columns[0].ToString());
                sb.Append("] ");
                sb.Append("FROM ");
                sb.Append("[");
                sb.Append(schema);
                sb.Append("]");
                sb.Append(".[");
                sb.Append(tableName);
                sb.Append("] ");
                sb.Append("WHERE ");
                sb.Append(columns[0].ToString());
                sb.Append("=");
                sb.Append("@");
                sb.Append(columns[0].ToString());
                sb.Append(") begin ");

                //update
                sb.Append("UPDATE ");
                sb.Append("[");
                sb.Append(schema);
                sb.Append("]");
                sb.Append(".[");
                sb.Append(tableName);
                sb.Append("] ");
                sb.Append("SET ");
                int colCounter = 0;
                foreach (string col in columns) {
                    if (colCounter > 0) sb.Append(", ");
                    sb.Append("[");
                    sb.Append(col);
                    sb.Append("]");
                    sb.Append("=");
                    sb.Append("@");
                    sb.Append(col);
                    colCounter++;
                }
                sb.Append(" WHERE ");
                sb.Append(columns[0].ToString());
                sb.Append("=");
                sb.Append("@");
                sb.Append(columns[0].ToString());

                sb.Append(" end else begin ");

                //insert
                sb.Append("INSERT INTO ");
                sb.Append("[");
                sb.Append(schema);
                sb.Append("]");
                sb.Append(".[");
                sb.Append(tableName);
                sb.Append("]");
                sb.Append(" (");
                colCounter = 0;
                foreach (string col in columns) {
                    if (colCounter > 0) sb.Append(", ");
                    sb.Append("[");
                    sb.Append(col);
                    sb.Append("]");
                    colCounter++;
                }
                sb.Append(") ");
                sb.Append("SELECT ");
                colCounter = 0;
                foreach (string col in columns) {
                    if (colCounter > 0) sb.Append(", ");
                    sb.Append("@");
                    sb.Append(col);
                    colCounter++;
                }

                sb.Append(" end ");
                sb.Append("COMMIT ");

                writer.WriteLine(sb.ToString());
                writer.Close();
                retval = true;
            }
            catch (Exception ex) {
                throw ex;
            }
            return retval;
        }
        private bool CreateDeleteProcedure(string schema, string tableName) {
            bool retval = false;
            try {
                if (!System.IO.Directory.Exists(this.path + @"\Database\StoredProcs")) {
                    System.IO.Directory.CreateDirectory(this.path + @"\Database\StoredProcs");
                }
                System.IO.StreamWriter writer = new System.IO.StreamWriter(this.path + @"\Database\StoredProcs\" + schema + ".Delete" + tableName + ".proc.sql");
                StringBuilder sb = new StringBuilder();
                sb.Append("CREATE PROC ");
                sb.Append("[");
                sb.Append(schema);
                sb.Append("]");
                sb.Append(".[Delete");
                sb.Append(tableName);
                sb.Append("] ");

                System.Collections.ArrayList columns = new System.Collections.ArrayList();

                //set parameters
                using (SqlConnection cnn = new SqlConnection(this.connectionString)) {
                    cnn.Open();
                    SqlCommand cmd = new SqlCommand("select TOP 1 * from INFORMATION_SCHEMA.COLUMNS order by ORDINAL_POSITION", cnn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@tableName", tableName));
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    int counter = 0;
                    while (reader.Read()) {
                        string columnName = reader["COLUMN_NAME"].ToString();
                        string dataType = reader["DATA_TYPE"].ToString();
                        string isNullable = reader["IS_NULLABLE"].ToString();
                        string columnDefault = ((reader["COLUMN_DEFAULT"] != System.DBNull.Value) ? reader["COLUMN_DEFAULT"].ToString() : "");
                        string maxLength = ((reader["CHARACTER_MAXIMUM_LENGTH"] != System.DBNull.Value) ? reader["CHARACTER_MAXIMUM_LENGTH"].ToString() : "");
                        if (counter > 0) sb.Append(", ");
                        sb.Append("@");
                        sb.Append(columnName);
                        sb.Append(" ");
                        sb.Append(dataType);
                        if (!string.IsNullOrEmpty(maxLength)) {
                            sb.Append(" (");
                            sb.Append(maxLength);
                            sb.Append(") ");
                        }
                        if (!string.IsNullOrEmpty(columnDefault)) {
                            sb.Append(" = ");
                            sb.Append(columnDefault.Replace("(", string.Empty).Replace(")", string.Empty));
                        }
                        else if (isNullable != "NO") {
                            sb.Append(" = NULL ");
                        }
                        columns.Add(columnName);
                        counter++;
                    }
                }

                sb.Append(" AS ");
                sb.Append("SET XACT_ABORT ON ");
                sb.Append("BEGIN TRAN ");

                //delete
                sb.Append("DELETE FROM ");
                sb.Append("[");
                sb.Append(schema);
                sb.Append("]");
                sb.Append(".[");
                sb.Append(tableName);
                sb.Append("] ");
                sb.Append(" WHERE ");
                sb.Append(columns[0].ToString());
                sb.Append("=");
                sb.Append("@");
                sb.Append(columns[0].ToString());

                sb.Append(" COMMIT ");

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
