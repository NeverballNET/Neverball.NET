//  The original source code has been modified by AltSoftLab Inc. 2011-2021
//  This source code is provided "as is" without express or implied warranty of any kind.
//
//  AltSoftLab on Facebook  - http://www.facebook.com/AltSoftLab
//  AltSoftLab on Twitter   - http://www.twitter.com/AltSoftLab
//  AltSoftLab on Instagram - http://www.instagram.com/AltSoftLab
//  Join discussion in Unity forums      - http://forum.unity3d.com/threads/335966
//  Join discussion in AltSoftLab forums - http://www.AltSoftLab.com/forums.aspx
//  Contact us by mail mailto:AltSoftLab@AltSoftLab.com for feedback, bug reports or suggestions.
//  Visit http://www.AltSoftLab.com/ for more info.


#if ALTSDK_1_0


using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

using Alt.Sketch;
using Alt.Threading;


namespace Alt.PCL
{
    partial class PCLTools
    {
#if !SILVERLIGHT
        public static System.Collections.Specialized.StringCollection ToStringCollection(Alt.Collections.Specialized.StringCollection src)
        {
            System.Collections.Specialized.StringCollection dest = new System.Collections.Specialized.StringCollection();

            foreach (string s in src)
            {
                dest.Add(s);
            }

            return dest;
        }

        public static Alt.Collections.Specialized.StringCollection ToStringCollection(System.Collections.Specialized.StringCollection src)
        {
            Alt.Collections.Specialized.StringCollection dest = new Alt.Collections.Specialized.StringCollection();

            foreach (string s in src)
            {
                dest.Add(s);
            }

            return dest;
        }
#endif


        public static System.IO.DirectoryInfo ToDirectoryInfo(Alt.IO.DirectoryInfo src)
        {
            return new System.IO.DirectoryInfo(src.FullName);
        }

        public static Alt.IO.DirectoryInfo ToDirectoryInfo(System.IO.DirectoryInfo src)
        {
            return new Alt.IO.DirectoryInfo(src.FullName);
        }


#if !SILVERLIGHT && !UNITY_WEBPLAYER
        public static System.Diagnostics.ProcessWindowStyle ToProcessWindowStyle(Alt.Diagnostics.ProcessWindowStyle src)
        {
            switch (src)
            {
                case Alt.Diagnostics.ProcessWindowStyle.Normal:
                    return System.Diagnostics.ProcessWindowStyle.Normal;
                case Alt.Diagnostics.ProcessWindowStyle.Hidden:
                    return System.Diagnostics.ProcessWindowStyle.Hidden;
                case Alt.Diagnostics.ProcessWindowStyle.Minimized:
                    return System.Diagnostics.ProcessWindowStyle.Minimized;
                case Alt.Diagnostics.ProcessWindowStyle.Maximized:
                    return System.Diagnostics.ProcessWindowStyle.Maximized;
            }

            return System.Diagnostics.ProcessWindowStyle.Normal;
        }
#endif


        public static System.IO.FileMode ToFileMode(Alt.IO.FileMode src)
        {
            switch (src)
            {
                case Alt.IO.FileMode.CreateNew:
                    return System.IO.FileMode.CreateNew;
                case Alt.IO.FileMode.Create:
                    return System.IO.FileMode.Create;
                case Alt.IO.FileMode.Open:
                    return System.IO.FileMode.Open;
                case Alt.IO.FileMode.OpenOrCreate:
                    return System.IO.FileMode.OpenOrCreate;
                case Alt.IO.FileMode.Truncate:
                    return System.IO.FileMode.Truncate;
                case Alt.IO.FileMode.Append:
                    return System.IO.FileMode.Append;
            }

            return (System.IO.FileMode)0;
        }

        public static System.IO.FileAccess ToFileAccess(Alt.IO.FileAccess src)
        {
            System.IO.FileAccess dest = (System.IO.FileAccess)0;

            if ((src & Alt.IO.FileAccess.Read) == Alt.IO.FileAccess.Read)
                dest |= System.IO.FileAccess.Read;
            if ((src & Alt.IO.FileAccess.Write) == Alt.IO.FileAccess.Write)
                dest |= System.IO.FileAccess.Write;
            if ((src & Alt.IO.FileAccess.ReadWrite) == Alt.IO.FileAccess.ReadWrite)
                dest |= System.IO.FileAccess.ReadWrite;

            return dest;
        }

