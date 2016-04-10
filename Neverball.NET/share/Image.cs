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
using Alt.IO;
using Alt.Sketch;

using OpenTK.Graphics.OpenGL;


namespace Neverball.NET
{
    public class Image
    {
        public static void image_size(out int W, out int H, int w, int h)
        {
            //  Round the image size up to the next power-of-two.

            W = w != 0 ? 1 : 0;
            H = h != 0 ? 1 : 0;

            while (W < w) W *= 2;
            while (H < h) H *= 2;
        }


        //  Allocate and return a lnew down-sampled image buffer.
        static byte[] image_scale(byte[] p, int w, int h, int b, ref int wn, ref int hn, int n)
        {
            byte[] src = p;
            byte[] dst = null;

            int W = w / n;
            int H = h / n;

            dst = new byte[W * H * b];
            {
                int si, di;
                int sj, dj;
                int i;

                /* Iterate each component of each destination pixel. */

                for (di = 0; di < H; di++)
                    for (dj = 0; dj < W; dj++)
                        for (i = 0; i < b; i++)
                        {
                            int c = 0;

                            /* Average the NxN source pixel block for each. */

                            for (si = di * n; si < (di + 1) * n; si++)
                                for (sj = dj * n; sj < (dj + 1) * n; sj++)
                                    c += src[(si * w + sj) * b + i];

                            dst[(di * W + dj) * b + i] =
                                (byte)(c / (n * n));
                        }

                if (wn != 0) wn = W;
                if (hn != 0) hn = H;
            }

            return dst;
        }


        //  Whiten the RGB channels of the given image without touching any alpha.
        public static void image_white(byte[] p, int w, int h, int b)
        {
            byte[] s = p;

            int r;
            int c;

            for (r = 0; r < h; r++)
                for (c = 0; c < w; c++)
                {
                    int k = (r * w + c) * b;

                    s[k + 0] = 0xFF;

                    if (b > 2)
                    {
                        s[k + 1] = 0xFF;
                        s[k + 2] = 0xFF;
                    }
                }
        }


        public static byte[] image_load(string filename, out int width, out int height, out int bytes)
        {
            width = height = bytes = 0;

            string path = "data/" + filename;
            if (!VirtualFile.Exists(path))
            {
                return null;
            }

            byte[] p = null;

            Bitmap bitmap = Bitmap.FromFile(path);
            if (bitmap != null)
            {
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

                BitmapData bitmapData = bitmap.LockBits(bitmap.PixelRectangle, ImageLockMode.ReadOnly, Alt.Sketch.PixelFormat.Format32bppArgb);
                if (bitmapData != null)
                {
                    width = bitmapData.PixelWidth;
                    height = bitmapData.PixelHeight;
                    bytes = bitmapData.ByteDepth;

                    p = bitmapData.Scan0;

                    bitmap.UnlockBits(bitmapData);
                }
            }

            return p;
        }




        /*
         * Create an OpenGL texture object using the given image buffer.
         */
        static readonly PixelInternalFormat[] PixelInternalFormats = new PixelInternalFormat[]
        {
            (PixelInternalFormat)0,
            PixelInternalFormat.Luminance,
            PixelInternalFormat.LuminanceAlpha,
            PixelInternalFormat.Rgb,
            PixelInternalFormat.Rgba
        };
        static readonly OpenTK.Graphics.OpenGL.PixelFormat[] PixelFormats = new OpenTK.Graphics.OpenGL.PixelFormat[]
        {
            (OpenTK.Graphics.OpenGL.PixelFormat)0,
            OpenTK.Graphics.OpenGL.PixelFormat.Luminance,
            OpenTK.Graphics.OpenGL.PixelFormat.LuminanceAlpha,
            OpenTK.Graphics.OpenGL.PixelFormat.Rgb,
            OpenTK.Graphics.OpenGL.PixelFormat.Rgba
        };
        public static int make_texture(byte[] p, int w, int h, int b)
        {
            int o = 0;

            /* Scale the image as configured, or to fit the OpenGL limitations. */

#if GL_TEXTURE_MAX_ANISOTROPY_EXT
            int a = Config.config_get_d(Config.CONFIG_ANISO);
#endif
            int m = Config.config_get_d(Config.CONFIG_MIPMAP);
            int k = Config.config_get_d(Config.CONFIG_TEXTURES);
            int W = w;
            int H = h;

            byte[] q = null;

            int max;
            GL.GetInteger(GetPName.MaxTextureSize, out max);

            while (w / k > (int)max || h / k > (int)max)
                k *= 2;

            if (k > 1)
                q = image_scale(p, w, h, b, ref W, ref H, k);

            /* Generate and configure a lnew OpenGL texture. */

            GL.GenTextures(1, out o);
            GL.BindTexture(TextureTarget.Texture2D, o);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);

#if GL_GENERATE_MIPMAP_SGIS
            if (m)
            {
                GL.TexParameter(TextureTarget.Texture2D, Gl.GL_GENERATE_MIPMAP_SGIS, true);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                                Gl.GL_LINEAR_MIPMAP_LINEAR);
            }
#endif
#if GL_TEXTURE_MAX_ANISOTROPY_EXT
            if (a)
            {
                GL.TexParameter(TextureTarget.Texture2D, Gl.GL_TEXTURE_MAX_ANISOTROPY_EXT, a);
            }
#endif

