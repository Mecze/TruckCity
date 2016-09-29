using UnityEngine;

public class NotificationTextFX : MonoBehaviour
{
	public int charsPerSecond = 40;
	
	UILabel mLabel;
	string originalText;
	string mText;
	int mOffset = 0;
	float mNextChar = 0f;
	bool mReset = true;
	
	void OnEnable () { 
		mReset = true; 
	}
	
	void Update ()
	{
		if (mReset)
		{
			mOffset = 0;
			mReset = false;
			mLabel = GetComponent<UILabel>();
			mText = mLabel.processedText;
		}
		
		if (mOffset < mText.Length && mNextChar <= RealTime.time)
		{
			charsPerSecond = Mathf.Max(1, charsPerSecond);
			
			// Periods and end-of-line characters should pause for a longer time.
			float delay = 1f / charsPerSecond;
			char c = mText[mOffset];
			if (c == '.' || c == '\n' || c == '!' || c == '?') delay *= 4f;
			
			// Automatically skip all symbols
			NGUIText.ParseSymbol(mText, ref mOffset);
			
			mNextChar = RealTime.time + delay;
			mLabel.text = mText.Substring(0, ++mOffset);
		}
	}
}
