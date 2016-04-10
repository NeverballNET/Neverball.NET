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
    public class FileSystem
    {
        static int m_LastFileID = 1;
        class FileDescription : Alt.DisposableObject
        {
            public System.IO.Stream m_Stream;
            public System.IO.BinaryReader m_BinaryReader;

            public FileDescription(System.IO.Stream stream)
            {
                m_Stream = stream;
                m_BinaryReader = new System.IO.BinaryReader(stream, Alt.EncodingHelper.ASCII);
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
            }
        }
        static System.Collections.Generic.Dictionary<IntPtr, FileDescription> m_Files =
            new System.Collections.Generic.Dictionary<IntPtr, FileDescription>();


        public static IntPtr fs_open(string path)
        {
            IntPtr fid = IntPtr.Zero;

            //if (mode == "r")
            {
                string fn = "Data/" + path;
                if (VirtualFile.Exists(fn))
                {
                    System.IO.Stream fs = VirtualFile.OpenRead(fn);
                    if (fs != null)
                    {
                        fid = new IntPtr(m_LastFileID++);

                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, (int)fs.Length);
                        fs.Close();

                        fs = new System.IO.MemoryStream(buffer, false);

                        m_Files.Add(fid, new FileDescription(fs));

                        return fid;
                    }
                }
            }

            return fid;
        }

        static FileDescription GetStreamByID(IntPtr fid)
        {
            FileDescription fs;
            m_Files.TryGetValue(fid, out fs);

            return fs;
        }


        public static void fs_close(IntPtr fid)
        {
            FileDescription fs = GetStreamByID(fid);
            if (fs != null)
            {
                m_Files.Remove(fid);
                fs.Dispose();
            }
        }


        public static void fs_read(ref string data, int count, IntPtr fid)
        {
            FileDescription fs = GetStreamByID(fid);
            if (fs != null)
            {
                count = (int)System.Math.Min(count, fs.m_Stream.Length - fs.m_Stream.Position - 1);

                string new_data = "";
                while (count > 0)
                {
                    count--;

                    byte c = fs.m_BinaryReader.ReadByte();
                    if (c == 0)
                    {
                        fs.m_Stream.Position += count;
                        break;
                    }
                    else
                    {
                        new_data += (char)c;
                    }
                }

                data = new_data;
            }
        }


        public static void fs_read(ref System.Collections.Generic.List<string> data, int count, IntPtr fid)
        {
            FileDescription fs = GetStreamByID(fid);
            if (fs != null)
            {
                count = (int) System.Math.Min(count, fs.m_Stream.Length - fs.m_Stream.Position - 1);

                System.Collections.Generic.List<string> new_data = new System.Collections.Generic.List<string>();

                string current = "";
                while (count > 0)
                {
                    byte c = fs.m_BinaryReader.ReadByte();
                    if (c == 0)
                    {
                        new_data.Add(current);
                        current = "";
                    }
                    else
                    {
                        current += (char)c;
                    }

                    count--;
                }
                if (!string.IsNullOrEmpty(current))
                {
                    new_data.Add(current);
                }

                data = new_data;
            }
        }


        public static void fs_seek(IntPtr fid, int offset)
        {
            FileDescription fs = GetStreamByID(fid);
            if (fs != null)
            {
                fs.m_Stream.Position += offset;
            }
        }


        public static int fs_getc(IntPtr fid)
        {
            int c = 0;

            FileDescription fs = GetStreamByID(fid);
            if (fs != null)
            {
                int new_c = fs.m_BinaryReader.ReadByte();

                c = new_c;
            }

            return c;
        }


        public static string fs_gets(ref string dst, int count, IntPtr fid)
        {
            string s = null;

            FileDescription fs = GetStreamByID(fid);
            if (fs != null)
            {
                count = (int)System.Math.Min(count, fs.m_Stream.Length - fs.m_Stream.Position - 1);

                string current = "";

                char c;
                while (count > 0)
                {
                    count--;

                    c = (char)fs.m_BinaryReader.ReadByte();

                    /* Keep a newline and break. */

                    if (c == '\n')
                    {
                        current += c;
                        break;
                    }

                    /* Ignore carriage returns. */

                    if (c == '\r')
                    {
                        count++;
                    }
                    else
                    {
                        current += c;
                    }
                }

                if (string.IsNullOrEmpty(current))
                {
                    current = null;
                }

                s = current;
            }

            return (dst = s);
        }


        public static int read_line(ref string str, IntPtr fin)
        {
            string buff = null;

            string line = null;
            int len0, len1;

            while (fs_gets(ref buff, Config.MAXSTR, fin) != null)
            {
                /* Append to data read so far. */

                if (line != null)
                {
                    line += buff;
                }
                else
                {
                    line = buff;
                }

                /* Strip newline, if any. */

                len0 = line.Length;
                Common.strip_newline(ref line);
                len1 = line.Length;

                if (len1 != len0)
                {
                    /* We hit a newline, clean up and break. */
                    //NoNeed??? line = (char*) realloc(line, len1 + 1);
                    break;
                }
            }

            return (str = line) != null ? 1 : 0;
        }
    }
}
