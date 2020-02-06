using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ParticleManager : MonoBehaviour {


	public int pLength=0;
	public int pCurrent;
	public Text pText;
	public int pTest=0;
	public GameObject[] particles;

	public bool disableObject = false;
	public GameObject goToDisable;
	void Start(){
		pLength = transform.childCount;
		particles = new GameObject[pLength];
		pTest = 0;
		foreach(Transform child in gameObject.transform){
			particles[pTest] = child.gameObject;
			pTest++;
		}

		pLength = particles.Length;
		pCurrent = 0;
		particles [pCurrent].SetActive (true);
		pText.text = particles [pCurrent].name;

		if(disableObject){
			goToDisable.SetActive(false);
		}
	}



	public void GoForward(){
		//Debug.Log (pCurrent);
		//Debug.Log (pLength);
		if((pCurrent+1) < pLength){
			particles [pCurrent].SetActive (false);
			particles [pCurrent+1].SetActive (true);
			pCurrent+=1;
		}
		else{
			particles [pCurrent].SetActive (false);
			pCurrent = 0;
			particles [pCurrent].SetActive (true);
		}

		pText.text = particles [pCurrent].name;
	}

	public void GoBackward(){
		//Debug.Log (pCurrent);
		//Debug.Log (pLength);
		if((pCurrent) > 0){
			particles [pCurrent].SetActive (false);
			particles [pCurrent-1].SetActive (true);
			pCurrent-=1;
		}
		else{
			particles [pCurrent].SetActive (false);
			pCurrent = pLength-1;
			particles [pCurrent].SetActive (true);
		}
		pText.text = particles [pCurrent].name;
	}
}
