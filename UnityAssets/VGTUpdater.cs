using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System;

public class VGTUpdater : MonoBehaviour {

	public string[] Tags;
 	private int FramesPast = 0;
	public int FrameRefresh = 30;
	public long _goId = 0;
	private string OutMsg;
	
	public struct GOInfo
	{
		public GOInfo(string Id) { this.Id = Id; Group = ""; Name = ""; Vars = new Dictionary<string, string>(); IsDirty = false; }
		public string Id;
		public string Group;
		public string Name;
		//Map<Path,Value>
		public Dictionary<string,string> Vars;
		public bool IsDirty;
	}
	public static Dictionary<string, GOInfo> GOMap = new Dictionary<string, GOInfo>();
	private GOInfo _currentGO;
	
	// Use this for initialization
	void Start () {
		Application.ExternalCall("AlertVGTURL");
	}
	
	private void GetComponents(Component[] comps,string parent)
	{	
		foreach(Component comp in comps)
		{
			//start the path for each goInfo
			GetFields(comp,parent);
			
		}			
	}
	
	private void GetFields(object field, string parent)
    {
        foreach (FieldInfo finfo in field.GetType().GetFields())
        {
			object fVal = finfo.GetValue(field);
			double Num;
			if(fVal != null)
				//if the value isnt a number
				if(!double.TryParse(fVal.ToString(),out Num))
				{
					//Ignore all other types here: Vectors, Positions, Rotations, etc...
				
					//we are only interested in sub components
					if(fVal.GetType().IsSubclassOf(typeof(Component)))
					{
						Component go = (Component)fVal;
						Component[] comps = go.GetComponentsInChildren(fVal.GetType());
						GetComponents(comps,finfo.Name + "$");
					}
					else {
						//Debug.Log("Field$ "+finfo.Name+" is nested.");
					//	GetFields(fVal,info+"$"+finfo.Name);
					} 
				} else {
					_currentGO.Vars[parent + finfo.Name] = Convert.ToString(fVal);
				}
        }
    }
	
	void PublishObjects()
	{
		foreach(string tag in Tags)
		{
			//Debug.Log("Tag$ "+tag);
			GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tag);
			//Debug.Log("Game objects found");
			
			foreach(GameObject go in gameObjects)
			{
				try{
					//Debug.Log("GameObject$ "+go.name);
						
					//create a new game object info for each object found in group named tag
					_currentGO = new GOInfo(tag+":"+go.name);
					_currentGO.Group = tag;
					_currentGO.Name = go.name;
					
						
					Component[] comps = go.GetComponentsInChildren<Component>();
					GetComponents(comps,"");
					
					string pathVarPairs = "";
					int countOfVarsForObj = 0;
					foreach(KeyValuePair<string,string> Var in _currentGO.Vars)
					{
						if(Var.Key != "" && Var.Value != "")
						{
							pathVarPairs += Var.Key + "," + Var.Value + "$";
							countOfVarsForObj++;
						}
					}
					
					//if there are variables to update then add them to the msg being sent out
					if(countOfVarsForObj > 0)
					{
						
						GOMap[_currentGO.Id] = _currentGO;
						OutMsg+= "@"+_currentGO.Id + "|" + pathVarPairs.Remove(pathVarPairs.Length-1); //remove trailing $
					}
					
				}catch(Exception e)
				{ Debug.Log(e.ToString()); }
			}
		}
	
		Application.ExternalCall("UpdateGraph", OutMsg);
		// Wait for two seconds
	   // yield return new WaitForSeconds (2);
		
	}
	
	void UpdateObjects()
	{
		foreach(KeyValuePair<string,VGTUpdater.GOInfo> goip in GOMap)
		{			
			GOInfo goi = goip.Value;
			if(goi.IsDirty)
			{
				//update their values in the game
				//gameMsg = "Updating - "+goi.Name+" = "+goi.Value;
				GameObject go = GameObject.Find(goi.Name);
				if(go != null)
				{
					foreach(KeyValuePair<string,string> Var in goi.Vars)
					{
						string path = Var.Key;
						string val = Var.Value;
						Component goComp = go.GetComponent(path);
						FieldInfo goInfo = goComp.GetType().GetField(path);
						try{
							//string oldVal = System.Convert.ToString(goInfo.GetValue(go));
							//print("OLD: object name: "+objName+" comp: "+compName+" field: "+fieldName + " = "+ goInfo.GetValue(go).ToString());
							object o = val;
							object objVal = System.Convert.ChangeType(o,goInfo.GetValue(go).GetType());
							//string setterName = "Set"+fieldName;
							//go.SendMessage(setterName,objVal);
							//Debug.Log("new value for "+objInfo[0]+" is: "+go.GetComponent(compName).GetType().GetField(fieldName).GetValue(go));
							//string newVal = System.Convert.ToString(goInfo.GetValue(go));
							//print("NEW: object name: "+objName+" comp: "+compName+" field: "+fieldName + " = "+ goInfo.GetValue(go).ToString());	
							goi.IsDirty = false;
						}catch(Exception e)
						{ Debug.Log(e.ToString()); }
					}
				}
			}
		}
	}
	// Update is called once per frame
	void Update () {
		if(FramesPast >= FrameRefresh)
		{
			FramesPast = 0;
			UpdateObjects();
			OutMsg = System.DateTime.Now.Hour+":"+System.DateTime.Now.Minute+":"+System.DateTime.Now.Second + ":"+System.DateTime.Now.Millisecond;
			PublishObjects();
		}
		++FramesPast;
	}
}
