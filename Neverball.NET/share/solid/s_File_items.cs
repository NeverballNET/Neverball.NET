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


/*
 * Some might  be taken  aback at  the terseness of  the names  of the
 * structure  members and  the variables  used by  the  functions that
 * access them.  Yes, yes, I know:  readability.  I  contend that once
 * the naming  convention is embraced, the names  become more readable
 * than any  verbose alternative, and their brevity  and uniformity do
 * more to augment readability than longVariableNames ever could.
 *
 * Members  and variables  are named  XY.   X determines  the type  of
 * structure to which the variable  refers.  Y determines the usage of
 * the variable.
 *
 * The Xs are as documented by s_file:
 *
 *     f  File          (s_file)
 *     m  Material      (s_mtrl)
 *     v  Vertex        (s_vert)
 *     e  Edge          (s_edge)
 *     s  Side          (s_side)
 *     t  Texture coord (s_texc)
 *     g  Geometry      (s_geom)
 *     l  Lump          (s_lump)
 *     n  Node          (s_node)
 *     p  Path          (s_path)
 *     b  Body          (s_body)
 *     h  Item          (s_item)
 *     z  Goal          (s_goal)
 *     j  Jump          (s_jump)
 *     x  Switch        (s_swch)
 *     r  Billboard     (s_bill)
 *     u  User          (s_ball)
 *     w  Viewpoint     (s_view)
 *     d  Dictionary    (s_dict)
 *     i  Index         (int)
 *     a  Text          (char)
 *
 * The Ys are as follows:
 *
 *     c  Counter
 *     p  Pointer
 *     v  Vector (array)
 *     0  Index of the first
 *     i  Index
 *     j  Subindex
 *     k  Subsubindex
 *
 * Thus "up" is a pointer to  a user structure.  "lc" is the number of
 * lumps.  "ei" and "ej" are  edge indices into some "ev" edge vector.
 * An edge is  defined by two vertices, so  an edge structure consists
 * of "vi" and "vj".  And so on.
 *
 * Those members that do not conform to this convention are explicitly
 * documented with a comment.
 *
 * These prefixes are still available: c k o q y.
 */


namespace Neverball.NET
{
    class s_vert
    {
        public float[] m_p = new float[3];                                /* vertex position            */
    }


    class s_edge
    {
        public int m_vi;
        public int m_vj;
    }


    class s_side
    {
        public float[] m_n = new float[3];                                /* plane normal vector        */
        public float m_d;                                   /* distance from origin       */
    }


    class s_texc
    {
        public float[] m_u = new float[2];                                /* texture coordinate         */
    }


    class s_geom
    {
        public int m_mi;
        public int m_ti;
        public int m_si;
        public int m_vi;
        public int m_tj;
        public int m_sj;
        public int m_vj;
        public int m_tk;
        public int m_sk;
        public int m_vk;
    }


    class s_lump
    {
        public int m_fl;                                    /* lump flags                 */
        public int m_v0;
        public int m_vc;
        public int m_e0;
        public int m_ec;
        public int m_g0;
        public int m_gc;
        public int m_s0;
        public int m_sc;
    }


    class s_node
    {
        public int m_si;
        public int m_ni;
        public int m_nj;
        public int m_l0;
        public int m_lc;
    }


    class s_path
    {
        public float[] m_p = new float[3];                                /* starting position          */
        public float m_t;                                   /* travel time                */

        public int m_pi;
        public int m_f;                                     /* enable flag                */
        public int m_s;                                     /* smooth flag                */
    }


    class s_ball
    {
        public float[][] m_e = new float[3][]
        {
            new float[3],
            new float[3],
            new float[3]
        };                                      /* basis of orientation       */
        public float[] m_p = new float[3];        /* position vector            */
        public float[] m_v = new float[3];        /* velocity vector            */
        public float[] m_w = new float[3];        /* angular velocity vector    */
        public float m_r;                         /* radius                     */
    }


    class s_view
    {
        public float[] m_p = new float[3];
        public float[] m_q = new float[3];
    }


    class s_dict
    {
        public int m_ai;
        public int m_aj;
    }
}
