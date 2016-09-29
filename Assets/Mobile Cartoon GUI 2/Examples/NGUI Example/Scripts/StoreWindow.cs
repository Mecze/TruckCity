using UnityEngine;
using System.Collections;

public class StoreWindow : MonoBehaviour {
	public NGUIThemeSwitch themeSwitch;

	public int currentTab;

	public Color32 fontHighlightColor;
	public Color32 fontColor;

	public UIWidget[] widgets;
	public UIButton[] tabs;

	void OnEnable () {
		ToggleWidgetA ();
		currentTab = 0;
	}
	
	public void ToggleWidgetA () {
		ToggleWidget (0);
		currentTab = 0;
	}
	
	public void ToggleWidgetB () {
		ToggleWidget (1);
		currentTab = 1;
	}
	
	public void ToggleWidgetC () {
		ToggleWidget (2);
		currentTab = 2;
	}
	
	void ToggleWidget (int panelID) {
		foreach (UIWidget p in widgets) {
			NGUITools.SetActive (p.gameObject, false);
		}
		NGUITools.SetActive (widgets[panelID].gameObject, true);

		themeSwitch.SwitchTheme (themeSwitch.currentTheme);

		foreach (UIButton b in tabs) {
			b.isEnabled = true;
			b.GetComponentInChildren <UILabel> ().color = fontColor;
		}
		tabs[panelID].isEnabled = false;
		tabs[panelID].GetComponentInChildren <UILabel> ().color = fontHighlightColor;
	}

	public void RefreshTabs () {
		foreach (UIButton b in tabs) {
			b.isEnabled = true;
			b.GetComponentInChildren <UILabel> ().color = fontColor;
		}
		tabs[currentTab].isEnabled = false;
		tabs[currentTab].GetComponentInChildren <UILabel> ().color = fontHighlightColor;
	}
}
