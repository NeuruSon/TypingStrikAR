using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void toGameScene()
    {
        SaveController.saveDatas(); //씬 전환 시 저장 
        SceneManager.LoadScene("Game_Main");
        loadDatas();
    }

    public void toHomeScene()
    {
        SaveController.saveDatas();
        SceneManager.LoadScene("Home");
        //홈은 로드가 내장됨. 
    }

    void loadDatas() //다만 씬이 추가될 가능성을 고려해 씬 교체 스크립트에 로드 데이터 내장. 
    {
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
}
