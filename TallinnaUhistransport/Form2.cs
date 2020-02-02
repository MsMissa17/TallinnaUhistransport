using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GMap.NET.MapProviders;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

/// <summary>
/// This form shows location of selected bus/trolleybus/tram on the map.
/// Using only the first row data - must change to use all filtered data rows
/// Using Google Map.
/// </summary>

namespace TallinnaUhistransport
{
    public partial class Form2 : Form
    {
        // map markers list
        private List<PointLatLng> _points;

        public Form2()
        {
            InitializeComponent();
            // map markers list
            _points = new List<PointLatLng>();
        }

        // get latitude and longtitude from Form1
        public DataGridView Dgv { get; set; }
        public double mapLat { get; set; }
        public double mapLng { get; set; }

        private void Form2_Load(object sender, EventArgs e)
        {
            // Form2 is on top of other window
            this.TopMost = true;

            timer2.Interval = 1 * 1000;//5 seconds
            timer2.Enabled = true;
            timer2.Start();

            // location shown on the map
            // enable navigate by using mouse left button
            GMapProviders.GoogleMap.ApiKey = @"AIzaSyA1mID7g9lPLv0DgmqjoPO4Jhr9IiAO5-M";
            map.DragButton = MouseButtons.Left;
            map.MapProvider = GMapProviders.GoogleMap;
            map.ShowCenter = false;
            map.SetPositionByKeywords("Tallinn, Estonia");

            //set the position of map to the same point as the first transport coordinates in table
            map.Position = new PointLatLng(this.mapLat, this.mapLng);
            // default zoom
            map.MinZoom = 5;
            map.MaxZoom = 20;
            map.Zoom = 11;

            // add pins from filtred table
            // custom icon, marker type
            List<PointLatLng> coordinates = new List<PointLatLng>();
            foreach (DataGridViewRow row in Dgv.Rows)
            {
                PointLatLng item = new PointLatLng();
                foreach (DataGridViewCell cell in row.Cells)
                {
                    PointLatLng point = new PointLatLng(Convert.ToDouble(row.Cells[2].Value.ToString()), Convert.ToDouble(row.Cells[3].Value.ToString()));
                    Bitmap dot = (Bitmap)Image.FromFile("img/reddot.png");
                    GMapMarker marker = new GMarkerGoogle(point, dot);
                    GMapOverlay markers = new GMapOverlay("markers"); // overlay
                    markers.Markers.Add(marker); // add available markers
                    map.Overlays.Add(markers); // cover map with the overlay
                }
                coordinates.Add(item);
            }
        }

        /// <summary>
        /// Buttons
        /// Enable select zooming percentage
        /// Close the window
        /// </summary>

        private void btn100_Click(object sender, EventArgs e)
        {
            map.Zoom = 16;
        }

        private void btn75_Click(object sender, EventArgs e)
        {
            map.Zoom = 15;
        }

        private void btn50_Click(object sender, EventArgs e)
        {
            map.Zoom = 13;
        }

        private void btn25_Click(object sender, EventArgs e)
        {
            map.Zoom = 12;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
        }
    }
}
