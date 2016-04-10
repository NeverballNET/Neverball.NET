//  The original source code has been ported to .NET with deep assistance by AltSoftLab in 2015-2016
//  The solution source code based on and requires AltSDK (visit http://www.AltSoftLab.com/ for more info),
//  and is provided "as is" without express or implied warranty of any kind.
//
//  The solution can still require several optimizations: some OpenGL display lists has been removed and
//  render logic changed to be more transparent and be possible to port to other render engines (maybe
//  MonoGame or Unity). Also vector arrays can be used for positions, texture coords & colors. Audio is
//  not implemented directly, but all sound calls directed to Audio class. Game menu ported partly.
//
//  Thanks so much to AltSoftLab for help!
//
//  AltSoftLab on Facebook      - http://www.facebook.com/AltSoftLab
//  AltSoftLab on Twitter       - http://www.twitter.com/AltSoftLab
//  AltSoftLab on Instagram     - http://www.instagram.com/AltSoftLab
//  AltSoftLab on Unity forums  - http://forum.unity3d.com/threads/335966
//  AltSoftLab website          - http://www.AltSoftLab.com


using System;

using Alt.Collections.Generic;
using Alt.IO;


namespace Neverball.NET
{
    class Binary
    {
        static byte[] ReadBuffer;
        static System.IO.MemoryStream ReadStream;
        static System.IO.BinaryReader Reader;


        static Binary()
        {
            ReadBuffer = new byte[4];
            ReadStream = new System.IO.MemoryStream(ReadBuffer);
            Reader = new System.IO.BinaryReader(ReadStream);
        }


        public static void get_float(IntPtr fin, ref float f)
        {
            ReadBuffer[0] = (byte)FileSystem.fs_getc(fin);
            ReadBuffer[1] = (byte)FileSystem.fs_getc(fin);
            ReadBuffer[2] = (byte)FileSystem.fs_getc(fin);
            ReadBuffer[3] = (byte)FileSystem.fs_getc(fin);

            ReadStream.Position = 0;
            f = Reader.ReadSingle();
        }


        public static void get_index(IntPtr fin, ref int i)
        {
            ReadBuffer[0] = (byte)FileSystem.fs_getc(fin);
            ReadBuffer[1] = (byte)FileSystem.fs_getc(fin);
            ReadBuffer[2] = (byte)FileSystem.fs_getc(fin);
            ReadBuffer[3] = (byte)FileSystem.fs_getc(fin);

            ReadStream.Position = 0;
            i = Reader.ReadInt32();
        }


        public static void get_array(IntPtr fin, float[] v, int n)
        {
            for (int i = 0; i < n; i++)
            {
                get_float(fin, ref v[i]);
            }
        }


        public static void get_string(IntPtr fin, ref string s, int max)
        {
            s = null;

            int c;
            while ((c = FileSystem.fs_getc(fin)) > 0)
            {
                if (max > 0)
                {
                    s += (char)c;
                    max--;
                }
                else
                {
                    break;
                }
            }
        }
    }
}
