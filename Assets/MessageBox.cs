using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    #region Private Memebers

    /* The text component that this messageBox is attached to */
    Text text;

    #endregion

    #region Private Methods

    private void Start()
    {
        // finding the text component
        text = GetComponent<Text>();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Hides the MessageBox.
    /// </summary>
    public void Hide()
    {
        text.gameObject.SetActive(false);
    }

    /// <summary>
    /// Shows the MessageBox.
    /// </summary>
    public void Show()
    {
        text.gameObject.SetActive(true);
    }

    /// <summary>
    /// Sets the text and the font size of the MessageBox.
    /// </summary>
    /// <param name="text">The text to set.</param>
    /// <param name="fontSize">The font size to set.</param>
    public void SetText(string text, int fontSize)
    {
        this.text.text = text;
        this.text.fontSize = fontSize;
    }


    #endregion
}
