using UnityEngine;
using System.Collections;

public class FadeLight : MonoBehaviour {

	public Light lightToDim = null;    
	public float maxTime = 30; //30 seconds
	
	private float mEndTime = 0;
	private float mStartTime = 0;
	
	private void Awake()
	{
		mStartTime = Time.time;
		mEndTime= mStartTime + maxTime;
	}
	
	private void Update()
	{
		if(lightToDim)
			lightToDim.intensity = Mathf.InverseLerp(mEndTime, mStartTime, Time.time)*4;
	}
}
