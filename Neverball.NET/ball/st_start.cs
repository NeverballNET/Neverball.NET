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
    class st_start
    {
        public const int START_BACK = -1;
        public const int START_CHALLENGE = -2;
        public const int START_OPEN_GOALS = -3;
        public const int START_LOCK_GOALS = -4;

        public static int shot_id;
        public static int file_id;
        public static int challenge_id;


        /* Create a level selector button based upon its existence and status. */

        public static void gui_level(int id, int i)
        {
            float[] fore = null;
            float[] back = null;

            int jd;

            if (Level.level_exists(i) == 0)
            {
                gui.gui_label(id, " ", gui.GUI_SML, gui.GUI_ALL, widget.gui_blk, widget.gui_blk);
                return;
            }

            if (Level.level_opened(i) != 0)
            {
                fore = Level.level_bonus(i) != 0 ? widget.gui_grn : widget.gui_wht;
                back = Level.level_completed(i) != 0 ? fore : widget.gui_yel;
            }

            jd = gui.gui_label(id, Level.level_name(i), gui.GUI_SML, gui.GUI_ALL, back, fore);

            if (Level.level_opened(i) != 0 || Config.config_cheat() != 0)
                gui.gui_active(jd, i, 0);
        }


        public static void start_over_level(int i)
        {
            if (Level.level_opened(i) != 0 || Config.config_cheat() != 0)
            {
                gui.gui_set_image(shot_id, Level.level_shot(i));

                Util.set_score_board(Set.get_level(i).most_coins, -1,
                                Set.get_level(i).best_times, -1,
                                Set.get_level(i).fast_unlock, -1);

                if (file_id != 0)
                    gui.gui_set_label(file_id, Level.level_file(i));
            }
        }


        public static void start_over(int id, int pulse)
        {
            int i;

            if (id == 0)
                return;

            if (pulse != 0)
                gui.gui_pulse(id, 1.2f);

            i = gui.gui_token(id);

            if (i == START_CHALLENGE || i == START_BACK)
            {
                gui.gui_set_image(shot_id, Set.set_shot(Set.curr_set()));

                Util.set_score_board(Set.set_coin_score(Set.curr_set()), -1,
                                Set.set_time_score(Set.curr_set()), -1,
                                null, -1);
            }

            if (i >= 0 &&
                Util.GUI_ISMSK(i) == 0)
                start_over_level(i);
        }


        public static int start_action(int i)
        {
            Audio.audio_play(Common.AUD_MENU, 1.0f);

            switch (i)
            {
                case START_BACK:
                    return State.goto_state(st_set.get_st_set());

                case START_CHALLENGE:
                    if (Config.config_cheat() != 0)
                    {
                        Progress.progress_init(Progress.curr_mode() == MODE.MODE_CHALLENGE ?
                                      MODE.MODE_NORMAL : MODE.MODE_CHALLENGE);
                        gui.gui_toggle(challenge_id);
                        return 1;
                    }
                    else
                    {
                        Progress.progress_init(MODE.MODE_CHALLENGE);
                        return start_action(0);
                    }

                case Util.GUI_MOST_COINS:
                case Util.GUI_BEST_TIMES:
                case Util.GUI_FAST_UNLOCK:
                    Util.gui_score_set(i);
                    return State.goto_state(get_st_start());

                case START_OPEN_GOALS:
                    Config.config_set_d(Config.CONFIG_LOCK_GOALS, 0);
                    return State.goto_state(get_st_start());

                case START_LOCK_GOALS:
                    Config.config_set_d(Config.CONFIG_LOCK_GOALS, 1);
                    return State.goto_state(get_st_start());

                default:
                    if (Progress.progress_play(i) != 0)
                        return State.goto_state(st_level.get_st_level());
                    break;
            }

            return 1;
        }


        public static State get_st_start()
        {
            if (st_start_class.Instance == null)
            {
                st_start_class.Instance = new st_start_class();
            }

            return st_start_class.Instance;
        }
    }


    class st_start_class : State
    {
        public static st_start_class Instance = null;


        internal override int OnEnter()
        {
            int w = Config.config_get_d(Config.CONFIG_WIDTH);
            int h = Config.config_get_d(Config.CONFIG_HEIGHT);
            int i, j;

            int id, jd, kd, ld;

            Progress.progress_init(MODE.MODE_NORMAL);

            if ((id = gui.gui_vstack(0)) != 0)
            {
                if ((jd = gui.gui_hstack(id)) != 0)
                {

                    gui.gui_label(jd, Set.set_name(Set.curr_set()), gui.GUI_SML, gui.GUI_ALL,
                              widget.gui_yel, widget.gui_red);
                    gui.gui_filler(jd);
                    gui.gui_start(jd, "Back", gui.GUI_SML, st_start.START_BACK, 0);
                }

                gui.gui_space(id);

                if ((jd = gui.gui_harray(id)) != 0)
                {
                    if (Config.config_cheat() != 0)
                    {
                        if ((kd = gui.gui_vstack(jd)) != 0)
                        {
                            st_start.shot_id = gui.gui_image(kd, Set.set_shot(Set.curr_set()), 6 * w / 16, 6 * h / 16);

                            st_start.file_id = gui.gui_label(kd, " ", gui.GUI_SML, gui.GUI_ALL,
                                                widget.gui_yel, widget.gui_red);
                        }
                    }
                    else
                    {
                        st_start.shot_id = gui.gui_image(jd, Set.set_shot(Set.curr_set()), 7 * w / 16, 7 * h / 16);
                    }

                    if ((kd = gui.gui_varray(jd)) != 0)
                    {
                        for (i = 0; i < 5; i++)
                            if ((ld = gui.gui_harray(kd)) != 0)
                                for (j = 4; j >= 0; j--)
                                    st_start.gui_level(ld, i * 5 + j);

                        st_start.challenge_id = gui.gui_state(kd, "Challenge",
                                                 gui.GUI_SML, st_start.START_CHALLENGE,
                                                 Progress.curr_mode() == MODE.MODE_CHALLENGE ? 1 : 0);
                    }
                }
                gui.gui_space(id);
                Util.gui_score_board(id, (Util.GUI_MOST_COINS |
                                     Util.GUI_BEST_TIMES |
                                     Util.GUI_FAST_UNLOCK), 0, 0);
                gui.gui_space(id);

                if ((jd = gui.gui_hstack(id)) != 0)
                {
                    gui.gui_filler(jd);

                    if ((kd = gui.gui_harray(jd)) != 0)
                    {
                        /* TODO, replace the whitespace hack with something sane. */

                        gui.gui_state(kd,
                            /* Translators: adjust the amount of whitespace here
                             * as necessary for the buttons to look good. */
                                  "   No   ", gui.GUI_SML, st_start.START_OPEN_GOALS,
                                  Config.config_get_d(Config.CONFIG_LOCK_GOALS) == 0 ? 1 : 0);

                        gui.gui_state(kd, "Yes", gui.GUI_SML, st_start.START_LOCK_GOALS,
                                  Config.config_get_d(Config.CONFIG_LOCK_GOALS) == 1 ? 1 : 0);
                    }

                    gui.gui_space(jd);

                    gui.gui_label(jd, "Lock Goals of Completed Levels?",
                              gui.GUI_SML, gui.GUI_ALL, null, null);

                    gui.gui_filler(jd);
                }

                gui.gui_layout(id, 0, 0);

                Util.set_score_board(null, -1, null, -1, null, -1);
            }

            Audio.audio_music_fade_to(0.5f, "bgm/inter.ogg");

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
            st_start.start_over(gui.gui_point(id, x, y), 1);
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
                if (c == Alt.GUI.Keys.C && Config.config_cheat() != 0)
                {
                    Set.set_cheat();
                    return State.goto_state(st_start.get_st_start());
                }
                else if (Config.config_tst_d(Config.CONFIG_KEY_SCORE_NEXT, c) != 0)
                {
                    int active = gui.gui_click();

                    if (st_start.start_action(Util.gui_score_next(Util.gui_score_get())) != 0)
                    {
                        /* HACK ALERT
                         *
                         * This assumes that 'active' is a valid widget ID even after
                         * the above start_action has recreated the entire widget
                         * hierarchy.  Maybe it is.  Maybe it isn't.
                         */
                        gui.gui_focus(active);
                        st_start.start_over(active, 0);

                        return 1;
                    }
                    else
                        return 0;
                }
            }

            return 1;
        }


        internal override int OnButton(int b, int d)
        {
            if (d != 0)
            {
                if (Config.config_tst_d(Config.CONFIG_JOYSTICK_BUTTON_A, b) != 0)
                    return st_start.start_action(gui.gui_token(gui.gui_click()));
                if (Config.config_tst_d(Config.CONFIG_JOYSTICK_BUTTON_EXIT, b) != 0)
                    return st_start.start_action(st_start.START_BACK);
            }
            return 1;
        }


        public st_start_class()
        {
            pointer = 1;
            gui_id = 0;
        }
    }
}
