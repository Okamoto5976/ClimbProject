using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class InitializeManager : MonoBehaviour
{
    public GameObject Player;
    public List<GameObject> Enemy = new List<GameObject>();

    public void GameInitialize()
    {
        //各マネージャーのInitialize　また参照を渡す
        //各マネージャーはデータを生成する
        
        //必要なデータがあれば取得する
        //インターネットの場合
        //ClientIdごとにカメラをセットしたり
    }
}
