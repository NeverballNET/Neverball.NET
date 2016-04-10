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
    class st_fall_out
    {
        public const int FALL_OUT_NEXT = 1;
        public const int FALL_OUT_SAME = 2;
        public const int FALL_OUT_BACK = 4;
        public const int FALL_OUT_OVER = 5;

        public static int resume;


        public static int fall_out_action(int i)
        {
            Audio.audio_play(Common.AUD_MENU, 1.0f);

            switch (i)
            {
                case FALL_OUT_BACK:
                /* Fall through. */

                case FALL_OUT_OVER:
                    return State.goto_state(st_over.get_st_ball_over());

                case FALL_OUT_NEXT:
                    if (Progress.progress_next() != 0)
                    {
                        return State.goto_state(st_level.get_st_level());
                    }

                    break;

                case FALL_OUT_SAME:
                    if (Progress.progress_same() != 0)
                    {
                        return State.goto_state(st_level.get_st_level());
                    }

                    break;
            }

            return 1;
        }


        public static State get_st_fall_out()
        {
            if (st_fall_out_class.Instance == null)
            {
                st_fall_out_class.Instance = new st_fall_out_class();
            }

            return st_fall_out_class.Instance;
        }
    }



    class st_fall_out_class : State
    {
        public static st_fall_out_class Instance = null;


        internal override int OnEnter()
        {
            int id, jd, kd;

            /* Reset hack. */
            st_fall_out.resume = 0;

            if ((id = gui.gui_vstack(0)) != 0)
            {
                kd = gui.gui_label(id, "Fall-out!", gui.GUI_LRG, gui.GUI_ALL, widget.gui_gry, widget.gui_red);

                gui.gui_space(id);

                if ((jd = gui.gui_harray(id)) != 0)
                {
                    if (Progress.progress_dead() != 0)
                        gui.gui_start(jd, "Exit", gui.GUI_SML, st_fall_out.FALL_OUT_OVER, 0);

                    if (Progress.progress_next_avail() != 0)
                        gui.gui_start(jd, "Next Level", gui.GUI_SML, st_fall_out.FALL_OUT_NEXT, 0);

                    if (Progress.progress_same_avail() != 0)
                        gui.gui_start(jd, "Retry Level", gui.GUI_SML, st_fall_out.FALL_OUT_SAME, 0);
                }

                gui.gui_space(id);

                gui.gui_pulse(kd, 1.2f);
                gui.gui_layout(id, 0, 0);
            }

            Audio.audio_music_fade_out(2.0f);

            Video.video_clr_grab();

            return id;
        }


        internal override void OnLeave(int id)
        {
            /* HACK:  don't run animation if only "visiting" a state. */
            m_fTickFallOut = st_fall_out.resume == 0;

            gui.gui_delete(id);
        }


        internal override void OnPaint(int id, float t)
        {
            game_client.ball_game_draw(0, t);
            gui.gui_paint(id);
        }


        bool m_fTickFallOut = true;
        internal override void OnTimer(int id, float dt)
        {
            if (m_fTickFallOut &&
                State.time_state() < 2.0f)
            {
                game_server.game_server_step(dt);
                game_client.game_client_step();
            }

            gui.gui_timer(id, dt);
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
                if (Config.config_tst_d(Config.CONFIG_KEY_RESTART, c) != 0 &&
                    Progress.progress_same_avail() != 0)
                {
                    if (Progress.progress_same() != 0)
                    {
                        State.goto_state(st_play.get_st_play_ready());
                    }
                }
            }
            return 1;
        }

        
        internal override int OnButton(int b, int d)
        {
            if (d != 0)
            {
                if (Config.config_tst_d(Config.CONFIG_JOYSTICK_BUTTON_A, b) != 0)
                {
                    return st_fall_out.fall_out_action(gui.gui_token(gui.gui_click()));
                }

                if (Config.config_tst_d(Config.CONFIG_JOYSTICK_BUTTON_EXIT, b) != 0)
                {
                    return st_fall_out.fall_out_action(st_fall_out.FALL_OUT_BACK);
                }
            }

            return 1;
        }


        public st_fall_out_class()
        {
            pointer = 1;
            gui_id = 0;
        }
    }
}
