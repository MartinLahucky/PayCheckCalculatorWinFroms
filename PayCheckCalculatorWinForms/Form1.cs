using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using PayCheckCalculatorWinForms.CustomComponents;

namespace PayCheckCalculatorWinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PrepareForm();
            dataGridView1.CellEndEdit += CellEChanged;
            dataGridView1.DefaultValuesNeeded += SetDefaultTimes;
        }

        private void PrepareForm()
        {
            // Columns in DataGridView
            var dateTimePickerColumn = new DataGridViewDateTimePickerColumn();
            dateTimePickerColumn.HeaderText = "Datum";
            dataGridView1.Columns.Add(dateTimePickerColumn);

            dataGridView1.Columns.Add("Project", "Projekt");
            dataGridView1.Columns.Add("Task", "Úkol");

            var startTimeColumn = new DataGridViewTimePickerColumn
            {
                Name = "StartTime",
                HeaderText = "Začátek"
            };

            dataGridView1.Columns.Add(startTimeColumn);

            var endTimeColumn = new DataGridViewTimePickerColumn
            {
                Name = "EndTime",
                HeaderText = "Konec"
            };
            dataGridView1.Columns.Add(endTimeColumn);

            var hoursWorkedColumn = new DataGridViewTextBoxColumn
            {
                Name = "Hours",
                HeaderText = "Hodiny",
                ValueType = typeof(string),
                ReadOnly = true
            };
            dataGridView1.Columns.Add(hoursWorkedColumn);
        }

        private void SetDefaultTime(DataGridViewRow row)
        {
            row.Cells["StartTime"].Value = DateTime.Today.AddHours(9); // Sets value 9:00
            row.Cells["EndTime"].Value = DateTime.Today.AddHours(17); // Sets value 17:00
            row.Cells["Hours"].Value =
                (((DateTime)row.Cells["EndTime"].Value).TimeOfDay - ((DateTime)row.Cells["StartTime"].Value).TimeOfDay)
                .TotalHours.ToString("0.00");
        }

        private void SetDefaultTimes(object sender, DataGridViewRowEventArgs e)
        {
            SetDefaultTime(e.Row);
        }
        
        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV soubory (*.csv)|*.csv|Všechny soubory (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var dataTable = new DataTable();
                using (StreamReader sr = new StreamReader(openFileDialog.FileName))
                {
                    var headers = sr.ReadLine().Split(',');
                    foreach (var header in headers)
                    {
                        dataTable.Columns.Add(header);
                    }

                    while (!sr.EndOfStream)
                    {
                        var rows = sr.ReadLine().Split(',');
                        var dataRow = dataTable.NewRow();
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
                var sb = new StringBuilder();
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
        // Different name because of shadowing
        private void CellEChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && !dataGridView1.Rows[e.RowIndex].IsNewRow)
                {
                    var row = dataGridView1.Rows[e.RowIndex];
                    if (dataGridView1.Columns.Contains("StartTime") && dataGridView1.Columns.Contains("EndTime") &&
                        dataGridView1.Columns.Contains("Hours"))
                    {
                        var startValue = row.Cells["StartTime"].Value;
                        var endValue = row.Cells["EndTime"].Value;

                        if (startValue != null && endValue != null &&
                            DateTime.TryParse(startValue.ToString(), out DateTime startTime) &&
                            DateTime.TryParse(endValue.ToString(), out DateTime endTime))
                        {
                            row.Cells["Hours"].Value =
                                (endTime.TimeOfDay - startTime.TimeOfDay).TotalHours.ToString("0.00");
                        }
                        else
                        {
                            row.Cells["Hours"].Value = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při zpracování změny hodnoty buňky: " + ex.Message);
                Console.WriteLine(ex.Message);
            }
        }
    }
}