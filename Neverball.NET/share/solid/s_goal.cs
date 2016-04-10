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
    class s_goal
    {
        public float[] m_p = new float[3];                                /* position                   */
        public float m_r;                                   /* radius                     */

        //  Geom
        static int goal_init_n;

        static int goal_list;


        public static void goal_init(int b)
        {
            goal_init_n = b != 0 ? 32 : 8;

            //  init list
            goal_list_draw();
        }

        static void goal_list_draw()
        {
            if (goal_list == 0)
            {
                goal_list = Video.GenLists(1);

                Video.NewList(goal_list, Video.COMPILE);
                {
                    Video.Begin(Video.TRIANGLE_STRIP);
                    {
                        for (int i = 0; i <= goal_init_n; i++)
                        {
                            float x = (float)System.Math.Cos(MathHelper.TwoPi * i / goal_init_n);
                            float y = (float)System.Math.Sin(MathHelper.TwoPi * i / goal_init_n);

                            Video.Color(1.0f, 1.0f, 0.0f, 0.5f);
                            Video.Vertex3(x, 0.0f, y);

                            Video.Color(1.0f, 1.0f, 0.0f, 0.0f);
                            Video.Vertex3(x, Config.GOAL_HEIGHT, y);
                        }
                    }
                    Video.End();
                }
                Video.EndList();
            }
            else
            {
                Video.CallList(goal_list);
            }
        }

        public void goal_draw()
        {
            if (game_client.goal_e != 0)
            {
                Video.MODELVIEW_PushMatrix();
                {
                    Video.MODELVIEW_Translate(m_p[0], m_p[1], m_p[2]);

                    Video.MODELVIEW_Scale(m_r, game_client.goal_k, m_r);

                    goal_list_draw();
                }
                Video.MODELVIEW_PopMatrix();
            }
        }
    }
}
