using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media; 
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
    private ToggleButton musicToggle;
    private Song backgroundMusic;
    private bool _previousMusicState = false;
    private MouseState _previousMouseState;
    private string _selectedBuilding = null;
    private Texture2D _popupPixel;

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

    private static Dictionary<string, string> _displayNames =
        new Dictionary<string, string>
        {
            { "BuildingA", "### Apartment A ###" },
            { "BuildingB", "### Apartment B ###" },
            { "BuildingC", "### Apartment C ###" },
            { "BuildingD", "### Apartment D ###" },
            { "BuildingE", "### Apartment E ###" },
            { "BuildingF", "### Apartment F ###" }
        };
    
    private static Dictionary<string, string> _popupDescriptions =
        new Dictionary<string, string>
        {
            { "BuildingA", "Rent: $$   Amenities: Low" },
            { "BuildingB", "Rent: $$   Amenities: High" },
            { "BuildingC", "Rent: $$$  Amenities: Medium" },
            { "BuildingD", "Rent: $    Amenities: High" },
            { "BuildingE", "Rent: $$   Amenities: Low" },
            { "BuildingF", "Rent: $$$  Amenities: High" }
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
        backgroundMusic = Content.Load<Song>("imgs/447515__alittlebitdrunkguy__simple-lofi-hip-hop-track-n");
        
        // Optional: If you want the song to loop infinitely when turned on
        MediaPlayer.IsRepeating = true; 

        
        musicToggle = new ToggleButton(toggleOnTex, toggleOffTex, new Rectangle(20, 50, 75, 75), null);

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
        // Update the new button
        musicToggle.Update(currentMouse);

        // --- MUSIC BUTTON LOGIC ---
        // Check if the music button's state just changed this frame
        if (musicToggle.IsOn != _previousMusicState)
        {
            if (musicToggle.IsOn) 
            {
                // Button was turned ON -> Play the song
                MediaPlayer.Play(backgroundMusic);
            }
            else 
            {
                // Button was turned OFF -> Pause the song
                MediaPlayer.Pause();
            }
            
            // Save the new state so this only triggers once per click
            _previousMusicState = musicToggle.IsOn;
        }
        
        if (currentMouse.LeftButton == ButtonState.Pressed &&
            _previousMouseState.LeftButton == ButtonState.Released)
        {
            string clickedBuilding = _heatMap.GetClickedBuilding(currentMouse.Position);

            if (clickedBuilding != null)
            {
                _selectedBuilding = clickedBuilding;
            }
            else
            {
                _selectedBuilding = null;
            }
        }
        
        _popupPixel = new Texture2D(GraphicsDevice, 1, 1);
        _popupPixel.SetData(new[] { Color.White });
        

        _heatMap.Update(gameTime);
       
        _previousMouseState = currentMouse;

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
        musicToggle.Draw(_spriteBatch);
        _spriteBatch.DrawString(_uiFont, "Rent Heat Map", new Vector2(1000, 20), Color.Black);
        
        
        _spriteBatch.DrawString(_uiFont, "Music", new Vector2(20, 25), Color.Black);

        _spriteBatch.DrawString(_uiFont, "Amenities", new Vector2(1043, 220), Color.Black);
        _spriteBatch.DrawString(_uiFont, "West Campus Heat Map", new Vector2(400, 22), Color.Black);
        
    
        if (_selectedBuilding != null)
        {
            Rectangle popupRect = new Rectangle(30, 650, 320, 110);

           

            _spriteBatch.Draw(_popupPixel, popupRect, Color.White * 0.9f);

            _spriteBatch.Draw(_popupPixel, new Rectangle(popupRect.X, popupRect.Y, popupRect.Width, 2), Color.Black);
            _spriteBatch.Draw(_popupPixel, new Rectangle(popupRect.X, popupRect.Bottom - 2, popupRect.Width, 2), Color.Black);
            _spriteBatch.Draw(_popupPixel, new Rectangle(popupRect.X, popupRect.Y, 2, popupRect.Height), Color.Black);
            _spriteBatch.Draw(_popupPixel, new Rectangle(popupRect.Right - 2, popupRect.Y, 2, popupRect.Height), Color.Black);

            string title = _displayNames.ContainsKey(_selectedBuilding)
                ? _displayNames[_selectedBuilding]
                : _selectedBuilding;

            string info = _popupDescriptions.ContainsKey(_selectedBuilding)
                ? _popupDescriptions[_selectedBuilding]
                : "No data available";

            _spriteBatch.DrawString(_uiFont, title, new Vector2(popupRect.X + 15, popupRect.Y + 12), Color.Black);
            _spriteBatch.DrawString(_uiFont, info, new Vector2(popupRect.X + 15, popupRect.Y + 50), Color.Black);
        }
        
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}