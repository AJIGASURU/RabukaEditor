using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EditorExWindow03 : EditorWindow
{
	public int timeSlider = 0;
	int maxFrame = 1000;

	//SOUND OBJ
	GameObject soundObject;
	AudioSource audioSource;
	bool inspectorTitlebarForSound = false;

	//Other OBJ

	GameObject selectedGameObject;//洗濯中オブジェクト追加用
	GameObject tmpObject;//必ず一時的に使ってください!!!!!

	//Scroll
	Vector2 objectsScrollPos = Vector2.zero;

	//CheckPoint
	//2次元リストバージョン（チェックポイントだけ持って、ターゲットオブジェクト自体は最悪持たない手もある。）
	List<List<GameObject>> checkPointList = new List<List<GameObject>>();

	//出現系オブジェクト
	GameObject rabuka;//司令塔、情報はここに保存するか？

    //for用
    int i = 0, j = 0, k = 0;

	[MenuItem("Window/RABUKA EDITOR")]//よく考えたらなんだこれ

	static void Open()
	{
		Debug.Log("Open!");
		EditorWindow.GetWindow<EditorExWindow03>("RABUKA EDITOR");
	}

    private void Awake()
    {
		Debug.Log("Awake!");
		//初期化（ロード）
		//ラブカ
		if (!GameObject.Find("Rabuka"))
		{
			rabuka = new GameObject("Rabuka");//ラブかにフレームナンバープロパティ をもつスクリプトつけよう。Rabuka.cs
			rabuka.AddComponent<Rabuka>();
        }
        else
        {
			rabuka = GameObject.Find("Rabuka");
            if (rabuka.GetComponent<Rabuka>().soundObject != null)
            {
				soundObject = rabuka.GetComponent<Rabuka>().soundObject;
			}
        }
		//チェックポイントのロード
		LoadCheckPoints();
	}

    void Update()//このアップデートが他と同一かって話よな。一応。フレームレート指定しよう。
	{
		//EditorApplication.step();1フレームごと？
		if (EditorApplication.isPlaying)//実行中UPDATE
		{
			if (audioSource != null && !audioSource.isPlaying)//おと
            {
				audioSource.time = (float)(timeSlider / 30.0f);//同期
				audioSource.Play();
            }

			if (timeSlider % 50 == 0)//ある程度の周期ごとに行う演算？
			{
				audioSource.time = (float)(timeSlider / 30.0f);//音楽の方じゃなくて描画側を同期するべきでは->スライダの値自体を全てのオブジェクトで同期しないと難しい。
			}

			timeSlider++;
			Repaint();//再描画

			if(timeSlider == maxFrame)//最後に来たら終了。
            {
				EditorApplication.ExecuteMenuItem("Edit/Play");//停止（再生）
			}

			//フレーム番号代入（実行中のみなので注意、基本的に参照はtimeSliderで。）
			rabuka.GetComponent<Rabuka>().frame = timeSlider;
		}
		//実行外含有update
	}

	void OnGUI()//不定期で通る
	{
        EditorGUILayout.BeginVertical(GUI.skin.box);//縦
		{
			if (GUILayout.Button("再生/ポーズ", GUILayout.Width(200), GUILayout.Height(20)))
			{
				Debug.Log("再生ボタン");

				EditorApplication.ExecuteMenuItem("Edit/Play");//ゲーム再生
			}

			timeSlider = EditorGUILayout.IntSlider("TIME（FRAME）:", timeSlider, 0, maxFrame);

			//まず音楽再生するところ。
			EditorGUILayout.BeginHorizontal(GUI.skin.box);//横
			{
				soundObject = EditorGUILayout.ObjectField("Audio", soundObject, typeof(GameObject), true) as GameObject;
				if (soundObject != null && soundObject.GetComponent<AudioSource>() != null)
				{
					audioSource = soundObject.GetComponent<AudioSource>();
					maxFrame = (int)(audioSource.clip.length * 30.0f);
					inspectorTitlebarForSound = EditorGUILayout.InspectorTitlebar(inspectorTitlebarForSound, soundObject);
					if (inspectorTitlebarForSound)
					{
						EditorGUILayout.LabelField("オーディオクリップの長さは" + audioSource.clip.length.ToString() + "秒です。");
					}
					//ラブカのサウンドオブジェクトを更新
					rabuka.GetComponent<Rabuka>().soundObject = soundObject;
				}
				else
				{
					EditorGUILayout.LabelField("オーディオソースコンポーネントを所持したオブジェクトを入れてください。");
				}
			}EditorGUILayout.EndHorizontal();

			//その他のオブジェクトのところ
			//選択中のオブジェクトをボタンで入れる
			selectedGameObject = EditorGUILayout.ObjectField("SELECTED OBJECT ", selectedGameObject, typeof(GameObject), true) as GameObject;
			//オブジェクト追加ボタン
			if (GUILayout.Button("選択中のオブジェクトを追加", GUILayout.Width(150), GUILayout.Height(30)))
            {
                if (selectedGameObject)//ぬるなら入らない。
				{ 
					rabuka.GetComponent<Rabuka>().objectList.Add(selectedGameObject);
					checkPointList.Add(new List<GameObject>());
					tmpObject = new GameObject("TargetObject:" + (rabuka.GetComponent<Rabuka>().objectList.Count - 1).ToString());
					tmpObject.transform.SetParent(rabuka.transform);//ラブ下につけます。
				}
			}

			objectsScrollPos = EditorGUILayout.BeginScrollView(objectsScrollPos, GUI.skin.box);
			{
				int index = 0;
				foreach(GameObject g in rabuka.GetComponent<Rabuka>().objectList)//インデックスつける？
                {
					EditorGUILayout.BeginVertical(GUI.skin.box);//縦
					{
						if (g != null)//多分必要ないがエラー対策
						{
							//----オブジェクトがどんな種類でも共通の処理-----
							EditorGUILayout.LabelField("OBJECT NUMBER: " + index.ToString());
							//じゃあとりあえず、自動でオブジェクトの種類を振り分け、その他だったら強制的にデフォルトのCSを追加するって方針でいきます。
							if (g.GetComponent<Text>())//テキストオブジェクトと判定。
							{
								//EditorGUILayout.LabelField("Name: " + g.name);
								EditorGUILayout.LabelField("TYPE TEXT: " + g.GetFullPath());
								TextCheckPointDisplay(g, index);
							}
							else
							{
								EditorGUILayout.LabelField("The object type could not be recognized. It is classified into OTHER.");
							}

						}
					}
					EditorGUILayout.EndVertical();
					index++;
				}
			}
			EditorGUILayout.EndScrollView();
		}
		EditorGUILayout.EndVertical();
	}

	void TextCheckPointDisplay(GameObject targetObject, int objectIndex)//Textオブジェクトだった場合の（そのターゲとオブジェクト固有の）表示
    {
		GameObject checkPointParent = rabuka.transform.Find("TargetObject:" + objectIndex.ToString()).gameObject;
		if(checkPointParent == null)
        {
			Debug.Log("エラー:TextCheckPointDisplay()");
        }
		//チェックポイント追加
		if (GUILayout.Button("今の条件でチェックポイントを追加", GUILayout.Width(300), GUILayout.Height(30)))
		{
			tmpObject = new GameObject("TextCheckPoint:" + checkPointList[objectIndex].Count.ToString());//これして親消えないかな・・・？わからん。
			tmpObject.transform.SetParent(checkPointParent.transform);
			tmpObject.AddComponent<CheckPointText>();
			tmpObject.GetComponent<CheckPointText>().SetCheckPoint(targetObject, timeSlider);
			checkPointList[objectIndex].Add(tmpObject);
		}

		EditorGUILayout.BeginHorizontal(GUI.skin.box);
		{
			foreach (GameObject checkPoint in checkPointList[objectIndex])//これが参照の肝
			{
				EditorGUILayout.BeginVertical(GUI.skin.box);
				{
					EditorGUILayout.LabelField("Text:" + checkPoint.GetComponent<CheckPointText>().text);
					EditorGUILayout.LabelField("Frame:" + checkPoint.GetComponent<CheckPointText>().frameNum.ToString());
				}
				EditorGUILayout.EndVertical();
			}
		}
		EditorGUILayout.EndHorizontal();
	}

	void LoadCheckPoints()
    {
		for (i = 0; i < rabuka.transform.childCount; i++)
		{
			tmpObject = rabuka.transform.GetChild(i).gameObject;
			checkPointList.Add(new List<GameObject>());
			for (j = 0; j < tmpObject.transform.childCount; j++)
				checkPointList[i].Add(tmpObject.transform.GetChild(j).gameObject);
		}
	}

}
