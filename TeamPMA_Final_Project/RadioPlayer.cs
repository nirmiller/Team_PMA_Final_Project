using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace TeamPMA_Final_Project;

public class RadioPlayer
{
    private List<Song> _playlist;
    private int _currentSongIndex = 0;

    public RadioPlayer(List<Song> songs)
    {
        _playlist = songs;
        MediaPlayer.IsRepeating = false; // We want to move to the NEXT song, not repeat one forever
    }

    public void Play()
    {
        if (MediaPlayer.State != MediaState.Playing)
        {
            MediaPlayer.Play(_playlist[_currentSongIndex]);
        }
    }

    public void Stop()
    {
        MediaPlayer.Stop(); // You can also use MediaPlayer.Pause() if you prefer!
    }

    public void NextSong()
    {
        _currentSongIndex++;
        if (_currentSongIndex >= _playlist.Count)
            _currentSongIndex = 0; // Loop to the beginning

        MediaPlayer.Play(_playlist[_currentSongIndex]);
    }

    public void PreviousSong()
    {
        _currentSongIndex--;
        if (_currentSongIndex < 0)
            _currentSongIndex = _playlist.Count - 1; // Loop to the end

        MediaPlayer.Play(_playlist[_currentSongIndex]);
    }

    public void Update()
    {
        // Auto-play the next song if the current one finishes naturally
        if (MediaPlayer.State == MediaState.Stopped && _playlist.Count > 0)
        {
            // NextSong(); // Uncomment this if you want it to auto-play the next track!
        }
    }
}