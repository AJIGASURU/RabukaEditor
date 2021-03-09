using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CheckPointCamera : CheckPoint
{

    public Color color;
    public Color overlayColor;
    public Vector3 position;
    public Vector3 rotation;

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
        this.color = targetObject.GetComponent<PostEffect>()._material.GetColor("_Color");
        this.overlayColor = targetObject.GetComponent<PostEffect>()._material.GetColor("_Color2");
        this.position = targetObject.transform.position;
        this.rotation = targetObject.transform.rotation.eulerAngles;
    }

    public void LoadCheckPoint()//これポインタ的に使えるのか？
    {
        targetObject.GetComponent<PostEffect>()._material.SetColor("_Color", this.color);
        targetObject.GetComponent<PostEffect>()._material.SetColor("_Color2", this.overlayColor);
        targetObject.transform.SetPositionAndRotation(this.position, Quaternion.Euler(this.rotation));
    }
}
