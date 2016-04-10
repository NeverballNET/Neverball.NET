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
    class st_goal
    {
        public const int GOAL_NEXT = 1;
        public const int GOAL_SAME = 2;
        public const int GOAL_BACK = 4;
        public const int GOAL_DONE = 5;
        public const int GOAL_OVER = 6;
        public const int GOAL_LAST = 7;

        public static int balls_id;
        public static int coins_id;
        public static int score_id;

        /* Bread crumbs. */

        public static int new_name;
        public static int resume;


        public static int goal_action(int i)
        {
            Audio.audio_play(Common.AUD_MENU, 1.0f);

            switch (i)
            {
                case GOAL_BACK:
                /* Fall through. */

                case GOAL_OVER:
                    return State.goto_state(st_over.get_st_ball_over());

                case GOAL_DONE:
                    Progress.progress_exit();
                    return State.goto_state(st_done.get_st_done());

                case GOAL_LAST:
                    return State.goto_state(st_start.get_st_start());

                case Util.GUI_MOST_COINS:
                case Util.GUI_BEST_TIMES:
                case Util.GUI_FAST_UNLOCK:
                    Util.gui_score_set(i);
                    resume = 1;
                    return State.goto_state(get_st_ball_goal());

                case GOAL_NEXT:
                    if (Progress.progress_next() != 0)
                        return State.goto_state(st_level.get_st_level());
                    break;

                case GOAL_SAME:
                    if (Progress.progress_same() != 0)
                        return State.goto_state(st_level.get_st_level());
                    break;
            }

            return 1;
        }



        public static st_ball_goal_class get_st_ball_goal()
        {
            if (st_ball_goal_class.Instance == null)
            {
                st_ball_goal_class.Instance = new st_ball_goal_class();
            }

            return st_ball_goal_class.Instance;
        }
    }



    class st_ball_goal_class : State
    {
        public static st_ball_goal_class Instance = null;


        internal override int OnEnter()
        {
            string s1 = "New Record";
            string s2 = "GOAL";

            int id, jd, kd, ld, md;

            Level l = Set.get_level(Progress.curr_level());

            int high = Progress.progress_lvl_high();

            if (st_goal.new_name != 0)
            {
                Progress.progress_rename(0);
                st_goal.new_name = 0;
            }

            if ((id = gui.gui_vstack(0)) != 0)
            {
                int gid;

                if (high != 0)
                    gid = gui.gui_label(id, s1, gui.GUI_MED, gui.GUI_ALL, widget.gui_grn, widget.gui_grn);
                else
                    gid = gui.gui_label(id, s2, gui.GUI_LRG, gui.GUI_ALL, widget.gui_blu, widget.gui_grn);

                gui.gui_space(id);

                if (Progress.curr_mode() == MODE.MODE_CHALLENGE)
                {
                    int coins, score, balls;
                    int i;

                    /* Reverse-engineer initial score and balls. */

                    if (st_goal.resume != 0)
                    {
                        coins = 0;
                        score = Progress.curr_score();
                        balls = Progress.curr_balls();
                    }
                    else
                    {
                        coins = game_client.curr_coins();
                        score = Progress.curr_score() - coins;
                        balls = Progress.curr_balls();

                        for (i = Progress.curr_score(); i > score; i--)
                            if (Progress.progress_reward_ball(i) != 0)
                                balls--;
                    }

                    string msg = Progress.curr_bonus().ToString() +
                        (Progress.curr_bonus() == 1 ? " new bonus level" : " new bonus levels");

                    if ((jd = gui.gui_hstack(id)) != 0)
                    {
                        gui.gui_filler(jd);

                        if ((kd = gui.gui_vstack(jd)) != 0)
                        {
                            if ((ld = gui.gui_hstack(kd)) != 0)
                            {
                                if ((md = gui.gui_harray(ld)) != 0)
                                {
                                    st_goal.balls_id = gui.gui_count(md, 100, gui.GUI_MED, gui.GUI_NE);
                                    gui.gui_label(md, "Balls", gui.GUI_SML, 0,
                                              widget.gui_wht, widget.gui_wht);
                                }
                                if ((md = gui.gui_harray(ld)) != 0)
                                {
                                    st_goal.score_id = gui.gui_count(md, 1000, gui.GUI_MED, 0);
                                    gui.gui_label(md, "Score", gui.GUI_SML, 0,
                                              widget.gui_wht, widget.gui_wht);
                                }
                                if ((md = gui.gui_harray(ld)) != 0)
                                {
                                    st_goal.coins_id = gui.gui_count(md, 100, gui.GUI_MED, 0);
                                    gui.gui_label(md, "Coins", gui.GUI_SML, gui.GUI_NW,
                                              widget.gui_wht, widget.gui_wht);
                                }

                                gui.gui_set_count(st_goal.balls_id, balls);
                                gui.gui_set_count(st_goal.score_id, score);
                                gui.gui_set_count(st_goal.coins_id, coins);
                            }

                            gui.gui_label(kd, msg, gui.GUI_SML, gui.GUI_BOT, null, null);
                        }

                        gui.gui_filler(jd);
                    }

                    gui.gui_space(id);
                }
                else
                {
                    st_goal.balls_id = st_goal.score_id = st_goal.coins_id = 0;
                }

                Util.gui_score_board(id, (Util.GUI_MOST_COINS |
                                     Util.GUI_BEST_TIMES |
                                     Util.GUI_FAST_UNLOCK), 1, high);

                gui.gui_space(id);

                if ((jd = gui.gui_harray(id)) != 0)
                {
                    if (Progress.progress_done() != 0)
                        gui.gui_start(jd, "Finish", gui.GUI_SML, st_goal.GOAL_DONE, 0);
                    else if (Progress.progress_last() != 0)
                        gui.gui_start(jd, "Finish", gui.GUI_SML, st_goal.GOAL_LAST, 0);

                    if (Progress.progress_next_avail() != 0)
                        gui.gui_start(jd, "Next Level", gui.GUI_SML, st_goal.GOAL_NEXT, 0);

                    if (Progress.progress_same_avail() != 0)
                        gui.gui_start(jd, "Retry Level", gui.GUI_SML, st_goal.GOAL_SAME, 0);
                }

                if (st_goal.resume == 0)
                {
                    gui.gui_pulse(gid, 1.2f);
                }

                gui.gui_layout(id, 0, 0);

            }

            Util.set_score_board(l.most_coins, Progress.progress_coin_rank(),
                            l.best_times, Progress.progress_time_rank(),
                            l.fast_unlock, Progress.progress_goal_rank());

            Audio.audio_music_fade_out(2.0f);

            Video.video_clr_grab();

            /* Reset hack. */
            st_goal.resume = 0;

            return id;
        }


        internal override void OnLeave(int id)
        {
            /* HACK:  don't run animation if only "visiting" a state. */
            m_fTickGoal = st_goal.resume == 0;

            gui.gui_delete(id);
        }


        internal override void OnPaint(int id, float t)
        {
            game_client.ball_game_draw(0, t);
            gui.gui_paint(id);
        }


        bool m_fTickGoal = true;
        static float t = 0.0f;
        internal override void OnTimer(int id, float dt)
        {
            if (m_fTickGoal)
            {
                t += dt;

                if (State.time_state() < 1)
                {
                    game_server.game_server_step(dt);
                    game_client.game_client_step();
                }
                else if (t > 0.05f &&
                    st_goal.coins_id != 0)
                {
                    int coins = gui.gui_value(st_goal.coins_id);

                    if (coins > 0)
                    {
                        int score = gui.gui_value(st_goal.score_id);
                        int balls = gui.gui_value(st_goal.balls_id);

                        gui.gui_set_count(st_goal.coins_id, coins - 1);
                        gui.gui_pulse(st_goal.coins_id, 1.1f);

                        gui.gui_set_count(st_goal.score_id, score + 1);
                        gui.gui_pulse(st_goal.score_id, 1.1f);

                        if (Progress.progress_reward_ball(score + 1) != 0)
                        {
                            gui.gui_set_count(st_goal.balls_id, balls + 1);
                            gui.gui_pulse(st_goal.balls_id, 2.0f);
                            Audio.audio_play(Common.AUD_BALL, 1.0f);
                        }
                    }
                    t = 0.0f;
                }
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
                if (Config.config_tst_d(Config.CONFIG_KEY_SCORE_NEXT, c) != 0)
                {
                    return st_goal.goal_action(Util.gui_score_next(Util.gui_score_get()));
                }

                if (Config.config_tst_d(Config.CONFIG_KEY_RESTART, c) != 0 &&
                    Progress.progress_same_avail() != 0)
                {
                    return st_goal.goal_action(st_goal.GOAL_SAME);
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
                    return st_goal.goal_action(gui.gui_token(gui.gui_click()));
                }

                if (Config.config_tst_d(Config.CONFIG_JOYSTICK_BUTTON_EXIT, b) != 0)
                {
                    return st_goal.goal_action(st_goal.GOAL_BACK);
                }
            }

            return 1;
        }


        public st_ball_goal_class()
        {
            pointer = 1;
            gui_id = 0;
        }
    }
}
