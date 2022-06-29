using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using nsRabuka;

//チェックポイント群親クラス
public class CheckPoint : MonoBehaviour
{
    public Rabuka rabuka;
    public int frameNum;//変化が起こるフレームの位置番号
    public GameObject targetObject;

    public bool titlebarFold;//表示するとき省略しているかどうかを保持できるかテスト

    void Start()
    {
        this.rabuka = GameObject.Find(Macro.rabukaObjName).GetComponent<Rabuka>();
    }
}
