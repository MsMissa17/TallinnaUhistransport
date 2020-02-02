using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TallinnaUhistransport
{
    class gpsInfo
    {
            // table attributes
            private string type;
            private string number;
            private string lat;
            private string lng;

            public gpsInfo(string type, string number, string lat, string lng)
            {
                this.type = type;
                this.number = number;
                this.lat = lat;
                this.lng = lng;
            }

            public string Type
            {
                get { return type; }
            }

            public string Number
            {
                get { return number; }
            }

            public string Lat
            {
                get { return lat; }
            }

            public string Lng
            {
                get { return lng; }
            }
    }
}
