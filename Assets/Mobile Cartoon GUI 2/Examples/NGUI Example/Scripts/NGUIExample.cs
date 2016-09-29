using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NGUIExample : MonoBehaviour {
	public List <UISlider> bars = new List<UISlider> ();
	private float sliderDelay = 5.0f;
	private List <GameObject> panels;
	private List <UIButton> buttons;
	public UIPanel levelPanel;
	public UIPanel completePanel;
	public UIPanel resultPanel;
	public UIPanel storePanel;
	public UIPanel missionPanel;
	public UIPanel settingsPanel;
	public UIPanel descriptionPanel;
	public UIPanel statesPanel;
	public UIPanel startPanel;
	public UIPanel themeSwitchPanel;

	private bool switchPanelDisplay;

	public UIButton levelButton;
	public UIButton completeButton;
	public UIButton resultButton;
	public UIButton storeButton;
	public UIButton missionButton;
	public UIButton settingsButton;
	public UIButton descriptionButton;

	void Start () {
		panels = new List<GameObject> () {
			levelPanel.gameObject,
			completePanel.gameObject,
			resultPanel.gameObject,
			storePanel.gameObject,
			missionPanel.gameObject,
			settingsPanel.gameObject,
			descriptionPanel.gameObject,
			startPanel.gameObject
		};

		buttons = new List<UIButton> () {
			levelButton,
			completeButton,
			resultButton,
			storeButton,
			missionButton,
			settingsButton,
			descriptionButton
		};
	}
	
	void Update () {
		int i = 0;
		foreach (UISlider bar in bars) {
			bar.value = Mathf.Sin (Time.time + sliderDelay * i) * 0.5f + 0.5f;
			i++;
		}
	}

	public void GoToLevel () {
		GoToPanel (levelPanel);
		UpdateDockButtons (levelButton);
	}

	public void GoToComplete () {
		GoToPanel (completePanel);
		UpdateDockButtons (completeButton);
	}

	public void GoToResult () {
		GoToPanel (resultPanel);
		UpdateDockButtons (resultButton);
	}

	public void GoToStore () {
		GoToPanel (storePanel);
		UpdateDockButtons (storeButton);
	}

	public void GoToMission () {
		GoToPanel (missionPanel);
		UpdateDockButtons (missionButton);
		gameObject.GetComponent <AchievementWindow> ().UpdateAchievementWindow ();
	}

	public void GoToSettings () {
		GoToPanel (settingsPanel);
		UpdateDockButtons (settingsButton);
	}

	public void GoToDescription () {
		GoToPanel (descriptionPanel);
		UpdateDockButtons (descriptionButton);
	}

	void GoToPanel (UIPanel targetPanel) {
		foreach (GameObject g in panels) {
			NGUITools.SetActive (g,false);
		}
		NGUITools.SetActive (targetPanel.gameObject, true);
		NGUITools.SetActive (statesPanel.gameObject, true);
		gameObject.GetComponent <NGUIThemeSwitch> ().SwitchTheme (gameObject.GetComponent <NGUIThemeSwitch> ().currentTheme);

		if (!switchPanelDisplay && targetPanel != startPanel) {
			NGUITools.SetActive (themeSwitchPanel.gameObject, true);
			switchPanelDisplay = true;

			StopDockButtonBouncing ();
		}
	}

	void UpdateDockButtons (UIButton focusButton) {
		foreach (UIButton b in buttons) {
			b.isEnabled = true;
		}
		focusButton.isEnabled = false;
	}

	void StopDockButtonBouncing () {
		levelButton.GetComponent <DockButtonBouncing> ().StopBouncing ();
	}
}
