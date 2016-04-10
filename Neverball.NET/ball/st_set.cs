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
    class st_set
    {
        public static int SET_STEP = 6;

        public static int total = 0;
        public static int first = 0;

        public static int shot_id;
        public static int desc_id;

        public static int do_init = 1;


        public static void gui_set(int id, int i)
        {
            if (Set.set_exists(i) != 0)
                gui.gui_state(id, Set.set_name(i), gui.GUI_SML, i, 0);
            else
                gui.gui_label(id, "", gui.GUI_SML, gui.GUI_ALL, null, null);
        }


        public static void set_over(int i)
        {
            gui.gui_set_image(st_set.shot_id, Set.set_shot(i));

            gui.gui_set_multi(st_set.desc_id, Set.set_desc(i));
        }


        public static int set_action(int i)
        {
            Audio.audio_play(Common.AUD_MENU, 1.0f);

            switch (i)
            {
                case Util.GUI_BACK:
                    Set.set_quit();
                    return State.goto_state(st_title.st_ball_title());

                case Util.GUI_PREV:

                    st_set.first -= st_set.SET_STEP;

                    st_set.do_init = 0;
                    return State.goto_state(get_st_set());

                case Util.GUI_NEXT:

                    st_set.first += st_set.SET_STEP;

                    st_set.do_init = 0;
                    return State.goto_state(get_st_set());

                case Util.GUI_NULL:
                    return 1;

                default:
                    if (Set.set_exists(i) != 0)
                    {
                        Set.set_goto(i);
                        return State.goto_state(st_start.get_st_start());
                    }
                    break;
            }

            return 1;
        }


        public static State get_st_set()
        {
            if (st_set_class.Instance == null)
            {
                st_set_class.Instance = new st_set_class();
            }

            return st_set_class.Instance;
        }
    }



    class st_set_class : State
    {
        public static st_set_class Instance = null;


        internal override int OnEnter()
        {
            int w = Config.config_get_d(Config.CONFIG_WIDTH);
            int h = Config.config_get_d(Config.CONFIG_HEIGHT);

            int id, jd, kd;

            int i;

            if (st_set.do_init != 0)
            {
                st_set.total = Set.set_init();
                st_set.first = System.Math.Min//MIN
                    (st_set.first, (st_set.total - 1) - ((st_set.total - 1) % st_set.SET_STEP));

                Audio.audio_music_fade_to(0.5f, "bgm/inter.ogg");
                Audio.audio_play(Common.AUD_START, 1);
            }
            else
                st_set.do_init = 1;

            if ((id = gui.gui_vstack(0)) != 0)
            {
                if ((jd = gui.gui_hstack(id)) != 0)
                {
                    gui.gui_label(jd, "Level Set", gui.GUI_SML, gui.GUI_ALL, widget.gui_yel, widget.gui_red);
                    gui.gui_filler(jd);
                    Util.gui_navig(jd, (st_set.first > 0) ? 1 : 0, (st_set.first + st_set.SET_STEP < st_set.total) ? 1 : 0);
                }

                gui.gui_space(id);

                if ((jd = gui.gui_harray(id)) != 0)
                {
                    st_set.shot_id = gui.gui_image(jd, Set.set_shot(st_set.first), 7 * w / 16, 7 * h / 16);

                    if ((kd = gui.gui_varray(jd)) != 0)
                    {
                        for (i = st_set.first; i < st_set.first + st_set.SET_STEP; i++)
                            st_set.gui_set(kd, i);
                    }
                }

                gui.gui_space(id);
                st_set.desc_id = gui.gui_multi(id, " \\ \\ \\ \\ \\", gui.GUI_SML, gui.GUI_ALL,
                                    widget.gui_yel, widget.gui_wht);

                gui.gui_layout(id, 0, 0);
            }

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

            int i = gui.gui_token(jd);
            if (jd != 0 &&
                Set.set_exists(i) != 0)
                st_set.set_over(i);
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


        internal override int OnButton(int b, int d)
        {
            if (d != 0)
            {
                if (Config.config_tst_d(Config.CONFIG_JOYSTICK_BUTTON_A, b) != 0)
                    return st_set.set_action(gui.gui_token(gui.gui_click()));
                if (Config.config_tst_d(Config.CONFIG_JOYSTICK_BUTTON_EXIT, b) != 0)
                    return st_set.set_action(Util.GUI_BACK);
            }
            return 1;
        }


        public st_set_class()
        {
            pointer = 1;
            gui_id = 0;
        }
    }
}
