using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ltr.MultiProtocol.Nmea
{
    /// <summary>
    /// The NMEA-0183 protocol message holder class
    /// 
    /// </summary>
    public class NMEAMessage
    {
        /// <summary>
        /// Global access to message parameters. Unfilled values will have null value.
        /// </summary>
        public string[] Parameters;

        /// <summary>
        /// constructing by not using inheritance
        /// </summary>
        /// <param name="fields">message fields</param>
        public NMEAMessage(params string[] fields)
        {
            Parameters = fields;
        }

        /// <summary>
        /// constructor usable for inheriting builders
        /// </summary>
        /// <param name="first">Sender and command</param>
        /// <param name="paramcount">Count of additional parameters in message</param>
        protected NMEAMessage(string first, int paramcount = 0)
        {
            Parameters = new string[paramcount + 1];
            Parameters[0] = first;
        }


        /// <summary>
        /// Build the NMEA-0183 message protocol
        /// </summary>
        /// <returns>The message as string</returns>
        public override string ToString()
        {
            if ((Parameters == null) || (Parameters.Length == 0))
                return null;

            for (int i = 0; i < Parameters.Length; i++)
                if (Parameters[i] == null)
                    Parameters[i] = "";

            string txt = string.Join(",", Parameters);
            byte crc = 0;
            foreach (char c in txt)
                crc ^= (byte)c;

            StringBuilder sb = new StringBuilder(txt.Length + 4);

            sb.Append('$');
            sb.Append(txt);
            sb.Append('*');
            sb.AppendFormat(_ci, "{0:X2}", crc);
            return sb.ToString();
        }



        #region NMEA Getters
        protected static readonly CultureInfo ci = CultureInfo.InvariantCulture;

        public double GetDbl(int index)
        {
            if (index >= Parameters.Length)
                throw new BadImageFormatException();

            string s = Parameters[index];
            if (string.IsNullOrWhiteSpace(s))
                throw new BadImageFormatException();

            return double.Parse(s, ci);
        }

        public int GetInt(int index)
        {
            if (index >= Parameters.Length)
                throw new BadImageFormatException();

            string s = Parameters[index];
            if (string.IsNullOrWhiteSpace(s))
                throw new BadImageFormatException();

            return int.Parse(s, ci);
        }


        public string GetStr(int index)
        {
            if (index >= Parameters.Length)
                return null;

            string s = Parameters[index];

            if (string.IsNullOrWhiteSpace(s))
                return null;

            return s;
        }

        public double GetGpsLatLon(int index)
        {
            if ((index + 1) >= Parameters.Length)
                throw new BadImageFormatException();

            string s = Parameters[index];
            string s2 = Parameters[index + 1];

            if (string.IsNullOrWhiteSpace(s) || string.IsNullOrWhiteSpace(s2))
                throw new BadImageFormatException();

            if (s2[0] == 'N')
                return int.Parse(s.Substring(0, 2), ci) + double.Parse(s.Substring(2), ci) / 60.0;
            else if (s2[0] == 'S')
                return -int.Parse(s.Substring(0, 2), ci) + -double.Parse(s.Substring(2), ci) / 60.0;
            else if (s2[0] == 'E')
                return int.Parse(s.Substring(0, 3), ci) + double.Parse(s.Substring(3), ci) / 60.0;
            else if (s2[0] == 'W')
                return -int.Parse(s.Substring(0, 3), ci) + -double.Parse(s.Substring(3), ci) / 60.0;
            else
                throw new BadImageFormatException();
        }

        
        #endregion

        private static CultureInfo _ci = CultureInfo.InvariantCulture;

        /// <summary>
        /// Get fields from found message
        /// </summary>
        /// <param name="message">$(message)*CC</param>
        /// <returns></returns>
        public static NMEAMessage Parse(string message)
        {
            if (string.IsNullOrEmpty(message))
                return null;

            // skip first $ and ending *CC
            var fields= message
                .Substring(1, message.Length - 4)
                .Split(new[] { ',' }, StringSplitOptions.None);

            return new NMEAMessage(fields);
        }
    }
}
