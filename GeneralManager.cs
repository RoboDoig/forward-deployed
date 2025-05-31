using Godot;
using System;

// TODO: Stupidly named for now, just testing stuff out
public partial class GeneralManager : Node3D
{
    private UiManager UiManager;
    private PlayerController PlayerController;

    public override void _Ready()
    {
        base._Ready();

        UiManager = GetNode<UiManager>("CanvasLayer");
        PlayerController = GetNode<PlayerController>("Player");

        PlayerController.WorldObjectClicked += (worldObject) =>
        {
            UiManager.CreateWorldObjectPanel(worldObject);
        };

        PlayerController.WorldObjectHovered += (WorldObject) =>
        {
            UiManager.SetCrosshairScale(new Vector2(1.5f, 1.5f));
        };

        PlayerController.NoWorldObjectHovered += (WorldObject) =>
        {
            UiManager.SetCrosshairScale(new Vector2(1, 1));
        };

        UiManager.RequestUiControls += () =>
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
            PlayerController.LockControls = true;
        };

        UiManager.RequestReleaseUiControls += () =>
        {
            Input.MouseMode = Input.MouseModeEnum.Captured;
            PlayerController.LockControls = false;
        };
    }
}
