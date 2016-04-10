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
    class Set
    {
        public const string SET_FILE = "sets.txt";
        public const string SET_MISC = "set-misc.txt";

        public const int MAXLVL = 25;


        public static int curr;


        public static int curr_set()
        {
            return curr;
        }
        

        public string file;

        public string id;                  /* Internal set identifier    */
        public string name;                /* Set name                   */
        public string desc;                /* Set description            */
        public string shot;                /* Set screen-shot            */

        public string user_scores;         /* User high-score file       */
        public string cheat_scores;        /* Cheat mode score file      */

        public Score coin_score;   /* Challenge score            */
        public Score time_score;   /* Challenge score            */

        /* Level info */

        public int count;                /* Number of levels           */
        public string[] level_name_v; /* List of level file names   */


        public Set()
        {
            coin_score = new Score();   /* Challenge score            */
            time_score = new Score();   /* Challenge score            */

            level_name_v = new string[MAXLVL];
            for (int i = 0; i < MAXLVL; i++)
            {
                level_name_v[i] = null;
            }
        }


        public void Zero()
        {
            file = null;

            id = null;                  /* Internal set identifier    */
            name = null;                /* Set name                   */
            desc = null;                /* Set description            */
            shot = null;                /* Set screen-shot            */

            user_scores = null;         /* User high-score file       */
            cheat_scores = null;        /* Cheat mode score file      */

            coin_score.Zero();   /* Challenge score            */
            time_score.Zero();   /* Challenge score            */

            count = 0;
            for (int i = 0; i < MAXLVL; i++)
            {
                level_name_v[i] = null;
            }
        }

        
        public static Level[] level_v;
        public static MyArray sets = null;


        static Set()
        {
            level_v = new Level[MAXLVL];
            for (int i = 0; i < MAXLVL; i++)
            {
                level_v[i] = new Level();
            }
        }


        public static Set SET_GET(MyArray a, int i)
        {
            return ((Set)MyArray.array_get((a), (i)));
        }


        public static void set_cheat()
        {
            int i;

            for (i = 0; i < SET_GET(sets, curr).count; i++)
            {
                level_v[i].is_locked = 0;
                level_v[i].is_completed = 1;
            }
        }


        public static int set_load(Set s, string filename)
        {
            IntPtr fin;
            string scores = null;
            string level_name = null;

            /* Skip "Misc" set when not in dev mode. */

            if (string.Equals(filename, SET_MISC) &&
                Config.config_cheat() == 0)
                return 0;

            fin = FileSystem.fs_open(filename);

            if (fin == IntPtr.Zero)
            {
                return 0;
            }

            s.Zero();

            /* Set some sane values in case the scores are missing. */

            Score.score_init_hs(s.time_score, 359999, 0);
            Score.score_init_hs(s.coin_score, 359999, 0);

            s.file = filename;

            if (FileSystem.read_line(ref s.name, fin) != 0 &&
                FileSystem.read_line(ref s.desc, fin) != 0 &&
                FileSystem.read_line(ref s.id, fin) != 0 &&
                FileSystem.read_line(ref s.shot, fin) != 0 &&
                FileSystem.read_line(ref scores, fin) != 0)
            {
                if (scores != null)
                {
                    string[] arr = scores.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (arr != null)
                    {
                        int i = 0;
                        while (arr.Length > i)
                        {
                            if (arr[i].Length > 0)
                            {
                                s.time_score.timer[0] = int.Parse(arr[i++], System.Globalization.CultureInfo.InvariantCulture);
                                break;
                            }

                            i++;
                        }

                        while (arr.Length > i)
                        {
                            if (arr[i].Length > 0)
                            {
                                s.time_score.timer[1] = int.Parse(arr[i++], System.Globalization.CultureInfo.InvariantCulture);
                                break;
                            }

                            i++;
                        }

                        while (arr.Length > i)
                        {
                            if (arr[i].Length > 0)
                            {
                                s.time_score.timer[2] = int.Parse(arr[i++], System.Globalization.CultureInfo.InvariantCulture);
                                break;
                            }

                            i++;
                        }

                        while (arr.Length > i)
                        {
                            if (arr[i].Length > 0)
                            {
                                s.coin_score.coins[0] = int.Parse(arr[i++], System.Globalization.CultureInfo.InvariantCulture);
                                break;
                            }

                            i++;
                        }

                        while (arr.Length > i)
                        {
                            if (arr[i].Length > 0)
                            {
                                s.coin_score.coins[1] = int.Parse(arr[i++], System.Globalization.CultureInfo.InvariantCulture);
                                break;
                            }

                            i++;
                        }

                        while (arr.Length > i)
                        {
                            if (arr[i].Length > 0)
                            {
                                s.coin_score.coins[2] = int.Parse(arr[i++], System.Globalization.CultureInfo.InvariantCulture);
                                break;
                            }

                            i++;
                        }
                    }
                }

                scores = null;

                s.user_scores = "Scores/" + s.id + ".txt";
                s.cheat_scores = "Scores/" + s.id + "-cheat.txt";

                s.count = 0;

                while (s.count < MAXLVL && FileSystem.read_line(ref level_name, fin) != 0)
                {
                    s.level_name_v[s.count] = level_name;
                    s.count++;
                }

                FileSystem.fs_close(fin);

                return 1;
            }

            s.name = null;
            s.desc = null;
            s.id = null;
            s.shot = null;

            FileSystem.fs_close(fin);

            return 0;
        }


        public static void set_free(Set s)
        {
            int i;

            s.name = null;
            s.desc = null;
            s.id = null;
            s.shot = null;

            s.user_scores = null;
            s.cheat_scores = null;

            for (i = 0; i < s.count; i++)
                s.level_name_v[i] = null;
        }


        public static int set_exists(int i)
        {
            return (0 <= i && i < MyArray.array_len(sets)) ? 1 : 0;
        }


        public static string set_name(int i)
        {
            return set_exists(i) != 0 ? SET_GET(sets, i).name : null;
        }


        public static string set_desc(int i)
        {
            return set_exists(i) != 0 ? SET_GET(sets, i).desc : null;
        }


        public static string set_shot(int i)
        {
            return set_exists(i) != 0 ? SET_GET(sets, i).shot : null;
        }


        public static Score set_time_score(int i)
        {
            return set_exists(i) != 0 ? SET_GET(sets, i).time_score : null;
        }


        public static Score set_coin_score(int i)
        {
            return set_exists(i) != 0 ? SET_GET(sets, i).coin_score : null;
        }


        public static int set_level_exists(int i, int l)
        {
            return (l >= 0 && l < SET_GET(sets, i).count) ? 1 : 0;
        }


        public static Level get_level(int i)
        {
            return (i >= 0 && i < SET_GET(sets, curr).count) ? level_v[i] : null;
        }


        public static void set_rename_player(int score_rank, int times_rank, string player)
        {
            Set s = SET_GET(sets, curr);

            s.coin_score.player[score_rank] = player;
            s.time_score.player[times_rank] = player;
        }


        public static void set_score_update(int timer, int coins, ref int score_rank, ref int times_rank)
        {
            Set s = SET_GET(sets, curr);
            string player = Config.config_get_s(Config.CONFIG_PLAYER);

            Score.score_coin_insert(s.coin_score, ref score_rank, false, player, timer, coins);
            Score.score_time_insert(s.time_score, ref times_rank, false, player, timer, coins);
        }


        public static void set_quit()
        {
            int i;

            for (i = 0; i < MyArray.array_len(sets); i++)
                set_free((Set)MyArray.array_get(sets, i));

            MyArray.array_free(sets);
            sets = null;
        }


        static string[] roman = new string[]
        {
            "",
            "I",   "II",   "III",   "IV",   "V",
            "VI",  "VII",  "VIII",  "IX",   "X",
            "XI",  "XII",  "XIII",  "XIV",  "XV",
            "XVI", "XVII", "XVIII", "XIX",  "XX",
            "XXI", "XXII", "XXIII", "XXIV", "XXV"
        };
        public static void set_load_levels()
        {
            Level l;
            int nb = 1, bnb = 1;

            int i;

            for (i = 0; i < SET_GET(sets, curr).count; i++)
            {
                l = level_v[i];

                Level.level_load(SET_GET(sets, curr).level_name_v[i], l);

                l.set = SET_GET(sets, curr);
                l.number = i;

                if (l.is_bonus != 0)
                {
                    l.name = roman[bnb++];
                }
                else
                {
                    l.name = nb.ToString();
                    if (nb < 10)
                    {
                        l.name = "0" + l.name;
                    }
                    nb++;
                }

                l.is_locked = 1;
                l.is_completed = 0;
            }

            /* Unlock first level. */

            level_v[0].is_locked = 0;
        }


        public static int set_init()
        {
            IntPtr fin;
            string name = null;

            MyArray items;
            int i;

            if (Set.sets != null)
                Set.set_quit();

            Set.sets = MyArray.array_new();
            Set.curr = 0;

            /*
             * First, load the sets listed in the set file, preserving order.
             */

            if ((fin = FileSystem.fs_open(Set.SET_FILE)) != IntPtr.Zero)
            {
                while (FileSystem.read_line(ref name, fin) != 0)
                {
                    Set s = (Set)MyArray.array_add(Set.sets, new Set());

                    if (Set.set_load(s, name) == 0)
                        MyArray.array_del(Set.sets);

                    name = null;
                }
                FileSystem.fs_close(fin);
            }

            return MyArray.array_len(Set.sets);
        }


        static int get_score(IntPtr fp, Score s)
        {
            int j;
            int res = 1;
            string line = null;

            for (j = 0; j < Score.NSCORE && res != 0; j++)
            {
                line = null;
                res = FileSystem.fs_gets(ref line, Config.MAXSTR, fp) != null ? 1 : 0;
                if (res != 0)
                {
                    string[] arr = line.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    int i = 0;
                    int n;
                    if (i < arr.Length)
                    {
                        if (int.TryParse(arr[i], out n))
                        {
                            s.timer[j] = n;
                            i++;
                        }
                    }

                    if (i < arr.Length)
                    {
                        if (int.TryParse(arr[i], out n))
                        {
                            s.coins[j] = n;
                            i++;
                        }
                    }

                    if (i < arr.Length)
                    {
                        s.player[j] = arr[i];
                        i++;
                    }

                    res = i == 3 ? 1 : 0;
                }
            }

            return res;
        }


        /* Get the score of the set. */
        static void set_load_hs()
        {
            Set s = SET_GET(sets, curr);
            IntPtr fin;
            int i;
            int res = 0;
            Level l;
            string fn = Config.config_cheat() != 0 ? s.cheat_scores : s.user_scores;
            string states = null;

            if ((fin = FileSystem.fs_open(fn)) != IntPtr.Zero)
            {
                res = (FileSystem.fs_gets(ref states, MAXLVL + 2,//sizeof ("\n"),//sizeof (states),
                    fin) != null &&
                    states.Length - 1 == s.count) ? 1 : 0;

                for (i = 0; i < s.count && res != 0; i++)
                {
                    switch (states[i])
                    {
                        case 'L':
                            level_v[i].is_locked = 1;
                            level_v[i].is_completed = 0;
                            break;

                        case 'C':
                            level_v[i].is_locked = 0;
                            level_v[i].is_completed = 1;
                            break;

                        case 'O':
                            level_v[i].is_locked = 0;
                            level_v[i].is_completed = 0;
                            break;

                        default:
                            res = 0;
                            break;
                    }
                }

                res = (res != 0 &&
                    get_score(fin, s.time_score) != 0 &&
                    get_score(fin, s.coin_score) != 0) ? 1 : 0;

                for (i = 0; i < s.count && res != 0; i++)
                {
                    l = level_v[i];
                    res = (get_score(fin, l.best_times) != 0 &&
                        get_score(fin, l.fast_unlock) != 0 &&
                        get_score(fin, l.most_coins) != 0) ? 1 : 0;
                }

                FileSystem.fs_close(fin);
            }
        }


        public static void set_goto(int i)
        {
            curr = i;

            set_load_levels();
            set_load_hs();
        }
    }
}