        public static System.IO.FileShare ToFileShare(Alt.IO.FileShare src)
        {
            System.IO.FileShare dest = (System.IO.FileShare)0;

            if ((src & Alt.IO.FileShare.None) == Alt.IO.FileShare.None)
                dest |= System.IO.FileShare.None;
            if ((src & Alt.IO.FileShare.Read) == Alt.IO.FileShare.Read)
                dest |= System.IO.FileShare.Read;
            if ((src & Alt.IO.FileShare.Write) == Alt.IO.FileShare.Write)
                dest |= System.IO.FileShare.Write;
            if ((src & Alt.IO.FileShare.ReadWrite) == Alt.IO.FileShare.ReadWrite)
                dest |= System.IO.FileShare.ReadWrite;
            if ((src & Alt.IO.FileShare.Delete) == Alt.IO.FileShare.Delete)
                dest |= System.IO.FileShare.Delete;
            if ((src & Alt.IO.FileShare.Inheritable) == Alt.IO.FileShare.Inheritable)
                dest |= System.IO.FileShare.Inheritable;

            return dest;
        }


        public static Alt.Threading.ThreadState ToThreadState(System.Threading.ThreadState src)
        {
            switch (src)
            {
                case System.Threading.ThreadState.Running:
                    return Alt.Threading.ThreadState.Running;
                case System.Threading.ThreadState.StopRequested:
                    return Alt.Threading.ThreadState.StopRequested;
                case System.Threading.ThreadState.SuspendRequested:
                    return Alt.Threading.ThreadState.SuspendRequested;
                case System.Threading.ThreadState.Background:
                    return Alt.Threading.ThreadState.Background;
                case System.Threading.ThreadState.Unstarted:
                    return Alt.Threading.ThreadState.Unstarted;
                case System.Threading.ThreadState.Stopped:
                    return Alt.Threading.ThreadState.Stopped;
                case System.Threading.ThreadState.WaitSleepJoin:
                    return Alt.Threading.ThreadState.WaitSleepJoin;
                case System.Threading.ThreadState.Suspended:
                    return Alt.Threading.ThreadState.Suspended;
                case System.Threading.ThreadState.AbortRequested:
                    return Alt.Threading.ThreadState.AbortRequested;
                case System.Threading.ThreadState.Aborted:
                    return Alt.Threading.ThreadState.Aborted;
            }

            return Alt.Threading.ThreadState.Unstarted;
        }


#if !SILVERLIGHT
        public static System.Threading.ApartmentState ToApartmentState(Alt.Threading.ApartmentState src)
        {
            switch (src)
            {
                case Alt.Threading.ApartmentState.STA:
                    return System.Threading.ApartmentState.STA;
                case Alt.Threading.ApartmentState.MTA:
                    return System.Threading.ApartmentState.MTA;
                case Alt.Threading.ApartmentState.Unknown:
                    return System.Threading.ApartmentState.Unknown;
            }

            return System.Threading.ApartmentState.Unknown;
        }

        public static Alt.Threading.ApartmentState ToApartmentState(System.Threading.ApartmentState src)
        {
            switch (src)
            {
                case System.Threading.ApartmentState.STA:
                    return Alt.Threading.ApartmentState.STA;
                case System.Threading.ApartmentState.MTA:
                    return Alt.Threading.ApartmentState.MTA;
                case System.Threading.ApartmentState.Unknown:
                    return Alt.Threading.ApartmentState.Unknown;
            }

            return Alt.Threading.ApartmentState.Unknown;
        }


