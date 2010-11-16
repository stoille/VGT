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
		string time = objStrs[0];
		//realize the objects in the array based off of their name
		for(int i=1; i<objStrs.Length;++i){
			string obj = objStrs[i];
			string[] objInfo = obj.Split('=');
			//string objVal = objInfo[1];
			//obj objValNum;
			//if(!double.TryParse(objVal,out objValNum))
			//	return;
			string[] objPaths = objInfo[0].Split('$');
			string objName = objPaths[0];
			string compName = objPaths[1];
			string fieldName = objPaths[2];
			for(int j = 4; j<objPaths.Length;++j)
				fieldName += "."+objPaths[j];
			//update their values in the game
			_gameMsg = "Updating - "+fieldName+" = "+objInfo[1];
			GameObject go = GameObject.Find(objName);
			if(go != null)
			{
				Component goComp = go.GetComponent(compName);
				FieldInfo goInfo = goComp.GetType().GetField(fieldName);
				try{
					//string oldVal = System.Convert.ToString(goInfo.GetValue(go));
					//print("OLD: object name: "+objName+" comp: "+compName+" field: "+fieldName + " = "+ goInfo.GetValue(go).ToString());
					object o = objInfo[1];
					object objVal = System.Convert.ChangeType(o,goInfo.GetValue(go).GetType());
					string setterName = "Set"+fieldName;
					go.SendMessage(setterName,objVal);
					//Debug.Log("new value for "+objInfo[0]+" is: "+go.GetComponent(compName).GetType().GetField(fieldName).GetValue(go));
					//string newVal = System.Convert.ToString(goInfo.GetValue(go));
					//print("NEW: object name: "+objName+" comp: "+compName+" field: "+fieldName + " = "+ goInfo.GetValue(go).ToString());	
				}catch(Exception e)
				{ Debug.Log(e.ToString()); }
			}
		}
		
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {	
	}
}
