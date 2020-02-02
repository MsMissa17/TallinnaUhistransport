using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Windows.Forms;

/// <summary>
/// select transport type
/// then select transport number
/// solution: coordinates of selected transport type and number, route name and weekdays
/// </summary>

namespace TallinnaUhistransport
{
    public partial class Form1 : Form
    {
        private List<string> Transport; // transport type list
        private DataTable dt;
        private DataView dv;
        private List<gpsInfo> gpsInfoData;
        WebClient client = new WebClient();

        public Form1()
        {
            InitializeComponent();
            TypeCB(); // transport type combobox

            // initialize datatable and attributes
            dt = new DataTable();
            dt.Columns.Add("Liik");
            dt.Columns.Add("Number");
            dt.Columns.Add("Laiuskraad (DD)");
            dt.Columns.Add("Pikkuskraad (DD)");

            // fill the table with gps.txt file data
            fillTable(dataList());

            dv = new DataView(dt);
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.DataSource = dv;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // refresh data in table
            timer1.Interval = 5 * 1000;
            timer1.Enabled = true;
            timer1.Start();
            dv.RowFilter = "Number <> '0'";
        }

        // fill transport type combobox with the list named Transport
        private void TypeCB()
        {
            Transport = new List<String>
            {
                "---Vali liik---",
                "Buss",
                "Tramm",
                "Troll"
            };
            transportTypeCB.DataSource = Transport;
        }

        // table data list
        private List<gpsInfo> dataList()
        {
            // using data from gps.txt file
            Stream stream = client.OpenRead("https://transport.tallinn.ee/gps.txt");
            StreamReader gpsFile = new StreamReader(stream);
            string line = gpsFile.ReadLine();

            while (!gpsFile.EndOfStream)
            {
                string value1 = line.Split(',')[0]; // transport type (int 1 (trolleybus), int 2 (bus), int 3 (tram))
                string value2 = line.Split(',')[1]; // transport number
                string value3 = line.Split(',')[3]; // coordinates (DD) - lat 
                string value4 = line.Split(',')[2]; // coordinates (DD) - lng

                // change number strings to text string
                if (value1 == "1") { value1 = "Troll"; }
                else if (value1 == "2") { value1 = "Buss"; }
                else if (value1 == "3") { value1 = "Tramm"; }

                // format coordinates
                if (value3.Substring(0, 1) == "5") { value3 = "59." + line.Split(',')[3].ToString().Substring(2, 5); }
                if (value4.Substring(0, 1) == "2") { value4 = "24." + line.Split(',')[2].ToString().Substring(2, 5); }

                var rowIndex = dt.Rows.Add(value1, value2, value3, value4);
                line = gpsFile.ReadLine();
                gpsInfoData = new List<gpsInfo>
            {
                new gpsInfo(value1, value2, value3, value4)
            };
            }
            return gpsInfoData;
        }
        
        // fill table with data
        private void fillTable(List<gpsInfo> gpsInfoData)
        {
            //add data in each columns
            foreach (var d in gpsInfoData)
            {
                dt.Rows.Add(d.Type, d.Number, d.Lat, d.Lng);
            }
        }

        // if transport type is selected, filters type, enable selection of numbers
        private void transportTypeCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            // fill selection of numbers - depends on seleced transport type
            String selectedItem = transportTypeCB.SelectedItem.ToString();
            var busnrFile = File.ReadAllLines(@".\BusNumbers.txt");
            var busNr = new List<string>(busnrFile);
            var tramnrFile = File.ReadAllLines(@".\TramNumbers.txt");
            var tramNr = new List<string>(tramnrFile);
            var trolnrFile = File.ReadAllLines(@".\TrolNumbers.txt");
            var trolNr = new List<string>(trolnrFile);

