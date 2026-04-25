using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace TeamPMA_Final_Project;

public class SaveManager
{
    private List<string> favorites;

    public SaveManager()
    {
        favorites = new List<string>();
    }

    public  List<string> LoadFavorites(string loadFile)
    {
        if (!File.Exists(loadFile))
        {
            favorites = new List<string>();
            return new List<string>();
        }

        string json = File.ReadAllText(loadFile);

        if (string.IsNullOrWhiteSpace(json))
        {
            favorites = new List<string>();
            return favorites;
        }

        favorites = JsonSerializer.Deserialize<List<string>>(json);

        if (favorites == null)
        {
            favorites = new List<string>();
        }

        return favorites;
    }

    public void AddFavorite(string buildingName)
    {
        if (!favorites.Contains(buildingName))
        {
            favorites.Add(buildingName);
        }
    }

    public void Save(string saveFile)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        string json = JsonSerializer.Serialize(favorites, options);

        File.WriteAllText(saveFile, json);
    }

    public List<string> GetFavorites()
    {
        return favorites;
    }
    public void RemoveFavorite(string buildingName)
    {
        favorites.Remove(buildingName);
    }
    public void ClearFavorites()
    {
        favorites.Clear();
    }
}