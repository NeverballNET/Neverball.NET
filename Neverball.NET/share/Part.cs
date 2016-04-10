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
    class Part
    {
        public const string IMG_PART_STAR = "png/part.png";
        public const string IMG_PART_SQUIGGLE = "png/squiggle.png";

        public const int PART_MAX_COIN = 256;
        public const int PART_MAX_GOAL = 64;
        public const int PART_MAX_JUMP = 64;

        public const float PART_SIZE = 0.1f;


        public class part
        {
            public float t;
            public float a;
            public float w;
            public float[] c = new float[3];
            public float[] p = new float[3];
            public float[] v = new float[3];


            public const string PartNamePrefix = Config.BallResourcePrefix + "Part";
            public string m_Name;


            public part(string postfixName)
            {
                m_Name = PartNamePrefix + postfixName;
            }


            public void Zero()
            {
                t = 0;
                a = 0;
                w = 0;
                c[0] = c[1] = c[2] = 0;
                p[0] = p[1] = p[2] = 0;
                v[0] = v[1] = v[2] = 0;
            }
        }

        public static part[] part_coin;
        public static part[] part_goal;
        public static part[] part_jump;
        public static int part_text_star;
        public static int part_text_squiggle;
        
        public static float goal_height;
        public static float jump_height;


        static Part()
        {
            part_coin = new part[Part.PART_MAX_COIN];
            {
                for (int i = 0; i < Part.PART_MAX_COIN; i++)
                {
                    part_coin[i] = new part("Coin" + i.ToString());
                }
            }

            part_goal = new part[Part.PART_MAX_GOAL];
            {
                for (int i = 0; i < Part.PART_MAX_GOAL; i++)
                {
                    part_goal[i] = new part("Goal" + i.ToString());
                }
            }

            part_jump = new part[Part.PART_MAX_JUMP];
            {
                for (int i = 0; i < Part.PART_MAX_JUMP; i++)
                {
                    part_jump[i] = new part("Jump" + i.ToString());
                }
            }
        }


        public static void ZeroAllParts()
        {
            for (int i = 0; i < Part.PART_MAX_COIN; i++)
            {
                part_coin[i].Zero();
            }

            for (int i = 0; i < Part.PART_MAX_GOAL; i++)
            {
                part_goal[i].Zero();
            }

            for (int i = 0; i < Part.PART_MAX_JUMP; i++)
            {
                part_jump[i].Zero();
            }
        }


        public static float rnd(double l, double h)
        {
            return (float)(l + (h - l) * Common.rand() / Common.RAND_MAX);
        }


        public static void part_reset(float zh, float jh)
        {
            int i;

            goal_height = zh;
            jump_height = jh;

            for (i = 0; i < PART_MAX_COIN; i++)
            {
                part_coin[i].t = 0.0f;
            }

            for (i = 0; i < PART_MAX_GOAL; i++)
            {
                float t = rnd(+0.1f, +1.0f);
                float a = rnd(-MathHelper.Pi, +MathHelper.Pi);
                float w = rnd(-MathHelper.TwoPi, +MathHelper.TwoPi);

                part_goal[i].t = t;
                part_goal[i].a = (float)MathHelper.RadiansToDegrees(a);
                part_goal[i].w = (float)MathHelper.RadiansToDegrees(w);

                part_goal[i].c[0] = 1;
                part_goal[i].c[1] = 1;
                part_goal[i].c[2] = 0;

                part_goal[i].p[0] = (float)System.Math.Sin(a);
                part_goal[i].p[1] = (1 - t) * goal_height;
                part_goal[i].p[2] = (float)System.Math.Cos(a);

                part_goal[i].v[0] = 0;
                part_goal[i].v[1] = 0;
                part_goal[i].v[2] = 0;
            }

            for (i = 0; i < PART_MAX_JUMP; i++)
            {
                float t = rnd(+0.1f, +1.0f);
                float a = rnd(-MathHelper.Pi, +MathHelper.Pi);
                float w = rnd(+0.5f, +2.5f);

                float vy = rnd(+0.025f, +0.25f);

                part_jump[i].t = t;
                part_jump[i].a = (float)MathHelper.RadiansToDegrees(a);
                part_jump[i].w = w;

                part_jump[i].c[0] = 1.0f;
                part_jump[i].c[1] = 1.0f;
                part_jump[i].c[2] = 1.0f;

                part_jump[i].p[0] = (float)System.Math.Sin(a);
                part_jump[i].p[1] = (1 - t) * jump_height;
                part_jump[i].p[2] = (float)System.Math.Cos(a);

                part_jump[i].v[0] = 0;
                part_jump[i].v[1] = vy;
                part_jump[i].v[2] = 0;
            }
        }


        public static void part_init(float zh, float jh)
        {
            ZeroAllParts();

            part_text_star = Image.make_image_from_file(IMG_PART_STAR);
            part_text_squiggle = Image.make_image_from_file(IMG_PART_SQUIGGLE);

            part_reset(zh, jh);


            //  Parts
            for (int i = 0; i < Part.PART_MAX_COIN; i++)
            {
                init_part(part_coin[i], IMG_PART_STAR);//lStarMaterial);
            }

            for (int i = 0; i < Part.PART_MAX_GOAL; i++)
            {
                init_part(part_goal[i], IMG_PART_STAR);//lStarMaterial);
            }

            for (int i = 0; i < Part.PART_MAX_JUMP; i++)
            {
                init_part(part_jump[i], IMG_PART_SQUIGGLE);//lSquiggleMaterial);
            }
        }

        static void init_part(part part, string textureName)//Material material)
        {
            if (part == null)
            {
                return;
            }
        }


        static int part_list;
        static void part_list_draw()
        {
            if (part_list == 0)
            {
                part_list = Video.GenLists(1);

                Video.NewList(part_list, Video.COMPILE);
                {
                    Video.Begin(Video.QUADS);
                    {
                        Video.TexCoord2(0, 0);
                        Video.Vertex2(-PART_SIZE, -PART_SIZE);

                        Video.TexCoord2(1, 0);
                        Video.Vertex2(+PART_SIZE, -PART_SIZE);

                        Video.TexCoord2(1, 1);
                        Video.Vertex2(+PART_SIZE, +PART_SIZE);

                        Video.TexCoord2(0, 1);
                        Video.Vertex2(-PART_SIZE, +PART_SIZE);
                    }
                    Video.End();
                }
                Video.EndList();
            }
            else
            {
                Video.CallList(part_list);
            }

            //part_reset(zh, jh);
        }


        public static void part_free()
        {
            if (GL.IsTexture(part_text_star))
            {
                GL.DeleteTextures(1, ref part_text_star);
                part_text_star = 0;
            }

            if (GL.IsTexture(part_text_squiggle))
            {
                GL.DeleteTextures(1, ref part_text_squiggle);
                part_text_squiggle = 0;
            }
        }



        public static void part_burst(float[] p, float[] c)
        {
            int i, n = 0;

            for (i = 0; n < 10 && i < PART_MAX_COIN; i++)
                if (part_coin[i].t <= 0)
                {
                    float a = rnd(-1.0f * MathHelper.Pi, +1.0f * MathHelper.Pi);
                    float b = rnd(+0.3f * MathHelper.Pi, +MathHelper.PiOver2);
                    float w = rnd(-4.0f * MathHelper.Pi, +4.0f * MathHelper.Pi);

                    part_coin[i].p[0] = p[0];
                    part_coin[i].p[1] = p[1];
                    part_coin[i].p[2] = p[2];

                    part_coin[i].v[0] = (float)(4 * System.Math.Cos(a) * System.Math.Cos(b));
                    part_coin[i].v[1] = (float)(4 * System.Math.Sin(b));
                    part_coin[i].v[2] = (float)(4 * System.Math.Sin(a) * System.Math.Cos(b));

                    part_coin[i].c[0] = c[0];
                    part_coin[i].c[1] = c[1];
                    part_coin[i].c[2] = c[2];

                    part_coin[i].t = 1;
                    part_coin[i].a = 0;
                    part_coin[i].w = (float)MathHelper.RadiansToDegrees(w);

                    n++;
                }
        }


        public static void part_fall(part[] part, int n, float[] g, float dt)
        {
            int i;

            for (i = 0; i < n; i++)
                if (part[i].t > 0)
                {
                    part[i].t -= dt;

                    part[i].v[0] += (g[0] * dt);
                    part[i].v[1] += (g[1] * dt);
                    part[i].v[2] += (g[2] * dt);

                    part[i].p[0] += (part[i].v[0] * dt);
                    part[i].p[1] += (part[i].v[1] * dt);
                    part[i].p[2] += (part[i].v[2] * dt);
                }
        }


        public static void part_spin(part[] part, int n, float[] g, float dt)
        {
            int i;

            for (i = 0; i < n; i++)
                if (part[i].t > 0)
                {
                    part[i].a += 30 * dt;

                    part[i].p[0] = (float)System.Math.Sin(MathHelper.DegreesToRadians(part[i].a));
                    part[i].p[2] = (float)System.Math.Cos(MathHelper.DegreesToRadians(part[i].a));
                }
        }


        public static void part_step(float[] g, float dt)
        {
            int i;

            part_fall(part_coin, PART_MAX_COIN, g, dt);

            if (g[1] > 0)
                part_fall(part_goal, PART_MAX_GOAL, g, dt);
            else
                part_spin(part_goal, PART_MAX_GOAL, g, dt);

            for (i = 0; i < PART_MAX_JUMP; i++)
            {
                part_jump[i].p[1] += part_jump[i].v[1] * dt;

                if (part_jump[i].p[1] > jump_height)
                    part_jump[i].p[1] = 0.0f;
            }
        }



        public static void part_draw(part part, float[] M, float[] p, float r, float rz, float s)
        {
            Video.MODELVIEW_PushMatrix();
            {
                Video.MODELVIEW_Translate(r * p[0], p[1], r * p[2]);
                Video.MODELVIEW_MultMatrix(M);
                Video.MODELVIEW_Rotate(rz, 0, 0, 1);
                Video.MODELVIEW_Scale(s, s, 1.0f);

                //Video.CallList(part_list);
                part_list_draw();
            }
            Video.MODELVIEW_PopMatrix();
        }


        public static void part_draw_coin(float[] M, float t)
        {
            int i;

            GL.BindTexture(TextureTarget.Texture2D, part_text_star);

            for (i = 0; i < PART_MAX_COIN; i++)
            {
                if (part_coin[i].t > 0)
                {
                    Video.Color(part_coin[i].c[0],
                              part_coin[i].c[1],
                              part_coin[i].c[2],
                              part_coin[i].t);

                    part_draw(part_coin[i], M, part_coin[i].p, 1.0f, t * part_coin[i].w, 1.0f);
                }
            }
        }


        public static void part_draw_goal(float[] M, float radius, float a, float t)
        {
            int i;

            GL.BindTexture(TextureTarget.Texture2D, part_text_star);

            Video.Color(1.0f, 1.0f, 0.0f, a);

            for (i = 0; i < PART_MAX_GOAL; i++)
            {
                if (part_goal[i].t > 0.0f)
                {
                    part_draw(part_goal[i], M, part_goal[i].p, radius - 0.05f, t * part_goal[i].w, 1.0f);
                }
            }
        }


        public static void part_draw_jump(float[] M, float radius, float a, float t)
        {
            int i;

            GL.BindTexture(TextureTarget.Texture2D, part_text_squiggle);

            for (i = 0; i < PART_MAX_JUMP; i++)
            {
                Video.Color(part_jump[i].c[0],
                          part_jump[i].c[1],
                          part_jump[i].c[2],
                          1.0f - part_jump[i].p[1] / jump_height);

                part_draw(part_jump[i], M, part_jump[i].p, radius - 0.05f, 0.0f, (float)(System.Math.Abs(System.Math.Sin(((t) / (part_jump[i].w) + 0.5f) * MathHelper.Pi))));
            }
        }
    }
}
