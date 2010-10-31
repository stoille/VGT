using UnityEngine;
using System.Collections;
using System.Reflection;
using System;

public class VGTUnityUpdater : MonoBehaviour {

	private string _gameMsg = "";
	
	void OnGUI () {
		GUI.Button(new Rect(10,10,200,25), _gameMsg); 
	}
	
	//msg format: time@$GameObj1Name$ScriptName$ScriptParam=value@$GameObj2Name$etc....
	void UpdateGame(string gameMsg){
		
		//parse the msg into an array of values to update
		string[] objStrs = gameMsg.Split('@');
		//realize the objects in the array based off of their name
		for(int i=1; i<objStrs.Length;++i){
			string obj = objStrs[i];
			string[] objInfo = obj.Split('=');

			string[] objPaths = objInfo[0].Split('|');
			string objId = objPaths[0];
			string objPath = objPaths[1];
			string objVal = objPaths[2];
			
			VGTUpdater.GOInfo goi = VGTUpdater.GOMap[objId];
			goi.Vars[objPath] = objVal;
			goi.IsDirty = true;
		}
		
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {	
	}
}
