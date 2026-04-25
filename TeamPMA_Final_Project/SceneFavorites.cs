using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace TeamPMA_Final_Project;

public class SceneFavorites : Scene
{
    private SpriteFont _font;
    private KeyboardState _previousKeyboard;

    public SceneFavorites(Game game) : base(game)
    {
    }

    public override void LoadContent()
    {
        _font = Game.Content.Load<SpriteFont>("imgs/fontt");
    }

    public override void Update(GameTime gameTime)
    {
        KeyboardState keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.B) && _previousKeyboard.IsKeyUp(Keys.B))
        {
            Game.ChangeScene(Game.MapScene);
        }

        _previousKeyboard = keyboard;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(_font, "Favorites", new Vector2(50, 50), Color.Black);
        spriteBatch.DrawString(_font, "Press B to go back", new Vector2(50, 80), Color.DarkGray);

        List<string> favorites = Game.SaveManager.GetFavorites();

        if (favorites.Count == 0)
        {
            spriteBatch.DrawString(_font, "No favorites saved.", new Vector2(50, 130), Color.Gray);
            return;
        }

        for (int i = 0; i < favorites.Count; i++)
        {
            string buildingId = favorites[i];

            BuildingData building = Game.BuildingDataManager.GetBuilding(buildingId);

            string nameToDraw = building != null
                ? building.DisplayName
                : buildingId;

            spriteBatch.DrawString(
                _font,
                nameToDraw,
                new Vector2(50, 130 + i * 30),
                Color.Black
            );
        }
    }
}