        public static System.Threading.ThreadPriority ToThreadPriority(Alt.Threading.ThreadPriority src)
        {
            switch (src)
            {
                case Alt.Threading.ThreadPriority.Lowest:
                    return System.Threading.ThreadPriority.Lowest;
                case Alt.Threading.ThreadPriority.BelowNormal:
                    return System.Threading.ThreadPriority.BelowNormal;
                case Alt.Threading.ThreadPriority.Normal:
                    return System.Threading.ThreadPriority.Normal;
                case Alt.Threading.ThreadPriority.AboveNormal:
                    return System.Threading.ThreadPriority.AboveNormal;
                case Alt.Threading.ThreadPriority.Highest:
                    return System.Threading.ThreadPriority.Highest;
            }

            return System.Threading.ThreadPriority.Normal;
        }

        public static Alt.Threading.ThreadPriority ToThreadPriority(System.Threading.ThreadPriority src)
        {
            switch (src)
            {
                case System.Threading.ThreadPriority.Lowest:
                    return Alt.Threading.ThreadPriority.Lowest;
                case System.Threading.ThreadPriority.BelowNormal:
                    return Alt.Threading.ThreadPriority.BelowNormal;
                case System.Threading.ThreadPriority.Normal:
                    return Alt.Threading.ThreadPriority.Normal;
                case System.Threading.ThreadPriority.AboveNormal:
                    return Alt.Threading.ThreadPriority.AboveNormal;
                case System.Threading.ThreadPriority.Highest:
                    return Alt.Threading.ThreadPriority.Highest;
            }

            return Alt.Threading.ThreadPriority.Normal;
        }
#endif


        public static System.PlatformID ToPlatformID(Alt.PlatformID src)
        {
            switch (src)
            {
                case Alt.PlatformID.Win32S:
                    return System.PlatformID.Win32S;
                case Alt.PlatformID.Win32Windows:
                    return System.PlatformID.Win32Windows;
                case Alt.PlatformID.Win32NT:
                    return System.PlatformID.Win32NT;
                case Alt.PlatformID.WinCE:
                    return System.PlatformID.WinCE;
                case Alt.PlatformID.Unix:
                    return System.PlatformID.Unix;
                case Alt.PlatformID.Xbox:
                    return System.PlatformID.Xbox;
                case Alt.PlatformID.MacOSX:
                    return System.PlatformID.MacOSX;
            }

            return System.PlatformID.Win32Windows;
        }

        public static Alt.PlatformID ToPlatformID(System.PlatformID src)
        {
            switch (src)
            {
                case System.PlatformID.Win32S:
                    return Alt.PlatformID.Win32S;
                case System.PlatformID.Win32Windows:
                    return Alt.PlatformID.Win32Windows;
                case System.PlatformID.Win32NT:
                    return Alt.PlatformID.Win32NT;
                case System.PlatformID.WinCE:
                    return Alt.PlatformID.WinCE;
                case System.PlatformID.Unix:
                    return Alt.PlatformID.Unix;
                case System.PlatformID.Xbox:
                    return Alt.PlatformID.Xbox;
                case System.PlatformID.MacOSX:
                    return Alt.PlatformID.MacOSX;
            }

            return Alt.PlatformID.Unknown;
        }


