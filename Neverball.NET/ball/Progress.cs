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
    class Progress
    {
        public struct progress
        {
            public int balls;
            public int score;
            public int times;
        }


        public static MODE mode = MODE.MODE_NORMAL;

        public static int level = 0;
        public static int next = -1;
        public static int done = 0;

        public static int bonus = 0;

        public static progress curr = new progress();
        public static progress prev = new progress();

        /* Set stats. */

        public static int score_rank = 3;
        public static int times_rank = 3;

        /* Level stats. */

        public static GAME status = GAME.GAME_NONE;

        public static int coins = 0;
        public static int timer = 0;

        public static int goal = 0; /* Current goal value. */
        public static int goal_i = 0; /* Initial goal value. */

        public static int goal_e = 0; /* Goal enabled flag                */
        public static int same_goal_e = 0; /* Reuse existing goal enabled flag */

        public static int time_rank = 3;
        public static int goal_rank = 3;
        public static int coin_rank = 3;



        public static void progress_init(MODE m)
        {
            mode = m;
            bonus = 0;

            curr.balls = 2;
            curr.score = 0;
            curr.times = 0;

            prev = curr;

            score_rank = times_rank = 3;

            done = 0;
        }


        public static int progress_dead()
        {
            return mode == MODE.MODE_CHALLENGE ? (curr.balls < 0 ? 1 : 0) : 0;
        }


        public static int progress_done()
        {
            return done;
        }


        public static int progress_lvl_high()
        {
            return (time_rank < 3 || goal_rank < 3 || coin_rank < 3) ? 1 : 0;
        }


        public static int progress_set_high()
        {
            return (score_rank < 3 || times_rank < 3) ? 1 : 0;
        }


        public static int progress_reward_ball(int s)
        {
            return (s > 0 && s % 100 == 0) ? 1 : 0;
        }


        public static int curr_level() { return level; }
        public static int curr_balls() { return curr.balls; }
        public static int curr_score() { return curr.score; }
        public static MODE curr_mode() { return mode; }
        public static int curr_bonus() { return bonus; }
        public static int curr_goal() { return goal; }

        public static int progress_time_rank() { return time_rank; }
        public static int progress_goal_rank() { return goal_rank; }
        public static int progress_coin_rank() { return coin_rank; }

        public static int progress_times_rank() { return times_rank; }
        public static int progress_score_rank() { return score_rank; }


        public static void progress_step()
        {
            if (goal > 0)
            {
                goal = goal_i - game_client.curr_coins();

                if (goal <= 0)
                {
                    game_server.game_set_goal();

                    goal = 0;
                }
            }
        }


        public static int progress_same_avail()
        {
            switch (status)
            {
                case GAME.GAME_NONE:
                    return mode != MODE.MODE_CHALLENGE ? 1 : 0;

                default:
                    if (mode == MODE.MODE_CHALLENGE)
                        return progress_dead() == 0 ? 1 : 0;
                    else
                        return 1;
            }
        }


        public static void progress_rename(int set_only)
        {
            string player = Config.config_get_s(Config.CONFIG_PLAYER);

            if (set_only != 0)
            {
                Set.set_rename_player(score_rank, times_rank, player);
            }
            else
            {
                Level.level_rename_player(level, time_rank, goal_rank, coin_rank, player);

                if (progress_done() != 0)
                    Set.set_rename_player(score_rank, times_rank, player);
            }
        }


        public static int progress_last()
        {
            return (mode != MODE.MODE_CHALLENGE && status == GAME.GAME_GOAL &&
                Level.level_exists(next) == 0) ? 1 : 0;
        }


        public static int progress_next_avail()
        {
            if (mode == MODE.MODE_CHALLENGE)
                return (status == GAME.GAME_GOAL && Level.level_exists(next) != 0) ? 1 : 0;
            else
                return Level.level_opened(next);
        }


        public static void progress_stat(GAME s)
        {
            int i, dirty = 0;

            status = s;

            coins = game_client.curr_coins();
            timer = (Level.level_time(level) == 0 ?
                     game_client.curr_clock() :
                     Level.level_time(level) - game_client.curr_clock());

            switch (status)
            {
                case GAME.GAME_GOAL:

                    for (i = curr.score + 1; i <= curr.score + coins; i++)
                        if (progress_reward_ball(i) != 0)
                            curr.balls++;

                    curr.score += coins;
                    curr.times += timer;

                    dirty = Level.level_score_update(level, timer, coins,
                                               ref time_rank,
                                               ref goal_rank,
                                               goal != 0,
                                               ref coin_rank);

                    if (Level.level_completed(level) == 0)
                    {
                        Level.level_complete(level);
                        dirty = 1;
                    }

                    /* Compute next level. */

                    if (mode == MODE.MODE_CHALLENGE)
                    {
                        for (next = level + 1; Level.level_bonus(next) != 0; next++)
                            if (Level.level_opened(next) == 0)
                            {
                                Level.level_open(next);
                                dirty = 1;
                                bonus++;
                            }
                    }
                    else
                    {
                        for (next = level + 1;
                             Level.level_bonus(next) != 0 &&
                             Level.level_opened(next) == 0;
                             next++)
                            /* Do nothing. */
                            ;
                    }

                    /* Open next level or complete the set. */

                    if (Level.level_exists(next) != 0)
                    {
                        Level.level_open(next);
                        dirty = 1;
                    }
                    else
                        done = mode == MODE.MODE_CHALLENGE ? 1 : 0;

                    break;

                case GAME.GAME_FALL:
                case GAME.GAME_TIME:
                    for (next = level + 1;
                         Level.level_exists(next) != 0 &&
                         Level.level_opened(next) == 0;
                         next++)
                        /* Do nothing. */
                        ;

                    curr.times += timer;
                    curr.balls -= 1;

                    break;
            }
        }


        public static void progress_exit()
        {
            Set.set_score_update(curr.times, curr.score, ref score_rank, ref times_rank);
        }


        public static int progress_play(int i)
        {
            if (Level.level_opened(i) != 0 || Config.config_cheat() != 0)
            {
                level = i;

                next = -1;
                status = GAME.GAME_NONE;
                coins = 0;
                timer = 0;
                goal = goal_i = Level.level_goal(level);

                if (same_goal_e != 0)
                    same_goal_e = 0;
                else
                    goal_e = ((mode != MODE.MODE_CHALLENGE && Level.level_completed(level) != 0 &&
                        Config.config_get_d(Config.CONFIG_LOCK_GOALS) == 0) || goal == 0) ? 1 : 0;

                prev = curr;

                time_rank = goal_rank = coin_rank = 3;

                if (Demo.demo_play_init(Config.USER_REPLAY_FILE, Set.get_level(level), mode,
                                   Level.level_time(level), Level.level_goal(level),
                                   goal_e, curr.score, curr.balls, curr.times) != 0)
                {
                    return 1;
                }
            }

            return 0;
        }


        public static int progress_next()
        {
            return progress_play(next);
        }


        public static int progress_same()
        {
            /* Reset progress and goal enabled state. */

            if (status == GAME.GAME_GOAL)
                curr = prev;

            same_goal_e = 1;

            return progress_play(level);
        }
    }
}
