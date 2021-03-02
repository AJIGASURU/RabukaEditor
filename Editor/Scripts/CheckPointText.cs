using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckPointText : CheckPoint
{
    public Text text;
    public RectTransform rectTransform;

    public CheckPointText(GameObject targetObject, int frame)//コンストラクタ
    {
        this.text = targetObject.GetComponent<Text>();//これ参照しちゃうかな？
        this.rectTransform = targetObject.GetComponent<RectTransform>();
    }

    public virtual void LoadCheckPoint(GameObject targetObject)//これポインタ的に使えるのか？
    {
        targetObject.GetComponent<Text>().text = this.text.text;
    }
}
