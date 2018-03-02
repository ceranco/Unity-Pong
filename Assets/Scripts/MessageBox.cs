using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    #region Private Members

    /* The text component that this messageBox is attached to */
    Text messageBoxText;

    /* The current timed message Coroutine */
    Coroutine currentTimedMessage;

    #endregion


    #region Public Properties

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

    private void Start()
    {
        // finding the text component
        messageBoxText = GetComponent<Text>();

        // starting the message box as not active
        IsActive = false;
    }

    private IEnumerator TimedMessageRoutine(string message, int fontSize, float time, Action action)
    {
        IsActive = true;
        messageBoxText.fontSize = fontSize;
        messageBoxText.text = message;
        yield return new WaitForSeconds(time);
        IsActive = false;
        action?.Invoke();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Show the message for the given amount of time, optionally performing an action at the end.
    /// </summary>
    /// <param name="message">The messag to show.</param>
    /// <param name="fontSize">The font size of the message.</param>
    /// <param name="time">The amount of time to show the message in seconds.</param>
    /// <param name="action">The action to take at the end of the message.</param>
    public void TimedMessage(string message, int fontSize, float time, Action action = null)
    {
        IsActive = true;
        if (currentTimedMessage != null)
        {
            StopCoroutine(currentTimedMessage);
        }
        currentTimedMessage = StartCoroutine(TimedMessageRoutine(message, fontSize, time, action));
    }


    #endregion
}
