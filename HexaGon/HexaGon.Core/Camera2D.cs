using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;

namespace HexaGon
{
	public class Camera2d
	{
		protected float          _zoom; // Camera Zoom
		public Matrix             _transform; // Matrix Transform
		public Vector2          _pos; // Camera Position
		protected float         _rotation; // Camera Rotation
		
		public Camera2d()
		{
			_zoom = 1.0f;
			_rotation = 0.0f;
			_pos = Vector2.Zero;
		}

		public float Zoom
		{
			get { return _zoom; }
			set { _zoom = value; if (_zoom < 0.1f) _zoom = 0.1f; } // Negative zoom will flip image
		}
		
		public float Rotation
		{
			get {return _rotation; }
			set { _rotation = value; }
		}
		
		// Auxiliary function to move the camera
		public void Move(Vector2 amount)
		{
			_pos += amount;
		}
		// Get set position
		public Vector2 Pos
		{
			get{ return  _pos; }
			set{ _pos = value; }
		}

		public Matrix get_transformation(GraphicsDevice graphicsDevice)
		{
			Viewport viewPort= graphicsDevice.Viewport;

			_transform =       // Thanks to o KB o for this solution
				Matrix.CreateTranslation(new Vector3(-_pos.X * Zoom, -_pos.Y * Zoom, 0)) *
					Matrix.CreateRotationZ(Rotation) *
					Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
					Matrix.CreateTranslation(new Vector3(viewPort.Width*0.5f, viewPort.Height*0.5f, 0));
			return _transform;
		}
		
	}
}

