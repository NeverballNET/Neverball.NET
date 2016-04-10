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
    class Back
    {
        public const float BACK_DIST = 256.0f;
        public const float FAR_DIST = 512.0f;

        static int back_text;

        const string BackMaterialName = Config.BallResourcePrefix + "Background" + "Material";

        static int slices = 0;
        static int stacks = 0;


        public static void back_init(string s, int b)
        {
            back_free();


            slices = b != 0 ? 32 : 16;
            stacks = b != 0 ? 16 : 8;


            back_text = Image.make_image_from_file(s);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);


            //  init list
            back_list_draw();
        }


        static int back_list;
        static void back_list_draw()
        {
            if (back_list == 0)
            {
                back_list = Video.GenLists(1);

                Video.NewList(back_list, Video.COMPILE);
                {
                    GL.BindTexture(TextureTarget.Texture2D, back_text);

                    Video.Color(1.0f, 1.0f, 1.0f);

                    for (int i = 0; i < stacks; i++)
                    {
                        float k0 = (float)i / stacks;
                        float k1 = (float)(i + 1) / stacks;

                        float s0 = (float)System.Math.Sin(MathHelper.Pi * (k0 - 0.5));
                        float c0 = (float)System.Math.Cos(MathHelper.Pi * (k0 - 0.5));
                        float s1 = (float)System.Math.Sin(MathHelper.Pi * (k1 - 0.5));
                        float c1 = (float)System.Math.Cos(MathHelper.Pi * (k1 - 0.5));

                        Video.Begin(Video.TRIANGLE_STRIP);
                        {
                            for (int j = 0; j <= slices; j++)
                            {
                                float k = (float)j / slices;
                                float fs = (float)System.Math.Sin(MathHelper.TwoPi * k);
                                float c = (float)System.Math.Cos(MathHelper.TwoPi * k);

                                Video.TexCoord2(k, 1.0f - k1);
                                Video.Vertex3(fs * c1, c * c1, s1);

                                Video.TexCoord2(k, 1.0f - k0);
                                Video.Vertex3(fs * c0, c * c0, s0);
                            }
                        }
                        Video.End();
                    }
                }
                Video.EndList();
            }
            else
            {
                Video.CallList(back_list);
            }
        }


        public static void back_free()
        {
            if (GL.IsList(back_list))
            {
                GL.DeleteLists(back_list, 1);
                back_list = 0;
            }

            if (GL.IsTexture(back_text))
            {
                GL.DeleteTextures(1, ref back_text);
                back_text = 0;
            }

            back_text = 0;
        }


        public static void back_draw()
        {
            float t = 0;

            Video.MODELVIEW_PushMatrix();
            {
                float dx = (float)(60 * System.Math.Sin(t / 10) + 90);
                float dz = (float)(180 * System.Math.Sin(t / 12));

                GL.Disable(EnableCap.Lighting);
                GL.DepthMask(false);
                {
                    Video.MODELVIEW_Scale(BACK_DIST, BACK_DIST, BACK_DIST);
                    Video.MODELVIEW_Rotate(dz, 0, 0, 1);
                    Video.MODELVIEW_Rotate(dx, 1, 0, 0);

                    back_list_draw();
                }
                GL.DepthMask(true);
                GL.Enable(EnableCap.Lighting);
            }
            Video.MODELVIEW_PopMatrix();
        }
    }
}
