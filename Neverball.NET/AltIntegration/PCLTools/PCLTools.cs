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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Alt.Diagnostics;
using Alt.Sketch;
using Alt.Threading;


namespace Alt.PCL
{
    sealed partial class PCLTools : IPCLTools
    {
        PCLTools(bool loadSystemFonts) :
            base(loadSystemFonts)
		{
		}

        internal static void Initialize()
        {
            Initialize(true);
        }
        internal static void Initialize(bool loadSystemFonts)
		{
			if (!IsInitialized)
			{
                new PCLTools(loadSystemFonts);
			}
		}


        static bool m_LoadSystemFonts_CalledOnce = false;
        protected override void LoadSystemFonts()
        {
            if (m_LoadSystemFonts_CalledOnce)
            {
                return;
            }
            m_LoadSystemFonts_CalledOnce = true;


            FontFamily[] families = new InstalledFontCollection().Families;
            if (families != null &&
                families.Length > 0)
            {
                return;
            }


#if PORTABLE || SILVERLIGHT
            string[] names = Enum.GetNames(typeof(System.Environment.SpecialFolder));
            foreach (string name in names)
            {
                if (string.Compare(name, "Fonts") == 0)
                {
                    base.LoadSystemFonts();
                    return;
                }
            }
#endif


            String fontsDir = null;
#if !WINDOWS_PHONE && !WINDOWS_PHONE_7 && !WINDOWS_PHONE_71
#if NET40 || ANDROID || __IOS__
            //  NET40 or higher
            fontsDir = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
#else
#if SILVERLIGHT && !WINDOWS_PHONE && !WINDOWS_PHONE_7 && !WINDOWS_PHONE_71
            try
            {
                fontsDir = Environment.GetFolderPath(Environment.SpecialFolder.System);

                // get parent of System folder to have Windows folder
                IO.DirectoryInfo dirWindowsFolder = new IO.DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.System));

                // Concatenate Fonts folder onto Windows folder.
                fontsDir = IO.Path.Combine(dirWindowsFolder.Parent.FullName, "Fonts");
            }
            catch
            {
                fontsDir = null;
            }
#else
            PlatformID pid = Environment.OSVersion.Platform;
            if (pid == PlatformID.Win32NT)
            {
                // get parent of System folder to have Windows folder
                IO.DirectoryInfo dirWindowsFolder = IO.Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.System));
				if (dirWindowsFolder != null)
				{
                	// Concatenate Fonts folder onto Windows folder.
                	fontsDir = IO.Path.Combine(dirWindowsFolder.FullName, "Fonts");
				}
            }
            else if (pid == PlatformID.Unix)
            {
                //Path.Combine(home, ".fonts");
                fontsDir = "usr/share/fonts/truetype";
            }
#if !WINDOWS_PHONE && !WINDOWS_PHONE_7 && !WINDOWS_PHONE_71
            else if (pid == PlatformID.MacOSX)
            {
                //  Path.Combine(home, "Library", "Fonts");
                fontsDir = "Library/Fonts";
            }
