using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CheckPointCamera : CheckPoint
{
    /*
    public string text;
    public int fontSize;
    public Color color;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    */

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
        this.frameNum = frame;
        /*
        this.text = targetObject.GetComponent<Text>().text;
        this.fontSize = targetObject.GetComponent<Text>().fontSize;
        this.color = targetObject.GetComponent<Text>().color;
        this.position = targetObject.GetComponent<RectTransform>().localPosition;
        this.rotation = targetObject.GetComponent<RectTransform>().localRotation.eulerAngles;
        this.scale = targetObject.GetComponent<RectTransform>().localScale;
        */
    }

    public void LoadCheckPoint()//これポインタ的に使えるのか？
    {
        /*
        this.targetObject.GetComponent<Text>().text = this.text;
        targetObject.GetComponent<Text>().fontSize = this.fontSize;
        targetObject.GetComponent<Text>().color = this.color;
        targetObject.GetComponent<RectTransform>().localPosition = this.position;
        targetObject.GetComponent<RectTransform>().localRotation = Quaternion.Euler(this.rotation);
        targetObject.GetComponent<RectTransform>().localScale = this.scale;
        */
    }
}
