using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CodeGenerator {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            this.messageLabel.Text = string.Empty;
            string connectionstring = this.connectionStringTextBox.Text.Trim();
            string path = this.folderTextBox.Text.Trim();
            string nameSpace = this.namespaceTextBox.Text.Trim();
            Generator.StoredProcedures procs = new Generator.StoredProcedures(connectionstring, path);
            Generator.Entities entities = new Generator.Entities(connectionstring, nameSpace, path);
            Generator.Collections collections = new Generator.Collections(connectionstring, nameSpace, path);
            Generator.DataStructures structures = new Generator.DataStructures(connectionstring, nameSpace, path);
            Generator.DataAccessLayer dal = new Generator.DataAccessLayer(connectionstring, nameSpace, path);
            Generator.Constants constants = new Generator.Constants(nameSpace, path);
            Generator.Log log = new Generator.Log(nameSpace, path);
            Generator.Engine engine = new Generator.Engine(connectionstring, nameSpace, path);
            StringBuilder sb = new StringBuilder();
            if (procs.Generate()) {
                sb.AppendLine("Procs generated successfully");
            }
            else {
                sb.AppendLine("Procs generation FAILED.");
            }
            if (entities.Generate()) {
                sb.AppendLine("Entities generated successfully");
            }
            else {
                sb.AppendLine("Entities generation FAILED.");
            }
            if (collections.Generate()) {
                sb.AppendLine("Collections generated successfully");
            }
            else {
                sb.AppendLine("Collections generation FAILED.");
            }
            if (structures.Generate()) {
                sb.AppendLine("DataStructures generated successfully");
            }
            else {
                sb.AppendLine("DataStructures generation FAILED.");
            }
            if (dal.Generate()) {
                sb.AppendLine("DataAccessLayer generated successfully");
            }
            else {
                sb.AppendLine("DataAccessLayer generation FAILED.");
            }
            if (constants.Generate()) {
                sb.AppendLine("Constants generated successfully");
            }
            else {
                sb.AppendLine("Constants generation FAILED.");
            }
            if (log.Generate()) {
                sb.AppendLine("Log generated successfully");
            }
            else {
                sb.AppendLine("Log generation FAILED.");
            }
            if (engine.Generate()) {
                sb.AppendLine("Engine generated successfully");
            }
            else {
                sb.AppendLine("Engine generation FAILED.");
            }
            this.messageLabel.Text = sb.ToString();
        }

        private void button2_Click(object sender, EventArgs e) {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
                this.folderTextBox.Text = folderBrowserDialog1.SelectedPath;
            }
        }
    }
}
