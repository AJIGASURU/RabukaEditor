using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CheckPointText : CheckPoint
{
    public char[] textChar;
    public string text;

    void Start()
    {
        this.rabuka = GameObject.Find("Rabuka").GetComponent<Rabuka>();
    }

    void Update()
    {
        if(this.rabuka.frame == this.frameNum)
        {
            LoadCheckPoint();
        }
    }

    public void SetCheckPoint(GameObject targetObject, int frame)
    {
        this.targetObject = targetObject;
        this.text = targetObject.GetComponent<Text>().text;
        Debug.Log(text);
        this.frameNum = frame;
    }

    public void LoadCheckPoint()//これポインタ的に使えるのか？
    {
        this.targetObject.GetComponent<Text>().text = this.text;
    }
}
