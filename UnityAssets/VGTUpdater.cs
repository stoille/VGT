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
						GetComponents(comps,finfo.Name + ":");
					}
					else {
						//Debug.Log("Field$ "+finfo.Name+" is nested.");
					//	GetFields(fVal,info+"$"+finfo.Name);
					} 
				} else {
		//			if(GOMap.ContainsKey(parent + finfo.Name) && Convert.ToString(fVal) != _currentGO[parent + finfo.Name)
		//				_currentGO.IsDirty = true;
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
					if(!GOMap.ContainsKey(tag+":"+go.name))
					{
						_currentGO = new GOInfo(tag+":"+go.name);
						_currentGO.Group = tag;
						_currentGO.Name = go.name;
						_currentGO.Vars = new Dictionary<string, string>();
						GOMap[_currentGO.Id] = _currentGO;
					}else _currentGO = GOMap[tag+":"+go.name];
					//populate the current game object's variables
					GetComponents(go.GetComponentsInChildren<Component>(),"");
					
					string pathVarPairs = "";
					int countOfVarsForObj = 0;
					foreach(KeyValuePair<string,string> Var in _currentGO.Vars)
						if(Var.Key != "" && Var.Value != "")
						{
							pathVarPairs += Var.Key + "," + Var.Value + "$";
							countOfVarsForObj++;
						}
					
					//if there are variables to update then add them to the msg being sent out
					if(countOfVarsForObj > 0)
						OutMsg+= "@"+_currentGO.Id + "|" + pathVarPairs.Remove(pathVarPairs.Length-1); //remove trailing $
				}catch(Exception e)
				{ Debug.Log(e.ToString()); }
			}
		}
	
		Application.ExternalCall("UpdateGraph", OutMsg);
	}
	
	// Update is called once per frame
	void Update () {
		if(FramesPast >= FrameRefresh)
		{
			FramesPast = 0;
			//UpdateObjects();
			OutMsg = System.DateTime.Now.Hour+":"+System.DateTime.Now.Minute+":"+System.DateTime.Now.Second + ":"+System.DateTime.Now.Millisecond;
			PublishObjects();
		}
		++FramesPast;
	}
}
