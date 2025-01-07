# Animation C#
This project showcases the fundamentals of creating smooth animations. The application features a moving rectangle that glides across the screen, illustrating key concepts such as frame independence and real-time rendering.



![011](https://github.com/user-attachments/assets/98898424-0885-4a80-9638-46ac8619f0da)



In this app, you'll learn how to manage timing, handle graphics rendering, and implement basic animation principles. Whether you're a beginner looking to understand the basics of animation or an experienced developer seeking a refresher, this project provides a hands-on approach to mastering animation techniques.


# Animation C# Code Walkthrough

Welcome to the Animation C# project! In this lesson, we will explore the code step by step, breaking down each part to understand how it works. This project demonstrates the fundamentals of creating smooth animations in a Windows Forms application using C#. By the end of this walkthrough, you will have a solid understanding of the code and the principles behind animation.

## Overview

Animation is the art of creating the illusion of motion by displaying a series of static images in quick succession. In our app, we animate a rectangle that moves to the right. To ensure smooth animation across different devices, we designed it to be **frame-independent**, meaning it runs consistently regardless of frame rate.

## License

This project is licensed under the MIT License, allowing you to use, copy, modify, and distribute the software freely, as long as you include the copyright notice.

## Code Breakdown

### Importing Necessary Libraries

```csharp
using System.Diagnostics;
```
We start by importing the `System.Diagnostics` namespace, which provides classes for debugging and tracing. This will help us print debug messages to the console.

### Namespace Declaration

```csharp
namespace Animation_CS
{
```
Here, we define a namespace called `Animation_CS`, which helps organize our code and avoid naming conflicts with other parts of the program.

### Class Definition

```csharp
public partial class Form1 : Form
{
```
We define a class named `Form1` that inherits from `Form`. This means that `Form1` is a type of window in our application. The `partial` keyword allows us to define the class across multiple files, if needed.

### Variable Declarations

```csharp
private BufferedGraphicsContext Context = new();
private BufferedGraphics? Buffer;
private Size MinimumMaxBufferSize = new (1280, 720);
private readonly Color BackgroundColor = Color.Black;
private readonly Brush RectangleBrush = new SolidBrush(Color.Chartreuse);
private readonly Brush FpsDisplayBrush = new SolidBrush(Color.MediumSpringGreen);
private readonly String FpsIdentifier = new(" FPS");
```
- **BufferedGraphicsContext Context**: This manages the buffering for smoother graphics rendering.
- **BufferedGraphics? Buffer**: This holds our graphics buffer.
- **Size MinimumMaxBufferSize**: Sets a minimum size for the buffer.
- **Color BackgroundColor**: Defines the background color of the form.
- **Brush RectangleBrush**: Specifies the color used to fill the rectangle.
- **Brush FpsDisplayBrush**: Defines the color for the FPS (frames per second) display.
- **String FpsIdentifier**: A string to identify the FPS display.

### Rectangle Structure

```csharp
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

    public readonly int GetNearestX() { return (int)Math.Round(X); }
    public readonly int GetNearestY() { return (int)Math.Round(Y); }
    public readonly int GetNearestWidth() { return (int)Math.Round(Width); }
    public readonly int GetNearestHeight() { return (int)Math.Round(Height); }
}
```
This structure represents a rectangle with double-precision coordinates and dimensions. It includes:
- **Constructor**: Initializes the rectangle's position and size.
- **GetNearestX/Y/Width/Height**: Methods that round the rectangle's attributes to the nearest integer values for drawing.

### Rectangle Instance

```csharp
private RectangleDouble Rectangle = new(0, 0, 256, 256);
```
Here, we create an instance of `RectangleDouble`, starting at position (0, 0) with a width and height of 256 pixels.

### Delta Time Structure

```csharp
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
}
```
This structure keeps track of the time between frames, which is crucial for smooth animations. It stores:
- **CurrentFrame**: The time of the current frame.
- **LastFrame**: The time of the last frame.
- **ElapsedTime**: The time difference between the two frames.

### Velocity Variable

```csharp
private readonly double Velocity = 64.0;
```
This variable determines how fast the rectangle moves, set to 64 pixels per second.

### Display Structure

```csharp
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
```
This structure is used to display text (like FPS) on the screen. It includes:
- **Location**: The position where the text will be displayed.
- **Text**: The actual text to display.
- **Font**: The font used for the text.

### Frame Counter Structure

```csharp
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
```
This structure tracks the number of frames rendered and the elapsed time, helping us calculate the FPS.

### Form Load Event

```csharp
private void Form1_Load(object sender, EventArgs e)
{
    InitializeApp();
    Debug.Print($"Running...{DateTime.Now}");
}
```
This method is called when the form loads. It initializes the application and prints a debug message to the console.

### Form Resize Event

```csharp
private void Form1_Resize(object sender, EventArgs e)
{
    if (WindowState != FormWindowState.Minimized)
    {
        ResizeFPS();
        ResizeRectangle();
        DisposeBuffer();
    }
}
```
This method is triggered when the form is resized. It adjusts the FPS display and rectangle size, and disposes of the buffer if the window is not minimized.

### Timer Tick Event

```csharp
private void timer1_Tick(object sender, EventArgs e)
{
    if (WindowState != FormWindowState.Minimized)
    {
        UpdateFrame();
        Invalidate(); // Calls OnPaint
    }
}
```
This event is triggered at regular intervals (every 10 ms). It updates the frame and calls the `OnPaint` method to redraw the form.

### OnPaint Method

```csharp
protected override void OnPaint(PaintEventArgs e)
{
    AllocateBuffer();
    DrawFrame();
    Buffer?.Render(e.Graphics);
    UpdateFrameCounter();
    base.OnPaint(e);
}
```
This method is called whenever the form needs to be redrawn. It allocates the buffer, draws the current frame, renders it on the form, and updates the frame counter.

### Update Frame Method

```csharp
private void UpdateFrame()
{
    UpdateDeltaTime();
    MoveRectangle();
}
```
This method updates the time since the last frame and moves the rectangle.

### Update Delta Time Method

```csharp
private void UpdateDeltaTime()
{
    DeltaTime.CurrentFrame = DateTime.Now;
    DeltaTime.ElapsedTime = DeltaTime.CurrentFrame - DeltaTime.LastFrame;
    DeltaTime.LastFrame = DeltaTime.CurrentFrame;
}
```
This method calculates the elapsed time since the last frame, updating the `DeltaTime` structure accordingly.

### Move Rectangle Method

```csharp
private void MoveRectangle()
{
    Rectangle.X += Velocity * DeltaTime.ElapsedTime.TotalSeconds;
    if (Rectangle.X > ClientRectangle.Right)
    {
        Rectangle.X = ClientRectangle.Left - Rectangle.Width;
    }
}
```
Here, we update the rectangle's position based on its velocity and wrap it around when it exits the screen.

### Initialize Buffer Method

```csharp
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
```
This method sets up the graphics buffer based on the screen size and allocates the buffer.

### Allocate Buffer Method

```csharp
private void AllocateBuffer()
{
    if (Buffer == null)
    {
        Buffer = Context.Allocate(CreateGraphics(), ClientRectangle);
        Buffer.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
        Buffer.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
    }
}
```
This method allocates the buffer for drawing graphics, ensuring that the graphics are rendered smoothly.

### Draw Frame Method

```csharp
private void DrawFrame()
{
    Buffer?.Graphics.Clear(BackgroundColor);
    Buffer?.Graphics.FillRectangle(RectangleBrush, Rectangle.GetNearestX(), Rectangle.GetNearestY(), Rectangle.GetNearestWidth(), Rectangle.GetNearestHeight());
    Buffer?.Graphics.DrawString(FpsDisplay.Text, FpsDisplay.Font, FpsDisplayBrush, FpsDisplay.Location);
}
```
This method clears the buffer, draws the rectangle, and displays the current FPS.

### Dispose Buffer Method

```csharp
private void DisposeBuffer()
{
    if (Buffer != null)
    {
        Buffer.Dispose();
        Buffer = null; // Set to null to avoid using a disposed object
    }
}
```
This method disposes of the graphics buffer to free up resources.

### Initialize Application Method

```csharp
private void InitializeApp()
{
    InitializeForm();
    Timer1.Interval = 10;
    Timer1.Start();
}
```
This method initializes the form and starts the timer for regular updates.

### Initialize Form Method

```csharp
private void InitializeForm()
{
    CenterToScreen();
    SetStyle(ControlStyles.UserPaint, true);
    SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
    Text = "Animation C# - Code with Joe";
    WindowState = FormWindowState.Maximized;
}
```
This method sets up the form's appearance and behavior, centering it on the screen and maximizing the window.

### Update Frame Counter Method

```csharp
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
```
This method updates the frame counter, calculating the FPS and updating the display text accordingly.

### Resize Rectangle Method

```csharp
private void ResizeRectangle()
{
    Rectangle.Y = ClientRectangle.Height / 2 - Rectangle.Height / 2;
}
```
This method centers the rectangle vertically in the client area of the form.

### Resize FPS Method

```csharp
private void ResizeFPS()
{
    FpsDisplay.Location = new Point(FpsDisplay.Location.X, ClientRectangle.Bottom - 75);
}
```
This method positions the FPS display at the bottom of the client area.

### Constructor

```csharp
public Form1()
{
    InitializeComponent();
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
    Buffer = Context.Allocate(CreateGraphics(), ClientRectangle);
    Buffer.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
    Buffer.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
}
```
The constructor initializes the form and sets up the graphics context and buffer.

## Conclusion

Congratulations! You've just completed a detailed walkthrough of the Animation C# project. We explored each part of the code, understanding how it works together to create a smooth animation of a rectangle moving across the screen. This project serves as a solid foundation for learning more about animation techniques and graphics programming in C#. Happy coding!


For more information and to access the complete code, visit the [GitHub repository](https://github.com/JoeLumbley/Animation-CS).



![012](https://github.com/user-attachments/assets/4a5ed26f-6d66-4b92-b824-4745f6d51275)
























