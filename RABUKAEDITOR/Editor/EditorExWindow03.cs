using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using nsRabuka;

public class EditorExWindow03 : EditorWindow
{
	//==========================================================
	//メンバ変数>>>

	//スライダ
	public int timeSlider = 0;
	//最大フレーム番号
	int maxFrame = 1000;

	//SOUND OBJ この仕様も変更しなければ・・・・！
	GameObject soundObject;
	AudioSource audioSource;
	bool inspectorTitlebarForSound = false;

	//OTHER OBJ
	GameObject selectedGameObject;//選択中オブジェクト追加用
	GameObject objInstructorParent;//インストラクタの親 ラブカに下げる
	GameObject tmpObject;//必ず一時的に使ってください!!!!!

	//Scroll
	Vector2 objectsScrollPos = Vector2.zero;//これも増やすのいややんなあ。

	//出現系オブジェクト
	GameObject rabuka;//司令塔、情報はここに保存するか？

	//メンバ変数<<<
	//==========================================================

	[MenuItem("Window/RABUKA EDITOR")]//よく考えたらなんだこれ

	//こっちはスタティック。
	static void Open()
	{
		Debug.Log("Open!");
		EditorWindow.GetWindow<EditorExWindow03>("RABUKA EDITOR");
	}

	//新規ウィンドウが開くときに呼び出されます
	private void Awake()
    {
		Debug.Log("Awake!");
		//初期化（ロード）->OPENに移動したほうがいいかも
		//ラブカを設置する
		if (!GameObject.Find(Macro.rabukaObjName))
		{
			rabuka = new GameObject(Macro.rabukaObjName);//ラブかにフレームナンバープロパティ をもつスクリプトつけよう。Rabuka.cs
			rabuka.AddComponent<Rabuka>();
        }
        else
        {
			rabuka = GameObject.Find(Macro.rabukaObjName);
            if (rabuka.GetComponent<Rabuka>().soundObject != null)
            {
				soundObject = rabuka.GetComponent<Rabuka>().soundObject;
			}
        }
	}

    void Update()//このアップデートが他と同一かって話よな。一応。フレームレート指定しよう。
	{
		//EditorApplication.step();1フレームごと？
		if (EditorApplication.isPlaying)//実行中
		{
			if (audioSource != null && !audioSource.isPlaying)//音再生
            {
				audioSource.time = (float)((float)timeSlider / Config.frameRate);//同期
				audioSource.Play();
            }

			//音楽の同期（一旦保留）
			/*
			if (timeSlider % 10 == 0)
			{
				audioSource.time = (float)(timeSlider / 30.0f);
			}
			*/
			
			timeSlider++;
			//Repaint();//再描画（編集モード）

			if(timeSlider == maxFrame)//最後に来たら終了。
            {
				EditorApplication.ExecuteMenuItem("Edit/Play");//停止（再生）
			}

			//フレーム番号代入（実行中のみなので注意、基本的に参照はtimeSliderで。）
			rabuka.GetComponent<Rabuka>().frame = timeSlider;
		}
		//実行外含有update
	}

