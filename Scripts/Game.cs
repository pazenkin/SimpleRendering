using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace SimpleRendering.Scripts;

public class Game : GameWindow
{
    public Game(int width, int height, string title) : base(GameWindowSettings.Default,
        new NativeWindowSettings { ClientSize = (width, height), Title = title })
    {
    }

    private readonly float[] _vertices =
    [
        -0.5f, -0.5f, -0.5f, 0.0f, 0.0f,
        0.5f, -0.5f, -0.5f, 1.0f, 0.0f,
        0.5f, 0.5f, -0.5f, 1.0f, 1.0f,
        0.5f, 0.5f, -0.5f, 1.0f, 1.0f,
        -0.5f, 0.5f, -0.5f, 0.0f, 1.0f,
        -0.5f, -0.5f, -0.5f, 0.0f, 0.0f,

        -0.5f, -0.5f, 0.5f, 0.0f, 0.0f,
        0.5f, -0.5f, 0.5f, 1.0f, 0.0f,
        0.5f, 0.5f, 0.5f, 1.0f, 1.0f,
        0.5f, 0.5f, 0.5f, 1.0f, 1.0f,
        -0.5f, 0.5f, 0.5f, 0.0f, 1.0f,
        -0.5f, -0.5f, 0.5f, 0.0f, 0.0f,

        -0.5f, 0.5f, 0.5f, 1.0f, 0.0f,
        -0.5f, 0.5f, -0.5f, 1.0f, 1.0f,
        -0.5f, -0.5f, -0.5f, 0.0f, 1.0f,
        -0.5f, -0.5f, -0.5f, 0.0f, 1.0f,
        -0.5f, -0.5f, 0.5f, 0.0f, 0.0f,
        -0.5f, 0.5f, 0.5f, 1.0f, 0.0f,

        0.5f, 0.5f, 0.5f, 1.0f, 0.0f,
        0.5f, 0.5f, -0.5f, 1.0f, 1.0f,
        0.5f, -0.5f, -0.5f, 0.0f, 1.0f,
        0.5f, -0.5f, -0.5f, 0.0f, 1.0f,
        0.5f, -0.5f, 0.5f, 0.0f, 0.0f,
        0.5f, 0.5f, 0.5f, 1.0f, 0.0f,

        -0.5f, -0.5f, -0.5f, 0.0f, 1.0f,
        0.5f, -0.5f, -0.5f, 1.0f, 1.0f,
        0.5f, -0.5f, 0.5f, 1.0f, 0.0f,
        0.5f, -0.5f, 0.5f, 1.0f, 0.0f,
        -0.5f, -0.5f, 0.5f, 0.0f, 0.0f,
        -0.5f, -0.5f, -0.5f, 0.0f, 1.0f,

        -0.5f, 0.5f, -0.5f, 0.0f, 1.0f,
        0.5f, 0.5f, -0.5f, 1.0f, 1.0f,
        0.5f, 0.5f, 0.5f, 1.0f, 0.0f,
        0.5f, 0.5f, 0.5f, 1.0f, 0.0f,
        -0.5f, 0.5f, 0.5f, 0.0f, 0.0f,
        -0.5f, 0.5f, -0.5f, 0.0f, 1.0f
    ];

    private readonly uint[] _indices =
    [
        0, 1, 3,
        1, 2, 3
    ];

    private Camera _camera;

    private int _elementBufferObject;
    private int _vertexBufferObject;
    private int _vertexArrayObject;
    private double _time;
    private Shader _shader;
    private Texture _texture;
    private Texture _texture2;
    private bool _firstMove = true;
    private Vector2 _lastPos;

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.Enable(EnableCap.DepthTest);

        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

        _vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject);

        _vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices,
            BufferUsageHint.StaticDraw);

        _elementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices,
            BufferUsageHint.StaticDraw);

        _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
        _shader.Use();

        var vertexLocation = _shader.GetAttribLocation("aPos");
        GL.EnableVertexAttribArray(vertexLocation);
        GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

        var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
        GL.EnableVertexAttribArray(texCoordLocation);
        GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float),
            3 * sizeof(float));

        _texture = Texture.LoadFromFile("Resources/container.jpg");
        _texture.Use(TextureUnit.Texture0);

        _texture2 = Texture.LoadFromFile("Resources/awesomeface.png");
        _texture2.Use(TextureUnit.Texture1);

        _shader.SetInt("texture0", 0);
        _shader.SetInt("texture1", 1);

        _camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);
        CursorState = CursorState.Grabbed;
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.BindVertexArray(_vertexArrayObject);

        _time += 4.0 * e.Time;
        var model = Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(_time));

        _texture.Use(TextureUnit.Texture0);
        _texture2.Use(TextureUnit.Texture1);
        _shader.Use();

        _shader.SetMatrix4("model", model);
        _shader.SetMatrix4("view", _camera.GetViewMatrix());
        _shader.SetMatrix4("projection", _camera.GetProjectionMatrix());

        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        if (!IsFocused) return;

        base.OnUpdateFrame(e);

        var input = KeyboardState;

        if (input.IsKeyDown(Keys.Escape))
        {
            Close();
        }

        const float cameraSpeed = 1.5f;
        var speed = cameraSpeed * (float)e.Time;

        if (input.IsKeyDown(Keys.W))
        {
            _camera.Position += _camera.Front * speed;
        }

        if (input.IsKeyDown(Keys.S))
        {
            _camera.Position -= _camera.Front * speed;
        }

        if (input.IsKeyDown(Keys.A))
        {
            _camera.Position -= Vector3.Normalize(Vector3.Cross(_camera.Front, _camera.Up)) * speed;
        }

        if (input.IsKeyDown(Keys.D))
        {
            _camera.Position += Vector3.Normalize(Vector3.Cross(_camera.Front, _camera.Up)) * speed;
        }

        if (input.IsKeyDown(Keys.Space))
        {
            _camera.Position += _camera.Up * speed;
        }

        if (input.IsKeyDown(Keys.LeftShift))
        {
            _camera.Position -= _camera.Up * speed;
        }

        var mouse = MouseState;
        const float sensitivity = 0.2f;

        if (_firstMove)
        {
            _lastPos = new Vector2(mouse.X, mouse.Y);
            _firstMove = false;
        }
        else
        {
            var deltaX = mouse.X - _lastPos.X;
            var deltaY = mouse.Y - _lastPos.Y;
            _lastPos = new Vector2(mouse.X, mouse.Y);

            _camera.Yaw += deltaX * sensitivity;
            _camera.Pitch -= deltaY * sensitivity;
        }
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(0, 0, Size.X, Size.Y);
    }
    
    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        _camera.Fov -= e.OffsetY;
    }
}