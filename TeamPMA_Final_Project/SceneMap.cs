using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TeamPMA_Final_Project;

public class SceneMap : Scene
{
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
    private bool _previousSaveFavoriteState = false;

    private SoundEffect _buttonClickSound;
    private ToggleButton saveFavorite;
    private ToggleButton starButton;
    private MouseState _previousMouseState;
    private string _selectedBuilding = null;
    private Texture2D _popupPixel;

    private RadioPlayer _radio;
    private ToggleButton _btnPrev;
    private ToggleButton _btnPlayStop;
    private ToggleButton _btnNext;

    private bool _prevPrevState = false;
    private bool _prevPlayStopState = false;
    private bool _prevNextState = false;

    public SceneMap(Game game) : base(game)
    {
    }

    public override void LoadContent()
    {
        Texture2D toggleOnTex = Game.Content.Load<Texture2D>("imgs/on_button7");
        Texture2D toggleOffTex = Game.Content.Load<Texture2D>("imgs/off_button7");

        _uiFont = Game.Content.Load<SpriteFont>("imgs/fontt");
        _buttonClickSound = Game.Content.Load<SoundEffect>("imgs/506054__mellau__button-click-1");

        Song track1 = Game.Content.Load<Song>("imgs/447515__alittlebitdrunkguy__simple-lofi-hip-hop-track-n");
        Song track2 = Game.Content.Load<Song>("imgs/song2");
        Song track3 = Game.Content.Load<Song>("imgs/song3");

        List<Song> myPlaylist = new List<Song> { track1, track2, track3 };
        _radio = new RadioPlayer(myPlaylist);

        Texture2D stopTex = Game.Content.Load<Texture2D>("imgs/stop_button-transp");
        Texture2D playTex = Game.Content.Load<Texture2D>("imgs/play_button-transp");
        Texture2D prevTex = Game.Content.Load<Texture2D>("imgs/play_left-transp");
        Texture2D nextTex = Game.Content.Load<Texture2D>("imgs/play_right-transp");

        _btnPrev = new ToggleButton(prevTex, prevTex, new Rectangle(35, 75, 34, 34), _buttonClickSound);
        _btnPlayStop = new ToggleButton(stopTex, playTex, new Rectangle(82, 75, 34, 34), _buttonClickSound);
        _btnNext = new ToggleButton(nextTex, nextTex, new Rectangle(129, 75, 34, 34), _buttonClickSound);
        
        rentHeatmapToggle = new ToggleButton(toggleOnTex, toggleOffTex, new Rectangle(1115, 120, 50, 50), _buttonClickSound);
        rentHeatmapToggle.Opacity = 1f;

        amenitiesHeatmapToggle = new ToggleButton(toggleOnTex, toggleOffTex, new Rectangle(1115, 180, 50, 50), _buttonClickSound);
        amenitiesHeatmapToggle.Opacity = 1f;

        saveFavorite = new ToggleButton(toggleOnTex, toggleOffTex, new Rectangle(1115, 240, 50, 50), _buttonClickSound);
        saveFavorite.Opacity = 1f;

        _mapTexture = Game.Content.Load<Texture2D>("imgs/map");
        _shortBuilding = Game.Content.Load<Texture2D>("imgs/short_building");
        _tallBuilding = Game.Content.Load<Texture2D>("imgs/tall_building");

        Texture2D starOnTex = Game.Content.Load<Texture2D>("imgs/star_on");
        Texture2D starOffTex = Game.Content.Load<Texture2D>("imgs/star_off");

        _bubbleTexture = Bubble.CreateCircleTexture(Game.GraphicsDevice, 64);

        float mapScale = 0.8f;
        float mapDrawWidth = _mapTexture.Width * mapScale;
        float mapDrawHeight = _mapTexture.Height * mapScale;

        mapPosition = new Vector2(
            (Game.Graphics.PreferredBackBufferWidth - mapDrawWidth) / 2f,
            (Game.Graphics.PreferredBackBufferHeight - mapDrawHeight) / 2f
        );

        starButton = new ToggleButton(starOnTex, starOffTex, new Rectangle(55, 715, 45, 45), _buttonClickSound);
        starButton.Opacity = 1f;

        List<(Vector2 position, string buildingName, bool isTallBuilding)> buildingInformation =
            GetBuildingInformation();

        _heatMap = new HeatMap(
            _mapTexture,
            _shortBuilding,
            _tallBuilding,
            buildingInformation,
            mapPosition
        );

        _heatMap.ResetMap();

        _popupPixel = new Texture2D(Game.GraphicsDevice, 1, 1);
        _popupPixel.SetData(new[] { Color.White });
    }

