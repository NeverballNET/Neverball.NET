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
    class Score
    {
        public const int NSCORE = 3;

        public string[] player;

        public int[] timer;
        public int[] coins;


        public Score()
        {
            player = new string[NSCORE + 1];

            timer = new int[NSCORE + 1];
            coins = new int[NSCORE + 1];
        }


        public void Zero()
        {
            for (int i = 0; i < NSCORE + 1; i++)
            {
                for (int j = 0; j < Config.MAXNAM; j++)
                {
                    player[i] = null;
                }

                timer[i] = 0;
                coins[i] = 0;
            }
        }


        public static void score_insert(Score s, int i, string player, int timer, int coins)
        {
            s.player[i] = player;

            s.timer[i] = timer;
            s.coins[i] = coins;
        }


        public static void score_init_hs(Score s, int timer, int coins)
        {
            score_insert(s, 0, "Hard", timer, coins);
            score_insert(s, 1, "Medium", timer, coins);
            score_insert(s, 2, "Easy", timer, coins);
            score_insert(s, 3, "", timer, coins);
        }


        public static void score_swap(Score S, int i, int j)
        {
            string player = S.player[i];
            S.player[i] = S.player[j];
            S.player[j] = player;

            int tmp = S.timer[i];
            S.timer[i] = S.timer[j];
            S.timer[j] = tmp;

            tmp = S.coins[i];
            S.coins[i] = S.coins[j];
            S.coins[j] = tmp;
        }


        public static int score_time_comp(Score S, int i, int j)
        {
            if (S.timer[i] < S.timer[j])
                return 1;

            if (S.timer[i] == S.timer[j] && S.coins[i] > S.coins[j])
                return 1;

            return 0;
        }


        public static int score_coin_comp(Score S, int i, int j)
        {
            if (S.coins[i] > S.coins[j])
                return 1;

            if (S.coins[i] == S.coins[j] && S.timer[i] < S.timer[j])
                return 1;

            return 0;
        }


        public static void score_time_insert(Score s, ref int rank, bool rank_IsNull,
                               string player, int timer, int coins)
        {
            int i;

            score_insert(s, 3, player, timer, coins);

            if (!rank_IsNull)
            {
                for (i = 2; i >= 0 && score_time_comp(s, i + 1, i) != 0; i--)
                    score_swap(s, i + 1, i);

                rank = i + 1;
            }
        }


        public static void score_coin_insert(Score s, ref int rank, bool rank_IsNull,
                               string player, int timer, int coins)
        {
            int i;

            score_insert(s, 3, player, timer, coins);

            if (!rank_IsNull)
            {
                for (i = 2; i >= 0 && score_coin_comp(s, i + 1, i) != 0; i--)
                    score_swap(s, i + 1, i);

                rank = i + 1;
            }
        }
    }
}
