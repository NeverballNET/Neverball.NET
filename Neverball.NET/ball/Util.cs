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
    class Util
    {
        const int GUI_BIT = (1 << 24);

        public static int GUI_ISMSK(int i)
        {
            return (((i) & GUI_BIT) != 0 ? 1 : 0);
        }

        public const int GUI_NULL = ((0) | GUI_BIT);
        public const int GUI_BACK = ((1) | GUI_BIT);
        public const int GUI_PREV = ((2) | GUI_BIT);
        public const int GUI_NEXT = ((3) | GUI_BIT);
        public const int GUI_BS = ((4) | GUI_BIT);
        public const int GUI_CL = ((5) | GUI_BIT);

        public const int GUI_MOST_COINS = ((8) | GUI_BIT);
        public const int GUI_BEST_TIMES = ((16) | GUI_BIT);
        public const int GUI_FAST_UNLOCK = ((32) | GUI_BIT);


        public static int is_special_name(string n)
        {
            return (string.Equals(n, "Hard") ||
                    string.Equals(n, "Medium") ||
                    string.Equals(n, "Easy")) ? 1 : 0;
        }


        public static int coin_btn_id;
        public static int time_btn_id;
        public static int goal_btn_id;


        public static void set_score_color(int id, int hi,
                                    float[] c0,
                                    float[] c1)
        {
            if (hi >= 0)
            {
                if (hi < Score.NSCORE)
                    gui.gui_set_color(id, c0, c0);
                else
                    gui.gui_set_color(id, c1, c1);
            }
        }


        public static int score_label;

        public static int[] score_coin = new int[4];
        public static int[] score_name = new int[4];
        public static int[] score_time = new int[4];

        public static int score_extra_row;


        /* Build a top three score list with default values. */

        public static void gui_scores(int id, int e)
        {
            string s = "1234567";

            int j, jd, kd, ld, md;

            score_extra_row = e;

            if ((jd = gui.gui_hstack(id)) != 0)
            {
                gui.gui_filler(jd);

                if ((kd = gui.gui_vstack(jd)) != 0)
                {
                    score_label = gui.gui_label(kd, "Unavailable", gui.GUI_SML, gui.GUI_TOP, null, null);

                    if ((ld = gui.gui_hstack(kd)) != 0)
                    {
                        if ((md = gui.gui_vstack(ld)) != 0)
                        {
                            for (j = 0; j < Score.NSCORE - 1; j++)
                                score_coin[j] = gui.gui_count(md, 1000, gui.GUI_SML, 0);

                            score_coin[j++] = gui.gui_count(md, 1000, gui.GUI_SML, gui.GUI_SE);

                            if (e != 0)
                            {
                                gui.gui_space(md);
                                score_coin[j++] = gui.gui_count(md, 1000, gui.GUI_SML, gui.GUI_RGT);
                            }
                        }

                        if ((md = gui.gui_vstack(ld)) != 0)
                        {
                            for (j = 0; j < Score.NSCORE; j++)
                            {
                                score_name[j] = gui.gui_label(md, s, gui.GUI_SML, 0,
                                                          widget.gui_yel, widget.gui_wht);
                                gui.gui_set_trunc(score_name[j], trunc.TRUNC_TAIL);
                            }

                            if (e != 0)
                            {
                                gui.gui_space(md);
                                score_name[j++] = gui.gui_label(md, s, gui.GUI_SML, 0,
                                                            widget.gui_yel, widget.gui_wht);
                                gui.gui_set_trunc(score_name[j - 1], trunc.TRUNC_TAIL);
                            }
                        }

                        if ((md = gui.gui_vstack(ld)) != 0)
                        {
                            for (j = 0; j < Score.NSCORE - 1; j++)
                                score_time[j] = gui.gui_clock(md, 359999, gui.GUI_SML, 0);

                            score_time[j++] = gui.gui_clock(md, 359999, gui.GUI_SML, gui.GUI_SW);

                            if (e != 0)
                            {
                                gui.gui_space(md);
                                score_time[j++] = gui.gui_clock(md, 359999, gui.GUI_SML, gui.GUI_LFT);
                            }
                        }
                    }
                }

                gui.gui_filler(jd);
            }
        }


        public static int score_type = GUI_MOST_COINS;


        public static void gui_score_board(int pd, int types, int e, int h)
        {
            int id, jd, kd;

            /* Make sure current score type matches the spec. */

            while ((types & score_type) != score_type)
                score_type = gui_score_next(score_type);

            if ((id = gui.gui_hstack(pd)) != 0)
            {
                gui.gui_filler(id);

                if ((jd = gui.gui_hstack(id)) != 0)
                {
                    gui.gui_filler(jd);

                    if ((kd = gui.gui_vstack(jd)) != 0)
                    {
                        gui.gui_filler(kd);

                        if ((types & GUI_MOST_COINS) == GUI_MOST_COINS)
                        {
                            coin_btn_id = gui.gui_state(kd, "Most Coins",
                                                    gui.GUI_SML, GUI_MOST_COINS,
                                                    score_type == GUI_MOST_COINS ? 1 : 0);
                        }
                        if ((types & GUI_BEST_TIMES) == GUI_BEST_TIMES)
                        {
                            time_btn_id = gui.gui_state(kd, "Best Times",
                                                    gui.GUI_SML, GUI_BEST_TIMES,
                                                    score_type == GUI_BEST_TIMES ? 1 : 0);
                        }
                        if ((types & GUI_FAST_UNLOCK) == GUI_FAST_UNLOCK)
                        {
                            goal_btn_id = gui.gui_state(kd, "Fast Unlock",
                                                    gui.GUI_SML, GUI_FAST_UNLOCK,
                                                    score_type == GUI_FAST_UNLOCK ? 1 : 0);
                        }

                        gui.gui_filler(kd);
                    }

                    gui.gui_filler(jd);
                }

                gui.gui_filler(id);
                gui_scores(id, e);
                gui.gui_filler(id);
            }
        }


        public static int gui_score_next(int t)
        {
            switch (t)
            {
                case GUI_MOST_COINS: return GUI_BEST_TIMES;
                case GUI_BEST_TIMES: return GUI_FAST_UNLOCK;
                case GUI_FAST_UNLOCK: return GUI_MOST_COINS;

                default:
                    return GUI_MOST_COINS;
            }
        }


        public static void gui_score_set(int t)
        {
            score_type = t;
        }


        public static int gui_score_get()
        {
            return score_type;
        }


        /*
         * XXX Watch  out when  using these  functions. Be  sure to  check for
         * GUI_NULL in addition to GUI_NEXT and GUI_PREV when using the latter
         * two as labels for a switch with a default label.
         */

        public static int gui_navig(int id, int prev, int next)
        {
            int jd;

            if ((jd = gui.gui_hstack(id)) != 0)
            {
                if (next != 0 || prev != 0)
                {
                    gui_maybe(jd, "Next", GUI_NEXT, GUI_NULL, next);
                    gui_maybe(jd, "Prev", GUI_PREV, GUI_NULL, prev);
                }

                gui.gui_space(jd);

                gui.gui_start(jd, "Back", gui.GUI_SML, GUI_BACK, 0);
            }
            return jd;
        }

        public static int gui_maybe(int id, string label, int etoken, int dtoken, int enabled)
        {
            int bd;

            if (enabled == 0)
            {
                bd = gui.gui_state(id, label, gui.GUI_SML, dtoken, 0);
                gui.gui_set_color(bd, widget.gui_gry, widget.gui_gry);
            }
            else
                bd = gui.gui_state(id, label, gui.GUI_SML, etoken, 0);

            return bd;
        }


        /* Set the top three score list values. */

        public static void gui_set_scores(string label, Score s, int hilite)
        {
            int j;

            if (s == null)
            {
                gui.gui_set_label(score_label, "Unavailable");

                for (j = 0; j < Score.NSCORE + score_extra_row; j++)
                {
                    gui.gui_set_count(score_coin[j], -1);
                    gui.gui_set_label(score_name[j], "");
                    gui.gui_set_clock(score_time[j], -1);
                }
            }
            else
            {
                gui.gui_set_label(score_label, label);

                for (j = 0; j < Score.NSCORE + score_extra_row; j++)
                {
                    string name = s.player[j];

                    if (j == hilite)
                        set_score_color(score_name[j], j, widget.gui_grn, widget.gui_red);
                    else
                        gui.gui_set_color(score_name[j], widget.gui_yel, widget.gui_wht);

                    gui.gui_set_count(score_coin[j], s.coins[j]);
                    gui.gui_set_label(score_name[j], is_special_name(name) != 0 ? name : name);
                    gui.gui_set_clock(score_time[j], s.timer[j]);
                }
            }
        }


        public static void set_score_board(Score smc, int hmc,
                             Score sbt, int hbt,
                             Score sfu, int hfu)
        {
            switch (score_type)
            {
                case GUI_MOST_COINS:
                    gui_set_scores("Most Coins", smc, hmc);
                    break;

                case GUI_BEST_TIMES:
                    gui_set_scores("Best Times", sbt, hbt);
                    break;

                case GUI_FAST_UNLOCK:
                    gui_set_scores("Fast Unlock", sfu, hfu);
                    break;
            }

            set_score_color(coin_btn_id, hmc, widget.gui_grn, widget.gui_wht);
            set_score_color(time_btn_id, hbt, widget.gui_grn, widget.gui_wht);
            set_score_color(goal_btn_id, hfu, widget.gui_grn, widget.gui_wht);
        }
    }
}
