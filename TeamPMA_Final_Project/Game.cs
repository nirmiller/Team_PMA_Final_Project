using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TeamPMA_Final_Project;

public class Game : Microsoft.Xna.Framework.Game
{
    public static string SavePath;

    public SaveManager SaveManager { get; private set; }
    public BuildingDataManager BuildingDataManager { get; private set; }
    public GraphicsDeviceManager Graphics { get; private set; }

    public SceneMap MapScene { get; private set; }
    public SceneFavorites FavoritesScene { get; private set; }

    private SpriteBatch _spriteBatch;
    private Scene _currentScene;
    private KeyboardState _previousKeyboardState;

    public Game()
    {
        Graphics = new GraphicsDeviceManager(this);
        SaveManager = new SaveManager();
        BuildingDataManager = new BuildingDataManager();

        Graphics.PreferredBackBufferHeight = 800;
        Graphics.PreferredBackBufferWidth = 1200;

        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        Exiting += OnGameExit;
    }

    protected override void LoadContent()
    {
        SavePath = "Content/favorites.json";

        SaveManager.LoadFavorites(SavePath);
        BuildingDataManager.LoadBuildings("Content/buildings");

        _spriteBatch = new SpriteBatch(GraphicsDevice);

        MapScene = new SceneMap(this);
        FavoritesScene = new SceneFavorites(this);

        MapScene.LoadContent();
        FavoritesScene.LoadContent();

        _currentScene = MapScene;
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyboardState = Keyboard.GetState();

        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        if (keyboardState.IsKeyDown(Keys.D1) && _previousKeyboardState.IsKeyUp(Keys.D1))
        {
            _currentScene = MapScene;
        }

        if (keyboardState.IsKeyDown(Keys.D2) && _previousKeyboardState.IsKeyUp(Keys.D2))
        {
            _currentScene = FavoritesScene;
        }

        _currentScene.Update(gameTime);

        _previousKeyboardState = keyboardState;

        base.Update(gameTime);
    }

    public void ChangeScene(Scene newScene)
    {
        _currentScene = newScene;
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();
        _currentScene.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void OnGameExit(object sender, EventArgs e)
    {
        SaveManager.Save(SavePath);
    }
}