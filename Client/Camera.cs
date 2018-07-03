using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZarknorthClient
{
    public class Camera
    {

        public Vector2 Position { get; set; }
        public Vector2 Origin { get { return new Vector2(viewport.Width / 2.0f, viewport.Height / 2.0f); } }
        public float Zoom { get; set; }
        public float Rotation { get; set; }

        private Viewport viewport;
        
        public Camera(Viewport viewport)
        {
            this.viewport = viewport;
            Zoom = 1.0f;
        }

        public Matrix GetViewMatrix(Vector2 parallax)
        {
            return Matrix.CreateTranslation(new Vector3(-Position * parallax, 0.0f)) *
                   Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                   Matrix.CreateRotationZ(Rotation) *
                   Matrix.CreateScale(Zoom, Zoom, 1) *
                   Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
        }
        public void Move(Vector2 displacement, bool respectRotation = false)
        {
             if (respectRotation)
             {
                   displacement = Vector2.Transform(displacement, Matrix.CreateRotationZ(-Rotation));
             }
     
             Position += displacement;
        }

        public void LookAt(Vector2 position)
        {
            Position = position - Origin;
        }

        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return Vector2.Transform(worldPosition,GetViewMatrix(Vector2.One));
        }
	
        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, Matrix.Invert(GetViewMatrix(Vector2.One)));
        }
    }
}