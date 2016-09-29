using UnityEngine;
using System.Collections;

public class NGUIThemeSwitch : MonoBehaviour {
	public int currentTheme = 0;

	public UIButton[] themeButtons;

	public UIAtlas[] atlases;

	public Color32[] colorTheme01;
	public Color32[] colorTheme02;
	public Color32[] colorTheme03;
	public Color32[] colorTheme04;
	public Color32[] colorTheme05;
	
	public StoreWindow storeWindow;

	public void SwitchTheme01 () {
		SwitchTheme (0);
	}

	public void SwitchTheme02 () {
		SwitchTheme (1);
	}

	public void SwitchTheme03 () {
		SwitchTheme (2);
	}

	public void SwitchTheme04 () {
		SwitchTheme (3);
	}

	public void SwitchTheme05 () {
		SwitchTheme (4);
	}

	public void SwitchTheme (int themeID) {
		GameObject[] UISprites;
		UISprites = GameObject.FindGameObjectsWithTag ("NGUI");
		Color32[] targetColors = colorTheme01;
		storeWindow.fontHighlightColor = targetColors[0];
		switch (themeID) {
		case 0:
			targetColors = colorTheme01;
			break;
		case 1:
			targetColors = colorTheme02;
			break;
		case 2:
			targetColors = colorTheme03;
			break;
		case 3:
			targetColors = colorTheme04;
			break;
		case 4:
			targetColors = colorTheme05;
			storeWindow.fontHighlightColor = targetColors[0];
			break;
		default:
			targetColors = colorTheme01;
			break;
		}

		foreach (GameObject go in UISprites) {
			if (go.GetComponent <UISprite> () != null) {
				go.GetComponent <UISprite> ().atlas = atlases[themeID];
			}
			if (go.GetComponent <UILabel> () != null) {
				if (go.GetComponent <UILabel> ().name == "Label") {
					go.GetComponent <UILabel> ().color = targetColors[0];
				}
				
				if (go.GetComponent <UILabel> ().name == "ButtonLabel01") {
					go.GetComponent <UILabel> ().color = targetColors[0];
				}
				if (go.GetComponent <UILabel> ().name == "ButtonLabel02") {
					if (themeID == 4) {
						go.GetComponent <UILabel> ().color = targetColors[0];
					}
					else go.GetComponent <UILabel> ().color = targetColors[1];
				}
				
				if (go.GetComponent <UILabel> ().name == "TextLabel") {
					if (themeID == 4) {
						go.GetComponent <UILabel> ().color = targetColors[0];
					}
					else go.GetComponent <UILabel> ().color = targetColors[1];
				}
				
				if (go.GetComponent <UILabel> ().name == "FieldLabel") {
					go.GetComponent <UILabel> ().color = targetColors[1];
				}
				
				if (go.GetComponent <UILabel> ().name == "ResultLabel") {
					go.GetComponent <UILabel> ().color = targetColors[2];
				}
				
				if (go.GetComponent <UILabel> ().name == "AchievementLabel") {
					go.GetComponent <UILabel> ().color = targetColors[0];
				}
				
				if (go.GetComponent <UILabel> ().name == "AchievementPopup") {
					go.GetComponent <UILabel> ().color = targetColors[0];
				}
				
				if (go.GetComponent <UILabel> ().name == "LevelLabel") {
					go.GetComponent <UILabel> ().color = targetColors[0];
				}
				
				if (go.GetComponent <UILabel> ().name == "TitleLabel") {
					if (themeID == 2 || themeID == 3) {
						go.GetComponent <UILabel> ().color = targetColors[1];
					}
					else go.GetComponent <UILabel> ().color = targetColors[0];
				}

				if (go.GetComponent <UILabel> ().name == "LevelUpLabel") {
					if (themeID == 1) {
						go.GetComponent <UILabel> ().color = targetColors[0];
					}
					else go.GetComponent <UILabel> ().color = targetColors[1];
				}
			}
		}

		if (gameObject.GetComponent <NGUIExample> ().storePanel.gameObject.activeSelf) {
			storeWindow.RefreshTabs ();
		}

		currentTheme = themeID;
		UpdateThemeButtons (currentTheme);
	}

	void UpdateThemeButtons (int focusButton) {
		foreach (UIButton b in themeButtons) {
			b.isEnabled = true;
		}
		themeButtons[focusButton].isEnabled = false;
	}
}
