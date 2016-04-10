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
    class StateNull : State
    {
        static StateNull m_Instance = null;
        public static State Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new StateNull();
                }

                return m_Instance;
            }
        }


        StateNull()
        {
            pointer = 1;
            gui_id = 0;
        }


        internal override int OnEnter()
        {
            hud.hud_free();
            gui.gui_free();
            Ball.ball_free();
            Ball.shad_free();
            Part.part_free();

            return 0;
        }


        internal override void OnLeave(int id)
        {
            int g = Config.config_get_d(Config.CONFIG_GEOMETRY);

            Part.part_init(Config.GOAL_HEIGHT, Config.JUMP_HEIGHT);
            Ball.shad_init();
            Ball.ball_init();
            s_goal.goal_init(g);
            s_jump.jump_init(g);
            s_swch.swch_init(g);
            gui.gui_init();
            hud.hud_init();
        }
    }
}