        public static System.IO.FileAttributes ToFileAttributes(Alt.IO.FileAttributes src)
        {
            System.IO.FileAttributes dest = (System.IO.FileAttributes)0;

            if ((src & Alt.IO.FileAttributes.ReadOnly) == Alt.IO.FileAttributes.ReadOnly)
                dest |= System.IO.FileAttributes.ReadOnly;
            if ((src & Alt.IO.FileAttributes.Hidden) == Alt.IO.FileAttributes.Hidden)
                dest |= System.IO.FileAttributes.Hidden;
            if ((src & Alt.IO.FileAttributes.System) == Alt.IO.FileAttributes.System)
                dest |= System.IO.FileAttributes.System;
            if ((src & Alt.IO.FileAttributes.Directory) == Alt.IO.FileAttributes.Directory)
                dest |= System.IO.FileAttributes.Directory;
            if ((src & Alt.IO.FileAttributes.Archive) == Alt.IO.FileAttributes.Archive)
                dest |= System.IO.FileAttributes.Archive;
            if ((src & Alt.IO.FileAttributes.Device) == Alt.IO.FileAttributes.Device)
                dest |= System.IO.FileAttributes.Device;
            if ((src & Alt.IO.FileAttributes.Normal) == Alt.IO.FileAttributes.Normal)
                dest |= System.IO.FileAttributes.Normal;
            if ((src & Alt.IO.FileAttributes.Temporary) == Alt.IO.FileAttributes.Temporary)
                dest |= System.IO.FileAttributes.Temporary;
            if ((src & Alt.IO.FileAttributes.SparseFile) == Alt.IO.FileAttributes.SparseFile)
                dest |= System.IO.FileAttributes.SparseFile;
            if ((src & Alt.IO.FileAttributes.ReparsePoint) == Alt.IO.FileAttributes.ReparsePoint)
                dest |= System.IO.FileAttributes.ReparsePoint;
            if ((src & Alt.IO.FileAttributes.Compressed) == Alt.IO.FileAttributes.Compressed)
                dest |= System.IO.FileAttributes.Compressed;
            if ((src & Alt.IO.FileAttributes.Offline) == Alt.IO.FileAttributes.Offline)
                dest |= System.IO.FileAttributes.Offline;
            if ((src & Alt.IO.FileAttributes.NotContentIndexed) == Alt.IO.FileAttributes.NotContentIndexed)
                dest |= System.IO.FileAttributes.NotContentIndexed;
            if ((src & Alt.IO.FileAttributes.Encrypted) == Alt.IO.FileAttributes.Encrypted)
                dest |= System.IO.FileAttributes.Encrypted;

            return dest;
        }

        public static Alt.IO.FileAttributes ToFileAttributes(System.IO.FileAttributes src)
        {
            Alt.IO.FileAttributes dest = (Alt.IO.FileAttributes)0;

            if ((src & System.IO.FileAttributes.ReadOnly) == System.IO.FileAttributes.ReadOnly)
                dest |= Alt.IO.FileAttributes.ReadOnly;
            if ((src & System.IO.FileAttributes.Hidden) == System.IO.FileAttributes.Hidden)
                dest |= Alt.IO.FileAttributes.Hidden;
            if ((src & System.IO.FileAttributes.System) == System.IO.FileAttributes.System)
                dest |= Alt.IO.FileAttributes.System;
            if ((src & System.IO.FileAttributes.Directory) == System.IO.FileAttributes.Directory)
                dest |= Alt.IO.FileAttributes.Directory;
            if ((src & System.IO.FileAttributes.Archive) == System.IO.FileAttributes.Archive)
                dest |= Alt.IO.FileAttributes.Archive;
            if ((src & System.IO.FileAttributes.Device) == System.IO.FileAttributes.Device)
                dest |= Alt.IO.FileAttributes.Device;
            if ((src & System.IO.FileAttributes.Normal) == System.IO.FileAttributes.Normal)
                dest |= Alt.IO.FileAttributes.Normal;
            if ((src & System.IO.FileAttributes.Temporary) == System.IO.FileAttributes.Temporary)
                dest |= Alt.IO.FileAttributes.Temporary;
            if ((src & System.IO.FileAttributes.SparseFile) == System.IO.FileAttributes.SparseFile)
                dest |= Alt.IO.FileAttributes.SparseFile;
            if ((src & System.IO.FileAttributes.ReparsePoint) == System.IO.FileAttributes.ReparsePoint)
                dest |= Alt.IO.FileAttributes.ReparsePoint;
            if ((src & System.IO.FileAttributes.Compressed) == System.IO.FileAttributes.Compressed)
                dest |= Alt.IO.FileAttributes.Compressed;
            if ((src & System.IO.FileAttributes.Offline) == System.IO.FileAttributes.Offline)
                dest |= Alt.IO.FileAttributes.Offline;
            if ((src & System.IO.FileAttributes.NotContentIndexed) == System.IO.FileAttributes.NotContentIndexed)
                dest |= Alt.IO.FileAttributes.NotContentIndexed;
            if ((src & System.IO.FileAttributes.Encrypted) == System.IO.FileAttributes.Encrypted)
                dest |= Alt.IO.FileAttributes.Encrypted;

            return dest;
        }