#endif
#endif
#endif
#endif

            if (!string.IsNullOrEmpty(fontsDir) &&
                IO.Directory.Exists(fontsDir))
            {
                InstalledFontCollection.LoadCache();
                {
                    try
                    {
                        InstalledFontCollection.AddFontFiles(fontsDir, true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                InstalledFontCollection.SaveCache();
            }


            //  Load additional font files (if no any system fonts loaded) and set default params
            //???   if ((new InstalledFontCollection()).Families.Length == 0)
            {
#if SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7 || WINDOWS_PHONE_71 || ANDROID || __IOS__
                Alt.Sketch.Config.Font_NoAntiAliasMaxSize = 5;
#endif

                InstalledFontCollection.AddFontFiles("AltData/Fonts");
                InstalledFontCollection.AddFontFiles("AltData/Fonts/Open-Sans");
            }
        }



        //  Process

        //protected abstract object Process_Start(ProcessStartInfo startInfo);
        protected override object Process_Start(ProcessStartInfo startInfo)
        {
#if !SILVERLIGHT
#if !UNITY_WEBPLAYER
            System.Diagnostics.ProcessStartInfo processStartInfo =
                new System.Diagnostics.ProcessStartInfo();// startInfo.FileName, startInfo.Arguments);
#else
			System.Diagnostics.Process process = new System.Diagnostics.Process();
			System.Diagnostics.ProcessStartInfo processStartInfo =
				process.StartInfo;
#endif

            processStartInfo.Arguments = startInfo.Arguments;
            processStartInfo.CreateNoWindow = startInfo.CreateNoWindow;
#if !UNITY_WEBPLAYER
            processStartInfo.Domain = startInfo.Domain;
            //processStartInfo.EnvironmentVariables { get; }
            processStartInfo.ErrorDialog = startInfo.ErrorDialog;
            processStartInfo.ErrorDialogParentHandle = startInfo.ErrorDialogParentHandle;
#endif
            processStartInfo.FileName = startInfo.FileName;
#if !UNITY_WEBPLAYER
			processStartInfo.LoadUserProfile = startInfo.LoadUserProfile;
#endif
            //CantCopy  processStartInfo.Password = startInfo.Password;
            processStartInfo.RedirectStandardError = startInfo.RedirectStandardError;
            processStartInfo.RedirectStandardInput = startInfo.RedirectStandardInput;
            processStartInfo.RedirectStandardOutput = startInfo.RedirectStandardOutput;
#if !UNITY_WEBPLAYER
			processStartInfo.StandardErrorEncoding = startInfo.StandardErrorEncoding;
            processStartInfo.StandardOutputEncoding = startInfo.StandardOutputEncoding;
            processStartInfo.UserName = startInfo.UserName;
#endif
			processStartInfo.UseShellExecute = startInfo.UseShellExecute;
#if !UNITY_WEBPLAYER
			processStartInfo.Verb = startInfo.Verb;
            //processStartInfo.Verbs { get; }
            processStartInfo.WindowStyle = ToProcessWindowStyle(startInfo.WindowStyle);
            processStartInfo.WorkingDirectory = startInfo.WorkingDirectory;
#endif

#if !UNITY_WEBPLAYER
            return System.Diagnostics.Process.Start(processStartInfo);
#else
			//NoNeed	bool result =
			process.Start();

			return process;
#endif
#else
            return null;
#endif
        }



        //  Environment

        //protected abstract bool Environment_Exit(int exitCode);
        protected override bool Environment_Exit(int exitCode)
        {
#if !SILVERLIGHT
            System.Environment.Exit(exitCode);

            //  processed
            return true;
#else
            return false;
#endif
        }


        //protected abstract string[] Environment_GetCommandLineArgs();
        protected override string[] Environment_GetCommandLineArgs()
        {
#if !SILVERLIGHT
            return System.Environment.GetCommandLineArgs();
#else
            return new string[0];
#endif
        }


        //protected abstract string Environment_GetEnvironmentVariable(string variable);
        protected override string Environment_GetEnvironmentVariable(string variable)
        {
#if !SILVERLIGHT
            return System.Environment.GetEnvironmentVariable(variable);
#else
            //  TEST
            return string.Empty;
#endif
        }

        //protected abstract IDictionary Environment_GetEnvironmentVariables();
        protected override IDictionary Environment_GetEnvironmentVariables()
        {
#if !SILVERLIGHT
            return System.Environment.GetEnvironmentVariables();
#else
            //  TEST
            return new System.Collections.Generic.Dictionary<string, string>();
#endif
        }


        //protected abstract string Environment_GetFolderPath(Environment.SpecialFolder folder);
        protected override string Environment_GetFolderPath(Environment.SpecialFolder folder)
        {
			try
			{
            	return System.Environment.GetFolderPath(ToSpecialFolder(folder));
			}
			catch
			{
				return string.Empty;
			}
        }


        //protected abstract OperatingSystem Environment_OSVersion
        protected override OperatingSystem Environment_OSVersion
        {
            get
            {
                System.OperatingSystem os = System.Environment.OSVersion;
                if (os == null)
                {
                    return new OperatingSystem(PlatformID.Unknown, new Version(0, 0));
                }

                return new OperatingSystem(ToPlatformID(os.Platform), os.Version);
            }
        }


        //protected abstract bool Environment_UserInteractive
        protected override bool Environment_UserInteractive
        {
            get
            {
                return System.Environment.UserInteractive;
            }
        }


        //protected abstract Version Environment_Version
        protected override Version Environment_Version
        {
            get
            {
                return System.Environment.Version;
            }
        }



        //  Threading

        //protected abstract IThread Thread_CurrentThread
        protected override IPCLThread Thread_CurrentThread
        {
            get
            {
                return new PCLThread(System.Threading.Thread.CurrentThread);
            }
        }


        //protected abstract IThread Thread_NewThread(ParameterizedThreadStart start);
        protected override IPCLThread Thread_NewThread(ParameterizedThreadStart start)
        {
            return new PCLThread(new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(start)));
        }


        //protected abstract IThread Thread_NewThread(ThreadStart start);
        protected override IPCLThread Thread_NewThread(ThreadStart start)
        {
            return new PCLThread(new System.Threading.Thread(new System.Threading.ThreadStart(start)));
        }


        //protected abstract void Thread_Sleep(int millisecondsTimeout);
        protected override void Thread_Sleep(int millisecondsTimeout)
        {
            System.Threading.Thread.Sleep(millisecondsTimeout);
        }



        //  IO

        //  File

        //protected abstract void File_Close(Stream stream);
        protected override void File_Close(System.IO.Stream stream)
        {
            if (stream == null)
            {
                return;
            }

            stream.Close();
        }


        //protected abstract long File_GetLength(string path);
        protected override long File_GetLength(string path)
        {
#if !UNITY_WEBPLAYER
            try
            {
                return (new System.IO.FileInfo(path)).Length;
            }
            catch
#endif
            {
                return 0;
            }
        }


        //protected abstract void File_Copy(string sourceFileName, string destFileName, bool overwrite);
        protected override void File_Copy(string sourceFileName, string destFileName, bool overwrite)
        {
            System.IO.File.Copy(sourceFileName, destFileName, overwrite);
        }


#if !UNITY_WEBPLAYER
        //protected virtual StreamWriter File_CreateText(string path)
        protected override System.IO.StreamWriter File_CreateText(string path)
        {
            return System.IO.File.CreateText(path);
        }
#endif


        //protected abstract void File_Delete(string path);
        protected override void File_Delete(string path)
        {
            System.IO.File.Delete(path);
        }


        //protected abstract bool File_Exists(string path);
        protected override bool File_Exists(string path)
        {
            return System.IO.File.Exists(path);
        }


        //protected abstract FileAttributes File_GetAttributes(string path);
        protected override IO.FileAttributes File_GetAttributes(string path)
        {
            return ToFileAttributes(System.IO.File.GetAttributes(path));
        }


        //protected abstract DateTime File_GetCreationTime(string path);
        protected override DateTime File_GetCreationTime(string path)
        {
#if !UNITY_WEBPLAYER
            return System.IO.File.GetCreationTime(path);
#else
			return DateTime.MinValue;
#endif
        }


        //protected abstract DateTime File_GetLastAccessTime(string path);
        protected override DateTime File_GetLastAccessTime(string path)
        {
#if !UNITY_WEBPLAYER
            return System.IO.File.GetLastAccessTime(path);
#else
			return DateTime.MinValue;
#endif
		}


        //protected abstract DateTime File_GetLastWriteTime(string path);
        protected override DateTime File_GetLastWriteTime(string path)
        {
#if !UNITY_WEBPLAYER
			return System.IO.File.GetLastWriteTime(path);
#else
			return DateTime.MinValue;
#endif
		}


        //protected abstract void File_Move(string sourceFileName, string destFileName);
        protected override void File_Move(string sourceFileName, string destFileName)
        {
#if !UNITY_WEBPLAYER
            System.IO.File.Move(sourceFileName, destFileName);
#endif
        }


        //protected abstract Stream File_Open(string path, FileMode mode, FileAccess access, FileShare share);
        protected override System.IO.Stream File_Open(string path, IO.FileMode mode, IO.FileAccess access, IO.FileShare share)
        {
            return System.IO.File.Open(path, ToFileMode(mode), ToFileAccess(access), ToFileShare(share));
        }


#if !UNITY_WEBPLAYER
        //protected virtual string File_ReadAllText(string path)
        protected override string File_ReadAllText(string path)
        {
            return System.IO.File.ReadAllText(path);
        }

        //protected virtual string File_ReadAllText(string path, Encoding encoding)
        protected override string File_ReadAllText(string path, Encoding encoding)
        {
            return System.IO.File.ReadAllText(path, encoding);
        }
#endif


        //protected abstract void File_SetAttributes(string path, FileAttributes fileAttributes);
        protected override void File_SetAttributes(string path, IO.FileAttributes fileAttributes)
        {
            System.IO.File.SetAttributes(path, ToFileAttributes(fileAttributes));
        }


#if !UNITY_WEBPLAYER
        //protected virtual void File_WriteAllText(string path, string contents)
        protected override void File_WriteAllText(string path, string contents)
        {
            System.IO.File.WriteAllText(path, contents);
        }

        //protected virtual void File_WriteAllText(string path, string contents, Encoding encoding)
        protected override void File_WriteAllText(string path, string contents, Encoding encoding)
        {
            System.IO.File.WriteAllText(path, contents, encoding);
        }
#endif



        //  Directory

        //protected abstract DirectoryInfo Directory_CreateDirectory(string path);
        protected override IO.DirectoryInfo Directory_CreateDirectory(string path)
        {
            return ToDirectoryInfo(System.IO.Directory.CreateDirectory(path));
        }


        //protected abstract void Directory_Delete(string path, bool recursive);
        protected override void Directory_Delete(string path, bool recursive)
        {
#if !UNITY_WEBPLAYER
            System.IO.Directory.Delete(path, recursive);
#endif
        }


        //protected abstract bool Directory_Exists(string path);
        protected override bool Directory_Exists(string path)
        {
			try
			{
            	return System.IO.Directory.Exists(path);
			}
			catch
			{
				return false;
			}
        }


        //protected abstract string Directory_GetCurrentDirectory();
        protected override string Directory_GetCurrentDirectory()
        {
            return System.IO.Directory.GetCurrentDirectory();
        }


        //protected abstract IEnumerable<string> Directory_GetDirectories(string path, string searchPattern);
        protected override IEnumerable<string> Directory_GetDirectories(string path, string searchPattern)
        {
            return System.IO.Directory.
#if !SILVERLIGHT
                GetDirectories
#else
                EnumerateDirectories
#endif
                (path, searchPattern);
        }


        //protected abstract IEnumerable<string> Directory_GetFiles(string path, string searchPattern);
        protected override IEnumerable<string> Directory_GetFiles(string path, string searchPattern)
        {
            return System.IO.Directory.
#if !SILVERLIGHT
                GetFiles
#else
                EnumerateFiles
#endif
                (path, searchPattern);
        }


        /*PROCESS (http://dotnetslackers.com/articles/silverlight/Folder-Dialog-in-Silverlight-4.aspx)
        public class Drive
        {
            public string DriveLetter { get; set; }
            public string VolumeName { get; set; }
        }

        public class Folder
        {
            public string FolderName { get; set; }
            public string FolderPath { get; set; }
        }*/
        //protected abstract IEnumerable<string> Directory_GetLogicalDrives();
        protected override IEnumerable<string> Directory_GetLogicalDrives()
        {
#if !SILVERLIGHT && !UNITY_WEBPLAYER
            return System.IO.Directory.GetLogicalDrives();
#else
#if !UNITY_WEBPLAYER
            //  PROCESS
            /*{
                dynamic fileSystem = AutomationFactory.CreateObject("Scripting.FileSystemObject");
                dynamic drives = fileSystem.Drives;
                foreach (var drive in drives)
                {
                    try
                    {
                        Drive dr = new Drive();
                        if (string.IsNullOrEmpty(drive.VolumeName))
                        {
                            dr.VolumeName = "Local Disk";
                        }
                        else
                        {
                            dr.VolumeName = drive.VolumeName;
                        }
                        dr.DriveLetter = drive.DriveLetter;
                        TreeViewItem tvi = new TreeViewItem();
                        tvi.HeaderTemplate = this.Resources["DriveHeaderTemplate"] as DataTemplate;
                        tvi.Header = string.Format("{0} ({1}:)", dr.VolumeName, dr.DriveLetter);
                        tvi.Tag = dr.DriveLetter;
                        tvi.Selected += new RoutedEventHandler(Drive_Selected);
                        MyComputerItem.Items.Add(tvi);
                    }
                    catch (COMException) { }
                }
            }*/
#endif
            //  TEST
            return new List<string>();
#endif
        }


        //protected virtual DirectoryInfo Directory_GetParent(string path)
        protected override IO.DirectoryInfo Directory_GetParent(string path)
        {
			try
			{
#if !SILVERLIGHT && !UNITY_WEBPLAYER
            	return ToDirectoryInfo(System.IO.Directory.GetParent(path));
#else
            	//  TEST
            	return base.Directory_GetParent(path);
#endif
			}
			catch
			{
			}

			return null;//new IO.DirectoryInfo(string.Empty);
        }


        //protected abstract void Directory_Move(string sourceDirName, string destDirName);
        protected override void Directory_Move(string sourceDirName, string destDirName)
        {
#if !UNITY_WEBPLAYER
            System.IO.Directory.Move(sourceDirName, destDirName);
#endif
        }


        //protected abstract void Directory_SetCurrentDirectory(string path);
        protected override void Directory_SetCurrentDirectory(string path)
        {
#if !SILVERLIGHT
            System.IO.Directory.SetCurrentDirectory(path);
#endif
        }



        //  Path

        //protected abstract char Path_AltDirectorySeparatorChar
        protected override char Path_AltDirectorySeparatorChar
        {
            get
            {
                return System.IO.Path.AltDirectorySeparatorChar;
            }
        }

        //protected abstract char Path_DirectorySeparatorChar
        protected override char Path_DirectorySeparatorChar
        {
            get
            {
                return System.IO.Path.DirectorySeparatorChar;
            }
        }

        //protected virtual char[] Path_InvalidPathChars
        protected override char[] Path_InvalidPathChars
        {
            get
            {
#if !SILVERLIGHT
                return System.IO.Path.GetInvalidPathChars();
#else
                return base.Path_InvalidPathChars;
#endif
            }
        }

        //protected abstract char Path_PathSeparator
        protected override char Path_PathSeparator
        {
            get
            {
                return System.IO.Path.PathSeparator;
            }
        }

        //protected abstract char Path_VolumeSeparatorChar
        protected override char Path_VolumeSeparatorChar
        {
            get
            {
                return System.IO.Path.VolumeSeparatorChar;
            }
        }


        //protected virtual string Path_ChangeExtension(string path, string extension)
        protected override string Path_ChangeExtension(string path, string extension)
        {
            return System.IO.Path.ChangeExtension(path, extension);
        }


        //protected virtual string Path_Combine(string path1, string path2)
        protected override string Path_Combine(string path1, string path2)
        {
            return System.IO.Path.Combine(path1, path2);
        }


        //protected virtual string Path_GetDirectoryName(string path)
        protected override string Path_GetDirectoryName(string path)
        {
            return System.IO.Path.GetDirectoryName(path);
        }


        //protected virtual string Path_GetExtension(string path)
        protected override string Path_GetExtension(string path)
        {
            return System.IO.Path.GetExtension(path);
        }


        //protected virtual string Path_GetFileName(string path)
        protected override string Path_GetFileName(string path)
        {
            return System.IO.Path.GetFileName(path);
        }


        //protected virtual string Path_GetFileNameWithoutExtension(string path)
        protected override string Path_GetFileNameWithoutExtension(string path)
        {
            return System.IO.Path.GetFileNameWithoutExtension(path);
        }


        //protected virtual string Path_GetFullPath(string path)
        protected override string Path_GetFullPath(string path)
        {
            return System.IO.Path.GetFullPath(path);
        }


        //protected virtual char[] Path_GetInvalidFileNameChars()
        protected override char[] Path_GetInvalidFileNameChars()
        {
#if !SILVERLIGHT
            return System.IO.Path.GetInvalidFileNameChars();
#else
            return base.Path_GetInvalidFileNameChars();
#endif
        }


        //protected virtual char[] Path_GetInvalidPathChars()
        protected override char[] Path_GetInvalidPathChars()
        {
            return System.IO.Path.GetInvalidPathChars();
        }


        //protected virtual string Path_GetPathRoot(string path)
        protected override string Path_GetPathRoot(string path)
        {
            return System.IO.Path.GetPathRoot(path);
        }


        //protected virtual string Path_GetRandomFileName()
        protected override string Path_GetRandomFileName()
        {
#if !SILVERLIGHT
            return System.IO.Path.GetRandomFileName();
#else
            return base.Path_GetRandomFileName();
#endif
        }


        //public static string sPath_GetTempFileName()
        protected override string Path_GetTempFileName()
        {
            return System.IO.Path.GetTempFileName();
        }


        //protected abstract string Path_GetTempPath();
        protected override string Path_GetTempPath()
        {
            return System.IO.Path.GetTempPath();
        }


        //protected virtual bool Path_HasExtension(string path)
        protected override bool Path_HasExtension(string path)
        {
            return System.IO.Path.HasExtension(path);
        }


        //protected virtual bool Path_IsPathRooted(string path)
        protected override bool Path_IsPathRooted(string path)
        {
            return System.IO.Path.IsPathRooted(path);
        }
    }
}

#endif
