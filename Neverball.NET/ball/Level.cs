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
    class Level
    {
        /* TODO: turn into an internal structure. */

        public string file;
        public string shot;
        public string song;

        public string message;

        public string version;
        public string author;

        public int time; /* Time limit   */
        public int goal; /* Coins needed */

        public Score best_times;
        public Score fast_unlock;
        public Score most_coins;

        /* Set information. */

        public Set set;

        public int number;

        /* String representation of the number (eg. "IV") */
        public string name;

        public int is_locked;
        public int is_bonus;
        public int is_completed;


        public Level()
        {
            file = null;
            shot = null;
            song = null;

            message = null;

            version = null;
            author = null;


            best_times = new Score();
            fast_unlock = new Score();
            most_coins = new Score();


            name = null;
        }


        public void Zero()
        {
            file = null;
            shot = null;
            song = null;

            message = null;

            version = null;
            author = null;

            name = null;


            time = 0;
            goal = 0;


            best_times.Zero();
            fast_unlock.Zero();
            most_coins.Zero();


            set = null;

            number = 0;


            is_locked = 0;
            is_bonus = 0;
            is_completed = 0;
        }


        public static int level_exists(int i)
        {
            return Set.set_level_exists(Set.curr_set(), i);
        }


        public static void level_open(int i)
        {
            if (level_exists(i) != 0)
                Set.get_level(i).is_locked = 0;
        }


        public static int level_opened(int i)
        {
            return (level_exists(i) != 0 &&
                Set.get_level(i).is_locked == 0) ? 1 : 0;
        }


        public static void level_complete(int i)
        {
            if (level_exists(i) != 0)
                Set.get_level(i).is_completed = 1;
        }


        public static int level_completed(int i)
        {
            return (level_exists(i) != 0 && Set.get_level(i).is_completed != 0) ? 1 : 0;
        }


        public static int level_time(int i)
        {
            return Set.get_level(i).time;
        }


        public static int level_goal(int i)
        {
            return Set.get_level(i).goal;
        }


        public static int level_bonus(int i)
        {
            return (level_exists(i) != 0 && Set.get_level(i).is_bonus != 0) ? 1 : 0;
        }


        public static string level_shot(int i)
        {
            return level_exists(i) != 0 ? Set.get_level(i).shot : null;
        }


        public static string level_file(int i)
        {
            return level_exists(i) != 0 ? Set.get_level(i).file : null;
        }


        public static string level_name(int i)
        {
            return level_exists(i) != 0 ? Set.get_level(i).name : null;
        }


        public static string level_msg(int i)
        {
            if (level_exists(i) != 0)
            {
                string s = Set.get_level(i).message;

                if ((s != null ? s.Length : 0) > 0)
                {
                    return Set.get_level(i).message;
                }
            }

            return null;
        }


        public static void level_rename_player(int level,
                                 int time_rank,
                                 int goal_rank,
                                 int coin_rank,
                                 string player)
        {
            Level l = Set.get_level(level);

            l.best_times.player[time_rank] = player;
            l.fast_unlock.player[goal_rank] = player;
            l.most_coins.player[coin_rank] = player;
        }


        public static int level_score_update(int level,
                               int timer,
                               int coins,
                               ref int time_rank,
                               ref int goal_rank,
                               bool goal_rank_IsNull,
                               ref int coin_rank)
        {
            Level l = Set.get_level(level);
            string player = Config.config_get_s(Config.CONFIG_PLAYER);

            Score.score_time_insert(l.best_times, ref time_rank, false, player, timer, coins);
            Score.score_time_insert(l.fast_unlock, ref goal_rank, goal_rank_IsNull, player, timer, coins);
            Score.score_coin_insert(l.most_coins, ref coin_rank, false, player, timer, coins);

            if ((time_rank < 3) ||
                (!goal_rank_IsNull && goal_rank < 3) ||
                (coin_rank < 3))
                return 1;
            else
                return 0;
        }


        internal static void scan_level_attribs(Level l, s_file fp)
        {
            int i;

            int have_goal = 0, have_time = 0;
            int need_bt_easy = 0, need_fu_easy = 0, need_mc_easy = 0;

            for (i = 0; i < fp.m_dc; i++)
            {
                string k = fp.Get_av(fp.m_dv[i].m_ai);
                string v = fp.Get_av(fp.m_dv[i].m_aj);

                if (string.Equals(k, "message"))
                {
                    l.message = v;
                }
                else if (string.Equals(k, "song"))
                {
                    l.song = v;
                }
                else if (string.Equals(k, "shot"))
                {
                    l.shot = v;
                }
                else if (string.Equals(k, "goal"))
                {
                    l.goal = int.Parse(v, System.Globalization.CultureInfo.InvariantCulture);
                    have_goal = 1;
                }
                else if (string.Equals(k, "time"))
                {
                    l.time = int.Parse(v, System.Globalization.CultureInfo.InvariantCulture);
                    have_time = 1;
                }
                else if (string.Equals(k, "time_hs"))
                {
                    string[] arr = v.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (arr != null)
                    {
                        if (arr.Length > 0)
                        {
                            l.best_times.timer[0] = int.Parse(arr[0], System.Globalization.CultureInfo.InvariantCulture) != 0 ? 1 : 0;
                        }
                        if (arr.Length > 1)
                        {
                            l.best_times.timer[1] = int.Parse(arr[1], System.Globalization.CultureInfo.InvariantCulture) != 0 ? 1 : 0;
                        }
                        if (arr.Length > 2)
                        {
                            l.best_times.timer[2] = int.Parse(arr[2], System.Globalization.CultureInfo.InvariantCulture) != 0 ? 1 : 0;
                        }

                        switch (arr.Length)
                        {
                            case 2: need_bt_easy = 1; break;
                            case 3: break;

                            default:
                                /* TODO, complain loudly? */
                                break;
                        }
                    }
                }
                else if (string.Equals(k, "goal_hs"))
                {
                    string[] arr = v.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (arr != null)
                    {
                        if (arr.Length > 0)
                        {
                            l.fast_unlock.timer[0] = int.Parse(arr[0], System.Globalization.CultureInfo.InvariantCulture) != 0 ? 1 : 0;
                        }
                        if (arr.Length > 1)
                        {
                            l.fast_unlock.timer[1] = int.Parse(arr[1], System.Globalization.CultureInfo.InvariantCulture) != 0 ? 1 : 0;
                        }
                        if (arr.Length > 2)
                        {
                            l.fast_unlock.timer[2] = int.Parse(arr[2], System.Globalization.CultureInfo.InvariantCulture) != 0 ? 1 : 0;
                        }

                        switch (arr.Length)
                        {
                            case 2: need_fu_easy = 1; break;
                            case 3: break;

                            default:
                                /* TODO, complain loudly? */
                                break;
                        }
                    }
                }
                else if (string.Equals(k, "coin_hs"))
                {
                    string[] arr = v.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (arr != null)
                    {
                        if (arr.Length > 0)
                        {
                            l.most_coins.coins[0] = int.Parse(arr[0], System.Globalization.CultureInfo.InvariantCulture) != 0 ? 1 : 0;
                        }
                        if (arr.Length > 1)
                        {
                            l.most_coins.coins[1] = int.Parse(arr[1], System.Globalization.CultureInfo.InvariantCulture) != 0 ? 1 : 0;
                        }
                        if (arr.Length > 2)
                        {
                            l.most_coins.coins[2] = int.Parse(arr[2], System.Globalization.CultureInfo.InvariantCulture) != 0 ? 1 : 0;
                        }

                        switch (arr.Length)
                        {
                            case 2: need_mc_easy = 1; break;
                            case 3: break;

                            default:
                                /* TODO, complain loudly? */
                                break;
                        }
                    }
                }
                else if (string.Equals(k, "version"))
                {
                    l.version = v;
                }
                else if (string.Equals(k, "author"))
                {
                    l.author = v;
                }
                else if (string.Equals(k, "bonus"))
                {
                    l.is_bonus = int.Parse(v, System.Globalization.CultureInfo.InvariantCulture) != 0 ? 1 : 0;
                }
            }

            if (have_goal != 0)
            {
                if (need_mc_easy != 0)
                    l.most_coins.coins[2] = l.goal;

                l.fast_unlock.coins[0] =
                    l.fast_unlock.coins[1] =
                    l.fast_unlock.coins[2] = l.goal;
            }

            if (have_time != 0)
            {
                if (need_bt_easy != 0)
                    l.best_times.timer[2] = l.time;
                if (need_fu_easy != 0)
                    l.fast_unlock.timer[2] = l.time;

                l.most_coins.timer[0] =
                    l.most_coins.timer[1] =
                    l.most_coins.timer[2] = l.time;
            }
        }


        public static int level_load(string filename, Level level)
        {
            s_file sol = new s_file();

            int money;
            int i;

            level.Zero();

            if (Solid.sol_load_only_head(sol, filename) == 0)
            {
                return 0;
            }

            level.file = filename;

            Score.score_init_hs(level.best_times, 59999, 0);
            Score.score_init_hs(level.fast_unlock, 59999, 0);
            Score.score_init_hs(level.most_coins, 59999, 0);

            money = 0;

            for (i = 0; i < sol.m_hc; i++)
                if (sol.m_hv[i].m_t == Solid.ITEM_COIN)
                    money += sol.m_hv[i].m_n;

            level.most_coins.coins[0] = money;

            scan_level_attribs(level, sol);

            /* Compute initial hs default values */

            if (level.best_times.timer[2] <= level.best_times.timer[0])
                level.best_times.timer[0] = level.best_times.timer[1] = level.best_times.timer[2];
            else if (level.best_times.timer[2] <= level.best_times.timer[1])
                level.best_times.timer[1] = (level.best_times.timer[0] + level.best_times.timer[2]) / 2;

            if (level.fast_unlock.timer[2] <= level.fast_unlock.timer[0])
                level.fast_unlock.timer[0] = level.fast_unlock.timer[1] = level.fast_unlock.timer[2];
            else if (level.fast_unlock.timer[2] <= level.fast_unlock.timer[1])
                level.fast_unlock.timer[1] = (level.fast_unlock.timer[0] + level.fast_unlock.timer[2]) / 2;

            if (level.most_coins.coins[2] >= level.most_coins.coins[0])
                level.most_coins.coins[0] = level.most_coins.coins[1] = level.most_coins.coins[2];
            else if (level.most_coins.coins[2] >= level.most_coins.coins[1])
                level.most_coins.coins[1] = (level.most_coins.coins[0] + level.most_coins.coins[2]) / 2;

            sol.sol_free();

            return 1;
        }

    }
}
