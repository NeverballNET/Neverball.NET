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

using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;

using Alt;
using Alt.Collections.Generic;
using Alt.GUI;
using Alt.Sketch;


namespace Neverball.NET
{
    public class NeverballGameWindow : GameWindow
    {
        TPSCounter m_FPSCounter = new TPSCounter();
        public double FPS
        {
            get
            {
                return m_FPSCounter.TPS;
            }
        }


        bool m_Key_Alt_pressed = false;
        public bool IsAltPressed
        {
            get
            {
                return m_Key_Alt_pressed;
            }
        }


        public NeverballGameWindow()
            : this(800, 600)
        {
        }

        public NeverballGameWindow(int width, int height)
            : this(width, height, "Neverball.NET")
        {
        }

        public NeverballGameWindow(int width, int height, string title)
            : base(width, height)
        {
#if ALTSDK_1_0
            AltIntegration.Initialize();
#endif

            Title = title;
        }


        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == global::OpenTK.Input.Key.F4 &&
                IsAltPressed)
            {
                Exit();
                return;
            }

            if (e.Key == global::OpenTK.Input.Key.AltLeft ||
                e.Key == global::OpenTK.Input.Key.AltRight)
            {
                m_Key_Alt_pressed = true;
            }

            if (IsAltPressed &&
                e.Key == global::OpenTK.Input.Key.Enter)
            {
                if (WindowState == WindowState.Fullscreen)
                {
                    WindowState = WindowState.Normal;
                }
                else
                {
                    WindowState = WindowState.Fullscreen;
                }
            }


            int d = 1;
            switch (e.Key)
            {
                case global::OpenTK.Input.Key.Enter:
                    d = State.st_buttn(Config.config_get_d(Config.CONFIG_JOYSTICK_BUTTON_A), 1);
                    break;
                case global::OpenTK.Input.Key.Escape:
                    d = State.st_buttn(Config.config_get_d(Config.CONFIG_JOYSTICK_BUTTON_EXIT), 1);
                    break;

                default:
                    d = State.st_keybd(TranslateKeyCode(e.Key), 1);
                    break;
            }


            if (d == 0)
            {
                Exit();
            }
        }


        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.Key == global::OpenTK.Input.Key.AltLeft ||
                e.Key == global::OpenTK.Input.Key.AltRight)
            {
                m_Key_Alt_pressed = false;
            }


            int d = 1;
            switch (e.Key)
            {
                case global::OpenTK.Input.Key.Enter:
                    d = State.st_buttn(Config.config_get_d(Config.CONFIG_JOYSTICK_BUTTON_A), 0);
                    break;
                case global::OpenTK.Input.Key.Escape:
                    d = State.st_buttn(Config.config_get_d(Config.CONFIG_JOYSTICK_BUTTON_EXIT), 0);
                    break;

                default:
                    d = State.st_keybd(TranslateKeyCode(e.Key), 0);
                    break;
            }


            if (d == 0)
            {
                Exit();
            }
        }



        static Alt.GUI.MouseButtons ToMouseButtons(MouseButton mb)
        {
            Alt.GUI.MouseButtons buttons = MouseButtons.None;

            switch (mb)
            {
                case MouseButton.Left:
                    buttons = MouseButtons.Left;
                    break;
                case MouseButton.Right:
                    buttons = MouseButtons.Right;
                    break;
                case MouseButton.Middle:
                    buttons = MouseButtons.Middle;
                    break;
                case MouseButton.Button1:
                    buttons = MouseButtons.XButton1;
                    break;
                case MouseButton.Button2:
                    buttons = MouseButtons.XButton2;
                    break;
            }

            return buttons;
        }


        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            int d = State.st_click(ToMouseButtons(e.Button), 1);

            if (d == 0)
            {
                Exit();
            }
        }


        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            int d = State.st_click(ToMouseButtons(e.Button), 0);

            if (d == 0)
            {
                Exit();
            }
        }


        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);

            PointI mp = new PointI(e.X, e.Y);

            int xrel = mp.X - Video.LastMousePosition.X;
            int yrel = mp.Y - Video.LastMousePosition.Y;
            if (Video.video_get_grab() == 1)
            {
                if (mp == Video.LastMousePosition)
                {
                    return;
                }

                xrel = e.XDelta;
                yrel = e.YDelta;
            }

            Video.LastMousePosition = mp;

            State.st_point(+mp.X,
                     -mp.Y + Config.config_get_d(Config.CONFIG_HEIGHT),
                     +xrel,
                     Config.config_get_d(Config.CONFIG_MOUSE_INVERT) != 0
                     ? +yrel : -yrel);

            if (Video.video_get_grab() == 1)
            {
                Video.CenterMousePosition();
            }
        }



        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ball_main.Init();

            Config.config_set_d(Config.CONFIG_WIDTH, this.Width);
            Config.config_set_d(Config.CONFIG_HEIGHT, this.Height);
            Video.video_mode(Config.config_get_d(Config.CONFIG_WIDTH), Config.config_get_d(Config.CONFIG_HEIGHT));
            
            //TEMP  Video.Control = this;
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
#if ALTSDK_1_0
            Alt.AltIntegration.Tick();
