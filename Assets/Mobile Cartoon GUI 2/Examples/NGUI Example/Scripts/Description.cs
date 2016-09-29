using UnityEngine;
using System.Collections;

public class Description : MonoBehaviour {
	public AchievementNotification AN;
	public UIPanel scrollView;
	private Vector2 initialClippingOffset;

	public void MailToGameCube () {
		Application.OpenURL ("mailto:gcassets@gmail.com");
	}
	
	public void GoToTwitter () {
		Application.OpenURL ("https://twitter.com/GameCubeAssets");
	}
	
	public void GoToAssetStore () {
		Application.OpenURL ("https://www.assetstore.unity3d.com/#/publisher/5840");
	}
	
	public void LoadDFGUIExample () {
		Application.LoadLevel ("DFGUI Example");
	}

	void Start () {
		initialClippingOffset = scrollView.clipOffset;
	}

	void Update () {
		if (scrollView.clipOffset != initialClippingOffset) {
			AN.AddAchievementReader ();
		}
	}
}
