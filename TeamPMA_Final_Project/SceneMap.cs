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

    private static List<(Vector2 position, string buildingName, bool isTallBuilding)> _buildingInformation =
        new List<(Vector2 position, string buildingName, bool isTallBuilding)>
        {
            (new Vector2(430, 120), "BuildingA", false),
            (new Vector2(1275, 120), "BuildingB", false),
            (new Vector2(700, 550), "BuildingC", true),
            (new Vector2(650, 700), "BuildingD", true),
            (new Vector2(775, 700), "BuildingE", true),
            (new Vector2(1150, 900), "BuildingF", true)
        };

    private static Dictionary<string, float> _amenities =
        new Dictionary<string, float>
        {
            { "BuildingA", 0.1f },
            { "BuildingB", 0.2f },
            { "BuildingC", 0.5f },
            { "BuildingD", 0.7f },
            { "BuildingE", 0.7f },
            { "BuildingF", 0.7f }
        };

    private static Dictionary<string, float> _rentPrices =
        new Dictionary<string, float>
        {
            { "BuildingA", 0.1f },
            { "BuildingB", 0.2f },
            { "BuildingC", 0.5f },
            { "BuildingD", 0.7f },
            { "BuildingE", 0.7f },
            { "BuildingF", 0.6f }
        };

    private static Dictionary<string, string> _displayNames =
        new Dictionary<string, string>
        {
            { "BuildingA", "Galileo Condos" },
            { "BuildingB", "Walter Webb Hall" },
            { "BuildingC", "The Standard" },
            { "BuildingD", "The Mark" },
            { "BuildingE", "The Union" },
            { "BuildingF", "Moon Tower" }
        };

    private static Dictionary<string, string> _popupDescriptions =
        new Dictionary<string, string>
        {
            { "BuildingA", "Rent: $\nAmenities: Nothing" },
            { "BuildingB", "Rent: $$\nAmenities: Gym" },
            { "BuildingC", "Rent: $$\nAmenities: Pool,Gym" },
            { "BuildingD", "Rent: $$$\nAmenities: Pool,Gym,Sauna" },
            { "BuildingE", "Rent: $$$\nAmenities: Pool,Gym,Sauna" },
            { "BuildingF", "Rent: $$$\nAmenities: Pool,Gym" }
        };

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

        _btnPrev = new ToggleButton(prevTex, prevTex, new Rectangle(20, 50, 40, 40), _buttonClickSound);
        _btnPlayStop = new ToggleButton(stopTex, playTex, new Rectangle(70, 50, 40, 40), _buttonClickSound);
        _btnNext = new ToggleButton(nextTex, nextTex, new Rectangle(120, 50, 40, 40), _buttonClickSound);

        saveFavorite = new ToggleButton(toggleOnTex, toggleOffTex, new Rectangle(22, 440, 75, 75), _buttonClickSound);
        saveFavorite.Opacity = 1f;

        rentHeatmapToggle = new ToggleButton(toggleOnTex, toggleOffTex, new Rectangle(1100, 50, 75, 75), _buttonClickSound);
        rentHeatmapToggle.Opacity = 1f;

        amenitiesHeatmapToggle = new ToggleButton(toggleOnTex, toggleOffTex, new Rectangle(1100, 150, 75, 75), _buttonClickSound);
        amenitiesHeatmapToggle.Opacity = 1f;

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

        starButton = new ToggleButton(starOnTex, starOffTex, new Rectangle(300, 660, 40, 40), _buttonClickSound);
        starButton.Opacity = 1f;

        _heatMap = new HeatMap(
            _mapTexture,
            _shortBuilding,
            _tallBuilding,
            _buildingInformation,
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
                _heatMap.PullData(_rentPrices);
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
                _heatMap.PullData(_amenities);
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

        _btnPrev.Draw(spriteBatch);
        _btnPlayStop.Draw(spriteBatch);
        _btnNext.Draw(spriteBatch);

        rentHeatmapToggle.Draw(spriteBatch);
        amenitiesHeatmapToggle.Draw(spriteBatch);
        saveFavorite.Draw(spriteBatch);

        spriteBatch.DrawString(_uiFont, "Rent Heat Map", new Vector2(1000, 20), Color.Black);
        spriteBatch.DrawString(_uiFont, "Radio Controls", new Vector2(20, 25), Color.Black);
        spriteBatch.DrawString(_uiFont, "Amenities", new Vector2(1043, 220), Color.Black);
        spriteBatch.DrawString(_uiFont, "West Campus Heat Map", new Vector2(400, 22), Color.Black);
        spriteBatch.DrawString(_uiFont, "Save Favs", new Vector2(12, 400), Color.Black);

        if (_selectedBuilding != null)
        {
            Rectangle popupRect = new Rectangle(30, 650, 320, 110);

            spriteBatch.Draw(_popupPixel, popupRect, Color.White * 0.9f);
            spriteBatch.Draw(_popupPixel, new Rectangle(popupRect.X, popupRect.Y, popupRect.Width, 2), Color.Black);
            spriteBatch.Draw(_popupPixel, new Rectangle(popupRect.X, popupRect.Bottom - 2, popupRect.Width, 2), Color.Black);
            spriteBatch.Draw(_popupPixel, new Rectangle(popupRect.X, popupRect.Y, 2, popupRect.Height), Color.Black);
            spriteBatch.Draw(_popupPixel, new Rectangle(popupRect.Right - 2, popupRect.Y, 2, popupRect.Height), Color.Black);

            string title = _displayNames.ContainsKey(_selectedBuilding)
                ? _displayNames[_selectedBuilding]
                : _selectedBuilding;

            string info = _popupDescriptions.ContainsKey(_selectedBuilding)
                ? _popupDescriptions[_selectedBuilding]
                : "No data available";

            spriteBatch.DrawString(_uiFont, title, new Vector2(popupRect.X + 15, popupRect.Y + 12), Color.Black);
            spriteBatch.DrawString(_uiFont, info, new Vector2(popupRect.X + 15, popupRect.Y + 50), Color.Black);

            starButton.Draw(spriteBatch);
        }
    }
}