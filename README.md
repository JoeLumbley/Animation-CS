# Animation C#
This project showcases the fundamentals of creating smooth animations. The application features a moving rectangle that glides across the screen, illustrating key concepts such as frame independence and real-time rendering.



![007](https://github.com/user-attachments/assets/6cacbcae-e99f-4d63-be0c-c95b051576ae)



In this app, you'll learn how to manage timing, handle graphics rendering, and implement basic animation principles. Whether you're a beginner looking to understand the basics of animation or an experienced developer seeking a refresher, this project provides a hands-on approach to mastering animation techniques.


# Code Walkthrough

Welcome to the Animation in C# tutorial! In this lesson, we'll walk through a C# code example that demonstrates how to create a simple animation of a rectangle moving across the screen. We'll break down the code line by line to ensure a clear understanding. Let's get started!

## Overview

Animation is the art of creating the illusion of motion by displaying a series of static images in quick succession. In our app, we animate a rectangle moving to the right. The animation is designed to be frame-independent, meaning it will run smoothly regardless of the device's frame rate.

## License

This project is licensed under the MIT License. You can use, copy, modify, and distribute this software freely, as long as you include the copyright notice.

## Code Breakdown

### Using Directives and Namespace

```csharp
using System.Diagnostics;

namespace Animation_CS
{
    ...
}
```

- **`using System.Diagnostics;`**: This directive allows us to use classes from the `System.Diagnostics` namespace, which includes tools for debugging.
- **`namespace Animation_CS`**: This defines a namespace called `Animation_CS` to organize our code and avoid naming conflicts.

### Class Definition

```csharp
public partial class Form1 : Form
{
    ...
}
```

- **`public partial class Form1 : Form`**: This defines a class named `Form1` that inherits from the `Form` class. This means `Form1` is a type of window in our application.

### Buffer Graphics Context

```csharp
private BufferedGraphicsContext context = new();
private BufferedGraphics? buffer;
private Size MinimumMaxBufferSize = new (1280, 720);
```

- **`BufferedGraphicsContext`**: This manages a buffer for drawing graphics, allowing smoother animations.
- **`buffer`**: This is a variable that will hold our graphics buffer.
- **`MinimumMaxBufferSize`**: This sets a minimum size for the buffer if the primary screen cannot be detected.

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

    public int GetNearestX() { ... }
    public int GetNearestY() { ... }
    public int GetNearestWidth() { ... }
    public int GetNearestHeight() { ... }
}
```

- **`RectangleDouble`**: This structure represents a rectangle with double-precision coordinates and dimensions.
- **Constructor**: Initializes the rectangle's position and size.
- **GetNearestX/Y/Width/Height**: These methods round the rectangle's attributes to the nearest integer values for drawing purposes.

### Rectangle Instance

```csharp
private RectangleDouble rectangle = new RectangleDouble(0, 0, 256, 256);
```

- This creates an instance of `RectangleDouble`, starting at position (0, 0) with a width and height of 256 pixels.

### Delta Time Structure

```csharp
private struct DeltaTimeStructure
{
    public DateTime CurrentFrame;
    public DateTime LastFrame;
    public TimeSpan ElapsedTime;

    public DeltaTimeStructure(DateTime currentFrame, DateTime lastFrame, TimeSpan elapsedTime)
    {
        ...
    }
}
```

- **`DeltaTimeStructure`**: This structure keeps track of the time between frames, which is crucial for smooth animations.

### Velocity Variable

```csharp
private double velocity = 64.0;
```

- **`velocity`**: This variable determines how fast the rectangle moves (64 pixels per second).

### Display Structure

```csharp
private struct DisplayStructure
{
    public Point Location;
    public string Text;
    public Font Font;

    public DisplayStructure(Point location, string text, Font font)
    {
        ...
    }
}
```

- **`DisplayStructure`**: This structure is used to display text (like FPS) on the screen.

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
        ...
    }
}
```

- **`FrameCounterStructure`**: This structure keeps track of the number of frames rendered and the elapsed time.

### Form Load Event

```csharp
private void Form1_Load(object sender, EventArgs e)
{
    InitializeApp();
    Debug.Print($"Running...{DateTime.Now}");
}
```

- **`Form1_Load`**: This event is triggered when the form is loaded. It initializes the application and prints a debug message.

### Resize Event

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

- **`Form1_Resize`**: This event is triggered when the form is resized. It adjusts the FPS display and rectangle size, and disposes of the buffer.

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

- **`timer1_Tick`**: This event is triggered at regular intervals (every 10 ms). It updates the frame and calls the `OnPaint` method to redraw the form.

### OnPaint Method

```csharp
protected override void OnPaint(PaintEventArgs e)
{
    AllocateBuffer();
    DrawFrame();
    buffer?.Render(e.Graphics);
    UpdateFrameCounter();
    base.OnPaint(e);
}
```

- **`OnPaint`**: This method is called whenever the form needs to be redrawn. It allocates the buffer, draws the current frame, and renders it on the form.

### Update Frame Method

```csharp
private void UpdateFrame()
{
    UpdateDeltaTime();
    MoveRectangle();
}
```

- **`UpdateFrame`**: This method updates the time since the last frame and moves the rectangle.

### Move Rectangle Method

```csharp
private void MoveRectangle()
{
    rectangle.X += velocity * deltaTime.ElapsedTime.TotalSeconds;

    if (rectangle.X > ClientRectangle.Right)
    {
        rectangle.X = ClientRectangle.Left - rectangle.Width;
    }
}
```

- **`MoveRectangle`**: This method updates the rectangle's position based on its velocity and wraps it around when it exits the screen.

### Initialize Buffer Method

```csharp
private void InitializeBuffer()
{
    context = BufferedGraphicsManager.Current;

    if (Screen.PrimaryScreen != null)
    {
        context.MaximumBuffer = Screen.PrimaryScreen.WorkingArea.Size;
    }
    else
    {
        context.MaximumBuffer = MinimumMaxBufferSize;
        Debug.Print($"Primary screen not detected.");
    }

    AllocateBuffer();
}
```

- **`InitializeBuffer`**: This method sets up the graphics buffer based on the screen size.

### Draw Frame Method

```csharp
private void DrawFrame()
{
    buffer?.Graphics.Clear(Color.Black);
    
    buffer?.Graphics.FillRectangle(Brushes.Purple,
                                   rectangle.GetNearestX(), 
                                   rectangle.GetNearestY(), 
                                   rectangle.GetNearestWidth(), 
                                   rectangle.GetNearestHeight());
    
    buffer?.Graphics.DrawString(fpsDisplay.Text + " FPS",
                                fpsDisplay.Font,
                                Brushes.MediumOrchid,
                                fpsDisplay.Location);
}
```

- **`DrawFrame`**: This method clears the buffer, draws the rectangle, and displays the current FPS.

### Dispose Buffer Method

```csharp
private void DisposeBuffer()
{
    if (buffer != null)
    {
        buffer.Dispose();
        buffer = null;
    }
}
```

- **`DisposeBuffer`**: This method disposes of the graphics buffer to free up resources.

### Initialize Application Method

```csharp
private void InitializeApp()
{
    InitializeForm();
    timer1.Interval = 10;
    timer1.Start();
}
```

- **`InitializeApp`**: This method initializes the form and starts the timer.

### Final Thoughts

This code provides a solid foundation for understanding basic animation concepts in C#. By following the structure and logic laid out in this tutorial, you can create your own animations and explore further enhancements. Happy coding!

## Repository

For the complete code and additional resources, visit [Joe Lumbley's Animation C# GitHub Repository](https://github.com/JoeLumbley/Animation-CS).






