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
using System.Text;

using Alt.IO;
using Alt.Sketch;


namespace Alt
{
    public static partial class AltIntegration
    {
        static bool m_IsInitialized = false;
        static bool IsInitialized
        {
            get
            {
#if UNITY_5
				return AltSDKUnityApplication.IsInitialized;
#else
                return m_IsInitialized;
#endif
            }
        }


        static void InitializePlatformSpecificTools()
        {
            /*Maybe later use AltSDK zip-storage for resources
            Alt.IO.VirtualFileSystem.AddZipStorage(
                "AltData.zip",
                Alt.SDK.Demo.Resources.AltDataZipFile);*/

            m_IsInitialized = true;
        }


        public static void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }

            PCL.PCLTools.Initialize(
                //  Not need to Load System Fornts
                false);

            InitializePlatformSpecificTools();
        
            /*Maybe useful
			InstalledFontCollection.AddFontFiles("AltData/Fonts");
			InstalledFontCollection.AddFontFiles("AltData/Fonts/Open-Sans");*/
		}


        public static void Tick()
        {
            Alt.GUI.Timer.SystemTick();
        }
    }
}

#endif
