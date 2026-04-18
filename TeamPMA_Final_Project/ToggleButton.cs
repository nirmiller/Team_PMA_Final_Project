using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio; // NEW: You need this using statement for Audio!
namespace TeamPMA_Final_Project;

public class ToggleButton
{
    // Visuals
        private Texture2D textureOn;
        private Texture2D textureOff;
        private Rectangle bounds;
        

        public float Opacity { get; set; } = 0.8f; // Setting it to 0.8f makes it 80% opaque by default

        // State tracking
        private MouseState previousMouse;
        private SoundEffect clickSound;
        
        public bool IsOn { get; set; }
        public bool IsHovered { get; private set; }

        /// <summary>
        /// Creates a new toggle button widget.
        /// </summary>
        /// <param name="texOn">Texture when the toggle is active.</param>
        /// <param name="texOff">Texture when the toggle is inactive.</param>
        /// <param name="position">The X, Y, Width, and Height of the button.</param>
        /// <param name="defaultState">Starting state of the toggle.</param>

        public ToggleButton(Texture2D texOn, Texture2D texOff, Rectangle position, SoundEffect sound, bool defaultState = false)
        {
            textureOn = texOn;
            textureOff = texOff;
            bounds = position;
            clickSound = sound; // Store the sound
            IsOn = defaultState;
        }

        public void Update(MouseState currentMouse)
        {
            // 1. Check if the mouse cursor is over the button
            IsHovered = bounds.Contains(currentMouse.Position);

            // 2. Check for a click (Left button was pressed last frame, but released this frame)
            if (IsHovered && 
                currentMouse.LeftButton == ButtonState.Released && 
                previousMouse.LeftButton == ButtonState.Pressed)
            {
                // Toggle the state
                IsOn = !IsOn; 
                
                clickSound?.Play(volume: 0.5f, pitch: 0.0f, pan: 0.0f);
            }

            // 3. Save the current state for the next frame's comparison
            previousMouse = currentMouse;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Pick the right texture based on the boolean state
            Texture2D currentTexture = IsOn ? textureOn : textureOff;
    
            // Determine the base color (with hover effect)
            Color baseColor = IsHovered ? Color.LightGray : Color.White;

            // Multiply the base color by the opacity to make it transparent
            Color finalColor = baseColor * Opacity;

            // Draw the button with the transparent color
            spriteBatch.Draw(currentTexture, bounds, finalColor);
        }
}