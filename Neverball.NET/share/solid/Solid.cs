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
    class Solid
    {
        public const int MAGIC = 0x4c4f53af;
        public const int SOL_VERSION = 6;


        /* Material type flags */

        public const int M_OPAQUE = 1;
        public const int M_TRANSPARENT = 2;
        public const int M_REFLECTIVE = 4;
        public const int M_ENVIRONMENT = 8;
        public const int M_ADDITIVE = 16;
        public const int M_CLAMPED = 32;
        public const int M_DECAL = 64;
        public const int M_TWO_SIDED = 128;


        /* Billboard types. */

        public const int B_EDGE = 1;
        public const int B_FLAT = 2;
        public const int B_ADDITIVE = 4;
        public const int B_NOFACE = 8;


        /* Lump flags. */

        public const int L_DETAIL = 1;


        /* Item types. */

        public const int ITEM_NONE = 0;
        public const int ITEM_COIN = 1;
        public const int ITEM_GROW = 2;
        public const int ITEM_SHRINK = 3;



        public static void sol_load_vert(IntPtr fin, s_vert vp)
        {
            Binary.get_array(fin, vp.m_p, 3);
        }

        public static void sol_load_mtrl(IntPtr fin, s_mtrl mp)
        {
            Binary.get_array(fin, mp.m_d, 4);
            Binary.get_array(fin, mp.m_a, 4);
            Binary.get_array(fin, mp.m_s, 4);
            Binary.get_array(fin, mp.m_e, 4);
            Binary.get_array(fin, mp.m_h, 1);
            Binary.get_index(fin, ref mp.m_fl);

            FileSystem.fs_read(ref mp.m_TextureFileName, Config.PATHMAX, fin);
        }

        public static void sol_load_edge(IntPtr fin, s_edge ep)
        {
            Binary.get_index(fin, ref ep.m_vi);
            Binary.get_index(fin, ref ep.m_vj);
        }

        public static void sol_load_side(IntPtr fin, s_side sp)
        {
            Binary.get_array(fin, sp.m_n, 3);
            Binary.get_float(fin, ref sp.m_d);
        }

        public static void sol_load_texc(IntPtr fin, s_texc tp)
        {
            Binary.get_array(fin, tp.m_u, 2);
        }

        public static void sol_load_geom(IntPtr fin, s_geom gp)
        {
            Binary.get_index(fin, ref gp.m_mi);
            Binary.get_index(fin, ref gp.m_ti);
            Binary.get_index(fin, ref gp.m_si);
            Binary.get_index(fin, ref gp.m_vi);
            Binary.get_index(fin, ref gp.m_tj);
            Binary.get_index(fin, ref gp.m_sj);
            Binary.get_index(fin, ref gp.m_vj);
            Binary.get_index(fin, ref gp.m_tk);
            Binary.get_index(fin, ref gp.m_sk);
            Binary.get_index(fin, ref gp.m_vk);
        }

        public static void sol_load_lump(IntPtr fin, s_lump lp)
        {
            Binary.get_index(fin, ref lp.m_fl);
            Binary.get_index(fin, ref lp.m_v0);
            Binary.get_index(fin, ref lp.m_vc);
            Binary.get_index(fin, ref lp.m_e0);
            Binary.get_index(fin, ref lp.m_ec);
            Binary.get_index(fin, ref lp.m_g0);
            Binary.get_index(fin, ref lp.m_gc);
            Binary.get_index(fin, ref lp.m_s0);
            Binary.get_index(fin, ref lp.m_sc);
        }

        public static void sol_load_node(IntPtr fin, s_node np)
        {
            Binary.get_index(fin, ref np.m_si);
            Binary.get_index(fin, ref np.m_ni);
            Binary.get_index(fin, ref np.m_nj);
            Binary.get_index(fin, ref np.m_l0);
            Binary.get_index(fin, ref np.m_lc);
        }

        public static void sol_load_path(IntPtr fin, s_path pp)
        {
            Binary.get_array(fin, pp.m_p, 3);
            Binary.get_float(fin, ref pp.m_t);
            Binary.get_index(fin, ref pp.m_pi);
            Binary.get_index(fin, ref pp.m_f);
            Binary.get_index(fin, ref pp.m_s);
        }

        public static void sol_load_body(IntPtr fin, s_body bp)
        {
            Binary.get_index(fin, ref bp.m_pi);
            Binary.get_index(fin, ref bp.m_ni);
            Binary.get_index(fin, ref bp.m_l0);
            Binary.get_index(fin, ref bp.m_lc);
            Binary.get_index(fin, ref bp.m_g0);
            Binary.get_index(fin, ref bp.m_gc);
        }

        public static void sol_load_item(IntPtr fin, s_item hp)
        {
            Binary.get_array(fin, hp.m_p, 3);
            Binary.get_index(fin, ref hp.m_t);
            Binary.get_index(fin, ref hp.m_n);
        }

        public static void sol_load_goal(IntPtr fin, s_goal zp)
        {
            Binary.get_array(fin, zp.m_p, 3);
            Binary.get_float(fin, ref zp.m_r);
        }

        public static void sol_load_swch(IntPtr fin, s_swch xp)
        {
            Binary.get_array(fin, xp.m_p, 3);
            Binary.get_float(fin, ref xp.m_r);
            Binary.get_index(fin, ref xp.m_pi);
            Binary.get_float(fin, ref xp.m_t0);
            Binary.get_float(fin, ref xp.m_t);
            Binary.get_index(fin, ref xp.m_f0);
            Binary.get_index(fin, ref xp.m_f);
            Binary.get_index(fin, ref xp.m_i);
        }

        public static void sol_load_bill(IntPtr fin, s_bill rp)
        {
            Binary.get_index(fin, ref rp.m_fl);
            Binary.get_index(fin, ref rp.m_mi);
            Binary.get_float(fin, ref rp.m_t);
            Binary.get_float(fin, ref rp.m_d);
            Binary.get_array(fin, rp.m_w, 3);
            Binary.get_array(fin, rp.m_h, 3);
            Binary.get_array(fin, rp.m_rx, 3);
            Binary.get_array(fin, rp.m_ry, 3);
            Binary.get_array(fin, rp.m_rz, 3);
            Binary.get_array(fin, rp.m_p, 3);
        }

        public static void sol_load_jump(IntPtr fin, s_jump jp)
        {
            Binary.get_array(fin, jp.m_p, 3);
            Binary.get_array(fin, jp.m_q, 3);
            Binary.get_float(fin, ref jp.m_r);
        }

        public static void sol_load_ball(IntPtr fin, s_ball bp)
        {
            Binary.get_array(fin, bp.m_p, 3);
            Binary.get_float(fin, ref bp.m_r);

            bp.m_e[0][0] = 1.0f;
            bp.m_e[0][1] = 0.0f;
            bp.m_e[0][2] = 0.0f;

            bp.m_e[1][0] = 0.0f;
            bp.m_e[1][1] = 1.0f;
            bp.m_e[1][2] = 0.0f;

            bp.m_e[2][0] = 0.0f;
            bp.m_e[2][1] = 0.0f;
            bp.m_e[2][2] = 1.0f;
        }

        public static void sol_load_view(IntPtr fin, s_view wp)
        {
            Binary.get_array(fin, wp.m_p, 3);
            Binary.get_array(fin, wp.m_q, 3);
        }

        public static void sol_load_dict(IntPtr fin, s_dict dp)
        {
            Binary.get_index(fin, ref dp.m_ai);
            Binary.get_index(fin, ref dp.m_aj);
        }

        public static int sol_load_file(IntPtr fin, s_file fp)
        {
            int i;
            int magic = 0;
            int version = 0;

            Binary.get_index(fin, ref magic);
            Binary.get_index(fin, ref version);

            if (magic != MAGIC || version != SOL_VERSION)
                return 0;

            Binary.get_index(fin, ref fp.m_ac);
            Binary.get_index(fin, ref fp.m_dc);
            Binary.get_index(fin, ref fp.m_mc);
            Binary.get_index(fin, ref fp.m_vc);
            Binary.get_index(fin, ref fp.m_ec);
            Binary.get_index(fin, ref fp.m_sc);
            Binary.get_index(fin, ref fp.m_tc);
            Binary.get_index(fin, ref fp.m_gc);
            Binary.get_index(fin, ref fp.m_lc);
            Binary.get_index(fin, ref fp.m_nc);
            Binary.get_index(fin, ref fp.m_pc);
            Binary.get_index(fin, ref fp.m_bc);
            Binary.get_index(fin, ref fp.m_hc);
            Binary.get_index(fin, ref fp.m_zc);
            Binary.get_index(fin, ref fp.m_jc);
            Binary.get_index(fin, ref fp.m_xc);
            Binary.get_index(fin, ref fp.m_rc);
            Binary.get_index(fin, ref fp.m_uc);
            Binary.get_index(fin, ref fp.m_wc);
            Binary.get_index(fin, ref fp.m_ic);

            if (fp.m_mc != 0)
            {
                fp.m_mv = new s_mtrl[fp.m_mc];
                for (i = 0; i < fp.m_mc; i++)
                {
                    fp.m_mv[i] = new s_mtrl();
                }
            }

            if (fp.m_vc != 0)
            {
                fp.m_vv = new s_vert[fp.m_vc];
                for (i = 0; i < fp.m_vc; i++)
                {
                    fp.m_vv[i] = new s_vert();
                }
            }

            if (fp.m_ec != 0)
            {
                fp.m_ev = new s_edge[fp.m_ec];
                for (i = 0; i < fp.m_ec; i++)
                {
                    fp.m_ev[i] = new s_edge();
                }
            }

            if (fp.m_sc != 0)
            {
                fp.m_sv = new s_side[fp.m_sc];
                for (i = 0; i < fp.m_sc; i++)
                {
                    fp.m_sv[i] = new s_side();
                }
            }

            if (fp.m_tc != 0)
            {
                fp.m_tv = new s_texc[fp.m_tc];
                for (i = 0; i < fp.m_tc; i++)
                {
                    fp.m_tv[i] = new s_texc();
                }
            }

            if (fp.m_gc != 0)
            {
                fp.m_gv = new s_geom[fp.m_gc];
                for (i = 0; i < fp.m_gc; i++)
                {
                    fp.m_gv[i] = new s_geom();
                }
            }

            if (fp.m_lc != 0)
            {
                fp.m_lv = new s_lump[fp.m_lc];
                for (i = 0; i < fp.m_lc; i++)
                {
                    fp.m_lv[i] = new s_lump();
                }
            }

            if (fp.m_nc != 0)
            {
                fp.m_nv = new s_node[fp.m_nc];
                for (i = 0; i < fp.m_nc; i++)
                {
                    fp.m_nv[i] = new s_node();
                }
            }

            if (fp.m_pc != 0)
            {
                fp.m_pv = new s_path[fp.m_pc];
                for (i = 0; i < fp.m_pc; i++)
                {
                    fp.m_pv[i] = new s_path();
                }
            }

            if (fp.m_bc != 0)
            {
                fp.m_bv = new s_body[fp.m_bc];
                for (i = 0; i < fp.m_bc; i++)
                {
                    fp.m_bv[i] = new s_body();
                }
            }

            if (fp.m_hc != 0)
            {
                fp.m_hv = new s_item[fp.m_hc];
                for (i = 0; i < fp.m_hc; i++)
                {
                    fp.m_hv[i] = new s_item();
                }
            }

            if (fp.m_zc != 0)
            {
                fp.m_zv = new s_goal[fp.m_zc];
                for (i = 0; i < fp.m_zc; i++)
                {
                    fp.m_zv[i] = new s_goal();
                }
            }

            if (fp.m_jc != 0)
            {
                fp.m_jv = new s_jump[fp.m_jc];
                for (i = 0; i < fp.m_jc; i++)
                {
                    fp.m_jv[i] = new s_jump();
                }
            }

            if (fp.m_xc != 0)
            {
                fp.m_xv = new s_swch[fp.m_xc];
                for (i = 0; i < fp.m_xc; i++)
                {
                    fp.m_xv[i] = new s_swch();
                }
            }

            if (fp.m_rc != 0)
            {
                fp.m_rv = new s_bill[fp.m_rc];
                for (i = 0; i < fp.m_rc; i++)
                {
                    fp.m_rv[i] = new s_bill();
                }
            }

            if (fp.m_uc != 0)
            {
                fp.m_uv = new s_ball[fp.m_uc];
                for (i = 0; i < fp.m_uc; i++)
                {
                    fp.m_uv[i] = new s_ball();
                }
            }

            if (fp.m_wc != 0)
            {
                fp.m_wv = new s_view[fp.m_wc];
                for (i = 0; i < fp.m_wc; i++)
                {
                    fp.m_wv[i] = new s_view();
                }
            }

            if (fp.m_dc != 0)
            {
                fp.m_dv = new s_dict[fp.m_dc];
                for (i = 0; i < fp.m_dc; i++)
                {
                    fp.m_dv[i] = new s_dict();
                }
            }

            if (fp.m_ic != 0)
            {
                fp.m_iv = new int[fp.m_ic];
            }

            if (fp.m_ac != 0)
                FileSystem.fs_read(ref fp.m_av, fp.m_ac, fin);

            for (i = 0; i < fp.m_dc; i++) sol_load_dict(fin, fp.m_dv[i]);// + i);
            for (i = 0; i < fp.m_mc; i++) sol_load_mtrl(fin, fp.m_mv[i]);// + i);
            for (i = 0; i < fp.m_vc; i++) sol_load_vert(fin, fp.m_vv[i]);// + i);
            for (i = 0; i < fp.m_ec; i++) sol_load_edge(fin, fp.m_ev[i]);// + i);
            for (i = 0; i < fp.m_sc; i++) sol_load_side(fin, fp.m_sv[i]);// + i);
            for (i = 0; i < fp.m_tc; i++) sol_load_texc(fin, fp.m_tv[i]);// + i);
            for (i = 0; i < fp.m_gc; i++) sol_load_geom(fin, fp.m_gv[i]);// + i);
            for (i = 0; i < fp.m_lc; i++) sol_load_lump(fin, fp.m_lv[i]);// + i);
            for (i = 0; i < fp.m_nc; i++) sol_load_node(fin, fp.m_nv[i]);// + i);
            for (i = 0; i < fp.m_pc; i++) sol_load_path(fin, fp.m_pv[i]);// + i);
            for (i = 0; i < fp.m_bc; i++) sol_load_body(fin, fp.m_bv[i]);// + i);
            for (i = 0; i < fp.m_hc; i++) sol_load_item(fin, fp.m_hv[i]);// + i);
            for (i = 0; i < fp.m_zc; i++) sol_load_goal(fin, fp.m_zv[i]);// + i);
            for (i = 0; i < fp.m_jc; i++) sol_load_jump(fin, fp.m_jv[i]);// + i);
            for (i = 0; i < fp.m_xc; i++) sol_load_swch(fin, fp.m_xv[i]);// + i);
            for (i = 0; i < fp.m_rc; i++) sol_load_bill(fin, fp.m_rv[i]);// + i);
            for (i = 0; i < fp.m_uc; i++) sol_load_ball(fin, fp.m_uv[i]);// + i);
            for (i = 0; i < fp.m_wc; i++) sol_load_view(fin, fp.m_wv[i]);// + i);

            for (i = 0; i < fp.m_ic; i++) Binary.get_index(fin, ref fp.m_iv[i]);

            return 1;
        }

        public static int sol_load_head(IntPtr fin, s_file fp)
        {
            int magic = 0;
            int version = 0;

            Binary.get_index(fin, ref magic);
            Binary.get_index(fin, ref version);

            if (magic != MAGIC || version != SOL_VERSION)
                return 0;

            Binary.get_index(fin, ref fp.m_ac);
            Binary.get_index(fin, ref fp.m_dc);

#if false
            get_index(fin, fp.mc);
            get_index(fin, fp.vc);
            get_index(fin, fp.ec);
            get_index(fin, fp.sc);
            get_index(fin, fp.tc);
            get_index(fin, fp.gc);
            get_index(fin, fp.lc);
            get_index(fin, fp.nc);
            get_index(fin, fp.pc);
            get_index(fin, fp.bc);
            get_index(fin, fp.hc);
            get_index(fin, fp.zc);
            get_index(fin, fp.jc);
            get_index(fin, fp.xc);
            get_index(fin, fp.rc);
            get_index(fin, fp.uc);
            get_index(fin, fp.wc);
            get_index(fin, fp.ic);
#endif

            FileSystem.fs_seek(fin, 18 * 4);

            if (fp.m_ac != 0)
            {
                FileSystem.fs_read(ref fp.m_av, fp.m_ac, fin);
            }

            if (fp.m_dc != 0)
            {
                int i;

                fp.m_dv = new s_dict[fp.m_dc];

                for (i = 0; i < fp.m_dc; i++)
                {
                    fp.m_dv[i] = new s_dict();

                    sol_load_dict(fin, fp.m_dv[i]);// + i);
                }
            }

            return 1;
        }

        public static int sol_load_only_file(s_file fp, string filename)
        {
            IntPtr fin;
            int res = 0;

            if ((fin = FileSystem.fs_open(filename)) != IntPtr.Zero)
            {
                res = sol_load_file(fin, fp);
                FileSystem.fs_close(fin);
            }
            return res;
        }

        public static int sol_load_only_head(s_file fp, string filename)
        {
            IntPtr fin;
            int res = 0;

            if ((fin = FileSystem.fs_open(filename)) != IntPtr.Zero)
            {
                res = sol_load_head(fin, fp);
                FileSystem.fs_close(fin);
            }
            return res;
        }
    }
}
