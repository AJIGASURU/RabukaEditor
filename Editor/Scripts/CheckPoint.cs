using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckPoint : MonoBehaviour
{
    public int frameNum;//変化が起こるフレームの位置番号
    //GameObject targetObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCheckPoint(GameObject targetObject, int funcNum, int frame)
    {
        if(targetObject.GetComponent<Text>())//TEXT
        {

        }
        else
        {
            Debug.Log("The ObjectType is specified incorrectly. Failed to add checkpoint.");
        }
    }

    public void RunCheckPoint()
    {

    }
}
