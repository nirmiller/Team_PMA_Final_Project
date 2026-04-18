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

    private List<(Vector2 position, string buildingName, bool isTallBuilding)> _buildingInformation;
    

    private List<(string buildingName, float heatValue)> _dataPoints;

    private Vector2 _mapPosition;

    public HeatMap(
        Texture2D baseMap,
        Texture2D buildingShort,
        Texture2D buildingTall,
        List<(Vector2 position, string buildingName, bool isTallBuilding)> buildingInformation, Vector2 mapPosition)
    {
        _baseMap = baseMap;
        _buildingShort = buildingShort;
        _buildingTall = buildingTall;
        _buildingInformation = buildingInformation;
        _mapPosition = mapPosition;
    }

    public void pullData(List<(string buildingName, float value)> dataPoints)
    {
        _dataPoints = dataPoints;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_baseMap, _mapPosition, Color.White);

        int count = _buildingInformation.Count;

        for (int i = 0; i < count; i++)
        {
            bool isTall = _buildingInformation[i].isTallBuilding;
            ;

            Texture2D currentBuilding = isTall ? _buildingTall : _buildingShort;

            Vector2 drawPosition = _buildingInformation[i].position + _mapPosition ;

            drawPosition.X -= currentBuilding.Width / 2f;
            drawPosition.Y -= currentBuilding.Height / 2f;

            spriteBatch.Draw(currentBuilding, drawPosition, Color.White);
        }
    }

    public void resetMap()
    {
        _dataPoints.Clear();
    }

    // Example: datapoints are heat values at positions
    public void AnimateHeatMap(GameTime gameTime, bool showMap)
    {
        if (showMap & _dataPoints.Count > 0)
        {
            for (int i = 0; i < _dataPoints.Count; i++)
            {
                
            }
        }
      
    }
    
    
}