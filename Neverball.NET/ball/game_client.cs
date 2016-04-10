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

using OpenTK.Graphics.OpenGL;


namespace Neverball.NET
{
    class game_client
    {
        public static int game_compat_map;                    /* Client/server map compat flag     */


        public static int client_state = 0;

        public static int reflective;                /* Reflective geometry used?         */

        public static float timer = 0;          /* Clock time                        */

        public static int status = (int)GAME.GAME_NONE;          /* Outcome of the game               */

        public static int coins = 0;                /* Collected coins                   */
        public static int goal_e = 0;                /* Goal enabled flag                 */
        public static float goal_k = 0;                /* Goal animation                    */

        public static int jump_e = 1;                /* Jumping enabled flag              */
        public static int jump_b = 0;                /* Jump-in-progress flag             */
        public static float jump_dt;                   /* Jump duration                     */

        public static float fade_k = 0;              /* Fade in/out level                 */
        public static float fade_d = 0;              /* Fade in/out direction             */

        public static int ups;                         /* Updates per second                */
        public static int first_update;                /* First update flag                 */
        public static int curr_ball;                   /* Current ball index                */


        public static float[] view_c = new float[3];                 /* Current view center               */
        public static float[] view_p = new float[3];                 /* Current view position             */
        public static float[][] view_e = new float[3][]
        {
            new float[3],
            new float[3],
            new float[3]
        };              /* Current view reference frame      */


        public static game_tilt tilt = new game_tilt();           /* Floor rotation                    */


        public static s_file file;
        public static s_file back;


        static game_client()
        {
            file = new s_file();
            back = new s_file();
        }



        public static int curr_clock()
        {
            return (int)(timer * 100);
        }


        public static int curr_coins()
        {
            return coins;
        }


        public static int curr_status()
        {
            return status;
        }


        static float[][] game_draw_light_light_p = new float[2][]
        {
            new float[4]{ -8.0f, +32.0f, -8.0f, 0.0f },
            new float[4]{ +8.0f, +32.0f, +8.0f, 0.0f },
        };

        static float[][] game_draw_light_light_c = new float[2][]
        {
            new float[4]{ 1.0f, 0.8f, 0.8f, 1.0f },
            new float[4]{ 0.8f, 1.0f, 0.8f, 1.0f },
        };


        public static void game_draw_light()
        {
            //  Configure the lighting.

            GL.Enable(EnableCap.Light0);
            GL.Light(LightName.Light0, LightParameter.Position, game_draw_light_light_p[0]);
            GL.Light(LightName.Light0, LightParameter.Diffuse, game_draw_light_light_c[0]);
            GL.Light(LightName.Light0, LightParameter.Specular, game_draw_light_light_c[0]);

            GL.Enable(EnableCap.Light1);
            GL.Light(LightName.Light1, LightParameter.Position, game_draw_light_light_p[1]);
            GL.Light(LightName.Light1, LightParameter.Diffuse, game_draw_light_light_c[1]);
            GL.Light(LightName.Light1, LightParameter.Specular, game_draw_light_light_c[1]);
        }


        public static void game_clip_refl(int d)
        {
            /* Fudge to eliminate the floor from reflection. */

            double k = -0.00001;
            double[] e = new double[4];

            e[0] = 0;
            e[1] = 1;
            e[2] = 0;
            e[3] = k;

            GL.ClipPlane(ClipPlaneName.ClipPlane0, e);
        }


        public static void game_step_fade(float dt)
        {
            if ((fade_k < 1.0f && fade_d > 0.0f) ||
                (fade_k > 0.0f && fade_d < 0.0f))
                fade_k += fade_d * dt;

            if (fade_k < 0.0f)
            {
                fade_k = 0.0f;
                fade_d = 0.0f;
            }
            if (fade_k > 1.0f)
            {
                fade_k = 1.0f;
                fade_d = 0.0f;
            }
        }


        public static void game_fade(float d)
        {
            fade_d = d;
        }


        public static void game_client_free()
        {
            if (client_state != 0)
            {
                game_proxy.game_proxy_clr();
                file.sol_free_gl();
                back.sol_free_gl();
                Back.back_free();
            }

            client_state = 0;
        }


