using UnityEngine;
using UnityEditor;
using System.Collections;

public class EditorExWindow02 : EditorWindow
{
	//ストリーム型のオブジェクトでも作るか。AVIUTLでいうレイヤー、エディタ上でのオブジェクトは動的な生成ができなさそう。
	[MenuItem("Window/EditorEx02")]
	static void Open()
	{
		EditorWindow.GetWindow<EditorExWindow02>("EditorEx02");
	}

	bool toggle;
	bool toggleLeft;
	bool foldout;
	string textField = "";
	string textArea = "";
	string password = "";
	int intField = 0;
	int intSlider = 0;
	float floatField = 0.0f;
	float slider = 0.0f;
	float minMaxSliderMinValue = 20.0f;
	float minMaxSliderMaxValue = 50.0f;
	int popup = 0;
	int intPopup = 0;

	public enum EnumPopup
	{
		Enum1,
		Enum2,
		Enum3
	}
	EnumPopup enumPopup = EnumPopup.Enum1;
	int maskField = 0;
	EnumPopup enumMaskField = 0;
	int layer = 0;
	string tag = "";
	Vector2 vector2Field = Vector2.zero;
	Vector3 vector3Field = Vector3.zero;
	Vector4 vector4Field = Vector4.zero;
	Rect rectField;
	Color colorField = Color.white;
	Bounds boundsField;
	AnimationCurve curveField = AnimationCurve.Linear(0.0f, 0.0f, 60.0f, 1.0f);
	Object[] objectFieldArray = new Object[20];
	bool inspectorTitlebar = false;

	//SECTION5
	Vector2 buttonSize = new Vector2(100, 20);
	Vector2 buttonMinSize = new Vector2(100, 20);
	Vector2 buttonMaxSize = new Vector2(1000, 200);
	bool expandWidth = true;
	bool expandHeight = true;

	//SECTION6
	int rightSize = 10;
	Vector2 rightScrollPos = Vector2.zero;

	//7
	//GUISkin myGUISkin;
	

