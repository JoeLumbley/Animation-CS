﻿// Animation C#

// Animation is the art of creating the illusion of motion by displaying a series
// of static images in quick succession. In our app, we use animation to make it
// appear as though our rectangle is moving towards the right.To ensure that our
// animation runs smoothly on all devices, we have designed it to be frame
// independent. This means that our animation is not affected by changes in the
// frame rate, ensuring a consistent and seamless experience for all users.

// MIT License
// Copyright(c) 2025 Joseph W. Lumbley

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software Is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included In all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

// https://github.com/JoeLumbley/Animation-CS



using System.Diagnostics;

namespace Animation_CS
{
    public partial class Form1 : Form
    {
        private BufferedGraphicsContext Context = new();

        private BufferedGraphics? Buffer;
        
        private readonly Size MinimumMaxBufferSize = new (1280, 720);

        private Color BackgroundColor = Color.Black;

        // The RectangleDouble structure represents a rectangle with
        // double-precision coordinates and dimensions.
        public struct RectangleDouble
        {
            public double X, Y, Width, Height, Velocity;
            public Brush Brush;

            public RectangleDouble(double x, double y, double width, double height,
                                   double velocity, Brush brush)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
                Velocity = velocity;
                Brush = brush;
            }

            // Methods to round attributes to
            // the nearest integer values.
            public readonly int GetNearestX()
            {
                return RoundToNearest(X);
            }
            public readonly int GetNearestY()
            {
                return RoundToNearest(Y);
            }
            public readonly int GetNearestWidth()
            {
                return RoundToNearest(Width);
            }
            public readonly int GetNearestHeight()
            {
                return RoundToNearest(Height);
            }

            // Generic method to round attributes to the nearest integer values.
            private readonly int RoundToNearest(double value)
            {
                return (int)Math.Round(value);
            }

            public void MoveRight(TimeSpan deltaTime)
            {
                // Move the rectangle to the right.
                X += Velocity * deltaTime.TotalSeconds;
                // Displacement = Velocity x Delta Time ( Δs = V * Δt )

            }

            public void Wraparound(Rectangle clientRectangle)
            {
                // When the rectangle exits the right side of the client area.
                if (X > clientRectangle.Right)
                {
                    // The rectangle reappears on the left side the client area.
                    X = clientRectangle.Left - Width;

                }

            }

            public void MoveRightAndWraparound(Rectangle clientRectangle,
                                               TimeSpan deltaTime)
            {
                MoveRight(deltaTime);

                Wraparound(clientRectangle);

            }

            public void CenterVertically(Rectangle clientRectangle)
            {
                // Center our rectangle vertically in the client area of our form.
                Y = clientRectangle.Height / 2 - Height / 2;

            }

        }

        private RectangleDouble Rectangle = new(0.0f, 0.0f, 256.0f, 256.0f, 32.0f,
                                                new SolidBrush(Color.Chartreuse));

        // The DeltaTimeStructure represents the time difference
        // between two frames.
        private struct DeltaTimeStructure
        {
            public DateTime CurrentFrame;
            public DateTime LastFrame;
            public TimeSpan ElapsedTime;

            public DeltaTimeStructure(DateTime currentFrame, DateTime lastFrame,
                                      TimeSpan elapsedTime)
            {
                CurrentFrame = currentFrame;
                LastFrame = lastFrame;
                ElapsedTime = elapsedTime;
            }

            public void Update()
            {   
                // Set the current frame's time to the current system time.
                CurrentFrame = DateTime.Now;

                // Calculates the elapsed time ( delta time Δt ) between the
                // current frame and the last frame.
                ElapsedTime = CurrentFrame - LastFrame;

                // Updates the last frame's time to the current frame's time for
                // use in the next update.
                LastFrame = CurrentFrame;

            }

        }

        private DeltaTimeStructure DeltaTime = new(DateTime.Now, DateTime.Now,
                                                   TimeSpan.Zero);

        private struct DisplayStructure
        {
            public Point Location;
            public string Text;
            public Font Font;
            public Brush Brush;

            public DisplayStructure(Point location, string text, Font font,
                                    Brush brush)
            {
                Location = location;
                Text = text;
                Font = font;
                Brush = brush;
            }

            public void MoveToPosition(Rectangle clientRectangle)
            {

                // Place the FPS display at the bottom of the client area.
                Location.X = Location.X;
                Location.Y = clientRectangle.Bottom - 75;

            }

        }

        private DisplayStructure FpsDisplay = new(new Point(0, 0),
                                                             "--",
                                         new Font("Segoe UI", 25),
                          new SolidBrush(Color.MediumSpringGreen));

        private struct FrameCounterStructure
        {
            public int FrameCount;
            public DateTime StartTime;
            public TimeSpan TimeElapsed;
            public String FPS;

            public FrameCounterStructure()
            {
                FrameCount = 0;
                StartTime = DateTime.Now;
                TimeElapsed = TimeSpan.Zero;
                FPS = "--";
            }