        public static void game_draw_tilt(int d)
        {
            float[] ball_p = file.m_uv[0].m_p;

            /* Rotate the environment about the position of the ball. */

            Video.MODELVIEW_Translate(+ball_p[0], +ball_p[1] * d, +ball_p[2]);
            Video.MODELVIEW_Rotate(-tilt.rz * d, tilt.z[0], tilt.z[1], tilt.z[2]);
            Video.MODELVIEW_Rotate(-tilt.rx * d, tilt.x[0], tilt.x[1], tilt.x[2]);
            Video.MODELVIEW_Translate(-ball_p[0], -ball_p[1] * d, -ball_p[2]);
        }


        public static void game_refl_all()
        {
            Video.MODELVIEW_PushMatrix();
            {
                game_draw_tilt(1);

                /* Draw the floor. */

                file.sol_refl();
            }
            Video.MODELVIEW_PopMatrix();
        }


        public static s_file game_client_file()
        {
            return file;
        }


        public static void game_draw_balls(s_file fp,
                                    float[] bill_M,
                                    float t)
        {
            float[] c = new float[4] { 1.0f, 1.0f, 1.0f, 1.0f };

            float[] ball_M = new float[16];

            Vec3.m_basis(ball_M, fp.m_uv[0].m_e[0], fp.m_uv[0].m_e[1], fp.m_uv[0].m_e[2]);

            GL.PushAttrib(AttribMask.LightingBit);
            Video.MODELVIEW_PushMatrix();
            {
                Video.MODELVIEW_Translate(fp.m_uv[0].m_p[0],
                             fp.m_uv[0].m_p[1] + Ball.BALL_FUDGE,
                             fp.m_uv[0].m_p[2]);
                Video.MODELVIEW_Scale(fp.m_uv[0].m_r,
                         fp.m_uv[0].m_r,
                         fp.m_uv[0].m_r);

                Video.Color(c);
                Ball.ball_draw(ball_M, bill_M, t);
            }
            Video.MODELVIEW_PopMatrix();
            GL.PopAttrib();
        }


        public static void game_draw_items(s_file fp, float t)
        {
            float r = 360 * t;
            int hi;

            GL.PushAttrib(AttribMask.LightingBit);
            {
                Item.item_push(Solid.ITEM_COIN);
                {
                    for (hi = 0; hi < fp.m_hc; hi++)
                    {
                        if (fp.m_hv[hi].m_t == Solid.ITEM_COIN)
                        {
                            if (fp.m_hv[hi].m_n > 0)
                            {
                                Video.MODELVIEW_PushMatrix();
                                {
                                    Video.MODELVIEW_Translate(fp.m_hv[hi].m_p[0],
                                                 fp.m_hv[hi].m_p[1],
                                                 fp.m_hv[hi].m_p[2]);
                                    Video.MODELVIEW_Rotate(r, 0.0f, 1.0f, 0.0f);
                                    fp.m_hv[hi].item_draw(r);
                                }
                                Video.MODELVIEW_PopMatrix();
                            }
                        }
                    }
                }
                Item.item_pull();

                Item.item_push(Solid.ITEM_SHRINK);
                {
                    for (hi = 0; hi < fp.m_hc; hi++)
                    {
                        if (fp.m_hv[hi].m_t == Solid.ITEM_SHRINK)
                        {
                            Video.MODELVIEW_PushMatrix();
                            {
                                Video.MODELVIEW_Translate(fp.m_hv[hi].m_p[0],
                                             fp.m_hv[hi].m_p[1],
                                             fp.m_hv[hi].m_p[2]);
                                Video.MODELVIEW_Rotate(r, 0.0f, 1.0f, 0.0f);
                                fp.m_hv[hi].item_draw(r);
                            }
                            Video.MODELVIEW_PopMatrix();
                        }
                    }
                }
                Item.item_pull();

                Item.item_push(Solid.ITEM_GROW);
                {
                    for (hi = 0; hi < fp.m_hc; hi++)
                    {
                        if (fp.m_hv[hi].m_t == Solid.ITEM_GROW)
                        {
                            Video.MODELVIEW_PushMatrix();
                            {
                                Video.MODELVIEW_Translate(fp.m_hv[hi].m_p[0],
                                             fp.m_hv[hi].m_p[1],
                                             fp.m_hv[hi].m_p[2]);
                                Video.MODELVIEW_Rotate(r, 0.0f, 1.0f, 0.0f);
                                fp.m_hv[hi].item_draw(r);
                            }
                            Video.MODELVIEW_PopMatrix();
                        }
                    }
                }
                Item.item_pull();
            }
            GL.PopAttrib();
        }



