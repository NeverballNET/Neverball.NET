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
    class s_file
    {
        public int m_ac;
        public int m_mc;
        public int m_vc;
        public int m_ec;
        public int m_sc;
        public int m_tc;
        public int m_gc;
        public int m_lc;
        public int m_nc;
        public int m_pc;
        public int m_bc;
        public int m_hc;
        public int m_zc;
        public int m_jc;
        public int m_xc;
        public int m_rc;
        public int m_uc;
        public int m_wc;
        public int m_dc;
        public int m_ic;

        public System.Collections.Generic.List<string>//char          *
            m_av;
        public s_mtrl[] m_mv;
        public s_vert[] m_vv;
        public s_edge[] m_ev;
        public s_side[] m_sv;
        public s_texc[] m_tv;
        public s_geom[] m_gv;
        public s_lump[] m_lv;
        public s_node[] m_nv;
        public s_path[] m_pv;
        public s_body[] m_bv;
        public s_item[] m_hv;
        public s_goal[] m_zv;
        public s_jump[] m_jv;
        public s_swch[] m_xv;
        public s_bill[] m_rv;
        public s_ball[] m_uv;
        public s_view[] m_wv;
        public s_dict[] m_dv;
        public int[] m_iv;


        void Zero()
        {
            m_ac = 0;
            m_mc = 0;
            m_vc = 0;
            m_ec = 0;
            m_sc = 0;
            m_tc = 0;
            m_gc = 0;
            m_lc = 0;
            m_nc = 0;
            m_pc = 0;
            m_bc = 0;
            m_hc = 0;
            m_zc = 0;
            m_jc = 0;
            m_xc = 0;
            m_rc = 0;
            m_uc = 0;
            m_wc = 0;
            m_dc = 0;
            m_ic = 0;

            m_av = null;
            m_mv = null;
            m_vv = null;
            m_ev = null;
            m_sv = null;
            m_tv = null;
            m_gv = null;
            m_lv = null;
            m_nv = null;
            m_pv = null;
            m_bv = null;
            m_hv = null;
            m_zv = null;
            m_jv = null;
            m_xv = null;
            m_rv = null;
            m_uv = null;
            m_wv = null;
            m_dv = null;
            m_iv = null;
        }


        public string Get_av(int pos)
        {
            int i = 0;
            foreach (string str in m_av)
            {
                if (i == pos)
                {
                    return str;
                }

                i += str.Length + 1;
            }

            return null;
        }


        public void game_draw_goals(float[] M, float t)
        {
            if (game_client.goal_e != 0)
            {
                //  Draw the goal particles.

                GL.Enable(EnableCap.Texture2D);
                {
                    for (int zi = 0; zi < m_zc; zi++)
                    {
                        Video.MODELVIEW_PushMatrix();
                        {
                            Video.MODELVIEW_Translate(m_zv[zi].m_p[0], m_zv[zi].m_p[1], m_zv[zi].m_p[2]);

                            Part.part_draw_goal(M, m_zv[zi].m_r, game_client.goal_k, t);
                        }
                        Video.MODELVIEW_PopMatrix();
                    }
                }
                GL.Disable(EnableCap.Texture2D);
            }

            //  Draw the goal column.
            for (int zi = 0; zi < m_zc; zi++)
            {
                m_zv[zi].goal_draw();
            }
        }


        public void game_draw_swchs()
        {
            int xi;

            for (xi = 0; xi < m_xc; xi++)
            {
                m_xv[xi].swch_draw();
            }
        }


        public void sol_draw(int depthmask, int depthtest)
        {
            int bi;

            /* Render all opaque geometry into the color and depth buffers. */

            for (bi = 0; bi < m_bc; bi++)
            {
                m_bv[bi].sol_draw_list_ol(this);
            }

            /* Render all translucent geometry into only the color buffer. */

            if (depthtest == 0) GL.Disable(EnableCap.DepthTest);
            if (depthmask == 0) GL.DepthMask(false);

            {
                for (bi = 0; bi < m_bc; bi++)
                {
                    m_bv[bi].sol_draw_list_tl(this);
                }
            }

            if (depthmask == 0) GL.DepthMask(true);
            if (depthtest == 0) GL.Enable(EnableCap.DepthTest);
        }


        public void sol_refl()//s_file fp)
        {
            int bi;

            /* Render all reflective geometry into the color and depth buffers. */

            for (bi = 0; bi < m_bc; bi++)
            {
                m_bv[bi].sol_draw_list_rl(this);
            }
        }


        public void DrawBallShadow(int ball)
        {
            float[] ball_p = m_uv[ball//0
                ].m_p;
            float ball_r = m_uv[ball//0
                ].m_r;

            Ball.shad_draw_set(ball_p, ball_r);
            sol_shad();
            Ball.shad_draw_clr();
        }

        void sol_shad()//s_file fp)
        {
            int bi;

            /* Render all shadowed geometry. */

            GL.DepthMask(false);
            {
                for (bi = 0; bi < m_bc; bi++)
                {
                    m_bv[bi].sol_shad_list(this);
                }
            }
            GL.DepthMask(true);
        }


        public void sol_bill(float[] M, float t)
        {
            s_mtrl mp = s_mtrl.default_mtrl;

            int ri;

            for (ri = 0; ri < m_rc; ++ri)
            {
                s_bill rp = m_rv[ri];

                float T = rp.m_t * t;
                float S = (float)System.Math.Sin(T);

                float w = rp.m_w[0] + rp.m_w[1] * T + rp.m_w[2] * S;
                float h = rp.m_h[0] + rp.m_h[1] * T + rp.m_h[2] * S;
                float rx = rp.m_rx[0] + rp.m_rx[1] * T + rp.m_rx[2] * S;
                float ry = rp.m_ry[0] + rp.m_ry[1] * T + rp.m_ry[2] * S;
                float rz = rp.m_rz[0] + rp.m_rz[1] * T + rp.m_rz[2] * S;

                mp = m_mv[rp.m_mi].sol_draw_mtrl(mp);

                Video.MODELVIEW_PushMatrix();
                {
                    Video.MODELVIEW_Translate(rp.m_p[0], rp.m_p[1], rp.m_p[2]);

                    if (M != null &&
                        ((rp.m_fl & Solid.B_NOFACE) == 0))
                    {
                        Video.MODELVIEW_MultMatrix(M);
                    }

                    if (System.Math.Abs(rx) > 0.0f)
                        Video.MODELVIEW_Rotate(rx, 1.0f, 0.0f, 0.0f);
                    if (System.Math.Abs(ry) > 0.0f)
                        Video.MODELVIEW_Rotate(ry, 0.0f, 1.0f, 0.0f);
                    if (System.Math.Abs(rz) > 0.0f)
                        Video.MODELVIEW_Rotate(rz, 0.0f, 0.0f, 1.0f);

                    Video.Begin(Video.TRIANGLE_FAN);//GL_QUADS);
                    {
                        Video.TexCoord2(0.0f, 0.0f); Video.Vertex2(-w / 2, -h / 2);
                        Video.TexCoord2(1.0f, 0.0f); Video.Vertex2(+w / 2, -h / 2);
                        Video.TexCoord2(1.0f, 1.0f); Video.Vertex2(+w / 2, +h / 2);
                        Video.TexCoord2(0.0f, 1.0f); Video.Vertex2(-w / 2, +h / 2);
                    }
                    Video.End();
                }
                Video.MODELVIEW_PopMatrix();
            }

            mp = s_mtrl.default_mtrl.sol_draw_mtrl(mp);
        }


        public s_mtrl sol_draw_body(s_body bp, s_mtrl mp, int fl, int decal)
        {
            int mi, li, gi;

            /* Iterate all materials of the correct opacity. */

            for (mi = 0; mi < m_mc; mi++)
            {
                if ((m_mv[mi].m_fl & fl) != 0 &&
                    (m_mv[mi].m_fl & Solid.M_DECAL) == decal)
                {
                    if (sol_enum_mtrl(bp, mi) != 0)
                    {
                        //  Set the material state.

                        mp = m_mv[mi].sol_draw_mtrl(mp);

                        /* Render all geometry of that material. */

                        Video.Begin(Video.TRIANGLES);
                        {
                            for (li = 0; li < bp.m_lc; li++)
                            {
                                sol_draw_lump(m_lv[bp.m_l0 + li], mi);
                            }

                            for (gi = 0; gi < bp.m_gc; gi++)
                            {
                                sol_draw_geom(m_gv[m_iv[bp.m_g0 + gi]], mi);
                            }
                        }
                        Video.End();
                    }
                }
            }

            return mp;
        }

        void sol_draw_lump(s_lump lp, int mi)
        {
            int i;

            for (i = 0; i < lp.m_gc; i++)
            {
                sol_draw_geom(m_gv[m_iv[lp.m_g0 + i]], mi);
            }
        }


        /*
         * The  following code  renders a  body in  a  ludicrously inefficient
         * manner.  It iterates the materials and scans the data structure for
         * geometry using each.  This  has the effect of absolutely minimizing
         * material  changes,  texture  bindings,  and  Begin/End  pairs,  but
         * maximizing trips through the data.
         *
         * However, this  is only done once  for each level.   The results are
         * stored in display lists.  Thus, it is well worth it.
         */

        void sol_draw_geom(s_geom gp, int mi)
        {
            if (gp.m_mi == mi)
            {
                float[] ui = m_tv[gp.m_ti].m_u;
                float[] uj = m_tv[gp.m_tj].m_u;
                float[] uk = m_tv[gp.m_tk].m_u;

                float[] ni = m_sv[gp.m_si].m_n;
                float[] nj = m_sv[gp.m_sj].m_n;
                float[] nk = m_sv[gp.m_sk].m_n;

                float[] vi = m_vv[gp.m_vi].m_p;
                float[] vj = m_vv[gp.m_vj].m_p;
                float[] vk = m_vv[gp.m_vk].m_p;

                    GL.TexCoord2(ui);
                    GL.Normal3(ni);
                    Video.Vertex3(vi);

                    GL.TexCoord2(uj);
                    GL.Normal3(nj);
                    Video.Vertex3(vj);

                    GL.TexCoord2(uk);
                    GL.Normal3(nk);
                    Video.Vertex3(vk);
            }
        }


        public int sol_load_gl(string filename, int k, int s)
        {
            if (Solid.sol_load_only_file(this, filename) != 0)
            {
                sol_load_textures(k);
                sol_load_objects(s);

                return 1;
            }

            return 0;
        }
        

        void sol_load_textures(int k)
        {
            /* Load the image referenced by each material. */

            for (int i = 0; i < m_mc; i++)
            {
                if ((m_mv[i].m_o = sol_find_texture(m_mv[i].m_TextureFileName, ref m_mv[i].m_TextureFullFileName)) != 0)
                {
                    /* Set the texture to clamp or repeat based on material type. */

                    if ((m_mv[i].m_fl & Solid.M_CLAMPED) != 0)
                    {
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
                    }
                    else
                    {
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                    }
                }
            }
        }

        int sol_find_texture(string name, ref string full)
        {
            if (name.Contains("png/stars1"))
            {
                int k = 0;
            }

            int o;

            /* Prefer a lossless copy of the texture over a lossy compression. */

            /* Check for a PNG. */
            string png = name + ".png";

            if ((o = Image.make_image_from_file(png)) != 0)
            {
                full = png;
                return o;
            }

            /* Check for a JPG. */
            string jpg = name + ".jpg";

            if ((o = Image.make_image_from_file(jpg)) != 0)
            {
                full = jpg;
                return o;
            }

            full = null;

            return 0;
        }


        void sol_load_objects(int s)
        {
            int i;

            /* Here we sort geometry into display lists by material type. */

            for (i = 0; i < m_bc; i++)
            {
                s_body bp = m_bv[i];// + i;

                int on = sol_enum_body(bp, Solid.M_OPAQUE);
                int tn = sol_enum_body(bp, Solid.M_TRANSPARENT);
                int rn = sol_enum_body(bp, Solid.M_REFLECTIVE);
                int dn = sol_enum_body(bp, Solid.M_DECAL);

                /* Draw all opaque geometry, decals last. */

                if (on != 0)
                {
                    m_bv[i].draw_ol = true;
                    m_bv[i].sol_draw_list_ol(this);
                }
                else
                {
                    m_bv[i].draw_ol = false;
                }

                /* Draw all translucent geometry, decals first. */

                if (tn != 0)
                {
                    m_bv[i].draw_tl = true;
                    m_bv[i].sol_draw_list_tl(this);
                }
                else
                {
                    m_bv[i].draw_tl = false;
                }

                /* Draw all reflective geometry. */

                if (rn != 0)
                {
                    m_bv[i].draw_rl = true;
                    m_bv[i].sol_draw_list_rl(this);
                }
                else
                {
                    m_bv[i].draw_rl = false;
                }

                /* Draw all shadowed geometry. */

                if (s != 0 && (on != 0 || rn != 0))
                {
                    m_bv[i].draw_sl = true;
                    m_bv[i].sol_shad_list(this);
                }
                else
                {
                    m_bv[i].draw_sl = false;
                }
            }
        }


        public int sol_reflective()
        {
            int bi;

            for (bi = 0; bi < m_bc; bi++)
            {
                if (m_bv[bi].draw_rl)
                {
                    return 1;
                }
            }

            return 0;
        }


        public void sol_free_gl()
        {
            int i;

            for (i = 0; i < m_mc; i++)
            {
                if (GL.IsTexture(m_mv[i].m_o))
                {
                    GL.DeleteTextures(1, ref m_mv[i].m_o);
                    m_mv[i].m_o = 0;
                }
            }

            sol_free();
        }

        
        public void sol_free()
        {
            if (m_hv != null)
            {
                foreach (s_item item in m_hv)
                {
                    if (item != null)
                    {
                        item.Dispose();
                    }
                }
            }

            Zero();
        }


        public int sol_enum_mtrl(s_body bp, int mi)
        {
            int li, gi, c = 0;

            /* Count all lump geoms with this material. */

            for (li = 0; li < bp.m_lc; li++)
            {
                int g0 = m_lv[bp.m_l0 + li].m_g0;
                int gc = m_lv[bp.m_l0 + li].m_gc;

                for (gi = 0; gi < gc; gi++)
                {
                    if (m_gv[m_iv[g0 + gi]].m_mi == mi)
                    {
                        c++;
                    }
                }
            }

            /* Count all body geoms with this material. */

            for (gi = 0; gi < bp.m_gc; gi++)
            {
                if (m_gv[m_iv[bp.m_g0 + gi]].m_mi == mi)
                {
                    c++;
                }
            }

            return c;
        }


        public int sol_enum_body(s_body bp, int fl)
        {
            int c = 0;

            /* Count all geoms with this flag. */

            for (int mi = 0; mi < m_mc; mi++)
            {
                if ((m_mv[mi].m_fl & fl) != 0)
                {
                    c = c + sol_enum_mtrl(bp, mi);
                }
            }

            return c;
        }


        string m_Name;
        static int MaxID = 0;
        internal float sol_back_OffsetZ;

        public void sol_back(float n, float f, float t)
        {
            s_mtrl mp = s_mtrl.default_mtrl;

            //  Render all billboards in the given range.

            GL.Disable(EnableCap.Lighting);
            GL.DepthMask(false);
            {
                sol_back_OffsetZ = 0;
                for (int ri = 0; ri < m_rc; ri++)
                {
                    if (n <= m_rv[ri].m_d && m_rv[ri].m_d < f)
                    {
                        mp = m_rv[ri].sol_back_bill(this, mp, t);
                    }
                }

                mp = s_mtrl.default_mtrl.sol_draw_mtrl(mp);
            }
            GL.DepthMask(true);
            GL.Enable(EnableCap.Lighting);
        }


        public void DrawBackground(float t)
        {
            if (Config.config_get_d(Config.CONFIG_BACKGROUND) != 0)
            {
                /* Draw all background layers back to front. */

                sol_back(Back.BACK_DIST, Back.FAR_DIST, t);
                Back.back_draw();
                sol_back(0, Back.BACK_DIST, t);
            }
            else
            {
                Back.back_draw();
            }
        }
    }
}
