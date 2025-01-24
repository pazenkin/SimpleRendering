using OpenTK.Mathematics;

namespace SimpleRendering.Scripts;

public class Camera
{
    public Camera(Vector3 position, float aspectRatio)
    {
        Position = position;
        AspectRatio = aspectRatio;
    }

    public Vector3 Position { get; set; }
    public float AspectRatio { private get; set; }
    public Vector3 Front => _front;
    public Vector3 Up => _up;

    private readonly Vector3 _front = -Vector3.UnitZ;
    private readonly Vector3 _up = Vector3.UnitY;
    private readonly float _fov = MathHelper.PiOver2;

    public Matrix4 GetViewMatrix()
    {
        return Matrix4.LookAt(Position, Position + _front, _up);
    }

    public Matrix4 GetProjectionMatrix()
    {
        return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 100f);
    }
}