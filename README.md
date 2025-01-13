# Animation C#
This project showcases the fundamentals of creating smooth animations. The application features a moving rectangle that glides across the screen, illustrating key concepts such as frame independence and real-time rendering.



![011](https://github.com/user-attachments/assets/98898424-0885-4a80-9638-46ac8619f0da)



In this app, you'll learn how to manage timing, handle graphics rendering, and implement basic animation principles. Whether you're a beginner looking to understand the basics of animation or an experienced developer seeking a refresher, this project provides a hands-on approach to mastering animation techniques.

---

# Code Walkthrough

Welcome to the Animation C# project! In this lesson, we will explore the code step by step, breaking down each part to understand how it works. This project demonstrates the fundamentals of creating smooth animations in a Windows Forms application using C#. By the end of this walkthrough, you will have a solid understanding of the code and the principles behind animation.

Animation is the art of creating the illusion of motion by displaying a series of static images in quick succession. In our app, we use animation to make it appear as though our rectangle is moving towards the right. To ensure that our animation runs smoothly on all devices, we have designed it to be frame-independent. This means that our animation is not affected by changes in the frame rate, ensuring a consistent and seamless experience for all users.



## The Breakdown



### Using Directives
```csharp
using System.Diagnostics;
```
- This line imports the `System.Diagnostics` namespace, which provides classes for debugging and tracing. It allows us to print debug messages to the console.

### Namespace Declaration
```csharp
namespace Animation_CS
{
```
- Here, we define a namespace called `Animation_CS`. Namespaces are used to organize code and avoid naming conflicts with other parts of the program.

### Class Definition
```csharp
public partial class Form1 : Form
{
```
- We define a class named `Form1` that inherits from `Form`. This means that `Form1` is a type of window in our application. The `partial` keyword allows us to define the class across multiple files if needed.

### Buffered Graphics Context
```csharp
private BufferedGraphicsContext Context = new();
private BufferedGraphics? Buffer;
```
- `Context` is an instance of `BufferedGraphicsContext`, which manages the buffering for smoother graphics rendering. 
- `Buffer` is a nullable variable that will hold our graphics buffer used to draw graphics off-screen before displaying them.

### Minimum Buffer Size
```csharp
private readonly Size MinimumMaxBufferSize = new(1280, 720);
```
- This line defines a constant size for the minimum maximum buffer size, setting it to 1280x720 pixels. This ensures our graphics can render properly on smaller screens.

### Background Color
```csharp
private Color BackgroundColor = Color.Black;
```
- This variable defines the background color of the form, set to black.

### RectangleDouble Structure
```csharp
public struct RectangleDouble
{
    public double X, Y, Width, Height, Velocity;
    public Brush Brush;
```
- We define a structure called `RectangleDouble` to represent a rectangle with double-precision coordinates and dimensions. 
- It includes properties for the rectangle's position (`X`, `Y`), size (`Width`, `Height`), movement speed (`Velocity`), and its color (`Brush`).

#### Constructor
```csharp
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
```
- This constructor initializes the rectangle's position, size, velocity, and color when a new instance of `RectangleDouble` is created.

#### GetNearest Methods
```csharp
public readonly int GetNearestX() { return RoundToNearest(X); }
public readonly int GetNearestY() { return RoundToNearest(Y); }
public readonly int GetNearestWidth() { return RoundToNearest(Width); }
public readonly int GetNearestHeight() { return RoundToNearest(Height); }
```
- These methods round the rectangle's attributes to the nearest integer values for drawing. This is important because pixel coordinates must be whole numbers.

#### RoundToNearest Method
```csharp
private readonly int RoundToNearest(double value)
{
    return (int)Math.Round(value);
}
```
- This generic method rounds a given double value to the nearest integer.

#### MoveRight Method
```csharp
public void MoveRight(TimeSpan deltaTime)
{
    X += Velocity * deltaTime.TotalSeconds;
}
```
- This method updates the rectangle's position based on its velocity and the time elapsed since the last frame (`deltaTime`). The formula used is:
  - **Displacement = Velocity x Delta Time** (Δs = V * Δt)

#### Wraparound Method
```csharp
public void Wraparound(Rectangle clientRectangle)
{
    if (X > clientRectangle.Right)
    {
        X = clientRectangle.Left - Width;
    }
}
```
- This method checks if the rectangle has exited the right side of the client area. If it has, it repositions the rectangle to the left side of the screen, creating a wraparound effect.

#### MoveRightAndWraparound Method
```csharp
public void MoveRightAndWraparound(Rectangle clientRectangle, TimeSpan deltaTime)
{
    MoveRight(deltaTime);
    Wraparound(clientRectangle);
}
```
- This method combines moving the rectangle to the right and checking for wraparound in one call.

#### CenterVertically Method
```csharp
public void CenterVertically(Rectangle clientRectangle)
{
    Y = clientRectangle.Height / 2 - Height / 2;
}
```
- This method centers the rectangle vertically in the client area of the form.

### Rectangle Instance
```csharp
private RectangleDouble Rectangle = new(0.0f, 0.0f, 256.0f, 256.0f, 32.0f,
                                        new SolidBrush(Color.Chartreuse));
```
- Here, we create an instance of `RectangleDouble`, starting at position (0, 0) with a width and height of 256 pixels, a velocity of 32 pixels per second, and a color of chartreuse.

### DeltaTimeStructure
```csharp
private struct DeltaTimeStructure
{
    public DateTime CurrentFrame;
    public DateTime LastFrame;
    public TimeSpan ElapsedTime;
```
- This structure keeps track of the time between frames, which is crucial for smooth animations. It includes:
  - `CurrentFrame`: The time of the current frame.
  - `LastFrame`: The time of the last frame.
  - `ElapsedTime`: The time difference between the two frames.

#### Update Method
```csharp
public void Update()
{
    CurrentFrame = DateTime.Now;
    ElapsedTime = CurrentFrame - LastFrame;
    LastFrame = CurrentFrame;
}
```
- This method updates the current frame time, calculates the elapsed time since the last frame, and updates the last frame's time for the next iteration.

### DisplayStructure
```csharp
private struct DisplayStructure
{
    public Point Location;
    public string Text;
    public Font Font;
    public Brush Brush;
```
- This structure is used to display text (like FPS) on the screen. It includes:
  - `Location`: The position where the text will be displayed.
  - `Text`: The actual text to display.
  - `Font`: The font used for the text.
  - `Brush`: The color of the text.

#### MoveToPosition Method
```csharp
public void MoveToPosition(Rectangle clientRectangle)
{
    Location = new Point(Location.X, clientRectangle.Bottom - 75);
}
```
- This method places the FPS display at the bottom of the client area.

### FrameCounterStructure
```csharp
private struct FrameCounterStructure
{
    public int FrameCount;
    public DateTime StartTime;
    public TimeSpan TimeElapsed;
    public String FPS;
```
- This structure tracks the number of frames rendered and the elapsed time, helping us calculate the FPS (frames per second).

#### Update Method
```csharp
public void Update()
{
    TimeSpan timeElapsed = DateTime.Now.Subtract(StartTime);
    if (timeElapsed.TotalSeconds < 1)
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
```
- This method updates the frame counter, calculating the FPS and updating the display text accordingly.

### Form Load Event
```csharp
private void Form1_Load(object sender, EventArgs e)
{
    InitializeApp();
    Debug.Print($"Running...{DateTime.Now}");
}
```
- This method is called when the form loads. It initializes the application and prints a debug message to the console.

### Form Resize Event
```csharp
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
```
- This method is triggered when the form is resized. It adjusts the FPS display and rectangle size, and disposes of the buffer if the window is not minimized.

### Timer Tick Event
```csharp
private void Timer1_Tick(object sender, EventArgs e)
{
    UpdateFrame();
    Invalidate(); // Calls OnPaint
}
```
- This event is triggered at regular intervals (every 10 ms). It updates the frame and calls the `OnPaint` method to redraw the form.

### OnPaint Method
```csharp
protected override void OnPaint(PaintEventArgs e)
{
    AllocateBuffer();
    DrawFrame();
    Buffer?.Render(e.Graphics);
    FrameCounter.Update();
    EraseFrame();
    base.OnPaint(e);
}
```
- This method is called whenever the form needs to be redrawn. It allocates the buffer, draws the current frame, renders it on the form, and updates the frame counter.

### OnPaintBackground Method
```csharp
protected override void OnPaintBackground(PaintEventArgs e)
{
    // Intentionally left blank. Do not remove.
}
```
- This method intentionally does nothing to prevent flickering during the redraw process.

### Update Frame Method
```csharp
private void UpdateFrame()
{
    DeltaTime.Update();
    Rectangle.MoveRightAndWraparound(ClientRectangle, DeltaTime.ElapsedTime);
    FpsDisplay.Text = FrameCounter.FPS.ToString();
}
```
- This method updates the time since the last frame and moves the rectangle.

### Allocate Buffer Method
```csharp
private void AllocateBuffer()
{
    if (Buffer == null)
    {
        Buffer = Context.Allocate(CreateGraphics(), ClientRectangle);
        Buffer.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
        Buffer.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
        EraseFrame();
    }
}
```
- This method allocates the buffer for drawing graphics, ensuring that the graphics are rendered smoothly.

### Draw Frame Method
```csharp
private void DrawFrame()
{
    Buffer?.Graphics.FillRectangle(Rectangle.Brush,
                                   Rectangle.GetNearestX(), 
                                   Rectangle.GetNearestY(), 
                                   Rectangle.GetNearestWidth(), 
                                   Rectangle.GetNearestHeight());
    
    Buffer?.Graphics.DrawString(FpsDisplay.Text,
                                FpsDisplay.Font,
                                FpsDisplay.Brush,
                                FpsDisplay.Location);
}
```
- This method clears the buffer, draws the rectangle, and displays the current FPS.

### Erase Frame Method
```csharp
private void EraseFrame()
{
    Buffer?.Graphics.Clear(BackgroundColor);
}
```
- This method clears the buffer to prepare for the next frame.

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
- This method disposes of the graphics buffer to free up resources.

### Initialize Application Method
```csharp
private void InitializeApp()
{
    InitializeForm();
    Timer1.Interval = 10;
    Timer1.Start();
}
```
- This method initializes the form and starts the timer for regular updates.

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
- This method sets up the form's appearance and behavior, centering it on the screen and maximizing the window.

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
- The constructor initializes the form and sets up the graphics context and buffer.


Congratulations! You’ve just completed a detailed walkthrough of the Animation C# project. We explored each part of the code, understanding how it works together to create a smooth animation of a rectangle moving across the screen. This project serves as a solid foundation for learning more about animation techniques and graphics programming in C#. Feel free to experiment with different values and see how they affect the animation! Happy coding!






---


# Exercises

Here are some exercises to enhance your understanding of the Animation C# project by modifying various parameters:

1. **Change the Rectangle's Color**
   - **Task**: Modify the `RectangleBrush` variable to change the color of the rectangle.
   - **Instructions**:
     ```csharp
     private readonly Brush RectangleBrush = new SolidBrush(Color.Red); // Change Color.Chartreuse to Color.Red
     ```

2. **Change the Rectangle's Size**
   - **Task**: Adjust the dimensions of the rectangle by modifying the `Rectangle` instance.
   - **Instructions**:
     ```csharp
     private RectangleDouble Rectangle = new(0, 0, 128, 128); // Change width and height to 128
     ```

3. **Change the Rectangle's Velocity**
   - **Task**: Modify the `Velocity` variable to change how fast the rectangle moves across the screen.
   - **Instructions**:
     ```csharp
     private readonly double Velocity = 100.0; // Change from 64.0 to 100.0 for faster movement
     ```

4. **Add a Random Color Change on Wraparound**
   - **Task**: Implement functionality to change the rectangle's color randomly on Wraparound.
   - **Instructions**:
     ```csharp
     
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
     
                // Change color randomly
                Random rand = new Random();
                RectangleBrush = new SolidBrush(Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256)));
     
            }

        }

     ```

5. **Implement Rectangle Resizing on Key Press**
   - **Task**: Allow the user to resize the rectangle using keyboard input (e.g., increase size with the Up arrow and decrease with the Down arrow).
   - **Instructions**:
     ```csharp
     protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
     {
         if (keyData == Keys.Up)
         {
             Rectangle.Width += 10;
             Rectangle.Height += 10;
         }
         else if (keyData == Keys.Down)
         {
             Rectangle.Width -= 10;
             Rectangle.Height -= 10;
         }
         return base.ProcessCmdKey(ref msg, keyData);
     }
     ```


These exercises will help you explore the flexibility of the Animation C# project and deepen your understanding of graphics programming in C#. Feel free to experiment with different values and see how they affect the animation!


----

# More on DeltaTime

### What is DeltaTime?

**DeltaTime** refers to the time difference between the current frame and the last frame in a game or animation loop. It is crucial for creating smooth and consistent animations, especially in real-time applications where frame rates can vary.

### Importance of DeltaTime

1. **Frame Rate Independence**: 
   - Without DeltaTime, animations would run differently on machines with varying frame rates. By using DeltaTime, animations can be adjusted to maintain a consistent speed regardless of how many frames are rendered per second.

2. **Smooth Motion**: 
   - DeltaTime allows for smoother motion by ensuring that movement calculations are based on the actual time elapsed rather than relying solely on frame counts.

3. **Timing Events**: 
   - It helps in timing events accurately, ensuring that actions occur at the right moment relative to real-world time.

### How DeltaTime Works

1. **Capture Time**:
   - At the beginning of each frame, capture the current time (e.g., using `DateTime.Now` in C#).

2. **Calculate DeltaTime**:
   - Subtract the last captured time from the current time to get the elapsed time for that frame.

   ```csharp
   TimeSpan elapsedTime = currentFrameTime - lastFrameTime;
   ```

3. **Update Last Frame Time**:
   - Store the current time as the last frame time for the next iteration.

   ```csharp
   lastFrameTime = currentFrameTime;
   ```

4. **Use DeltaTime in Calculations**:
   - Use the calculated DeltaTime to adjust movement speeds, animations, and other time-dependent calculations.

   ```csharp
   position += velocity * elapsedTime.TotalSeconds;
   ```

### Example Code

Here’s a simple example of how DeltaTime might be implemented in a game loop:

```csharp
private DateTime lastFrameTime;
private double velocity = 100.0; // pixels per second
private double position = 0.0;

private void GameLoop()
{
    DateTime currentFrameTime = DateTime.Now;
    TimeSpan elapsedTime = currentFrameTime - lastFrameTime;
    lastFrameTime = currentFrameTime;

    // Update position based on velocity and elapsed time
    position += velocity * elapsedTime.TotalSeconds;

    // Render the new position
    Render(position);
}
```

### Best Practices

1. **Consistent Units**: 
   - Always ensure that DeltaTime is in a consistent unit (usually seconds) to avoid scaling issues.

2. **Cap DeltaTime**: 
   - Optionally cap DeltaTime to prevent large jumps in time due to frame drops, which can lead to erratic behavior.

   ```csharp
   if (elapsedTime.TotalSeconds > maxDeltaTime)
       elapsedTime = TimeSpan.FromSeconds(maxDeltaTime);
   ```

3. **Use Fixed Time Steps for Physics**: 
   - For physics calculations, consider using a fixed timestep to maintain stability.


DeltaTime is a fundamental concept in game development and animation that allows for smooth, frame-rate-independent motion. By accurately calculating and utilizing DeltaTime, developers can create more responsive and visually appealing applications. Understanding and implementing DeltaTime correctly is essential for any developer working in real-time graphics and animation.



---

# Related Projects

This project serves as a direct port of the original Animation project created in VB.NET, which you can also explore for a different perspective on the same concepts. For more information and to access the complete code, visit the [Animation Repository](https://github.com/JoeLumbley/Animation) and the [Animation C# Repository](https://github.com/JoeLumbley/Animation-CS). Happy coding!

![012](https://github.com/user-attachments/assets/4a5ed26f-6d66-4b92-b824-4745f6d51275)








---





# License Information

```plaintext
MIT License
Copyright (c) 2025 Joseph W. Lumbley

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction...
```













