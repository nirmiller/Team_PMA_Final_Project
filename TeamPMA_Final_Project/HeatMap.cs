using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeamPMA_Final_Project;

public class HeatMap
{
    private Texture2D _baseMap;
    private Texture2D _buildingShort;
    private Texture2D _buildingTall;

    private Dictionary<string, (Vector2 position, bool isTallBuilding)> _buildingInformation;
    private Dictionary<string, float> _dataPoints;

    private List<Bubble> _bubbles;

    private Vector2 _mapPosition;

    private float _mapScale;
    private Vector2 _centeredMapPosition;
    private float _buildingScale;

    public HeatMap(
        Texture2D baseMap,
        Texture2D buildingShort,
        Texture2D buildingTall,
        List<(Vector2 position, string buildingName, bool isTallBuilding)> buildingInformation,
        Vector2 mapPosition)
    {
        _baseMap = baseMap;
        _buildingShort = buildingShort;
        _buildingTall = buildingTall;
        _mapPosition = mapPosition;

        _buildingInformation = new Dictionary<string, (Vector2 position, bool isTallBuilding)>();
        for (int i = 0; i < buildingInformation.Count; i++)
        {
            string name = buildingInformation[i].buildingName;
            Vector2 position = buildingInformation[i].position;
            bool isTall = buildingInformation[i].isTallBuilding;

            _buildingInformation[name] = (position, isTall);
        }

        _dataPoints = new Dictionary<string, float>();
        _bubbles = new List<Bubble>();

        _mapScale = 1f;
        _centeredMapPosition = mapPosition;
        _buildingScale = 1f;
    }

    public void PullData(Dictionary<string, float> dataPoints)
    {
        _dataPoints.Clear();

        foreach (var kvp in dataPoints)
        {
            _dataPoints[kvp.Key] = kvp.Value;
        }
    }

    private void UpdateMapLayout(int screenWidth, int screenHeight)
    {
        float maxMapWidth = screenWidth * 0.8f;
        float maxMapHeight = screenHeight * 0.8f;

        float mapScaleX = maxMapWidth / _baseMap.Width;
        float mapScaleY = maxMapHeight / _baseMap.Height;
        _mapScale = Math.Min(mapScaleX, mapScaleY);

        int scaledMapWidth = (int)(_baseMap.Width * _mapScale);
        int scaledMapHeight = (int)(_baseMap.Height * _mapScale);

        _centeredMapPosition = new Vector2(
            (screenWidth - scaledMapWidth) / 2f,
            (screenHeight - scaledMapHeight) / 2f
        );

        _buildingScale = _mapScale * 0.2f;
    }

    public void DrawMap(SpriteBatch spriteBatch, int screenWidth, int screenHeight)
    {
        UpdateMapLayout(screenWidth, screenHeight);

        int scaledMapWidth = (int)(_baseMap.Width * _mapScale);
        int scaledMapHeight = (int)(_baseMap.Height * _mapScale);

        spriteBatch.Draw(
            _baseMap,
            new Rectangle(
                (int)_centeredMapPosition.X,
                (int)_centeredMapPosition.Y,
                scaledMapWidth,
                scaledMapHeight
            ),
            Color.White
        );

        foreach (KeyValuePair<string, (Vector2 position, bool isTallBuilding)> building in _buildingInformation)
        {
            Texture2D currentBuilding = building.Value.isTallBuilding ? _buildingTall : _buildingShort;

            Vector2 scaledBuildingPosition = building.Value.position * _mapScale;
            Vector2 drawPosition = _centeredMapPosition + scaledBuildingPosition;

            spriteBatch.Draw(
                currentBuilding,
                drawPosition,
                null,
                Color.White,
                0f,
                new Vector2(currentBuilding.Width / 2f, currentBuilding.Height / 2f),
                _buildingScale,
                SpriteEffects.None,
                0f
            );
        }
    }

    public void ResetMap()
    {
        _dataPoints.Clear();
        _bubbles.Clear();
    }

    public void Update(GameTime gameTime)
    {
        foreach (Bubble bubble in _bubbles)
        {
            bubble.Update(gameTime);
        }
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D bubbleTexture)
    {
        foreach (Bubble bubble in _bubbles)
        {
            bubble.Draw(spriteBatch, bubbleTexture);
        }
    }

    private Vector2 MapToScreen(Vector2 mapPosition)
    {
        return _centeredMapPosition + mapPosition * _mapScale;
    }

    public void AnimateHeatMap(bool showMap, int screenWidth, int screenHeight, Color targetColor)
    {
        _bubbles.Clear();

        UpdateMapLayout(screenWidth, screenHeight);

        if (showMap && _dataPoints.Count > 0)
        {
            float maxRadius = 100f;
            float growthTime = 2f;

            foreach (var dataPoint in _dataPoints)
            {
                string name = dataPoint.Key;

                if (_buildingInformation.ContainsKey(name))
                {
                    Vector2 bubblePosition = MapToScreen(_buildingInformation[name].position);

                    float value = MathHelper.Clamp(dataPoint.Value, 0f, 1f);
                    float startRadius = 0f;
                    float targetRadius = value * maxRadius * _mapScale;

                    _bubbles.Add(new Bubble(bubblePosition, startRadius, targetRadius, growthTime, maxRadius * _mapScale, targetColor));
                }
            }
        }
    }
    public string GetClickedBuilding(Point mousePosition)
    {
        foreach (KeyValuePair<string, (Vector2 position, bool isTallBuilding)> building in _buildingInformation)
        {
            Texture2D currentBuilding = building.Value.isTallBuilding ? _buildingTall : _buildingShort;

            Vector2 scaledBuildingPosition = building.Value.position * _mapScale;
            Vector2 drawPosition = _centeredMapPosition + scaledBuildingPosition;

            int width = (int)(currentBuilding.Width * _buildingScale);
            int height = (int)(currentBuilding.Height * _buildingScale);

            Rectangle buildingBounds = new Rectangle(
                (int)(drawPosition.X - width / 2f),
                (int)(drawPosition.Y - height / 2f),
                width,
                height
            );

            if (buildingBounds.Contains(mousePosition))
            {
                return building.Key;
            }
        }

        return null;
    }
}