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
    class hud
    {
        public static int Lhud_id;
        public static int Rhud_id;
        public static int time_id;

        public static int coin_id;
        public static int ball_id;
        public static int scor_id;
        public static int goal_id;
        public static int view_id;

        public static float view_timer;


        public static void hud_free()
        {
            gui.gui_delete(Rhud_id);
            gui.gui_delete(Lhud_id);
            gui.gui_delete(time_id);
            gui.gui_delete(view_id);
        }


        public static void hud_view_timer(float dt)
        {
            view_timer -= dt;
            gui.gui_timer(view_id, dt);
        }


        public static void hud_view_paint()
        {
            if (view_timer > 0.0f)
            {
                gui.gui_paint(view_id);
            }
        }


        public static void hud_paint()
        {
            if (Progress.curr_mode() == MODE.MODE_CHALLENGE)
                gui.gui_paint(Lhud_id);

            gui.gui_paint(Rhud_id);
            gui.gui_paint(time_id);

            hud_view_paint();
        }


        public static void hud_update(int pulse)
        {
            int clock = game_client.curr_clock();
            int coins = game_client.curr_coins();
            int goal = Progress.curr_goal();
            int balls = Progress.curr_balls();
            int score = Progress.curr_score();

            int c_id;
            int last;

            if (pulse == 0)
            {
                /* reset the hud */

                gui.gui_pulse(ball_id, 0);
                gui.gui_pulse(time_id, 0);
                gui.gui_pulse(coin_id, 0);
            }

            /* time and tick-tock */

            if (clock != (last = gui.gui_value(time_id)))
            {
                gui.gui_set_clock(time_id, clock);

                if (last > clock && pulse != 0)
                {
                    if (clock <= 1000 && (last / 100) > (clock / 100))
                    {
                        Audio.audio_play(Common.AUD_TICK, 1);
                        gui.gui_pulse(time_id, 1.50f);
                    }
                    else if (clock < 500 && (last / 50) > (clock / 50))
                    {
                        Audio.audio_play(Common.AUD_TOCK, 1);
                        gui.gui_pulse(time_id, 1.25f);
                    }
                }
            }

            /* balls and score + select coin widget */

            switch (Progress.curr_mode())
            {
                case MODE.MODE_CHALLENGE:
                    if (gui.gui_value(ball_id) != balls) gui.gui_set_count(ball_id, balls);
                    if (gui.gui_value(scor_id) != score) gui.gui_set_count(scor_id, score);

                    c_id = coin_id;
                    break;

                default:
                    c_id = coin_id;
                    break;
            }


            /* coins and pulse */

            if (coins != (last = gui.gui_value(c_id)))
            {
                last = coins - last;

                gui.gui_set_count(c_id, coins);

                if (pulse != 0 && last > 0)
                {
                    if (last >= 10) gui.gui_pulse(coin_id, 2.00f);
                    else if (last >= 5) gui.gui_pulse(coin_id, 1.50f);
                    else gui.gui_pulse(coin_id, 1.25f);

                    if (goal > 0)
                    {
                        if (last >= 10) gui.gui_pulse(goal_id, 2.00f);
                        else if (last >= 5) gui.gui_pulse(goal_id, 1.50f);
                        else gui.gui_pulse(goal_id, 1.25f);
                    }
                }
            }

            /* goal and pulse */

            if (goal != (last = gui.gui_value(goal_id)))
            {
                gui.gui_set_count(goal_id, goal);

                if (pulse != 0 && goal == 0 && last > 0)
                    gui.gui_pulse(goal_id, 2.00f);
            }
        }


        public static void hud_timer(float dt)
        {
            hud_update(1);

            gui.gui_timer(Rhud_id, dt);
            gui.gui_timer(Lhud_id, dt);
            gui.gui_timer(time_id, dt);

            hud_view_timer(dt);
        }


        public static void hud_init()
        {
            int id;
            string str_view;
            int v;

            if ((Rhud_id = gui.gui_hstack(0)) != 0)
            {
                if ((id = gui.gui_vstack(Rhud_id)) != 0)
                {
                    gui.gui_label(id, "Coins", gui.GUI_SML, 0, widget.gui_wht, widget.gui_wht);
                    gui.gui_label(id, "Goal", gui.GUI_SML, 0, widget.gui_wht, widget.gui_wht);
                }
                if ((id = gui.gui_vstack(Rhud_id)) != 0)
                {
                    coin_id = gui.gui_count(id, 100, gui.GUI_SML, gui.GUI_NW);
                    goal_id = gui.gui_count(id, 10, gui.GUI_SML, 0);
                }
                gui.gui_layout(Rhud_id, +1, -1);
            }

            if ((Lhud_id = gui.gui_hstack(0)) != 0)
            {
                if ((id = gui.gui_vstack(Lhud_id)) != 0)
                {
                    ball_id = gui.gui_count(id, 10, gui.GUI_SML, gui.GUI_NE);
                    scor_id = gui.gui_count(id, 1000, gui.GUI_SML, 0);
                }
                if ((id = gui.gui_vstack(Lhud_id)) != 0)
                {
                    gui.gui_label(id, "Balls", gui.GUI_SML, 0, widget.gui_wht, widget.gui_wht);
                    gui.gui_label(id, "Score", gui.GUI_SML, 0, widget.gui_wht, widget.gui_wht);
                }
                gui.gui_layout(Lhud_id, -1, -1);
            }

            if ((time_id = gui.gui_clock(0, 59999, gui.GUI_MED, gui.GUI_TOP)) != 0)
                gui.gui_layout(time_id, 0, -1);


            /* Find the longest view name. */

            for (str_view = "", v = (int)VIEW.VIEW_NONE + 1; v < (int)VIEW.VIEW_MAX; v++)
                if (game_common.view_to_str((VIEW)v).Length > str_view.Length)
                    str_view = game_common.view_to_str((VIEW)v);

            if ((view_id = gui.gui_label(0, str_view, gui.GUI_SML, gui.GUI_SW, widget.gui_wht, widget.gui_wht)) != 0)
                gui.gui_layout(view_id, 1, 1);
        }


        public static void hud_view_pulse(int c)
        {
            gui.gui_set_label(view_id, game_common.view_to_str((VIEW)c));
            gui.gui_pulse(view_id, 1.2f);
            view_timer = 2.0f;
        }
    }
}
