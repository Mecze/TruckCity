using UnityEngine;
using System.Collections;

public class SettingsWindow : MonoBehaviour {
	public AchievementNotification AN;
	public UIProgressBar[] musicBars;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame

	public void SoundAchievement () {
		AN.AddAchievement (1);
	}
}
