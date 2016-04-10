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
    class s_item : IDisposable
    {
        public float[] m_p = new float[3];                /* position                   */
        public int m_t;                                   /* type                       */
        public int m_n;                                   /* value                      */

        Item m_Item;


        public s_item()
        {
            m_Item = new Item();

            m_Item.item_init();
        }


        public void Dispose()
        {
            if (m_Item != null)
            {
                m_Item.Dispose();
                m_Item = null;
            }
        }


        public void item_draw(float r)
        {
            m_Item.item_draw(this, r);
        }
    }
}
