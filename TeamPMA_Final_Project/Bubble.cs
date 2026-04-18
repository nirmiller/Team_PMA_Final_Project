using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeamPMA_Final_Project;


public class Bubble
{
    public Vector2 Position;
    public float Radius;
    public float TargetRadius;
    public float GrowthSpeed;

    public Bubble(Vector2 position, float startRadius, float targetRadius, float growthSpeed)
    {
        Position = position;
        Radius = startRadius;
        TargetRadius = targetRadius;
        GrowthSpeed = growthSpeed;
    }

    public void Update(GameTime gameTime, bool isGrowing)
    {
        if (!isGrowing) return;

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (Radius < TargetRadius)
        {
            Radius += GrowthSpeed * dt;

            if (Radius > TargetRadius)
                Radius = TargetRadius;
        }
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D texture)
    {
        float diameter = Radius * 2f;

        Rectangle destRect = new Rectangle(
            (int)(Position.X - Radius),
            (int)(Position.Y - Radius),
            (int)diameter,
            (int)diameter
        );

        spriteBatch.Draw(texture, destRect, Color.White);
    }
}