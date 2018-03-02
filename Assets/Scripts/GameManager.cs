using System;
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
    [SerializeField] MessageBox messageBox = null;

    /* The Countdown Timer */
    [SerializeField] CountdownTimer countdownTimer = null;

    /* The buttons */
    [SerializeField] Button gameButton = null;

    /* The ball */
    [SerializeField] BallControl ballControl = null;

    /* Playing flag */
    bool playing = false;

    #endregion

    #region Private Methods

    /// <summary>
    /// Starts the current turn.
    /// </summary>
    private void StartTurn()
    {
        UnityEngine.Debug.Log("<color=green>Starting turn</color>");
        ballControl.SetActive(true);
        ballControl.RestartBallAndGo();
    }

    /// <summary>
    /// Starts the game.
    /// </summary>
    private void StartGame()
    {
        gameButton.GetComponentInChildren<Text>().text = "Stop";

        // start the turn at the end of the 3 second countdown of the countdown timer
        countdownTimer.CountdownFinishedOnce += (object sender, EventArgs e) => StartTurn();
        countdownTimer.StartCountdown(3, 1);
        playing = true;
    }

    /// <summary>
    /// Ends the game.
    /// </summary>
    private void EndGame()
    {
        UnityEngine.Debug.Log("<color=#800000ff>Stopping playing</color>");

        // stop countdown if in progress
        countdownTimer.StopCountdown();

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
            // if the game is won, then stop it and show an appropiate message
            EndGame();
            messageBox.TimedMessage($"Player {scoringPlayer} Wins!", 60, 3);
            playing = false;
        }

        else
        {
            // if the game is to be continued, show appropiate message, start the countdown timer and start anothe turn
            messageBox.TimedMessage($"Player {scoringPlayer} Scored!", 60, 1, () =>
            {
                // start the turn at the end of the 3 second countdown of the countdown timer
                countdownTimer.CountdownFinishedOnce += (object sender, EventArgs e) => StartTurn();
                countdownTimer.StartCountdown(3, 1);
            });
        }

    }

    public void OnGameButtonClick()
    {
        if (!playing)
        {
            StartGame();
        }
        else
        {
            EndGame();
        }

    }

    #endregion

}

