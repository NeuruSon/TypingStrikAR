using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HomeController : MonoBehaviour
{
    public TextMeshProUGUI rMT, hMT, scriptT;
    public GameObject scriptP, sT, elimsP;
    public bool isScriptGoOn, isTouched, breath, f1Checker, f2Checker = false;
    public int forScript = 0;
    float eW, eH;

    public bool isDevelopMode = false;
    bool isDeleteMode = false;

    int resetCount = 0;

    void Awake()
    {
        rMT.text = "/ " + YourData.requiredMemo; //필요한 메모 수 
        //if (isDevelopMode || isDeleteMode) //개발자 모드면 싹 날리고 시작. 
        //{
        //    SaveController.deleteDatas();
        //}
        //if(isDeleteMode)
        //{
        //    Debug.Log("초기화 완료");
        //    Application.Quit();
        //}
        loadDatas();
        //if (isDevelopMode)
        //{
        //    YourData.isFirst = 0;
        //    YourData.requiredMemo = 1;
        //    YourData.nativeMemo = 1;
        //}
    }
    void Start()
    {
        eW = 1236.762f; eH = 2729.405f;
        checkFirstUser();
        rMT.text = "/ " + YourData.requiredMemo; //필요한 메모 수 
        hMT.text = "" + YourData.nativeMemo; //갖고 있는 메모 수
        if (YourData.requiredMemo == YourData.nativeMemo && YourData.isFinished == 0)
        {
            finishEnough();
        }
    }

    void Update()
    {
        breathE();
        if (!isScriptGoOn && scriptP.activeSelf) //스크립트 끝나고 아직 켜져있으면  
        {
            forScript = 0;
            scriptP.SetActive(false);
            if (YourData.isFirst == 2 && f1Checker)
            {
                YourData.isFirst = 1;
            }
            else if (YourData.isFirst == 1 && f2Checker)
            {
                YourData.isFirst = 0;
                gameObject.GetComponent<SceneController>().toGameScene();
            }
            if (YourData.requiredMemo == YourData.nativeMemo && YourData.isFinished == 0)
            {
                YourData.isFinished = 1;
            }
        }
        else if (!isScriptGoOn && !scriptP.activeSelf && isTouched)
        {
            isTouched = false;
            //현재 상태 출력
            showYourAchivement();
        }

        if (Input.touchCount == 0) { return; }
        Touch touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began) { return; }

        if (!isScriptGoOn && Input.touchCount < 2 && !scriptP.activeSelf && isTouched)
        {
            isTouched = false;
            //현재 상태 출력
            showYourAchivement();
        }

        if (resetCount >= 10)
        {
            Application.Quit();
        }
    }

    void loadDatas()
    {
        if (SaveController.loadDatas() == null) //만약 초회라 데이터 파일이 없다면 로드하지 않고 초기 데이터값으로 시작함. 
        {
            return;
        }

        int[] loadedDatas = SaveController.loadDatas();
        YourData.nativeMemo = loadedDatas[0];
        YourData.requiredMemo = loadedDatas[1];
        YourData.listenedWordNum = loadedDatas[2];
        YourData.clearedWordNum = loadedDatas[3];
        YourData.failedWordNum = loadedDatas[4];
        YourData.timeoutNum = loadedDatas[5];
        YourData.distanceYouMoved = loadedDatas[6];
        YourData.isFirst = loadedDatas[7];
        YourData.isFinished = loadedDatas[8];
    }

    void checkFirstUser()
    {
        if (YourData.isFirst == 2)
        {
            f1Checker = true;
            isScriptGoOn = true;
            forScript = 1;
            scriptP.SetActive(true);
        }
        if (YourData.isFirst == 1)
        {
            f2Checker = true;
            isScriptGoOn = true;
            scriptP.SetActive(true);
            sT.SetActive(true);
            sT.GetComponent<TypeWriterEffect>().fulltext = ScriptData.secondScript;
            sT.GetComponent<TypeWriterEffect>().Get_Typing(ScriptData.secondScript.Length, ScriptData.secondScript);
        }
    }

    void showYourAchivement()
    {
        isScriptGoOn = true;
        forScript = 2;
        scriptP.SetActive(true);
        sT.SetActive(true);
        sT.GetComponent<TypeWriterEffect>().fulltext = ScriptData.currentAchivement;
        sT.GetComponent<TypeWriterEffect>().Get_Typing(ScriptData.currentAchivement.Length, ScriptData.currentAchivement);
    }

    void finishEnough()
    {
        isScriptGoOn = true;
        scriptP.SetActive(true);
        sT.SetActive(true);
        sT.GetComponent<TypeWriterEffect>().fulltext = ScriptData.finished;
        sT.GetComponent<TypeWriterEffect>().Get_Typing(ScriptData.finished.Length, ScriptData.finished);
    }

    public void elimsBtn()
    {
        isTouched = true;
    }

    public void firstStartBtn()
    {
        if (YourData.isFirst == 1) //처음이면 도움말 스크립트. 
            checkFirstUser();
        else { gameObject.GetComponent<SceneController>().toGameScene(); } //아니면 다음씬. 
    }

    void breathE()
    {
        if (eH > 2750f)
        {
            breath = true;
        }
        else if (eH < 2700f)
        {
            breath = false;
        }
        if (breath)
        {
            eH /= 1.00001009f;
        }
        else
        {
            eH *= 1.00001005f;
        }
        elimsP.GetComponent<RectTransform>().sizeDelta = new Vector2(eW, eH);

        //if (breath) //움직임 
        //{
        //    if (eH < 2750f)
        //    {
        //        if (eH > 23)
        //        {

        //        }
        //    }
        //    elimsP.GetComponent<RectTransform>().sizeDelta = new Vector2(eW, eH);
        //}
        //else //안 움직임 
        //{
        //    Invoke("breathReality", 0.5f); //0.5초의 텀 뒤에 움직임. 
        //}
    }

    //void breathReality()
    //{
    //    breath = true;
    //}

    public void resetBtn()
    {
        resetCount++;
        if (resetCount >= 10)
        {
            SaveController.deleteDatas();
            resetCount = 0;
        }
    }
}
