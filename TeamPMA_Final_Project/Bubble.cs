using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeamPMA_Final_Project;

public class Bubble
{
    public Vector2 Position;
    public float Radius;
    public float TargetRadius;

    private float _t;
    private float _growthTime;
    private float _startRadius;

    private float _maxPossibleRadius;
    private float _targetColorProgress;

    private Color _targetColor;

    public Bubble(
        Vector2 position,
        float startRadius,
        float targetRadius,
        float growthTime,
        float maxPossibleRadius,
        Color targetColor) 
    {
        Position = position;
        Radius = startRadius;
        _startRadius = startRadius;
        TargetRadius = targetRadius;
        _growthTime = growthTime;
        _maxPossibleRadius = maxPossibleRadius;

        _targetColor = targetColor;

        _t = 0f;

        _targetColorProgress = MathHelper.Clamp(TargetRadius / _maxPossibleRadius, 0f, 1f);
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_t < 1f)
        {
            _t += dt / _growthTime;

            if (_t > 1f)
                _t = 1f;

            float smoothT = _t * _t * (3f - 2f * _t);
            Radius = MathHelper.Lerp(_startRadius, TargetRadius, smoothT);
        }
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D texture)
    {
        float scale = Radius / (texture.Width / 2f);

        float currentColorProgress = _t * _targetColorProgress;
        Color bubbleColor = GetBubbleColor(currentColorProgress);

        spriteBatch.Draw(
            texture,
            Position,
            null,
            bubbleColor * 0.6f,
            0f,
            new Vector2(texture.Width / 2f, texture.Height / 2f),
            scale,
            SpriteEffects.None,
            0f
        );
    }

    private Color GetBubbleColor(float t)
    {
        t = MathHelper.Clamp(t, 0f, 1f);

        Color startColor = Color.Yellow;

        return Color.Lerp(startColor, _targetColor, t);
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
                    data[index] = Color.White;
                else
                    data[index] = Color.Transparent;
            }
        }

        texture.SetData(data);
        return texture;
    }
}