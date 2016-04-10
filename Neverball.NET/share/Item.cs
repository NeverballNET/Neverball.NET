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
    class Item : IDisposable
    {
        public const float ITEM_RADIUS = 0.15f;


        public s_file item_coin_file;
        public s_file item_grow_file;
        public s_file item_shrink_file;


        public Item()
        {
            item_coin_file = new s_file();
            item_grow_file = new s_file();
            item_shrink_file = new s_file();
        }


        public void Dispose()
        {
            item_free();
        }


        public static void item_pull()
        {
            Video.Color(1.0f, 1.0f, 1.0f);
            GL.Disable(EnableCap.ColorMaterial);
        }


        public void item_free()
        {
            if (item_coin_file != null)
            {
                item_coin_file.sol_free_gl();
                item_coin_file = null;
            }

            if (item_grow_file != null)
            {
                item_grow_file.sol_free_gl();
                item_grow_file = null;
            }

            if (item_shrink_file != null)
            {
                item_shrink_file.sol_free_gl();
                item_shrink_file = null;
            }
        }


        public static void item_push(int type)
        {
            GL.Enable(EnableCap.ColorMaterial);
        }


        public static void item_color(s_item hp, float[] c)
        {
            switch (hp.m_t)
            {
                case Solid.ITEM_COIN:

                    if (hp.m_n >= 1)
                    {
                        c[0] = 1.0f;
                        c[1] = 1.0f;
                        c[2] = 0.2f;
                    }
                    if (hp.m_n >= 5)
                    {
                        c[0] = 1.0f;
                        c[1] = 0.2f;
                        c[2] = 0.2f;
                    }
                    if (hp.m_n >= 10)
                    {
                        c[0] = 0.2f;
                        c[1] = 0.2f;
                        c[2] = 1.0f;
                    }
                    break;

                case Solid.ITEM_GROW:

                    c[0] = 0.00f;
                    c[1] = 0.51f;
                    c[2] = 0.80f;

                    break;

                case Solid.ITEM_SHRINK:

                    c[0] = 1.00f;
                    c[1] = 0.76f;
                    c[2] = 0.00f;

                    break;

                default:

                    c[0] = 1.0f;
                    c[1] = 1.0f;
                    c[2] = 1.0f;

                    break;
            }
        }


        public void item_draw(s_item hp, float r)
        {
            float[] c = new float[3];
            s_file fp = null;

            switch (hp.m_t)
            {
                case Solid.ITEM_COIN: fp = item_coin_file; break;
                case Solid.ITEM_GROW: fp = item_grow_file; break;
                case Solid.ITEM_SHRINK: fp = item_shrink_file; break;
            }

            item_color(hp, c);

            Video.Color(c);
            fp.sol_draw(0, 1);
        }


        public void item_init()
        {
            int T = Config.config_get_d(Config.CONFIG_TEXTURES);

            item_coin_file.sol_load_gl("item/coin/coin.sol", T, 0);
            item_grow_file.sol_load_gl("item/grow/grow.sol", T, 0);
            item_shrink_file.sol_load_gl("item/shrink/shrink.sol", T, 0);
        }
    }
}