	void OnGUI()//不定期で通る 描画コア関数
	{
        EditorGUILayout.BeginVertical(GUI.skin.box);//縦
		{
			//上段
			EditorGUILayout.BeginHorizontal(GUI.skin.box);//横
			{
				//再生ボタン
				if (GUILayout.Button(new GUIContent("再生/停止", "シーンを再生停止します"), GUILayout.Width(200), GUILayout.Height(20)))
				{
					Debug.Log("再生ボタン押下");
					EditorApplication.ExecuteMenuItem("Edit/Play");//ゲーム再生、でそもそもいいの？
				}
				//最大フレーム数入力
				maxFrame = EditorGUILayout.IntField("MaxFrame", maxFrame, GUILayout.Width(200), GUILayout.Height(20));
			}
			EditorGUILayout.EndHorizontal();

			//再生スライダー、再生したら動くようにしたい。
			timeSlider = EditorGUILayout.IntSlider("TIME（FRAME）:", timeSlider, 0, maxFrame);

			//仕切り
			GUI.color = Color.black;
			GUILayout.Box("", GUILayout.Height(5), GUILayout.ExpandWidth(true));
			GUI.color = Color.white;

			//まず音楽再生するところ。
			EditorGUILayout.BeginHorizontal(GUI.skin.box);//横
			{
				soundObject = EditorGUILayout.ObjectField("AUDIO SELECT", soundObject, typeof(GameObject), true) as GameObject;
				if (soundObject != null && soundObject.GetComponent<AudioSource>() != null)
				{
					audioSource = soundObject.GetComponent<AudioSource>();
					maxFrame = (int)(audioSource.clip.length * Config.frameRate);
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

			//その他のオブジェクト（アクター）のところ
			EditorGUILayout.BeginHorizontal(GUI.skin.box);//横
			{
			//選択中のオブジェクトをボタンで入れる
			selectedGameObject = EditorGUILayout.ObjectField("OBJECT SELECT", selectedGameObject, typeof(GameObject), true) as GameObject;
			//オブジェクト追加ボタン
			if (GUILayout.Button(new GUIContent("オブジェクト追加＋", "選択中のオブジェクトをアクターとしてエディタに追加します"), GUILayout.Width(200), GUILayout.Height(30)))
            {
                if (selectedGameObject)//ぬるなら入らない。
				{ 
					//インストラクタ（チェックポイントの親）を設置
					tmpObject = new GameObject("ObjectInstructor:" + (rabuka.transform.childCount).ToString());//変更予定
					tmpObject.AddComponent<ObjectInstructor>();
					tmpObject.GetComponent<ObjectInstructor>().targetObject = selectedGameObject;
					tmpObject.transform.SetParent(rabuka.transform);//ラブ下につけます（この場所は後で変更）
				}
				//elseで警告出す
			}
			}EditorGUILayout.EndHorizontal();

			//通常オブジェクトチェックポイント蘭表示
			objectsScrollPos = EditorGUILayout.BeginScrollView(objectsScrollPos, GUI.skin.box);
			{
				// ObjectInstructor数取得（ヒエラルキドリブン）
				int childCount = rabuka.transform.childCount;//rabukaは変更予定
				for (int i = 0; i < childCount; i++)
                {
					GUI.color = Color.black;//色変え
					EditorGUILayout.BeginVertical(GUI.skin.box);//縦
					{
					GUI.color = Color.white;//色戻し
						//インストラクタ
						GameObject objectInstructor = rabuka.transform.GetChild(i).gameObject;
						//ターゲットオブジェクト
						GameObject targetObject = objectInstructor.GetComponent<ObjectInstructor>().targetObject;
						//----オブジェクトがどんな種類でも共通の処理-----ターゲットが変わるのは流石に不味くないか。
						EditorGUILayout.LabelField("OBJECT NUMBER: " + i.ToString());
						//とりあえず、自動でターゲットオブジェクトの種類を振り分け、その他だったら強制的にデフォルトのCSを追加するって方針でいきます。
						if (targetObject.GetComponent<Text>())//テキスト判定。
						{
							EditorGUILayout.LabelField("TYPE:TEXT   PATH:" + targetObject.GetFullPath());
							TextCheckPointDisplay(objectInstructor);
						}
						else if (targetObject.GetComponent<Camera>())//カメラ判定、後で変更
						{
							if (!targetObject.GetComponent<PostEffect>())//ポストエフェクトの準備（まあ要らないけど）
							{
								//カメラにポストエフェクト追加
								targetObject.AddComponent<PostEffect>();
								//このパスまずいかも・・・。
								Material cameraMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/RABUKAEDITOR/Scripts/Camera/CameraMaterial.mat");
								targetObject.GetComponent<PostEffect>()._material = cameraMaterial;
								targetObject.GetComponent<PostEffect>()._material.shader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/RABUKAEDITOR/Scripts/Camera/posteffect.shader");
							}
							EditorGUILayout.LabelField("TYPE:CAMERA   PATH:" + targetObject.GetFullPath());
							CameraCheckPointDisplay(objectInstructor);
						}
						else
						{
							EditorGUILayout.LabelField("The object type could not be recognized. It is classified into OTHER.");
						}
					}EditorGUILayout.EndVertical();
				}//for
			}EditorGUILayout.EndScrollView();
		}EditorGUILayout.EndVertical();
	}

	//Display系はファイル分けたい。

	void TextCheckPointDisplay(GameObject objectInstructor)//Textオブジェクトだった場合の（そのターゲとオブジェクト固有の）表示
    {
		bool needReturn = false;//関数抜ける用

		//オブジェクトごと上段
		EditorGUILayout.BeginHorizontal(GUI.skin.box);
		{
			if (GUILayout.Button(new GUIContent("削除×", "このオブジェクトを削除します"), GUILayout.Width(300), GUILayout.Height(30)))
			{
				GameObject.DestroyImmediate(objectInstructor);
				needReturn = true;
			}
			if (GUILayout.Button(new GUIContent("並び替え", "チェックポイントをフレーム番号順に並び替えします"), GUILayout.Width(300), GUILayout.Height(30)))
			{
				sortCheckPoint(objectInstructor);
			}
		}
		EditorGUILayout.EndHorizontal();
        if (needReturn) { return; }//オブジェクト削除したならここで表示を終わらせる

		//チェックポイント表示
		EditorGUILayout.BeginHorizontal(GUI.skin.box, GUILayout.Width(500));
		{
			for (int j = 0; j < objectInstructor.transform.childCount; j++)//チェックポイントごと jじゃなくてiで良さそう。
			{
				EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(250));
				{
					tmpObject = objectInstructor.transform.GetChild(j).gameObject;
					//チェックポイントごとの表示
					//EditorGUILayout.LabelField("Text:" + checkPointParent.transform.GetChild(j).gameObject.GetComponent<CheckPointText>().text);
					//表示非表示処理
					tmpObject.GetComponent<CheckPointText>().titlebarFold = EditorGUILayout.InspectorTitlebar(tmpObject.GetComponent<CheckPointText>().titlebarFold, tmpObject);
					tmpObject.GetComponent<CheckPointText>().frameNum = EditorGUILayout.IntField("Frame", tmpObject.GetComponent<CheckPointText>().frameNum);
					if (tmpObject.GetComponent<CheckPointText>().titlebarFold)
					{
						tmpObject.GetComponent<CheckPointText>().text = EditorGUILayout.TextField("Text", tmpObject.GetComponent<CheckPointText>().text);
						tmpObject.GetComponent<CheckPointText>().fontSize = EditorGUILayout.IntField("FontSize", tmpObject.GetComponent<CheckPointText>().fontSize);
						tmpObject.GetComponent<CheckPointText>().color = EditorGUILayout.ColorField("Color", tmpObject.GetComponent<CheckPointText>().color);
						tmpObject.GetComponent<CheckPointText>().position = EditorGUILayout.Vector3Field("Position", tmpObject.GetComponent<CheckPointText>().position);
						tmpObject.GetComponent<CheckPointText>().rotation = EditorGUILayout.Vector3Field("Rotation", tmpObject.GetComponent<CheckPointText>().rotation);
						tmpObject.GetComponent<CheckPointText>().scale = EditorGUILayout.Vector3Field("Scale", tmpObject.GetComponent<CheckPointText>().scale);
					}
					//どのチェックポイントにも共通する処理は関数にでもするか。
					if (GUILayout.Button("チェックポイント削除×", GUILayout.Width(150), GUILayout.Height(30)))
					{
						GameObject.DestroyImmediate(objectInstructor.transform.GetChild(j).gameObject);
					}
				}
				EditorGUILayout.EndVertical();
			}
			//チェックポイント追加
			if (GUILayout.Button("チェックポイント追加＋", GUILayout.Width(200), GUILayout.Height(30)))
			{
				//削除された後、オブジェクト名のインデックスが戻るのでファインドは使わないこと
				tmpObject = new GameObject("CheckPoint(TEXT):" + (objectInstructor.transform.childCount).ToString());
				tmpObject.transform.SetParent(objectInstructor.transform);
				tmpObject.AddComponent<CheckPointText>();
				tmpObject.GetComponent<CheckPointText>().SetCheckPoint(objectInstructor.GetComponent<ObjectInstructor>().targetObject, timeSlider);
			}
		}
		EditorGUILayout.EndHorizontal();
	}

	void CameraCheckPointDisplay(GameObject objectInstructor)//カメラオブジェクトだった場合の（そのターゲとオブジェクト固有の）表示
	{
		bool needReturn = false;

		//チェックポイント追加（関数にするべきだけど優先的ではない）ここ関数出せるんじゃね？
		EditorGUILayout.BeginHorizontal(GUI.skin.box);
		{
			if (GUILayout.Button("このオブジェクトを削除", GUILayout.Width(300), GUILayout.Height(30)))
			{
				GameObject.DestroyImmediate(objectInstructor);
				needReturn = true;
			}
		}
		EditorGUILayout.EndHorizontal();
        if (needReturn) { return; }

		//チェックポイント表示
		EditorGUILayout.BeginHorizontal(GUI.skin.box, GUILayout.Width(500));
		{
			for (int j = 0; j < objectInstructor.transform.childCount; j++)//チェックポイントごと jじゃなくてiで良さそう。
			{
				EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(250));
				{
					tmpObject = objectInstructor.transform.GetChild(j).gameObject;
					//チェックポイントごとの表示
					tmpObject.GetComponent<CheckPointCamera>().frameNum = EditorGUILayout.IntField("Frame", tmpObject.GetComponent<CheckPointCamera>().frameNum);
					tmpObject.GetComponent<CheckPointCamera>().color = EditorGUILayout.ColorField("Color", tmpObject.GetComponent<CheckPointCamera>().color);
					tmpObject.GetComponent<CheckPointCamera>().overlayColor = EditorGUILayout.ColorField("OverLay", tmpObject.GetComponent<CheckPointCamera>().overlayColor);
					tmpObject.GetComponent<CheckPointCamera>().position = EditorGUILayout.Vector3Field("Position", tmpObject.GetComponent<CheckPointCamera>().position);
					tmpObject.GetComponent<CheckPointCamera>().rotation = EditorGUILayout.Vector3Field("Rotation", tmpObject.GetComponent<CheckPointCamera>().rotation);
					//どのチェックポイントにも共通する処理は関数にでもするか。
					if (GUILayout.Button("チェックポイント削除", GUILayout.Width(150), GUILayout.Height(30)))
					{
						GameObject.DestroyImmediate(objectInstructor.transform.GetChild(j).gameObject);
					}
				}
				EditorGUILayout.EndVertical();
			}
			if (GUILayout.Button("チェックポイント追加", GUILayout.Width(200), GUILayout.Height(30)))
			{
				tmpObject = new GameObject("CheckPoint(CAMERA):" + (objectInstructor.transform.childCount).ToString());
				tmpObject.transform.SetParent(objectInstructor.transform);
				tmpObject.AddComponent<CheckPointCamera>();
				tmpObject.GetComponent<CheckPointCamera>().SetCheckPoint(objectInstructor.GetComponent<ObjectInstructor>().targetObject, timeSlider);
			}
		}
		EditorGUILayout.EndHorizontal();
	}

	//これだと、ラブカのオブジェクトリストも入れ替えしないといけない。
	//チェックポイントソート関数(https://ftvoid.com/blog/post/869)
	void sortCheckPoint(GameObject checkpointParent)
	{
		List<Transform> objList = new List<Transform>();

		// 子階層のGameObject数取得
		int childCount = checkpointParent.transform.childCount;

		//用意したリストに子を追加
		for (int i = 0; i < childCount; i++)
		{
			objList.Add(checkpointParent.transform.GetChild(i));
		}

		// オブジェクトを昇順ソート 親クラス名じゃとれないかなあ・・・。とれた！！！感動、新たな発見。
		objList.Sort((obj1, obj2) => obj1.GetComponent<CheckPoint>().frameNum.CompareTo(obj2.GetComponent<CheckPoint>().frameNum));

		// ソート結果順にGameObjectの順序を反映
		foreach (Transform obj in objList)
		{
			obj.SetSiblingIndex(childCount - 1);//最後に入れ続ける
		}
	}

}//class

