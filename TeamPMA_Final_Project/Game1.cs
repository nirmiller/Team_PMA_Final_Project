using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TeamPMA_Final_Project;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private ToggleButton rentHeatmapToggle;
    
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        
        _graphics.PreferredBackBufferHeight = 800;
        _graphics.PreferredBackBufferWidth = 1200;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        Texture2D toggleOnTex = Content.Load<Texture2D>("imgs/on_button");
        Texture2D toggleOffTex = Content.Load<Texture2D>("imgs/off_button2");

        // Initialize the button at X: 50, Y: 50, Width: 100, Height: 50
        rentHeatmapToggle = new ToggleButton(toggleOnTex, toggleOffTex, new Rectangle(1100, 50, 75, 75));
        // Make this specific button 50% transparent!
        rentHeatmapToggle.Opacity = 1f;
    

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        // Grab the current mouse state
        MouseState currentMouse = Mouse.GetState();

        // Update the button logic
        rentHeatmapToggle.Update(currentMouse);

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();
    
        // Draw the button
        rentHeatmapToggle.Draw(_spriteBatch);
    
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}