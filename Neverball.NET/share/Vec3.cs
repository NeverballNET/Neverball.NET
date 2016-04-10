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
    class Vec3
    {
        public const int A = 10;
        public const int B = 11;
        public const int C = 12;
        public const int D = 13;
        public const int E = 14;
        public const int F = 15;

        public const double TINY = 1e-5;
        

        public static float v_dot(float[] u, float[] v)
        {
            return ((u)[0] * (v)[0] + (u)[1] * (v)[1] + (u)[2] * (v)[2]);
        }


        public static float v_len(float[] u)
        {
            return (float)System.Math.Sqrt(v_dot(u, u));
        }


        public static void v_cpy(float[] u, float[] v)
        {
            (u)[0] = (v)[0];
            (u)[1] = (v)[1];
            (u)[2] = (v)[2];
        }

        public static void v_inv(float[] u, float[] v)
        {
            (u)[0] = -(v)[0];
            (u)[1] = -(v)[1];
            (u)[2] = -(v)[2];
        }

        public static void v_scl(float[] u, float[] v, float k)
        {
            (u)[0] = (v)[0] * (k);
            (u)[1] = (v)[1] * (k);
            (u)[2] = (v)[2] * (k);
        }

        public static void v_add(float[] u, float[] v, float[] w)
        {
            (u)[0] = (v)[0] + (w)[0];
            (u)[1] = (v)[1] + (w)[1];
            (u)[2] = (v)[2] + (w)[2];
        }

        public static void v_sub(float[] u, float[] v, float[] w)
        {
            (u)[0] = (v)[0] - (w)[0];
            (u)[1] = (v)[1] - (w)[1];
            (u)[2] = (v)[2] - (w)[2];
        }

        public static void v_mid(float[] u, float[] v, float[] w)
        {
            (u)[0] = ((v)[0] + (w)[0]) / 2;
            (u)[1] = ((v)[1] + (w)[1]) / 2;
            (u)[2] = ((v)[2] + (w)[2]) / 2;
        }

        public static void v_mad(float[] u, float[] p, float[] v, float t)
        {
            (u)[0] = (p)[0] + (v)[0] * (t);
            (u)[1] = (p)[1] + (v)[1] * (t);
            (u)[2] = (p)[2] + (v)[2] * (t);
        }


        public static void m_xps(float[] M, float[] N)
        {
            M[0] = N[0]; M[1] = N[4]; M[2] = N[8]; M[3] = N[C];
            M[4] = N[1]; M[5] = N[5]; M[6] = N[9]; M[7] = N[D];
            M[8] = N[2]; M[9] = N[6]; M[A] = N[A]; M[B] = N[E];
            M[C] = N[3]; M[D] = N[7]; M[E] = N[B]; M[F] = N[F];
        }


        public static void m_mult(float[] M, float[] N, float[] O)
        {
            M[0] = N[0] * O[0] + N[4] * O[1] + N[8] * O[2] + N[C] * O[3];
            M[1] = N[1] * O[0] + N[5] * O[1] + N[9] * O[2] + N[D] * O[3];
            M[2] = N[2] * O[0] + N[6] * O[1] + N[A] * O[2] + N[E] * O[3];
            M[3] = N[3] * O[0] + N[7] * O[1] + N[B] * O[2] + N[F] * O[3];

            M[4] = N[0] * O[4] + N[4] * O[5] + N[8] * O[6] + N[C] * O[7];
            M[5] = N[1] * O[4] + N[5] * O[5] + N[9] * O[6] + N[D] * O[7];
            M[6] = N[2] * O[4] + N[6] * O[5] + N[A] * O[6] + N[E] * O[7];
            M[7] = N[3] * O[4] + N[7] * O[5] + N[B] * O[6] + N[F] * O[7];

            M[8] = N[0] * O[8] + N[4] * O[9] + N[8] * O[A] + N[C] * O[B];
            M[9] = N[1] * O[8] + N[5] * O[9] + N[9] * O[A] + N[D] * O[B];
            M[A] = N[2] * O[8] + N[6] * O[9] + N[A] * O[A] + N[E] * O[B];
            M[B] = N[3] * O[8] + N[7] * O[9] + N[B] * O[A] + N[F] * O[B];

            M[C] = N[0] * O[C] + N[4] * O[D] + N[8] * O[E] + N[C] * O[F];
            M[D] = N[1] * O[C] + N[5] * O[D] + N[9] * O[E] + N[D] * O[F];
            M[E] = N[2] * O[C] + N[6] * O[D] + N[A] * O[E] + N[E] * O[F];
            M[F] = N[3] * O[C] + N[7] * O[D] + N[B] * O[E] + N[F] * O[F];
        }


        public static void v_nrm(float[] n, float[] v)
        {
            float d = v_len(v);

            n[0] = v[0] / d;
            n[1] = v[1] / d;
            n[2] = v[2] / d;
        }


        public static void m_rot(float[] M, float[] v, double a)
        {
            float[] u = new float[3];
            float[] U = new float[16];
            float[] S = new float[16];

            float s = (float)System.Math.Sin(a);
            float c = (float)System.Math.Cos(a);

            v_nrm(u, v);

            U[0] = u[0] * u[0]; U[4] = u[0] * u[1]; U[8] = u[0] * u[2];
            U[1] = u[1] * u[0]; U[5] = u[1] * u[1]; U[9] = u[1] * u[2];
            U[2] = u[2] * u[0]; U[6] = u[2] * u[1]; U[A] = u[2] * u[2];

            S[0] = 0; S[4] = -u[2]; S[8] = u[1];
            S[1] = u[2]; S[5] = 0; S[9] = -u[0];
            S[2] = -u[1]; S[6] = u[0]; S[A] = 0;

            M[0] = U[0] + c * (1 - U[0]) + s * S[0];
            M[1] = U[1] + c * (0 - U[1]) + s * S[1];
            M[2] = U[2] + c * (0 - U[2]) + s * S[2];
            M[3] = 0;
            M[4] = U[4] + c * (0 - U[4]) + s * S[4];
            M[5] = U[5] + c * (1 - U[5]) + s * S[5];
            M[6] = U[6] + c * (0 - U[6]) + s * S[6];
            M[7] = 0;
            M[8] = U[8] + c * (0 - U[8]) + s * S[8];
            M[9] = U[9] + c * (0 - U[9]) + s * S[9];
            M[A] = U[A] + c * (1 - U[A]) + s * S[A];
            M[B] = 0;
            M[C] = 0;
            M[D] = 0;
            M[E] = 0;
            M[F] = 1;
        }


        public static void m_vxfm(float[] v, float[] M, float[] w)
        {
            v[0] = (w[0] * M[0] + w[1] * M[4] + w[2] * M[8]);
            v[1] = (w[0] * M[1] + w[1] * M[5] + w[2] * M[9]);
            v[2] = (w[0] * M[2] + w[1] * M[6] + w[2] * M[A]);
        }


        public static void v_crs(float[] u, float[] v, float[] w)
        {
            u[0] = v[1] * w[2] - v[2] * w[1];
            u[1] = v[2] * w[0] - v[0] * w[2];
            u[2] = v[0] * w[1] - v[1] * w[0];
        }


        public static void m_basis(float[] M,
                     float[] e0,//[3],
                     float[] e1,//[3],
                     float[] e2)//[3])
        {
            M[0] = e0[0]; M[4] = e1[0]; M[8] = e2[0]; M[C] = 0;
            M[1] = e0[1]; M[5] = e1[1]; M[9] = e2[1]; M[D] = 0;
            M[2] = e0[2]; M[6] = e1[2]; M[A] = e2[2]; M[E] = 0;
            M[3] = 0; M[7] = 0; M[B] = 0; M[F] = 1;
        }


        public static void m_view(float[] M,
                    float[] c,
                    float[] p,
                    float[] u)
        {
            float[] x = new float[3];
            float[] y = new float[3];
            float[] z = new float[3];

            v_sub(z, p, c);
            v_nrm(z, z);
            v_crs(x, u, z);
            v_nrm(x, x);
            v_crs(y, z, x);

            m_basis(M, x, y, z);
        }
    }
}
