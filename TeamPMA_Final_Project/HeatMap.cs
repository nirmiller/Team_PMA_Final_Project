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

    private List<(Vector2 position, string buildingName)> _buildingPositions;

    private List<(bool, string buildingName)> _isTallBuilding;

    private Vector2 _mapPosition;

    public HeatMap(
        Texture2D baseMap,
        Texture2D buildingShort,
        Texture2D buildingTall,
        List<(Vector2 position, string buildingName)> buildingPositions,
        List<(bool, string buildingName)> isTallBuilding,
        Vector2 mapPosition)
    {
        _baseMap = baseMap;
        _buildingShort = buildingShort;
        _buildingTall = buildingTall;
        _buildingPositions = buildingPositions;
        _isTallBuilding = isTallBuilding;
        _mapPosition = mapPosition;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_baseMap, _mapPosition, Color.White);

        int count = Math.Min(_buildingPositions.Count, _isTallBuilding.Count);

        for (int i = 0; i < count; i++)
        {
            bool isTall = _isTallBuilding[i].Item1;

            Texture2D currentBuilding = isTall ? _buildingTall : _buildingShort;

            Vector2 drawPosition = _buildingPositions[i].position;//+ _mapPosition ;

            drawPosition.X -= currentBuilding.Width / 2f;
            drawPosition.Y -= currentBuilding.Height / 2f;

            spriteBatch.Draw(currentBuilding, drawPosition, Color.White);
        }
    }

    // Example: datapoints are heat values at positions
    public void AnimateHeatMap(GameTime gameTime, List<(Vector2 position, float heatValue)> dataPoints)
    {
      
    }
}