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
        }

        public static new SafecastRow Parse(string line)
        {
            NMEAMessage msg = NMEAMessage.Parse(line);

            if (msg == null)
                return null;

            switch (msg.GetStr(0))
            {
                default:
                    return null;

                case "BNRDD":   // bGeigie nano
                case "BMRDD":   // bGeigie Mini
                case "BNXRDD":  // bGeigie NX
                    return new SafecastRow(msg);
            }
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
