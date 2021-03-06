using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CheckPointText : CheckPoint
{
    public string text;

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
        this.frameNum = frame;
    }

    public void LoadCheckPoint()//これポインタ的に使えるのか？
    {
        this.targetObject.GetComponent<Text>().text = this.text;
    }
}
