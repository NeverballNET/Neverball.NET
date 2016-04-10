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
    public enum trunc
    {
        TRUNC_NONE,
        TRUNC_HEAD,
        TRUNC_TAIL
    }



    class widget
    {
        public const int MAXWIDGET = 256;

        public const int GUI_TYPE = 0xFFFE;

        public const int GUI_FREE = 0;
        public const int GUI_STATE = 1;
        public const int GUI_HARRAY = 2;
        public const int GUI_VARRAY = 4;
        public const int GUI_HSTACK = 6;
        public const int GUI_VSTACK = 8;
        public const int GUI_FILLER = 10;
        public const int GUI_IMAGE = 12;
        public const int GUI_LABEL = 14;
        public const int GUI_COUNT = 16;
        public const int GUI_CLOCK = 18;
        public const int GUI_SPACE = 20;


        public int type;
        public int token;
        public int value;
        public int size;
        public int rect;

        public int x, y;
        public int w, h;
        public int car;
        public int cdr;

        public int text_img;
        public int text_obj;
        public int rect_obj;

        public float[] color0;
        public float[] color1;

        public float scale;

        public int text_obj_w;
        public int text_obj_h;

        public trunc trunc;


        public static readonly widget[] widgets;
        static widget()
        {
            widgets = new widget[MAXWIDGET];
            for (int i = 0; i < MAXWIDGET; i++)
            {
                widgets[i] = new widget();
            }
        }


        public static float[] gui_wht = new float[4] { 1.0f, 1.0f, 1.0f, 1.0f };
        public static float[] gui_yel = new float[4] { 1.0f, 1.0f, 0.0f, 1.0f };
        public static float[] gui_red = new float[4] { 1.0f, 0.0f, 0.0f, 1.0f };
        public static float[] gui_grn = new float[4] { 0.0f, 1.0f, 0.0f, 1.0f };
        public static float[] gui_blu = new float[4] { 0.0f, 0.0f, 1.0f, 1.0f };
        public static float[] gui_blk = new float[4] { 0.0f, 0.0f, 0.0f, 1.0f };
        public static float[] gui_gry = new float[4] { 0.3f, 0.3f, 0.3f, 1.0f };
    }



    class gui
    {
        public const string GUI_FACE = "ttf/DejaVuSans-Bold.ttf";

        public const int GUI_SML = 0;
        public const int GUI_MED = 1;
        public const int GUI_LRG = 2;

        public const int GUI_NW = 1;
        public const int GUI_SW = 2;
        public const int GUI_NE = 4;
        public const int GUI_SE = 8;

        public const int GUI_LFT = (GUI_NW | GUI_SW);
        public const int GUI_RGT = (GUI_NE | GUI_SE);
        public const int GUI_TOP = (GUI_NW | GUI_NE);
        public const int GUI_BOT = (GUI_SW | GUI_SE);
        public const int GUI_ALL = (GUI_TOP | GUI_BOT);


        public static int active;
        public static int radius;
        public static int[][] digit_text = new int[3][]
        {
            new int[11],
            new int[11],
            new int[11],
        };
        public static int[][] digit_list = new int[3][]
        {
            new int[11],
            new int[11],
            new int[11],
        };
        public static int[][] digit_w = new int[3][]
        {
            new int[11],
            new int[11],
            new int[11],
        };
        public static int[][] digit_h = new int[3][]
        {
            new int[11],
            new int[11],
            new int[11],
        };


        public static Alt.Sketch.Font[] font = new Alt.Sketch.Font[3];


        public static int gui_hot(int id)
        {
            return (widget.widgets[id].type & widget.GUI_STATE);
        }

        /*
         * Initialize a  display list  containing a  rectangle (x, y, w, h) to
         * which a  rendered-font texture  may be applied.   Colors  c0 and c1
         * determine the top-to-bottom color gradient of the text.
         */

        public static int gui_list(int x, int y,
                                     int w, int h, float[] c0, float[] c1)
        {
            int list = Video.GenLists(1);

            float s0, t0;
            float s1, t1;

            int W, H, ww, hh, d = h / 16;

            /* Assume the applied texture size is rect size rounded to power-of-two. */

            Image.image_size(out W, out H, w, h);

            ww = ((W - w) % 2) != 0 ? w + 1 : w;
            hh = ((H - h) % 2) != 0 ? h + 1 : h;

            s0 = 0.5f * (W - ww) / W;
            t0 = 0.5f * (H - hh) / H;
            s1 = 1.0f - s0;
            t1 = 1.0f - t0;

            Video.NewList(list, Video.COMPILE);
            {
                Video.Begin(Video.QUADS);
                {
                    Video.Color(0.0f, 0.0f, 0.0f, 0.5f);
                    Video.TexCoord2(s0, t1); Video.Vertex2(x + d, y - d);
                    Video.TexCoord2(s1, t1); Video.Vertex2(x + ww + d, y - d);
                    Video.TexCoord2(s1, t0); Video.Vertex2(x + ww + d, y + hh - d);
                    Video.TexCoord2(s0, t0); Video.Vertex2(x + d, y + hh - d);

                    Video.Color(c0);
                    Video.TexCoord2(s0, t1); Video.Vertex2(x, y);
                    Video.TexCoord2(s1, t1); Video.Vertex2(x + ww, y);

                    Video.Color(c1);
                    Video.TexCoord2(s1, t0); Video.Vertex2(x + ww, y + hh);
                    Video.TexCoord2(s0, t0); Video.Vertex2(x, y + hh);
                }
                Video.End();
            }
            Video.EndList();

            return list;
        }

        /*
         * Initialize a display list containing a rounded-corner rectangle (x,
         * y, w, h).  Generate texture coordinates to properly apply a texture
         * map to the rectangle as though the corners were not rounded.
         */

        public static int gui_rect(int x, int y, int w, int h, int f, int r)
        {
            int list = Video.GenLists(1);

            int n = 8;
            int i;

            Video.NewList(list, Video.COMPILE);
            {
                Video.Begin(Video.QUAD_STRIP);
                {
                    /* Left side... */

                    for (i = 0; i <= n; i++)
                    {
                        float a = (float)(MathHelper.PiOver2 * (float)i / (float)n);
                        float s = (float)(r * System.Math.Sin(a));
                        float c = (float)(r * System.Math.Cos(a));

                        float X = x + r - c;
                        float Ya = y + h + ((f & GUI_NW) != 0 ? (s - r) : 0);
                        float Yb = y + ((f & GUI_SW) != 0 ? (r - s) : 0);

                        Video.TexCoord2((X - x) / w, (Ya - y) / h);
                        Video.Vertex2(X, Ya);

                        Video.TexCoord2((X - x) / w, (Yb - y) / h);
                        Video.Vertex2(X, Yb);
                    }

                    /* ... Right side. */

                    for (i = 0; i <= n; i++)
                    {
                        float a = (float)(MathHelper.PiOver2 * (float)i / (float)n);
                        float s = (float)(r * System.Math.Sin(a));
                        float c = (float)(r * System.Math.Cos(a));

                        float X = x + w - r + s;
                        float Ya = y + h + ((f & GUI_NE) != 0 ? (c - r) : 0);
                        float Yb = y + ((f & GUI_SE) != 0 ? (r - c) : 0);

                        Video.TexCoord2((X - x) / w, (Ya - y) / h);
                        Video.Vertex2(X, Ya);

                        Video.TexCoord2((X - x) / w, (Yb - y) / h);
                        Video.Vertex2(X, Yb);
                    }
                }
                Video.End();
            }
            Video.EndList();

            return list;
        }


        public static int gui_widget(int pd, int type)
        {
            int id;

            /* Find an unused entry in the widget table. */

            for (id = 1; id < widget.MAXWIDGET; id++)
            {
                if (widget.widgets[id].type == widget.GUI_FREE)
                {
                    /* Set the type and default properties. */

                    widget.widgets[id].type = type;
                    widget.widgets[id].token = 0;
                    widget.widgets[id].value = 0;
                    widget.widgets[id].size = 0;
                    widget.widgets[id].rect = GUI_NW | GUI_SW | GUI_NE | GUI_SE;
                    widget.widgets[id].w = 0;
                    widget.widgets[id].h = 0;
                    widget.widgets[id].text_img = 0;
                    widget.widgets[id].text_obj = 0;
                    widget.widgets[id].rect_obj = 0;
                    widget.widgets[id].color0 = widget.gui_wht;
                    widget.widgets[id].color1 = widget.gui_wht;
                    widget.widgets[id].scale = 1.0f;
                    widget.widgets[id].trunc = trunc.TRUNC_NONE;

                    widget.widgets[id].text_obj_w = 0;
                    widget.widgets[id].text_obj_h = 0;

                    /* Insert the lnew widget into the parent's widget list. */

                    if (pd != 0)
                    {
                        widget.widgets[id].car = 0;
                        widget.widgets[id].cdr = widget.widgets[pd].car;
                        widget.widgets[pd].car = id;
                    }
                    else
                    {
                        widget.widgets[id].car = 0;
                        widget.widgets[id].cdr = 0;
                    }

                    return id;
                }
            }

            return 0;
        }


        public static int gui_harray(int pd) { return gui_widget(pd, widget.GUI_HARRAY); }
        public static int gui_varray(int pd) { return gui_widget(pd, widget.GUI_VARRAY); }
        public static int gui_hstack(int pd) { return gui_widget(pd, widget.GUI_HSTACK); }
        public static int gui_vstack(int pd) { return gui_widget(pd, widget.GUI_VSTACK); }
        public static int gui_filler(int pd) { return gui_widget(pd, widget.GUI_FILLER); }


        public static void gui_set_image(int id, string file)
        {
            if (GL.IsTexture(widget.widgets[id].text_img))
            {
                GL.DeleteTextures(1, ref widget.widgets[id].text_img);
                widget.widgets[id].text_img = 0;
            }

            widget.widgets[id].text_img = Image.make_image_from_file(file);
        }


        public static void gui_set_count(int id, int value)
        {
            widget.widgets[id].value = value;
        }


        public static void gui_set_clock(int id, int value)
        {
            widget.widgets[id].value = value;
        }


        public static void gui_set_color(int id, float[] c0, float[] c1)
        {
            if (id != 0)
            {
                c0 = c0 != null ? c0 : widget.gui_yel;
                c1 = c1 != null ? c1 : widget.gui_red;

                if (widget.widgets[id].color0 != c0 || widget.widgets[id].color1 != c1)
                {
                    widget.widgets[id].color0 = c0;
                    widget.widgets[id].color1 = c1;

                    if (GL.IsList(widget.widgets[id].text_obj))
                    {
                        int w, h;

                        GL.DeleteLists(widget.widgets[id].text_obj, 1);

                        w = widget.widgets[id].text_obj_w;
                        h = widget.widgets[id].text_obj_h;

                        widget.widgets[id].text_obj = gui_list(-w / 2, -h / 2, w, h,
                                                       widget.widgets[id].color0,
                                                       widget.widgets[id].color1);
                    }
                }
            }
        }


        public static void gui_set_trunc(int id, trunc trunc)
        {
            widget.widgets[id].trunc = trunc;
        }


        public static int gui_image(int pd, string file, int w, int h)
        {
            int id;

            if ((id = gui_widget(pd, widget.GUI_IMAGE)) != 0)
            {
                widget.widgets[id].text_img = Image.make_image_from_file(file);
                widget.widgets[id].w = w;
                widget.widgets[id].h = h;
            }
            return id;
        }


        public static int gui_count(int pd, int value, int size, int rect)
        {
            int i, id;

            if ((id = gui_widget(pd, widget.GUI_COUNT)) != 0)
            {
                for (i = value; i != 0; i /= 10)
                    widget.widgets[id].w += digit_w[size][0];

                widget.widgets[id].h = digit_h[size][0];
                widget.widgets[id].value = value;
                widget.widgets[id].size = size;
                widget.widgets[id].color0 = widget.gui_yel;
                widget.widgets[id].color1 = widget.gui_red;
                widget.widgets[id].rect = rect;
            }
            return id;
        }


        public static int gui_clock(int pd, int value, int size, int rect)
        {
            int id;

            if ((id = gui_widget(pd, widget.GUI_CLOCK)) != 0)
            {
                widget.widgets[id].w = digit_w[size][0] * 6;
                widget.widgets[id].h = digit_h[size][0];
                widget.widgets[id].value = value;
                widget.widgets[id].size = size;
                widget.widgets[id].color0 = widget.gui_yel;
                widget.widgets[id].color1 = widget.gui_red;
                widget.widgets[id].rect = rect;
            }
            return id;
        }


        public static int gui_space(int pd)
        {
            int id;

            if ((id = gui_widget(pd, widget.GUI_SPACE)) != 0)
            {
                widget.widgets[id].w = 0;
                widget.widgets[id].h = 0;
            }
            return id;
        }


        /*
         * The bottom-up pass determines the area of all widgets.  The minimum
         * width  and height of  a leaf  widget is  given by  the size  of its
         * contents.   Array  and  stack   widths  and  heights  are  computed
         * recursively from these.
         */

        static void gui_harray_up(int id)
        {
            int jd, c = 0;

            /* Find the widest child width and the highest child height. */

            for (jd = widget.widgets[id].car; jd != 0; jd = widget.widgets[jd].cdr)
            {
                gui_widget_up(jd);

                if (widget.widgets[id].h < widget.widgets[jd].h)
                    widget.widgets[id].h = widget.widgets[jd].h;
                if (widget.widgets[id].w < widget.widgets[jd].w)
                    widget.widgets[id].w = widget.widgets[jd].w;

                c++;
            }

            /* Total width is the widest child width times the child count. */

            widget.widgets[id].w *= c;
        }


        static void gui_varray_up(int id)
        {
            int jd, c = 0;

            /* Find the widest child width and the highest child height. */

            for (jd = widget.widgets[id].car; jd != 0; jd = widget.widgets[jd].cdr)
            {
                gui_widget_up(jd);

                if (widget.widgets[id].h < widget.widgets[jd].h)
                    widget.widgets[id].h = widget.widgets[jd].h;
                if (widget.widgets[id].w < widget.widgets[jd].w)
                    widget.widgets[id].w = widget.widgets[jd].w;

                c++;
            }

            /* Total height is the highest child height times the child count. */

            widget.widgets[id].h *= c;
        }


        static void gui_hstack_up(int id)
        {
            int jd;

            /* Find the highest child height.  Sum the child widths. */

            for (jd = widget.widgets[id].car; jd != 0; jd = widget.widgets[jd].cdr)
            {
                gui_widget_up(jd);

                if (widget.widgets[id].h < widget.widgets[jd].h)
                    widget.widgets[id].h = widget.widgets[jd].h;

                widget.widgets[id].w += widget.widgets[jd].w;
            }
        }


        static void gui_vstack_up(int id)
        {
            int jd;

            /* Find the widest child width.  Sum the child heights. */

            for (jd = widget.widgets[id].car; jd != 0; jd = widget.widgets[jd].cdr)
            {
                gui_widget_up(jd);

                if (widget.widgets[id].w < widget.widgets[jd].w)
                    widget.widgets[id].w = widget.widgets[jd].w;

                widget.widgets[id].h += widget.widgets[jd].h;
            }
        }


        static void gui_button_up(int id)
        {
            /* Store width and height for later use in text rendering. */

            widget.widgets[id].text_obj_w = widget.widgets[id].w;
            widget.widgets[id].text_obj_h = widget.widgets[id].h;

            if (widget.widgets[id].w < widget.widgets[id].h && widget.widgets[id].w > 0)
                widget.widgets[id].w = widget.widgets[id].h;

            /* Padded text elements look a little nicer. */

            if (widget.widgets[id].w < Config.config_get_d(Config.CONFIG_WIDTH))
                widget.widgets[id].w += radius;
            if (widget.widgets[id].h < Config.config_get_d(Config.CONFIG_HEIGHT))
                widget.widgets[id].h += radius;

            /* A button should be at least wide enough to accomodate the rounding. */

            if (widget.widgets[id].w < 2 * radius)
                widget.widgets[id].w = 2 * radius;
            if (widget.widgets[id].h < 2 * radius)
                widget.widgets[id].h = 2 * radius;
        }


        public static void gui_widget_up(int id)
        {
            if (id != 0)
                switch (widget.widgets[id].type & widget.GUI_TYPE)
                {
                    case widget.GUI_HARRAY: gui_harray_up(id); break;
                    case widget.GUI_VARRAY: gui_varray_up(id); break;
                    case widget.GUI_HSTACK: gui_hstack_up(id); break;
                    case widget.GUI_VSTACK: gui_vstack_up(id); break;
                    case widget.GUI_FILLER: break;
                    default: gui_button_up(id); break;
                }
        }


        /*
         * The  top-down layout  pass distributes  available area  as computed
         * during the bottom-up pass.  Widgets  use their area and position to
         * initialize rendering state.
         */

        static void gui_harray_dn(int id, int x, int y, int w, int h)
        {
            int jd, i = 0, c = 0;

            widget.widgets[id].x = x;
            widget.widgets[id].y = y;
            widget.widgets[id].w = w;
            widget.widgets[id].h = h;

            /* Count children. */

            for (jd = widget.widgets[id].car; jd != 0; jd = widget.widgets[jd].cdr)
                c += 1;

            /* Distribute horizontal space evenly to all children. */

            for (jd = widget.widgets[id].car; jd != 0; jd = widget.widgets[jd].cdr, i++)
            {
                int x0 = x + i * w / c;
                int x1 = x + (i + 1) * w / c;

                gui_widget_dn(jd, x0, y, x1 - x0, h);
            }
        }


        static void gui_varray_dn(int id, int x, int y, int w, int h)
        {
            int jd, i = 0, c = 0;

            widget.widgets[id].x = x;
            widget.widgets[id].y = y;
            widget.widgets[id].w = w;
            widget.widgets[id].h = h;

            /* Count children. */

            for (jd = widget.widgets[id].car; jd != 0; jd = widget.widgets[jd].cdr)
                c += 1;

            /* Distribute vertical space evenly to all children. */

            for (jd = widget.widgets[id].car; jd != 0; jd = widget.widgets[jd].cdr, i++)
            {
                int y0 = y + i * h / c;
                int y1 = y + (i + 1) * h / c;

                gui_widget_dn(jd, x, y0, w, y1 - y0);
            }
        }


        static void gui_hstack_dn(int id, int x, int y, int w, int h)
        {
            int jd, jx = x, jw = 0, c = 0;

            widget.widgets[id].x = x;
            widget.widgets[id].y = y;
            widget.widgets[id].w = w;
            widget.widgets[id].h = h;

            /* Measure the total width requested by non-filler children. */

            for (jd = widget.widgets[id].car; jd != 0; jd = widget.widgets[jd].cdr)
                if ((widget.widgets[jd].type & widget.GUI_TYPE) == widget.GUI_FILLER)
                    c += 1;
                else
                    jw += widget.widgets[jd].w;

            /* Give non-filler children their requested space.   */
            /* Distribute the rest evenly among filler children. */

            for (jd = widget.widgets[id].car; jd != 0; jd = widget.widgets[jd].cdr)
            {
                if ((widget.widgets[jd].type & widget.GUI_TYPE) == widget.GUI_FILLER)
                    gui_widget_dn(jd, jx, y, (w - jw) / c, h);
                else
                    gui_widget_dn(jd, jx, y, widget.widgets[jd].w, h);

                jx += widget.widgets[jd].w;
            }
        }


        static void gui_vstack_dn(int id, int x, int y, int w, int h)
        {
            int jd, jy = y, jh = 0, c = 0;

            widget.widgets[id].x = x;
            widget.widgets[id].y = y;
            widget.widgets[id].w = w;
            widget.widgets[id].h = h;

            /* Measure the total height requested by non-filler children. */

            for (jd = widget.widgets[id].car; jd != 0; jd = widget.widgets[jd].cdr)
                if ((widget.widgets[jd].type & widget.GUI_TYPE) == widget.GUI_FILLER)
                    c += 1;
                else
                    jh += widget.widgets[jd].h;

            /* Give non-filler children their requested space.   */
            /* Distribute the rest evenly among filler children. */

            for (jd = widget.widgets[id].car; jd != 0; jd = widget.widgets[jd].cdr)
            {
                if ((widget.widgets[jd].type & widget.GUI_TYPE) == widget.GUI_FILLER)
                    gui_widget_dn(jd, x, jy, w, (h - jh) / c);
                else
                    gui_widget_dn(jd, x, jy, w, widget.widgets[jd].h);

                jy += widget.widgets[jd].h;
            }
        }


        static void gui_filler_dn(int id, int x, int y, int w, int h)
        {
            /* Filler expands to whatever size it is given. */

            widget.widgets[id].x = x;
            widget.widgets[id].y = y;
            widget.widgets[id].w = w;
            widget.widgets[id].h = h;
        }


        static void gui_button_dn(int id, int x, int y, int w, int h)
        {
            /* Recall stored width and height for text rendering. */

            int W = widget.widgets[id].text_obj_w;
            int H = widget.widgets[id].text_obj_h;
            int R = widget.widgets[id].rect;

            float[] c0 = widget.widgets[id].color0;
            float[] c1 = widget.widgets[id].color1;

            widget.widgets[id].x = x;
            widget.widgets[id].y = y;
            widget.widgets[id].w = w;
            widget.widgets[id].h = h;

            /* Create display lists for the text area and rounded rectangle. */

            widget.widgets[id].text_obj = gui_list(-W / 2, -H / 2, W, H, c0, c1);
            widget.widgets[id].rect_obj = gui_rect(-w / 2, -h / 2, w, h, R, radius);
        }


        public static void gui_widget_dn(int id, int x, int y, int w, int h)
        {
            if (id != 0)
                switch (widget.widgets[id].type & widget.GUI_TYPE)
                {
                    case widget.GUI_HARRAY: gui_harray_dn(id, x, y, w, h); break;
                    case widget.GUI_VARRAY: gui_varray_dn(id, x, y, w, h); break;
                    case widget.GUI_HSTACK: gui_hstack_dn(id, x, y, w, h); break;
                    case widget.GUI_VSTACK: gui_vstack_dn(id, x, y, w, h); break;
                    case widget.GUI_FILLER: gui_filler_dn(id, x, y, w, h); break;
                    case widget.GUI_SPACE: gui_filler_dn(id, x, y, w, h); break;
                    default: gui_button_dn(id, x, y, w, h); break;
                }
        }


        public static void gui_pulse(int id, float k)
        {
            if (id != 0) widget.widgets[id].scale = k;
        }


        public static void gui_timer(int id, float dt)
        {
            int jd;

            if (id != 0)
            {
                for (jd = widget.widgets[id].car; jd != 0; jd = widget.widgets[jd].cdr)
                    gui_timer(jd, dt);

                if (widget.widgets[id].scale - 1.0f < dt)
                    widget.widgets[id].scale = 1.0f;
                else
                    widget.widgets[id].scale -= dt;
            }
        }


        static int gui_search(int id, int x, int y)
        {
            int jd, kd;

            /* Search the hierarchy for the widget containing the given point. */

            if (id != 0 &&
                (widget.widgets[id].x <= x && x < widget.widgets[id].x + widget.widgets[id].w &&
                       widget.widgets[id].y <= y && y < widget.widgets[id].y + widget.widgets[id].h))
            {
                if (gui_hot(id) != 0)
                    return id;

                for (jd = widget.widgets[id].car; jd != 0; jd = widget.widgets[jd].cdr)
                    if ((kd = gui_search(jd, x, y)) != 0)
                        return kd;
            }
            return 0;
        }


        /*
         * Activate a widget, allowing it  to behave as a normal state widget.
         * This may  be used  to create  image buttons, or  cause an  array of
         * widgets to behave as a single state widget.
         */
        public static int gui_active(int id, int token, int value)
        {
            widget.widgets[id].type |= widget.GUI_STATE;
            widget.widgets[id].token = token;
            widget.widgets[id].value = value;

            return id;
        }


        public static int gui_delete(int id)
        {
            if (id != 0)
            {
                /* Recursively delete all subwidgets. */

                gui_delete(widget.widgets[id].cdr);
                gui_delete(widget.widgets[id].car);

                /* Release any GL resources held by this widget. */

                if (GL.IsTexture(widget.widgets[id].text_img))
                {
                    GL.DeleteTextures(1, ref widget.widgets[id].text_img);
                    widget.widgets[id].text_img = 0;
                }

                if (GL.IsList(widget.widgets[id].text_obj))
                {
                    GL.DeleteLists(widget.widgets[id].text_obj, 1);
                    widget.widgets[id].text_obj = 0;
                }

                if (GL.IsList(widget.widgets[id].rect_obj))
                {
                    GL.DeleteLists(widget.widgets[id].rect_obj, 1);
                    widget.widgets[id].rect_obj = 0;
                }

                /* Mark this widget unused. */

                widget.widgets[id].type = widget.GUI_FREE;
                widget.widgets[id].text_img = 0;
                widget.widgets[id].text_obj = 0;
                widget.widgets[id].rect_obj = 0;
                widget.widgets[id].cdr = 0;
                widget.widgets[id].car = 0;
            }
            return 0;
        }


        static float[][] back = new float[4][]
        {
            new float[]{ 0.1f, 0.1f, 0.1f, 0.5f },             /* off and inactive    */
            new float[]{ 0.5f, 0.5f, 0.5f, 0.8f },             /* off and   active    */
            new float[]{ 1.0f, 0.7f, 0.3f, 0.5f },             /* on  and inactive    */
            new float[]{ 1.0f, 0.7f, 0.3f, 0.8f },             /* on  and   active    */
        };
        static void gui_paint_rect(int id, int st)
        {

            int jd, i = 0;

            /* Use the widget status to determine the background color. */

            if (gui_hot(id) != 0)
                i = st | (((widget.widgets[id].value) != 0 ? 2 : 0) |
                          ((id == active) ? 1 : 0));

            switch (widget.widgets[id].type & widget.GUI_TYPE)
            {
                case widget.GUI_IMAGE:
                case widget.GUI_SPACE:
                case widget.GUI_FILLER:
                    break;

                case widget.GUI_HARRAY:
                case widget.GUI_VARRAY:
                case widget.GUI_HSTACK:
                case widget.GUI_VSTACK:

                    /* Recursively paint all subwidgets. */

                    for (jd = widget.widgets[id].car; jd != 0; jd = widget.widgets[jd].cdr)
                        gui_paint_rect(jd, i);

                    break;

                default:

                    /* Draw a leaf's background, colored by widget state. */

                    Video.MODELVIEW_PushMatrix();
                    {
                        Video.MODELVIEW_Translate((float)(widget.widgets[id].x + widget.widgets[id].w / 2),
                                     (float)(widget.widgets[id].y + widget.widgets[id].h / 2), 0);

                        Video.Color(back[i]);
                        Video.CallList(widget.widgets[id].rect_obj);
                    }
                    Video.MODELVIEW_PopMatrix();

                    break;
            }
        }


        /*
         * During GUI layout, we make a bottom-up pass to determine total area
         * requirements for  the widget  tree.  We position  this area  to the
         * sides or center of the screen.  Finally, we make a top-down pass to
         * distribute this area to each widget.
         */

        public static void gui_layout(int id, int xd, int yd)
        {
            int x, y;

            int w, W = Config.config_get_d(Config.CONFIG_WIDTH);
            int h, H = Config.config_get_d(Config.CONFIG_HEIGHT);

            gui_widget_up(id);

            w = widget.widgets[id].w;
            h = widget.widgets[id].h;

            if (xd < 0) x = 0;
            else if (xd > 0) x = (W - w);
            else x = (W - w) / 2;

            if (yd < 0) y = 0;
            else if (yd > 0) y = (H - h);
            else y = (H - h) / 2;

            gui_widget_dn(id, x, y, w, h);

            /* Hilite the widget under the cursor, if any. */

            gui_point(id, -1, -1);
        }


        static void gui_paint_array(int id)
        {
            int jd;

            Video.MODELVIEW_PushMatrix();
            {
                float cx = widget.widgets[id].x + widget.widgets[id].w / 2.0f;
                float cy = widget.widgets[id].y + widget.widgets[id].h / 2.0f;
                float ck = widget.widgets[id].scale;

                Video.MODELVIEW_Translate(+cx, +cy, 0.0f);
                Video.MODELVIEW_Scale(ck, ck, ck);
                Video.MODELVIEW_Translate(-cx, -cy, 0.0f);

                /* Recursively paint all subwidgets. */

                for (jd = widget.widgets[id].car; jd != 0; jd = widget.widgets[jd].cdr)
                    gui_paint_text(jd);
            }
            Video.MODELVIEW_PopMatrix();
        }


        static void gui_paint_image(int id)
        {
            /* Draw the widget rect, textured using the image. */

            Video.MODELVIEW_PushMatrix();
            {
                Video.MODELVIEW_Translate((float)(widget.widgets[id].x + widget.widgets[id].w / 2),
                             (float)(widget.widgets[id].y + widget.widgets[id].h / 2), 0);

                Video.MODELVIEW_Scale(widget.widgets[id].scale,
                         widget.widgets[id].scale,
                         widget.widgets[id].scale);

                GL.BindTexture(TextureTarget.Texture2D, widget.widgets[id].text_img);
                Video.Color(widget.gui_wht);
                Video.CallList(widget.widgets[id].rect_obj);
            }
            Video.MODELVIEW_PopMatrix();
        }


        static void gui_paint_count(int id)
        {
            int j, i = widget.widgets[id].size;

            Video.MODELVIEW_PushMatrix();
            {
                Video.Color(widget.gui_wht);

                /* Translate to the widget center, and apply the pulse scale. */

                Video.MODELVIEW_Translate((float)(widget.widgets[id].x + widget.widgets[id].w / 2),
                             (float)(widget.widgets[id].y + widget.widgets[id].h / 2), 0);

                Video.MODELVIEW_Scale(widget.widgets[id].scale,
                         widget.widgets[id].scale,
                         widget.widgets[id].scale);

                if (widget.widgets[id].value > 0)
                {
                    /* Translate left by half the total width of the rendered value. */

                    for (j = widget.widgets[id].value; j != 0; j /= 10)
                        Video.MODELVIEW_Translate((float)+digit_w[i][j % 10] / 2.0f, 0.0f, 0.0f);

                    Video.MODELVIEW_Translate((float)-digit_w[i][0] / 2.0f, 0.0f, 0.0f);

                    /* Render each digit, moving right after each. */

                    for (j = widget.widgets[id].value; j != 0; j /= 10)
                    {
                        GL.BindTexture(TextureTarget.Texture2D, digit_text[i][j % 10]);
                        Video.CallList(digit_list[i][j % 10]);
                        Video.MODELVIEW_Translate((float)-digit_w[i][j % 10], 0.0f, 0.0f);
                    }
                }
                else if (widget.widgets[id].value == 0)
                {
                    /* If the value is zero, just display a zero in place. */

                    GL.BindTexture(TextureTarget.Texture2D, digit_text[i][0]);
                    Video.CallList(digit_list[i][0]);
                }
            }
            Video.MODELVIEW_PopMatrix();
        }


        static void gui_paint_clock(int id)
        {
            int i = widget.widgets[id].size;
            int mt = (widget.widgets[id].value / 6000) / 10;
            int mo = (widget.widgets[id].value / 6000) % 10;
            int st = ((widget.widgets[id].value % 6000) / 100) / 10;
            int so = ((widget.widgets[id].value % 6000) / 100) % 10;
            int ht = ((widget.widgets[id].value % 6000) % 100) / 10;
            int ho = ((widget.widgets[id].value % 6000) % 100) % 10;

            float dx_large = (float)digit_w[i][0];
            float dx_small = (float)digit_w[i][0] * 0.75f;

            if (widget.widgets[id].value < 0)
                return;

            Video.MODELVIEW_PushMatrix();
            {
                Video.Color(widget.gui_wht);

                /* Translate to the widget center, and apply the pulse scale. */

                Video.MODELVIEW_Translate((float)(widget.widgets[id].x + widget.widgets[id].w / 2),
                             (float)(widget.widgets[id].y + widget.widgets[id].h / 2), 0);

                Video.MODELVIEW_Scale(widget.widgets[id].scale,
                         widget.widgets[id].scale,
                         widget.widgets[id].scale);

                /* Translate left by half the total width of the rendered value. */

                if (mt > 0)
                    Video.MODELVIEW_Translate(-2.25f * dx_large, 0.0f, 0.0f);
                else
                    Video.MODELVIEW_Translate(-1.75f * dx_large, 0.0f, 0.0f);

                /* Render the minutes counter. */

                if (mt > 0)
                {
                    GL.BindTexture(TextureTarget.Texture2D, digit_text[i][mt]);
                    Video.CallList(digit_list[i][mt]);
                    Video.MODELVIEW_Translate(dx_large, 0.0f, 0.0f);
                }

                GL.BindTexture(TextureTarget.Texture2D, digit_text[i][mo]);
                Video.CallList(digit_list[i][mo]);
                Video.MODELVIEW_Translate(dx_small, 0.0f, 0.0f);

                /* Render the colon. */

                GL.BindTexture(TextureTarget.Texture2D, digit_text[i][10]);
                Video.CallList(digit_list[i][10]);
                Video.MODELVIEW_Translate(dx_small, 0.0f, 0.0f);

                /* Render the seconds counter. */

                GL.BindTexture(TextureTarget.Texture2D, digit_text[i][st]);
                Video.CallList(digit_list[i][st]);
                Video.MODELVIEW_Translate(dx_large, 0.0f, 0.0f);

                GL.BindTexture(TextureTarget.Texture2D, digit_text[i][so]);
                Video.CallList(digit_list[i][so]);
                Video.MODELVIEW_Translate(dx_small, 0.0f, 0.0f);

                /* Render hundredths counter half size. */

                Video.MODELVIEW_Scale(0.5f, 0.5f, 1.0f);

                GL.BindTexture(TextureTarget.Texture2D, digit_text[i][ht]);
                Video.CallList(digit_list[i][ht]);
                Video.MODELVIEW_Translate(dx_large, 0.0f, 0.0f);

                GL.BindTexture(TextureTarget.Texture2D, digit_text[i][ho]);
                Video.CallList(digit_list[i][ho]);
            }
            Video.MODELVIEW_PopMatrix();
        }


        static void gui_paint_label(int id)
        {
            /* Draw the widget text box, textured using the glyph. */

            Video.MODELVIEW_PushMatrix();
            {
                Video.MODELVIEW_Translate((float)(widget.widgets[id].x + widget.widgets[id].w / 2),
                             (float)(widget.widgets[id].y + widget.widgets[id].h / 2), 0);

                Video.MODELVIEW_Scale(widget.widgets[id].scale,
                         widget.widgets[id].scale,
                         widget.widgets[id].scale);

                GL.BindTexture(TextureTarget.Texture2D, widget.widgets[id].text_img);
                Video.CallList(widget.widgets[id].text_obj);
            }
            Video.MODELVIEW_PopMatrix();
        }


        static void gui_paint_text(int id)
        {
            switch (widget.widgets[id].type & widget.GUI_TYPE)
            {
                case widget.GUI_SPACE: break;
                case widget.GUI_FILLER: break;
                case widget.GUI_HARRAY: gui_paint_array(id); break;
                case widget.GUI_VARRAY: gui_paint_array(id); break;
                case widget.GUI_HSTACK: gui_paint_array(id); break;
                case widget.GUI_VSTACK: gui_paint_array(id); break;
                case widget.GUI_IMAGE: gui_paint_image(id); break;
                case widget.GUI_COUNT: gui_paint_count(id); break;
                case widget.GUI_CLOCK: gui_paint_clock(id); break;
                default: gui_paint_label(id); break;
            }
        }


        public static void gui_paint(int id)
        {
            if (id != 0)
            {
                Video.video_push_ortho();
                {
                    GL.Enable(EnableCap.ColorMaterial);
                    GL.Disable(EnableCap.Lighting);
                    GL.Disable(EnableCap.DepthTest);
                    {
                        GL.Disable(EnableCap.Texture2D);
                        gui_paint_rect(id, 0);

                        GL.Enable(EnableCap.Texture2D);
                        gui_paint_text(id);

                        Video.Color(widget.gui_wht);
                    }
                    GL.Enable(EnableCap.DepthTest);
                    GL.Enable(EnableCap.Lighting);
                    GL.Disable(EnableCap.ColorMaterial);
                }
                Video.video_pop_matrix();
            }
        }


        static int x_cache = 0;
        static int y_cache = 0;
        public static int gui_point(int id, int x, int y)
        {
            int jd;

            /* Reuse the last coordinates if (x,y) == (-1,-1) */

            if (x < 0 && y < 0)
                return gui_point(id, x_cache, y_cache);

            x_cache = x;
            y_cache = y;

            /* Short-circuit check the current active widget. */

            jd = gui_search(active, x, y);

            /* If not still active, search the hierarchy for a lnew active widget. */

            if (jd == 0)
                jd = gui_search(id, x, y);

            /* If the active widget has changed, return the lnew active id. */

            if (jd == 0 || jd == active)
                return 0;
            else
                return active = jd;
        }


        public static void gui_focus(int i)
        {
            active = i;
        }


        public static int gui_click()
        {
            return active;
        }


        public static int gui_token(int id)
        {
            return id != 0 ? widget.widgets[id].token : 0;
        }


        public static int gui_value(int id)
        {
            return id != 0 ? widget.widgets[id].value : 0;
        }


        public static void gui_toggle(int id)
        {
            widget.widgets[id].value = widget.widgets[id].value != 0 ? 0 : 1;
        }


        static int gui_vert_offset(int id, int jd)
        {
            /* Vertical offset between bottom of id and top of jd */

            return widget.widgets[id].y - (widget.widgets[jd].y + widget.widgets[jd].h);
        }


        static int gui_horz_offset(int id, int jd)
        {
            /* Horizontal offset between left of id and right of jd */

            return widget.widgets[id].x - (widget.widgets[jd].x + widget.widgets[jd].w);
        }


        static int gui_vert_dist(int id, int jd)
        {
            /* Vertical distance between the tops of id and jd */

            return System.Math.Abs((widget.widgets[id].y + widget.widgets[id].h) - (widget.widgets[jd].y + widget.widgets[jd].h));
        }


        static int gui_horz_dist(int id, int jd)
        {
            /* Horizontal distance between the left sides of id and jd */

            return System.Math.Abs(widget.widgets[id].x - widget.widgets[jd].x);
        }


        static int gui_stick_L(int id, int dd)
        {
            int jd, kd, hd;
            int o, omin, d, dmin;

            /* Find the closest "hot" widget to the left of dd (and inside id) */

            if (id != 0 &&
                gui_hot(id) != 0)
                return id;

            hd = 0;
            omin = widget.widgets[dd].x - widget.widgets[id].x + 1;
            dmin = widget.widgets[dd].y + widget.widgets[dd].h + widget.widgets[id].y + widget.widgets[id].h;

            for (jd = widget.widgets[id].car; jd != 0; jd = widget.widgets[jd].cdr)
            {
                kd = gui_stick_L(jd, dd);

                if (kd != 0 &&
                    kd != dd)
                {
                    o = gui_horz_offset(dd, kd);
                    d = gui_vert_dist(dd, kd);

                    if (0 <= o && o <= omin && d <= dmin)
                    {
                        hd = kd;
                        omin = o;
                        dmin = d;
                    }
                }
            }

            return hd;
        }


        static int gui_stick_R(int id, int dd)
        {
            int jd, kd, hd;
            int o, omin, d, dmin;

            /* Find the closest "hot" widget to the right of dd (and inside id) */

            if (id != 0 &&
                gui_hot(id) != 0)
                return id;

            hd = 0;
            omin = (widget.widgets[id].x + widget.widgets[id].w) - (widget.widgets[dd].x + widget.widgets[dd].w) + 1;
            dmin = widget.widgets[dd].y + widget.widgets[dd].h + widget.widgets[id].y + widget.widgets[id].h;

            for (jd = widget.widgets[id].car; jd != 0; jd = widget.widgets[jd].cdr)
            {
                kd = gui_stick_R(jd, dd);

                if (kd != 0 &&
                    kd != dd)
                {
                    o = gui_horz_offset(kd, dd);
                    d = gui_vert_dist(dd, kd);

                    if (0 <= o && o <= omin && d <= dmin)
                    {
                        hd = kd;
                        omin = o;
                        dmin = d;
                    }
                }
            }

            return hd;
        }


        static int gui_stick_D(int id, int dd)
        {
            int jd, kd, hd;
            int o, omin, d, dmin;

            /* Find the closest "hot" widget below dd (and inside id) */

            if (id != 0 &&
                gui_hot(id) != 0)
                return id;

            hd = 0;
            omin = widget.widgets[dd].y - widget.widgets[id].y + 1;
            dmin = widget.widgets[dd].x + widget.widgets[dd].w + widget.widgets[id].x + widget.widgets[id].w;

            for (jd = widget.widgets[id].car; jd != 0; jd = widget.widgets[jd].cdr)
            {
                kd = gui_stick_D(jd, dd);

                if (kd != 0 &&
                    kd != dd)
                {
                    o = gui_vert_offset(dd, kd);
                    d = gui_horz_dist(dd, kd);

                    if (0 <= o && o <= omin && d <= dmin)
                    {
                        hd = kd;
                        omin = o;
                        dmin = d;
                    }
                }
            }

            return hd;
        }


        static int gui_stick_U(int id, int dd)
        {
            int jd, kd, hd;
            int o, omin, d, dmin;

            /* Find the closest "hot" widget above dd (and inside id) */

            if (id != 0 &&
                gui_hot(id) != 0)
                return id;

            hd = 0;
            omin = (widget.widgets[id].y + widget.widgets[id].h) - (widget.widgets[dd].y + widget.widgets[dd].h) + 1;
            dmin = widget.widgets[dd].x + widget.widgets[dd].w + widget.widgets[id].x + widget.widgets[id].w;

            for (jd = widget.widgets[id].car; jd != 0; jd = widget.widgets[jd].cdr)
            {
                kd = gui_stick_U(jd, dd);

                if (kd != 0 &&
                    kd != dd)
                {
                    o = gui_vert_offset(kd, dd);
                    d = gui_horz_dist(dd, kd);

                    if (0 <= o && o <= omin && d <= dmin)
                    {
                        hd = kd;
                        omin = o;
                        dmin = d;
                    }
                }
            }

            return hd;
        }


        static int gui_wrap_L(int id, int dd)
        {
            int jd, kd;

            if ((jd = gui_stick_L(id, dd)) == 0)
                for (jd = dd; (kd = gui_stick_R(id, jd)) != 0; jd = kd)
                    ;

            return jd;
        }

        static int gui_wrap_R(int id, int dd)
        {
            int jd, kd;

            if ((jd = gui_stick_R(id, dd)) == 0)
                for (jd = dd; (kd = gui_stick_L(id, jd)) != 0; jd = kd)
                    ;

            return jd;
        }

        static int gui_wrap_U(int id, int dd)
        {
            int jd, kd;

            if ((jd = gui_stick_U(id, dd)) == 0)
                for (jd = dd; (kd = gui_stick_D(id, jd)) != 0; jd = kd)
                    ;

            return jd;
        }

        static int gui_wrap_D(int id, int dd)
        {
            int jd, kd;

            if ((jd = gui_stick_D(id, dd)) == 0)
                for (jd = dd; (kd = gui_stick_U(id, jd)) != 0; jd = kd)
                    ;

            return jd;
        }


        public static int gui_start(int pd, string text, int size, int token, int value)
        {
            int id;

            if ((id = gui_state(pd, text, size, token, value)) != 0)
                active = id;

            return id;
        }


        public static int gui_state(int pd, string text, int size, int token, int value)
        {
            int id;

            if ((id = gui_widget(pd, widget.GUI_STATE)) != 0)
            {
                widget.widgets[id].text_img = Image.make_image_from_font(//null, null,
                                                           ref widget.widgets[id].w,
                                                           ref widget.widgets[id].h,
                                                           text, font[size]);
                widget.widgets[id].size = size;
                widget.widgets[id].token = token;
                widget.widgets[id].value = value;
            }
            return id;
        }


        public static int gui_label(int pd, string text, int size, int rect, float[] c0, float[] c1)
        {
            int id;

            if ((id = gui_widget(pd, widget.GUI_LABEL)) != 0)
            {
                widget.widgets[id].text_img = Image.make_image_from_font(//null, null,
                                                           ref widget.widgets[id].w,
                                                           ref widget.widgets[id].h,
                                                           text, font[size]);
                widget.widgets[id].size = size;
                widget.widgets[id].color0 = c0 != null ? c0 : widget.gui_yel;
                widget.widgets[id].color1 = c1 != null ? c1 : widget.gui_red;
                widget.widgets[id].rect = rect;
            }
            return id;
        }


        /*
         * Create  a multi-line  text box  using a  vertical array  of labels.
         * Parse the  text for '\'  characters and treat them  as line-breaks.
         * Preserve the rect specification across the entire array.
         */

        public static int gui_multi(int pd, string text, int size, int rect, float[] c0, float[] c1)
        {
            int id = 0;

            if (!string.IsNullOrEmpty(text) &&// != null &&
                (id = gui_varray(pd)) != 0)
            {
                //  Set the curves for the first and last lines.

                while (text[text.Length - 1] == '\\')
                {
                    text = text.Remove(text.Length - 1);
                }
                string[] s = text.Split(new char[] { '\\' });
                int j = s.Length;

                int[] r = new int[j];

                if (j > 0)
                {
                    r[0] |= rect & (GUI_NW | GUI_NE);
                    r[j - 1] |= rect & (GUI_SW | GUI_SE);
                }

                //  Create a label widget for each line.

                for (int i = 0; i < j; i++)
                    gui_label(id, s[i], size, r[i], c0, c1);
            }

            return id;
        }


        public static void gui_init()
        {
            int w = Config.config_get_d(Config.CONFIG_WIDTH);
            int h = Config.config_get_d(Config.CONFIG_HEIGHT);

            int s = (h < w) ? h : w;

            //  Initialize font rendering
            {
                int s0 = s / 26;
                int s1 = s / 13;
                int s2 = s / 7;

                Alt.Sketch.PrivateFontCollection collection = new Alt.Sketch.PrivateFontCollection();
                collection.AddFontFile("Data/" + GUI_FACE);

                font[0] = new Alt.Sketch.Font(collection.Families[0], s0, Alt.Sketch.GraphicsUnit.Pixel);
                font[1] = new Alt.Sketch.Font(collection.Families[0], s1, Alt.Sketch.GraphicsUnit.Pixel);
                font[2] = new Alt.Sketch.Font(collection.Families[0], s2, Alt.Sketch.GraphicsUnit.Pixel);


                radius = s / 60;

                /* Initialize digit glyphs and lists for counters and clocks. */

                float[] c0 = widget.gui_yel;
                float[] c1 = widget.gui_red;
                for (int i = 0; i < 3; i++)
                {
                    /* Draw digits 0 through 9. */

                    int j;
                    for (j = 0; j < 10; j++)
                    {
                        string text = j.ToString();

                        digit_text[i][j] = Image.make_image_from_font(//null, null,
                                                                ref digit_w[i][j],
                                                                ref digit_h[i][j],
                                                                text, font[i]);
                        digit_list[i][j] = gui_list(-digit_w[i][j] / 2,
                                                    -digit_h[i][j] / 2,
                                                    +digit_w[i][j],
                                                    +digit_h[i][j], c0, c1);
                    }

                    /* Draw the colon for the clock. */

                    digit_text[i][j] = Image.make_image_from_font(//null, null,
                                                            ref digit_w[i][10],
                                                            ref digit_h[i][10],
                                                            ":", font[i]);
                    digit_list[i][j] = gui_list(-digit_w[i][10] / 2,
                                                -digit_h[i][10] / 2,
                                                +digit_w[i][10],
                                                +digit_h[i][10], c0, c1);
                }
            }

            active = 0;
        }


        public static void gui_free()
        {
            /* Release any remaining widget texture and display list indices. */

            for (int id = 1; id < widget.MAXWIDGET; id++)
            {
                if (GL.IsTexture(widget.widgets[id].text_img))
                {
                    GL.DeleteTextures(1, ref widget.widgets[id].text_img);
                    widget.widgets[id].text_img = 0;
                }

                if (GL.IsList(widget.widgets[id].text_obj))
                {
                    GL.DeleteLists(widget.widgets[id].text_obj, 1);
                    widget.widgets[id].text_obj = 0;
                }

                if (GL.IsList(widget.widgets[id].rect_obj))
                {
                    GL.DeleteLists(widget.widgets[id].rect_obj, 1);
                    widget.widgets[id].rect_obj = 0;
                }

                widget.widgets[id].type = widget.GUI_FREE;
                widget.widgets[id].text_img = 0;
                widget.widgets[id].text_obj = 0;
                widget.widgets[id].rect_obj = 0;
                widget.widgets[id].cdr = 0;
                widget.widgets[id].car = 0;
            }

            /* Release all digit textures and display lists. */

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 11; j++)
                {
                    if (GL.IsTexture(digit_text[i][j]))
                    {
                        GL.DeleteTextures(1, ref digit_text[i][j]);
                        digit_text[i][j] = 0;
                    }

                    if (GL.IsList(digit_list[i][j]))
                    {
                        GL.DeleteLists(digit_list[i][j], 1);
                        digit_list[i][j] = 0;
                    }
                }

            /* Release all loaded fonts and finalize font rendering. */

            if (font[GUI_LRG] != null)
                font[GUI_LRG] = null;
            if (font[GUI_MED] != null)
                font[GUI_MED] = null;
            if (font[GUI_SML] != null)
                font[GUI_SML] = null;
        }


        public static void gui_set_label(int id, string text)
        {
            int w = 0, h = 0;

            if (GL.IsTexture(widget.widgets[id].text_img))
            {
                GL.DeleteTextures(1, ref widget.widgets[id].text_img);
                widget.widgets[id].text_img = 0;
            }

            if (GL.IsList(widget.widgets[id].text_obj))
            {
                GL.DeleteLists(widget.widgets[id].text_obj, 1);
                widget.widgets[id].text_obj = 0;
            }

            text = gui_truncate(text, widget.widgets[id].w - radius,
                                font[widget.widgets[id].size],
                                widget.widgets[id].trunc);

            widget.widgets[id].text_img = Image.make_image_from_font(//null, null,
                ref w, ref h, text, font[widget.widgets[id].size]);
            widget.widgets[id].text_obj = gui_list(-w / 2, -h / 2, w, h,
                                           widget.widgets[id].color0, widget.widgets[id].color1);

            widget.widgets[id].text_obj_w = w;
            widget.widgets[id].text_obj_h = h;
        }


        public static void gui_set_multi(int id, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            //  Set the label value for each line.
            while (text[text.Length - 1] == '\\')
            {
                text = text.Remove(text.Length - 1);
            }
            string[] s = text.Split(new char[] { '\\' });
            int j = s.Length;

            for (int i = j - 1, jd = widget.widgets[id].car; i >= 0 && jd != 0; i--, jd = widget.widgets[jd].cdr)
                gui_set_label(jd, s[i]);
        }


        internal static Alt.Sketch.SizeI gui_measure(string text, Alt.Sketch.Font font)
        {
            return Alt.Sketch.Graphics.CreateDefaultGraphics().MeasureString(text, font).ToSizeI();
        }

        static string gui_trunc_head(string text, int maxwidth, Alt.Sketch.Font font)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            int left, right, mid;

            left = 0;
            right = text.Length;

            while (right - left > 1)
            {
                mid = (left + right) / 2;

                string str = "..." + text.Substring(mid);

                if (gui_measure(str, font).Width <= maxwidth)
                    right = mid;
                else
                    left = mid;
            }

            return ("..." + text.Substring(right));
        }

        static string gui_trunc_tail(string text,
                                    int maxwidth,
                                    Alt.Sketch.Font font)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }


            int left, right, mid;

            left = 0;
            right = text.Length;

            while (right - left > 1)
            {
                mid = (left + right) / 2;

                string str = text.Substring(0, mid) + "...";

                if (gui_measure(str, font).Width <= maxwidth)
                    left = mid;
                else
                    right = mid;
            }

            return (text.Substring(0, left) + "...");
        }

        static string gui_truncate(string text, int maxwidth, Alt.Sketch.Font font, trunc trunc)
        {
            if (gui_measure(text, font).Width <= maxwidth)
                return text;

            switch (trunc)
            {
                case trunc.TRUNC_NONE:
                    return text;
                case trunc.TRUNC_HEAD:
                    return gui_trunc_head(text, maxwidth, font);
                case trunc.TRUNC_TAIL:
                    return gui_trunc_tail(text, maxwidth, font);
            }

            return text;
        }
    }
}
