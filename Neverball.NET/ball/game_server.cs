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
    class game_server
    {
        public const float ANGLE_BOUND = 20.0f;             /* Angle limit of floor tilting       */
        public const float VIEWR_BOUND = 10.0f;             /* Maximum rate of view rotation      */


        public static game_tilt tilt = new game_tilt();           /* Floor rotation                    */


        public static int server_state = 0;


        public static float timer = 0;          /* Clock time                        */
        public static int timer_down = 1;            /* Timer go up or down?              */

        public static GAME status = GAME.GAME_NONE;          /* Outcome of the game               */


        public static double view_a;                    /* Ideal view rotation about Y axis  */
        public static float view_dc;                   /* Ideal view distance above ball    */
        public static float view_dp;                   /* Ideal view distance above ball    */
        public static float view_dz;                   /* Ideal view distance behind ball   */
        public static float[][] view_e = new float[3][]
        {
            new float[3],
            new float[3],
            new float[3]
        };              /* Current view reference frame      */


        public static float view_k;

        public static int coins = 0;                /* Collected coins                   */
        public static int goal_e = 0;                /* Goal enabled flag                 */
        public static float goal_k = 0;                /* Goal animation                    */
        public static int jump_e = 1;                /* Jumping enabled flag              */
        public static int jump_b = 0;                /* Jump-in-progress flag             */
        public static float jump_dt;                   /* Jump duration                     */


        public static Command cmd = new Command();

        public static s_file file = new s_file();


        public static float[] view_c = new float[3];                 /* Current view center               */
        public static float[] view_v = new float[3];                 /* Current view vector               */
        public static float[] view_p = new float[3];                 /* Current view position             */

        public static float[] jump_p = new float[3];                 /* Jump destination                  */
        public static float[] jump_w = new float[3];                 /* View destination                  */



        public static void game_cmd_sound(string filename, float a)
        {
            cmd.type = cmd_type.CMD_SOUND;

            cmd.sound.n = filename;
            cmd.sound.a = a;

            enq_fn_game_proxy_enq.game_proxy_enq(cmd);
        }


        public static void audio_play(string filename, float a)
        {
            game_cmd_sound(filename, a);
        }



        /*
         * This is an abstraction of the game's input state.  All input is
         * encapsulated here, and all references to the input by the game are
         * made here.  TODO: This used to have the effect of homogenizing
         * input for use in replay recording and playback, but it's not clear
         * how relevant this approach is with the introduction of the command
         * pipeline.
         *
         * x and z:
         *     -32767 = -ANGLE_BOUND
         *     +32767 = +ANGLE_BOUND
         *
         * r:
         *     -32767 = -VIEWR_BOUND
         *     +32767 = +VIEWR_BOUND
         *
         */

        public struct input
        {
            public short x;
            public short z;
            public short r;
            public short c;
        }

        public static input input_current = new input();



        public static int grow = 0;                  /* Should the ball be changing size? */
        public static float grow_orig = 0;             /* the original ball size            */
        public static float grow_goal = 0;             /* how big or small to get!          */
        public static float grow_t = 0;              /* timer for the ball to grow...     */
        public static float grow_strt = 0;             /* starting value for growth         */
        public static int got_orig = 0;              /* Do we know original ball size?    */

        public const float GROW_TIME = 0.5f;                 /* sec for the ball to get to size.  */
        public const float GROW_BIG = 1.5f;                /* large factor                      */
        public const float GROW_SMALL = 0.5f;               /* small factor                      */

        public static int grow_state = 0;            /* Current state (values -1, 0, +1)  */



        public static void input_init()
        {
            input_current.x = 0;
            input_current.z = 0;
            input_current.r = 0;
            input_current.c = 0;
        }

        public static void input_set_x(float x)
        {
            if (x < -ANGLE_BOUND) x = -ANGLE_BOUND;
            if (x > ANGLE_BOUND) x = ANGLE_BOUND;

            input_current.x = (short)(32767.0f * x / ANGLE_BOUND);
        }

        public static void input_set_z(float z)
        {
            if (z < -ANGLE_BOUND) z = -ANGLE_BOUND;
            if (z > ANGLE_BOUND) z = ANGLE_BOUND;

            input_current.z = (short)(32767.0f * z / ANGLE_BOUND);
        }

        public static void input_set_r(float r)
        {
            if (r < -VIEWR_BOUND) r = -VIEWR_BOUND;
            if (r > VIEWR_BOUND) r = VIEWR_BOUND;

            input_current.r = (short)(32767.0f * r / VIEWR_BOUND);
        }

        public static void input_set_c(int c)
        {
            input_current.c = (short)c;
        }

        public static float input_get_x()
        {
            return ANGLE_BOUND * (float)input_current.x / 32767.0f;
        }

        public static float input_get_z()
        {
            return ANGLE_BOUND * (float)input_current.z / 32767.0f;
        }

        public static float input_get_r()
        {
            return VIEWR_BOUND * (float)input_current.r / 32767.0f;
        }

        public static int input_get_c()
        {
            return (int)input_current.c;
        }


        public static void view_init()
        {
            view_dp = (float)Config.config_get_d(Config.CONFIG_VIEW_DP) / 100.0f;
            view_dc = (float)Config.config_get_d(Config.CONFIG_VIEW_DC) / 100.0f;
            view_dz = (float)Config.config_get_d(Config.CONFIG_VIEW_DZ) / 100.0f;
            view_k = 1.0f;
            view_a = 0.0f;

            view_c[0] = 0;
            view_c[1] = view_dc;
            view_c[2] = 0;

            view_p[0] = 0;
            view_p[1] = view_dp;
            view_p[2] = view_dz;

            view_e[0][0] = 1;
            view_e[0][1] = 0;
            view_e[0][2] = 0;
            view_e[1][0] = 0;
            view_e[1][1] = 1;
            view_e[1][2] = 0;
            view_e[2][0] = 0;
            view_e[2][1] = 0;
            view_e[2][2] = 1;
        }


        /*
         * Utility functions for preparing the "server" state and events for
         * consumption by the "client".
         */


        public static void game_cmd_map(string name, int ver_x, int ver_y)
        {
            cmd.type = cmd_type.CMD_MAP;
            cmd.map.name = name;
            cmd.map.version.x = ver_x;
            cmd.map.version.y = ver_y;
            enq_fn_game_proxy_enq.game_proxy_enq(cmd);
        }


        public static void game_cmd_eou()
        {
            cmd.type = cmd_type.CMD_END_OF_UPDATE;
            enq_fn_game_proxy_enq.game_proxy_enq(cmd);
        }


        public static void game_cmd_ups()
        {
            cmd.type = cmd_type.CMD_UPDATES_PER_SECOND;
            cmd.ups.n = Config.UPS;
            enq_fn_game_proxy_enq.game_proxy_enq(cmd);
        }


        public static void game_cmd_goalopen()
        {
            cmd.type = cmd_type.CMD_GOAL_OPEN;
            enq_fn_game_proxy_enq.game_proxy_enq(cmd);
        }


        public static void game_cmd_updball()
        {
            cmd.type = cmd_type.CMD_BALL_POSITION;

            System.Array.Copy(file.m_uv[0].m_p, cmd.ballpos.p, 3);

            enq_fn_game_proxy_enq.game_proxy_enq(cmd);

            cmd.type = cmd_type.CMD_BALL_BASIS;
            Vec3.v_cpy(cmd.ballbasis.e[0], file.m_uv[0].m_e[0]);
            Vec3.v_cpy(cmd.ballbasis.e[1], file.m_uv[0].m_e[1]);
            enq_fn_game_proxy_enq.game_proxy_enq(cmd);
        }


        public static void game_cmd_updview()
        {
            cmd.type = cmd_type.CMD_VIEW_POSITION;

            System.Array.Copy(view_p, cmd.viewpos.p, 3);

            enq_fn_game_proxy_enq.game_proxy_enq(cmd);

            cmd.type = cmd_type.CMD_VIEW_CENTER;

            System.Array.Copy(view_c, cmd.viewcenter.c, 3);

            enq_fn_game_proxy_enq.game_proxy_enq(cmd);

            cmd.type = cmd_type.CMD_VIEW_BASIS;
            Vec3.v_cpy(cmd.viewbasis.e[0], view_e[0]);
            Vec3.v_cpy(cmd.viewbasis.e[1], view_e[1]);
            enq_fn_game_proxy_enq.game_proxy_enq(cmd);
        }


        public static void game_cmd_ballradius()
        {
            cmd.type = cmd_type.CMD_BALL_RADIUS;
            cmd.ballradius.r = file.m_uv[0].m_r;
            enq_fn_game_proxy_enq.game_proxy_enq(cmd);
        }


        public static void game_cmd_init_balls()
        {
            cmd.type = cmd_type.CMD_CLEAR_BALLS;
            enq_fn_game_proxy_enq.game_proxy_enq(cmd);

            cmd.type = cmd_type.CMD_MAKE_BALL;
            enq_fn_game_proxy_enq.game_proxy_enq(cmd);

            game_cmd_updball();
            game_cmd_ballradius();
        }


        public static void game_cmd_init_items()
        {
            int i;

            cmd.type = cmd_type.CMD_CLEAR_ITEMS;
            enq_fn_game_proxy_enq.game_proxy_enq(cmd);

            for (i = 0; i < file.m_hc; i++)
            {
                cmd.type = cmd_type.CMD_MAKE_ITEM;

                Vec3.v_cpy(cmd.mkitem.p, file.m_hv[i].m_p);

                cmd.mkitem.t = file.m_hv[i].m_t;
                cmd.mkitem.n = file.m_hv[i].m_n;

                enq_fn_game_proxy_enq.game_proxy_enq(cmd);
            }
        }


        public static void game_cmd_pkitem(int hi)
        {
            cmd.type = cmd_type.CMD_PICK_ITEM;
            cmd.pkitem.hi = hi;
            enq_fn_game_proxy_enq.game_proxy_enq(cmd);
        }

        public static void game_cmd_jump(int e)
        {
            cmd.type = e != 0 ? cmd_type.CMD_JUMP_ENTER : cmd_type.CMD_JUMP_EXIT;
            enq_fn_game_proxy_enq.game_proxy_enq(cmd);
        }


        public static void game_cmd_tiltangles()
        {
            cmd.type = cmd_type.CMD_TILT_ANGLES;

            cmd.tiltangles.x = tilt.rx;
            cmd.tiltangles.z = tilt.rz;

            enq_fn_game_proxy_enq.game_proxy_enq(cmd);
        }


        public static void game_cmd_tiltaxes()
        {
            cmd.type = cmd_type.CMD_TILT_AXES;

            Vec3.v_cpy(cmd.tiltaxes.x, tilt.x);
            Vec3.v_cpy(cmd.tiltaxes.z, tilt.z);

            enq_fn_game_proxy_enq.game_proxy_enq(cmd);
        }


        public static void game_cmd_timer()
        {
            cmd.type = cmd_type.CMD_TIMER;
            cmd.timer.t = timer;
            enq_fn_game_proxy_enq.game_proxy_enq(cmd);
        }


        public static void game_cmd_coins()
        {
            cmd.type = cmd_type.CMD_COINS;
            cmd.coins.n = coins;
            enq_fn_game_proxy_enq.game_proxy_enq(cmd);
        }


        public static void game_cmd_status()
        {
            cmd.type = cmd_type.CMD_STATUS;
            cmd.status.t = (int)status;
            enq_fn_game_proxy_enq.game_proxy_enq(cmd);
        }


        public static void grow_init(s_file fp, int type)
        {
            if (got_orig == 0)
            {
                grow_orig = fp.m_uv[0].m_r;
                grow_goal = grow_orig;
                grow_strt = grow_orig;

                grow_state = 0;

                got_orig = 1;
            }

            if (type == Solid.ITEM_SHRINK)
            {
                switch (grow_state)
                {
                    case -1:
                        break;

                    case 0:
                        audio_play(Common.AUD_SHRINK, 1);
                        grow_goal = grow_orig * GROW_SMALL;
                        grow_state = -1;
                        grow = 1;
                        break;

                    case +1:
                        audio_play(Common.AUD_SHRINK, 1);
                        grow_goal = grow_orig;
                        grow_state = 0;
                        grow = 1;
                        break;
                }
            }
            else if (type == Solid.ITEM_GROW)
            {
                switch (grow_state)
                {
                    case -1:
                        audio_play(Common.AUD_GROW, 1);
                        grow_goal = grow_orig;
                        grow_state = 0;
                        grow = 1;
                        break;

                    case 0:
                        audio_play(Common.AUD_GROW, 1);
                        grow_goal = grow_orig * GROW_BIG;
                        grow_state = +1;
                        grow = 1;
                        break;

                    case +1:
                        break;
                }
            }

            if (grow != 0)
            {
                grow_t = 0;
                grow_strt = fp.m_uv[0].m_r;
            }
        }


        public static void grow_step(s_file fp, float dt)
        {
            float dr;

            if (grow == 0)
                return;

            /* Calculate new size based on how long since you touched the coin... */

            grow_t += dt;

            if (grow_t >= GROW_TIME)
            {
                grow = 0;
                grow_t = GROW_TIME;
            }

            dr = grow_strt + ((grow_goal - grow_strt) * (1.0f / (GROW_TIME / grow_t)));

            /* No sinking through the floor! Keeps ball's bottom constant. */

            fp.m_uv[0].m_p[1] += (dr - fp.m_uv[0].m_r);
            fp.m_uv[0].m_r = dr;

            game_cmd_ballradius();
        }



        public static void game_server_free()
        {
            if (server_state != 0)
            {
                file.sol_free();
                server_state = 0;
            }
        }


        static VIEW game_update_view_view_prev;

        public static void game_update_view(float dt)
        {
            float dc = view_dc * (jump_b != 0 ? 2.0f * System.Math.Abs(jump_dt - 0.5f) : 1.0f);
            float da = input_get_r() * dt * 90.0f;
            float k;

            float[] M = new float[16];
            float[] v = new float[3];
            float[] Y = new float[3] { 0.0f, 1.0f, 0.0f };

            /* Center the view about the ball. */

            Vec3.v_cpy(view_c, file.m_uv[0].m_p);

            view_v[0] = -file.m_uv[0].m_v[0];
            view_v[1] = 0.0f;
            view_v[2] = -file.m_uv[0].m_v[2];

            /* Restore usable vectors. */

            if (game_update_view_view_prev == VIEW.VIEW_TOPDOWN)
            {
                /* View basis. */

                Vec3.v_inv(view_e[2], view_e[1]);
                Vec3.v_cpy(view_e[1], Y);

                /* View position. */

                Vec3.v_scl(v, view_e[1], view_dp);
                Vec3.v_mad(v, v, view_e[2], view_dz);
                Vec3.v_add(view_p, v, file.m_uv[0].m_p);
            }

            game_update_view_view_prev = (VIEW)input_get_c();

            switch (input_get_c())
            {
                case (int)VIEW.VIEW_LAZY: /* Viewpoint chases the ball position. */

                    Vec3.v_sub(view_e[2], view_p, view_c);

                    break;

                case (int)VIEW.VIEW_MANUAL:  /* View vector is given by view angle. */
                case (int)VIEW.VIEW_TOPDOWN: /* Crude top-down view. */

                    view_e[2][0] = (float)System.Math.Sin(MathHelper.DegreesToRadians(view_a));
                    view_e[2][1] = 0;
                    view_e[2][2] = (float)System.Math.Cos(MathHelper.DegreesToRadians(view_a));

                    break;

                case (int)VIEW.VIEW_CHASE: /* View vector approaches the ball velocity vector. */

                    Vec3.v_sub(view_e[2], view_p, view_c);
                    Vec3.v_nrm(view_e[2], view_e[2]);
                    Vec3.v_mad(view_e[2], view_e[2], view_v, Vec3.v_dot(view_v, view_v) * dt / 4);

                    break;
            }

            /* Apply manual rotation. */

            Vec3.m_rot(M, Y, MathHelper.DegreesToRadians(da));
            Vec3.m_vxfm(view_e[2], M, view_e[2]);

            /* Orthonormalize the new view reference frame. */

            Vec3.v_crs(view_e[0], view_e[1], view_e[2]);
            Vec3.v_crs(view_e[2], view_e[0], view_e[1]);
            Vec3.v_nrm(view_e[0], view_e[0]);
            Vec3.v_nrm(view_e[2], view_e[2]);

            /* Compute the new view position. */

            k = 1.0f + Vec3.v_dot(view_e[2], view_v) / 10.0f;

            view_k = view_k + (k - view_k) * dt;

            if (view_k < 0.5)
                view_k = 0.5f;

            Vec3.v_scl(v, view_e[1], view_dp * view_k);
            Vec3.v_mad(v, v, view_e[2], view_dz * view_k);
            Vec3.v_add(view_p, v, file.m_uv[0].m_p);

            /* Compute the new view center. */

            Vec3.v_cpy(view_c, file.m_uv[0].m_p);
            Vec3.v_mad(view_c, view_c, view_e[1], dc);

            /* Note the current view angle. */

            view_a = MathHelper.RadiansToDegrees(System.Math.Atan2(view_e[2][0], view_e[2][2]));

            /* Override vectors for top-down view. */

            if (input_get_c() == (int)VIEW.VIEW_TOPDOWN)
            {
                Vec3.v_inv(view_e[1], view_e[2]);
                Vec3.v_cpy(view_e[2], Y);

                Vec3.v_cpy(view_c, file.m_uv[0].m_p);
                Vec3.v_mad(view_p, view_c, view_e[2], view_dz * 1.5f);
            }

            game_cmd_updview();
        }


        public static void game_update_time(float dt, int b)
        {
            if (goal_e != 0 && goal_k < 1.0f)
                goal_k += dt;

            /* The ticking clock. */

            if (b != 0 && timer_down != 0)
            {
                if (timer < 600)
                    timer -= dt;
                if (timer < 0)
                    timer = 0;
            }
            else if (b != 0)
            {
                timer += dt;
            }

            if (b != 0)
                game_cmd_timer();
        }


        public static int game_update_state(int bt)
        {
            s_file fp = file;
            s_goal zp;
            int hi;

            float[] p = new float[3];

            /* Test for an item. */

            if (bt != 0 && (hi = solid_phys.sol_item_test(fp, p, Item.ITEM_RADIUS)) != -1)
            {
                s_item hp = file.m_hv[hi];

                game_cmd_pkitem(hi);

                grow_init(fp, hp.m_t);

                if (hp.m_t == Solid.ITEM_COIN)
                {
                    coins += hp.m_n;
                    game_cmd_coins();
                }

                audio_play(Common.AUD_COIN, 1);

                /* Discard item. */

                hp.m_t = Solid.ITEM_NONE;
            }

            /* Test for a switch. */

            if (solid_phys.sol_swch_test(fp, 0) != 0)
                audio_play(Common.AUD_SWITCH, 1);

            /* Test for a jump. */

            if (jump_e == 1 && jump_b == 0 && solid_phys.sol_jump_test(fp, jump_p, 0) == 1)
            {
                jump_b = 1;
                jump_e = 0;
                jump_dt = 0;

                Vec3.v_sub(jump_w, jump_p, fp.m_uv[0].m_p);
                Vec3.v_add(jump_w, view_p, jump_w);

                audio_play(Common.AUD_JUMP, 1);

                game_cmd_jump(1);
            }
            if (jump_e == 0 && jump_b == 0 && solid_phys.sol_jump_test(fp, jump_p, 0) == 0)
            {
                jump_e = 1;
                game_cmd_jump(0);
            }

            /* Test for a goal. */

            if (bt != 0 && goal_e != 0 && (zp = solid_phys.sol_goal_test(fp, p, 0)) != null)
            {
                audio_play(Common.AUD_GOAL, 1.0f);
                return (int)GAME.GAME_GOAL;
            }

            /* Test for time-out. */

            if (bt != 0 && timer_down != 0 && timer <= 0)
            {
                audio_play(Common.AUD_TIME, 1.0f);
                return (int)GAME.GAME_TIME;
            }

            /* Test for fall-out. */

            if (bt != 0 && fp.m_uv[0].m_p[1] < fp.m_vv[0].m_p[1])
            {
                audio_play(Common.AUD_FALL, 1.0f);
                return (int)GAME.GAME_FALL;
            }

            return (int)GAME.GAME_NONE;
        }


        public static float[] game_server_step_gup = new float[] { 0.0f, +9.8f, 0.0f };
        public static float[] game_server_step_gdn = new float[] { 0.0f, -9.8f, 0.0f };



        public static int game_step(float[] g,//[3],
                             float dt, int bt)
        {
            if (server_state != 0)
            {
                s_file fp = file;

                float[] h = new float[3];

                /* Smooth jittery or discontinuous input. */

                tilt.rx += (input_get_x() - tilt.rx) * dt / Common.RESPONSE;
                tilt.rz += (input_get_z() - tilt.rz) * dt / Common.RESPONSE;

                game_common.game_tilt_axes(tilt, view_e);

                game_cmd_tiltaxes();
                game_cmd_tiltangles();

                grow_step(fp, dt);

                game_common.game_tilt_grav(h, g, tilt);

                if (jump_b != 0)
                {
                    jump_dt += dt;

                    /* Handle a jump. */

                    if (0.5f < jump_dt)
                    {
                        Vec3.v_cpy(fp.m_uv[0].m_p, jump_p);
                        Vec3.v_cpy(view_p, jump_w);
                    }

                    if (1.0f < jump_dt)
                        jump_b = 0;
                }
                else
                {
                    /* Run the sim. */

                    int temp = 0;
                    float b = solid_phys.sol_step(fp, h, dt, 0, ref temp, true);//NULL);

                    /* Mix the sound of a ball bounce. */

                    if (b > 0.5f)
                    {
                        float k = (b - 0.5f) * 2.0f;

                        if (got_orig != 0)
                        {
                            if (fp.m_uv[0].m_r > grow_orig)
                                audio_play(Common.AUD_BUMPL, k);
                            else if (fp.m_uv[0].m_r < grow_orig)
                                audio_play(Common.AUD_BUMPS, k);
                            else
                                audio_play(Common.AUD_BUMPM, k);
                        }
                        else
                            audio_play(Common.AUD_BUMPM, k);
                    }
                }

                game_cmd_updball();

                game_update_view(dt);
                game_update_time(dt, bt);

                return game_update_state(bt);
            }
            return (int)GAME.GAME_NONE;
        }


        public static void game_server_step(float dt)
        {
            switch (status)
            {
                case GAME.GAME_GOAL: game_step(game_server_step_gup, dt, 0); break;
                case GAME.GAME_FALL: game_step(game_server_step_gdn, dt, 0); break;

                case GAME.GAME_NONE:
                    if ((status = (GAME)game_step(game_server_step_gdn, dt, 1)) != GAME.GAME_NONE)
                        game_cmd_status();
                    break;
            }

            game_cmd_eou();
        }


        public static void game_set_goal()
        {
            audio_play(Common.AUD_SWITCH, 1.0f);
            goal_e = 1;

            game_cmd_goalopen();
        }


        public static void game_clr_goal()
        {
            goal_e = 0;
        }


        public static void game_set_pos(int x, int y)
        {
            input_set_x(input_get_x() + 40.0f * y / Config.config_get_d(Config.CONFIG_MOUSE_SENSE));
            input_set_z(input_get_z() + 40.0f * x / Config.config_get_d(Config.CONFIG_MOUSE_SENSE));
        }


        public static void game_set_cam(int c)
        {
            input_set_c(c);
        }


        public static void game_set_rot(float r)
        {
            input_set_r(r);
        }


        public static void game_set_fly(float k, s_file fp)
        {
            float[] x = new float[3] { 1, 0, 0 };
            float[] y = new float[3] { 0, 1, 0 };
            float[] z = new float[3] { 0, 0, 1 };
            float[] c0 = new float[3] { 0, 0, 0 };
            float[] p0 = new float[3] { 0, 0, 0 };
            float[] c1 = new float[3] { 0, 0, 0 };
            float[] p1 = new float[3] { 0, 0, 0 };
            float[] v = new float[3];

            if (fp == null)
                fp = file;

            view_init();

            z[0] = (float)System.Math.Sin(MathHelper.DegreesToRadians(view_a));
            z[2] = (float)System.Math.Cos(MathHelper.DegreesToRadians(view_a));

            Vec3.v_cpy(view_e[0], x);
            Vec3.v_cpy(view_e[1], y);
            Vec3.v_cpy(view_e[2], z);

            /* k = 0.0 view is at the ball. */

            if (fp.m_uc > 0)
            {
                Vec3.v_cpy(c0, fp.m_uv[0].m_p);
                Vec3.v_cpy(p0, fp.m_uv[0].m_p);
            }

            Vec3.v_mad(p0, p0, y, view_dp);
            Vec3.v_mad(p0, p0, z, view_dz);
            Vec3.v_mad(c0, c0, y, view_dc);

            /* k = +1.0 view is s_view 0 */

            if (k >= 0 && fp.m_wc > 0)
            {
                Vec3.v_cpy(p1, fp.m_wv[0].m_p);
                Vec3.v_cpy(c1, fp.m_wv[0].m_q);
            }

            /* k = -1.0 view is s_view 1 */

            if (k <= 0 && fp.m_wc > 1)
            {
                Vec3.v_cpy(p1, fp.m_wv[1].m_p);
                Vec3.v_cpy(c1, fp.m_wv[1].m_q);
            }

            /* Interpolate the views. */

            Vec3.v_sub(v, p1, p0);
            Vec3.v_mad(view_p, p0, v, k * k);

            Vec3.v_sub(v, c1, c0);
            Vec3.v_mad(view_c, c0, v, k * k);

            /* Orthonormalize the view basis. */

            Vec3.v_sub(view_e[2], view_p, view_c);
            Vec3.v_crs(view_e[0], view_e[1], view_e[2]);
            Vec3.v_crs(view_e[2], view_e[0], view_e[1]);
            Vec3.v_nrm(view_e[0], view_e[0]);
            Vec3.v_nrm(view_e[2], view_e[2]);

            game_cmd_updview();
        }


        public static int game_server_init(string file_name, int t, int e)
        {
            int version_x, version_y;

            int i;

            timer = (float)t / 100;
            timer_down = (t > 0) ? 1 : 0;
            coins = 0;
            status = GAME.GAME_NONE;

            if (server_state != 0)
                game_server_free();

            if (Solid.sol_load_only_file(file, file_name) == 0)
                return (server_state = 0);

            server_state = 1;

            version_x = 0;
            version_y = 0;

            for (i = 0; i < file.m_dc; i++)
            {
                string k = file.Get_av(file.m_dv[i].m_ai);
                string v = file.Get_av(file.m_dv[i].m_aj);

                if (string.Equals(k, "version"))
                {
                    string[] parts = v.Split(new char[] { ' ', '.' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts != null &&
                        parts.Length > 0)
                    {
                        version_x = int.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture);

                        if (parts.Length > 1)
                        {
                            version_y = int.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }
                }
            }

            input_init();

            game_common.game_tilt_init(tilt);

            /* Initialize jump and goal states. */

            jump_e = 1;
            jump_b = 0;

            goal_e = e != 0 ? 1 : 0;
            goal_k = e != 0 ? 1.0f : 0.0f;

            /* Initialize the view. */

            view_init();

            /* Initialize ball size tracking... */

            got_orig = 0;
            grow = 0;

            solid_phys.sol_cmd_enq_func(enq_fn_game_proxy_enq.Instance);//game_proxy_enq);

            /* Queue client commands. */

            game_cmd_map(file_name, version_x, version_y);
            game_cmd_ups();
            game_cmd_timer();

            if (goal_e != 0)
                game_cmd_goalopen();

            game_cmd_init_balls();
            game_cmd_init_items();

            return server_state;
        }
    }
}
