using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace PayCheckCalculatorWinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            LoadDefaultValues();
        }
        
        private void LoadDefaultValues()
        {
            // Přidejte sloupce do DataGridView
            dataGridView1.Columns.Add("Date", "Datum");
            dataGridView1.Columns.Add("Project", "Projekt");
            dataGridView1.Columns.Add("Task", "Úkol");
            dataGridView1.Columns.Add("Hours", "Hodiny");

            // Přidejte projekty do ComboBox
            comboBox1.Items.Add("Projekt 1");
            comboBox1.Items.Add("Projekt 2");
            comboBox1.Items.Add("Projekt 3");
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV soubory (*.csv)|*.csv|Všechny soubory (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                DataTable dataTable = new DataTable();
                using (StreamReader sr = new StreamReader(openFileDialog.FileName))
                {
                    var headers = sr.ReadLine().Split(',');
                    foreach (string header in headers)
                    {
                        dataTable.Columns.Add(header);
                    }
                    while (!sr.EndOfStream)
                    {
                        var rows = sr.ReadLine().Split(',');
                        DataRow dataRow = dataTable.NewRow();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            dataRow[i] = rows[i];
                        }
                        dataTable.Rows.Add(dataRow);
                    }
                }
                dataGridView1.DataSource = dataTable;
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV soubory (*.csv)|*.csv|Všechny soubory (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                StringBuilder sb = new StringBuilder();
                var columnNames = new string[dataGridView1.Columns.Count];
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    columnNames[i] = dataGridView1.Columns[i].HeaderText;
                }
                sb.AppendLine(string.Join(",", columnNames));

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        var cellValues = new string[row.Cells.Count];
                        for (int i = 0; i < row.Cells.Count; i++)
                        {
                            cellValues[i] = row.Cells[i].Value.ToString();
                        }
                        sb.AppendLine(string.Join(",", cellValues));
                    }
                }
                File.WriteAllText(saveFileDialog.FileName, sb.ToString());
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Opravdu chcete ukončit aplikaci?", "Ukončit", MessageBoxButtons.YesNo) ==
                DialogResult.Yes)
            {
                Application.Exit();    
            }
        }
    }
}