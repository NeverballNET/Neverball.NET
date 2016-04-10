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
    public class Video
    {
        static int check_extension(string needle)
        {
            //  Search for the given string in the OpenGL extension strings.

            string haystack = GL.GetString(StringName.Extensions);
            if (string.IsNullOrEmpty(haystack))
            {
                return 0;
            }

            string[] words = haystack.Split(new char[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string word in words)
            {
                if (string.Equals(needle, word))
                {
                    return 1;
                }
            }

            return 0;
        }


        public static int video_mode(int w, int h)
        {
            int stencil = Config.config_get_d(Config.CONFIG_REFLECTION) != 0 ? 1 : 0;

            Config.config_set_d(Config.CONFIG_WIDTH, w);
            Config.config_set_d(Config.CONFIG_HEIGHT, h);

            GL.Viewport(0, 0, w, h);
            GL.ClearColor(0.0f, 0.0f, 0.1f, 0.0f);

            GL.Enable(EnableCap.Normalize);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Blend);

            GL.LightModel(LightModelParameter.LightModelColorControl, (int)LightModelColorControl.SeparateSpecularColor);
            GL.LightModel(LightModelParameter.LightModelLocalViewer, 1);

            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.DepthFunc(DepthFunction.Lequal);

            GL.ReadBuffer(ReadBufferMode.Front);

            return 1;
        }


        public static PointI LastMousePosition = PointI.Zero;

        static int grabbed = 0;


        public static void CenterMousePosition()
        {
            LastMousePosition = new PointI((short)(Config.config_get_d(Config.CONFIG_WIDTH) / 2), (short)(Config.config_get_d(Config.CONFIG_HEIGHT) / 2));

            //TEMP  Cursor.Position = LastMousePosition;
        }


        public static void video_set_grab(int w)
        {
            if (w != 0)
            {
                CenterMousePosition();
            }

            //  TEMP    Capture = true;

            grabbed = 1;
        }


        public static void video_clr_grab()
        {
            //  TEMP    Capture = false;

            grabbed = 0;
        }


        public static int video_get_grab()
        {
            return grabbed;
        }


        public static void video_push_persp(float fov)
        {
            float n = 0.1f;
            float f = Back.FAR_DIST;

            double[] m = new double[4 * 4];

            double r = MathHelper.DegreesToRadians(fov / 2);
            double s = System.Math.Sin(r);
            double c = System.Math.Cos(r) / s;

            double a = ((double)Config.config_get_d(Config.CONFIG_WIDTH) /
                          (double)Config.config_get_d(Config.CONFIG_HEIGHT));

            GL.MatrixMode(MatrixMode.Projection);
            {
                GL.PushMatrix();
                GL.LoadIdentity();

                m[0 * 4 + 0] = c / a;
                m[0 * 4 + 1] = 0.0;
                m[0 * 4 + 2] = 0.0;
                m[0 * 4 + 3] = 0.0;
                m[1 * 4 + 0] = 0.0;
                m[1 * 4 + 1] = c;
                m[1 * 4 + 2] = 0.0;
                m[1 * 4 + 3] = 0.0;
                m[2 * 4 + 0] = 0.0;
                m[2 * 4 + 1] = 0.0;
                m[2 * 4 + 2] = -(f + n) / (f - n);
                m[2 * 4 + 3] = -1.0;
                m[3 * 4 + 0] = 0.0;
                m[3 * 4 + 1] = 0.0;
                m[3 * 4 + 2] = -2.0 * n * f / (f - n);
                m[3 * 4 + 3] = 0.0;

                //Alt.Sketch.Matrix4 mat = new Alt.Sketch.Matrix4(m, false);//true);

                GL.MultMatrix(m);//(float[])mat);
            }
            GL.MatrixMode(MatrixMode.Modelview);
        }


        public static void video_push_ortho()
        {
            double w = (double)Config.config_get_d(Config.CONFIG_WIDTH);
            double h = (double)Config.config_get_d(Config.CONFIG_HEIGHT);

            GL.MatrixMode(MatrixMode.Projection);
            {
                GL.PushMatrix();
                GL.LoadIdentity();
                GL.Ortho(0.0, w, 0.0, h, -1.0, +1.0);
            }
            GL.MatrixMode(MatrixMode.Modelview);
        }


        public static void video_pop_matrix()
        {
            GL.MatrixMode(MatrixMode.Projection);
            {
                GL.PopMatrix();
            }
            GL.MatrixMode(MatrixMode.Modelview);
        }


        public static void video_clear()
        {
            if (Config.config_get_d(Config.CONFIG_REFLECTION) != 0)
            {
                GL.Clear(
                    ClearBufferMask.ColorBufferBit |
                    ClearBufferMask.DepthBufferBit |
                    ClearBufferMask.StencilBufferBit);
            }
            else
            {
                GL.Clear(
                    ClearBufferMask.ColorBufferBit |
                    ClearBufferMask.DepthBufferBit);
            }
        }


        static System.Collections.Generic.Stack<Matrix4> MODELVIEW_MatrixStack = new System.Collections.Generic.Stack<Matrix4>();
        public static Matrix4 MODELVIEW_CurrentMatrix;// = Matrix4.Identity;

        static bool UseGLMatrix = true;
        //static bool UseGLMatrix = false;

        public static void MODELVIEW_PushMatrix()
        {
            MODELVIEW_MatrixStack.Push(MODELVIEW_CurrentMatrix);

            if (UseGLMatrix)
            {
                GL.MatrixMode(MatrixMode.Modelview);
                GL.PushMatrix();
            }
        }

        public static void MODELVIEW_PopMatrix()
        {
            MODELVIEW_CurrentMatrix = MODELVIEW_MatrixStack.Pop();

            if (UseGLMatrix)
            {
                GL.MatrixMode(MatrixMode.Modelview);
                GL.PopMatrix();
            }
            else
            {
                MODELVIEW_ApplyCurrentMatrix();
            }
        }

        public static void MODELVIEW_Translate(float offsetX, float offsetY, float offsetZ)
        {
            //TEMP  MODELVIEW_CurrentMatrix.Translate(offsetX, offsetY, offsetZ, MatrixOrder.Prepend);

            if (UseGLMatrix)
            {
                GL.MatrixMode(MatrixMode.Modelview);
                GL.Translate(offsetX, offsetY, offsetZ);
            }
            else
            {
                MODELVIEW_ApplyCurrentMatrix();
            }
        }

        public static void MODELVIEW_Rotate(float angle, float vecX, float vecY, float vecZ)
        {
            //TEMP  MODELVIEW_CurrentMatrix.Rotate(angle * MathHelper.Pi / 180, vecX, vecY, vecZ, MatrixOrder.Prepend);

            if (UseGLMatrix)
            {
                GL.MatrixMode(MatrixMode.Modelview);
                GL.Rotate(angle, vecX, vecY, vecZ);
            }
            else
            {
                MODELVIEW_ApplyCurrentMatrix();
            }
        }

        public static void MODELVIEW_Scale(float scaleX, float scaleY, float scaleZ)
        {
            //TEMP  MODELVIEW_CurrentMatrix.Scale(scaleX, scaleY, scaleZ, MatrixOrder.Prepend);

            if (UseGLMatrix)
            {
                GL.MatrixMode(MatrixMode.Modelview);
                GL.Scale(scaleX, scaleY, scaleZ);
            }
            else
            {
                MODELVIEW_ApplyCurrentMatrix();
            }
        }

        public static void MODELVIEW_MultMatrix(float[] m)
        {
            //TEMP  Matrix4 mat = new Matrix4(m, false);
            //TEMP  MODELVIEW_CurrentMatrix.Multiply(mat, MatrixOrder.Prepend);

            if (UseGLMatrix)
            {
                GL.MatrixMode(MatrixMode.Modelview);
                GL.MultMatrix(m);
            }
            else
            {
                MODELVIEW_ApplyCurrentMatrix();
            }
        }

        static void MODELVIEW_ApplyCurrentMatrix()
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            
            GL.MultMatrix(MODELVIEW_CurrentMatrix.ToFloatArray());
        }


        static System.Collections.Generic.Stack<Matrix4> TEXTURE_MatrixStack = new System.Collections.Generic.Stack<Matrix4>();
        public static Matrix4 TEXTURE_CurrentMatrix;// = Matrix4.Identity;

        public static void TEXTURE_PushMatrix()
        {
            TEXTURE_MatrixStack.Push(TEXTURE_CurrentMatrix);

            if (UseGLMatrix)
            {
                GL.MatrixMode(MatrixMode.Texture);
                GL.PushMatrix();
            }
        }

        public static void TEXTURE_PopMatrix()
        {
            TEXTURE_CurrentMatrix = TEXTURE_MatrixStack.Pop();

            if (UseGLMatrix)
            {
                GL.MatrixMode(MatrixMode.Texture);
                GL.PopMatrix();
            }
            else
            {
                TEXTURE_ApplyCurrentMatrix();
            }
        }

        public static void TEXTURE_Translate(float offsetX, float offsetY, float offsetZ)
        {
            //TEMP  TEXTURE_CurrentMatrix.Translate(offsetX, offsetY, offsetZ, MatrixOrder.Prepend);

            if (UseGLMatrix)
            {
                GL.MatrixMode(MatrixMode.Texture);
                GL.Translate(offsetX, offsetY, offsetZ);
            }
            else
            {
                TEXTURE_ApplyCurrentMatrix();
            }
        }

        public static void TEXTURE_Rotate(float angle, float vecX, float vecY, float vecZ)
        {
            //TEMP  TEXTURE_CurrentMatrix.Rotate(angle * MathHelper.Pi / 180, vecX, vecY, vecZ, MatrixOrder.Prepend);

            if (UseGLMatrix)
            {
                GL.MatrixMode(MatrixMode.Texture);
                GL.Rotate(angle, vecX, vecY, vecZ);
            }
            else
            {
                TEXTURE_ApplyCurrentMatrix();
            }
        }

        public static void TEXTURE_Scale(float scaleX, float scaleY, float scaleZ)
        {
            //TEMP  TEXTURE_CurrentMatrix.Scale(scaleX, scaleY, scaleZ, MatrixOrder.Prepend);

            if (UseGLMatrix)
            {
                GL.MatrixMode(MatrixMode.Texture);
                GL.Scale(scaleX, scaleY, scaleZ);
            }
            else
            {
                TEXTURE_ApplyCurrentMatrix();
            }
        }

        public static void TEXTURE_MultMatrix(float[] m)
        {
            //TEMP  Matrix4 mat = new Matrix4(m, false);
            //TEMP  TEXTURE_CurrentMatrix.Multiply(mat, MatrixOrder.Prepend);

            if (UseGLMatrix)
            {
                GL.MatrixMode(MatrixMode.Texture);
                GL.MultMatrix(m);
            }
            else
            {
                TEXTURE_ApplyCurrentMatrix();
            }
        }

        public static void TEXTURE_LoadIdentity()
        {
            TEXTURE_CurrentMatrix = Matrix4.Identity;

            if (UseGLMatrix)
            {
                GL.MatrixMode(MatrixMode.Texture);
                GL.LoadIdentity();
            }
            else
            {
                TEXTURE_ApplyCurrentMatrix();
            }
        }

        static void TEXTURE_ApplyCurrentMatrix()
        {
            GL.MatrixMode(MatrixMode.Texture);
            GL.LoadIdentity();
            //GL.MultMatrix((float[])CurrentMatrix);
            //TEMP  GL.MultMatrix(TEXTURE_CurrentMatrix.ToFloatArray(false));
        }


        public static ColorR CurrentColor = ColorR.White;
        public static void Color(float[] color)
        {
            if (color == null)
            {
                return;
            }

            if (color.Length == 3)
            {
                Color(color[0], color[1], color[2]);
            }
            else if (color.Length == 4)
            {
                Color(color[0], color[1], color[2], color[3]);
            }
        }

        public static void Color(float r, float g, float b, float a)
        {
            CurrentColor = ColorR.FromArgb(a, r, g, b);

            GL.Color4(r, g, b, a);
        }

        public static void Color(float r, float g, float b)
        {
            CurrentColor = ColorR.FromArgb(r, g, b);

            GL.Color3(r, g, b);
        }


        public static void TexCoord2(float s, float t)
        {
            GL.TexCoord2(s, t);
        }


        public static void Vertex2(float x, float y)
        {
            GL.Vertex2(x, y);
        }

        public static void Vertex3(float x, float y, float z)
        {
            GL.Vertex3(x, y, z);
        }

        public static void Vertex3(float[] v)
        {
            if (v == null)
            {
                return;
            }

            GL.Vertex3(v[0], v[1], v[2]);
        }


        public const int LINES = 1;
        public const int TRIANGLES = 4;
        public const int TRIANGLE_STRIP = 5;
        public const int TRIANGLE_FAN = 6;
        public const int QUADS = 7;
        public const int QUAD_STRIP = 8;
        
        public static void Begin(int mode)
        {
            PrimitiveType m = 0;
            switch (mode)
            {
                case LINES:
                    {
                        m = PrimitiveType.Lines;
                        break;
                    }
                case TRIANGLES:
                    {
                        m = PrimitiveType.Triangles;
                        break;
                    }
                case TRIANGLE_STRIP:
                    {
                        m = PrimitiveType.TriangleStrip;
                        break;
                    }
                case TRIANGLE_FAN:
                    {
                        m = PrimitiveType.TriangleFan;
                        break;
                    }
                case QUADS:
                    {
                        m = PrimitiveType.Quads;
                        break;
                    }
                case QUAD_STRIP:
                    {
                        m = PrimitiveType.QuadStrip;
                        break;
                    }
            }

            if (m == 0)
            {
                return;
            }

            GL.Begin(m);
        }
        
        public static void End()
        {
            GL.End();
        }


        static bool UseLists = true;


        public const int COMPILE = 4864;

        public static int GenLists(int range)
        {
            if (!UseLists)
            {
                return 0;
            }

            return GL.GenLists(range);
        }

        public static void NewList(int list, int mode)
        {
            if (!UseLists)
            {
                return;
            }

            ListMode m = (ListMode)0;
            switch (mode)
            {
                case COMPILE:
                    {
                        m = ListMode.Compile;
                        break;
                    }
            }

            if ((int)m == 0)
            {
                return;
            }

            GL.NewList(list, m);
        }

        public static void EndList()
        {
            if (!UseLists)
            {
                return;
            }

            GL.EndList();
        }

        public static void CallList(int list)
        {
            if (!UseLists)
            {
                return;
            }

            GL.CallList(list);
        }


        public static void PolygonOffset(float factor, float units)
        {
            GL.PolygonOffset(factor, units);
        }
    }
}
