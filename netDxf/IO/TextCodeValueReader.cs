#region netDxf library licensed under the MIT License
// 
//                       netDxf library
// Copyright (c) Daniel Carvajal (haplokuon@gmail.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// 
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace netDxf.IO
{
    internal class TextCodeValueReader :
        ICodeValueReader
    {
        #region private fields

        private readonly TextReader reader;
        private short code;
        private object value;
        private long currentPosition;

        #endregion

        #region constructors

        public TextCodeValueReader(TextReader reader)
        {
            this.reader = reader;
            code = 0;
            value = null;
            currentPosition = 0;
        }

        #endregion

        #region public properties

        public short Code
        {
            get { return code; }
        }

        public object Value
        {
            get { return value; }
        }

        public long CurrentPosition
        {
            get { return currentPosition; }
        }

        #endregion

        #region public methods

        public void Next()
        {
            string readCode = reader.ReadLine();
            if (readCode == null)
            {
                code = 0;
                value = DxfObjectCode.EndOfFile;
            }
            else
            {
                currentPosition += 1;
                if (!short.TryParse(readCode, NumberStyles.Integer, CultureInfo.InvariantCulture, out code))
                {
                    throw new Exception(string.Format("Code {0} not valid at line {1}", code, currentPosition));
                }
                value = ReadValue(reader.ReadLine());
                currentPosition += 1;
            }
        }

        public byte ReadByte()
        {
            return (byte) value;
        }

        public byte[] ReadBytes()
        {
            return (byte[]) value;
        }

        public short ReadShort()
        {
            return (short) value;
        }

        public int ReadInt()
        {
            return (int) value;
        }

        public long ReadLong()
        {
            return (long) value;
        }

        public bool ReadBool()
        {
            return (bool) value;
        }

        public double ReadDouble()
        {
            return (double) value;
        }

        public string ReadString()
        {
            return (string) value;
        }

        public string ReadHex()
        {
            return (string) value;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", code, value);
        }

        #endregion

        #region private methods

        private object ReadValue(string valueString)
        {
            if (code >= 0 && code <= 9) // string
            {
                return ReadString(valueString);
            }
            if (code >= 10 && code <= 39) // double precision 3D point value
            {
                return ReadDouble(valueString);
            }
            if (code >= 40 && code <= 59) // double precision floating point value
            {
                return ReadDouble(valueString);
            }
            if (code >= 60 && code <= 79) // 16-bit integer value
            {
                return ReadShort(valueString);
            }
            if (code >= 90 && code <= 99) // 32-bit integer value
            {
                return ReadInt(valueString);
            }
            if (code == 100) // string (255-character maximum; less for Unicode strings)
            {
                return ReadString(valueString);
            }
            if (code == 101) // string (255-character maximum; less for Unicode strings). This code is undocumented and seems to affect only the AcdsData in dxf version 2013
            {
                return ReadString(valueString);
            }
            if (code == 102) // string (255-character maximum; less for Unicode strings)
            {
                return ReadString(valueString);
            }
            if (code == 105) // string representing hexadecimal (hex) handle value
            {
                return ReadHex(valueString);
            }
            if (code >= 110 && code <= 119) // double precision floating point value
            {
                return ReadDouble(valueString);
            }
            if (code >= 120 && code <= 129) // double precision floating point value
            {
                return ReadDouble(valueString);
            }
            if (code >= 130 && code <= 139) // double precision floating point value
            {
                return ReadDouble(valueString);
            }
            if (code >= 140 && code <= 149) // double precision scalar floating-point value
            {
                return ReadDouble(valueString);
            }
            if (code >= 160 && code <= 169) // 64-bit integer value
            {
                return ReadLong(valueString);
            }
            if (code >= 170 && code <= 179) // 16-bit integer value
            {
                return ReadShort(valueString);
            }
            if (code >= 210 && code <= 239) // double precision scalar floating-point value
            {
                return ReadDouble(valueString);
            }
            if (code >= 270 && code <= 279) // 16-bit integer value
            {
                return ReadShort(valueString);
            }
            if (code >= 280 && code <= 289) // 16-bit integer value
            {
                return ReadShort(valueString);
            }
            if (code >= 290 && code <= 299) // byte (boolean flag value)
            {
                return ReadBool(valueString);
            }
            if (code >= 300 && code <= 309) // arbitrary text string
            {
                return ReadString(valueString);
            }
            if (code >= 310 && code <= 319) // string representing hex value of binary chunk
            {
                return ReadBytes(valueString);
            }
            if (code >= 320 && code <= 329) // string representing hex handle value
            {
                return ReadHex(valueString);
            }
            if (code >= 330 && code <= 369) // string representing hex object IDs
            {
                return ReadHex(valueString);
            }
            if (code >= 370 && code <= 379) // 16-bit integer value
            {
                return ReadShort(valueString);
            }
            if (code >= 380 && code <= 389) // 16-bit integer value
            {
                return ReadShort(valueString);
            }
            if (code >= 390 && code <= 399) // string representing hex handle value
            {
                return ReadHex(valueString);
            }
            if (code >= 400 && code <= 409) // 16-bit integer value
            {
                return ReadShort(valueString);
            }
            if (code >= 410 && code <= 419) // string
            {
                return ReadString(valueString);
            }
            if (code >= 420 && code <= 429) // 32-bit integer value
            {
                return ReadInt(valueString);
            }
            if (code >= 430 && code <= 439) // string
            {
                return ReadString(valueString);
            }
            if (code >= 440 && code <= 449) // 32-bit integer value
            {
                return ReadInt(valueString);
            }
            if (code >= 450 && code <= 459) // 32-bit integer value
            {
                return ReadInt(valueString);
            }
            if (code >= 460 && code <= 469) // double-precision floating-point value
            {
                return ReadDouble(valueString);
            }
            if (code >= 470 && code <= 479) // string
            {
                return ReadString(valueString);
            }
            if (code >= 480 && code <= 481) // string representing hex handle value
            {
                return ReadHex(valueString);
            }
            if (code == 999) // comment (string)
            {
                return ReadString(valueString);
            }
            if (code >= 1010 && code <= 1059) // double-precision floating-point value
            {
                return ReadDouble(valueString);
            }
            if (code >= 1000 && code <= 1003) // string (same limits as indicated with 0-9 code range)
            {
                return ReadString(valueString);
            }
            if (code == 1004) // string representing hex value of binary chunk
            {
                return ReadBytes(valueString);
            }
            if (code >= 1005 && code <= 1009) // string (same limits as indicated with 0-9 code range)
            {
                return ReadString(valueString);
            }
            if (code >= 1060 && code <= 1070) // 16-bit integer value
            {
                return ReadShort(valueString);
            }
            if (code == 1071) // 32-bit integer value
            {
                return ReadInt(valueString);
            }

            throw new Exception(string.Format("Code \"{0}\" not valid at line {1}", code, currentPosition));
        }

        //private byte ReadByte(string valueString)
        //{
        //    if (byte.TryParse(valueString, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.InvariantCulture, out byte result))
        //    {
        //        return result;
        //    }

        //    Debug.Assert(false, string.Format("Value \"{0}\" not valid at line {1}", valueString, this.currentPosition));

        //    return 0;
        //}

        private byte[] ReadBytes(string valueString)
        {
            List<byte> bytes = new List<byte>();
            for (int i = 0; i < valueString.Length; i++)
            {
                string hex = string.Concat(valueString[i], valueString[++i]);
                if (byte.TryParse(hex, NumberStyles.AllowHexSpecifier | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.InvariantCulture, out byte result))
                {
                    bytes.Add(result);
                }
                else
                {
                    Debug.Assert(false, string.Format("Value \"{0}\" not valid at line {1}", valueString, currentPosition));

                    return new byte[0];
                }
            }

            return bytes.ToArray();
        }

        private short ReadShort(string valueString)
        {
            if (short.TryParse(valueString, NumberStyles.Integer, CultureInfo.InvariantCulture, out short result))
            {
                return result;
            }

            Debug.Assert(false, string.Format("Value \"{0}\" not valid at line {1}", valueString, currentPosition));

            return 0;
        }

        private int ReadInt(string valueString)
        {
            if (int.TryParse(valueString, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
            {
                return result;
            }

            Debug.Assert(false, string.Format("Value \"{0}\" not valid at line {1}", valueString, currentPosition));

            return 0;
        }

        private long ReadLong(string valueString)
        {
            if (long.TryParse(valueString, NumberStyles.Integer, CultureInfo.InvariantCulture, out long result))
            {
                return result;
            }

            Debug.Assert(false, string.Format("Value \"{0}\" not valid at line {1}", valueString, currentPosition));

            return 0;
        }

        private bool ReadBool(string valueString)
        {
            if (byte.TryParse(valueString, NumberStyles.Integer, CultureInfo.InvariantCulture, out byte result))
            {
                return result > 0;
            }

            Debug.Assert(false, string.Format("Value \"{0}\" not valid at line {1}", valueString, currentPosition));

            return false;
        }

        private double ReadDouble(string valueString)
        {
            if (double.TryParse(valueString, NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
            {
                return result;
            }

            Debug.Assert(false, string.Format("Value \"{0}\" not valid at line {1}", valueString, currentPosition));

            return 0.0;
        }

        private string ReadString(string valueString)
        {
            return valueString;
        }

        private string ReadHex(string valueString)
        {
            if (long.TryParse(valueString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out long result))
            {
                return result.ToString("X");
            }

            Debug.Assert(false, string.Format("Value \"{0}\" not valid at line {1}", valueString, currentPosition));

            return string.Empty;
        }

        #endregion
    }
}