	void OnGUI()
	{
		//SECTION5---------------
		// 直接サイズを指定する場合は、
		// GUILayout.Width/Heightを使う。
		buttonSize = EditorGUILayout.Vector2Field("ButtonSize", buttonSize);
		if (GUILayout.Button("サイズ指定ボタン", GUILayout.Width(buttonSize.x), GUILayout.Height(buttonSize.y)))
		{
			Debug.Log("サイズ指定ボタン");
		}

		// 自動的にサイズ変更される範囲を指定する場合は
		// GUILayout.MinWidth/MaxWidth/MinHeight/MaxHeightを使う。
		buttonMinSize = EditorGUILayout.Vector2Field("ButtonMinSize", buttonMinSize);
		buttonMaxSize = EditorGUILayout.Vector2Field("ButtonMaxSize", buttonMaxSize);
		if (GUILayout.Button("最小最大指定ボタン",
							  GUILayout.MinWidth(buttonMinSize.x), GUILayout.MinHeight(buttonMinSize.y),
							  GUILayout.MaxWidth(buttonMaxSize.x), GUILayout.MaxHeight(buttonMaxSize.y)))
		{
			Debug.Log("最小最大指定ボタン");
		}

		// 有効範囲内全体に広げるかどうかは
		// GUILayout.ExpandWidth/ExpandHeightで指定する。
		expandWidth = EditorGUILayout.Toggle("ExpandWidth", expandWidth);
		expandHeight = EditorGUILayout.Toggle("ExpandHeight", expandHeight);
		if (GUILayout.Button("Expandボタン", GUILayout.ExpandWidth(expandWidth), GUILayout.ExpandHeight(expandHeight)))
		{
			Debug.Log("Expandボタン");
		}

		//end-SECTION5-----------------------

		//section6
		//動的できんかな
		rightSize = EditorGUILayout.IntSlider("Size", rightSize, 1, 20, GUILayout.ExpandWidth(false));

		GUI.skin = Resources.Load<GUISkin>("Assets/Editor/MyGUISkin");
		rightScrollPos = EditorGUILayout.BeginScrollView(rightScrollPos, GUI.skin.box);
		{
			// スクロール範囲
			for (int y = 0; y < rightSize; y++)
			{
				EditorGUILayout.BeginHorizontal(GUI.skin.box);
				{
					/*
					// ここの範囲は横並び
					EditorGUILayout.PrefixLabel("Index " + y);

					// 下に行くほどボタン数増やす
					for (int i = 0; i < y + 1; i++)
					{
						// ボタン(横幅100px)
						if (GUILayout.Button("Button" + i, GUILayout.Width(100)))
						{
							Debug.Log("Button" + i + "押したよ");
						}
					}
					*/
					objectFieldArray[y] = EditorGUILayout.ObjectField("LAYER " + y, objectFieldArray[y], typeof(Object), true);

					if (objectFieldArray[y] != null)
					{
						inspectorTitlebar = EditorGUILayout.InspectorTitlebar(inspectorTitlebar, objectFieldArray[y]);
						if (inspectorTitlebar)
						{
							EditorGUILayout.LabelField("ﾁﾗｯﾁﾗｯ");
						}
					}
				}
				EditorGUILayout.EndHorizontal();
			}
			// こんな感じで横幅固定しなくても、範囲からはみ出すときにスクロールバー出してくれる。
		}
		EditorGUILayout.EndScrollView();

		//




		EditorGUILayout.LabelField("ようこそ！　Unityエディタ拡張の沼へ！"); // やっぱり残しておこう。

		EditorGUILayout.LabelField("LabelField", "EditorGUILayoutはEditor拡張用に調整されてる系");

		EditorGUILayout.SelectableLabel("SelectableLabel : 選択してコピペできる。変更はできない");

        EditorGUILayout.BeginHorizontal(GUI.skin.box);
		{
			EditorGUILayout.BeginVertical(GUI.skin.box);
			{

				toggle = EditorGUILayout.Toggle("Toggle", toggle);

				toggleLeft = EditorGUILayout.ToggleLeft("ToggleLeft", toggleLeft);

				foldout = EditorGUILayout.Foldout(foldout, "Foldout");
				if (foldout)
				{
					EditorGUILayout.LabelField("ﾁﾗｯ");
				}
			}
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical(GUI.skin.box);
			{
				textField = EditorGUILayout.TextField("TextField", textField);

				textArea = EditorGUILayout.TextArea(textArea);
			}
			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndHorizontal();

		intField = EditorGUILayout.IntField("IntField", intField);

		intSlider = EditorGUILayout.IntSlider("IntSlider", intSlider, 0, 100);

		floatField = EditorGUILayout.FloatField("FloatField", floatField);

		slider = EditorGUILayout.Slider("Slider", slider, 0.0f, 100.0f);

		EditorGUILayout.MinMaxSlider(new GUIContent("MinMaxSlider"), ref minMaxSliderMinValue, ref minMaxSliderMaxValue, 0.0f, 100.0f);
		EditorGUILayout.LabelField("MinValue = ", minMaxSliderMinValue.ToString());
		EditorGUILayout.LabelField("MaxValue = ", minMaxSliderMaxValue.ToString());

		layer = EditorGUILayout.LayerField("LayerField", layer);

		tag = EditorGUILayout.TagField("TagField", tag);

		vector3Field = EditorGUILayout.Vector3Field("Vector3Field", vector3Field);

		rectField = EditorGUILayout.RectField("RectField", rectField);

		colorField = EditorGUILayout.ColorField("ColorField", colorField);

		boundsField = EditorGUILayout.BoundsField("BoundsField", boundsField);//center,extends,3D範囲、Boundsは構造体らしい。

		curveField = EditorGUILayout.CurveField("CurveField", curveField);

		/*
		objectField = EditorGUILayout.ObjectField("ObjectField", objectField, typeof(Object), true);

		if (objectField != null)
		{
			inspectorTitlebar = EditorGUILayout.InspectorTitlebar(inspectorTitlebar, objectField);
			if (inspectorTitlebar)
			{
				EditorGUILayout.LabelField("ﾁﾗｯﾁﾗｯ");
			}
		}
		*/

		EditorGUILayout.LabelField("ここからSpace");
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("ここまでSpace");

		EditorGUILayout.HelpBox("Heeeeeelllllp!!!!!", MessageType.Warning);

	}
}
