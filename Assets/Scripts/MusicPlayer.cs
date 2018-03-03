using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    #region Private Members

    /// <summary>
    /// A list of all the music to play.
    /// </summary>
    [SerializeField] List<AudioClip> musicList = null;

    /// <summary>
    /// Backing field for private <see cref="CurrentTrack"/> property.
    /// </summary>
    int currentTrack = 0;

    /// <summary>
    /// Backing field for private <see cref="AudioSource"/> property.
    /// </summary>
    AudioSource audioSource;

    bool isPlaying = false;
    bool isPaused = false;

    #endregion

    #region Properties

    /// <summary>
    /// The audio source of the music player.
    /// </summary>
    private AudioSource AudioSource
    {
        get
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
            return audioSource;
        }
    }
        
    /// <summary>
    /// The current track playing.
    /// </summary>
    private int CurrentTrack
    {
        get { return currentTrack; }
        set { currentTrack = value % musicList.Count; }
    }

    /// <summary>
    /// The volume of the music.
    /// </summary>
    public float Volume
    {
        get { return AudioSource.volume; }
        set { AudioSource.volume = value; }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Update logic.
    /// </summary>
    private void Update()
    {
        if (isPlaying && !isPaused && !AudioSource.isPlaying)
        {
            AudioSource.PlayOneShot(musicList[++CurrentTrack]);
        }
    }

    /// <summary>
    /// Shuffles the music playlist.
    /// </summary>
    private void ShuffleMusicList()
    {
        System.Random random = new System.Random();
        var tempList = new List<AudioClip>(musicList);
        int i = 0;
        while (tempList.Count != 0)
        {
            var randomIndex = random.Next(0, tempList.Count - 1);
            musicList[i++] = tempList[randomIndex];
            tempList.RemoveAt(randomIndex);
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Shuffles the playlist and starts playing.
    /// </summary>
    public void StartPlaying()
    {
        ShuffleMusicList();
        CurrentTrack = -1;
        NextTrack();
        isPlaying = true;
    }

    /// <summary>
    /// Pauses the track.
    /// </summary>
    public void PausePlaying()
    {
        isPaused = true;
        AudioSource.Pause();
    }

    /// <summary>
    /// Resumes the track.
    /// </summary>
    public void ResumePlaying()
    {
        isPaused = false;
        AudioSource.UnPause();
    }

    /// <summary>
    /// Skips to the next track.
    /// </summary>
    public void NextTrack()
    {
        AudioSource.Stop();
        CurrentTrack++;
        Debug.Log($"Playing Track {currentTrack}");
        AudioSource.PlayOneShot(musicList[CurrentTrack], 0.25f);
    }

    /// <summary>
    /// Goes back to the previous track.
    /// </summary>
    public void PreviousTrack()
    {
        CurrentTrack--;
        AudioSource.PlayOneShot(musicList[CurrentTrack]);
    }

    #endregion
}