        public static Alt.Environment.SpecialFolder ToSpecialFolder(System.Environment.SpecialFolder src)
        {
            switch (src)
            {
                case System.Environment.SpecialFolder.Desktop:
                    return Alt.Environment.SpecialFolder.Desktop;

                case System.Environment.SpecialFolder.Programs:
                    return Alt.Environment.SpecialFolder.Programs;

                case System.Environment.SpecialFolder.Personal:
                    //Equals to Personal - case System.Environment.SpecialFolder.MyDocuments:
                    return Alt.Environment.SpecialFolder.Personal;

                case System.Environment.SpecialFolder.Favorites:
                    return Alt.Environment.SpecialFolder.Favorites;

                case System.Environment.SpecialFolder.Startup:
                    return Alt.Environment.SpecialFolder.Startup;

                case System.Environment.SpecialFolder.Recent:
                    return Alt.Environment.SpecialFolder.Recent;

                case System.Environment.SpecialFolder.SendTo:
                    return Alt.Environment.SpecialFolder.SendTo;

                case System.Environment.SpecialFolder.StartMenu:
                    return Alt.Environment.SpecialFolder.StartMenu;

                case System.Environment.SpecialFolder.MyMusic:
                    return Alt.Environment.SpecialFolder.MyMusic;

                case System.Environment.SpecialFolder.DesktopDirectory:
                    return Alt.Environment.SpecialFolder.DesktopDirectory;

                case System.Environment.SpecialFolder.MyComputer:
                    return Alt.Environment.SpecialFolder.MyComputer;

                case System.Environment.SpecialFolder.Templates:
                    return Alt.Environment.SpecialFolder.Templates;

                case System.Environment.SpecialFolder.ApplicationData:
                    return Alt.Environment.SpecialFolder.ApplicationData;

                case System.Environment.SpecialFolder.LocalApplicationData:
                    return Alt.Environment.SpecialFolder.LocalApplicationData;

                case System.Environment.SpecialFolder.InternetCache:
                    return Alt.Environment.SpecialFolder.InternetCache;

                case System.Environment.SpecialFolder.Cookies:
                    return Alt.Environment.SpecialFolder.Cookies;

                case System.Environment.SpecialFolder.History:
                    return Alt.Environment.SpecialFolder.History;

                case System.Environment.SpecialFolder.CommonApplicationData:
                    return Alt.Environment.SpecialFolder.CommonApplicationData;

                case System.Environment.SpecialFolder.System:
                    return Alt.Environment.SpecialFolder.System;

                case System.Environment.SpecialFolder.ProgramFiles:
                    return Alt.Environment.SpecialFolder.ProgramFiles;

                case System.Environment.SpecialFolder.MyPictures:
                    return Alt.Environment.SpecialFolder.MyPictures;

                case System.Environment.SpecialFolder.CommonProgramFiles:
                    return Alt.Environment.SpecialFolder.CommonProgramFiles;


                //  NET40
#if ALTSDKPCL
                case System.Environment.SpecialFolder.MyVideos:
                    return Alt.Environment.SpecialFolder.MyVideos;

#if !SILVERLIGHT
                case System.Environment.SpecialFolder.NetworkShortcuts:
                    return Alt.Environment.SpecialFolder.NetworkShortcuts;

                case System.Environment.SpecialFolder.Fonts:
                    return Alt.Environment.SpecialFolder.Fonts;

                case System.Environment.SpecialFolder.CommonStartMenu:
                    return Alt.Environment.SpecialFolder.CommonStartMenu;

                case System.Environment.SpecialFolder.CommonPrograms:
                    return Alt.Environment.SpecialFolder.CommonPrograms;

                case System.Environment.SpecialFolder.CommonStartup:
                    return Alt.Environment.SpecialFolder.CommonStartup;

                case System.Environment.SpecialFolder.CommonDesktopDirectory:
                    return Alt.Environment.SpecialFolder.CommonDesktopDirectory;

                case System.Environment.SpecialFolder.PrinterShortcuts:
                    return Alt.Environment.SpecialFolder.PrinterShortcuts;

                case System.Environment.SpecialFolder.Windows:
                    return Alt.Environment.SpecialFolder.Windows;

                case System.Environment.SpecialFolder.UserProfile:
                    return Alt.Environment.SpecialFolder.UserProfile;

                case System.Environment.SpecialFolder.SystemX86:
                    return Alt.Environment.SpecialFolder.SystemX86;

                case System.Environment.SpecialFolder.ProgramFilesX86:
                    return Alt.Environment.SpecialFolder.ProgramFilesX86;

                case System.Environment.SpecialFolder.CommonProgramFilesX86:
                    return Alt.Environment.SpecialFolder.CommonProgramFilesX86;

                case System.Environment.SpecialFolder.CommonTemplates:
                    return Alt.Environment.SpecialFolder.CommonTemplates;

                case System.Environment.SpecialFolder.CommonDocuments:
                    return Alt.Environment.SpecialFolder.CommonDocuments;

                case System.Environment.SpecialFolder.CommonAdminTools:
                    return Alt.Environment.SpecialFolder.CommonAdminTools;

                case System.Environment.SpecialFolder.AdminTools:
                    return Alt.Environment.SpecialFolder.AdminTools;

                case System.Environment.SpecialFolder.CommonMusic:
                    return Alt.Environment.SpecialFolder.CommonMusic;

                case System.Environment.SpecialFolder.CommonPictures:
                    return Alt.Environment.SpecialFolder.CommonPictures;

                case System.Environment.SpecialFolder.CommonVideos:
                    return Alt.Environment.SpecialFolder.CommonVideos;

                case System.Environment.SpecialFolder.Resources:
                    return Alt.Environment.SpecialFolder.Resources;

                case System.Environment.SpecialFolder.LocalizedResources:
                    return Alt.Environment.SpecialFolder.LocalizedResources;

                case System.Environment.SpecialFolder.CommonOemLinks:
                    return Alt.Environment.SpecialFolder.CommonOemLinks;

                case System.Environment.SpecialFolder.CDBurning:
                    return Alt.Environment.SpecialFolder.CDBurning;
#endif

#endif
            }

            return Alt.Environment.SpecialFolder.Desktop;
        }