        public static void game_draw_back(int pose, int d, float t)
        {
            if (pose == 2)
                return;

            Video.MODELVIEW_PushMatrix();
            {
                if (d < 0)
                {
                    Video.MODELVIEW_Rotate(tilt.rz * 2, tilt.z[0], tilt.z[1], tilt.z[2]);
                    Video.MODELVIEW_Rotate(tilt.rx * 2, tilt.x[0], tilt.x[1], tilt.x[2]);
                }

                Video.MODELVIEW_Translate(view_p[0], view_p[1] * d, view_p[2]);

                back.DrawBackground(t);
            }
            Video.MODELVIEW_PopMatrix();
        }


        public static void game_clip_ball(int d, float[] p)
        {
            double r;
            double[] c = new double[3];
            double[] pz = new double[4];
            double[] nz = new double[4];
            double[] ny = new double[4];

            /* Compute the plane giving the front of the ball, as seen from view_p. */

            c[0] = p[0];
            c[1] = p[1] * d;
            c[2] = p[2];

            pz[0] = view_p[0] - c[0];
            pz[1] = view_p[1] - c[1];
            pz[2] = view_p[2] - c[2];

            r = Math.Sqrt(pz[0] * pz[0] + pz[1] * pz[1] + pz[2] * pz[2]);

            pz[0] /= r;
            pz[1] /= r;
            pz[2] /= r;
            pz[3] = -(pz[0] * c[0] +
                      pz[1] * c[1] +
                      pz[2] * c[2]);

            /* Find the plane giving the back of the ball, as seen from view_p. */

            nz[0] = -pz[0];
            nz[1] = -pz[1];
            nz[2] = -pz[2];
            nz[3] = -pz[3];

            /* Compute the plane giving the bottom of the ball. */

            ny[0] = 0.0;
            ny[1] = -1.0;
            ny[2] = 0.0;
            ny[3] = -(ny[0] * c[0] +
                      ny[1] * c[1] +
                      ny[2] * c[2]);

            /* Reflect these planes as necessary, and store them in the GL state. */

            pz[1] *= d;
            nz[1] *= d;
            ny[1] *= d;

            GL.ClipPlane(ClipPlaneName.ClipPlane1, nz);
            GL.ClipPlane(ClipPlaneName.ClipPlane2, pz);
            GL.ClipPlane(ClipPlaneName.ClipPlane3, ny);
        }


        public static void game_look(float phi, float theta)
        {
            view_c[0] = (float)(view_p[0] + System.Math.Sin(MathHelper.DegreesToRadians(theta)) * System.Math.Cos(MathHelper.DegreesToRadians(phi)));
            view_c[1] = (float)(view_p[1] + System.Math.Sin(MathHelper.DegreesToRadians(phi)));
            view_c[2] = (float)(view_p[2] - System.Math.Cos(MathHelper.DegreesToRadians(theta)) * System.Math.Cos(MathHelper.DegreesToRadians(phi)));
        }


        public static float[] game_run_cmd_gup = new float[] { 0.0f, +9.8f, 0.0f };
        public static float[] game_run_cmd_gdn = new float[] { 0.0f, -9.8f, 0.0f };


        public static int version_x = 0;
        public static int version_y = 0;


