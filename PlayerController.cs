using Godot;
using System;

// adapted from https://github.com/rbarongr/GodotFirstPersonController/blob/main/Player/player.gd

public partial class PlayerController : CharacterBody3D
{
    [Export] float MouseSensitivity = 0.3f;
    [Export] float MaxCameraAngle = 90;
    [Export] float MinCameraAngle = -90;

    Node3D head;
    Camera3D mainCamera;

    float cameraAngle;
    Vector2 cameraChange;
    Vector2 mousePosition;

    Vector3 walkVel;
    Vector3 gravVel;

    [Export] float speed = 10;
    [Export] float acceleration = 100;

    private WorldObject hoveredObject;

    [Signal]
    public delegate void WorldObjectClickedEventHandler(WorldObject worldObject);

    [Signal]
    public delegate void WorldObjectHoveredEventHandler(WorldObject worldObject);

    [Signal]
    public delegate void NoWorldObjectHoveredEventHandler(WorldObject worldObject);

    [Signal]
    public delegate void InventoryOpenRequestEventHandler();

    // Walk
    const float gravity = -9.8f * 3;

    public bool LockControls = false;

    public override void _Ready()
    {
        head = GetNode<Node3D>("Head");
        mainCamera = GetNode<Camera3D>("Head/Camera3D");

        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Input(InputEvent @event)
    {
        if (LockControls)
        {
            return;
        }

        if (@event is InputEventMouseMotion motion)
        {
            cameraChange = motion.Relative;
            mousePosition = motion.GlobalPosition;
        }

        if (@event.IsPressed() && @event is InputEventMouseButton inputEventMouseButton)
        {
            if (inputEventMouseButton.ButtonIndex == MouseButton.Left)
            {
                if (hoveredObject != null)
                {
                    EmitSignal(SignalName.WorldObjectClicked, hoveredObject);
                }
            }
        }

        if (Input.IsActionJustPressed("inventory"))
        {
            EmitSignal(SignalName.InventoryOpenRequest);
        }
    }

    public override void _Process(double delta)
    {
        CheckMouseObjectHover();
    }

    void CheckMouseObjectHover()
    {
        var cameraRayLength = 1000f;
        var cameraRayStart = mainCamera.ProjectRayOrigin(mousePosition);
        var cameraRayEnd = cameraRayStart + mainCamera.ProjectRayNormal(mousePosition) * cameraRayLength;
        var detected = GetWorld3D().
            DirectSpaceState.
            IntersectRay(new PhysicsRayQueryParameters3D { From = cameraRayStart, To = cameraRayEnd });
        if (detected.ContainsKey("collider"))
        {
            var node = (Node3D)detected["collider"];
            if (node is WorldObject)
            {
                var worldObject = node as WorldObject;
                hoveredObject = worldObject;

                EmitSignal(SignalName.WorldObjectHovered, hoveredObject);
            }
            else
            {
                hoveredObject = null;

                EmitSignal(SignalName.NoWorldObjectHovered, hoveredObject);
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (LockControls)
        {
            return;
        }

        Aim();
        Velocity = Walk(delta) + Gravity(delta);

        MoveAndSlide();
    }

    Vector3 Walk(double delta)
    {
        var moveDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
        Vector3 forward = mainCamera.GlobalTransform.Basis * new Vector3(moveDir.X, 0, moveDir.Y);
        Vector3 walkDir = new Vector3(forward.X, 0, forward.Z).Normalized();
        walkVel = walkVel.MoveToward(walkDir * speed * moveDir.Length(), acceleration * (float)delta);
        return walkVel;
    }

    Vector3 Gravity(double delta)
    {
        gravVel = IsOnFloor() ? Vector3.Zero : gravVel.MoveToward(new Vector3(0, Velocity.Y - gravity, 0), gravity * (float)delta);
        return gravVel;
    }

    void Aim()
    {
        if (cameraChange.Length() > 0)
        {
            head.RotateY(Mathf.DegToRad(-cameraChange.X * MouseSensitivity));

            var change = -cameraChange.Y * MouseSensitivity;

            if (change + cameraAngle < MaxCameraAngle && change + cameraAngle > MinCameraAngle)
            {
                mainCamera.RotateX(Mathf.DegToRad(change));

                cameraAngle += change;
            }

            cameraChange = new Vector2();
        }
    }
}
