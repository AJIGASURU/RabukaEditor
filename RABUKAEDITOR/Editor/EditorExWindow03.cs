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
	Vector2 objectsScrollPos = Vector2.zero;//これも増やすのいややんなあ。

	//出現系オブジェクト
	GameObject rabuka;//司令塔、情報はここに保存するか？

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

			if (timeSlider % 30 == 0)//ある程度の周期ごとに行う演算？
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
			if (GUILayout.Button("選択中のオブジェクトを追加", GUILayout.Width(200), GUILayout.Height(30)))
            {
                if (selectedGameObject)//ぬるなら入らない。
				{ 
					rabuka.GetComponent<Rabuka>().objectList.Add(selectedGameObject);
					tmpObject = new GameObject("TargetObject:" + (rabuka.GetComponent<Rabuka>().objectList.Count - 1).ToString());
					tmpObject.transform.SetParent(rabuka.transform);//ラブ下につけます。
				}
			}

			objectsScrollPos = EditorGUILayout.BeginScrollView(objectsScrollPos, GUI.skin.box);
			{
				for(int i = 0; i < rabuka.GetComponent<Rabuka>().objectList.Count; i++)//インデックスつける？
                {
					EditorGUILayout.BeginVertical(GUI.skin.box);//縦
					{
						if (rabuka.GetComponent<Rabuka>().objectList[i] != null)//多分必要ないがエラー対策
						{
							//----オブジェクトがどんな種類でも共通の処理-----ターゲットが変わるのは流石に不味くないか。
							EditorGUILayout.LabelField("OBJECT NUMBER: " + i.ToString());
							//じゃあとりあえず、自動でオブジェクトの種類を振り分け、その他だったら強制的にデフォルトのCSを追加するって方針でいきます。
							if (rabuka.GetComponent<Rabuka>().objectList[i].GetComponent<Text>())//テキストオブジェクトと判定。
							{
								EditorGUILayout.LabelField("TYPE TEXT: " + rabuka.GetComponent<Rabuka>().objectList[i].GetFullPath());
								TextCheckPointDisplay(rabuka.GetComponent<Rabuka>().objectList[i], i);
							}
							else if (rabuka.GetComponent<Rabuka>().objectList[i].GetComponent<Camera>())
                            {
                                if (!rabuka.GetComponent<Rabuka>().objectList[i].GetComponent<PostEffect>())//ポストエフェクトの準備（まあ要らないけど）
                                {
									rabuka.GetComponent<Rabuka>().objectList[i].AddComponent<PostEffect>();
									Material cameraMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/RABUKAEDITOR/Scripts/Camera/CameraMaterial.mat");
									rabuka.GetComponent<Rabuka>().objectList[i].GetComponent<PostEffect>()._material = cameraMaterial;
									rabuka.GetComponent<Rabuka>().objectList[i].GetComponent<PostEffect>()._material.shader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/RabukaEditor/Scripts/Camera/posteffect.shader");
								}
								EditorGUILayout.LabelField("TYPE CAMERA: " + rabuka.GetComponent<Rabuka>().objectList[i].GetFullPath());
								CameraCheckPointDisplay(rabuka.GetComponent<Rabuka>().objectList[i], i);
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
			EditorGUILayout.EndScrollView();
		}
		EditorGUILayout.EndVertical();
	}

	void TextCheckPointDisplay(GameObject targetObject, int objectIndex)//Textオブジェクトだった場合の（そのターゲとオブジェクト固有の）表示
    {
		GameObject checkPointParent = rabuka.transform.GetChild(objectIndex).gameObject;//objectIndex->i
		if(checkPointParent == null)
        {
			Debug.Log("エラー:TextCheckPointDisplay()");
        }
		//チェックポイント追加（関数にするべきだけど優先的ではない）
		EditorGUILayout.BeginHorizontal(GUI.skin.box);
		{
			if (GUILayout.Button("今の条件でチェックポイントを追加", GUILayout.Width(300), GUILayout.Height(30)))
			{
				//削除された後、オブジェクト名のインデックスが戻るのでファインドは使わないこと
				tmpObject = new GameObject("TextCheckPoint:" + (checkPointParent.transform.childCount).ToString());
				tmpObject.transform.SetParent(checkPointParent.transform);
				tmpObject.AddComponent<CheckPointText>();
				tmpObject.GetComponent<CheckPointText>().SetCheckPoint(targetObject, timeSlider);
			}
			if (GUILayout.Button("このオブジェクトを削除", GUILayout.Width(300), GUILayout.Height(30)))
			{
				rabuka.GetComponent<Rabuka>().objectList.RemoveAt(objectIndex);
				GameObject.DestroyImmediate(checkPointParent);
				return;//ゴリ押しジャン。
			}
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal(GUI.skin.box, GUILayout.Width(500));
		{
			for (int j = 0; j < checkPointParent.transform.childCount; j++)//チェックポイントごと jじゃなくてiで良さそう。
			{
				EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(250));
				{
					tmpObject = checkPointParent.transform.GetChild(j).gameObject;
					//チェックポイントごとの表示
					//EditorGUILayout.LabelField("Text:" + checkPointParent.transform.GetChild(j).gameObject.GetComponent<CheckPointText>().text);
					//EditorGUILayout.LabelField("Frame:" + checkPointParent.transform.GetChild(j).gameObject.GetComponent<CheckPointText>().frameNum.ToString());
					tmpObject.GetComponent<CheckPointText>().text = EditorGUILayout.TextField("Text", tmpObject.GetComponent<CheckPointText>().text);
					tmpObject.GetComponent<CheckPointText>().frameNum = EditorGUILayout.IntField("Frame", tmpObject.GetComponent<CheckPointText>().frameNum);
					tmpObject.GetComponent<CheckPointText>().fontSize = EditorGUILayout.IntField("FontSize", tmpObject.GetComponent<CheckPointText>().fontSize);
					tmpObject.GetComponent<CheckPointText>().color = EditorGUILayout.ColorField("Color", tmpObject.GetComponent<CheckPointText>().color);
					tmpObject.GetComponent<CheckPointText>().position = EditorGUILayout.Vector3Field("Position", tmpObject.GetComponent<CheckPointText>().position);
					tmpObject.GetComponent<CheckPointText>().rotation = EditorGUILayout.Vector3Field("Rotation", tmpObject.GetComponent<CheckPointText>().rotation);
					tmpObject.GetComponent<CheckPointText>().scale = EditorGUILayout.Vector3Field("Scale", tmpObject.GetComponent<CheckPointText>().scale);
					//どのチェックポイントにも共通する処理は関数にでもするか。
					if (GUILayout.Button("チェックポイント削除", GUILayout.Width(150), GUILayout.Height(30)))
					{
						GameObject.DestroyImmediate(checkPointParent.transform.GetChild(j).gameObject);
					}
				}
				EditorGUILayout.EndVertical();
			}
		}
		EditorGUILayout.EndHorizontal();
	}

	void CameraCheckPointDisplay(GameObject targetObject, int objectIndex)//Textオブジェクトだった場合の（そのターゲとオブジェクト固有の）表示
	{
		GameObject checkPointParent = rabuka.transform.GetChild(objectIndex).gameObject;//objectIndex->i
		if (checkPointParent == null)
		{
			Debug.Log("エラー:CameraCheckPointDisplay()");
		}
		//チェックポイント追加（関数にするべきだけど優先的ではない）
		EditorGUILayout.BeginHorizontal(GUI.skin.box);
		{
			if (GUILayout.Button("今の条件でチェックポイントを追加", GUILayout.Width(300), GUILayout.Height(30)))
			{
				tmpObject = new GameObject("CameraCheckPoint:" + (checkPointParent.transform.childCount).ToString());
				tmpObject.transform.SetParent(checkPointParent.transform);
				tmpObject.AddComponent<CheckPointCamera>();
				tmpObject.GetComponent<CheckPointCamera>().SetCheckPoint(targetObject, timeSlider);
			}
			if (GUILayout.Button("このオブジェクトを削除", GUILayout.Width(300), GUILayout.Height(30)))
			{
				rabuka.GetComponent<Rabuka>().objectList.RemoveAt(objectIndex);
				GameObject.DestroyImmediate(checkPointParent);
				return;//ゴリ押しジャン。
			}
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal(GUI.skin.box, GUILayout.Width(500));
		{
			for (int j = 0; j < checkPointParent.transform.childCount; j++)//チェックポイントごと jじゃなくてiで良さそう。
			{
				EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(250));
				{
					tmpObject = checkPointParent.transform.GetChild(j).gameObject;
					//チェックポイントごとの表示
					tmpObject.GetComponent<CheckPointCamera>().frameNum = EditorGUILayout.IntField("Frame", tmpObject.GetComponent<CheckPointCamera>().frameNum);
					tmpObject.GetComponent<CheckPointCamera>().color = EditorGUILayout.ColorField("Color", tmpObject.GetComponent<CheckPointCamera>().color);
					tmpObject.GetComponent<CheckPointCamera>().overlayColor = EditorGUILayout.ColorField("OverLay", tmpObject.GetComponent<CheckPointCamera>().overlayColor);
					tmpObject.GetComponent<CheckPointCamera>().position = EditorGUILayout.Vector3Field("Position", tmpObject.GetComponent<CheckPointCamera>().position);
					tmpObject.GetComponent<CheckPointCamera>().rotation = EditorGUILayout.Vector3Field("Rotation", tmpObject.GetComponent<CheckPointCamera>().rotation);
					//どのチェックポイントにも共通する処理は関数にでもするか。
					if (GUILayout.Button("チェックポイント削除", GUILayout.Width(150), GUILayout.Height(30)))
					{
						GameObject.DestroyImmediate(checkPointParent.transform.GetChild(j).gameObject);
					}
				}
				EditorGUILayout.EndVertical();
			}
		}
		EditorGUILayout.EndHorizontal();
	}

}
