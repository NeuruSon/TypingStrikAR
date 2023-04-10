using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

//[RequireComponent(typeof(ARPlaneManager))]

public class GameController : MonoBehaviour
{
    public GameObject tCon, rNM, bNM; //typingController, red/blue native model prefabs, infoQuadCon
    public TextMeshProUGUI nTotal, nCleared, tTimer;
    public TextAsset clauseDB;
    public string[] clauseQList;
    public int[] rNativeNum; //제시문 수에 맞춰 적당한 규칙에 따라 지정. but 인덱스 넘버 기준으로 적어둬야 함. 3, 7
    [HideInInspector] public int nT, nC, currentClauseNum;
    public bool isOvered, isNCol, isClause, isSpawned = false;
    public float distance = 20f;
    public Slider gTimerSlider;

    //private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private Camera cam;
    //public ARRaycastManager aRM; public ARSessionOrigin aSO;
    //ARPlaneManager aPM;
    public Transform startNativeT, targetObject; public GameObject startNative;

    float fTimer = 180.00f; //180초. 3분. 아무리 오래 걸려도 3분 이상 플레이X.
    public float gTimer = 0f; //게임 시작용 응시 타이머 
    bool isStarted = false; //isDetectingSpawn = false;
    AudioSource currentBgm;

    private void Awake()
    {
        rNativeNum = null;
        switch (gacha())
        {
            case 0:
                rNativeNum = new int[3];
                rNativeNum[0] = 1;
                rNativeNum[1] = 3;
                rNativeNum[2] = 4;
                break;
            case 1:
                rNativeNum = new int[2];
                rNativeNum[0] = 2;
                rNativeNum[1] = 4;
                break;
            case 2:
                rNativeNum = new int[3];
                rNativeNum[0] = 0;
                rNativeNum[1] = 2;
                rNativeNum[2] = 4;
                break;
            case 3:
                rNativeNum = new int[1];
                rNativeNum[0] = 4;
                break;
            case 4:
                rNativeNum = new int[2];
                rNativeNum[0] = 1;
                rNativeNum[1] = 3;
                break;
        }
        gTimerSlider.value = 0f;
        //aPM = GetComponent<ARPlaneManager>();
        nT = 5; nC = 0; currentClauseNum = 0;
        isStarted = true;
        clauseQList = new string[nT];
        nTotal.text = "/ " + nT;
        nCleared.text = "" + nC;
        selectClause(nT); //출제 문장 선택.
        currentBgm = GetComponent<AudioSource>();
    }

    void Start()
    {
        cam = Camera.main;
        //isSpawned = true;
        //pOp.GetComponent<PlaceOnPlane>().startGameNative(); //첫 주민 소환 
        //startGameNative(); //첫 주민 소환
        //tTimer.text = (int) fTimer + ":" + (Math.Truncate(fTimer * 100) - (int) fTimer * 100); //값은 원하는 대로 나오지만 소수점 부분이 00이 아니라 0 출력.
        tTimer.text = (string.Format("{0:000.00}", fTimer)).Replace(".", ":"); //000.00 형태 유지. 값이 없더라도 무조건 0을 표시하고 원작고증을 위해 초와 밀리초 사이 표기를 :로 교체. 
    }

    void Update()
    {
        //dozgu();
        perseptionNative();
        gTimerSlider.value = gTimer * 100f;

        if (isOvered != true) //게임이 끝나기 전에는 계속 타이머를 작동(업데이트)시킴. 
        {
            tTimer.text = (string.Format("{0:000.00}", fTimer)).Replace(".", ":");
            nCleared.text = "" + nC;

            if (isStarted == true && fTimer > 0f)
            {
                fTimer -= Time.deltaTime;
                tTimer.text = (string.Format("{0:000.00}", fTimer)).Replace(".", ":");
            }
            else if (fTimer < 0f) //타임오버(게임오버). 
            {
                fTimer = 0; //고정.
                isOvered = true; //게임 끝.
                tCon.GetComponent<TypingController>().isOvered = true;
            }

            if (isStarted && isNCol && !isClause)
            {
                gTimer += Time.deltaTime;
            }
            else if (isClause)
            {
                gTimer = 0f;
            }
            else if (!isNCol)
            {
                gTimer -= Time.deltaTime;
                if (gTimer <= 0)
                {
                    gTimer = 0f;
                }
            }
        }
        else //게임 끝.
        {
            fTimer = 0; //고정.
            currentBgm.Stop();
            if (isStarted)
            {
                isStarted = false;
                Invoke("toHomeScene", 6); //n초 후 홈화면으로. 
            }
        }
    }

