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

using OpenTK.Graphics.OpenGL;


namespace Neverball.NET
{
    class Geom
    {     
        public static void fade_draw(float k)
        {           
            if (k > 0.0f)
            {
                int w = Config.config_get_d(Config.CONFIG_WIDTH);
                int h = Config.config_get_d(Config.CONFIG_HEIGHT);

                Video.video_push_ortho();
                {
                    GL.Enable(EnableCap.ColorMaterial);
                    GL.Disable(EnableCap.Lighting);
                    GL.Disable(EnableCap.DepthTest);
                    GL.Disable(EnableCap.Texture2D);

                    Video.Color(0.0f, 0.0f, 0.0f, k);

                    Video.Begin(Video.QUADS);
                    {
                        Video.Vertex2(0, 0);
                        Video.Vertex2(w, 0);
                        Video.Vertex2(w, h);
                        Video.Vertex2(0, h);
                    }
                    Video.End();

                    Video.Color(1.0f, 1.0f, 1.0f, 1.0f);

                    GL.Enable(EnableCap.Texture2D);
                    GL.Enable(EnableCap.DepthTest);
                    GL.Enable(EnableCap.Lighting);
                    GL.Disable(EnableCap.ColorMaterial);
                }
                Video.video_pop_matrix();
            }
        }
    }
}
