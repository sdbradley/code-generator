using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace CodeGenerator.Generator {
    public class DataAccessLayer {
        private string connectionString = string.Empty;
        private string path = string.Empty;
        private string nameSpace = string.Empty;

        public DataAccessLayer(string ConnectionString, string NameSpace, string Path) {
            connectionString = ConnectionString;
            path = Path;
            nameSpace = NameSpace;
        }
        public bool Generate() {
            bool retval = false;
            try {
                CreateLoggingDataAccessLayer(nameSpace);
                using (SqlConnection cnn = new SqlConnection(this.connectionString)) {
                    cnn.Open();
                    SqlCommand cmd = new SqlCommand("select * from INFORMATION_SCHEMA.TABLES order by TABLE_NAME", cnn);
                    cmd.CommandType = CommandType.Text;
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (reader.Read()) {
                        string schema = reader["TABLE_SCHEMA"].ToString();
                        string tableName = reader["TABLE_NAME"].ToString();
                        CreateDataAccessLayer(nameSpace, schema, tableName);
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

        private bool CreateDataAccessLayer(string nameSpace, string Schema, string tableName) {
            bool retval = false;
            try {
                string className = tableName.Replace(" ", string.Empty) + "Data";
                string entityName = tableName.Replace(" ", string.Empty) + "Entity";
                System.Collections.ArrayList columns = new System.Collections.ArrayList();

                if (!System.IO.Directory.Exists(this.path + @"\Data")) {
                    System.IO.Directory.CreateDirectory(this.path + @"\Data");
                }
                System.IO.StreamWriter writer = new System.IO.StreamWriter(this.path + @"\Data\" + className + ".cs");
                StringBuilder sb = new StringBuilder();

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

                #region Instance and Constructor

                #region template
                //#region Instance
                ///// <summary>
                ///// An object to sync up the singleton instance
                ///// </summary>
                //private static object syncRoot = new object();

                ///// <summary>
                ///// An object to hold the reference to the singleton instance
                ///// </summary>
                //private static SetupData myInstance = null;

                ///// <summary>
                ///// Provides access to the single instance of the object
                ///// </summary>
                //public static SetupData Instance {
                //    get {
                //        if (myInstance == null) {
                //            lock (syncRoot) {
                //                if (myInstance == null) {
                //                    myInstance = new SetupData();
                //                }
                //            }
                //        }

                //        return myInstance;
                //    }
                //}
                //#endregion

                //private string _connectionString = string.Empty;
                //public StudentData() {
                //    this._connectionString = Constants.ConnectionString();
                //}
                //public StudentData(string ConnectionString) {
                //    this._connectionString = ConnectionString;
                //}
                #endregion

                sb.AppendLine("using System;");
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using System.Text;");
                sb.AppendLine("using System.Data;");
                sb.AppendLine("using System.Data.SqlClient;");
                sb.Append("using ");
                sb.Append(nameSpace);
                sb.AppendLine(".Common;");
                sb.Append("using ");
                sb.Append(nameSpace);
                sb.AppendLine(".Common.Entities;");
                sb.AppendLine("");
                sb.Append("namespace ");
                sb.Append(nameSpace);
                sb.Append(".Data");
                sb.AppendLine(" {");
                sb.Append("public class ");
                sb.Append(className);
                sb.AppendLine(" {");
                sb.AppendLine("");
                sb.AppendLine("#region Instance");
                sb.AppendLine("/// <summary>");
                sb.AppendLine("/// An object to sync up the singleton instance");
                sb.AppendLine("/// </summary>");
                sb.AppendLine("private static object syncRoot = new object();");
                sb.AppendLine("");
                sb.AppendLine("/// <summary>");
                sb.AppendLine("/// An object to hold the reference to the singleton instance");
                sb.AppendLine("/// </summary>");
                sb.Append("private static ");
                sb.Append(className);
                sb.AppendLine(" myInstance = null;");
                sb.AppendLine("");
                sb.AppendLine("/// <summary>");
                sb.AppendLine("/// Provides access to the single instance of the object");
                sb.AppendLine("/// </summary>");
                sb.Append("public static ");
                sb.Append(className);
                sb.AppendLine(" Instance {");
                sb.AppendLine("get {");
                sb.AppendLine("if (myInstance == null) {");
                sb.AppendLine("lock (syncRoot) {");
                sb.AppendLine("if (myInstance == null) {");
                sb.AppendLine("myInstance = new ");
                sb.Append(className);
                sb.AppendLine("();");
                sb.AppendLine("}");
                sb.AppendLine("}");
                sb.AppendLine("}");
                sb.AppendLine("return myInstance;");
                sb.AppendLine("}");
                sb.AppendLine("}");
                sb.AppendLine("#endregion");
                sb.AppendLine("");
                sb.AppendLine("private string _connectionString = string.Empty;");
                sb.Append("public ");
                sb.Append(className);
                sb.AppendLine("() {");
                sb.AppendLine("//this._connectionString = Constants.ConnectionString();");
                sb.AppendLine("}");
                sb.Append("public ");
                sb.Append(className);
                sb.AppendLine("(string ConnectionString) {");
                sb.AppendLine("this._connectionString = ConnectionString;");
                sb.AppendLine("}");
                sb.AppendLine("");

                #endregion

                #region Get
                ///template
                ///public DataStructures.StudentsResponse GetStudents() {
                sb.AppendLine("");
                sb.Append("public DataStructures.");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.Append("Response");
                sb.Append(" ");
                sb.Append("Get");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.AppendLine("(string searchTerm, string sortOrder, int pageIndex, int pageSize) {");

                ///template
                ///DataStructures.StudentsResponse response = new DataStructures.StudentsResponse();
                ///response.Result = false;
                ///response.Students = new TEPCharter.Common.Collections.StudentEntityCollection();
                ///response.UserMessage = string.Empty;
                ///response.ErrorMessage = string.Empty;
                sb.Append("DataStructures.");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.Append("Response response = new DataStructures.");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.AppendLine("Response();");
                sb.AppendLine("response.Result = false;");
                sb.Append("response.");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.Append(" = new List<");
                sb.Append(nameSpace);
                sb.Append(".Common.Entities.");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.AppendLine("Entity>();");
                sb.AppendLine("response.Error = new DataStructures.Error();");

                sb.AppendLine("try {");
                sb.AppendLine("using (SqlConnection cnn = new SqlConnection(this._connectionString)) {");
                sb.AppendLine("cnn.Open();");
                sb.Append("SqlCommand cmd = new SqlCommand(");
                sb.Append("\"[");
                sb.Append(Schema);
                sb.Append("].");
                sb.Append("PagedSearch");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.AppendLine("\", cnn);");

                sb.AppendLine("if (!string.IsNullOrEmpty(searchTerm)) cmd.Parameters.Add(new SqlParameter(\"@SearchTerm\", searchTerm));");
                sb.AppendLine("if (!string.IsNullOrEmpty(sortOrder)) cmd.Parameters.Add(new SqlParameter(\"@SortOrder\", sortOrder));");
                sb.AppendLine("if (pageIndex > 0) cmd.Parameters.Add(new SqlParameter(\"@pageIndex\", pageIndex));");
                sb.AppendLine("if (pageSize > 0) cmd.Parameters.Add(new SqlParameter(\"@pageSize\", pageSize));");
                sb.AppendLine("SqlParameter param = new SqlParameter(\"@totalRecordCount\", SqlDbType.Int);");
                sb.AppendLine("param.Direction = ParameterDirection.Output;");
                sb.AppendLine("cmd.Parameters.Add(param);");
                sb.AppendLine("cmd.CommandType = CommandType.StoredProcedure;");

                sb.AppendLine("using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection)) {");

                sb.AppendLine("while (reader.Read()) {");
                sb.Append(entityName);
                sb.Append(" entity = ");
                sb.Append(entityName);
                sb.Append(".New");
                sb.Append(entityName);
                sb.AppendLine("();");

                //get columns
                foreach (Column col in columns) {
                    sb.Append("if (reader[\"");
                    sb.Append(col.Name);
                    sb.Append("\"] != System.DBNull.Value) ");
                    sb.Append("entity.");
                    sb.Append(col.Name);
                    sb.Append(" = ");

                    switch (col.DataType) {
                        case "uniqueidentifier":
                            sb.Append("new Guid(reader[\"");
                            sb.Append(col.Name);
                            sb.AppendLine("\"].ToString());");
                            break;
                        case "varchar":
                            sb.Append("reader[\"");
                            sb.Append(col.Name);
                            sb.AppendLine("\"].ToString();");
                            break;
                        case "text":
                            sb.Append("reader[\"");
                            sb.Append(col.Name);
                            sb.AppendLine("\"].ToString();");
                            break;
                        case "nvarchar":
                            sb.Append("reader[\"");
                            sb.Append(col.Name);
                            sb.AppendLine("\"].ToString();");
                            break;
                        case "datetime":
                            sb.Append("Convert.ToDateTime(reader[\"");
                            sb.Append(col.Name);
                            sb.AppendLine("\"].ToString());");
                            break;
                        case "bit":
                            sb.Append("Convert.ToBoolean(reader[\"");
                            sb.Append(col.Name);
                            sb.AppendLine("\"].ToString());");
                            break;
                        case "float":
                        case "decimal":
                            sb.Append("Convert.ToDouble(reader[\"");
                            sb.Append(col.Name);
                            sb.AppendLine("\"].ToString());");
                            break;
                        case "int":
                            sb.Append("Convert.ToInt32(reader[\"");
                            sb.Append(col.Name);
                            sb.AppendLine("\"].ToString());");
                            break;
                        case "bigint":
                            sb.Append("Convert.ToInt64(reader[\"");
                            sb.Append(col.Name);
                            sb.AppendLine("\"].ToString());");
                            break;
                    }
                }

                sb.Append("response.");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.AppendLine(".Add(entity);");
                sb.AppendLine("}");

                sb.AppendLine("if (!reader.IsClosed) { reader.Close(); }");
                sb.AppendLine("}");

                // get output param
                sb.AppendLine("object ofound = cmd.Parameters[\"@totalRecordCount\"].Value;");
                sb.AppendLine("if (ofound != null) {");
                sb.AppendLine("response.TotalCount = (System.Int32)ofound;");
                sb.AppendLine("}");

                sb.AppendLine("response.PageIndex = pageIndex;");
                sb.AppendLine("response.PageSize = pageSize;");

                sb.AppendLine("}");
                sb.AppendLine("response.Result = true;");
                sb.AppendLine("}");
                
                sb.AppendLine("catch (Exception ex) {");
                sb.AppendLine("response.Result = false;");
                sb.AppendLine("response.Error = new DataStructures.Error { ");
                sb.Append("Code = 5000, ");
                sb.Append("Message = ex.Message + \", \" + ex.StackTrace, ");
                sb.Append("UserMessage = \"An unspecified error occurred\"");
                sb.Append("};");
                sb.AppendLine("}");

                sb.AppendLine("return response;");
                sb.AppendLine("}");
                #endregion

                #region GetEntity
                ///template
                ///public DataStructures.StudentsResponse GetStudentEntity(Guid key) {
                sb.AppendLine("");
                sb.Append("public DataStructures.");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.Append("EntityResponse");
                sb.Append(" ");
                sb.Append("Get");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.Append("Entity");
                sb.AppendLine("(Guid key) {");

                ///template
                ///DataStructures.StudentsResponse response = new DataStructures.StudentsResponse();
                ///response.Result = false;
                ///response.Students = new TEPCharter.Common.Collections.StudentEntityCollection();
                ///response.UserMessage = string.Empty;
                ///response.ErrorMessage = string.Empty;
                sb.Append("DataStructures.");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.Append("EntityResponse response = new DataStructures.");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.AppendLine("EntityResponse();");
                sb.AppendLine("response.Result = false;");
                sb.Append("response.Entity = null;");
                sb.AppendLine("response.Error = new DataStructures.Error();");

                sb.AppendLine("try {");
                sb.AppendLine("using (SqlConnection cnn = new SqlConnection(this._connectionString)) {");
                sb.AppendLine("cnn.Open();");
                sb.Append("SqlCommand cmd = new SqlCommand(");
                sb.Append("\"[");
                sb.Append(Schema);
                sb.Append("].");
                sb.Append("Get");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.Append("Entity");
                sb.AppendLine("\", cnn);");
                sb.AppendLine("cmd.CommandType = CommandType.StoredProcedure;");
                sb.Append("cmd.Parameters.Add(new SqlParameter(\"@");
                if (columns.Count > 0) {
                    sb.Append(((Column)columns[0]).Name);
                }
                sb.AppendLine("\", key));");
                sb.AppendLine("using(SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection)) {");
                sb.AppendLine("");
                sb.AppendLine("while (reader.Read()) {");
                sb.Append(entityName);
                sb.Append(" entity = ");
                sb.Append(entityName);
                sb.Append(".New");
                sb.Append(entityName);
                sb.AppendLine("();");

                //lopp through columns
                foreach (Column col in columns) {

                    sb.Append("if (reader[\"");
                    sb.Append(col.Name);
                    sb.Append("\"] != System.DBNull.Value) ");
                    sb.Append("entity.");
                    sb.Append(col.Name);
                    sb.Append(" = ");

                    switch (col.DataType) {
                        case "uniqueidentifier":
                            sb.Append("new Guid(reader[\"");
                            sb.Append(col.Name);
                            sb.AppendLine("\"].ToString());");
                            break;
                        case "varchar":
                            sb.Append("reader[\"");
                            sb.Append(col.Name);
                            sb.AppendLine("\"].ToString();");
                            break;
                        case "text":
                            sb.Append("reader[\"");
                            sb.Append(col.Name);
                            sb.AppendLine("\"].ToString();");
                            break;
                        case "nvarchar":
                            sb.Append("reader[\"");
                            sb.Append(col.Name);
                            sb.AppendLine("\"].ToString();");
                            break;
                        case "datetime":
                            sb.Append("Convert.ToDateTime(reader[\"");
                            sb.Append(col.Name);
                            sb.AppendLine("\"].ToString());");
                            break;
                        case "bit":
                            sb.Append("Convert.ToBoolean(reader[\"");
                            sb.Append(col.Name);
                            sb.AppendLine("\"].ToString());");
                            break;
                        case "float":
                        case "decimal":
                            sb.Append("Convert.ToDouble(reader[\"");
                            sb.Append(col.Name);
                            sb.AppendLine("\"].ToString());");
                            break;
                        case "int":
                            sb.Append("Convert.ToInt32(reader[\"");
                            sb.Append(col.Name);
                            sb.AppendLine("\"].ToString());");
                            break;
                        case "bigint":
                            sb.Append("Convert.ToInt64(reader[\"");
                            sb.Append(col.Name);
                            sb.AppendLine("\"].ToString());");
                            break;
                    }
                }

                sb.Append("response.Entity = entity;");
                sb.AppendLine("}");
                sb.AppendLine("}");
                sb.AppendLine("}");
                sb.AppendLine("response.Result = true;");
                sb.AppendLine("}");
                sb.AppendLine("catch (Exception ex) {");
                sb.AppendLine("response.Result = false;");
                sb.AppendLine("response.Error = new DataStructures.Error { ");
                sb.Append("Code = 5000, ");
                sb.Append("Message = ex.Message + \", \" + ex.StackTrace, ");
                sb.Append("UserMessage = \"An unspecified error occurred\"");
                sb.Append("};");
                sb.AppendLine("}");
                sb.AppendLine("return response;");
                sb.AppendLine("}");
                #endregion

                #region SetEntity

                #region template
                //public DataStructures.BooleanResponse SetStudentEntity(TEPCharter.Common.Entities.StudentEntity entity) {
                //DataStructures.BooleanResponse response = new DataStructures.BooleanResponse();
                //response.Result = false;
                //response.UserMessage = string.Empty;
                //response.ErrorMessage = string.Empty;

                //try {
                //using (SqlConnection cnn = new SqlConnection(this._connectionString)) {
                //cnn.Open();
                //SqlCommand cmd = new SqlCommand("[TEP].SetStudents", cnn);
                //cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.Add(new SqlParameter("@student_key", entity.StudentKey));
                //cmd.Parameters.Add(new SqlParameter("@first_name", entity.FirstName));
                //cmd.Parameters.Add(new SqlParameter("@last_name", entity.LastName));
                //cmd.Parameters.Add(new SqlParameter("@ssn", entity.SSN));
                //cmd.Parameters.Add(new SqlParameter("@dob", entity.DOB));
                //cmd.Parameters.Add(new SqlParameter("@gender", entity.Gender));
                //cmd.Parameters.Add(new SqlParameter("@address", entity.Address));
                //cmd.Parameters.Add(new SqlParameter("@city", entity.City));
                //cmd.Parameters.Add(new SqlParameter("@state", entity.State));
                //cmd.Parameters.Add(new SqlParameter("@zipcode", entity.ZipCode));
                //cmd.Parameters.Add(new SqlParameter("@gradeLevelKey", entity.GradeLevelKey));
                //cmd.Parameters.Add(new SqlParameter("@classKey", entity.ClassKey));
                //cmd.Parameters.Add(new SqlParameter("@created_date", entity.CreatedDate));
                //cmd.Parameters.Add(new SqlParameter("@created_by_user", entity.CreatedByUser));
                //cmd.Parameters.Add(new SqlParameter("@updated_date", entity.UpdatedDate));
                //cmd.Parameters.Add(new SqlParameter("@updated_by_user", entity.UpdatedByUser));

                //cmd.ExecuteNonQuery();
                //}

                //response.Result = true;
                //}
                //catch (Exception ex) {
                //response.Result = false;
                //response.ErrorMessage = ex.Message + ex.StackTrace;
                //response.UserMessage = ex.Message;
                //}

                //return response;
                                //}
                #endregion

                sb.AppendLine("");
                sb.Append("public DataStructures.BooleanResponse");
                sb.Append(" ");
                sb.Append("Set");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.Append("Entity");
                sb.Append("(");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.AppendLine("Entity entity) {");

                sb.Append("DataStructures.BooleanResponse response = new DataStructures.BooleanResponse();");
                sb.AppendLine("response.Result = false;");
                sb.AppendLine("response.Error = new DataStructures.Error();");

                sb.AppendLine("try {");
                sb.AppendLine("using (SqlConnection cnn = new SqlConnection(this._connectionString)) {");
                sb.AppendLine("cnn.Open();");
                sb.Append("SqlCommand cmd = new SqlCommand(");
                sb.Append("\"[");
                sb.Append(Schema);
                sb.Append("].");
                sb.Append("Set");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.AppendLine("\", cnn);");
                sb.AppendLine("cmd.CommandType = CommandType.StoredProcedure;");

                //loop through columns
                foreach (Column col in columns) {
                    sb.Append("cmd.Parameters.Add(new SqlParameter(\"@");
                    sb.Append(col.Name);
                    sb.Append("\", entity.");
                    sb.Append(col.Name);
                    sb.AppendLine("));");
                }
                sb.AppendLine("cmd.ExecuteNonQuery();");
                sb.AppendLine("}");

                sb.AppendLine("response.Result = true;");
                sb.AppendLine("}");

                sb.AppendLine("catch (Exception ex) {");
                sb.AppendLine("response.Result = false;");
                sb.AppendLine("response.Error = new DataStructures.Error { ");
                sb.Append("Code = 5000, ");
                sb.Append("Message = ex.Message + \", \" + ex.StackTrace, ");
                sb.Append("UserMessage = \"An unspecified error occurred\"");
                sb.Append("};");
                sb.AppendLine("}");
                sb.AppendLine("return response;");
                sb.AppendLine("}");
                #endregion

                #region DeleteEntity

                #region template
                //public DataStructures.BooleanResponse DeleteStudentEntity(Guid key) {
                //    DataStructures.BooleanResponse response = new DataStructures.BooleanResponse();
                //    response.Result = false;
                //    response.UserMessage = string.Empty;
                //    response.ErrorMessage = string.Empty;

                //    try {
                //        using (SqlConnection cnn = new SqlConnection(this._connectionString)) {
                //            cnn.Open();
                //            SqlCommand cmd = new SqlCommand("[TEP].DeleteStudents", cnn);
                //            cmd.CommandType = CommandType.StoredProcedure;
                //            cmd.Parameters.Add(new SqlParameter("@student_key", key));
                //            int count = cmd.ExecuteNonQuery();
                //        }
                //        response.Result = true;
                //    }
                //    catch (Exception ex) {
                //        response.Result = false;
                //        response.ErrorMessage = ex.Message + ex.StackTrace;
                //        response.UserMessage = ex.Message;
                //    }

                //    return response;
                //}
                #endregion

                sb.AppendLine("");
                sb.Append("public DataStructures.BooleanResponse");
                sb.Append(" ");
                sb.Append("Delete");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.Append("Entity");
                sb.AppendLine("(Guid key) {");

                sb.Append("DataStructures.BooleanResponse response = new DataStructures.BooleanResponse();");
                sb.AppendLine("response.Result = false;");
                sb.AppendLine("response.Error = new DataStructures.Error();");

                sb.AppendLine("try {");
                sb.AppendLine("using (SqlConnection cnn = new SqlConnection(this._connectionString)) {");
                sb.AppendLine("cnn.Open();");
                sb.Append("SqlCommand cmd = new SqlCommand(");
                sb.Append("\"[");
                sb.Append(Schema);
                sb.Append("].");
                sb.Append("Delete");
                sb.Append(tableName.Replace(" ", string.Empty));
                sb.AppendLine("\", cnn);");
                sb.AppendLine("cmd.CommandType = CommandType.StoredProcedure;");

                //loop through columns
                if(columns.Count > 0) {
                    sb.Append("cmd.Parameters.Add(new SqlParameter(\"@");
                    sb.Append(((Column)columns[0]).Name);
                    sb.AppendLine("\", key));");
                }
                sb.AppendLine("int count = cmd.ExecuteNonQuery();");
                sb.AppendLine("}");

                sb.AppendLine("response.Result = true;");
                sb.AppendLine("}");

                sb.AppendLine("catch (Exception ex) {");
                sb.AppendLine("response.Result = false;");
                sb.AppendLine("response.Error = new DataStructures.Error { ");
                sb.Append("Code = 5000, ");
                sb.Append("Message = ex.Message + \", \" + ex.StackTrace, ");
                sb.Append("UserMessage = \"An unspecified error occurred\"");
                sb.Append("};");
                sb.AppendLine("}");
                sb.AppendLine("return response;");
                sb.AppendLine("}");
                #endregion

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
        private bool CreateLoggingDataAccessLayer(string nameSpace) {
            bool retval = false;
            try {
                if (!System.IO.Directory.Exists(this.path + @"\Data")) {
                    System.IO.Directory.CreateDirectory(this.path + @"\Data");
                }
                System.IO.StreamWriter writer = new System.IO.StreamWriter(this.path + @"\Data\LoggingData.cs");
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("using System;");
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using System.Text;");
                sb.AppendLine("using System.Data;");
                sb.Append("using ");
                sb.Append(nameSpace);
                sb.AppendLine(".Common;");

                sb.Append("namespace ");
                sb.Append(nameSpace);
                sb.AppendLine(".Data {");

                sb.AppendLine("public class LoggingData {");
                sb.AppendLine("private string _connectionString = string.Empty;");
                sb.AppendLine("public LoggingData(string ConnectionString) {");
                sb.AppendLine("this._connectionString = ConnectionString;");
                sb.AppendLine("}");

                sb.AppendLine("public DataStructures.LogResponse Log(Log log) {");
                sb.AppendLine("DataStructures.LogResponse response = new DataStructures.LogResponse();");
                sb.AppendLine("response.Result = false;");
                sb.AppendLine("response.Error = new DataStructures.Error();");
                sb.AppendLine("");
                sb.AppendLine("try {");
                sb.AppendLine("using (System.Data.SqlClient.SqlConnection cnn = new System.Data.SqlClient.SqlConnection(this._connectionString)) {");
                sb.AppendLine("cnn.Open();");
                sb.AppendLine("System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(\"LogErrorProcedureName\", cnn);");
                sb.AppendLine("cmd.CommandType = CommandType.StoredProcedure;");
                sb.AppendLine("cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter(\"@userName\", log.UserName));");
                sb.AppendLine("cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter(\"@errorNumber\", log.ErrorNumber));");
                sb.AppendLine("cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter(\"@errorSeverity\", log.Level));");
                sb.AppendLine("cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter(\"@errorMethod\", log.Method));");
                sb.AppendLine("cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter(\"@errorMessage\", log.Message));");
                sb.AppendLine("cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter(\"@trace\", log.Trace));");
                sb.AppendLine("");
                sb.AppendLine("int rows = cmd.ExecuteNonQuery();");
                sb.AppendLine("cnn.Close();");
                sb.AppendLine("response.Result = true;");
                sb.AppendLine("}");
                sb.AppendLine("}");
                sb.AppendLine("catch (Exception ex) {");
                sb.AppendLine("response.Result = false;");
                sb.AppendLine("response.Error = new DataStructures.Error { ");
                sb.Append("Code = 5000, ");
                sb.Append("Message = ex.Message + \", \" + ex.StackTrace, ");
                sb.Append("UserMessage = \"An unspecified error occurred\"");
                sb.Append("};");
                sb.AppendLine("}");
                sb.AppendLine("");
                sb.AppendLine("return response;");
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
