using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Private Members
    /* The scores of the players */
    [SerializeField] Text player01ScoreText = null;
    [SerializeField] Text player02ScoreText = null;
    uint scorePlayer01 = 0;
    uint scorePlayer02 = 0;

    /* The Message box */
    [SerializeField] Text messageBox = null;

    /* A threadsafe queue of action to happen in the Update phase */
    ConcurrentQueue<System.Action> actionQueue = new ConcurrentQueue<System.Action>();

    /* The buttons */
    [SerializeField] Button gameButton = null;

    /* The ball */
    [SerializeField] BallControl ballControl = null;

    /* Playing flag */
    bool playing = false;

    #endregion

    #region Private Methods

    /// <summary>
    /// On each update, do all the actions in the queue.
    /// </summary>
    private void Update()
    {
        System.Action action;
        while (actionQueue.TryDequeue(out action))
        {
            action();
        }
    }

    /// <summary>
    /// Starts a countdown timer in another thread.
    /// </summary>
    /// <param name="seconds">The amount to countdown</param>
    private async Task CountdownAndStart(ConcurrentQueue<System.Action> actionQueue, int seconds = 3)
    {
        int i = seconds;
        actionQueue.Enqueue(() =>
        {
            ballControl.gameObject.SetActive(false);
            messageBox.gameObject.SetActive(true);
            messageBox.fontSize = 75;
        });
        while (i > 0)
        {
            actionQueue.Enqueue(() => { messageBox.text = i.ToString(); });
            await Task.Delay(1000);
            i--;
        }
        actionQueue.Enqueue(() =>
        {
            messageBox.gameObject.SetActive(false);
            ballControl.gameObject.SetActive(true);
            ballControl?.RestartBallAndGo();
        });
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Function that should be called by the ball.
    /// </summary>
    /// <param name="wallPosition">The position of the wall</param>
    public void PointScored(WallPosition wallPosition)
    {
        switch (wallPosition)
        {
            case WallPosition.RightWall:
                scorePlayer01++;
                player01ScoreText.text = $"Score: {scorePlayer01}";
                break;
            case WallPosition.LeftWall:
                scorePlayer02++;
                player02ScoreText.text = $"Score: {scorePlayer02}";
                break;
            default:
                return;
        }

        Task.Run(() => CountdownAndStart(actionQueue));
    }

    public void OnGameButtonClick()
    {
        if (!playing)
        {
            // if we are not playing, start the countdown and change the button text to 'Stop'
            gameButton.GetComponentInChildren<Text>().text = "Stop";
            Task.Run(() => CountdownAndStart(actionQueue));
            playing = true;
        }
        else
        {
            // if we are playing, change the queue so that any ongoing tasks will not fill it, and make sure that the message box is hidden
            actionQueue = new ConcurrentQueue<System.Action>();
            messageBox.gameObject.SetActive(false);

            // hide the ball and reset the score counter
            ballControl.gameObject.SetActive(false);
            scorePlayer01 = 0;
            player01ScoreText.text = $"Score: {scorePlayer01}";
            scorePlayer02 = 0;
            player02ScoreText.text = $"Score: {scorePlayer01}";
            gameButton.GetComponentInChildren<Text>().text = "Start";
            playing = false;
        }

    }

    #endregion
}

