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
	const int maxObjectNum = 10;
	const int maxCheckPointNum = 30;

    GameObject[] gameObjects = new GameObject[maxObjectNum];
	bool[] inspectorTitlebars = new bool[maxObjectNum];
	//CheckPointText[,] checkPointTexts = new CheckPointText[maxObjectNum,maxCheckPointNum];

	//Scroll
	Vector2 objectsScrollPos = Vector2.zero;

	//CheckPoint
	//List<CheckPointText> checkPointTextList = new List<CheckPointText>();//これをオブジェクトごとに分けるかどうかという話。
	GameObject[] checkPointObject = new GameObject[maxCheckPointNum];

	//出現系オブジェクト
	GameObject rabuka;//Parent

	[MenuItem("Window/RABUKA EDITOR")]//よく考えたらなんだこれ

	static void Open()
	{
		Debug.Log("Open!");
		EditorWindow.GetWindow<EditorExWindow03>("RABUKA EDITOR");
	}

    private void Awake()
    {
		Debug.Log("Awake!");
		//初期化
		if (!GameObject.Find("Rabuka"))
		{
			rabuka = new GameObject("Rabuka");//ラブかにフレームナンバープロパティ をもつスクリプトつけよう。Rabuka.cs
			rabuka.AddComponent<Rabuka>();
		}
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
				//timeSlider = (int)audioSource.time * 30;
			}

			timeSlider++;
			Repaint();//再描画

			if(timeSlider == maxFrame)//最後に来たら終了。
            {
				EditorApplication.ExecuteMenuItem("Edit/Play");//停止（再生）
			}

			//フレーム番号代入
			rabuka.GetComponent<Rabuka>().frame = timeSlider;
		}
		//実行外含有update
	}

	void OnGUI()
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
					inspectorTitlebarForSound = EditorGUILayout.InspectorTitlebar(inspectorTitlebarForSound, soundObject);
					if (inspectorTitlebarForSound)
					{
						EditorGUILayout.LabelField("オーディオクリップの長さは" + audioSource.clip.length.ToString() + "秒です。");
						maxFrame = (int)(audioSource.clip.length * 30.0f);
					}
				}
				else
				{
					EditorGUILayout.LabelField("オーディオソースコンポーネントを所持したオブジェクトを入れてください。");
				}
			}EditorGUILayout.EndHorizontal();

			//その他のオブジェクト？
			objectsScrollPos = EditorGUILayout.BeginScrollView(objectsScrollPos, GUI.skin.box);
			{
				for(int i=0; i<maxObjectNum; i++)
                {
					EditorGUILayout.BeginVertical(GUI.skin.box);//縦
					{
						gameObjects[i] = EditorGUILayout.ObjectField("GAME OBJECT " + i.ToString(), gameObjects[i], typeof(GameObject), true) as GameObject;
						if (gameObjects[i] != null)
						{
							inspectorTitlebars[i] = EditorGUILayout.InspectorTitlebar(inspectorTitlebars[i], gameObjects[i]);
							if (inspectorTitlebars[i])
							{
								//じゃあとりあえず、自動でオブジェクトの種類を振り分け、その他だったら強制的にデフォルトのCSを追加するって方針でいきます。
								if (gameObjects[i].GetComponent<Text>())
								{
									TextObjectButton(i);
								}
								else
								{
									EditorGUILayout.LabelField("The object type could not be recognized. It is classified into OTHER.");
								}
                            }
						}
						EditorGUILayout.EndVertical();
					}
				}
			}
			EditorGUILayout.EndScrollView();
		}
		EditorGUILayout.EndVertical();
	}

	void TextObjectButton(int i)//Textオブジェクトだった場合の表示
    {
		EditorGUILayout.LabelField("Object Type is TEXT.");
		EditorGUILayout.BeginHorizontal(GUI.skin.box);
		{
			/*
			if (GUILayout.Button("ポイント追加", GUILayout.Width(100), GUILayout.Height(20)))
			{
                //checkPointTextList.Add(new CheckPointText(gameObjects[i], timeSlider));//よくわからないけど動的ダメっぽい。

			}
			EditorGUILayout.BeginVertical(GUI.skin.box);
            {
				foreach (CheckPointText c in checkPointTexts)
                {
					EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    {
						EditorGUILayout.LabelField(c.text.text);
					}
					EditorGUILayout.EndHorizontal();
				}
            }
			EditorGUILayout.EndVertical();
			*/

			for (int j = 0; j < maxCheckPointNum; j++)//これが参照の肝
			{
				EditorGUILayout.BeginVertical(GUI.skin.box);
				{
					if (GUILayout.Button("追加", GUILayout.Width(80), GUILayout.Height(20)))
					{
						//ゲームオブジェクト自体に付けちゃうという横暴
						//次->チェックポイントをオブジェクト型にして、オブジェクトフィールド使おう。その前に整理
						checkPointObject[j] = new GameObject("TextCheckPoint " + i.ToString() + " " + j.ToString());
						checkPointObject[j].transform.SetParent(gameObjects[i].transform);//親指定
						checkPointObject[j].AddComponent<CheckPointText>();
						checkPointObject[j].GetComponent<CheckPointText>().SetCheckPoint(gameObjects[i], timeSlider);
					}
					if (checkPointObject[j])//最初nullなのかな。削除時どうしよう・・・null代入とか？
					{
						EditorGUILayout.LabelField("Text:" + checkPointObject[j].GetComponent<CheckPointText>().text);
						EditorGUILayout.LabelField("Frame:" + checkPointObject[j].GetComponent<CheckPointText>().frameNum.ToString());
					}
				}
				EditorGUILayout.EndVertical();
			}
			//表示（テキストに限らないので、ここじゃなくていいかも）、Findするならチェックポイントオブジェクトの保持必要なくね？
			/*
			if (checkPointParent = GameObject.Find("CheckPoints"))
			{
				for(int k=0; k<checkPointParent.transform.childCount; k++)
				{
					ChildObject[i] = ParentObject.transform.GetChild(i).gameObject;
				}
				//EditorGUILayout.LabelField("Text:" + checkPointTexts[i, j].text.text);
				//EditorGUILayout.LabelField("Frame:" + checkPointTexts[i, j].frameNum.ToString());
			}
			*/
		}
		EditorGUILayout.EndHorizontal();
	}

}
