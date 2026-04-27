using Microsoft.Xna.Framework;

namespace TeamPMA_Final_Project;

public class BuildingData
{
    public string Id { get; set; }
    public string DisplayName { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public bool IsTallBuilding { get; set; }
    public float RentValue { get; set; }
    public float AmenitiesValue { get; set; }
    public string Subtitle { get; set; }
    public string RentText { get; set; }
    public string AmenitiesText { get; set; }
    public Vector2 Position
    {
        get { return new Vector2(X, Y); }
    }
}