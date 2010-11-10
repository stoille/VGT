using UnityEngine;
using System.Collections;
using System.Reflection;
using System;

public class VGTUnityUpdater : MonoBehaviour {

	private string _gameMsg = "";
	
	void OnGUI () {
		GUI.Button(new Rect(10,10,200,25), _gameMsg); 
	}
	
	//msg format: GameObj1Name:ScriptName:ScriptParam,value@GameObj2Name:etc....
	void UpdateGame(string gameMsg){
		//parse the msg into an array of values to update
		_gameMsg = gameMsg;
		string[] objInfo = gameMsg.Split(',');
		//get the object names
		string[] objPaths = objInfo[0].Split(':');
		string objName = objPaths[0];
		string objScriptName = objPaths[1];
		string objVarName = objPaths[2];
		//get the variable value
		string objValStr = objInfo[1];
		//fine the game object whos name matches and its script
		GameObject go = GameObject.Find(objName);
		Component goScript = go.GetComponent(objScriptName);
	
		FieldInfo fi = goScript.GetType().GetField(objVarName);
		try{
			//try to change the value for the specified variable
			object o = objValStr;
			object objVal = System.Convert.ChangeType(o,fi.GetValue(goScript).GetType());
			//go.GetType().GetField(objVarName).SetValue(goScript,objVal);
			string camelCasedVarName = objVarName.Substring(0,1).ToUpper() + 
				objVarName.Substring(1,objVarName.Length);
			goScript.SendMessage("Set"+objVarName,objVal);
		}catch(Exception e){
			Application.ExternalCall("alert","Exception: "+e.ToString());
		}
		
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//UpdateGame("Car:Car:maximumTurn,-10");//+ new System.Random().Next(20).ToString());
	}
}