        public static int game_client_init(string file_name)
        {
            string back_name = null;
            string grad_name = null;
            int i;

            coins = 0;
            status = (int)GAME.GAME_NONE;

            if (client_state != 0)
                game_client_free();

            if (file.sol_load_gl(file_name, Config.config_get_d(Config.CONFIG_TEXTURES), Config.config_get_d(Config.CONFIG_SHADOW)) == 0)
                return (client_state = 0);

            reflective = file.sol_reflective();

            client_state = 1;

            game_common.game_tilt_init(tilt);

            //  Initialize jump and goal states.

            jump_e = 1;
            jump_b = 0;

            goal_e = 0;
            goal_k = 0.0f;

            //  Initialise the level, background, particles, fade, and view.

            fade_k = 1.0f;
            fade_d = -2.0f;


            version_x = 0;
            version_y = 0;

            for (i = 0; i < file.m_dc; i++)
            {
                string k = file.Get_av(file.m_dv[i].m_ai);
                string v = file.Get_av(file.m_dv[i].m_aj);

                if (string.Equals(k, "back"))
                    back_name = v;

                if (string.Equals(k, "grad"))
                    grad_name = v;

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


            //  If the client map's version is 1, assume the map is compatible
            //  with the server.  This ensures that 1.5.0 replays don't trigger
            //  bogus map compatibility warnings.  (Post-1.5.0 replays will
            //  have cmd_type.CMD_MAP override this.)


            game_compat_map = version_x == 1 ? 1 : 0;

            Part.part_reset(Config.GOAL_HEIGHT, Config.JUMP_HEIGHT);

            ups = 0;
            first_update = 1;

            Back.back_init(grad_name, Config.config_get_d(Config.CONFIG_GEOMETRY));
            
            back.sol_load_gl(back_name, Config.config_get_d(Config.CONFIG_TEXTURES), 0);

            return client_state;
        }


        public static void game_draw_jumps(s_file fp, float[] M, float t)
        {
            int ji;

            GL.Enable(EnableCap.Texture2D);
            {
                for (ji = 0; ji < fp.m_jc; ji++)
                {
                    Video.MODELVIEW_PushMatrix();
                    {
                        Video.MODELVIEW_Translate(fp.m_jv[ji].m_p[0],
                                     fp.m_jv[ji].m_p[1],
                                     fp.m_jv[ji].m_p[2]);

                        Part.part_draw_jump(M, fp.m_jv[ji].m_r, 1.0f, t);
                    }
                    Video.MODELVIEW_PopMatrix();
                }
            }
            GL.Disable(EnableCap.Texture2D);

            for (ji = 0; ji < fp.m_jc; ji++)
            {
                fp.m_jv[ji].jump_draw(jump_e == 0 ? 1 : 0);
            }
        }


        public static void game_draw_fore(int pose, float[] M, int d, float t)
        {
            float[] ball_p = file.m_uv[0].m_p;
            float ball_r = file.m_uv[0].m_r;

            Video.MODELVIEW_PushMatrix();
            {
                //  Rotate the environment about the position of the ball.

                game_draw_tilt(d);

                //  Compute clipping planes for reflection and ball facing.

                game_clip_refl(d);
                game_clip_ball(d, ball_p);

                if (d < 0)
                {
                    GL.Enable(EnableCap.ClipPlane0);
                }

                switch (pose)
                {
                    case 1:
                        file.sol_draw(0, 1);
                        break;

                    case 0:
                        //  Draw the coins.

                        game_draw_items(file, t);

                        //  Draw the floor.

                        file.sol_draw(0, 1);

                        //  Fall through.
                        goto case 2;

                    case 2:

                        //  Draw the ball shadow.

                        if (d > 0 &&
                            Config.config_get_d(Config.CONFIG_SHADOW) != 0)
                        {
                            file.DrawBallShadow(0);
                        }

                        //  Draw the ball.

                        game_draw_balls(file, M, t);

                        break;
                }

                //  Draw the particles and light columns.

                GL.Enable(EnableCap.ColorMaterial);
                GL.Disable(EnableCap.Lighting);
                GL.DepthMask(false);
                {
                    Video.Color(1.0f, 1.0f, 1.0f);

                    file.sol_bill(M, t);
                    Part.part_draw_coin(M, t);

                    GL.Disable(EnableCap.Texture2D);
                    {
                        file.game_draw_goals(M, t);
                        game_draw_jumps(file, M, t);
                        file.game_draw_swchs();
                    }
                    GL.Enable(EnableCap.Texture2D);

                    Video.Color(1.0f, 1.0f, 1.0f);
                }
                GL.DepthMask(true);
                GL.Enable(EnableCap.Lighting);
                GL.Disable(EnableCap.ColorMaterial);

                if (d < 0)
                {
                    GL.Disable(EnableCap.ClipPlane0);
                }
            }
            Video.MODELVIEW_PopMatrix();
        }


        public static void ball_game_draw(int pose, float t)
        {
            float fov = (float)Config.config_get_d(Config.CONFIG_VIEW_FOV);

            if (jump_b != 0)
                fov *= (float)(2 * System.Math.Abs(jump_dt - 0.5));

            if (client_state != 0)
            {
                Video.video_push_persp(fov);
                Video.MODELVIEW_PushMatrix();
                {
                    float[] T = new float[16];
                    float[] U = new float[16];
                    float[] M = new float[16];
                    float[] v = new float[3];

                    //  Compute direct and reflected view bases.

                    v[0] = +view_p[0];
                    v[1] = -view_p[1];
                    v[2] = +view_p[2];

                    Vec3.m_view(T, view_c, view_p, view_e[1]);
                    Vec3.m_view(U, view_c, v, view_e[1]);

                    Vec3.m_xps(M, T);

                    //  Apply the current view.

                    Vec3.v_sub(v, view_c, view_p);

                    Video.MODELVIEW_Translate(0, 0, -Vec3.v_len(v));
                    Video.MODELVIEW_MultMatrix(M);
                    Video.MODELVIEW_Translate(-view_c[0], -view_c[1], -view_c[2]);

                    if (reflective != 0 &&
                        Config.config_get_d(Config.CONFIG_REFLECTION) != 0)
                    {
                        GL.Enable(EnableCap.StencilTest);
                        {
                            //  Draw the mirrors only into the stencil buffer.

                            GL.StencilFunc(StencilFunction.Always, 1, 0xFFFFFFFF);
                            GL.StencilOp(StencilOp.Replace, StencilOp.Replace, StencilOp.Replace);
                            GL.ColorMask(false, false, false, false);
                            GL.DepthMask(false);

                            game_refl_all();

                            GL.DepthMask(true);
                            GL.ColorMask(true, true, true, true);
                            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
                            GL.StencilFunc(StencilFunction.Equal, 1, 0xFFFFFFFF);

                            //  Draw the scene reflected into color and depth buffers.

                            GL.FrontFace(FrontFaceDirection.Cw);
                            Video.MODELVIEW_PushMatrix();
                            {
                                Video.MODELVIEW_Scale(+1.0f, -1.0f, +1.0f);

                                game_draw_light();
                                game_draw_back(pose, -1, t);
                                game_draw_fore(pose, U, -1, t);
                            }
                            Video.MODELVIEW_PopMatrix();
                            GL.FrontFace(FrontFaceDirection.Ccw);
                        }
                        GL.Disable(EnableCap.StencilTest);
                    }

                    //  Draw the scene normally.

                    game_draw_light();

                    if (reflective != 0)
                    {
                        if (Config.config_get_d(Config.CONFIG_REFLECTION) != 0)
                        {
                            //  Draw background while preserving reflections.

                            GL.Enable(EnableCap.StencilTest);
                            {
                                GL.StencilFunc(StencilFunction.Notequal, 1, 0xFFFFFFFF);
                                game_draw_back(pose, +1, t);
                            }
                            GL.Disable(EnableCap.StencilTest);

                            //  Draw mirrors.

                            game_refl_all();
                        }
                        else
                        {
                            //  Draw background.

                            game_draw_back(pose, +1, t);


                            //  Draw mirrors, first fully opaque with a custom
                            //  material color, then blending normally with the
                            //  opaque surfaces using their original material
                            //  properties.  (Keeps background from showing
                            //  through.)


                            GL.Enable(EnableCap.ColorMaterial);
                            {
                                Video.Color(0.0f, 0.0f, 0.05f, 1.0f);
                                game_refl_all();
                                Video.Color(1.0f, 1.0f, 1.0f, 1.0f);
                            }
                            GL.Disable(EnableCap.ColorMaterial);

                            game_refl_all();
                        }
                    }
                    else
                    {
                        game_draw_back(pose, +1, t);
                        game_refl_all();
                    }

                    game_draw_fore(pose, T, +1, t);
                }
                Video.MODELVIEW_PopMatrix();
                Video.video_pop_matrix();

                //  Draw the fade overlay.

                Geom.fade_draw(fade_k);
            }
        }


        static int game_run_cmd_got_tilt_axes;

        public static void game_run_cmd(Command cmd)
        {
            /*
             * Neverball <= 1.5.1 does not send explicit tilt axes, rotation
             * happens directly around view vectors.  So for compatibility if
             * at the time of receiving tilt angles we have not yet received
             * the tilt axes, we use the view vectors.
             */

            float[] f = new float[3];

            if (client_state != 0)
            {
                s_ball[] up;

                float dt;
                int i;

                switch (cmd.type)
                {
                    case cmd_type.CMD_END_OF_UPDATE:

                        game_run_cmd_got_tilt_axes = 0;

                        if (first_update != 0)
                        {
                            first_update = 0;
                            break;
                        }

                        /* Compute gravity for particle effects. */

                        if (status == (int)GAME.GAME_GOAL)
                            game_common.game_tilt_grav(f, game_run_cmd_gup, tilt);
                        else
                            game_common.game_tilt_grav(f, game_run_cmd_gdn, tilt);

                        /* Step particle, goal and jump effects. */

                        if (ups > 0)
                        {
                            dt = 1.0f / (float)ups;

                            if (goal_e != 0 && goal_k < 1.0f)
                                goal_k += dt;

                            if (jump_b != 0)
                            {
                                jump_dt += dt;

                                if (1.0f < jump_dt)
                                    jump_b = 0;
                            }

                            Part.part_step(f, dt);
                        }

                        break;

                    case cmd_type.CMD_MAKE_BALL:
                        /* Allocate a new ball and mark it as the current ball. */

                        up = new s_ball[file.m_uc + 1];
                        {
                            for (i = 0; i < file.m_uc; i++)
                            {
                                up[i] = file.m_uv[i];
                            }
                            up[file.m_uc] = new s_ball();
                        }
                        {
                            file.m_uv = up;
                            curr_ball = file.m_uc;
                            file.m_uc++;
                        }
                        break;

                    case cmd_type.CMD_MAKE_ITEM:
                        /* Allocate and initialise a new item. */
                        {
                            s_item[] hp = new s_item[file.m_hc + 1];
                            {
                                for (i = 0; i < file.m_hc; i++)
                                {
                                    hp[i] = file.m_hv[i];
                                }
                            }
                            {
                                s_item h = new s_item();

                                Vec3.v_cpy(h.m_p, cmd.mkitem.p);

                                h.m_t = cmd.mkitem.t;
                                h.m_n = cmd.mkitem.n;

                                file.m_hv = hp;
                                file.m_hv[file.m_hc] = h;
                                file.m_hc++;
                            }
                        }

                        break;

                    case cmd_type.CMD_PICK_ITEM:
                        /* Set up particle effects and discard the item. */
                        {
                            s_item hp = file.m_hv[cmd.pkitem.hi];

                            Item.item_color(hp, f);
                            Part.part_burst(hp.m_p, f);

                            hp.m_t = Solid.ITEM_NONE;
                        }

                        break;

                    case cmd_type.CMD_TILT_ANGLES:
                        if (game_run_cmd_got_tilt_axes == 0)
                            game_common.game_tilt_axes(tilt, view_e);

                        tilt.rx = cmd.tiltangles.x;
                        tilt.rz = cmd.tiltangles.z;
                        break;

                    case cmd_type.CMD_SOUND:
                        /* Play the sound, then free its file name. */

                        if (cmd.sound.n != null)
                        {
                            Audio.audio_play(cmd.sound.n, cmd.sound.a);

                            /*
                             * FIXME Command memory management should be done
                             * elsewhere and done properly.
                             */

                            cmd.sound.n = null;
                        }
                        break;

                    case cmd_type.CMD_TIMER:
                        timer = cmd.timer.t;
                        break;

                    case cmd_type.CMD_STATUS:
                        status = cmd.status.t;
                        break;

                    case cmd_type.CMD_COINS:
                        coins = cmd.coins.n;
                        break;

                    case cmd_type.CMD_JUMP_ENTER:
                        jump_b = 1;
                        jump_e = 0;
                        jump_dt = 0.0f;
                        break;

                    case cmd_type.CMD_JUMP_EXIT:
                        jump_e = 1;
                        break;

                    case cmd_type.CMD_BODY_PATH:
                        file.m_bv[cmd.bodypath.bi].m_pi = cmd.bodypath.pi;
                        break;

                    case cmd_type.CMD_BODY_TIME:
                        file.m_bv[cmd.bodytime.bi].m_t = cmd.bodytime.t;
                        break;

                    case cmd_type.CMD_GOAL_OPEN:
                        /*
                         * Enable the goal and make sure it's fully visible if
                         * this is the first update.
                         */

                        if (goal_e == 0)
                        {
                            goal_e = 1;
                            goal_k = first_update != 0 ? 1.0f : 0.0f;
                        }
                        break;

                    case cmd_type.CMD_SWCH_ENTER:
                        file.m_xv[cmd.swchenter.xi].m_e = 1;
                        break;

                    case cmd_type.CMD_SWCH_TOGGLE:
                        file.m_xv[cmd.swchtoggle.xi].m_f =
                            file.m_xv[cmd.swchtoggle.xi].m_f == 0 ? 1 : 0;
                        break;

                    case cmd_type.CMD_SWCH_EXIT:
                        file.m_xv[cmd.swchexit.xi].m_e = 0;
                        break;

                    case cmd_type.CMD_UPDATES_PER_SECOND:
                        ups = cmd.ups.n;
                        break;

                    case cmd_type.CMD_BALL_RADIUS:
                        file.m_uv[curr_ball].m_r = cmd.ballradius.r;
                        break;

                    case cmd_type.CMD_CLEAR_ITEMS:
                        if (file.m_hv != null)
                        {
                            file.m_hv = null;
                        }
                        file.m_hc = 0;
                        break;

                    case cmd_type.CMD_CLEAR_BALLS:
                        if (file.m_uv != null)
                        {
                            file.m_uv = null;
                        }
                        file.m_uc = 0;
                        break;

                    case cmd_type.CMD_BALL_POSITION:
                        Vec3.v_cpy(file.m_uv[curr_ball].m_p, cmd.ballpos.p);
                        break;

                    case cmd_type.CMD_BALL_BASIS:
                        Vec3.v_cpy(file.m_uv[curr_ball].m_e[0], cmd.ballbasis.e[0]);
                        Vec3.v_cpy(file.m_uv[curr_ball].m_e[1], cmd.ballbasis.e[1]);

                        Vec3.v_crs(file.m_uv[curr_ball].m_e[2],
                              file.m_uv[curr_ball].m_e[0],
                              file.m_uv[curr_ball].m_e[1]);
                        break;

                    case cmd_type.CMD_VIEW_POSITION:
                        Vec3.v_cpy(view_p, cmd.viewpos.p);
                        break;

                    case cmd_type.CMD_VIEW_CENTER:
                        Vec3.v_cpy(view_c, cmd.viewcenter.c);
                        break;

                    case cmd_type.CMD_VIEW_BASIS:
                        Vec3.v_cpy(view_e[0], cmd.viewbasis.e[0]);
                        Vec3.v_cpy(view_e[1], cmd.viewbasis.e[1]);

                        Vec3.v_crs(view_e[2], view_e[0], view_e[1]);

                        break;

                    case cmd_type.CMD_CURRENT_BALL:
                        curr_ball = cmd.currball.ui;
                        break;

                    case cmd_type.CMD_PATH_FLAG:
                        file.m_pv[cmd.pathflag.pi].m_f = cmd.pathflag.f;
                        break;

                    case cmd_type.CMD_STEP_SIMULATION:
                        /*
                         * Simulate body motion.
                         *
                         * This is done on the client side due to replay file size
                         * concerns and isn't done as part of cmd_type.CMD_END_OF_UPDATE to
                         * match the server state as closely as possible.  Body
                         * time is still synchronised with the server on a
                         * semi-regular basis and path indices are handled through
                         * cmd_type.CMD_BODY_PATH, thus this code doesn't need to be as
                         * sophisticated as sol_body_step.
                         */

                        dt = cmd.stepsim.dt;

                        for (i = 0; i < file.m_bc; i++)
                        {
                            s_body bp = file.m_bv[i];// + i;

                            if (bp.m_pi >= 0)
                            {
                                s_path pp = file.m_pv[bp.m_pi];

                                if (pp.m_f != 0)
                                {
                                    bp.m_t += dt;
                                }
                            }
                        }
                        break;

                    case cmd_type.CMD_MAP:

                        /*
                         * Note if the loaded map matches the server's
                         * expectations. (No, this doesn't actually load a map,
                         * yet.  Something else somewhere else does.)
                         */

                        cmd.map.name = null;

                        game_compat_map = version_x == cmd.map.version.x ? 1 : 0;
                        break;

                    case cmd_type.CMD_TILT_AXES:
                        game_run_cmd_got_tilt_axes = 1;
                        Vec3.v_cpy(tilt.x, cmd.tiltaxes.x);
                        Vec3.v_cpy(tilt.z, cmd.tiltaxes.z);
                        break;

                    case cmd_type.CMD_NONE:
                    case cmd_type.CMD_MAX:
                        break;
                }
            }
        }


        public static void game_client_step()
        {
            Command cmdp;

            while ((cmdp = game_proxy.game_proxy_deq()) != null)
            {
                /*
                 * Note: cmd_put is called first here because game_run_cmd
                 * frees some command struct members.
                 */

                game_run_cmd(cmdp);

                cmdp = null;
            }
        }

    }
}
