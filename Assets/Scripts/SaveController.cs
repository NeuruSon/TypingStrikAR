using System.Collections;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveController
{
    public static void saveDatas()
    {
        BinaryFormatter formatter = new BinaryFormatter(); //이진 변환 객체 생성 
        FileStream streamer = new FileStream(Application.persistentDataPath + "/you.sav", FileMode.Create); //파일 입출력 객체 생성. 파일 생성 
        PlayerData data = new PlayerData(); //데이터 객체 생성 
        formatter.Serialize(streamer, data); //암호화  
        streamer.Close(); //파일 변환 종료 
    }

    public static int[] loadDatas()
    {
        if (File.Exists(Application.persistentDataPath + "/you.sav")) //세이브 파일이 있으면 
        {
            BinaryFormatter formatter = new BinaryFormatter(); //이진 변환 객체 생성 
            FileStream streamer = new FileStream(Application.persistentDataPath + "/you.sav", FileMode.Open); //파일 열기 
            PlayerData data = formatter.Deserialize(streamer) as PlayerData; //복호화하고 데이터 값으로 assign해줌 
            streamer.Close(); //파일 입출력 종료
            return data.datas; //데이터 값 반환 
        }

        saveDatas(); //없으면 만들기 
        return null; //아무것도 반환하지 않음 
    }

    public static void deleteDatas()
    {
        if (File.Exists(Application.persistentDataPath + "/you.sav")) //세이브 파일이 있으면 
        {
            string path = Application.persistentDataPath + "/you.sav";
            FileInfo fileInfo = new FileInfo(path);
            fileInfo.Delete(); //지운다.
            loadDatas();
        }
    }
}

[Serializable]
public class PlayerData
{
    public int[] datas;
    public PlayerData()
    {
        datas = new int[9];
        datas[0] = YourData.nativeMemo;
        datas[1] = YourData.requiredMemo;
        datas[2] = YourData.listenedWordNum;
        datas[3] = YourData.clearedWordNum;
        datas[4] = YourData.failedWordNum;
        datas[5] = YourData.timeoutNum;
        datas[6] = YourData.distanceYouMoved;
        datas[7] = YourData.isFirst;
        datas[8] = YourData.isFinished;
    }
}