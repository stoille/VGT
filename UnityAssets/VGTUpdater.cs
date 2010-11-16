using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Generic.Dictionary;
using System;

public class VGTUpdater : MonoBehaviour {

	public string[] Tags;
 	private int FramesPast = 0;
	public int FrameRefresh = 30;
	private string OutMsg;
	
	public struct GOInfo
	{
		public GOInfo(string Id) { this.Id = Id; Group = ""; Name = ""; Path = ""; Value = ""; }
		public GOInfo(string Group, string Name, string Path, string Value) 
		{ this.Id = System.Guid.NewGuid().ToString(); this.Group = Group; this.Name = Name; this.Path = Path; this.Value = Value; }
		public string Id;
		public string Group;
		public string Name;
		public string Path;
		public string Value;
	}
	public static Dictionary<string, GOInfo> GOMap = new Dictionary<string, GOInfo>();
	private GOInfo _currentGO;
	
	// Use this for initialization
	void Start () {
		Application.ExternalCall("AlertVGTURL");
		_currentGO = null;
	}
	
	void Publish()
	{
		PublishObjects();
	}
	
	private void GetComponents(Component[] comps, string info)
	{	
		foreach(Component comp in comps)
		{
			
			//Debug.Log("Component$ "+info+"$"+comp.name);
			//start the path for each goInfo
			_currentGO.Path += comp.name;
			GetFields(comp,info+"$"+comp.name);
			
		}			
	}
	
	private void GetFields(object outer, string info)
    {
        foreach (FieldInfo finfo in outer.GetType().GetFields())
        {
			object fVal = finfo.GetValue(outer);
			double Num;
			//if(!fVal.GetType().IsAssignableFrom(typeof(Integer)))
			if(fVal != null)
			if(!double.TryParse(fVal.ToString(),out Num))
			{
				//Debug.Log("Field$ "+info+"$"+finfo.Name+" = NaN $ "+fVal.GetType());
				
				#region specific Unity Type Handling
				if(fVal.GetType().ToString() == "UnityEngine.Transform" )
				{
					UnityEngine.Transform xform = (UnityEngine.Transform)fVal;
					OutMsg += info+"$"+finfo.Name+"$"+xform.name+"$position$x="+xform.position.x+"@";
					OutMsg += info+"$"+finfo.Name+"$"+xform.name+"$position$y="+xform.position.y+"@";
					OutMsg += info+"$"+finfo.Name+"$"+xform.name+"$position$z="+xform.position.z+"@";
					OutMsg += info+"$"+finfo.Name+"$"+xform.name+"$rotation$x="+xform.rotation.x+"@";
					OutMsg += info+"$"+finfo.Name+"$"+xform.name+"$rotation$y="+xform.rotation.y+"@";
					OutMsg += info+"$"+finfo.Name+"$"+xform.name+"$rotation$z="+xform.rotation.z+"@";
					OutMsg += info+"$"+finfo.Name+"$"+xform.name+"$localScale$x="+xform.localScale.x+"@";
					OutMsg += info+"$"+finfo.Name+"$"+xform.name+"$localScale$y="+xform.localScale.y+"@";
					OutMsg += info+"$"+finfo.Name+"$"+xform.name+"$localScale$z="+xform.localScale.z+"@";
				}
				else if(fVal.GetType().ToString() == "UnityEngine.Transform[]")
				{
					UnityEngine.Transform[] xforms = (UnityEngine.Transform[])fVal;
					foreach(Transform xform in xforms)
					{
						OutMsg += info+"$"+finfo.Name+"$"+xform.name+"$position$x="+xform.position.x+"@";
						OutMsg += info+"$"+finfo.Name+"$"+xform.name+"$position$y="+xform.position.y+"@";
						OutMsg += info+"$"+finfo.Name+"$"+xform.name+"$position$z="+xform.position.z+"@";
						OutMsg += info+"$"+finfo.Name+"$"+xform.name+"$rotation$x="+xform.rotation.x+"@";
						OutMsg += info+"$"+finfo.Name+"$"+xform.name+"$rotation$y="+xform.rotation.y+"@";
						OutMsg += info+"$"+finfo.Name+"$"+xform.name+"$rotation$z="+xform.rotation.z+"@";
						OutMsg += info+"$"+finfo.Name+"$"+xform.name+"$localScale$x="+xform.localScale.x+"@";
						OutMsg += info+"$"+finfo.Name+"$"+xform.name+"$localScale$y="+xform.localScale.y+"@";
						OutMsg += info+"$"+finfo.Name+"$"+xform.name+"$localScale$z="+xform.localScale.z+"@";
						
					}
				}
				else if(fVal.GetType().ToString() == "UnityEngine.Vector2" )
				{
					Vector2 vec2 = (Vector2)fVal;
					OutMsg += info+"$"+finfo.Name+"$x="+vec2.x+"@";
					OutMsg += info+"$"+finfo.Name+"$y="+vec2.y+"@";
				}				
				else if(fVal.GetType().ToString() == "UnityEngine.Vector3")
				{
					Vector3 vec3 = (Vector3)fVal;
					OutMsg += info+"$"+finfo.Name+"$x="+vec3.x+"@";
					OutMsg += info+"$"+finfo.Name+"$y="+vec3.y+"@";
					OutMsg += info+"$"+finfo.Name+"$z="+vec3.z+"@";
				}				
				else if(fVal.GetType().ToString() == "UnityEngine.Vector4")
				{
					Vector4 vec4 = (Vector4)fVal;
					OutMsg += info+"$"+finfo.Name+"$w="+vec4.w+"@";
					OutMsg += info+"$"+finfo.Name+"$x="+vec4.x+"@";
					OutMsg += info+"$"+finfo.Name+"$y="+vec4.y+"@";
					OutMsg += info+"$"+finfo.Name+"$z="+vec4.z+"@";
				}
				#endregion
				
				if(fVal.GetType().IsSubclassOf(typeof(Component)))
				{
					//Debug.Log("Field$ "+finfo.Name+" is subclass of Component.");
					Component go = (Component)fVal;
					Component[] comps = go.GetComponentsInChildren(fVal.GetType());
					GetComponents(comps,info+"$"+finfo.Name);
				}
				else {
					//Debug.Log("Field$ "+finfo.Name+" is nested.");
				//	GetFields(fVal,info+"$"+finfo.Name);
				} 
			} else {
				//Debug.Log("Field$ "+info+"$"+finfo.Name+" = "+fVal);
				OutMsg += info+"$"+finfo.Name+"="+fVal+"@";
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
				string goId = System.Guid.NewGuid().ToString();
				_currentGO = new GOInfo(goId);
				_currentGO.Group = tag;
				_currentGO.Name = go.name;
				GOMap.Add(goId,_currentGO);
					
				Component[] comps = go.GetComponentsInChildren<Component>();
				GetComponents(comps,go.name);
				//not sure why this wont work? isnt the rigidbody a derivative of component?
				//comps = go.GetComponentsInChildren<Rigidbody>();
				//GetComponents(comps,go.name);
				}catch(Exception e)
				{ Debug.Log(e.ToString()); }
			}
		}
	
		Application.ExternalCall("UpdateGraph", OutMsg);
		// Wait for two seconds
	   // yield return new WaitForSeconds (2);
		
	}
	
	// Update is called once per frame
	void Update () {
		if(FramesPast >= FrameRefresh)
		{
			FramesPast = 0;
			Publish();
			OutMsg = System.DateTime.Now.Hour+":"+System.DateTime.Now.Minute+":"+System.DateTime.Now.Second + ":"+System.DateTime.Now.Millisecond+"@";
		}
		++FramesPast;
	}
}
