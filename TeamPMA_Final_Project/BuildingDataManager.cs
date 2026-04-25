using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace TeamPMA_Final_Project;

public class BuildingDataManager
{
    private Dictionary<string, BuildingData> buildings;

    public BuildingDataManager()
    {
        buildings = new Dictionary<string, BuildingData>();
    }

    public void LoadBuildings(string filePath)
    {
        if (!File.Exists(filePath))
        {
            buildings = new Dictionary<string, BuildingData>();
            return;
        }

        string json = File.ReadAllText(filePath);

        buildings = JsonSerializer.Deserialize<Dictionary<string, BuildingData>>(json)
                    ?? new Dictionary<string, BuildingData>();
    }

    public Dictionary<string, BuildingData> GetBuildings()
    {
        return buildings;
    }

    public BuildingData GetBuilding(string id)
    {
        if (buildings.ContainsKey(id))
        {
            return buildings[id];
        }

        return null;
    }
}