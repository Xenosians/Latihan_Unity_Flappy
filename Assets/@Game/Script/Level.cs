using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class Level : MonoBehaviour
{
    private const float CAMERA_ORTHO_SIZE = 50f;
    private const float PIPE_WIDTH = 7.8f;
    private const float PIPE_HEAD_HEIGHT = 3.75f;
    private const float PIPE_MOVE_SPEED = 30f;
    private const float PIPE_DESTROY_X_POSITION = -100f;
    private const float PIPE_SPAWN_X_POSITION = +100f;
    private const float GROUND_DESTROY_X_POSITION = -200f;
    private const float CLOUD_DESTROY_X_POSITION = -160f;
    private const float CLOUD_SPAWN_X_POSITION = +160f;
    private const float CLOUD_SPAWN_Y_POSITION = +30f;
    private const float BIRD_X_POSITION = 0f;

    private static Level instance;

    public static Level GetInstance(){
        return instance;
    }

    private List<Transform> groundList;
    private List<Transform> cloudList;
    private float cloudSpawnTimer;
    private List<Pipe> pipeList;
    private int pipesPassedCount;
    private int pipeSpawned;
    private float pipeSpawnTimer;
    private float pipeSpawnTimerMax;
    private float gapSize;
    private State state;
    public DateTime dateTime;
    public DateTime dateTimeEnd;

    public enum Difficulty{
        Easy,
        Medium,
        Hard,
        Impossible,
    }

    private enum State{
        WaitingToStart,
        Playing,
        BirdDead,
    }

    private void Awake(){
        instance = this;
        SpawnInitialGround();
        SpawnInitialClouds();
        pipeList = new List<Pipe>();
        pipeSpawnTimerMax = 1f;
        SetDifficulty(Difficulty.Easy);
        state = State.WaitingToStart;

    }

    private void Start(){
        Bird.GetInstance().OnDied += Bird_OnDied;
        Bird.GetInstance().OnStartedPlaying += Bird_OnStartedPlaying;
    }

    private void Bird_OnStartedPlaying(object sender,System.EventArgs e){
        state = State.Playing;
        dateTime = DateTime.Now;
    }

    private void Bird_OnDied(object sender,System.EventArgs e){
        state = State.BirdDead;
        dateTimeEnd = DateTime.Now;

        GameHandler.GetInstance().SetTime(GetTime());
        GameHandler.GetInstance().SaveLog();
    }

    public TimeSpan GetTime() {
        return dateTimeEnd - dateTime;
    }

    private void Update(){
        if (state == State.Playing){
            HandlePipeMovement();
            HandlePipeSpawning();
            HandleGround();
            HandleCloud();
        }
    }

    private void SpawnInitialClouds(){
        cloudList = new List<Transform>();
        Transform cloudTransform;
        cloudTransform = Instantiate(GetcloudPrefabTransform(), new Vector3(0,CLOUD_SPAWN_Y_POSITION, 0), Quaternion.identity);
        cloudList.Add(cloudTransform);
    }

    private Transform GetcloudPrefabTransform(){
        switch (UnityEngine.Random.Range(0, 3)){
        default:
        case 0: return GameAssets.GetInstance().pfCloud_1;
        case 1: return GameAssets.GetInstance().pfCloud_2;
        case 2: return GameAssets.GetInstance().pfCloud_3;

        }
    }

    private void HandleCloud()
    {
        cloudSpawnTimer -= Time.deltaTime;
        if (cloudSpawnTimer < 0)
        {
            float cloudSpawnTimerMax = 5f;
            cloudSpawnTimer = cloudSpawnTimerMax;
            Transform cloudTransform = Instantiate(GetcloudPrefabTransform(), new Vector3(CLOUD_SPAWN_X_POSITION, CLOUD_SPAWN_Y_POSITION, 0), Quaternion.identity);
            cloudList.Add(cloudTransform);
        }
        for(int i = 0; i < cloudList.Count; i++){ 
            Transform cloudTransform = cloudList[i];
            cloudTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime * .7f;

            if (cloudTransform.position.x < CLOUD_DESTROY_X_POSITION)
            {
                Destroy(cloudTransform.gameObject);
                cloudList.RemoveAt(i);
                i++;
            }
        }
    }


    private void SpawnInitialGround(){
        groundList = new List<Transform>();
        Transform groundTransform;
        float groundY = -47.5f;
        float groundWidth = 192f;
        groundTransform = Instantiate(GameAssets.GetInstance().pfGround, new Vector3(0, groundY, 0), Quaternion.identity);
        groundList.Add(groundTransform);
        groundTransform = Instantiate(GameAssets.GetInstance().pfGround, new Vector3(groundWidth, groundY, 0), Quaternion.identity);
        groundList.Add(groundTransform);
        groundTransform = Instantiate(GameAssets.GetInstance().pfGround, new Vector3(groundWidth * 2f, groundY, 0), Quaternion.identity);
        groundList.Add(groundTransform);
    }

    private void HandleGround(){
        foreach(Transform groundTransform in groundList){
            groundTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;

            if(groundTransform.position.x < GROUND_DESTROY_X_POSITION){
                float rightMostXPosition = -100f;
                for (int i = 0; i < groundList.Count; i++){
                    if(groundList[i].position.x > rightMostXPosition){
                        rightMostXPosition = groundList[i].position.x;
                    }
                }

                float groundWidth = 192f;
                groundTransform.position = new Vector3(rightMostXPosition + groundWidth, groundTransform.position.y, groundTransform.position.z);
            }
        }
    }

    private void HandlePipeSpawning(){
        pipeSpawnTimer -= Time.deltaTime;
        if (pipeSpawnTimer < 0){
            pipeSpawnTimer += pipeSpawnTimerMax;

            float heightEdgeLimit = 10f;
            float minHeight = gapSize * .5f + heightEdgeLimit;
            float totalHeight = CAMERA_ORTHO_SIZE * 2f;
            float maxHeight = totalHeight - gapSize * .5f - heightEdgeLimit;

            float height = UnityEngine.Random.Range(minHeight, maxHeight);
            CreateGapPipes(height, gapSize, PIPE_SPAWN_X_POSITION);
        }
    }
    private void HandlePipeMovement(){
        for (int i = 0; i < pipeList.Count; i++) {
            Pipe pipe = pipeList[i];
            bool isToTherightOfBird = pipe.GetXPosition() > BIRD_X_POSITION;
            pipe.Move();
            if(isToTherightOfBird && pipe.GetXPosition() <= BIRD_X_POSITION && pipe.IsBottom()){
                pipesPassedCount++;
                GameHandler.GetInstance().IncreaseScore();
                SoundManager.Playsound(SoundManager.Sound.Score);
            }

            if (pipe.GetXPosition() < PIPE_DESTROY_X_POSITION ) {
                pipe.DestroySelf();
                pipeList.Remove(pipe);
                i--;
            }
           
        }
    }

    private void SetDifficulty(Difficulty difficulty){
       switch (difficulty){
       case Difficulty.Easy:
            gapSize = 50f;
            pipeSpawnTimerMax = 1.4f;
            break;
       case Difficulty.Medium:
            gapSize = 40f;
            pipeSpawnTimerMax = 1.3f;
            break;
       case Difficulty.Hard:
            gapSize = 33f;
            pipeSpawnTimerMax = 1.1f;
            break;
       case Difficulty.Impossible:
            gapSize = 20f;
            pipeSpawnTimerMax = 1.0f;
            break;
       }
    }

    private Difficulty GetDifficulty(){
        if (pipeSpawned >= 30) return Difficulty.Impossible;
        if (pipeSpawned >= 20) return Difficulty.Hard;
        if (pipeSpawned >= 10) return Difficulty.Medium;
        return Difficulty.Easy;
    }

    private void CreateGapPipes(float gapY,float gapSize,float xPosition){
        CreatePipe(gapY - gapSize * .5f, xPosition, true);
        CreatePipe(CAMERA_ORTHO_SIZE * 2f - gapY - gapSize * .5f, xPosition, false);
        pipeSpawned++;
        SetDifficulty(GetDifficulty());
    }

    private void CreatePipe(float height , float xPosition , bool createBottom) { 
        Transform pipeHead = Instantiate(GameAssets.GetInstance().pfPipeHead);
        float pipeHeadPosition;
        if (createBottom){
            pipeHeadPosition = -CAMERA_ORTHO_SIZE + height - PIPE_HEAD_HEIGHT * .5f;
        }
        else{
            pipeHeadPosition = +CAMERA_ORTHO_SIZE - height + PIPE_HEAD_HEIGHT * .5f;
        }
        pipeHead.position = new Vector3(xPosition,pipeHeadPosition);
     

        Transform pipeBody = Instantiate(GameAssets.GetInstance().pfPipeBody);
        float pipeBodyPosition;
        if (createBottom){
            pipeBodyPosition = -CAMERA_ORTHO_SIZE;
        }
        else{
            pipeBodyPosition = +CAMERA_ORTHO_SIZE;
            pipeBody.localScale = new Vector3(1, -1, 1);
        }
        pipeBody.position = new Vector3(xPosition,pipeBodyPosition );
        

        SpriteRenderer pipeBodySpriteRenderer = pipeBody.GetComponent<SpriteRenderer>();
        pipeBodySpriteRenderer.size = new Vector2(PIPE_WIDTH, height);

        BoxCollider2D pipeBodyBoxCollider = pipeBody.GetComponent<BoxCollider2D>();
        pipeBodyBoxCollider.size = new Vector2(PIPE_WIDTH, height);
        pipeBodyBoxCollider.offset = new Vector2(0f,height * .5f);

        Pipe pipe = new Pipe(pipeHead, pipeBody , createBottom);
        pipeList.Add(pipe);
    }

    public int GetPipesSpawned(){
        return pipeSpawned;
    }

    public int GetPipesPassedCount(){
        return pipesPassedCount;
    }

    private class Pipe{

        private Transform pipeHeadTransform;
        private Transform pipeBodyTransform;
        private bool isBottom;

        public Pipe(Transform pipeHeadTransform, Transform pipeBodyTransform,bool isBottom)
        {
            this.pipeHeadTransform = pipeHeadTransform;
            this.pipeBodyTransform = pipeBodyTransform;
            this.isBottom = isBottom;
        }

        public void Move(){
            pipeHeadTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
            pipeBodyTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
        }

        public float GetXPosition(){
            return pipeHeadTransform.position.x;
        }

        public bool IsBottom(){
            return isBottom;
        }

        public void DestroySelf(){
            Destroy(pipeHeadTransform.gameObject);
            Destroy(pipeBodyTransform.gameObject);
        }
    }
}