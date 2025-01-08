// Animation C#

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

        private Brush RectangleBrush = new SolidBrush(Color.Chartreuse);

        private Brush FpsDisplayBrush = new SolidBrush(Color.MediumSpringGreen);

        private readonly String FpsIdentifier = new(" FPS");

        // The RectangleDouble structure represents a rectangle with
        // double-precision coordinates and dimensions.
        public struct RectangleDouble
        {
            public double X, Y, Width, Height;

            public RectangleDouble(double x, double y, double width, double height)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }

            // Methods to round attributes to
            // the nearest integer values.
            public readonly int GetNearestX()
            {
                return (int)Math.Round(X);
            }
            public readonly int GetNearestY()
            {
                return (int)Math.Round(Y);
            }
            public readonly int GetNearestWidth()
            {
                return (int)Math.Round(Width);
            }
            public readonly int GetNearestHeight()
            {
                return (int)Math.Round(Height);
            }
        }

        private RectangleDouble Rectangle = new(0, 0, 256, 256);

        // The DeltaTimeStructure represents the time difference
        // between two frames.
        private struct DeltaTimeStructure
        {
            public DateTime CurrentFrame;
            public DateTime LastFrame;
            public TimeSpan ElapsedTime;

            public DeltaTimeStructure(DateTime currentFrame, DateTime lastFrame, TimeSpan elapsedTime)
            {
                CurrentFrame = currentFrame;
                LastFrame = lastFrame;
                ElapsedTime = elapsedTime;
            }

            public void Update()
            {   
                // Set the current frame's time to the current system time.
                CurrentFrame = DateTime.Now;

                // Calculates the elapsed time ( delta time Δt ) between the current frame
                // and the last frame.
                ElapsedTime = CurrentFrame - LastFrame;

                // Updates the last frame's time to the current frame's time for use in
                // the next update.
                LastFrame = CurrentFrame;

            }

        }

        private DeltaTimeStructure DeltaTime = new(DateTime.Now, DateTime.Now, TimeSpan.Zero);

        private readonly double Velocity = 64.0;

        private struct DisplayStructure
        {
            public Point Location;
            public string Text;
            public Font Font;

            public DisplayStructure(Point location, string text, Font font)
            {
                Location = location;
                Text = text;
                Font = font;
            }
        }

        private DisplayStructure FpsDisplay = new(new Point(0, 0), "--", new Font("Segoe UI", 25));

        private struct FrameCounterStructure
        {
            public int FrameCount;
            public DateTime StartTime;
            public TimeSpan TimeElapsed;
            public double SecondsElapsed;

            public FrameCounterStructure(int frameCount, DateTime startTime, TimeSpan timeElapsed, double secondsElapsed)
            {
                FrameCount = frameCount;
                StartTime = startTime;
                TimeElapsed = timeElapsed;
                SecondsElapsed = secondsElapsed;
            }
        }

        private FrameCounterStructure FrameCounter = new(0, DateTime.Now, TimeSpan.Zero, 0);

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeApp();

            Debug.Print($"Running...{DateTime.Now}");

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
            {
                ResizeFPS();

                ResizeRectangle();

                DisposeBuffer();

            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
            {
                UpdateFrame();

                Invalidate(); // Calls OnPaint

            }

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            AllocateBuffer();

            DrawFrame();

            // Show buffer on form.
            Buffer?.Render(e.Graphics);

            UpdateFrameCounter();

            base.OnPaint(e);

        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Intentionally left blank. Do not remove.

        }

        private void UpdateFrame()
        {
            DeltaTime.Update();

            MoveRectangle();

        }

        private void MoveRectangle()
        {
            // Move the rectangle to the right.
            Rectangle.X += Velocity * DeltaTime.ElapsedTime.TotalSeconds;
            // Displacement = Velocity x Delta Time ( Δs = V * Δt )

            // Wraparound
            // When the rectangle exits the right side of the client area.
            if (Rectangle.X > ClientRectangle.Right)
            {
                // The rectangle reappears on the left side the client area.
                Rectangle.X = ClientRectangle.Left - Rectangle.Width;

            }

        }

        private void InitializeBuffer()
        {
            Context = BufferedGraphicsManager.Current;

            if (Screen.PrimaryScreen != null)
            {
                Context.MaximumBuffer = Screen.PrimaryScreen.WorkingArea.Size;
            }
            else
            {
                Context.MaximumBuffer = MinimumMaxBufferSize;

                Debug.Print($"Primary screen not detected.");

            }

            AllocateBuffer();

        }

        private void AllocateBuffer()
        {
            if (Buffer == null)
            {
                Buffer = Context.Allocate(CreateGraphics(), ClientRectangle);
                Buffer.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                Buffer.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            }

        }

        private void DrawFrame()
        {
            Buffer?.Graphics.Clear(BackgroundColor);
            
            Buffer?.Graphics.FillRectangle(RectangleBrush,
                                           Rectangle.GetNearestX(), 
                                           Rectangle.GetNearestY(), 
                                           Rectangle.GetNearestWidth(), 
                                           Rectangle.GetNearestHeight());
            
            // Draw frames per second display.
            Buffer?.Graphics.DrawString(FpsDisplay.Text,
                                        FpsDisplay.Font,
                                        FpsDisplayBrush,
                                        FpsDisplay.Location);

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

        private void UpdateFrameCounter()
        {
            FrameCounter.TimeElapsed = DateTime.Now - FrameCounter.StartTime;

            FrameCounter.SecondsElapsed = FrameCounter.TimeElapsed.TotalSeconds;

            if (FrameCounter.SecondsElapsed < 1)
            {
                FrameCounter.FrameCount += 1;
            }
            else
            {
                FpsDisplay.Text = $"{FrameCounter.FrameCount}{FpsIdentifier}";

                FrameCounter.FrameCount = 0;

                FrameCounter.StartTime = DateTime.Now;

            }

        }

        private void ResizeRectangle()
        {
            // Center our rectangle vertically in the client area of our form.
            Rectangle.Y = ClientRectangle.Height / 2 - Rectangle.Height / 2;

        }

        private void ResizeFPS()
        {
            // Place the FPS display at the bottom of the client area.
            FpsDisplay.Location = new Point(FpsDisplay.Location.X,
                                            ClientRectangle.Bottom - 75);

        }

        public Form1()
        {
            InitializeComponent();

            // Initialize Buffer //

            // Set context to the context of this app.
            Context = BufferedGraphicsManager.Current;

            // Ensure that Screen.PrimaryScreen is not null.
            if (Screen.PrimaryScreen != null)
            {
                // Set buffer size to the primary working area.
                Context.MaximumBuffer = Screen.PrimaryScreen.WorkingArea.Size;
            }
            else
            {
                // Set to MinimumMaxBufferSize if PrimaryScreen is null
                Context.MaximumBuffer = MinimumMaxBufferSize;

                Debug.Print($"Primary screen not detected.");
            }
            Buffer = Context.Allocate(CreateGraphics(), ClientRectangle);
            Buffer.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            Buffer.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
        }





    }


}


// Monica is our an AI assistant.
// https://monica.im/