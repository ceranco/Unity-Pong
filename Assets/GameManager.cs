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

        Task.Run(() => CountdownAndStart());
    }

    private void Update()
    {
        System.Action action;
        while (actionQueue.TryDequeue(out action))
        {
            action();
        }
    }

    public void OnGameButtonClick()
    {
        if (!playing)
        {
            gameButton.GetComponentInChildren<Text>().text = "Stop";
            Task.Run(() => CountdownAndStart());
            playing = true;
        }
        else
        {
            ballControl.gameObject.SetActive(false);
            scorePlayer01 = 0;
            player01ScoreText.text = $"Score: {scorePlayer01}";
            scorePlayer02 = 0;
            player02ScoreText.text = $"Score: {scorePlayer01}";
            gameButton.GetComponentInChildren<Text>().text = "Start";
            playing = false;
        }

    }

    private async Task CountdownAndStart(int seconds = 3)
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
}

