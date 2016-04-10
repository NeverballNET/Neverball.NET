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
    class st_title
    {
        public static float real_time = 0.0f;
        public static int mode = 0;

        public static int play_id = 0;


        public const int TITLE_PLAY = 1;
        public const int TITLE_EXIT = 5;


        public static int init_title_level()
        {
            if (game_client.game_client_init("map-medium/title.sol") != 0)
            {
                Command cmd = new Command();

                cmd.type = cmd_type.CMD_GOAL_OPEN;
                enq_fn_game_proxy_enq.game_proxy_enq(cmd);

                game_client.game_client_step();

                return 1;
            }
            return 0;
        }


        public static int title_action(int i)
        {
            Audio.audio_play(Common.AUD_MENU, 1.0f);

            switch (i)
            {
                case TITLE_PLAY:
                    return State.goto_state(st_set.get_st_set());

                case TITLE_EXIT:
                    return 0;
            }

            return 1;
        }


        public static State st_ball_title()
        {
            if (st_ball_title_class.Instance == null)
            {
                st_ball_title_class.Instance = new st_ball_title_class();
            }

            return st_ball_title_class.Instance;
        }
    }



    class st_ball_title_class : State
    {
        public static st_ball_title_class Instance = null;


        internal override int OnEnter()
        {
            int id, jd, kd;

            /* Build the title GUI. */

            if ((id = gui.gui_vstack(0)) != 0)
            {
                gui.gui_label(id, "Neverball", gui.GUI_LRG, gui.GUI_ALL, null, null);

                gui.gui_space(id);

                if ((jd = gui.gui_harray(id)) != 0)
                {
                    gui.gui_filler(jd);

                    if ((kd = gui.gui_varray(jd)) != 0)
                    {
                        if (Config.config_cheat() != 0)
                        {
                            st_title.play_id = gui.gui_start(kd, "Cheat",
                                                gui.GUI_MED, st_title.TITLE_PLAY, 1);
                        }
                        else
                        {
                            st_title.play_id = gui.gui_start(kd, "Play",
                                                gui.GUI_MED, st_title.TITLE_PLAY, 1);
                        }

                        gui.gui_state(kd, "Exit", gui.GUI_MED, st_title.TITLE_EXIT, 0);
                    }

                    gui.gui_filler(jd);
                }

                gui.gui_layout(id, 0, 0);
            }

            /* Start the title screen music. */

            Audio.audio_music_fade_to(0.5f, "bgm/title.ogg");

            /* Initialize the title level for display. */

            st_title.init_title_level();

            st_title.real_time = 0.0f;
            st_title.mode = 0;

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


        static string demo = null;
        internal override void OnTimer(int id, float dt)
        {
            st_title.real_time += dt;

            switch (st_title.mode)
            {
                case 0: /* Mode 0: Pan across title level. */

                    if (st_title.real_time <= 20.0f)
                    {
                        game_server.game_set_fly((float)System.Math.Cos(MathHelper.Pi * st_title.real_time / 20.0f),
                                     game_client.game_client_file());
                        game_client.game_client_step();
                    }
                    else
                    {
                        game_client.game_fade(+1.0f);
                        st_title.real_time = 0.0f;
                        st_title.mode = 1;
                    }
                    break;

                case 1: /* Mode 1: Fade out.  Load demo level. */

                    if (st_title.real_time > 1.0f)
                    {
                        game_client.game_fade(-1.0f);
                        st_title.real_time = 0.0f;
                        st_title.mode = 0;
                    }
                    break;
            }

            gui.gui_timer(id, dt);
            game_client.game_step_fade(dt);
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


        internal override int OnButton(int b, int d)
        {
            if (d != 0)
            {
                if (Config.config_tst_d(Config.CONFIG_JOYSTICK_BUTTON_A, b) != 0)
                    return st_title.title_action(gui.gui_token(gui.gui_click()));
                if (Config.config_tst_d(Config.CONFIG_JOYSTICK_BUTTON_EXIT, b) != 0)
                    return 0;
            }
            return 1;
        }


        public st_ball_title_class()
        {
            pointer = 1;
            gui_id = 0;
        }
    }
}
