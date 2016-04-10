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
    /*
     * Global settings are stored in USER_CONFIG_FILE.  Replays are stored
     * in  USER_REPLAY_FILE.  These files  are placed  in the  user's home
     * directory as given by the HOME environment var.  If the config file
     * is deleted, it will be recreated using the defaults.
     */

    public class Config
    {
        public const string BallResourcePrefix = "Ball_";


        public const int MAXSTR = 256;
        public const int PATHMAX = 64;
        public const int MAXNAM = 9;

        public const int UPS = 90;
        public const float DT = (1.0f / (float)UPS);


        public const string CONFIG_DATA = "./data";        /* Game data directory */

        /* User config directory */
        public const string CONFIG_USER = "Neverball";

        public const string USER_REPLAY_FILE = "Last";


        public const float JUMP_HEIGHT = 2.00f;
        public const float SWCH_HEIGHT = 2.00f;
        public const float GOAL_HEIGHT = 3.00f;


        /* Integer options. */

        public const int CONFIG_WIDTH = 1;
        public const int CONFIG_HEIGHT = 2;
        public const int CONFIG_CAMERA = 4;
        public const int CONFIG_TEXTURES = 5;
        public const int CONFIG_GEOMETRY = 6;
        public const int CONFIG_REFLECTION = 7;
        public const int CONFIG_MIPMAP = 9;
        public const int CONFIG_ANISO = 10;
        public const int CONFIG_BACKGROUND = 11;
        public const int CONFIG_SHADOW = 12;
        public const int CONFIG_MOUSE_SENSE = 14;
        public const int CONFIG_MOUSE_INVERT = 15;
        public const int CONFIG_MOUSE_CAMERA_1 = 17;
        public const int CONFIG_MOUSE_CAMERA_2 = 18;
        public const int CONFIG_MOUSE_CAMERA_3 = 19;
        public const int CONFIG_MOUSE_CAMERA_TOGGLE = 20;
        public const int CONFIG_MOUSE_CAMERA_L = 21;
        public const int CONFIG_MOUSE_CAMERA_R = 22;
        
        public const int CONFIG_JOYSTICK_BUTTON_A = 32;
        public const int CONFIG_JOYSTICK_BUTTON_EXIT = 36;

        public const int CONFIG_KEY_CAMERA_1 = 46;
        public const int CONFIG_KEY_CAMERA_2 = 47;
        public const int CONFIG_KEY_CAMERA_3 = 48;
        public const int CONFIG_KEY_CAMERA_TOGGLE = 49;
        public const int CONFIG_KEY_CAMERA_R = 50;
        public const int CONFIG_KEY_CAMERA_L = 51;
        public const int CONFIG_VIEW_FOV = 52;
        public const int CONFIG_VIEW_DP = 53;
        public const int CONFIG_VIEW_DC = 54;
        public const int CONFIG_VIEW_DZ = 55;
        public const int CONFIG_ROTATE_FAST = 56;
        public const int CONFIG_ROTATE_SLOW = 57;
        public const int CONFIG_KEY_FORWARD = 58;
        public const int CONFIG_KEY_BACKWARD = 59;
        public const int CONFIG_KEY_LEFT = 60;
        public const int CONFIG_KEY_RIGHT = 61;
        public const int CONFIG_KEY_PAUSE = 62;
        public const int CONFIG_KEY_RESTART = 63;
        public const int CONFIG_KEY_SCORE_NEXT = 64;
        public const int CONFIG_KEY_ROTATE_FAST = 65;
        public const int CONFIG_UNIFORM = 68;
        public const int CONFIG_LOCK_GOALS = 69;

        /* String options. */

        public const int CONFIG_PLAYER = 0;
        public const int CONFIG_BALL_FILE = 1;


        struct option_d_struct
        {
            public readonly int sym;
            public readonly string name;
            public readonly int def;
            public int cur;

            public option_d_struct(int par_sym, string par_name, int par_def)
            {
                sym = par_sym;
                name = par_name;
                def = par_def;
                cur = def;
            }

            public void set_cur(int v)
            {
                cur = v;
            }
        }

        static option_d_struct[] option_d = InitOptions();

        static void AddOption(int index, string sp, int ip)
        {
            option_d[index] = new option_d_struct(index, sp, ip);
        }

        static option_d_struct[] InitOptions()
        {
            option_d = new option_d_struct[70];
        
            AddOption( CONFIG_WIDTH,        "width",        800 );
            AddOption( CONFIG_HEIGHT,       "height",       600 );
            AddOption( CONFIG_CAMERA,       "camera",       0 );
            AddOption( CONFIG_TEXTURES,     "textures",     1 );
            AddOption( CONFIG_GEOMETRY,     "geometry",     1 );
            AddOption( CONFIG_REFLECTION,   "reflection",   1 );
            AddOption( CONFIG_MIPMAP,       "mipmap",       0 );
            AddOption( CONFIG_ANISO,        "aniso",        0 );
            AddOption( CONFIG_BACKGROUND,   "background",   1 );
            AddOption( CONFIG_SHADOW,       "shadow",       1 );
            AddOption( CONFIG_MOUSE_SENSE,  "mouse_sense",  300 );
            AddOption( CONFIG_MOUSE_INVERT, "mouse_invert", 0 );

            AddOption( CONFIG_MOUSE_CAMERA_1,      "mouse_camera_1",      0 );
            AddOption( CONFIG_MOUSE_CAMERA_2,      "mouse_camera_2",      0 );
            AddOption( CONFIG_MOUSE_CAMERA_3,      "mouse_camera_3",      0 );
            AddOption( CONFIG_MOUSE_CAMERA_TOGGLE, "mouse_camera_toggle", (int)Alt.GUI.MouseButtons.Middle );
            AddOption( CONFIG_MOUSE_CAMERA_L,      "mouse_camera_l",      (int)Alt.GUI.MouseButtons.Left );
            AddOption( CONFIG_MOUSE_CAMERA_R,      "mouse_camera_r",      (int)Alt.GUI.MouseButtons.Right );

            AddOption(CONFIG_JOYSTICK_BUTTON_A, "joystick_button_a", 0);
            AddOption(CONFIG_JOYSTICK_BUTTON_EXIT, "joystick_button_exit", 4);

            AddOption( CONFIG_KEY_CAMERA_1,      "key_camera_1",      (int)Alt.GUI.Keys.F1 );
            AddOption( CONFIG_KEY_CAMERA_2,      "key_camera_2",      (int)Alt.GUI.Keys.F2 );
            AddOption( CONFIG_KEY_CAMERA_3,      "key_camera_3",      (int)Alt.GUI.Keys.F3 );
            AddOption( CONFIG_KEY_CAMERA_TOGGLE, "key_camera_toggle", (int)Alt.GUI.Keys.E );
            AddOption( CONFIG_KEY_CAMERA_R,      "key_camera_r",      (int)Alt.GUI.Keys.D );
            AddOption( CONFIG_KEY_CAMERA_L,      "key_camera_l",      (int)Alt.GUI.Keys.S );
            AddOption( CONFIG_VIEW_FOV,          "view_fov",          50 );
            AddOption( CONFIG_VIEW_DP,           "view_dp",           75 );
            AddOption( CONFIG_VIEW_DC,           "view_dc",           25 );
            AddOption( CONFIG_VIEW_DZ,           "view_dz",           200 );
            AddOption( CONFIG_ROTATE_FAST,       "rotate_fast",       300 );
            AddOption( CONFIG_ROTATE_SLOW,       "rotate_slow",       150 );
            AddOption( CONFIG_KEY_FORWARD,       "key_forward",       (int)Alt.GUI.Keys.Up );
            AddOption( CONFIG_KEY_BACKWARD,      "key_backward",      (int)Alt.GUI.Keys.Down );
            AddOption( CONFIG_KEY_LEFT,          "key_left",          (int)Alt.GUI.Keys.Left );
            AddOption( CONFIG_KEY_RIGHT,         "key_right",         (int)Alt.GUI.Keys.Right );
            AddOption( CONFIG_KEY_PAUSE,         "key_pause",         (int)Alt.GUI.Keys.Escape );
            AddOption( CONFIG_KEY_RESTART,       "key_restart",       (int)Alt.GUI.Keys.R );
            AddOption( CONFIG_KEY_SCORE_NEXT,    "key_score_next",    (int)Alt.GUI.Keys.Tab );
            AddOption( CONFIG_KEY_ROTATE_FAST,   "key_rotate_fast",   (int)Alt.GUI.Keys.LShiftKey );
            AddOption( CONFIG_UNIFORM,           "uniform",           0 );
            AddOption( CONFIG_LOCK_GOALS,        "lock_goals",        1 );

            return option_d;
        }


        struct option_s_struct
        {
            public readonly int sym;
            public readonly string name;
            public readonly string def;
            public string cur;

            public option_s_struct(int par_sym, string par_name, string par_def)
            {
                sym = par_sym;
                name = par_name;
                def = par_def;
                cur = def;
            }

            public void set_cur(string v)
            {
                cur = v;
            }
        }

        static readonly option_s_struct[] option_s = new option_s_struct[]
        {
            new option_s_struct( CONFIG_PLAYER,       "player",       "Neverballer" ),
            new option_s_struct( CONFIG_BALL_FILE,    "ball_file",    "ball/basic-ball/basic-ball" ),
        };


        //  temp public (just to prevent warnings)
        public static int dirty = 0;


        public static void config_init()
        {
            int i;

            /*
             * Store index of each option in its associated config symbol and
             * initialise current values with defaults.
             */

            int len = option_d.Length;
            for (i = 0; i < len; i++)
            {
                config_set_d(i, option_d[i].def);
            }

            len = option_s.Length;
            for (i = 0; i < len; i++)
            {
                config_set_s(i, option_s[i].def);
            }
        }


        public static void config_set_d(int i, int d)
        {
            option_d[i].cur = d;
            dirty = 1;
        }

        public static void config_tgl_d(int i)
        {
            option_d[i].cur = (option_d[i].cur != 0 ? 0 : 1);
            dirty = 1;
        }

        public static int config_tst_d(int i, Alt.GUI.Keys d)
        {
            return config_tst_d(i, (int)d);
        }
        public static int config_tst_d(int i, Alt.GUI.MouseButtons d)
        {
            return config_tst_d(i, (int)d);
        }
        public static int config_tst_d(int i, int d)
        {
            return (option_d[i].cur == d) ? 1 : 0;
        }

        public static int config_get_d(int i)
        {
            return option_d[i].cur;
        }


        public static void config_set_s(int i, string src)
        {
            option_s[i].cur = src;

            dirty = 1;
        }

        public static string config_get_s(int i)
        {
            return option_s[i].cur;
        }


        public static int config_cheat()
        {
            return 1;
        }
    }
}
