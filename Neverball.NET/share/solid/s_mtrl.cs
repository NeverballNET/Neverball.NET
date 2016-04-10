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
    class s_mtrl
    {
        public float[] m_d = new float[4];                                /* diffuse color              */
        public float[] m_a = new float[4];                                /* ambient color              */
        public float[] m_s = new float[4];                                /* specular color             */
        public float[] m_e = new float[4];                                /* emission color             */
        public float[] m_h = new float[1];                                /* specular exponent          */
        public float m_angle;

        public int m_fl;                                    /* material flags             */

        public int m_o;                                  /* OpenGL texture object      */
        public string m_TextureFileName;                         /* texture file name          */
        public string m_TextureFullFileName;

        public static readonly s_mtrl default_mtrl;


        static s_mtrl()
        {
            default_mtrl = new s_mtrl();
            {
                default_mtrl.m_d = new float[4] { 0.8f, 0.8f, 0.8f, 1.0f };
                default_mtrl.m_a = new float[4] { 0.2f, 0.2f, 0.2f, 1.0f };
                default_mtrl.m_s = new float[4] { 0.0f, 0.0f, 0.0f, 1.0f };
                default_mtrl.m_e = new float[4] { 0.0f, 0.0f, 0.0f, 1.0f };
                default_mtrl.m_h = new float[1] { 0.0f };
                default_mtrl.m_angle = 0;
                default_mtrl.m_fl = Solid.M_OPAQUE;
                default_mtrl.m_o = 0;
                default_mtrl.m_TextureFileName = null;
            }
        }


        public s_mtrl sol_draw_mtrl(s_mtrl mq)
        {
            /* Change material properties only as needed. */

            if (!color_cmp(m_a, mq.m_a))
                GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, m_a);
            if (!color_cmp(m_d, mq.m_d))
                GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, m_d);
            if (!color_cmp(m_s, mq.m_s))
                GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, m_s);
            if (!color_cmp(m_e, mq.m_e))
                GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, m_e);
            if (tobyte(m_h[0]) != tobyte(mq.m_h[0]))
                GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, m_h);

            /* Bind the texture. */

            if (m_o != mq.m_o &&
                m_o != 0)
            {
                GL.BindTexture(TextureTarget.Texture2D, m_o);
            }

            /* Enable environment mapping. */

            if ((m_fl & Solid.M_ENVIRONMENT) != 0 && (mq.m_fl & Solid.M_ENVIRONMENT) == 0)
            {
                GL.Enable(EnableCap.TextureGenS);
                GL.Enable(EnableCap.TextureGenT);

                GL.TexGen(TextureCoordName.S, TextureGenParameter.TextureGenMode, (int)TextureGenMode.SphereMap);
                GL.TexGen(TextureCoordName.T, TextureGenParameter.TextureGenMode, (int)TextureGenMode.SphereMap);
            }

            /* Disable environment mapping. */

            if ((mq.m_fl & Solid.M_ENVIRONMENT) != 0 && (m_fl & Solid.M_ENVIRONMENT) == 0)
            {
                GL.Disable(EnableCap.TextureGenS);
                GL.Disable(EnableCap.TextureGenT);
            }

            /* Enable additive blending. */

            if ((m_fl & Solid.M_ADDITIVE) != 0 &&
                (mq.m_fl & Solid.M_ADDITIVE) == 0)
            {
                GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
            }

            /* Enable standard blending. */

            if ((mq.m_fl & Solid.M_ADDITIVE) != 0 && (m_fl & Solid.M_ADDITIVE) == 0)
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            /* Enable visibility-from-behind. */

            if ((m_fl & Solid.M_TWO_SIDED) != 0 && (mq.m_fl & Solid.M_TWO_SIDED) == 0)
            {
                GL.Disable(EnableCap.CullFace);
                GL.LightModel(LightModelParameter.LightModelTwoSide, 1);
            }

            /* Disable visibility-from-behind. */

            if ((mq.m_fl & Solid.M_TWO_SIDED) != 0 && (m_fl & Solid.M_TWO_SIDED) == 0)
            {
                GL.Enable(EnableCap.CullFace);
                GL.LightModel(LightModelParameter.LightModelTwoSide, 0);
            }

            /* Enable decal offset. */

            if ((m_fl & Solid.M_DECAL) != 0 && (mq.m_fl & Solid.M_DECAL) == 0)
            {
                GL.Enable(EnableCap.PolygonOffsetFill);
                Video.PolygonOffset(-1.0f, -2.0f);
            }

            /* Disable decal offset. */

            if ((mq.m_fl & Solid.M_DECAL) != 0 && (m_fl & Solid.M_DECAL) == 0)
            {
                GL.Disable(EnableCap.PolygonOffsetFill);
            }

            return this;// mp;
        }


        byte tobyte(float f)
        {
            return ((byte)(f * 255.0f));
        }


        bool color_cmp(float[] a, float[] b)
        {
            return (tobyte((a)[0]) == tobyte((b)[0]) &&
                                 tobyte((a)[1]) == tobyte((b)[1]) &&
                                 tobyte((a)[2]) == tobyte((b)[2]) &&
                                 tobyte((a)[3]) == tobyte((b)[3]));
        }
    }
}