        public static System.Environment.SpecialFolder ToSpecialFolder(Alt.Environment.SpecialFolder src)
        {
            switch (src)
            {
                case Alt.Environment.SpecialFolder.Desktop:
                    return System.Environment.SpecialFolder.Desktop;

                case Alt.Environment.SpecialFolder.Programs:
                    return System.Environment.SpecialFolder.Programs;

                case Alt.Environment.SpecialFolder.Personal:
                    //Equals to Personal - case Alt.Environment.SpecialFolder.MyDocuments:
                    return System.Environment.SpecialFolder.Personal;

                case Alt.Environment.SpecialFolder.Favorites:
                    return System.Environment.SpecialFolder.Favorites;

                case Alt.Environment.SpecialFolder.Startup:
                    return System.Environment.SpecialFolder.Startup;

                case Alt.Environment.SpecialFolder.Recent:
                    return System.Environment.SpecialFolder.Recent;

                case Alt.Environment.SpecialFolder.SendTo:
                    return System.Environment.SpecialFolder.SendTo;

                case Alt.Environment.SpecialFolder.StartMenu:
                    return System.Environment.SpecialFolder.StartMenu;

                case Alt.Environment.SpecialFolder.MyMusic:
                    return System.Environment.SpecialFolder.MyMusic;

                case Alt.Environment.SpecialFolder.DesktopDirectory:
                    return System.Environment.SpecialFolder.DesktopDirectory;

                case Alt.Environment.SpecialFolder.MyComputer:
                    return System.Environment.SpecialFolder.MyComputer;

                case Alt.Environment.SpecialFolder.Templates:
                    return System.Environment.SpecialFolder.Templates;

                case Alt.Environment.SpecialFolder.ApplicationData:
                    return System.Environment.SpecialFolder.ApplicationData;

                case Alt.Environment.SpecialFolder.LocalApplicationData:
                    return System.Environment.SpecialFolder.LocalApplicationData;

                case Alt.Environment.SpecialFolder.InternetCache:
                    return System.Environment.SpecialFolder.InternetCache;

                case Alt.Environment.SpecialFolder.Cookies:
                    return System.Environment.SpecialFolder.Cookies;

                case Alt.Environment.SpecialFolder.History:
                    return System.Environment.SpecialFolder.History;

                case Alt.Environment.SpecialFolder.CommonApplicationData:
                    return System.Environment.SpecialFolder.CommonApplicationData;

                case Alt.Environment.SpecialFolder.System:
                    return System.Environment.SpecialFolder.System;

                case Alt.Environment.SpecialFolder.ProgramFiles:
                    return System.Environment.SpecialFolder.ProgramFiles;

                case Alt.Environment.SpecialFolder.MyPictures:
                    return System.Environment.SpecialFolder.MyPictures;

                case Alt.Environment.SpecialFolder.CommonProgramFiles:
                    return System.Environment.SpecialFolder.CommonProgramFiles;


                //  NET40
#if ALTSDKPCL
                case Alt.Environment.SpecialFolder.MyVideos:
                    return System.Environment.SpecialFolder.MyVideos;

#if !SILVERLIGHT
                case Alt.Environment.SpecialFolder.NetworkShortcuts:
                    return System.Environment.SpecialFolder.NetworkShortcuts;

                case Alt.Environment.SpecialFolder.Fonts:
                    return System.Environment.SpecialFolder.Fonts;

                case Alt.Environment.SpecialFolder.CommonStartMenu:
                    return System.Environment.SpecialFolder.CommonStartMenu;

                case Alt.Environment.SpecialFolder.CommonPrograms:
                    return System.Environment.SpecialFolder.CommonPrograms;

                case Alt.Environment.SpecialFolder.CommonStartup:
                    return System.Environment.SpecialFolder.CommonStartup;

                case Alt.Environment.SpecialFolder.CommonDesktopDirectory:
                    return System.Environment.SpecialFolder.CommonDesktopDirectory;

                case Alt.Environment.SpecialFolder.PrinterShortcuts:
                    return System.Environment.SpecialFolder.PrinterShortcuts;

                case Alt.Environment.SpecialFolder.Windows:
                    return System.Environment.SpecialFolder.Windows;

                case Alt.Environment.SpecialFolder.UserProfile:
                    return System.Environment.SpecialFolder.UserProfile;

                case Alt.Environment.SpecialFolder.SystemX86:
                    return System.Environment.SpecialFolder.SystemX86;

                case Alt.Environment.SpecialFolder.ProgramFilesX86:
                    return System.Environment.SpecialFolder.ProgramFilesX86;

                case Alt.Environment.SpecialFolder.CommonProgramFilesX86:
                    return System.Environment.SpecialFolder.CommonProgramFilesX86;

                case Alt.Environment.SpecialFolder.CommonTemplates:
                    return System.Environment.SpecialFolder.CommonTemplates;

                case Alt.Environment.SpecialFolder.CommonDocuments:
                    return System.Environment.SpecialFolder.CommonDocuments;

                case Alt.Environment.SpecialFolder.CommonAdminTools:
                    return System.Environment.SpecialFolder.CommonAdminTools;

                case Alt.Environment.SpecialFolder.AdminTools:
                    return System.Environment.SpecialFolder.AdminTools;

                case Alt.Environment.SpecialFolder.CommonMusic:
                    return System.Environment.SpecialFolder.CommonMusic;

                case Alt.Environment.SpecialFolder.CommonPictures:
                    return System.Environment.SpecialFolder.CommonPictures;

                case Alt.Environment.SpecialFolder.CommonVideos:
                    return System.Environment.SpecialFolder.CommonVideos;

                case Alt.Environment.SpecialFolder.Resources:
                    return System.Environment.SpecialFolder.Resources;

                case Alt.Environment.SpecialFolder.LocalizedResources:
                    return System.Environment.SpecialFolder.LocalizedResources;

                case Alt.Environment.SpecialFolder.CommonOemLinks:
                    return System.Environment.SpecialFolder.CommonOemLinks;

                case Alt.Environment.SpecialFolder.CDBurning:
                    return System.Environment.SpecialFolder.CDBurning;
#endif

#endif
            }

            return System.Environment.SpecialFolder.Desktop;
        }
    }
}

#endif