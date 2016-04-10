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
    class enq_fn_game_proxy_enq : enq_fn
    {
        public static enq_fn_game_proxy_enq Instance;

        static enq_fn_game_proxy_enq()
        {
            Instance = new enq_fn_game_proxy_enq();
        }


        /*
         * Enqueue SRC in the game's command queue.
         */
        public static void game_proxy_enq(Command src)
        {
            Command dst;

            /*
             * Create the queue.  This is done only once during the life time
             * of the program.  For simplicity's sake, the queue is never
             * destroyed.
             */

            if (game_proxy.cmd_queue == null)
                game_proxy.cmd_queue = CLRQueue.CLR_queue_new();

            /*
             * Add a copy of the command to the end of the queue.
             */

            dst = new Command();
            {
                dst.CopyFrom(src);
                CLRQueue.queue_enq(game_proxy.cmd_queue, dst);
            }
        }


        public override void func(Command command)
        {
            game_proxy_enq(command);
        }
    }


    class game_proxy
    {
        public static CLRQueue cmd_queue = null;


        /*
         * Dequeue and return the head element in the game's command queue.
         * The element must be freed after use.
         */
        public static Command game_proxy_deq()
        {
            return cmd_queue != null ? (Command)CLRQueue.queue_deq(cmd_queue) : null;
        }


        /*
         * Clear the entire queue.
         */
        public static void game_proxy_clr()
        {
            Command cmdp;

            while ((cmdp = game_proxy_deq()) != null)
                cmdp = null;
        }
    }
}
