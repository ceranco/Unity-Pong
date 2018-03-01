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

    [SerializeField] uint winScore = 0;

    /* The Message box */
    //[SerializeField]  Text messageBox = null;
    [SerializeField] MessageBox messageBox = null;

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
    /// Starts a countdown timer and starts the game. Should be called from different thread.
    /// </summary>
    /// <param name="seconds">The amount to countdown</param>
    private void CountdownAndStart(ConcurrentQueue<System.Action> actionQueue, int seconds = 3)
    {
        int i = seconds;
        actionQueue.Enqueue(() =>
        {
            ballControl.SetActive(false);
            messageBox.Show();
        });
        while (i > 0)
        {
            actionQueue.Enqueue(() => { messageBox.SetText(i.ToString(), 75); });
            Thread.Sleep(1000);
            i--;
        }
        actionQueue.Enqueue(() =>
        {
            messageBox.Hide();
            ballControl.SetActive(true);
            ballControl.RestartBallAndGo();
        });
    }

    /// <summary>
    /// Shows a message for a fixed amount of time. Should be called from another thread.
    /// </summary>
    /// <param name="actionQueue"></param>
    /// <param name="message"></param>
    /// <param name="fontSize"></param>
    /// <param name="millis"></param>
    private void TimedMessage(ConcurrentQueue<System.Action> actionQueue, string message, int fontSize, int millis)
    {
        actionQueue.Enqueue(() =>
        {
            messageBox.Show();
            messageBox.SetText(message, fontSize);
        });
        Thread.Sleep(millis);
        actionQueue.Enqueue(() => { messageBox.Hide(); });
    }

    private void StopPlaying()
    {
        messageBox.Hide();

        // hide the ball and reset the score counter
        ballControl.SetActive(false);
        scorePlayer01 = 0;
        player01ScoreText.text = $"Score: {scorePlayer01}";
        scorePlayer02 = 0;
        player02ScoreText.text = $"Score: {scorePlayer01}";
        gameButton.GetComponentInChildren<Text>().text = "Start";
        playing = false;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Function that should be called by the ball.
    /// </summary>
    /// <param name="wallPosition">The position of the wall</param>
    public void PointScored(WallPosition wallPosition)
    {
        int scoringPlayer;
        switch (wallPosition)
        {
            case WallPosition.RightWall:
                scoringPlayer = 1;
                scorePlayer01++;
                player01ScoreText.text = $"Score: {scorePlayer01}";
                break;
            case WallPosition.LeftWall:
                scoringPlayer = 2;
                scorePlayer02++;
                player02ScoreText.text = $"Score: {scorePlayer02}";
                break;
            default:
                return;
        }

        ballControl.SetActive(false);

        if (scorePlayer01 == winScore || scorePlayer02 == winScore)
        {
            int winningPlayer = scorePlayer01 == winScore ? 1 : 2;

            // change the queue so that any ongoing tasks will not fill it, and make sure that the message box is hidden
            actionQueue = new ConcurrentQueue<System.Action>();

            StopPlaying();
            Task.Run(() => TimedMessage(actionQueue, $"Player {winningPlayer} Wins!", 60, 1000));
            playing = false;
        }

        else
        {
            var queue = actionQueue;
            Task.Run(() =>
            {
                TimedMessage(queue, $"Player {scoringPlayer} Scored!", 60, 1000);
                CountdownAndStart(queue);
            });
        }

    }

    public void OnGameButtonClick()
    {
        // if we are playing, change the queue so that any ongoing tasks will not fill it, and make sure that the message box is hidden
        actionQueue = new ConcurrentQueue<System.Action>();

        if (!playing)
        {
            // if we are not playing, start the countdown and change the button text to 'Stop'
            gameButton.GetComponentInChildren<Text>().text = "Stop";
            Task.Run(() => CountdownAndStart(actionQueue));
            playing = true;
        }
        else
        {
            StopPlaying();
        }

    }

    #endregion
}

