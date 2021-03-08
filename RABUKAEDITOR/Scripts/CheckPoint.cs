using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckPoint : MonoBehaviour
{
    public Rabuka rabuka;
    public int frameNum;//変化が起こるフレームの位置番号
    public GameObject targetObject;

    void Start()
    {
        this.rabuka = GameObject.Find("Rabuka").GetComponent<Rabuka>();
    }
}
