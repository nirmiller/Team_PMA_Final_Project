using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TeamPMA_Final_Project;
//jhkhk

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private ToggleButton rentHeatmapToggle;
    private ToggleButton amenitiesHeatmapToggle;
    private Texture2D _bubbleTexture;
    private Texture2D _mapTexture;
    private Texture2D _shortBuilding;
    private Texture2D _tallBuilding;
    private Vector2 mapPosition;
    private HeatMap _heatMap;
    private SpriteFont _uiFont;
    private bool _previousRentState = false;
    private bool _previousAmenitiesState = false;
    private SoundEffect _buttonClickSound;

    private static List<(Vector2 position, string buildingName, bool isTallBuilding)> _buildingInformation =
        new List<(Vector2 position, string buildingName, bool isTallBuilding)>
        {
            (new Vector2(180, 140), "BuildingA", false),
            (new Vector2(1200, 220), "BuildingB", true),
            (new Vector2(600, 600), "BuildingC", false),
            (new Vector2(650, 300), "BuildingD", true),
            (new Vector2(1000, 800), "BuildingE", false),
            (new Vector2(760, 210), "BuildingF", true)
        };
    
    private static Dictionary<string, float> _amenities =
        new Dictionary<string, float>
        {
            { "BuildingA", 0.2f },
            { "BuildingB", 0.9f },
            { "BuildingC", 0.5f },
            { "BuildingD", 0.7f },
            { "BuildingE", 0.3f },
            { "BuildingF", 0.8f }
        };
    
    private static Dictionary<string, float> _rentPrices =
        new Dictionary<string, float>
        {
            { "BuildingA", 0.3f },
            { "BuildingB", 0.2f },
            { "BuildingC", 0.5f },
            { "BuildingD", 0.1f },
            { "BuildingE", 0.3f },
            { "BuildingF", 0.5f }
        };

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
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        Texture2D toggleOnTex = Content.Load<Texture2D>("imgs/on_button7");
        Texture2D toggleOffTex = Content.Load<Texture2D>("imgs/off_button7");
        _uiFont = Content.Load<SpriteFont>("imgs/fontt");

        _buttonClickSound = Content.Load<SoundEffect>("imgs/506054__mellau__button-click-1");

        // UPDATED: Pass the sound effect into the buttons when you create them
        rentHeatmapToggle = new ToggleButton(toggleOnTex, toggleOffTex, new Rectangle(1100, 50, 75, 75), _buttonClickSound);
        rentHeatmapToggle.Opacity = 1f;
    
        amenitiesHeatmapToggle = new ToggleButton(toggleOnTex, toggleOffTex, new Rectangle(1100, 150, 75, 75), _buttonClickSound);
        amenitiesHeatmapToggle.Opacity = 1f;
    

        _mapTexture = Content.Load<Texture2D>("imgs/map");
        _shortBuilding = Content.Load<Texture2D>("imgs/short_building");
        _tallBuilding = Content.Load<Texture2D>("imgs/tall_building");

        _bubbleTexture = Bubble.CreateCircleTexture(GraphicsDevice, 64);

        float mapScale = 0.8f;

        float mapDrawWidth = _mapTexture.Width * mapScale;
        float mapDrawHeight = _mapTexture.Height * mapScale;

        mapPosition = new Vector2(
            (_graphics.PreferredBackBufferWidth - mapDrawWidth) / 2f,
            (_graphics.PreferredBackBufferHeight - mapDrawHeight) / 2f
        );

        _heatMap = new HeatMap(
            _mapTexture,
            _shortBuilding,
            _tallBuilding,
            _buildingInformation,
            mapPosition
        );
        
        _heatMap.ResetMap();
        //_heatMap.PullData(_amenities);
        //_heatMap.AnimateHeatMap(true,_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight );
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
        amenitiesHeatmapToggle.Update(currentMouse);
        
        // --- RENT HEATMAP LOGIC ---
        // Check if the rent button's state just changed
        if (rentHeatmapToggle.IsOn != _previousRentState)
        {
            if (rentHeatmapToggle.IsOn) // Button was turned ON
            {
                _heatMap.ResetMap();
                _heatMap.PullData(_rentPrices); // Pull the rent data
                _heatMap.AnimateHeatMap(true, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            }
            else // Button was turned OFF
            {
                _heatMap.ResetMap(); // Clears the screen
            }
            
            // Save the new state
            _previousRentState = rentHeatmapToggle.IsOn;
        }

        // --- AMENITIES HEATMAP LOGIC ---
        // Check if the amenities button's state just changed
        if (amenitiesHeatmapToggle.IsOn != _previousAmenitiesState)
        {
            if (amenitiesHeatmapToggle.IsOn) // Button was turned ON
            {
                _heatMap.ResetMap();
                _heatMap.PullData(_amenities); // Pull your amenities dictionary data
                _heatMap.AnimateHeatMap(true, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            }
            else // Button was turned OFF
            {
                _heatMap.ResetMap(); // Clears the screen
            }
            
            // Save the new state
            _previousAmenitiesState = amenitiesHeatmapToggle.IsOn;
        }

        _heatMap.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        _heatMap.DrawMap(_spriteBatch, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);     
        _heatMap.Draw(_spriteBatch, _bubbleTexture);

        _spriteBatch.End();
        // TODO: Add your drawing code here
        _spriteBatch.Begin();
    
        // Draw the button
        rentHeatmapToggle.Draw(_spriteBatch);
        amenitiesHeatmapToggle.Draw(_spriteBatch);
        _spriteBatch.DrawString(_uiFont, "Rent Heat Map", new Vector2(1000, 20), Color.Black);
        
        // Amenities label placed at Y: 125 (above the Y: 150 button)
        _spriteBatch.DrawString(_uiFont, "Amenities", new Vector2(1043, 220), Color.Black);
        _spriteBatch.DrawString(_uiFont, "West Campus Heat Map", new Vector2(400, 22), Color.Black);
    
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}