using Ltr.MultiProtocol.Nmea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafecastToGpx
{
    class SafecastRow : NMEAMessage
    {
        public SafecastRow(NMEAMessage msg) : base(msg.Parameters)
        {
            if (Parameters.Length != 15)
                throw new IndexOutOfRangeException();

            GetInt(1);
            DateTime.Parse(GetStr(2));
            GetInt(3);
            GetInt(4);
            GetInt(5);
            GetStr(6);
            GetGpsLatLon(7);
            GetGpsLatLon(9);
            GetDbl(11);
            GetStr(12);
            GetDbl(13);
            GetStr(14);
        }

        public int DeviceId { get { return GetInt(1); }  }
        public DateTime TimeStamp { get { return DateTime.Parse(GetStr(2)); } }

        public int RadiationMinute { get { return GetInt(3); } }
        public int Radiation5Sec { get { return GetInt(4); } }

        public int RadiationTotal { get { return GetInt(5); } }
        public string RadiationValidity { get { return GetStr(6); } }

        public double Latitude { get { return GetGpsLatLon(7); } }
        public double Longitude { get { return GetGpsLatLon(9); } }

        public double Altitude { get { return GetDbl(11); } }
        public string GPSValidity { get { return GetStr(12); } }
        public double HDOP { get { return GetDbl(13); } }
        public int DGPSFix { get { return GetInt(14); } }
    }
}
