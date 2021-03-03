using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CheckPointText : CheckPoint
{
    //public Text text;
    //public RectTransform rectTransform;
    public char[] textChar;
    public string text;

    public void SetCheckPoint(GameObject targetObject, int frame)
    {
        //this.text = targetObject.GetComponent<Text>();//これ参照しちゃうかな？
        //this.rectTransform = targetObject.GetComponent<RectTransform>();
        //textChar = targetObject.GetComponent<Text>().text.ToCharArray();
        text = targetObject.GetComponent<Text>().text;
        Debug.Log(text);
        frameNum = frame;
    }

    public virtual void LoadCheckPoint(GameObject targetObject)//これポインタ的に使えるのか？
    {
        targetObject.GetComponent<Text>().text = this.text;
    }
}
