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
    class CLRList
    {
        public object data;
        public CLRList next;

        
        public CLRList()
        {
            data = null;
            next = null;
        }

        
        /*
         * Allocate and return a list cell initialised with FIRST and REST as
         * "data" and "next" members, respectively.
         */
        public static CLRList list_cons(object first, CLRList rest)
        {
            CLRList lnew;

            lnew = new CLRList();
            {
                lnew.data = first;
                lnew.next = rest;
            }

            return lnew;
        }


        /*
         * Free the list cell FIRST and return the "next" member. The "data"
         * member is not freed.
         */
        public static CLRList list_rest(CLRList first)
        {
            CLRList rest = first.next;

            first = null;

            return rest;
        }
    }
}
