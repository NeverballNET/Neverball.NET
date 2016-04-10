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
using Alt.Sketch;


namespace Neverball.NET
{
    public class Common
    {
        public const float RESPONSE = 0.05f;                /* Input smoothing time               */


        public const string AUD_MENU = "snd/menu.ogg";
        public const string AUD_START = "snd/select.ogg";
        public const string AUD_READY = "snd/ready.ogg";
        public const string AUD_SET = "snd/set.ogg";
        public const string AUD_GO = "snd/go.ogg";
        public const string AUD_BALL = "snd/ball.ogg";
        public const string AUD_BUMPS = "snd/bumplil.ogg";
        public const string AUD_BUMPM = "snd/bump.ogg";
        public const string AUD_BUMPL = "snd/bumpbig.ogg";
        public const string AUD_COIN = "snd/coin.ogg";
        public const string AUD_TICK = "snd/tick.ogg";
        public const string AUD_TOCK = "snd/tock.ogg";
        public const string AUD_SWITCH = "snd/switch.ogg";
        public const string AUD_JUMP = "snd/jump.ogg";
        public const string AUD_GOAL = "snd/goal.ogg";
        public const string AUD_SCORE = "snd/record.ogg";
        public const string AUD_FALL = "snd/fall.ogg";
        public const string AUD_TIME = "snd/time.ogg";
        public const string AUD_OVER = "snd/over.ogg";
        public const string AUD_GROW = "snd/grow.ogg";
        public const string AUD_SHRINK = "snd/shrink.ogg";


        /* Maximum value that can be returned by the rand function. */
        public const int RAND_MAX = 0x7fff;

        static Random m_Random;


        static Common()
        {
            m_Random = new Random(DateTime.Now.Second * DateTime.Now.Millisecond);
        }


        public static int rand()
        {
            return m_Random.Next();
        }


        public static string strip_newline(ref string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            int c_index = str.Length - 1;
            while (c_index >= 0 && (str[c_index] == '\n' || str[c_index] == '\r'))
            {
                str = str.Remove(c_index);
                c_index--;
            }

            return str;
        }
    }
}
