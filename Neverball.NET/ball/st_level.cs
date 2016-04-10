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
    class st_level
    {
        public static State get_st_level()
        {
            if (st_level_class.Instance == null)
            {
                st_level_class.Instance = new st_level_class();
            }

            return st_level_class.Instance;
        }
    }


    class st_level_class : State
    {
        public static st_level_class Instance = null;


        internal override int OnEnter()
        {
            int id, jd, kd;

            if ((id = gui.gui_vstack(0)) != 0)
            {
                if ((jd = gui.gui_hstack(id)) != 0)
                {
                    gui.gui_filler(jd);

                    if ((kd = gui.gui_vstack(jd)) != 0)
                    {
                        string ln = Level.level_name(Progress.curr_level());
                        int b = Level.level_bonus(Progress.curr_level());

                        string setattr, lvlattr;

                        if (b != 0)
                        {
                            lvlattr = "Bonus Level " + ln;
                        }
                        else
                        {
                            lvlattr = "Level " + ln;
                        }

                        if (Progress.curr_mode() == MODE.MODE_CHALLENGE)
                        {
                            setattr = Set.set_name(Set.curr_set()) + ": " + game_common.mode_to_str(MODE.MODE_CHALLENGE, 1);
                        }
                        else
                        {
                            setattr = Set.set_name(Set.curr_set());
                        }

                        gui.gui_label(kd, lvlattr, b != 0 ? gui.GUI_MED : gui.GUI_LRG, gui.GUI_TOP,
                                  b != 0 ? widget.gui_wht : null, b != 0 ? widget.gui_grn : null);
                        gui.gui_label(kd, setattr, gui.GUI_SML, gui.GUI_BOT,
                                  widget.gui_wht, widget.gui_wht);
                    }
                    gui.gui_filler(jd);
                }
                gui.gui_space(id);

                gui.gui_multi(id, Level.level_msg(Progress.curr_level()),
                          gui.GUI_SML, gui.GUI_ALL,
                          widget.gui_wht, widget.gui_wht);

                gui.gui_layout(id, 0, 0);
            }

            game_server.game_set_fly(1, null);
            game_client.game_client_step();

            return id;
        }


        internal override void OnLeave(int id)
        {
            gui.gui_delete(id);
        }


        internal override void OnPaint(int id, float t)
        {
            game_client.ball_game_draw(0, t);
            gui.gui_paint(id);
        }


        internal override void OnTimer(int id, float dt)
        {
            game_client.game_step_fade(dt);
        }


        internal override int OnClick(Alt.GUI.MouseButtons b, int d)
        {
            return (b == Alt.GUI.MouseButtons.Left && d == 1) ? State.goto_state(st_play.get_st_play_ready()) : 1;
        }


        internal override int OnButton(int b, int d)
        {
            if (d != 0)
            {
                if (Config.config_tst_d(Config.CONFIG_JOYSTICK_BUTTON_A, b) != 0)
                {
                    return State.goto_state(st_play.get_st_play_ready());
                }
                if (Config.config_tst_d(Config.CONFIG_JOYSTICK_BUTTON_EXIT, b) != 0)
                {
                    return State.goto_state(st_over.get_st_ball_over());
                }
            }

            return 1;
        }

        
        public st_level_class()
        {
            pointer = 1;
            gui_id = 0;
        }
    }
}
