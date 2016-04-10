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
    class CLRQueue
    {
        public CLRList head;
        public CLRList tail;

        public CLRQueue()
        {
            head = null;
            tail = null;
        }


        public static CLRQueue CLR_queue_new()
        {
            CLRQueue lnew;

            lnew = new CLRQueue();
            lnew.head = lnew.tail = CLRList.list_cons((object)null, null);

            return lnew;
        }


        public static void queue_free(CLRQueue q)
        {
            q.head = null;
            q = null;
        }


        public static int queue_empty(CLRQueue q)
        {
            return q.head == q.tail ? 1 : 0;
        }


        public static void queue_enq(CLRQueue q, object data)
        {
            q.tail.data = data;
            q.tail.next = CLRList.list_cons((object)null, null);

            q.tail = q.tail.next;
        }


        public static object queue_deq(CLRQueue q)
        {
            object data = null;

            if (queue_empty(q) == 0)
            {
                data = q.head.data;
                q.head = CLRList.list_rest(q.head);
            }

            return data;
        }
    }
}
