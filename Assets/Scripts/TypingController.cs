using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TypingController : MonoBehaviour
{
    //수정할 것: 클리어하면 주민은 오브젝트째로 사라짐. 페일이거나 피니시면 안 사라짐. 고려해서 bool 변수 나눠놓기. 

    //조금 더 독립적으로, 그러나 의존적으로. 게임 한 판이 끝날 때까지 변하지 않는 값은 받아 저장해 사용함. 수정될 여지가 있는 값은 모두 gCon이 총괄하고 실시간으로 가져와 사용. 
    public GameObject gCon, spawnCube, cPanel, pBP, pRP, cBP, cRP, fBP, fRP, tP, fP; //패널 모음. tP(timeoutPanel) fP(finishPanel) 
    public TextMeshProUGUI qC, aC, cBT, cRT, tIT; //제시문과 입력문. 
    public TMP_InputField inputClause;
    //public bool isOvered, isPresented, isCleared = false; //분리하게 됐으니 조심해야 함! 
    public bool isStarted, isOvered, isPresented, isCleared = false; 
    public Slider rSlider;

    int totalClauseNum, beforeMemoNum = 0;
    string[] clauseQList; //gCon에서 가져옴.
    float tStopwatch, tTimer, timerSave; //문장 작성까지 걸린 시간 측정(스톱워치), 탈락까지 남은 작성 시간(타이머). 타이머 값 초기화에 사용할 임시 변수. 
    TouchScreenKeyboard iosKey;


    void Start()
    {
        beforeMemoNum = YourData.nativeMemo; //시작 당시의 가진 메모 수 저장.
        timerSave = 10f; tTimer = timerSave; //빨간 주민 타이머 10초 세팅.
        tStopwatch = 0f;
        clauseQList = gCon.GetComponent<GameController>().clauseQList; //중앙에서 내려받기.
        totalClauseNum = gCon.GetComponent<GameController>().nT;
    }

    void Update()
    {
        if(isStarted) //처음에만 쓰이는 함수. 
        {
            isStarted = false;
            showPanel();
        }

        if (isOvered == true) 
        {
            tTimer = 0f; //게임 끝나면 타이머 정지 후 초기화 
            gCon.GetComponent<GameController>().isOvered = true;
            showPanel(); //게임 끝난 뒤 골인 여부 판단해 게임 클리어로 종료인지 게임 오버로 종료인지 판단 후 분기 나눔.
        }

        if (isPresented && gCon.GetComponent<GameController>().currentClauseNum < totalClauseNum && !isOvered) //아직 입력할 문장이 남아있으면 
        {
            if (Input.GetKeyDown(KeyCode.Return) || iosKey.status == TouchScreenKeyboard.Status.Done) //엔터 눌러도 비교 가능. done 눌러도 비교 가능. 
            {
                showPanel();
            }
        }
        else if (!isPresented && gCon.GetComponent<GameController>().currentClauseNum == totalClauseNum && !isOvered) //모든 문제를 클리어 했다면 
        {
            isOvered = true;
            CancelInvoke();
            YourData.nativeMemo++; //주민 단어 메모지 획득 
        }

        if (isPresented && !isOvered) //게임 진행중-문제 출제중이면 
        {
            tStopwatch += Time.deltaTime; //스톱워치 
            if (isNativeColorRed()) //빨간 주민이면? 
            {
                tTimer -= Time.deltaTime;
                rSlider.value = tTimer * 10;
                if (tTimer <= 0) { isOvered = true; }
            }
        }
    }

    void presentClause() //문장 제시.
    {
        YourData.listenedWordNum++;
        qC.text = clauseQList[gCon.GetComponent<GameController>().currentClauseNum].Trim();
        cPanel.SetActive(true);
        inputClause.ActivateInputField(); //활성화.

        //키보드 꺼내기
        iosKey = TouchScreenKeyboard.Open("d", TouchScreenKeyboardType.Default, false);
    }

    public void matchClause() //제시 문장(출제 문장)과 입력 문장이 동일한가 확인하는 메소드. 
    {
        Debug.Log(inputClause.text.Trim() + " / " + clauseQList[gCon.GetComponent<GameController>().currentClauseNum].Trim());

        if (inputClause.text.Trim() == clauseQList[gCon.GetComponent<GameController>().currentClauseNum].Trim()) //string 앞뒤의 쓸데없는 공백을 제외한 문자열이 완전히 동일하면 
        {
            YourData.clearedWordNum++;
            inputClause.Select(); inputClause.text = ""; //클리어.
            isCleared = true;
            gCon.GetComponent<GameController>().nC++; //클리어 수 보고. 
        }
        else //다르면
        {
            YourData.failedWordNum++;
            inputClause.Select(); inputClause.text = ""; //클리어.
            inputClause.ActivateInputField(); //활성화.
        }
    }

    bool isNativeColorRed()
    {
        foreach (int temp in gCon.GetComponent<GameController>().rNativeNum)
        {
            if (gCon.GetComponent<GameController>().currentClauseNum == temp) //지금 나올 주민이 붉은색일 차례인가 확인. 
            {
                return true;
            }
        }

        return false;
    }

    public void showPanel() //문제 제시, 정답 패널, 오답 패널, 게임 오버 패널. 
    {
        if (isCleared) { } //성공 판정 중에는 패스함. 
        else if (isPresented != true && isOvered != true) //문제 제시할 차례. 
        {
            if(isNativeColorRed()) //빨간 주민 차례면 
            {
                pRP.SetActive(true);
                presentClause();
            }
            else
            {
                pBP.SetActive(true);
                presentClause();
            }
            isPresented = true;
        }
        else if (isPresented == true && isOvered != true) //타이핑 성공 혹은 실패.  
        {
            matchClause();

            if (isNativeColorRed()) //빨간 주민 차례면 
            {
                if (isCleared) //성공 
                {
                    hidePanel('p'); tTimer = timerSave;
                    cRT.text = string.Format("{0:0.0}", tStopwatch) + "초";
                    tStopwatch = 0f; //스톱워치 초기화. 
                    cRP.SetActive(true);
                    gCon.GetComponent<GameController>().currentClauseNum++;
                    //isCleared = false;
                }  
                else { fRP.SetActive(true); } //실패 
                Invoke("hidePanel", 1f);
            }
            else
            {
                if (isCleared) //성공 
                {
                    hidePanel('p'); 
                    cBT.text = string.Format("{0:0.0}", tStopwatch) + "초";
                    tStopwatch = 0f;
                    cBP.SetActive(true);
                    gCon.GetComponent<GameController>().currentClauseNum++;
                    //isCleared = false;
                }
                else { fBP.SetActive(true); } //실패 
                Invoke("hidePanel", 1f);
            }
        }
        else //if (isOvered == true) //타임아웃이거나 빨간 주민에 닿았거나. 
        {
            //게임 끝난 뒤 패널 부르면 메모 수 비교해 게임 클리어로 종료인지 게임 오버로 종료인지 판단 후 분기 나눔.
            if (beforeMemoNum < YourData.nativeMemo) //처음 시작할 때 갖고 있던 메모 수보다 현재 가진 메모 수가 더 많으면(클리어했으면) 
            {
                //게임 성공으로 클리어 패널 
                fP.SetActive(true);
            }
            else
            {
                if(inputClause.enabled)
                {
                    inputClause.Select(); inputClause.enabled = false; //입력 불가. 
                    YourData.timeoutNum++;
                }
                //게임오버 패널
                if (isPresented) { tIT.text = "그만 쫓겨나 버렸다!"; } //빨간 주민 게임 오버 
                else { tIT.text = "너무 더워서 탈진해 버렸다..."; }
                tP.SetActive(true);
            }
        }
    }

    void hidePanel() //문제 패널 제외 숨김. 
    {
        cBP.SetActive(false);
        cRP.SetActive(false);
        fBP.SetActive(false);
        fRP.SetActive(false);
        if (isCleared)
        {
            isCleared = !isCleared;
            isPresented = false;
        }
    }
    void hidePanel(char role) //특정 역할 패널 숨김. 
    {
        switch (role)
        {
            case 'p':
                cPanel.SetActive(false);
                pBP.SetActive(false);
                pRP.SetActive(false);
                break;
            case 'c':
                cBP.SetActive(false);
                cRP.SetActive(false);
                break;
            case 'f':
                fBP.SetActive(false);
                fRP.SetActive(false);
                break;
        }

        if(isCleared)
        {
            gCon.GetComponent<GameController>().isClause = false;
            spawnCube.GetComponent<SpawnTranslate>().spawnTranslate();
            Invoke("isNotSpawned", 4f);
        }
    }

    void isNotSpawned()
    {
        gCon.GetComponent<GameController>().isSpawned = false;
    }
}
