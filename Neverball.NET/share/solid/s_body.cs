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
    class s_body
    {
        public float m_t;                                   /* time on current path       */

        public bool draw_ol;
        public bool draw_tl;
        public bool draw_rl;
        public bool draw_sl;

        public int m_pi;
        public int m_ni;
        public int m_l0;
        public int m_lc;
        public int m_g0;
        public int m_gc;


        public void sol_draw_list_ol(s_file fp)
        {
            if (draw_ol)
            {
                float[] p = new float[3];

                solid_phys.sol_body_p(p, fp, this);

                Video.MODELVIEW_PushMatrix();
                {
                    /* Translate a moving body. */

                    Video.MODELVIEW_Translate(p[0], p[1], p[2]);

                    /* Draw the body. */

                    fp_bv_i_ol_draw(fp);
                }
                Video.MODELVIEW_PopMatrix();
            }
        }

        void fp_bv_i_ol_draw(s_file fp)
        {
            //fp.bv[i].ol = Video.GenLists(1);

            //Video.NewList(fp.bv[i].ol, Video.COMPILE);
            {
                s_mtrl mp = s_mtrl.default_mtrl;

                mp = fp.sol_draw_body(this, mp, Solid.M_OPAQUE, 0);
                mp = fp.sol_draw_body(this, mp, Solid.M_OPAQUE, Solid.M_DECAL);

                mp = s_mtrl.default_mtrl.sol_draw_mtrl(mp);
            }
            //Video.EndList();
        }


        public void sol_draw_list_tl(s_file fp)
        {
            if (draw_tl)
            {
                float[] p = new float[3];

                solid_phys.sol_body_p(p, fp, this);

                Video.MODELVIEW_PushMatrix();
                {
                    /* Translate a moving body. */

                    Video.MODELVIEW_Translate(p[0], p[1], p[2]);

                    /* Draw the body. */

                    fp_bv_i_tl_draw(fp);
                }
                Video.MODELVIEW_PopMatrix();
            }
        }

        void fp_bv_i_tl_draw(s_file fp)
        {
            s_mtrl mp = s_mtrl.default_mtrl;

            mp = fp.sol_draw_body(this, mp, Solid.M_TRANSPARENT, Solid.M_DECAL);
            mp = fp.sol_draw_body(this, mp, Solid.M_TRANSPARENT, 0);

            mp = s_mtrl.default_mtrl.sol_draw_mtrl(mp);
        }


        public void sol_draw_list_rl(s_file fp)
        {
            if (draw_rl)
            {
                float[] p = new float[3];

                solid_phys.sol_body_p(p, fp, this);

                Video.MODELVIEW_PushMatrix();
                {
                    /* Translate a moving body. */

                    Video.MODELVIEW_Translate(p[0], p[1], p[2]);

                    /* Draw the body. */

                    fp_bv_i_rl_draw(fp);
                }
                Video.MODELVIEW_PopMatrix();
            }
        }

        void fp_bv_i_rl_draw(s_file fp)
        {
            s_mtrl mp = s_mtrl.default_mtrl;

            mp = fp.sol_draw_body(this, mp, Solid.M_REFLECTIVE, 0);

            mp = s_mtrl.default_mtrl.sol_draw_mtrl(mp);
        }


        public void sol_shad_list(s_file fp)
        {
            if (draw_sl)
            {
                float[] p = new float[3];

                solid_phys.sol_body_p(p, fp, this);

                Video.MODELVIEW_PushMatrix();
                {
                    /* Translate a moving body. */

                    Video.MODELVIEW_Translate(p[0], p[1], p[2]);

                    /* Translate the shadow on a moving body. */

                    Video.TEXTURE_PushMatrix();
                    Video.TEXTURE_Translate(p[0], p[2], 0.0f);

                    /* Draw the body. */

                    fp_bv_i_sl_draw(fp);

                    /* Pop the shadow translation. */

                    Video.TEXTURE_PopMatrix();
                }
                Video.MODELVIEW_PopMatrix();
            }
        }

        void fp_bv_i_sl_draw(s_file fp)
        {
            int on = fp.sol_enum_body(this, Solid.M_OPAQUE);
            int rn = fp.sol_enum_body(this, Solid.M_REFLECTIVE);
            int dn = fp.sol_enum_body(this, Solid.M_DECAL);

            if (on != 0)
            {
                sol_shad_body(fp, Solid.M_OPAQUE, 0);
            }

            if (rn != 0)
            {
                sol_shad_body(fp, Solid.M_REFLECTIVE, 0);
            }

            if (dn != 0)
            {
                sol_shad_body(fp, Solid.M_OPAQUE, Solid.M_DECAL);
            }
        }

        void sol_shad_body(s_file fp, int fl, int decal)
        {
            int mi, li, gi;

            if ((fl & Solid.M_DECAL) != 0)
            {
                GL.Enable(EnableCap.PolygonOffsetFill);
                Video.PolygonOffset(-1.0f, -2.0f);
            }

            Video.Begin(Video.TRIANGLES);
            {
                for (mi = 0; mi < fp.m_mc; mi++)
                {
                    if ((fp.m_mv[mi].m_fl & fl) != 0 &&
                        (fp.m_mv[mi].m_fl & Solid.M_DECAL) == decal)
                    {
                        for (li = 0; li < m_lc; li++)
                            sol_shad_lump(fp, fp.m_lv[m_l0 + li], mi);

                        for (gi = 0; gi < m_gc; gi++)
                            sol_shad_geom(fp, fp.m_gv[fp.m_iv[m_g0 + gi]], mi);
                    }
                }
            }
            Video.End();

            if ((fl & Solid.M_DECAL) != 0)
            {
                GL.Disable(EnableCap.PolygonOffsetFill);
            }
        }

        void sol_shad_lump(s_file fp, s_lump lp, int mi)
        {
            int i;

            for (i = 0; i < lp.m_gc; i++)
            {
                sol_shad_geom(fp, fp.m_gv[fp.m_iv[lp.m_g0 + i]], mi);
            }
        }

        void sol_shad_geom(s_file fp, s_geom gp, int mi)
        {
            if (gp.m_mi == mi)
            {
                float[] vi = fp.m_vv[gp.m_vi].m_p;
                float[] vj = fp.m_vv[gp.m_vj].m_p;
                float[] vk = fp.m_vv[gp.m_vk].m_p;

                Video.TexCoord2(vi[0], vi[2]);
                Video.Vertex3(vi);

                Video.TexCoord2(vj[0], vj[2]);
                Video.Vertex3(vj);

                Video.TexCoord2(vk[0], vk[2]);
                Video.Vertex3(vk);
            }
        }
    }
}
