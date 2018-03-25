﻿using Ltr.MultiProtocol.Nmea;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafecastToGpx
{
    public class DataBase
    {
        public string FileName;
        public List<DataBaseItem> Items = new List<DataBaseItem>();

        public DataBase(string filename)
        {
            FileName = filename;
            int logcnt = 0;
            using (var sr = new StreamReader(filename))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith("# NEW LOG"))
                    {
                        logcnt++;
                        continue;
                    }
                    if (line.StartsWith("#"))
                        continue;

                    try
                    {
                        var msg = SafecastRow.Parse(line);
                        if (msg == null)
                            continue;

                        Items.Add(new DataBaseItem()
                        {
                            GpsLatitude = msg.Latitude,
                            GpsLongitude = msg.Longitude,
                            GpsAltitude = msg.Altitude,
                            GpsTimeStamp = msg.GPSValidity.StartsWith("A") ? msg.TimeStamp : DateTime.MinValue,
                            GpsValid = msg.GPSValidity.StartsWith("A"),
                            GpsHDOP = msg.HDOP,
                            GpsFix = msg.DGPSFix,

                            RadValid = msg.RadiationValidity.StartsWith("A"),
                            RadShort = msg.Radiation5Sec,
                            RadLong = msg.RadiationMinute,
                            RadTotal = msg.RadiationTotal,

                            RunCount = logcnt
                            
                        });                        
                    }
                    catch { }
                }
            }
        }

        public IEnumerable<IGrouping<int,DataBaseItem>> AllTracks
        {
            get
            {
                foreach (var grp in Items.Where(x => x.GpsValid).GroupBy(x => x.RunCount))
                {
                    if (grp.Count() < 2)
                        continue;

                    yield return grp;
                }
            }
        }
    }

    public class DataBaseItem
    {
        public bool GpsValid;
        public double GpsLatitude;
        public double GpsLongitude;
        public double GpsAltitude;
        public double GpsHDOP;
        public int GpsFix;
        public DateTime GpsTimeStamp;

        public bool RadValid;
        public int RadShort;
        public int RadLong;
        public int RadTotal;

        public int RunCount;
    }
}
