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
    class st_pause
    {
        public const int PAUSE_CONTINUE = 1;
        public const int PAUSE_RESTART = 2;
        public const int PAUSE_EXIT = 3;


        public static int paused;


        public static State st_continue = null;



        public static int is_paused()
        {
            return paused;
        }


        public static void clear_pause()
        {
            paused = 0;
        }


        public static int goto_pause()
        {
            st_continue = State.Current;
            paused = 1;
            return State.goto_state(st_ball_pause());
        }


        public static int pause_action(int i)
        {
            Audio.audio_play(Common.AUD_MENU, 1.0f);

            switch (i)
            {
                case st_pause.PAUSE_CONTINUE:
                    Video.video_set_grab(0);
                    return State.goto_state(st_pause.st_continue);

                case st_pause.PAUSE_RESTART:
                    if (Progress.progress_same() != 0)
                    {
                        st_pause.clear_pause();
                        Video.video_set_grab(1);
                        return State.goto_state(st_play.get_st_play_ready());
                    }
                    break;

                case st_pause.PAUSE_EXIT:
                    Progress.progress_stat(GAME.GAME_NONE);
                    st_pause.clear_pause();
                    Audio.audio_music_stop();
                    return State.goto_state(st_over.get_st_ball_over());
            }

            return 1;
        }


        public static State st_ball_pause()
        {
            if (st_ball_pause_class.Instance == null)
            {
                st_ball_pause_class.Instance = new st_ball_pause_class();
            }

            return st_ball_pause_class.Instance;
        }
    }


    class st_ball_pause_class : State
    {
        public static st_ball_pause_class Instance = null;


        internal override int OnEnter()
        {
            int id, jd, title_id;

            Video.video_clr_grab();

            //  Build the pause GUI

            if ((id = gui.gui_vstack(0)) != 0)
            {
                title_id = gui.gui_label(id, "Paused", gui.GUI_LRG, gui.GUI_ALL, null, null);

                gui.gui_space(id);

                if ((jd = gui.gui_harray(id)) != 0)
                {
                    gui.gui_state(jd, "Quit", gui.GUI_SML, st_pause.PAUSE_EXIT, 0);

                    if (Progress.progress_same_avail() != 0)
                        gui.gui_state(jd, "Restart", gui.GUI_SML, st_pause.PAUSE_RESTART, 0);

                    gui.gui_start(jd, "Continue", gui.GUI_SML, st_pause.PAUSE_CONTINUE, 1);
                }

                gui.gui_pulse(title_id, 1.2f);
                gui.gui_layout(id, 0, 0);
            }

            hud.hud_update(0);

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

            hud.hud_paint();
        }


        internal override void OnTimer(int id, float dt)
        {
            gui.gui_timer(id, dt);
            hud.hud_timer(dt);
        }


        internal override void OnPoint(int id, int x, int y, int dx, int dy)
        {
            /* Pulse, activate and return the active id (if changed) */

            int jd = gui.gui_point(id, x, y);

            if (jd != 0)
                gui.gui_pulse(jd, 1.2f);
        }


        internal override int OnClick(Alt.GUI.MouseButtons b, int d)
        {
            if (b == Alt.GUI.MouseButtons.Left &&
                d == 1)
            {
                return State.st_buttn(Config.config_get_d(Config.CONFIG_JOYSTICK_BUTTON_A), 1);
            }
            else
            {
                return 1;
            }
        }


        internal override int OnKeybd(Alt.GUI.Keys c, int d)
        {
            if (d != 0)
            {
                if (Config.config_tst_d(Config.CONFIG_KEY_PAUSE, c) != 0)
                    return st_pause.pause_action(st_pause.PAUSE_CONTINUE);

                if (Config.config_tst_d(Config.CONFIG_KEY_RESTART, c) != 0 &&
                    Progress.progress_same_avail() != 0)
                    return st_pause.pause_action(st_pause.PAUSE_RESTART);
            }
            return 1;
        }


        internal override int OnButton(int b, int d)
        {
            if (d != 0)
            {
                if (Config.config_tst_d(Config.CONFIG_JOYSTICK_BUTTON_A, b) != 0)
                {
                    return st_pause.pause_action(gui.gui_token(gui.gui_click()));
                }

                if (Config.config_tst_d(Config.CONFIG_JOYSTICK_BUTTON_EXIT, b) != 0)
                {
                    return st_pause.pause_action(st_pause.PAUSE_CONTINUE);
                }
            }

            return 1;
        }


        public st_ball_pause_class()
        {
            pointer = 1;
            gui_id = 0;
        }
    }
}
