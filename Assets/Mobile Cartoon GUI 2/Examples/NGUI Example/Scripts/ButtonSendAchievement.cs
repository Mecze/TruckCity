using UnityEngine;
using System.Collections;

public class ButtonSendAchievement : MonoBehaviour {
	public AchievementNotification AN;
	public int achievementID;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnClick() {
		AN.AddAchievement (achievementID);
	}
}
