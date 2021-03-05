using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GameObjectElement
{
    public GameObject gameObject;
    public bool inspectorTitlebar;

    public GameObjectElement(GameObject gameObject, bool inspectorTitlebar)
    {
        this.gameObject = gameObject;
        this.inspectorTitlebar = inspectorTitlebar;
    }
}

public class Rabuka : MonoBehaviour
{
    public int frame;
    //public List<GameObject> objects = new List<GameObject>();//ラブカの子オブジェクトが参照してるターゲットオブジェクトをとる。
    //チェックポイントロード->ターゲットのロード、、だとチェックポイントを入れてないゲームオブジェクトが保存されない。->入れたときに初期のチェックポイントを自動で入れるなど。

    //public List<GameObjectElement> objectElementList = new List<GameObjectElement>();//ターゲットオブジェクトのリスト
    public List<GameObject> objectList = new List<GameObject>();

    //リスト保存実験
    //public List<int> exp = new List<int>();

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

    //ターゲットオブジェクトリスト
    /*
    public void AddObjectElement()//リストを長くするだけ。
    {
        objectElementList.Add(new GameObjectElement(gameObject ,false));//gameObject->自分自身じゃなかったっけ。
    }

    public void SetObjectElement(GameObject gameObject, int i)
    {
        objectElementList.Insert(i, new GameObjectElement(gameObject, false));
    }

    public int ObjectElementCount()
    {
        return objectElementList.Count;
    }
    */

}
