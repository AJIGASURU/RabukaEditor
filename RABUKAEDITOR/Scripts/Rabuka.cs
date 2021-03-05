using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rabuka : MonoBehaviour
{
    public int frame;
    public List<GameObject> gameObjects = new List<GameObject>();//ラブカの子オブジェクトが参照してるターゲットオブジェクトをとる。
    //チェックポイントロード->ターゲットのロード、、だとチェックポイントを入れてないゲームオブジェクトが保存されない。->入れたときに初期のチェックポイントを自動で入れるなど。

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init()
    {
        //ロード
    }
}
