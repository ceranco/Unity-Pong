using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    #region Private Members

    /* The text of the timer */
    Text timerText;

    /* The sound the timer makes on a tick */
    [SerializeField] AudioClip tickSound = null;

    /* The sound the timer makes on end of countdown */
    [SerializeField] AudioClip endOfCountdownSound = null;

    /* The audio source of the timer */
    AudioSource audioSource;

    /* The current countdown coroutine */
    Coroutine currentCountdown;

    #endregion

    #region Public Properties


    /// <summary>
    /// The font size of the timer.
    /// </summary>
    public int FontSize
    {
        get { return timerText.fontSize; }
        set { timerText.fontSize = value; }
    }


    /// <summary>
    /// Whether the timer is active.
    /// </summary>
    public bool IsActive
    {
        get { return gameObject.activeSelf; }
        private set { gameObject.SetActive(value); }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Initializing the GameObject.
    /// </summary>
    private void Start()
    {
        // getting the text componet
        timerText = GetComponent<Text>();

        // getting the audio source component
        audioSource = GetComponent<AudioSource>();

        // starting the timer as not active
        IsActive = false;
    }

    /// <summary>
    /// Coroutine for the countdown.
    /// </summary>
    /// <param name="startNumber">The number to count down from.</param>
    /// <param name="delay">The delay between each count in seconds.</param>
    /// <returns></returns>
    private IEnumerator CountdownRoutine(uint startNumber, float delay)
    {
        for (int i = (int)startNumber; i > 0; i--)
        {
            timerText.text = i.ToString();
            audioSource.PlayOneShot(tickSound);
            yield return new WaitForSeconds(delay);
        }
        // raising the CountdownFinshed event
        audioSource.PlayOneShot(endOfCountdownSound);
        OnCountdownFinished(new EventArgs());
        OnCountdownFinishedOnce(new EventArgs());
        
        // hiding the timer until the end of countdown sound is finished playing, then disabling the gameobject
        timerText.text = "";
        yield return new WaitUntil(() => !audioSource.isPlaying);
        IsActive = false;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Starts the count down coroutine.
    /// </summary>
    /// <param name="startNumber">The number to count down from.</param>
    /// <param name="delay">The delay between each count in seconds.</param>
    public void StartCountdown(uint startNumber, float delay)
    {
        if (!IsActive)
        {
            IsActive = true;
            currentCountdown = StartCoroutine(CountdownRoutine(startNumber, delay));
        }
    }

    /// <summary>
    /// Stops the current countdown if in progress. Also clears the <see cref="CountdownFinishedOnce"/> handlers.
    /// </summary>
    public void StopCountdown()
    {
        if (IsActive)
        {
            StopCoroutine(currentCountdown);
        }
        CountdownFinishedOnce = null;
        IsActive = false;
    }

    #endregion

    #region Event Members

    /// <summary>
    /// Event that fires when the countdown is finished.
    /// </summary>
    public event EventHandler CountdownFinished;

    /// <summary>
    /// Event that fires when the countdown is finished, then removes all of the subscribed handler.
    /// </summary>
    public event EventHandler CountdownFinishedOnce;

    protected virtual void OnCountdownFinished(EventArgs e)
    {
        CountdownFinished?.Invoke(this, e);
    }

    protected virtual void OnCountdownFinishedOnce(EventArgs e)
    {
        CountdownFinishedOnce?.Invoke(this, e);
        CountdownFinishedOnce = null;
    }

    #endregion
}
