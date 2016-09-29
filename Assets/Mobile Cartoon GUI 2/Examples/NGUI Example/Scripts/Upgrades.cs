using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Upgrades : MonoBehaviour {
	public List <UIProgressBar> upgradesProgress = new List<UIProgressBar> ();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ReduceEnergy () {
		ProgressBarChange (1, false);
	}

	public void AddEnergy () {
		ProgressBarChange (1, true);
	}

	public void ReduceHealth () {
		ProgressBarChange (0, false);
	}
	
	public void AddHealth () {
		ProgressBarChange (0, true);
	}

	public void ReduceShield () {
		ProgressBarChange (2, false);
	}
	
	public void AddShield () {
		ProgressBarChange (2, true);
	}

	void ProgressBarChange (int progressBarID, bool isAdd) {
		upgradesProgress[progressBarID].value += (isAdd) ? 0.1f : -0.1f;
	}
}