            /* Copy the image to an OpenGL texture. */

            GL.TexImage2D(TextureTarget.Texture2D, 0,
                         PixelInternalFormats[b], W, H, 0,
                         PixelFormats[b], PixelType.UnsignedByte, q != null ? q : p);

            //if (q) free(q);

            return o;
        }


        //  Load an image from the named file.  Return an OpenGL texture object.
        public static int make_image_from_file(string filename)
        {
            byte[] p;
            int w;
            int h;
            int b;
            int o = 0;

            /* Load the image. */

            if ((p = Image.image_load(filename, out w, out h, out b)) != null)
            {
                o = make_texture(p, w, h, b);
                //free(p);
            }

            return o;
        }




        /*
         * Allocate and return a power-of-two image buffer with the given pixel buffer
         * centered within in.
         */
        static byte[] image_next2(byte[] src, int w, int h, int b, int transparent_left, ref int w2, ref int h2)
        {
            int W;
            int H;

            Image.image_size(out W, out H, w, h);

            //if ((dst = (unsigned char *) calloc(W * H * b, sizeof (unsigned char))))
            byte[] dst = new byte[W * H * b];
            {
                int r, dr = (H - h) / 2;
                int c, dc = (W - w) / 2 - (transparent_left / 2);
                int i;

                for (r = 0; r < h; ++r)
                    for (c = 0; c < w; ++c)
                        for (i = 0; i < b; ++i)
                        {
                            int R = r + dr;
                            int C = c + dc;

                            dst[(R * W + C) * b + i] = src[(r * w + c) * b + i];
                        }

                w2 = W;
                h2 = H;
            }

            return dst;
        }


        /*
         * Render the given  string using the given font.   Transfer the image
         * to a  surface of  power-of-2 size large  enough to fit  the string.
         * Return an OpenGL texture object.
         */
        //  Image
        public static int make_image_from_font(
                    ref int w, ref int h,
                    string text, Alt.Sketch.Font font)
        {
            int o = 0;

            /* Render the text. */

            if (!string.IsNullOrEmpty(text))
            {
                Alt.Sketch.SizeI size = gui.gui_measure(text, font);

                Alt.Sketch.Bitmap bitmap = new Alt.Sketch.Bitmap(size, Alt.Sketch.PixelFormat.Format32bppArgb);
                using (Alt.Sketch.Graphics graphics = Alt.Sketch.Graphics.FromImage(bitmap))
                {
                    graphics.Clear(Alt.Sketch.Color.Empty);
                    graphics.DrawString(text, font, Alt.Sketch.Brushes.White, Alt.Sketch.Point.Zero);
                }
                {
                    w = bitmap.PixelWidth;
                    h = bitmap.PixelHeight;

                    int trw = 0;
                    bool f = false;
                    //  left
                    for (int x = 0; x < w; x++)
                    {
                        for (int y = 0; y < h; y++)
                        {
                            if (bitmap.GetPixel(x, y).A != 0)
                            {
                                f = true;
                                break;
                            }
                        }

                        if (f)
                        {
                            break;
                        }

                        trw++;
                    }
                    
                    //  right
                    f = false;
                    for (int x = w - 1; x >= 0; x--)
                    {
                        for (int y = 0; y < h; y++)
                        {
                            if (bitmap.GetPixel(x, y).A != 0)
                            {
                                f = true;
                                break;
                            }
                        }

                        if (f)
                        {
                            break;
                        }

                        trw++;
                    }

                    //  Pad the text to power-of-two

                    Alt.Sketch.BitmapData bdata = bitmap.LockBits(Alt.Sketch.ImageLockMode.ReadOnly);

                    int w2 = 0;
                    int h2 = 0;
                    byte[] p = image_next2(bdata.Scan0, bdata.PixelWidth, bdata.PixelHeight, bdata.ByteDepth, trw, ref w2, ref h2);

                    bitmap.UnlockBits(bdata);


                    /* Saturate the color channels.  Modulate ONLY in alpha. */

                    Image.image_white(p, w2, h2, bitmap.ByteDepth);

                    /* Create the OpenGL texture object. */

                    o = Image.make_texture(p, w2, h2, bitmap.ByteDepth);
                }
            }
            else
            {
                /* Empty string. */

                //if (w) *
                w = 0;
                //if (h) *
                h = 0;
                //if (W) *W = 0;
                //if (H) *H = 0;
            }

            return o;
        }
    }
}