    void selectClause(int totalClause) //출제 문장 결정 메소드. 
    {
        string[] clauseDBList = clauseDB.text.Split('\n'); //텍스트DB 파일을 읽어와 엔터 기준으로 끊어서 나눠 넣음.
        if (clauseDBList.Length < totalClause) //만약 DB 양이 정해둔 출제 수보다 적으면 
        {
            Debug.Log("DB부족!"); //부족 로그를 띄우고 
            return; //탈출! 
        }

        for(int i = 0; i < totalClause;) //정해둔 수만큼 
        {
            int r = Random.Range(0, clauseDBList.Length); //문장을 뽑는데 
            if (clauseDBList[r] != "") //이미 뽑힌 문장이 아니라면 
            {
                clauseQList[i] = clauseDBList[r]; //출제 문장 리스트에 추가하고 
                clauseDBList[r] = ""; //"배열"에서 삭제해준다. 원본 텍스트 파일에는 그대로 남아있음.
                i++; //다음!
            }
            else { } //이미 뽑힌 문장이라면 다시 뽑는다 
        }

    }

    void toHomeScene()
    {
        gameObject.GetComponent<SceneController>().toHomeScene();
    }

    bool isNativeColorRed()
    {
        foreach (int temp in rNativeNum)
        {
            if (currentClauseNum == temp) //지금 나올 주민이 붉은색일 차례인가 확인. 
            {
                return true;
            }
        }

        return false;
    }

    //void startGameNative()
    //{
    //    if (aRM.Raycast(cam.transform.position, hits, TrackableType.Planes))
    //    {
    //        Pose hitPose = hits[0].pose;
    //        startNative.SetActive(true);
    //        aSO.MakeContentAppearAt(targetObject, hitPose.position - new Vector3(0, 0, 20), hitPose.rotation);
    //        tCon.GetComponent<TypingController>().isStarted = true;
    //        isSpawned = true;
    //    }
    //}

    //void dozgu()
    //{
    //    if (Input.touchCount == 0)
    //    {
    //        return;
    //    }

    //    Touch touch = Input.GetTouch(0);

    //    if (touch.phase != TouchPhase.Began)
    //    {
    //        return;
    //    }

    //    if (Input.touchCount < 2 && !isDetectingSpawn)
    //    {

    //        if (aRM.Raycast(touch.position, hits, TrackableType.Planes))
    //        {
    //            Pose hitPose = hits[0].pose;

    //            //TogglePlaneDetection();

    //            startGameNative();
    //            isDetectingSpawn = true;
    //        }
    //    }
    //}

    //public void TogglePlaneDetection()
    //{
    //    aPM.enabled = !aPM.enabled;

    //    if (aPM.enabled)
    //    {
    //        SetAllPlanesActive(true);
    //    }
    //    else { SetAllPlanesActive(false); }
    //}

    //public void SetAllPlanesActive(bool value)
    //{
    //    foreach (var plane in aPM.trackables)
    //        plane.gameObject.SetActive(value);
    //}

    //public void ChangeRespawnStatus()
    //{
    //    isDetectingSpawn = false;
    //    aSO.MakeContentAppearAt(targetObject, Vector3.zero, Quaternion.Euler(0, 0, 0));
    //    GameObject.FindGameObjectWithTag("SpawnObject").transform.GetChild(0).gameObject.SetActive(false);
    //    GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(0).gameObject.SetActive(false);
    //}

    public void spawnNative(Transform tr)
    {

        //Vector3 targetTR = new Vector3(targetObject.transform.position.x + 30, targetObject.transform.position.y + 30, targetObject.transform.position.z + 30);
        //Transform targetTR = target.transform; //반경 어느 위치에서(랜덤) 스폰하도록 조정.

        //Vector3 rayOrigin = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        //Vector3 rayDirection = cam.transform.forward;
        //isSpawned = true;
        if (isNativeColorRed())
        {
            //빨강
            GameObject rN = Instantiate(rNM, tr) as GameObject;
        }
        else if (!isNativeColorRed())
        {
            //파랑
            GameObject bN = Instantiate(bNM, tr) as GameObject;
        }

        //if (aRM.Raycast(cam.transform.position, hits, TrackableType.Planes)) //평면 감지 
        //{
        //    //Pose hitPose = hits[0].pose;
        //    //RaycastHit hit;



        //isSpawned = true;
        //}
        ////현재 문장 번호로 색 구분해서 instantiate

    }

    void perseptionNative()
    {
        Vector3 rayOrigin = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        Vector3 rayDirection = cam.transform.forward;

        if (!isClause)
        {
            if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit) && hit.collider.gameObject.CompareTag("Native"))
            {
                if (Vector3.Distance(cam.transform.position, hit.collider.gameObject.transform.position) < 20)
                {
                    isNCol = true;
                }
                else
                {
                    isNCol = false;
                }
            }
            else
            {
                isNCol = false;
            }
            if (gTimer > 5.0f)
            {
                isClause = true;
                tCon.GetComponent<TypingController>().showPanel(); //문제 함수 작동.
            }
            if (!isSpawned)
            {
                isSpawned = true;
                spawnNative(targetObject);
            }
        }
    }

    int gacha()
    {
        int dice = Random.Range(0, 5);

        return dice;
    }
}