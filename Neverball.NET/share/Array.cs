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
    public class MyArray
    {
        object[] data;

        int elem_num;
        int data_len;


        public static MyArray array_new()
        {
            MyArray a;

            a = new MyArray();
            {
                a.data = new object[2];

                a.elem_num = 0;
                a.data_len = 2;
            }

            return a;
        }


        public static void array_free(MyArray a)
        {
            a.data = null;
        }


        public static object array_add(MyArray a, object item)
        {
            if ((a.elem_num + 1)
                 > a.data_len)
            {
                object[] new_data = new object[a.data_len *= 2];
                for (int i = 0; i < a.elem_num; i++)
                {
                    new_data[i] = a.data[i];
                }

                a.data = new_data;
            }

            a.data[a.elem_num] = item;

            return a.data[a.elem_num++];
        }


        public static void array_del(MyArray a)
        {
            a.elem_num--;
            a.data[a.elem_num] = null;
        }


        public static object array_get(MyArray a, int i)
        {
            return a.data[i];// * a.elem_len];
        }


        public static int array_len(MyArray a)
        {
            return a.elem_num;
        }
    }
}
