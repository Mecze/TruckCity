using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AchievementWindow : MonoBehaviour {
	public AchievementNotification achievementNotification;
	public UILabel unlockProgressLabel;
	public List <string> achievements;
	public List <UISprite> achievementUIs = new List<UISprite> ();

	// Use this for initialization
	void Start () {
		achievements = new List<string> (new string[] {
			"Explorer", 			//#0 open a menu
//			"Louder, Please!", 		//#1 music or sound more than 0.8
			"Have Some Taste", 		//#1 switch theme
			"Just Can't Resist",		//#2 press replay button
			"Dishonored Kill",		//#3 skip mission
			"Be a Whale",			//#4 switch to bank panel
			"Keep It Tidy",			//#5 press reset button
			"Core Player",			//#6 press any button on upgrade window
			"Deep Reader",			//#7 scroll the label of description
			"Achiever",				//#8 unlock all the achievements
		});
	}

	public void UpdateAchievementWindow () {
		for (int i = 0; i < achievements.Count; i++) {
			achievementUIs [i].GetComponentInChildren <UILabel> ().text = achievements [i];
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (gameObject.GetComponent <NGUIExample> ().missionPanel.gameObject.activeSelf) {
			foreach (int i in achievementNotification.informedAchievements) {
				if (achievementUIs [i].transform.FindChild ("Checkmark").GetComponentInChildren <UISprite> ().spriteName != "Check_success") {
					achievementUIs [i].transform.FindChild ("Checkmark").GetComponentInChildren <UISprite> ().spriteName = "Check_success";
				}
			}
			unlockProgressLabel.text = "You have unlocked " + achievementNotification.informedAchievements.Count + "/9";
		}

	}
}