#else
            Timer.SystemTick();
#endif

            base.OnUpdateFrame(e);
        }


        protected override void OnRenderFrame(FrameEventArgs e)
        {
            m_FPSCounter.Tick();

            CursorVisible = Video.video_get_grab() == 0;

            ball_main.DoCircle();
            
            GL.Flush();

            this.SwapBuffers();
        }


        /// <summary>
        /// Translates alphanumeric OpenTK key code to AltGUI character value.
        /// </summary>
        /// <param name="key">OpenTK key code.</param>
        /// <returns>Translated character.</returns>
        static Keys TranslateKeyChar(global::OpenTK.Input.Key key)
        {
            if (key >= global::OpenTK.Input.Key.A && key <= global::OpenTK.Input.Key.Z)
            {
                return (Keys.A + ((int)key - (int)global::OpenTK.Input.Key.A));
            }

            return Keys.Space;
        }


        System.Collections.Generic.Dictionary<global::OpenTK.Input.Key, Keys> m_TranslateKeyCodeDict;
        Keys TranslateKeyCode(global::OpenTK.Input.Key key)
        {
            if (m_TranslateKeyCodeDict == null)
            {
                m_TranslateKeyCodeDict = new System.Collections.Generic.Dictionary<global::OpenTK.Input.Key, Keys>();

                //     A key outside the known keys.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Unknown, Keys.Unknown);
                //     The left shift key (equivalent to ShiftLeft).
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.LShift, Keys.LShiftKey);
                //     The left shift key.
                //DUPLICATE m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.ShiftLeft, Keys.LeftShift);
                //     The right shift key (equivalent to ShiftRight).
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.RShift, Keys.RShiftKey);
                //     The right shift key.
                //DUPLICATE m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.ShiftRight, Keys.RightShift);
                //     The left control key (equivalent to ControlLeft).
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.LControl, Keys.LControlKey);
                //     The left control key.
                //DUPLICATE m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.ControlLeft, Keys.LeftControl);
                //     The right control key (equivalent to ControlRight).
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.RControl, Keys.RControlKey);
                //     The right control key.
                //DUPLICATE m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.ControlRight, Keys.RightControl);
                //     The left alt key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.AltLeft, Keys.LeftAlt);
                //     The left alt key (equivalent to AltLeft.
                //DUPLICATE m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.LAlt, Keys.);
                //     The right alt key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.AltRight, Keys.RightAlt);
                //     The right alt key (equivalent to AltRight).
                //DUPLICATE m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.RAlt, Keys.);
                //     The left win key.
                //DUPLICATE m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.WinLeft, Keys.);
                //     The left win key (equivalent to WinLeft).
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.LWin, Keys.LWin);
                //     The right win key (equivalent to WinRight).
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.RWin, Keys.RWin);
                //     The right win key.
                //DUPLICATE m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.WinRight, Keys.);
                //     The menu key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Menu, Keys.Menu);
                //     The F1 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F1, Keys.F1);
                //     The F2 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F2, Keys.F2);
                //     The F3 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F3, Keys.F3);
                //     The F4 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F4, Keys.F4);
                //     The F5 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F5, Keys.F5);
                //     The F6 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F6, Keys.F6);
                //     The F7 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F7, Keys.F7);
                //     The F8 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F8, Keys.F8);
                //     The F9 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F9, Keys.F9);
                //     The F10 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F10, Keys.F10);
                //     The F11 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F11, Keys.F11);
                //     The F12 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F12, Keys.F12);
                //     The F13 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F13, Keys.F13);
                //     The F14 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F14, Keys.F14);
                //     The F15 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F15, Keys.F15);
                //     The F16 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F16, Keys.F16);
                //     The F17 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F17, Keys.F17);
                //     The F18 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F18, Keys.F18);
                //     The F19 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F19, Keys.F19);
                //     The F20 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F20, Keys.F20);
                //     The F21 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F21, Keys.F21);
                //     The F22 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F22, Keys.F22);
                //     The F23 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F23, Keys.F23);
                //     The F24 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F24, Keys.F24);
                //     The F25 key.
                //m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F25, Keys.);
                //     The F26 key.
                //m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F26, Keys.);
                //     The F27 key.
                //m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F27, Keys.);
                //     The F28 key.
                //m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F28, Keys.);
                //     The F29 key.
                //m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F29, Keys.);
                //     The F30 key.
                //m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F30, Keys.);
                //     The F31 key.
                //m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F31, Keys.);
                //     The F32 key.
                //m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F32, Keys.);
                //     The F33 key.
                //m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F33, Keys.);
                //     The F34 key.
                //m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F34, Keys.);
                //     The F35 key.
                //m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F35, Keys.);
                //     The up arrow key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Up, Keys.Up);
                //     The down arrow key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Down, Keys.Down);
                //     The left arrow key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Left, Keys.Left);
                //     The right arrow key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Right, Keys.Right);
                //     The enter key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Enter, Keys.Enter);
                //     The escape key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Escape, Keys.Escape);
                //     The space key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Space, Keys.Space);
                //     The tab key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Tab, Keys.Tab);
                //     The backspace key (equivalent to BackSpace).
                //DUPLICATE m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Back, Keys.Back);
                //     The backspace key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.BackSpace, Keys.Backspace);
                //     The insert key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Insert, Keys.Insert);
                //     The delete key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Delete, Keys.Delete);
                //     The page up key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.PageUp, Keys.PageUp);
                //     The page down key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.PageDown, Keys.PageDown);
                //     The home key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Home, Keys.Home);
                //     The end key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.End, Keys.End);
                //     The caps lock key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.CapsLock, Keys.CapsLock);
                //     The scroll lock key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.ScrollLock, Keys.ScrollLock);
                //     The print screen key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.PrintScreen, Keys.PrintScreen);
                //     The pause key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Pause, Keys.Pause);
                //     The num lock key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.NumLock, Keys.NumLock);
                //     The clear key (Keypad5 with NumLock disabled, on typical keyboards).
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Clear, Keys.Clear);
                //     The sleep key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Sleep, Keys.Sleep);
                //     The keypad 0 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Keypad0, Keys.NumPad0);
                //     The keypad 1 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Keypad1, Keys.NumPad1);
                //     The keypad 2 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Keypad2, Keys.NumPad2);
                //     The keypad 3 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Keypad3, Keys.NumPad3);
                //     The keypad 4 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Keypad4, Keys.NumPad4);
                //     The keypad 5 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Keypad5, Keys.NumPad5);
                //     The keypad 6 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Keypad6, Keys.NumPad6);
                //     The keypad 7 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Keypad7, Keys.NumPad7);
                //     The keypad 8 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Keypad8, Keys.NumPad8);
                //     The keypad 9 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Keypad9, Keys.NumPad9);
                //     The keypad divide key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.KeypadDivide, Keys.Divide);
                //     The keypad multiply key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.KeypadMultiply, Keys.Multiply);
                //     The keypad minus key (equivalent to KeypadSubtract).
                //DUPLICATE m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.KeypadMinus, Keys.);
                //     The keypad subtract key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.KeypadSubtract, Keys.Subtract);
                //     The keypad add key.
                //DUPLICATE m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.KeypadAdd, Keys.Add);
                //     The keypad plus key (equivalent to KeypadAdd).
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.KeypadPlus, Keys.Plus);
                //     The keypad decimal key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.KeypadDecimal, Keys.Decimal);
                //     The keypad enter key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.KeypadEnter, Keys.Enter);
                //     The A key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.A, Keys.A);
                //     The B key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.B, Keys.B);
                //     The C key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.C, Keys.C);
                //     The D key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.D, Keys.D);
                //     The E key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.E, Keys.E);
                //     The F key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.F, Keys.F);
                //     The G key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.G, Keys.G);
                //     The H key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.H, Keys.H);
                //     The I key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.I, Keys.I);
                //     The J key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.J, Keys.J);
                //     The K key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.K, Keys.K);
                //     The L key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.L, Keys.L);
                //     The M key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.M, Keys.M);
                //     The N key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.N, Keys.N);
                //     The O key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.O, Keys.O);
                //     The P key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.P, Keys.P);
                //     The Q key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Q, Keys.Q);
                //     The R key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.R, Keys.R);
                //     The S key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.S, Keys.S);
                //     The T key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.T, Keys.T);
                //     The U key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.U, Keys.U);
                //     The V key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.V, Keys.V);
                //     The W key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.W, Keys.W);
                //     The X key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.X, Keys.X);
                //     The Y key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Y, Keys.Y);
                //     The Z key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Z, Keys.Z);
                //     The number 0 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Number0, Keys.D0);
                //     The number 1 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Number1, Keys.D1);
                //     The number 2 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Number2, Keys.D2);
                //     The number 3 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Number3, Keys.D3);
                //     The number 4 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Number4, Keys.D4);
                //     The number 5 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Number5, Keys.D5);
                //     The number 6 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Number6, Keys.D6);
                //     The number 7 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Number7, Keys.D7);
                //     The number 8 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Number8, Keys.D8);
                //     The number 9 key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Number9, Keys.D9);
                //     The tilde key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Tilde, Keys.Tilde);
                //     The minus key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Minus, Keys.Subtract);
                //     The plus key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Plus, Keys.Plus);
                //     The left bracket key (equivalent to BracketLeft).
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.LBracket, Keys.OpenBracket);
                //     The left bracket key.
                //DUPLICATE m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.BracketLeft, Keys.);
                //     The right bracket key.
                //DUPLICATE m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.BracketRight, Keys.);
                //     The right bracket key (equivalent to BracketRight).
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.RBracket, Keys.CloseBracket);
                //     The semicolon key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Semicolon, Keys.Semicolon);
                //     The quote key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Quote, Keys.Quotes);
                //     The comma key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Comma, Keys.Comma);
                //     The period key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Period, Keys.Period);
                //     The slash key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.Slash, Keys.Pipe);
                //     The backslash key.
                m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.BackSlash, Keys.Backslash);
                //     Indicates the last available keyboard key.
                //m_TranslateKeyCodeDict.Add(global::OpenTK.Input.Key.LastKey, Keys.);
            }

            Keys keys;
            if (m_TranslateKeyCodeDict.TryGetValue(key, out keys))
            {
                return keys;
            }

            return (Keys)key;
        }
    }
}
