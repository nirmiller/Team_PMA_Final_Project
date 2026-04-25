using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeamPMA_Final_Project;

public abstract class Scene
{
    protected Game Game;

    protected Scene(Game game)
    {
        Game = game;
    }

    public abstract void LoadContent();
    public abstract void Update(GameTime gameTime);
    public abstract void Draw(SpriteBatch spriteBatch);
}