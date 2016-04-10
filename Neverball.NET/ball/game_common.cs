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
    public enum MODE
    {
        MODE_NONE = 0,

        MODE_CHALLENGE,
        MODE_NORMAL,

        MODE_MAX
    }


    enum GAME
    {
        GAME_NONE = 0,

        GAME_TIME,
        GAME_GOAL,
        GAME_FALL,

        GAME_MAX
    }


    enum VIEW
    {
        VIEW_NONE = -1,

        VIEW_CHASE,
        VIEW_LAZY,
        VIEW_MANUAL,
        VIEW_TOPDOWN,

        VIEW_MAX
    }



    class game_tilt
    {
        public float[] x = new float[3];
        public float rx;
        public float[] z = new float[3];
        public float rz;
    }



    class game_common
    {
        public static string status_to_str(GAME s)
        {
            switch (s)
            {
                case GAME.GAME_NONE: return "Aborted";
                case GAME.GAME_TIME: return "Time-out";
                case GAME.GAME_GOAL: return "Success";
                case GAME.GAME_FALL: return "Fall-out";
                default: return "Unknown";
            }
        }


        public static string view_to_str(VIEW v)
        {
            switch (v)
            {
                case VIEW.VIEW_CHASE: return "Chase";
                case VIEW.VIEW_LAZY: return "Lazy";
                case VIEW.VIEW_MANUAL: return "Manual";
                case VIEW.VIEW_TOPDOWN: return "Top-Down";
                default: return "Unknown";
            }
        }


        public static string mode_to_str(MODE m, int l)
        {
            switch (m)
            {
                case MODE.MODE_CHALLENGE: return l != 0 ? "Challenge Mode" : "Challenge";
                case MODE.MODE_NORMAL: return l != 0 ? "Normal Mode" : "Normal";
                default: return l != 0 ? "Unknown Mode" : "Unknown";
            }
        }


        public static void game_tilt_init(game_tilt tilt)
        {
            tilt.x[0] = 1.0f;
            tilt.x[1] = 0.0f;
            tilt.x[2] = 0.0f;

            tilt.rx = 0.0f;

            tilt.z[0] = 0.0f;
            tilt.z[1] = 0.0f;
            tilt.z[2] = 1.0f;

            tilt.rz = 0.0f;
        }


        /*
         * Compute appropriate tilt axes from the view basis.
         */
        public static void game_tilt_axes(game_tilt tilt, float[][] view_e)
        {
            float[] Y = new float[3] { 0.0f, 1.0f, 0.0f };

            Vec3.v_cpy(tilt.x, view_e[0]);
            Vec3.v_cpy(tilt.z, view_e[2]);

            /* Handle possible top-down view. */

            if (System.Math.Abs(Vec3.v_dot(view_e[1], Y)) < System.Math.Abs(Vec3.v_dot(view_e[2], Y)))
                Vec3.v_inv(tilt.z, view_e[1]);
        }


        public static void game_tilt_grav(float[] h,
                            float[] g,
                            game_tilt tilt)
        {
            float[] X = new float[16];
            float[] Z = new float[16];
            float[] M = new float[16];

            /* Compute the gravity vector from the given world rotations. */

            Vec3.m_rot(Z, tilt.z, MathHelper.DegreesToRadians(tilt.rz));
            Vec3.m_rot(X, tilt.x, MathHelper.DegreesToRadians(tilt.rx));
            Vec3.m_mult(M, Z, X);
            Vec3.m_vxfm(h, M, g);
        }
    }
}
