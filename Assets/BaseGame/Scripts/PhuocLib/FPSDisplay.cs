using UnityEngine;
using System.Collections;

public class FPSDisplay : MonoBehaviour
{
	float m_DeltaTime = 0.0f;

	private void Update()
	{
		m_DeltaTime += (Time.unscaledDeltaTime - m_DeltaTime) * 0.1f;
	}

	private void OnGUI()
	{
		int w = Screen.width, h = Screen.height;

		GUIStyle style = new GUIStyle();

		Rect rect = new Rect(0, 0, w, h * 2f / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 100;
		style.normal.textColor = Color.white;// new Color(0.0f, 0.0f, 0.5f, 1.0f);
		float sec = m_DeltaTime * 1000.0f;
		float fps = 1.0f / m_DeltaTime;
		string text = $"{sec:0.0} ms ({fps:0.} fps)";
		GUI.Label(rect, text, style);
	}
}
