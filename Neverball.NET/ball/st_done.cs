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
    class st_done
    {
        public const int DONE_OK = 1;

        /* Bread crumbs. */

        public static int new_name;
        public static int resume;


        public static int done_action(int i)
        {
            Audio.audio_play(Common.AUD_MENU, 1.0f);

            switch (i)
            {
                case st_done.DONE_OK:
                    return State.goto_state(st_start.get_st_start());

                case Util.GUI_MOST_COINS:
                case Util.GUI_BEST_TIMES:
                case Util.GUI_FAST_UNLOCK:
                    Util.gui_score_set(i);
                    st_done.resume = 1;
                    return State.goto_state(get_st_done());
            }
            return 1;
        }


        public static State get_st_done()
        {
            if (st_done_class.Instance == null)
            {
                st_done_class.Instance = new st_done_class();
            }

            return st_done_class.Instance;
        }
    }



    class st_done_class : State
    {
        public static st_done_class Instance = null;


        internal override int OnEnter()
        {
            string s1 = "New Set Record";
            string s2 = "Set Complete";

            int id;

            int high = Progress.progress_set_high();

            if (st_done.new_name != 0)
            {
                Progress.progress_rename(1);
                st_done.new_name = 0;
            }

            if ((id = gui.gui_vstack(0)) != 0)
            {
                int gid;

                if (high != 0)
                    gid = gui.gui_label(id, s1, gui.GUI_MED, gui.GUI_ALL, widget.gui_grn, widget.gui_grn);
                else
                    gid = gui.gui_label(id, s2, gui.GUI_MED, gui.GUI_ALL, widget.gui_blu, widget.gui_grn);

                gui.gui_space(id);
                Util.gui_score_board(id, Util.GUI_MOST_COINS | Util.GUI_BEST_TIMES, 1, high);
                gui.gui_space(id);

                gui.gui_start(id, "Select Level", gui.GUI_SML, st_done.DONE_OK, 0);

                if (st_done.resume == 0)
                    gui.gui_pulse(gid, 1.2f);

                gui.gui_layout(id, 0, 0);
            }

            Util.set_score_board(Set.set_coin_score(Set.curr_set()), Progress.progress_score_rank(),
                            Set.set_time_score(Set.curr_set()), Progress.progress_times_rank(),
                            null, -1);

            /* Reset hack. */
            st_done.resume = 0;

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
            if (d != 0 &&
                Config.config_tst_d(Config.CONFIG_KEY_SCORE_NEXT, c) != 0)
            {
                return st_done.done_action(Util.gui_score_next(Util.gui_score_get()));
            }

            return 1;
        }


        internal override int OnButton(int b, int d)
        {
            if (d != 0)
            {
                if (Config.config_tst_d(Config.CONFIG_JOYSTICK_BUTTON_A, b) != 0)
                {
                    return st_done.done_action(gui.gui_token(gui.gui_click()));
                }

                if (Config.config_tst_d(Config.CONFIG_JOYSTICK_BUTTON_EXIT, b) != 0)
                {
                    return st_done.done_action(st_done.DONE_OK);
                }
            }

            return 1;
        }


        public st_done_class()
        {
            pointer = 1;
            gui_id = 0;
        }
    }
}