    public override void Update(GameTime gameTime)
    {
        MouseState currentMouse = Mouse.GetState();

        rentHeatmapToggle.Update(currentMouse);
        amenitiesHeatmapToggle.Update(currentMouse);
        saveFavorite.Update(currentMouse);

        _btnPrev.Update(currentMouse);
        _btnPlayStop.Update(currentMouse);
        _btnNext.Update(currentMouse);
        _radio.Update();

        if (_btnPrev.IsOn != _prevPrevState)
        {
            if (_btnPrev.IsOn)
            {
                _radio.PreviousSong();
                _btnPlayStop.IsOn = true;
                _btnPrev.IsOn = false;
            }

            _prevPrevState = _btnPrev.IsOn;
        }

        if (_btnPlayStop.IsOn != _prevPlayStopState)
        {
            if (_btnPlayStop.IsOn)
            {
                _radio.Play();
            }
            else
            {
                _radio.Stop();
            }

            _prevPlayStopState = _btnPlayStop.IsOn;
        }

        if (_btnNext.IsOn != _prevNextState)
        {
            if (_btnNext.IsOn)
            {
                _radio.NextSong();
                _btnPlayStop.IsOn = true;
                _btnNext.IsOn = false;
            }

            _prevNextState = _btnNext.IsOn;
        }

        if (rentHeatmapToggle.IsOn != _previousRentState)
        {
            if (rentHeatmapToggle.IsOn)
            {
                _heatMap.ResetMap();
                _heatMap.PullData(GetRentDictionary());
                _heatMap.AnimateHeatMap(
                    true,
                    Game.Graphics.PreferredBackBufferWidth,
                    Game.Graphics.PreferredBackBufferHeight,
                    Color.Red
                );
            }
            else
            {
                _heatMap.ResetMap();
            }

            _previousRentState = rentHeatmapToggle.IsOn;
        }

        if (amenitiesHeatmapToggle.IsOn != _previousAmenitiesState)
        {
            if (amenitiesHeatmapToggle.IsOn)
            {
                _heatMap.ResetMap();
                _heatMap.PullData(GetAmenitiesDictionary());
                _heatMap.AnimateHeatMap(
                    true,
                    Game.Graphics.PreferredBackBufferWidth,
                    Game.Graphics.PreferredBackBufferHeight,
                    Color.Green
                );
            }
            else
            {
                _heatMap.ResetMap();
            }

            _previousAmenitiesState = amenitiesHeatmapToggle.IsOn;
        }

        if (currentMouse.LeftButton == ButtonState.Pressed &&
            _previousMouseState.LeftButton == ButtonState.Released)
        {
            string clickedBuilding = _heatMap.GetClickedBuilding(currentMouse.Position);

            if (clickedBuilding != null)
            {
                _selectedBuilding = clickedBuilding;
                starButton.IsOn = Game.SaveManager.GetFavorites().Contains(_selectedBuilding);
            }
            else
            {
                if (!starButton.IsHovered || _selectedBuilding == null)
                {
                    _selectedBuilding = null;
                }
            }
        }

        if (_selectedBuilding != null)
        {
            bool previousStarState = starButton.IsOn;
            starButton.Update(currentMouse);

            if (starButton.IsOn != previousStarState)
            {
                if (starButton.IsOn)
                {
                    Game.SaveManager.AddFavorite(_selectedBuilding);
                }
                else
                {
                    Game.SaveManager.RemoveFavorite(_selectedBuilding);
                }
            }
        }

        if (saveFavorite.IsOn != _previousSaveFavoriteState)
        {
            if (saveFavorite.IsOn)
            {
                Game.SaveManager.Save(Game.SavePath);
                saveFavorite.IsOn = false;
            }

            _previousSaveFavoriteState = saveFavorite.IsOn;
        }

        _heatMap.Update(gameTime);
        _previousMouseState = currentMouse;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        _heatMap.DrawMap(
            spriteBatch,
            Game.Graphics.PreferredBackBufferWidth,
            Game.Graphics.PreferredBackBufferHeight
        );

        _heatMap.Draw(spriteBatch, _bubbleTexture);

        // Right-side controls panel
        Rectangle controlsPanel = new Rectangle(985, 70, 195, 235);
        spriteBatch.Draw(_popupPixel, controlsPanel, Color.Black * 0.65f);

// Panel border
        spriteBatch.Draw(_popupPixel, new Rectangle(controlsPanel.X, controlsPanel.Y, controlsPanel.Width, 2), Color.White * 0.25f);
        spriteBatch.Draw(_popupPixel, new Rectangle(controlsPanel.X, controlsPanel.Bottom - 2, controlsPanel.Width, 2), Color.White * 0.25f);
        spriteBatch.Draw(_popupPixel, new Rectangle(controlsPanel.X, controlsPanel.Y, 2, controlsPanel.Height), Color.White * 0.25f);
        spriteBatch.Draw(_popupPixel, new Rectangle(controlsPanel.Right - 2, controlsPanel.Y, 2, controlsPanel.Height), Color.White * 0.25f);

        spriteBatch.DrawString(_uiFont, "Controls", new Vector2(1005, 90), Color.White);
        spriteBatch.DrawString(_uiFont, "Rent", new Vector2(1005, 132), Color.LightGray);
        spriteBatch.DrawString(_uiFont, "Amenities", new Vector2(1005, 192), Color.LightGray);
        spriteBatch.DrawString(_uiFont, "Save", new Vector2(1005, 252), Color.LightGray);
        rentHeatmapToggle.Draw(spriteBatch);
        amenitiesHeatmapToggle.Draw(spriteBatch);
        saveFavorite.Draw(spriteBatch);

        // Top-left radio panel
        Rectangle radioPanel = new Rectangle(20, 35, 160, 90);
        spriteBatch.Draw(_popupPixel, radioPanel, Color.Black * 0.55f);

        spriteBatch.Draw(_popupPixel, new Rectangle(radioPanel.X, radioPanel.Y, radioPanel.Width, 2), Color.White * 0.25f);
        spriteBatch.Draw(_popupPixel, new Rectangle(radioPanel.X, radioPanel.Bottom - 2, radioPanel.Width, 2), Color.White * 0.25f);
        spriteBatch.Draw(_popupPixel, new Rectangle(radioPanel.X, radioPanel.Y, 2, radioPanel.Height), Color.White * 0.25f);
        spriteBatch.Draw(_popupPixel, new Rectangle(radioPanel.Right - 2, radioPanel.Y, 2, radioPanel.Height), Color.White * 0.25f);

        spriteBatch.DrawString(_uiFont, "Music", new Vector2(55, 42), Color.White);

        _btnPrev.Draw(spriteBatch);
        _btnPlayStop.Draw(spriteBatch);
        _btnNext.Draw(spriteBatch);
        
        spriteBatch.DrawString(_uiFont, "West Campus Heat Map", new Vector2(400, 22), Color.Black);
        //spriteBatch.DrawString(_uiFont, "Radio Controls", new Vector2(20, 25), Color.Black);
        if (_selectedBuilding != null)
        {
            DrawPopup(spriteBatch);
        }
    }

