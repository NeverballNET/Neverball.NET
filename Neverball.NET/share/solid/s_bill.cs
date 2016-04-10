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
    class s_bill
    {
        public int m_fl;
        public int m_mi;
        public float m_t;                                   /* repeat time interval       */
        public float m_d;                                   /* distance                   */

        public float[] m_w = new float[3];                                /* width coefficients         */
        public float[] m_h = new float[3];                                /* height coefficients        */

        public float[] m_rx = new float[3];                               /* X rotation coefficients    */
        public float[] m_ry = new float[3];                               /* Y rotation coefficients    */
        public float[] m_rz = new float[3];                               /* Z rotation coefficients    */

        public float[] m_p = new float[3];


        public s_mtrl sol_back_bill(s_file fp, s_mtrl mp, float t)
        {
            float T = (m_t > 0.0f) ? (float)(System.Math.IEEERemainder(t, m_t) - m_t / 2) : 0.0f;

            float w = m_w[0] + m_w[1] * T + m_w[2] * T * T;
            float h = m_h[0] + m_h[1] * T + m_h[2] * T * T;

            if (w > 0 && h > 0)
            {
                float rx = m_rx[0] + m_rx[1] * T + m_rx[2] * T * T;
                float ry = m_ry[0] + m_ry[1] * T + m_ry[2] * T * T;
                float rz = m_rz[0] + m_rz[1] * T + m_rz[2] * T * T;

                Video.MODELVIEW_PushMatrix();
                {
                    float y0 = (m_fl & Solid.B_EDGE) != 0 ? 0 : -h / 2;
                    float y1 = (m_fl & Solid.B_EDGE) != 0 ? h : +h / 2;

                    Video.MODELVIEW_Rotate(ry, 0.0f, 1.0f, 0.0f);
                    Video.MODELVIEW_Rotate(rx, 1.0f, 0.0f, 0.0f);
                    Video.MODELVIEW_Translate(0.0f, 0.0f, -m_d);

                    if ((m_fl & Solid.B_FLAT) != 0)
                    {
                        Video.MODELVIEW_Rotate(-rx - 90.0f, 1.0f, 0.0f, 0.0f);
                        Video.MODELVIEW_Rotate(-ry, 0.0f, 0.0f, 1.0f);
                    }

                    if ((m_fl & Solid.B_EDGE) != 0)
                    {
                        Video.MODELVIEW_Rotate(-rx, 1.0f, 0.0f, 0.0f);
                    }

                    Video.MODELVIEW_Rotate(rz, 0.0f, 0.0f, 1.0f);

                    mp = fp.m_mv[m_mi].sol_draw_mtrl(mp);

                    Video.Begin(Video.TRIANGLE_FAN);
                    {
                        Video.TexCoord2(0.0f, 0.0f); Video.Vertex2(-w / 2, y0);
                        Video.TexCoord2(1.0f, 0.0f); Video.Vertex2(+w / 2, y0);
                        Video.TexCoord2(1.0f, 1.0f); Video.Vertex2(+w / 2, y1);
                        Video.TexCoord2(0.0f, 1.0f); Video.Vertex2(-w / 2, y1);
                    }
                    Video.End();
                }
                Video.MODELVIEW_PopMatrix();
            }

            return mp;
        }
    }
}
