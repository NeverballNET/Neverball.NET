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
    class s_swch
    {
        public float[] m_p = new float[3];                                /* position                   */
        public float m_r;                                   /* radius                     */
        public int m_pi;                                   /* the linked path            */

        public float m_t0;                                  /* default timer              */
        public float m_t;                                   /* current timer              */
        public int m_f0;                                  /* default state              */
        public int m_f;                                   /* current state              */
        public int m_i;                                   /* is invisible?              */
        public int m_e;                                   /* is a ball inside it?       */

        static float[][] swch_colors = new float[8][]
        {
            new float[4]{ 1.0f, 0.0f, 0.0f, 0.5f }, /* red out */
            new float[4]{ 1.0f, 0.0f, 0.0f, 0.0f },
            new float[4]{ 1.0f, 0.0f, 0.0f, 0.8f }, /* red in */
            new float[4]{ 1.0f, 0.0f, 0.0f, 0.0f },
            new float[4]{ 0.0f, 1.0f, 0.0f, 0.5f }, /* green out */
            new float[4]{ 0.0f, 1.0f, 0.0f, 0.0f },
            new float[4]{ 0.0f, 1.0f, 0.0f, 0.8f }, /* green in */
            new float[4]{ 0.0f, 1.0f, 0.0f, 0.0f }
        };

        static int swch_init_n = 0;

        static int MaxID = 0;


        public s_swch()
        {
            int id = MaxID++;
        }


        public static void swch_init(int b)
        {
            swch_init_n = b != 0 ? 32 : 8;
        }


        static int swch_list;
        void swch_list_draw(int index)
        {
            if (swch_list == 0)
            {
                swch_list = Video.GenLists(4);

                /* Create the display lists. */

                for (int k = 0; k < 4; k++)
                {
                    Video.NewList(swch_list + k, Video.COMPILE);
                    {
                        Video.Begin(Video.QUAD_STRIP);
                        {
                            for (int i = 0; i <= swch_init_n; i++)
                            {
                                float x = (float)System.Math.Cos(MathHelper.TwoPi * i / swch_init_n);
                                float y = (float)System.Math.Sin(MathHelper.TwoPi * i / swch_init_n);

                                Video.Color(swch_colors[2 * k + 0]);
                                Video.Vertex3(x, 0.0f, y);

                                Video.Color(swch_colors[2 * k + 1]);
                                Video.Vertex3(x, Config.SWCH_HEIGHT, y);
                            }
                        }
                        Video.End();
                    }
                    Video.EndList();
                }
            }
            else
            {
                Video.CallList(swch_list + index);
            }
        }


        public void swch_draw()
        {
            int index = m_f * 2 + m_e;
            if (m_i != 0)
            {
                index = -1;
            }

            for (int k = 0; k < 4; k++)
            {
                if (k == index)
                {
                    Video.MODELVIEW_PushMatrix();
                    {
                        Video.MODELVIEW_Translate(m_p[0], m_p[1], m_p[2]);
                        Video.MODELVIEW_Scale(m_r, 1, m_r);

                        swch_list_draw(index);
                    }
                    Video.MODELVIEW_PopMatrix();
                }
            }
        }
    }
}
