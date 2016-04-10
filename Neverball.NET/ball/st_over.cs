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
    class st_over
    {
        public static State get_st_ball_over()
        {
            if (st_ball_over_class.Instance == null)
            {
                st_ball_over_class.Instance = new st_ball_over_class();
            }

            return st_ball_over_class.Instance;
        }
    }


    class st_ball_over_class : State
    {
        public static st_ball_over_class Instance = null;


        internal override int OnEnter()
        {
            int id;

            if (Progress.curr_mode() != MODE.MODE_CHALLENGE)
                return 0;

            if ((id = gui.gui_label(0, "GAME OVER", gui.GUI_LRG, gui.GUI_ALL, widget.gui_gry, widget.gui_red)) != 0)
            {
                gui.gui_layout(id, 0, 0);
                gui.gui_pulse(id, 1.2f);
            }

            Audio.audio_music_fade_out(2.0f);
            Audio.audio_play(Common.AUD_OVER, 1);

            Video.video_clr_grab();

            return id;
        }


        internal override void OnLeave(int id)
        {
            gui.gui_delete(id);
        }


        internal override void OnPaint(int id, float t)
        {
            game_client.ball_game_draw(0, t);
            gui.gui_paint(id);
        }


        internal override void OnTimer(int id, float dt)
        {
            if (Progress.curr_mode() != MODE.MODE_CHALLENGE || State.time_state() > 3)
                State.goto_state(st_start.get_st_start());

            gui.gui_timer(id, dt);
        }


        internal override int OnClick(Alt.GUI.MouseButtons b, int d)
        {
            return (b == Alt.GUI.MouseButtons.Left && d == 1) ? State.goto_state(st_start.get_st_start()) : 1;
        }


        internal override int OnButton(int b, int d)
        {
            if (d != 0)
            {
                if (Config.config_tst_d(Config.CONFIG_JOYSTICK_BUTTON_A, b) != 0 ||
                    Config.config_tst_d(Config.CONFIG_JOYSTICK_BUTTON_EXIT, b) != 0)
                {
                    return State.goto_state(st_start.get_st_start());
                }
            }

            return 1;
        }


        public st_ball_over_class()
        {
            pointer = 1;
            gui_id = 0;
        }
    }
}
