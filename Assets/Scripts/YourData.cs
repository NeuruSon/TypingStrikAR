using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class YourData
{
    public static int nativeMemo = 0; //지금까지 작성한 주민 메모지 
    public static int requiredMemo = 10; //모아야 하는 메모지 양 
    public static int listenedWordNum = 0; //지금까지 들은 단어 수 
    public static int clearedWordNum = 0; //지금까지 제대로 작성한 단어 수 
    public static int failedWordNum = 0; //제대로 작성하지 못한 단어 수(엔터 버튼을 눌러 비교했을 때 틀렸다고 나온 경우)
    public static int timeoutNum = 0; //타임아웃된 횟수 
    public static int distanceYouMoved = 0; //지금까지 움직인 거리
    public static int isFirst = 2; //1~2이면 처음, 0이면 처음 아님
    public static int isFinished = 0;
}