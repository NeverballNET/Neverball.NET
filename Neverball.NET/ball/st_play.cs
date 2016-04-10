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
    class st_play
    {
        public static float view_rotate;
        public static int fast_rotate;


        public static int show_hud;


        public static float phi;
        public static float theta;


        public static void set_camera(VIEW c)
        {
            Config.config_set_d(Config.CONFIG_CAMERA, (int)c);
            hud.hud_view_pulse((int)c);
        }


        public static void toggle_camera()
        {
            VIEW cam = (Config.config_tst_d(Config.CONFIG_CAMERA, (int)VIEW.VIEW_MANUAL) != 0 ?
                       VIEW.VIEW_CHASE : VIEW.VIEW_MANUAL);

            set_camera(cam);
        }


        public static void keybd_camera(Alt.GUI.Keys c)
        {
            if (Config.config_tst_d(Config.CONFIG_KEY_CAMERA_1, c) != 0)
                set_camera(VIEW.VIEW_CHASE);
            if (Config.config_tst_d(Config.CONFIG_KEY_CAMERA_2, c) != 0)
                set_camera(VIEW.VIEW_LAZY);
            if (Config.config_tst_d(Config.CONFIG_KEY_CAMERA_3, c) != 0)
                set_camera(VIEW.VIEW_MANUAL);

            if (c == Alt.GUI.Keys.F4 && Config.config_cheat() != 0)
                set_camera(VIEW.VIEW_TOPDOWN);

            if (Config.config_tst_d(Config.CONFIG_KEY_CAMERA_TOGGLE, c) != 0)
                toggle_camera();
        }


        public static void click_camera(Alt.GUI.MouseButtons b)
        {
            if (Config.config_tst_d(Config.CONFIG_MOUSE_CAMERA_1, b) != 0)
                set_camera(VIEW.VIEW_CHASE);
            if (Config.config_tst_d(Config.CONFIG_MOUSE_CAMERA_2, b) != 0)
                set_camera(VIEW.VIEW_LAZY);
            if (Config.config_tst_d(Config.CONFIG_MOUSE_CAMERA_3, b) != 0)
                set_camera(VIEW.VIEW_MANUAL);

            if (Config.config_tst_d(Config.CONFIG_MOUSE_CAMERA_TOGGLE, b) != 0)
                toggle_camera();
        }


        public static int pause_or_exit()
        {
            if (Config.config_tst_d(Config.CONFIG_KEY_PAUSE, Alt.GUI.Keys.Escape) != 0)
            {
                return st_pause.goto_pause();
            }
            else
            {
                Progress.progress_stat(GAME.GAME_NONE);

                Video.video_clr_grab();

                return State.goto_state(st_over.get_st_ball_over());
            }
        }


        public static State get_st_play_ready()
        {
            if (st_play_ready_class.Instance == null)
            {
                st_play_ready_class.Instance = new st_play_ready_class();
            }

            return st_play_ready_class.Instance;
        }

        public static State get_st_play_set()
        {
            if (st_play_set_class.Instance == null)
            {
                st_play_set_class.Instance = new st_play_set_class();
            }

            return st_play_set_class.Instance;
        }

        public static State get_st_play_loop()
        {
            if (st_play_loop_class.Instance == null)
            {
                st_play_loop_class.Instance = new st_play_loop_class();
            }

            return st_play_loop_class.Instance;
        }

        public static State get_st_look()
        {
            if (st_look_class.Instance == null)
            {
                st_look_class.Instance = new st_look_class();
            }

            return st_look_class.Instance;
        }
    }



    class st_play_ready_class : State
    {
        public static st_play_ready_class Instance;


        internal override int OnEnter()
        {
            int id;

            if ((id = gui.gui_label(0, "Ready?", gui.GUI_LRG, gui.GUI_ALL, null, null)) != 0)
            {
                gui.gui_layout(id, 0, 0);
                gui.gui_pulse(id, 1.2f);
            }

            Audio.audio_play(Common.AUD_READY, 1.0f);
            Video.video_set_grab(1);

            hud.hud_view_pulse(Config.config_get_d(Config.CONFIG_CAMERA));

            return id;
        }


        internal override void OnLeave(int id)
        {
            gui.gui_delete(id);
        }


        internal override void OnPaint(int id, float t)
        {
            game_client.ball_game_draw(0, t);
            hud.hud_view_paint();
            gui.gui_paint(id);
        }


        internal override void OnTimer(int id, float dt)
        {
            float t = State.time_state();

            game_server.game_set_fly(1.0f - 0.5f * t, null);
            game_client.game_client_step();

            if (dt > 0.0f && t > 1.0f)
                State.goto_state(st_play.get_st_play_set());

            game_client.game_step_fade(dt);
            hud.hud_view_timer(dt);
            gui.gui_timer(id, dt);
        }


        internal override int OnClick(Alt.GUI.MouseButtons b, int d)
        {
            if (d != 0)
            {
                st_play.click_camera(b);

                if (b == Alt.GUI.MouseButtons.Left)
                {
                    State.goto_state(st_play.get_st_play_loop());
                }
            }

            return 1;
        }


        internal override int OnKeybd(Alt.GUI.Keys c, int d)
        {
            if (d != 0)
            {
                st_play.keybd_camera(c);

                if (Config.config_tst_d(Config.CONFIG_KEY_PAUSE, c) != 0)
                    st_pause.goto_pause();
            }

            return 1;
        }


        internal override int OnButton(int b, int d)
        {
            if (d != 0)
            {
                if (Config.config_tst_d(Config.CONFIG_JOYSTICK_BUTTON_A, b) != 0)
                    return State.goto_state(st_play.get_st_play_loop());
                if (Config.config_tst_d(Config.CONFIG_JOYSTICK_BUTTON_EXIT, b) != 0)
                    return st_play.pause_or_exit();
            }
            return 1;
        }


        public st_play_ready_class()
        {
            pointer = 1;
            gui_id = 0;
        }
    }


    class st_play_set_class : State
    {
        public static st_play_set_class Instance;


        internal override int OnEnter()
        {
            int id;

            if ((id = gui.gui_label(0, "Set?", gui.GUI_LRG, gui.GUI_ALL, null, null)) != 0)
            {
                gui.gui_layout(id, 0, 0);
                gui.gui_pulse(id, 1.2f);
            }

            Audio.audio_play(Common.AUD_SET, 1);

            st_pause.clear_pause();

            return id;
        }


        internal override void OnLeave(int id)
        {
            gui.gui_delete(id);
        }


        internal override void OnPaint(int id, float t)
        {
            game_client.ball_game_draw(0, t);
            hud.hud_view_paint();
            gui.gui_paint(id);
        }


        internal override void OnTimer(int id, float dt)
        {
            float t = State.time_state();

            game_server.game_set_fly(0.5f - 0.5f * t, null);
            game_client.game_client_step();

            if (dt > 0.0f && t > 1.0f)
                State.goto_state(st_play.get_st_play_loop());

            game_client.game_step_fade(dt);
            hud.hud_view_timer(dt);
            gui.gui_timer(id, dt);
        }


        internal override int OnClick(Alt.GUI.MouseButtons b, int d)
        {
            if (d != 0)
            {
                st_play.click_camera(b);

                if (b == Alt.GUI.MouseButtons.Left)
                {
                    State.goto_state(st_play.get_st_play_loop());
                }
            }

            return 1;
        }


        internal override int OnKeybd(Alt.GUI.Keys c, int d)
        {
            if (d != 0)
            {
                st_play.keybd_camera(c);

                if (Config.config_tst_d(Config.CONFIG_KEY_PAUSE, c) != 0)
                    st_pause.goto_pause();
            }

            return 1;
        }


        internal override int OnButton(int b, int d)
        {
            if (d != 0)
            {
                if (Config.config_tst_d(Config.CONFIG_JOYSTICK_BUTTON_A, b) != 0)
                    return State.goto_state(st_play.get_st_play_loop());
                if (Config.config_tst_d(Config.CONFIG_JOYSTICK_BUTTON_EXIT, b) != 0)
                    return st_play.pause_or_exit();
            }
            return 1;
        }


        public st_play_set_class()
        {
            pointer = 1;
            gui_id = 0;
        }
    }


    class st_play_loop_class : State
    {
        public static st_play_loop_class Instance;


        internal override int OnEnter()
        {
            Command cmd = new Command();
            int id;

            if (st_pause.is_paused() != 0)
            {
                st_pause.clear_pause();
                st_play.view_rotate = 0;
                st_play.fast_rotate = 0;
                return 0;
            }

            if ((id = gui.gui_label(0, "GO!", gui.GUI_LRG, gui.GUI_ALL, widget.gui_blu, widget.gui_grn)) != 0)
            {
                gui.gui_layout(id, 0, 0);
                gui.gui_pulse(id, 1.2f);
            }

            Audio.audio_play(Common.AUD_GO, 1);

            game_server.game_set_fly(0, null);

            /* End first update. */

            cmd.type = cmd_type.CMD_END_OF_UPDATE;
            enq_fn_game_proxy_enq.game_proxy_enq(cmd);

            /* Sync client. */

            game_client.game_client_step();

            st_play.view_rotate = 0;
            st_play.fast_rotate = 0;

            st_play.show_hud = 1;

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

            if (st_play.show_hud != 0)
                hud.hud_paint();

            if (State.time_state() < 1)
                gui.gui_paint(id);
        }


        internal override void OnTimer(int id, float dt)
        {
            float k = (st_play.fast_rotate != 0 ?
                       (float)Config.config_get_d(Config.CONFIG_ROTATE_FAST) / 100.0f :
                       (float)Config.config_get_d(Config.CONFIG_ROTATE_SLOW) / 100.0f);

            gui.gui_timer(id, dt);
            hud.hud_timer(dt);
            game_server.game_set_rot(st_play.view_rotate * k);
            game_server.game_set_cam(Config.config_get_d(Config.CONFIG_CAMERA));

            game_client.game_step_fade(dt);

            game_server.game_server_step(dt);
            game_client.game_client_step();

            switch (game_client.curr_status())
            {
                case (int)GAME.GAME_GOAL:
                    Progress.progress_stat(GAME.GAME_GOAL);
                    State.goto_state(st_goal.get_st_ball_goal());
                    break;

                case (int)GAME.GAME_FALL:
                    Progress.progress_stat(GAME.GAME_FALL);
                    State.goto_state(st_fall_out.get_st_fall_out());
                    break;

                case (int)GAME.GAME_TIME:
                    Progress.progress_stat(GAME.GAME_TIME);
                    State.goto_state(st_time_out.get_st_time_out());
                    break;

                default:
                    Progress.progress_step();
                    break;
            }
        }


        internal override void OnPoint(int id, int x, int y, int dx, int dy)
        {
            game_server.game_set_pos(dx, dy);
        }


        internal override int OnClick(Alt.GUI.MouseButtons b, int d)
        {
            if (d != 0)
            {
                if (Config.config_tst_d(Config.CONFIG_MOUSE_CAMERA_R, b) != 0)
                    st_play.view_rotate = +1;
                if (Config.config_tst_d(Config.CONFIG_MOUSE_CAMERA_L, b) != 0)
                    st_play.view_rotate = -1;

                st_play.click_camera(b);
            }
            else
            {
                if (Config.config_tst_d(Config.CONFIG_MOUSE_CAMERA_R, b) != 0)
                    st_play.view_rotate = 0;
                if (Config.config_tst_d(Config.CONFIG_MOUSE_CAMERA_L, b) != 0)
                    st_play.view_rotate = 0;
            }

            return 1;
        }


        internal override int OnKeybd(Alt.GUI.Keys c, int d)
        {
            if (d != 0)
            {
                if (Config.config_tst_d(Config.CONFIG_KEY_CAMERA_R, c) != 0)
                {
                    st_play.view_rotate = +1;
                }

                if (Config.config_tst_d(Config.CONFIG_KEY_CAMERA_L, c) != 0)
                {
                    st_play.view_rotate = -1;
                }

                if (Config.config_tst_d(Config.CONFIG_KEY_ROTATE_FAST, c) != 0)
                {
                    st_play.fast_rotate = 1;
                }

                st_play.keybd_camera(c);

                if (Config.config_tst_d(Config.CONFIG_KEY_RESTART, c) != 0 &&
                    Progress.progress_same_avail() != 0)
                {
                    if (Progress.progress_same() != 0)
                    {
                        State.goto_state(st_play.get_st_play_ready());
                    }
                }

                if (Config.config_tst_d(Config.CONFIG_KEY_PAUSE, c) != 0)
                {
                    st_pause.goto_pause();
                }
            }
            else
            {
                if (Config.config_tst_d(Config.CONFIG_KEY_CAMERA_R, c) != 0)
                {
                    st_play.view_rotate = 0;
                }

                if (Config.config_tst_d(Config.CONFIG_KEY_CAMERA_L, c) != 0)
                {
                    st_play.view_rotate = 0;
                }

                if (Config.config_tst_d(Config.CONFIG_KEY_ROTATE_FAST, c) != 0)
                {
                    st_play.fast_rotate = 0;
                }
            }

            if (d != 0 &&
                c == Alt.GUI.Keys.F12 &&
                Config.config_cheat() != 0)
            {
                return State.goto_state(st_play.get_st_look());
            }

            if (d != 0 &&
                c == Alt.GUI.Keys.F6)
            {
                st_play.show_hud =
                    //!st_play.show_hud;
                    st_play.show_hud == 0 ? 1 : 0;
            }

            if (d != 0 &&
                c == Alt.GUI.Keys.C &&
                Config.config_cheat() != 0)
            {
                Progress.progress_stat(GAME.GAME_GOAL);
                return State.goto_state(st_goal.get_st_ball_goal());
            }

            return 1;
        }


        internal override int OnButton(int b, int d)
        {
            if (d == 1)
            {
                if (Config.config_tst_d(Config.CONFIG_JOYSTICK_BUTTON_EXIT, b) != 0)
                    st_play.pause_or_exit();
            }

            return 1;
        }


        public st_play_loop_class()
        {
            pointer = 0;
            gui_id = 0;
        }
    }


    class st_look_class : State
    {
        public static st_look_class Instance;


        internal override int OnEnter()
        {
            st_play.phi = 0;
            st_play.theta = 0;
            return 0;
        }


        internal override void OnPaint(int id, float t)
        {
            game_client.ball_game_draw(0, t);
        }


        internal override void OnPoint(int id, int x, int y, int dx, int dy)
        {
            st_play.phi += 90.0f * dy / Config.config_get_d(Config.CONFIG_HEIGHT);
            st_play.theta += 180.0f * dx / Config.config_get_d(Config.CONFIG_WIDTH);

            if (st_play.phi > +90.0f) st_play.phi = +90.0f;
            if (st_play.phi < -90.0f) st_play.phi = -90.0f;

            if (st_play.theta > +180.0f) st_play.theta -= 360.0f;
            if (st_play.theta < -180.0f) st_play.theta += 360.0f;

            game_client.game_look(st_play.phi, st_play.theta);
        }


        internal override int OnKeybd(Alt.GUI.Keys c, int d)
        {
            if (d != 0 && c == Alt.GUI.Keys.F12)
                return State.goto_state(st_play.get_st_play_loop());

            return 1;
        }


        internal override int OnButton(int b, int d)
        {
            if (d != 0 &&
                Config.config_tst_d(Config.CONFIG_JOYSTICK_BUTTON_EXIT, b) != 0)
            {
                return State.goto_state(st_play.get_st_play_loop());
            }

            return 1;
        }


        public st_look_class()
        {
            pointer = 0;
            gui_id = 0;
        }
    }
}