            public void Update()
            {
                TimeElapsed = DateTime.Now.Subtract(StartTime);

                if (TimeElapsed.TotalSeconds < 1)
                {
                    FrameCount += 1;
                }
                else
                {
                    FPS = $"{FrameCount} FPS";

                    FrameCount = 0;

                    StartTime = DateTime.Now;

                }

            }

        }

        private FrameCounterStructure FrameCounter = new();

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeApp();

            Debug.Print($"Running...{DateTime.Now}");

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
            {
                FpsDisplay.MoveToPosition(ClientRectangle);

                Rectangle.CenterVertically(ClientRectangle);

                DisposeBuffer();

                Timer1.Enabled = true;
            }
            else
            {
                Timer1.Enabled = false;

            }

        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            UpdateFrame();

            Invalidate(); // Calls OnPaint

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            AllocateBuffer();

            DrawFrame();

            // Show buffer on form.
            Buffer?.Render(e.Graphics);

            FrameCounter.Update();

            EraseFrame();

            base.OnPaint(e);

        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Intentionally left blank. Do not remove.

        }

        private void UpdateFrame()
        {
            DeltaTime.Update();

            Rectangle.MoveRightAndWraparound(ClientRectangle, 
                                             DeltaTime.ElapsedTime);

            FpsDisplay.Text = FrameCounter.FPS.ToString();

        }

        //private void InitializeBuffer()
        //{
        //    Context = BufferedGraphicsManager.Current;

        //    if (Screen.PrimaryScreen != null)
        //    {
        //        Context.MaximumBuffer = Screen.PrimaryScreen.WorkingArea.Size;
        //    }
        //    else
        //    {
        //        Context.MaximumBuffer = MinimumMaxBufferSize;

        //        Debug.Print($"Primary screen not detected.");

        //    }

        //    AllocateBuffer();

        //}

        private void AllocateBuffer()
        {
            if (Buffer == null)
            {
                Buffer = Context.Allocate(CreateGraphics(), ClientRectangle);

                Buffer.Graphics.CompositingMode = 
         System.Drawing.Drawing2D.CompositingMode.SourceOver;

                Buffer.Graphics.TextRenderingHint = 
              System.Drawing.Text.TextRenderingHint.AntiAlias;

                EraseFrame();

            }

        }

        private void DrawFrame()
        {
            // Draw rectangle.
            Buffer?.Graphics.FillRectangle(Rectangle.Brush,
                                           Rectangle.GetNearestX(), 
                                           Rectangle.GetNearestY(), 
                                           Rectangle.GetNearestWidth(), 
                                           Rectangle.GetNearestHeight());
            
            // Draw frames per second display.
            Buffer?.Graphics.DrawString(FpsDisplay.Text,
                                        FpsDisplay.Font,
                                        FpsDisplay.Brush,
                                        FpsDisplay.Location);

        }

        private void EraseFrame()
        {
            Buffer?.Graphics.Clear(BackgroundColor);

        }

        private void DisposeBuffer()
        {
            if (Buffer != null)
            {
                Buffer.Dispose();

                Buffer = null; // Set to null to avoid using a disposed object

                // The buffer will be reallocated in OnPaint

            }

        }

        private void InitializeApp()
        {
            InitializeForm();

            // InitializeBuffer();

            Timer1.Interval = 10;

            Timer1.Start();

        }

        private void InitializeForm()
        {
            CenterToScreen();

            SetStyle(ControlStyles.UserPaint, true);

            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            Text = "Animation C# - Code with Joe";

            WindowState = FormWindowState.Maximized;

        }

        public Form1()
        {
            InitializeComponent();

            // Initialize Buffer //

            // Set context to the context of this app.
            Context = BufferedGraphicsManager.Current;

            // Ensure that PrimaryScreen is not null.
            if (Screen.PrimaryScreen != null)
            {
                // Set buffer size to the primary working area size.
                Context.MaximumBuffer = Screen.PrimaryScreen.WorkingArea.Size;

            }
            else
            {
                // Set to MinimumMaxBufferSize if PrimaryScreen is null
                Context.MaximumBuffer = MinimumMaxBufferSize;

                Debug.Print($"Primary screen not detected.");

            }

            Buffer = Context.Allocate(CreateGraphics(), ClientRectangle);

            Buffer.Graphics.CompositingMode =
     System.Drawing.Drawing2D.CompositingMode.SourceOver;

            Buffer.Graphics.TextRenderingHint =
           System.Drawing.Text.TextRenderingHint.AntiAlias;

        }


    }

}


// Monica is our an AI assistant.
// https://monica.im/


// No sacrifice, no victory. - The Witwicky family motto

// or in Latin "absque sui detrimento non datur victoria"

// Victory is not granted without self-sacrifice
// A powerful reminder that every success often comes with its own set of
// challenges and sacrifices. It’s like a call to embrace the journey,
// with all its ups and downs.

