using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;

public class Bird : MonoBehaviour{

    private const float JUMP_AMOUNT = 80f;

    private static Bird instance;

    public static Bird GetInstance(){
        return instance;
    }

    #region Event
    public event EventHandler OnDied;
    public event EventHandler OnStartedPlaying;
    #endregion

    private Rigidbody2D birdRigidbody2D;
    private State state;


    private enum State{
        WaitingToStart,
        Playing,
        Dead
    }

    private void Awake(){
        instance = this;
        birdRigidbody2D = GetComponent<Rigidbody2D>();
        birdRigidbody2D.bodyType = RigidbodyType2D.Static;
        state = State.WaitingToStart;
    }

    private void Update(){
        switch (state){
            case State.WaitingToStart:
                    OnWaitingGame();
                 break;
            case State.Playing:
                    OnPlayingGame();
                 break;
            default:
                break;
        }
    }


    #region InputGameEvent
    private void OnWaitingGame() 
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {

            state = State.Playing;
            birdRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            Jump();
            if (OnStartedPlaying != null) OnStartedPlaying(this, EventArgs.Empty);

        }
    }

    private void OnPlayingGame()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Jump();
            GameHandler.GetInstance().IncreaseTapCount();
        }

        transform.eulerAngles = new Vector3(0, 0, birdRigidbody2D.velocity.y * .15f);
    }
    #endregion

    private void Jump(){
        birdRigidbody2D.velocity = Vector2.up * JUMP_AMOUNT;
        SoundManager.Playsound(SoundManager.Sound.BirdJump);
    }

    private void OnTriggerEnter2D(Collider2D collider){
        birdRigidbody2D.bodyType = RigidbodyType2D.Static;
        SoundManager.Playsound(SoundManager.Sound.Lose);
        if (OnDied != null) OnDied(this, EventArgs.Empty);
    }
}
