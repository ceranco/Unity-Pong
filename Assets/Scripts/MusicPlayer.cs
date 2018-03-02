using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    #region Private Members

    /* A list of all the music to play */
    [SerializeField] List<AudioClip> musicList;

    int currentClip = 0;

    AudioSource audioSource;

    bool isPlaying = false;
    bool isPaused = false;

    #endregion

    #region Public Properties

    /// <summary>
    /// The volume of the music.
    /// </summary>
    public float Volume
    {
        get { return audioSource.volume; }
        set { audioSource.volume = value; }
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ShuffleMusicList();
    }

    private void Update()
    {
        if (isPlaying && !isPaused && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(musicList[currentClip++]);
        }
    }

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

    public void StartPlaying()
    {
        currentClip = 0;
        audioSource.PlayOneShot(musicList[currentClip++]);
        isPlaying = true;
    }

    public void PausePlaying()
    {
        audioSource.Pause();
    }

    public void ResumePlaying()
    {
        audioSource.UnPause();
    }

    #endregion
}
