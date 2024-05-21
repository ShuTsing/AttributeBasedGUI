using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AttributeBasedGUI;

[Tool]
public partial class WindowContainer : MarginContainer
{
	[Export]
	public Vector2I DefaultWindowSize { get; set; } = new Vector2I(800, 600);
	
	[Export]
	public Vector2I MinWindowSize { get; set; } = new Vector2I(400, 400);
	
	[Export]
	public Texture2D Icon { get; set; }
	
	[Export]
	public string WindowName { get; set; } = "Default Window";
	
	[Export]
	public bool Resizable { get; set; } = true;
	
	private Window _window;
	private Window OwnerWindow => _window ?? GetWindow();
	
	private BehaviorState _behaviorState = BehaviorState.None;
	
	private MousePosition _mousePosition = MousePosition.Default;
	
	private Vector2 _relativeMousePosition;
	
	private List<Control> _windowContentControls = new List<Control>();
	
	private const int BorderSize = 6;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}
	
	private void SetTitleInfo()
	{
		Node titleNode = this.GetNode("Body/Title");
		if (titleNode == null) return;

		if (titleNode.GetNode("Begin/IconMargin/Icon") is TextureRect iconNode)
			iconNode.Texture = Icon;

		if (titleNode.GetNode("Begin/WindowName") is Label windowNameNode)
			windowNameNode.Text = WindowName;

		if (titleNode.GetNode("End/Minimize/MinimizeButton") is Button minimizeButton)
		{
			minimizeButton.Pressed += () => OwnerWindow.Mode = Window.ModeEnum.Minimized;
			_windowContentControls.Add(minimizeButton);
		}


		if (titleNode.GetNode("End/Maximize/MaximizeButton") is Button maximizeButton)
		{
			maximizeButton.Pressed += () => OwnerWindow.Mode = OwnerWindow.Mode == Window.ModeEnum.Maximized ? Window.ModeEnum.Windowed : Window.ModeEnum.Maximized;
			_windowContentControls.Add(maximizeButton);
		}


		if (titleNode.GetNode("End/Close/CloseButton") is Button closeButton)
		{
			closeButton.Pressed += () => this.GetTree().Quit();
			_windowContentControls.Add(closeButton);
		}

		if (this.GetNode("Body/Content") is Control contentNode)
			_windowContentControls.Add(contentNode);
			
	}

	private void SetContent()
	{
		
		Node contentNode = this.GetNode("Body/Content");
		
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		if (!Engine.IsEditorHint())
		{
			OwnerWindow.Size = DefaultWindowSize;
			OwnerWindow.Borderless = true;
		}

		AnchorsPreset = (int)MarginContainer.LayoutPreset.FullRect;
		SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		SizeFlagsVertical = Control.SizeFlags.ExpandFill;
		AddThemeConstantOverride("margin_left", 4);
		AddThemeConstantOverride("margin_right", 4);
		AddThemeConstantOverride("margin_bottom", 4);
		
		var windowBody = GD.Load<PackedScene>("res://addons/AttributeBasedGUI/Resources/Prefabs/PB_WindowBody.tscn").Instantiate();
		AddChild(windowBody);

		SetTitleInfo();
		SetContent();
	}
	
	private enum BehaviorState
	{
		None = 0,
		Moving = 1,
		Resizing = 2,
	}

	private enum MousePosition
	{
		Default = 0,
		LeftBorder = 1,
		RightBorder = 2,
		TopBorder = 3,
		BottomBorder = 4,
		LeftBottomCorner = 5,
		RightTopCorner = 6,
		LeftTopCorner = 7,
		RightBottomCorner = 8,
		OnContent = 9,
	}
	
	private MousePosition MouseOnVerticalBorder(Vector2 mousePosition, float borderSize)
	{
		Vector2 windowBegin = Vector2.Zero;
		Vector2 windowEnd = OwnerWindow.Size;
		
		if (mousePosition.Y >= windowBegin.Y && mousePosition.Y <= windowBegin.Y + borderSize)
			return MousePosition.TopBorder;
		
		if (mousePosition.Y <= windowEnd.Y && mousePosition.Y >= windowEnd.Y - borderSize)
			return MousePosition.BottomBorder;
		
		return 0;
	}
	
	private MousePosition MouseOnHorizontalBorder(Vector2 mousePosition, float borderSize)
	{
		Vector2 windowBegin = Vector2.Zero;
		Vector2 windowEnd = OwnerWindow.Size;
		
		if (mousePosition.X >= windowBegin.X && mousePosition.X <= windowBegin.X + borderSize)
			return MousePosition.LeftBorder;
		
		if (mousePosition.X <= windowEnd.X && mousePosition.X >= windowEnd.X - borderSize)
			return MousePosition.RightBorder;
		
		return 0;
	}
	
	private MousePosition MouseOnMousePositionType(Vector2 mousePosition, float borderSize)
	{
		MousePosition onVerticalBorder = MouseOnVerticalBorder(mousePosition, borderSize);
		MousePosition onHorizontalBorder = MouseOnHorizontalBorder(mousePosition, borderSize);
		
		if (onVerticalBorder != 0 && onHorizontalBorder != 0)
		{
			if (onVerticalBorder == MousePosition.TopBorder && onHorizontalBorder == MousePosition.LeftBorder)
				return MousePosition.LeftTopCorner;
			
			if (onVerticalBorder == MousePosition.TopBorder && onHorizontalBorder == MousePosition.RightBorder)
				return MousePosition.RightTopCorner;
			
			if (onVerticalBorder == MousePosition.BottomBorder && onHorizontalBorder == MousePosition.LeftBorder)
				return MousePosition.LeftBottomCorner;
			
			if (onVerticalBorder == MousePosition.BottomBorder && onHorizontalBorder == MousePosition.RightBorder)
				return MousePosition.RightBottomCorner;
		}
		else if (onVerticalBorder != 0)
			return onVerticalBorder;
		else if (onHorizontalBorder != 0)
			return onHorizontalBorder;
		
		
		return MousePosition.Default;
	}
	
	private void RefreshRelativeMousePosition()
	{
		Vector2I windowPosition = OwnerWindow.Position;
		Vector2I mousePosition = DisplayServer.MouseGetPosition();
		_relativeMousePosition = windowPosition - mousePosition;
	}
	
	private void MoveWindow(InputEventMouseMotion mouseMotion)
	{
		if (_behaviorState != BehaviorState.None && _behaviorState != BehaviorState.Moving) return;
		
		if (_behaviorState == BehaviorState.None)
			RefreshRelativeMousePosition();
		
		OwnerWindow.Position = (Vector2I)(DisplayServer.MouseGetPosition() + _relativeMousePosition);
		_behaviorState = BehaviorState.Moving;
	}
	
	private void SetWindowSizePosition(Vector2I newSize, Vector2I newPosition)
	{
		newPosition.X = newSize.X > MinWindowSize.X ? newPosition.X : OwnerWindow.Position.X;
		newPosition.Y = newSize.Y > MinWindowSize.Y ? newPosition.Y : OwnerWindow.Position.Y;
		newSize.X = newSize.X > MinWindowSize.X ? newSize.X : MinWindowSize.X;
		newSize.Y = newSize.Y > MinWindowSize.Y ? newSize.Y : MinWindowSize.Y;
		
		OwnerWindow.Size = newSize;
		OwnerWindow.Position = newPosition;
	}

	private void ResizeWindow(InputEventMouseMotion mouseMotion, MousePosition borderType)
	{
		if (_behaviorState != BehaviorState.None && _behaviorState != BehaviorState.Resizing) return;
		
		if (_behaviorState == BehaviorState.None)
			RefreshRelativeMousePosition();

		if (borderType == MousePosition.LeftBorder)
		{
			Vector2I newPosition = new Vector2I((int)(DisplayServer.MouseGetPosition().X + _relativeMousePosition.X), OwnerWindow.Position.Y);
			Vector2I sizeOffset = new Vector2I((int)(newPosition.X - OwnerWindow.Position.X), 0);
			Vector2I newSize = new Vector2I(OwnerWindow.Size.X - sizeOffset.X, OwnerWindow.Size.Y);
			
			SetWindowSizePosition(newSize, newPosition);
		}
		else if (borderType == MousePosition.RightBorder)
		{
			Vector2I newSize = new Vector2I(OwnerWindow.Size.X + (int)mouseMotion.Relative.X, OwnerWindow.Size.Y);
			
			SetWindowSizePosition(newSize, OwnerWindow.Position);
		}
		else if (borderType == MousePosition.TopBorder)
		{
			Vector2I newPosition = new Vector2I(OwnerWindow.Position.X, (int)(DisplayServer.MouseGetPosition().Y + _relativeMousePosition.Y));
			Vector2I sizeOffset = new Vector2I(0, (int)(newPosition.Y - OwnerWindow.Position.Y));
			Vector2I newSize = new Vector2I(OwnerWindow.Size.X, OwnerWindow.Size.Y - sizeOffset.Y);
			
			SetWindowSizePosition(newSize, newPosition);
		}
		else if (borderType == MousePosition.BottomBorder)
		{
			Vector2I newSize = new Vector2I(OwnerWindow.Size.X, OwnerWindow.Size.Y + (int)mouseMotion.Relative.Y);
			
			SetWindowSizePosition(newSize, OwnerWindow.Position);
		}
		else if (borderType == MousePosition.LeftBottomCorner)
		{
			Vector2I newPosition = new Vector2I((int)(DisplayServer.MouseGetPosition().X + _relativeMousePosition.X), OwnerWindow.Position.Y);
			Vector2I sizeOffset = new Vector2I((int)(newPosition.X - OwnerWindow.Position.X), 0);
			Vector2I newSize = new Vector2I(OwnerWindow.Size.X - sizeOffset.X, OwnerWindow.Size.Y + (int)mouseMotion.Relative.Y);
			
			SetWindowSizePosition(newSize, newPosition);
		}
		else if (borderType == MousePosition.RightTopCorner)
		{
			Vector2I newPosition = new Vector2I(OwnerWindow.Position.X, (int)(DisplayServer.MouseGetPosition().Y + _relativeMousePosition.Y));
			Vector2I sizeOffset = new Vector2I(0, (int)(newPosition.Y - OwnerWindow.Position.Y));
			Vector2I newSize = new Vector2I(OwnerWindow.Size.X + (int)mouseMotion.Relative.X, OwnerWindow.Size.Y - sizeOffset.Y);
			
			SetWindowSizePosition(newSize, newPosition);
		}
		else if (borderType == MousePosition.LeftTopCorner)
		{
			Vector2I newPosition = new Vector2I((int)(DisplayServer.MouseGetPosition().X + _relativeMousePosition.X), (int)(DisplayServer.MouseGetPosition().Y + _relativeMousePosition.Y));
			Vector2I sizeOffset = new Vector2I((int)(newPosition.X - OwnerWindow.Position.X), (int)(newPosition.Y - OwnerWindow.Position.Y));
			Vector2I newSize = new Vector2I(OwnerWindow.Size.X - sizeOffset.X, OwnerWindow.Size.Y - sizeOffset.Y);
			
			SetWindowSizePosition(newSize, newPosition);
		}
		else if (borderType == MousePosition.RightBottomCorner)
		{
			Vector2I newSize = new Vector2I(OwnerWindow.Size.X + (int)mouseMotion.Relative.X, OwnerWindow.Size.Y + (int)mouseMotion.Relative.Y);
			
			SetWindowSizePosition(newSize, OwnerWindow.Position);
		}
		
		_behaviorState = BehaviorState.Resizing;
	}
	
	private bool CheckMouseInContent(Vector2 mousePosition)
	{
		foreach (var control in _windowContentControls)
		{
			if (control.GetGlobalRect().HasPoint(mousePosition))
				return true;
		}
		
		return false;
	}

	private void SetMouseCursorShape(InputEventMouseMotion mouseMotion)
	{
		if (_behaviorState != BehaviorState.None || Resizable == false) return;
		
		bool isOnContent = CheckMouseInContent(mouseMotion.GlobalPosition);
		if (isOnContent)
		{
			this.MouseDefaultCursorShape = Control.CursorShape.Arrow;
			_mousePosition = MousePosition.OnContent;
			return;
		}
		
		MousePosition borderType = MouseOnMousePositionType(mouseMotion.GlobalPosition, BorderSize);
		
		//check mouse only on own control, and not on children
		
		if (borderType == MousePosition.LeftBottomCorner || borderType == MousePosition.RightTopCorner)
			this.MouseDefaultCursorShape = Control.CursorShape.Bdiagsize;
		else if (borderType == MousePosition.LeftTopCorner || borderType == MousePosition.RightBottomCorner)
			this.MouseDefaultCursorShape = Control.CursorShape.Fdiagsize;
		else if (borderType == MousePosition.TopBorder || borderType == MousePosition.BottomBorder)
			this.MouseDefaultCursorShape = Control.CursorShape.Vsize;
		else if (borderType == MousePosition.LeftBorder || borderType == MousePosition.RightBorder)
			this.MouseDefaultCursorShape = Control.CursorShape.Hsize;
		else
			this.MouseDefaultCursorShape = Control.CursorShape.Arrow;
		
		_mousePosition = borderType;
	}

	private void ProcessWindow(InputEventMouseMotion mouseMotion)
	{
		var borderType = _mousePosition;

		if (mouseMotion.ButtonMask != MouseButtonMask.Left) return;
		
		if (borderType == MousePosition.Default)
		{
			MoveWindow(mouseMotion);
		}
		else
		{
			if (Resizable)
				ResizeWindow(mouseMotion, borderType);
		}

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
		if (!Engine.IsEditorHint())
		{
            int dpi = DisplayServer.ScreenGetDpi(OwnerWindow.GetWindowId());
            OwnerWindow.ContentScaleFactor = dpi / 96.0f;
		}
		
	}
	
	private void ResetState()
	{
		_behaviorState = BehaviorState.None;
	}
	
	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (@event is InputEventMouseMotion mouseMotion)
		{
			SetMouseCursorShape(mouseMotion);
			
			if (mouseMotion.ButtonMask == MouseButtonMask.Left)
				ProcessWindow(mouseMotion);
			else
				ResetState();
			
		}
	}
}
