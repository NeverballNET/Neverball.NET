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
    public abstract class State
    {
        public int pointer;
        public int gui_id;

        public static State Current = null;


        internal virtual int OnEnter()
        {
            return 0;
        }

        internal virtual void OnLeave(int id)
        {
        }

        internal virtual void OnPaint(int id, float t)
        {
        }

        internal virtual void OnTimer(int id, float dt)
        {
        }
        
        internal virtual void OnPoint(int id, int x, int y, int dx, int dy)
        {
        }

        internal virtual int OnClick(Alt.GUI.MouseButtons b, int d)
        {
            return 1;//because of null 0;
        }

        internal virtual int OnKeybd(Alt.GUI.Keys c, int d)
        {
            return 1;//because of null 0;
        }

        internal virtual int OnButton(int b, int d)
        {
            return 1;//because of null 0;
        }



        static float         state_time;
        static int           state_drawn;


        public static float time_state()
        {
            return state_time;
        }


        public static int goto_state(State st)
        {
            if (Current != null)
	        {
                Current.OnLeave(Current.gui_id);
	        }

            Current       = st;
            state_time  = 0;
            state_drawn = 0;

            if (Current != null)
	        {
                Current.gui_id = Current.OnEnter();
	        }

            return 1;
        }



        public static void st_paint(float t)
        {
            state_drawn = 1;

            if (Current != null)
            {
                Video.video_clear();

                Current.OnPaint(Current.gui_id, t);
            }
        }


        public static void st_timer(float dt)
        {
            if (//!
                state_drawn == 0)
	        {
                return;
	        }

            state_time += dt;

            if (Current != null)
	        {
		        Current.OnTimer(Current.gui_id, dt);
	        }
        }


        public static void st_point(int x, int y, int dx, int dy)
        {
            if (Current != null)
	        {
		        Current.OnPoint(Current.gui_id, x, y, dx, dy);
	        }
        }


        public static int st_click(Alt.GUI.MouseButtons b, int d)
        {
            return (Current != null) ? Current.OnClick(b, d) : 1;
        }


        public static int st_keybd(Alt.GUI.Keys c, int d)
        {
            return (Current != null) ? Current.OnKeybd(c, d) : 1;
        }


        public static int st_buttn(int b, int d)
        {
            return (Current != null) ? Current.OnButton(b, d) : 1;
        }
    }
}