    private List<(Vector2 position, string buildingName, bool isTallBuilding)> GetBuildingInformation()
    {
        List<(Vector2 position, string buildingName, bool isTallBuilding)> buildingInformation =
            new List<(Vector2 position, string buildingName, bool isTallBuilding)>();

        foreach (BuildingData building in Game.BuildingDataManager.GetBuildings().Values)
        {
            buildingInformation.Add(
                (building.Position, building.Id, building.IsTallBuilding)
            );
        }

        return buildingInformation;
    }

    private Dictionary<string, float> GetRentDictionary()
    {
        Dictionary<string, float> rentDictionary = new Dictionary<string, float>();

        foreach (BuildingData building in Game.BuildingDataManager.GetBuildings().Values)
        {
            rentDictionary[building.Id] = building.RentValue;
        }

        return rentDictionary;
    }

    private Dictionary<string, float> GetAmenitiesDictionary()
    {
        Dictionary<string, float> amenitiesDictionary = new Dictionary<string, float>();

        foreach (BuildingData building in Game.BuildingDataManager.GetBuildings().Values)
        {
            amenitiesDictionary[building.Id] = building.AmenitiesValue;
        }

        return amenitiesDictionary;
    }

    private void DrawPopup(SpriteBatch spriteBatch)
{
    int screenHeight = Game.Graphics.PreferredBackBufferHeight;

    Rectangle cardRect = new Rectangle(35, screenHeight - 330, 420, 285);
    
    // Dark transparent card
    spriteBatch.Draw(_popupPixel, cardRect, Color.Black * 0.72f);

    // Soft border
    spriteBatch.Draw(_popupPixel, new Rectangle(cardRect.X, cardRect.Y, cardRect.Width, 2), Color.White * 0.22f);
    spriteBatch.Draw(_popupPixel, new Rectangle(cardRect.X, cardRect.Bottom - 2, cardRect.Width, 2), Color.White * 0.22f);
    spriteBatch.Draw(_popupPixel, new Rectangle(cardRect.X, cardRect.Y, 2, cardRect.Height), Color.White * 0.22f);
    spriteBatch.Draw(_popupPixel, new Rectangle(cardRect.Right - 2, cardRect.Y, 2, cardRect.Height), Color.White * 0.22f);
    
    // Divider lines inside the card
    spriteBatch.Draw(
        _popupPixel,
        new Rectangle(cardRect.X + 25, cardRect.Y + 115, cardRect.Width - 50, 1),
        Color.White * 0.18f
    );
    spriteBatch.Draw(
        _popupPixel,
        new Rectangle(cardRect.X + 25, cardRect.Y + 205, cardRect.Width - 50, 1),
        Color.White * 0.18f
    );

    BuildingData building = Game.BuildingDataManager.GetBuilding(_selectedBuilding);

    string title = building != null ? building.DisplayName.ToUpper() : _selectedBuilding;
    string subtitle = building != null ? building.Subtitle : "West Campus Housing";
    string rent = building != null ? building.RentText : "No rent data";
    string amenities = building != null ? building.AmenitiesText : "No amenities data";

    // Only show a shorter amenities preview
    string amenitiesLine1 = GetAmenityLine(amenities, 0);
    string amenitiesLine2 = GetAmenityLine(amenities, 1);
    
    bool isFavorited = Game.SaveManager.GetFavorites().Contains(_selectedBuilding);

    int leftX = cardRect.X + 25;
    int textX = cardRect.X + 35;

    // Header
    spriteBatch.DrawString(_uiFont, title, new Vector2(leftX, cardRect.Y + 18), Color.White);
    spriteBatch.DrawString(_uiFont, subtitle, new Vector2(leftX, cardRect.Y + 52), Color.LightGray);

    // Rent section
    
    spriteBatch.DrawString(_uiFont, "RENT", new Vector2(textX, cardRect.Y + 82), Color.LightGray);
    spriteBatch.DrawString(_uiFont, rent, new Vector2(textX, cardRect.Y + 108), Color.White);

    // Amenities section
   
    spriteBatch.DrawString(_uiFont, "AMENITIES", new Vector2(textX, cardRect.Y + 145), Color.LightGray);
    spriteBatch.DrawString(_uiFont, amenitiesLine1, new Vector2(textX, cardRect.Y + 170), Color.White);
    spriteBatch.DrawString(_uiFont, amenitiesLine2, new Vector2(textX, cardRect.Y + 202), Color.White);
    
    // Favorite section
    string favoriteText = isFavorited ? "FAVORITED" : "NOT FAVORITED";
    Color favoriteColor = isFavorited ? Color.Gold : Color.LightGray;

   
    spriteBatch.DrawString(_uiFont, favoriteText, new Vector2(textX, cardRect.Y + 238), favoriteColor);
    starButton.Draw(spriteBatch);
}
    
    private string GetAmenityLine(string amenities, int lineNumber)
    {
        string[] parts = amenities.Split(',');
        if (lineNumber == 0)
        {
            string result = "";
            for (int i = 0; i < parts.Length && i < 2; i++)
            {
                if (i > 0)
                {
                    result += ", ";
                }
                result += parts[i].Trim();
            }
            return result;
        }
        if (lineNumber == 1)
        {
            if (parts.Length >= 3)
            {
                return parts[2].Trim();
            }
        }
        return "";
    }
    
}