            if (dv != null)
            {
                if (selectedItem != "---Vali liik---")
                {
                    if (selectedItem == "Buss")
                    {
                        textBox2.Enabled = true; // enable textbox2
                        textBox2.Text = "Vali bussi number";
                        transportNrCB.Enabled = true; // enable selection of transport number

                        transportNrCB.DataSource = busNr; // get data from text file
                        dv.RowFilter = string.Format("Liik Like '{0}' AND Number <> '0'", selectedItem); // filter table
                        dt.DefaultView.RowFilter = null;

                    }
                    else if (selectedItem == "Tramm")
                    {
                        textBox2.Enabled = true; // enable textbox2
                        textBox2.Text = "Vali trammi number";
                        transportNrCB.Enabled = true; // enable selection of transport number

                        transportNrCB.DataSource = tramNr; // get data from text file
                        dv.RowFilter = string.Format("Liik Like '{0}' AND Number <> '0'", selectedItem); // filter table
                        dt.DefaultView.RowFilter = null;
                    }
                    else if (selectedItem == "Troll")
                    {
                        textBox2.Enabled = true; // enable textbox2
                        textBox2.Text = "Vali trolli number";
                        transportNrCB.Enabled = true; // enable selection of transport number

                        transportNrCB.DataSource = trolNr; // get data from text file
                        dv.RowFilter = string.Format("Liik Like '{0}' AND Number <> '0'", selectedItem); // filter table
                        dt.DefaultView.RowFilter = null;
                    }
                }
                else
                {
                    dv.RowFilter = "Number <> '0'";
                    textBox2.Text = "Vali transpordi number";
                    textBox2.Enabled = false;
                    transportNrCB.Enabled = false;
                    textBox5.Enabled = false;
                    textBox6.Enabled = false;
                    textBox7.Enabled = false;
                    textBox8.Enabled = false;
                }
            }
        }

        // if transport type and number are selected, filters by type and number
        private void transportNrCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            String selectedItem = transportTypeCB.SelectedItem.ToString();
            String selectednrItem = transportNrCB.SelectedItem.ToString();

            if (dv != null)
            {
                if (selectednrItem != "---Vali number---")
                {
                    StreamReader rlines = new StreamReader(@".\TransportRoutes.txt");
                    string r = rlines.ReadLine();
                    textBox5.Visible = false;
                    textBox8.Visible = false;
                    while (!rlines.EndOfStream)
                    {
                        string rType = r.Split(',')[1]; // transport type in Estonian
                        string rNr = r.Split(',')[0]; // transport number
                        string rRoute = r.Split(',')[4]; // route name
                        string rDay = r.Split(',')[5]; // route days

                        if (rDay == "1234567z") { rDay = "E-P"; }
                        else if (rDay == "123456z") { rDay = "E-L"; }
                        else if (rDay == "12345z" || rDay == "12345") { rDay = "E-R"; }

                        if (selectedItem == rType && selectednrItem == rNr)
                        {
                            textBox5.Visible = true;
                            textBox8.Visible = true;
                            textBox5.Text = rRoute;
                            textBox8.Text = rDay;
                        }
                        r = rlines.ReadLine();
                    }

                    if (selectedItem == "Buss") // filter bus and bus number
                    {
                        dv.RowFilter = string.Format("Number Like '{0}' AND Liik = 'Buss'", selectednrItem);
                        dt.DefaultView.RowFilter = null;

                    }
                    else if (selectedItem == "Tramm") // filter tram and tram number
                    {
                        dv.RowFilter = string.Format("Number Like '{0}' AND Liik = 'Tramm'", selectednrItem);
                        dt.DefaultView.RowFilter = null;
                    }
                    else if (selectedItem == "Troll") // filter trol and trol number
                    {
                        dv.RowFilter = string.Format("Number Like '{0}' AND Liik = 'Troll'", selectednrItem);
                        dt.DefaultView.RowFilter = null;
                    }
                    else
                    {
                        dataGridView1.DataSource = dt;
                    }
                }
            }
        }

        // open Form2 in new window
        private void btnOpenMap_Click(object sender, EventArgs e)
        {
            try
            {
                // minimize current window
                this.WindowState = FormWindowState.Minimized;
                String fRowLat = dataGridView1.Rows[0].Cells[2].Value.ToString();
                String fRowLng = dataGridView1.Rows[0].Cells[3].Value.ToString();
                double MapLat = Convert.ToDouble(fRowLat);
                double MapLng = Convert.ToDouble(fRowLng);
                // new form
                // send tableview to form2
                // wait Form2 to close before load the Form1
                Form2 newForm = new Form2();
                newForm.Dgv = dataGridView1;
                newForm.mapLat = MapLat;
                newForm.mapLng = MapLng;
                newForm.ShowDialog();
                this.WindowState = FormWindowState.Normal;
            }
            catch (Exception)
            {
                // if the table is empty, can't show pins on map
                MessageBox.Show("Ei saa kaardil näidata!", "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            dt.Clear();
            fillTable(dataList());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}