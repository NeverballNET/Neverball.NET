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
    class Ball
    {
        public const float BALL_FUDGE = 0.001f;


        public const int F_DRAWBACK = 2;
        public const int F_DEPTHMASK = 8;
        public const int F_DEPTHTEST = 16;


        public static int has_solid = 0;

        public static int solid_flags;

        public static float solid_alpha;

        public static s_file solid;
        public static s_file outer;

        static Ball()
        {
            solid = new s_file();
            outer = new s_file();
        }


        public static void ball_free()
        {
            if (has_solid != 0)
            {
                solid.sol_free_gl();

                has_solid = 0;
            }
        }


        public static int SET(int B, int v, int b)
        {
            return ((v != 0) ? ((B) | (b)) : ((B) & ~(b)));
        }


        public static int ball_opts(s_file fp, ref float alpha)
        {
            int flags = F_DEPTHTEST;
            int di;

            //	MY
            for (di = 0; di < fp.m_dc; ++di)
            {
                string k = fp.Get_av(fp.m_dv[di].m_ai);
                string v = fp.Get_av(fp.m_dv[di].m_aj);

                if (string.Equals(k, "drawback"))
                    flags = SET(flags, int.Parse(v, System.Globalization.CultureInfo.InvariantCulture), F_DRAWBACK);
                if (string.Equals(k, "depthmask"))
                    flags = SET(flags, int.Parse(v, System.Globalization.CultureInfo.InvariantCulture), F_DEPTHMASK);
                if (string.Equals(k, "depthtest"))
                    flags = SET(flags, int.Parse(v, System.Globalization.CultureInfo.InvariantCulture), F_DEPTHTEST);
                if (string.Equals(k, "alphatest"))
                {
                    if (v != null)
                    {
                        string[] parts = v.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts != null && parts.Length > 0)
                        {
                            alpha = (float)double.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }
                }
            }

            return flags;
        }


        public static void ball_init()
        {
            int T = Config.config_get_d(Config.CONFIG_TEXTURES);

            string solid_file = Config.config_get_s(Config.CONFIG_BALL_FILE) + "-solid.sol";

            solid_flags = 0;

            solid_alpha = 1.0f;

            if ((has_solid = solid.sol_load_gl(solid_file, T, 0)) != 0)
                solid_flags = ball_opts(solid, ref solid_alpha);
        }


        public static void ball_draw_solid(float[] ball_M,
                                    float[] ball_bill_M, float t)
        {
            if (has_solid != 0)
            {
                int mask = (solid_flags & F_DEPTHMASK);
                int test = (solid_flags & F_DEPTHTEST);

                if (solid_alpha < 1.0f)
                {
                    GL.Enable(EnableCap.AlphaTest);
                    GL.AlphaFunc(AlphaFunction.Gequal, solid_alpha);
                }

                Video.MODELVIEW_PushMatrix();
                {
                    /* Apply the ball rotation. */

                    Video.MODELVIEW_MultMatrix(ball_M);

                    /* Draw the solid billboard geometry. */

                    if (solid.m_rc != 0)
                    {
                        if (test == 0) GL.Disable(EnableCap.DepthTest);
                        if (mask == 0) GL.DepthMask(false);
                        GL.Disable(EnableCap.Lighting);
                        {
                            solid.sol_bill(ball_bill_M, t);
                        }
                        GL.Enable(EnableCap.Lighting);
                        if (mask == 0) GL.DepthMask(true);
                        if (test == 0) GL.Enable(EnableCap.DepthTest);
                    }

                    /* Draw the solid opaque and transparent geometry. */

                    solid.sol_draw(mask, test);
                }
                Video.MODELVIEW_PopMatrix();

                if (solid_alpha < 1.0f)
                {
                    GL.Disable(EnableCap.AlphaTest);
                }
            }
        }


        public static void ball_pass_solid(
            float[] ball_M,
            float[] ball_bill_M,
            float t)
        {
            /* Sort the solid ball with the inner ball using face culling. */

            if ((solid_flags & F_DRAWBACK) != 0)
            {
                GL.CullFace(CullFaceMode.Front);
                ball_draw_solid(ball_M, ball_bill_M, t);
                GL.CullFace(CullFaceMode.Back);

                ball_draw_solid(ball_M, ball_bill_M, t);
            }

            /* Draw the solid ball after the inner ball. */

            else
            {
                ball_draw_solid(ball_M, ball_bill_M, t);
            }
        }


        public static void ball_pass_outer(float[] ball_M,
                                    float[] ball_bill_M,
                                    float t)
        {
            ball_pass_solid(ball_M, ball_bill_M, t);
        }


        static float[] ball_T = new float[16];
        static float[] ball_bill_M = new float[16];

        public static void ball_draw(
            float[] ball_M,
            float[] bill_M,
            float t)
        {
            /* Compute transforms for ball. */

            Vec3.m_xps(ball_T, ball_M);

            Vec3.m_mult(ball_bill_M, ball_T, bill_M);

            /* Go to GREAT pains to ensure all layers are drawn back-to-front. */

            ball_pass_outer(ball_M, ball_bill_M, t);
        }



        //  BALL SHADOW

        /*
         * A note about lighting and shadow: technically speaking, it's wrong.
         * The  light  position  and   shadow  projection  behave  as  if  the
         * light-source rotates with the  floor.  However, the skybox does not
         * rotate, thus the light should also remain stationary.
         *
         * The  correct behavior  would eliminate  a significant  3D  cue: the
         * shadow of  the ball indicates  the ball's position relative  to the
         * floor even  when the ball is  in the air.  This  was the motivating
         * idea  behind the  shadow  in  the first  place,  so correct  shadow
         * projection would only magnify the problem.
         */

        static int shad_text;
        const string IMG_SHAD = "png/shadow.png";

        public static void shad_init()
        {
            shad_text = Image.make_image_from_file(IMG_SHAD);

            if (Config.config_get_d(Config.CONFIG_SHADOW) == 2)
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            }
        }

        public static void shad_free()
        {
            if (GL.IsTexture(shad_text))
            {
                GL.DeleteTextures(1, ref shad_text);
                shad_text = 0;
            }
        }

        public static void shad_draw_set(float[] p, float r)
        {
            float k = 0.25f / r;

            GL.BindTexture(TextureTarget.Texture2D, shad_text);

            Video.TEXTURE_LoadIdentity();
            Video.TEXTURE_Translate(0.5f - k * p[0],
                         0.5f - k * p[2], 0);
            Video.TEXTURE_Scale(k, k, 1.0f);

            if (Config.config_get_d(Config.CONFIG_SHADOW) != 3)
            {
                GL.Enable(EnableCap.ClipPlane3);
            }
        }

        public static void shad_draw_clr()
        {
            Video.TEXTURE_LoadIdentity();

            if (Config.config_get_d(Config.CONFIG_SHADOW) != 3)
            {
                GL.Disable(EnableCap.ClipPlane3);
            }
        }
    }
}
