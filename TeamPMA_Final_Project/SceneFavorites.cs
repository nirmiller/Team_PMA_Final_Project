using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace TeamPMA_Final_Project;

public class SceneFavorites : Scene
{
    private SpriteFont _font;
    private List<string> _favorites;

    private KeyboardState _previousKeyboard;

    public SceneFavorites(Game game) : base(game)
    {
    }

    public override void LoadContent()
    {
        _font = Game.Content.Load<SpriteFont>("imgs/fontt");

        // get favorites from your SaveManager
        _favorites = Game.SaveManager.GetFavorites();
    }

    public override void Update(GameTime gameTime)
    {
        KeyboardState keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.B) && _previousKeyboard.IsKeyUp(Keys.B))
        {
            Game.ChangeScene(new SceneMap(Game));
        }

        _previousKeyboard = keyboard;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(_font, "Favorites", new Vector2(50, 50), Color.Black);

        if (_favorites.Count == 0)
        {
            spriteBatch.DrawString(_font, "No favorites saved.", new Vector2(50, 100), Color.Gray);
            return;
        }

        // draw list
        for (int i = 0; i < _favorites.Count; i++)
        {
            spriteBatch.DrawString(
                _font,
                _favorites[i],
                new Vector2(50, 100 + i * 30),
                Color.Black
            );
        }
    }
}