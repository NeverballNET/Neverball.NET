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
    /*
 * Let's pretend these aren't Ridiculously Convoluted Macros from
 * Hell, and that all this looks pretty straight-forward.  (In all
 * fairness, the macros have paid off.)
 *
 * A command's "write" and "read" functions are defined by calling the
 * PUT_FUNC or the GET_FUNC macro, respectively, with the command type
 * as argument, followed by the body of the function (which has
 * variables "fp" and "cmd" available), and finalised with the
 * END_FUNC macro, which must be terminated with a semi-colon.  Before
 * the function definitions, the BYTES macro must be redefined for
 * each command to an expression evaluating to the number of bytes
 * that the command will occupy in the file.  (See existing commands
 * for examples.)
 */


    /*
     * In an attempt to improve replay compatibility, a few guidelines
     * apply to command addition, removal, and modification:
     *
     * - New commands are added at the bottom of the list.
     *
     * - Existing commands are never modified nor removed.
     *
     * - The list is never reordered.  (It's tempting...)
     *
     * However, commands can be renamed (e.g., to add a "deprecated" tag,
     * because it's superseded by another command).
     */


    enum cmd_type
    {
        CMD_NONE = 0,

        CMD_END_OF_UPDATE,
        CMD_MAKE_BALL,
        CMD_MAKE_ITEM,
        CMD_PICK_ITEM,
        CMD_TILT_ANGLES,
        CMD_SOUND,
        CMD_TIMER,
        CMD_STATUS,
        CMD_COINS,
        CMD_JUMP_ENTER,
        CMD_JUMP_EXIT,
        CMD_BODY_PATH,
        CMD_BODY_TIME,
        CMD_GOAL_OPEN,
        CMD_SWCH_ENTER,
        CMD_SWCH_TOGGLE,
        CMD_SWCH_EXIT,
        CMD_UPDATES_PER_SECOND,
        CMD_BALL_RADIUS,
        CMD_CLEAR_ITEMS,
        CMD_CLEAR_BALLS,
        CMD_BALL_POSITION,
        CMD_BALL_BASIS,
        CMD_VIEW_POSITION,
        CMD_VIEW_CENTER,
        CMD_VIEW_BASIS,
        CMD_CURRENT_BALL,
        CMD_PATH_FLAG,
        CMD_STEP_SIMULATION,
        CMD_MAP,
        CMD_TILT_AXES,

        CMD_MAX
    }



    /*
     * Here are the members common to all structures.  Note that it
     * explicitly says "enum cmd_type", not "int".  This allows GCC to
     * catch and warn about unhandled command types in switch constructs
     * (handy when adding lnew commands).
     */


    class cmd_end_of_update
    {
        public void CopyFrom(cmd_end_of_update from)
        {
        }
    }

    class cmd_make_ball
    {
        public void CopyFrom(cmd_make_ball from)
        {
        }
    }

    class cmd_make_item
    {
        public float[] p = new float[3];
        public int t;
        public int n;

        public void CopyFrom(cmd_make_item from)
        {
            p[0] = from.p[0];
            p[1] = from.p[1];
            p[2] = from.p[2];
            t = from.t;
            n = from.n;
        }
    }

    class cmd_pick_item
    {
        public int hi;

        public void CopyFrom(cmd_pick_item from)
        {
            hi = from.hi;
        }
    }

    class cmd_tilt_angles
    {
        public float x;
        public float z;

        public void CopyFrom(cmd_tilt_angles from)
        {
            x = from.x;
            z = from.z;
        }
    }

    class cmd_sound
    {
        public string n;
        public float a;

        public void CopyFrom(cmd_sound from)
        {
            n = from.n;
            a = from.a;
        }
    }

    class cmd_timer
    {
        public float t;

        public void CopyFrom(cmd_timer from)
        {
            t = from.t;
        }
    }

    class cmd_status
    {
        public int t;

        public void CopyFrom(cmd_status from)
        {
            t = from.t;
        }
    }

    class cmd_coins
    {
        public int n;

        public void CopyFrom(cmd_coins from)
        {
            n = from.n;
        }
    }

    class cmd_jump_enter
    {
        public void CopyFrom(cmd_jump_enter from)
        {
        }
    }

    class cmd_jump_exit
    {
        public void CopyFrom(cmd_jump_exit from)
        {
        }
    }

    class cmd_body_path
    {
        public int bi;
        public int pi;

        public void CopyFrom(cmd_body_path from)
        {
            bi = from.bi;
            pi = from.pi;
        }
    }

    class cmd_body_time
    {
        public int bi;
        public float t;

        public void CopyFrom(cmd_body_time from)
        {
            bi = from.bi;
            t = from.t;
        }
    }

    class cmd_goal_open
    {
        public void CopyFrom(cmd_goal_open from)
        {
        }
    }

    class cmd_swch_enter
    {
        public int xi;

        public void CopyFrom(cmd_swch_enter from)
        {
            xi = from.xi;
        }
    }

    class cmd_swch_toggle
    {
        public int xi;

        public void CopyFrom(cmd_swch_toggle from)
        {
            xi = from.xi;
        }
    }

    class cmd_swch_exit
    {
        public int xi;

        public void CopyFrom(cmd_swch_exit from)
        {
            xi = from.xi;
        }
    }

    class cmd_updates_per_second
    {
        public int n;

        public void CopyFrom(cmd_updates_per_second from)
        {
            n = from.n;
        }
    }

    class cmd_ball_radius
    {
        public float r;

        public void CopyFrom(cmd_ball_radius from)
        {
            r = from.r;
        }
    }

    class cmd_clear_items
    {
        public void CopyFrom(cmd_clear_items from)
        {
        }
    }

    class cmd_clear_balls
    {
        public void CopyFrom(cmd_clear_balls from)
        {
        }
    }

    class cmd_ball_position
    {
        public float[] p = new float[3];

        public void CopyFrom(cmd_ball_position from)
        {
            p[0] = from.p[0];
            p[1] = from.p[1];
            p[2] = from.p[2];
        }
    }

    class cmd_ball_basis
    {
        public float[][] e = new float[2][]
        {
            new float[3],
            new float[3]
        };

        public void CopyFrom(cmd_ball_basis from)
        {
            e[0][0] = from.e[0][0];
            e[0][1] = from.e[0][1];
            e[0][2] = from.e[0][2];
            e[1][0] = from.e[1][0];
            e[1][1] = from.e[1][1];
            e[1][2] = from.e[1][2];
        }
    }

    class cmd_view_position
    {
        public float[] p = new float[3];

        public void CopyFrom(cmd_view_position from)
        {
            p[0] = from.p[0];
            p[1] = from.p[1];
            p[2] = from.p[2];
        }
    }

    class cmd_view_center
    {
        public float[] c = new float[3];

        public void CopyFrom(cmd_view_center from)
        {
            c[0] = from.c[0];
            c[1] = from.c[1];
            c[2] = from.c[2];
        }
    }

    class cmd_view_basis
    {
        public float[][] e = new float[2][]
        {
            new float[3],
            new float[3]
        };

        public void CopyFrom(cmd_view_basis from)
        {
            e[0][0] = from.e[0][0];
            e[0][1] = from.e[0][1];
            e[0][2] = from.e[0][2];
            e[1][0] = from.e[1][0];
            e[1][1] = from.e[1][1];
            e[1][2] = from.e[1][2];
        }
    }

    class cmd_current_ball
    {
        public int ui;

        public void CopyFrom(cmd_current_ball from)
        {
            ui = from.ui;
        }
    }

    class cmd_path_flag
    {
        public int pi;
        public int f;

        public void CopyFrom(cmd_path_flag from)
        {
            pi = from.pi;
            f = from.f;
        }
    }

    class cmd_step_simulation
    {
        public float dt;

        public void CopyFrom(cmd_step_simulation from)
        {
            dt = from.dt;
        }
    }

    class cmd_map
    {
        public string name;

        public struct cmd_map_version
        {
            public int x;
            public int y;
        }
        public cmd_map_version version;

        public void CopyFrom(cmd_map from)
        {
            name = from.name;
            version = from.version;
        }
    }

    class cmd_tilt_axes
    {
        public float[] x = new float[3];
        public float[] z = new float[3];

        public void CopyFrom(cmd_tilt_axes from)
        {
            x[0] = from.x[0];
            x[1] = from.x[1];
            x[2] = from.x[2];
            z[0] = from.z[0];
            z[1] = from.z[1];
            z[2] = from.z[2];
        }
    }



    class Command
    {
        public cmd_type type;

        public cmd_end_of_update eou;
        public cmd_make_ball mkball;
        public cmd_make_item mkitem;
        public cmd_pick_item pkitem;
        public cmd_tilt_angles tiltangles;
        public cmd_sound sound;
        public cmd_timer timer;
        public cmd_status status;
        public cmd_coins coins;
        public cmd_jump_enter jumpenter;
        public cmd_jump_exit jumpexit;
        public cmd_body_path bodypath;
        public cmd_body_time bodytime;
        public cmd_goal_open goalopen;
        public cmd_swch_enter swchenter;
        public cmd_swch_toggle swchtoggle;
        public cmd_swch_exit swchexit;
        public cmd_updates_per_second ups;
        public cmd_ball_radius ballradius;
        public cmd_clear_items clritems;
        public cmd_clear_balls clrballs;
        public cmd_ball_position ballpos;
        public cmd_ball_basis ballbasis;
        public cmd_view_position viewpos;
        public cmd_view_center viewcenter;
        public cmd_view_basis viewbasis;
        public cmd_current_ball currball;
        public cmd_path_flag pathflag;
        public cmd_step_simulation stepsim;
        public cmd_map map;
        public cmd_tilt_axes tiltaxes;


        public Command()
        {
            type = cmd_type.CMD_NONE;

            eou = new cmd_end_of_update();
            mkball = new cmd_make_ball();
            mkitem = new cmd_make_item();
            pkitem = new cmd_pick_item();
            tiltangles = new cmd_tilt_angles();
            sound = new cmd_sound();
            timer = new cmd_timer();
            status = new cmd_status();
            coins = new cmd_coins();
            jumpenter = new cmd_jump_enter();
            jumpexit = new cmd_jump_exit();
            bodypath = new cmd_body_path();
            bodytime = new cmd_body_time();
            goalopen = new cmd_goal_open();
            swchenter = new cmd_swch_enter();
            swchtoggle = new cmd_swch_toggle();
            swchexit = new cmd_swch_exit();
            ups = new cmd_updates_per_second();
            ballradius = new cmd_ball_radius();
            clritems = new cmd_clear_items();
            clrballs = new cmd_clear_balls();
            ballpos = new cmd_ball_position();
            ballbasis = new cmd_ball_basis();
            viewpos = new cmd_view_position();
            viewcenter = new cmd_view_center();
            viewbasis = new cmd_view_basis();
            currball = new cmd_current_ball();
            pathflag = new cmd_path_flag();
            stepsim = new cmd_step_simulation();
            map = new cmd_map();
            tiltaxes = new cmd_tilt_axes();
        }


        public void CopyFrom(Command from)
        {
            type = from.type;

            eou.CopyFrom(from.eou);
            mkball.CopyFrom(from.mkball);
            mkitem.CopyFrom(from.mkitem);
            pkitem.CopyFrom(from.pkitem);
            tiltangles.CopyFrom(from.tiltangles);
            sound.CopyFrom(from.sound);
            timer.CopyFrom(from.timer);
            status.CopyFrom(from.status);
            coins.CopyFrom(from.coins);
            jumpenter.CopyFrom(from.jumpenter);
            jumpexit.CopyFrom(from.jumpexit);
            bodypath.CopyFrom(from.bodypath);
            bodytime.CopyFrom(from.bodytime);
            goalopen.CopyFrom(from.goalopen);
            swchenter.CopyFrom(from.swchenter);
            swchtoggle.CopyFrom(from.swchtoggle);
            swchexit.CopyFrom(from.swchexit);
            ups.CopyFrom(from.ups);
            ballradius.CopyFrom(from.ballradius);
            clritems.CopyFrom(from.clritems);
            clrballs.CopyFrom(from.clrballs);
            ballpos.CopyFrom(from.ballpos);
            ballbasis.CopyFrom(from.ballbasis);
            viewpos.CopyFrom(from.viewpos);
            viewcenter.CopyFrom(from.viewcenter);
            viewbasis.CopyFrom(from.viewbasis);
            currball.CopyFrom(from.currball);
            pathflag.CopyFrom(from.pathflag);
            stepsim.CopyFrom(from.stepsim);
            map.CopyFrom(from.map);
            tiltaxes.CopyFrom(from.tiltaxes);
        }
    }
}
