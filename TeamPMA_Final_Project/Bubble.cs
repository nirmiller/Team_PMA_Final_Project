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
    
    private float _t; // goes from 0 → 1
    private float _growthTime;
    private float _startRadius;

    public Bubble(Vector2 position, float startRadius, float targetRadius, float growthTime)
    {
        Position = position;
        Radius = startRadius;
        _startRadius = startRadius;
        TargetRadius = targetRadius;
        _growthTime = growthTime;

        _t = 0f;
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_t < 1f)
        {
            _t += dt / _growthTime;

            if (_t > 1f)
                _t = 1f;

            float smoothT = _t * _t * (3f - 2f * _t)*0.5f; 
            Radius = MathHelper.Lerp(_startRadius, TargetRadius, smoothT);        }
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D texture)
    {
        float scale = Radius / (texture.Width / 2f);

        spriteBatch.Draw(
            texture,
            Position,
            null,
            Color.Red * 0.6f, // slight transparency = better heatmap look
            0f,
            new Vector2(texture.Width / 2f, texture.Height / 2f),
            scale,
            SpriteEffects.None,
            0f
        );
    }
    
    public static Texture2D CreateCircleTexture(GraphicsDevice graphicsDevice, int radius)
    {
        int diameter = radius * 2;
        Texture2D texture = new Texture2D(graphicsDevice, diameter, diameter);

        Color[] data = new Color[diameter * diameter];

        Vector2 center = new Vector2(radius, radius);

        for (int y = 0; y < diameter; y++)
        {
            for (int x = 0; x < diameter; x++)
            {
                int index = x + y * diameter;

                float distance = Vector2.Distance(new Vector2(x, y), center);

                if (distance <= radius)
                {
                    data[index] = Color.White; // inside circle
                }
                else
                {
                    data[index] = Color.Transparent; // outside
                }
            }
        }

        texture.SetData(data);
        return texture;
    }
}