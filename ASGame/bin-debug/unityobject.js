// Licensed Under the "I will buy milkytreat a beer" license.
// version 0.3 last updated 16/03/07
// Special Thanks to OTEE and Geoff Stearns 
 
if(typeof otee == "undefined") var otee = new Object();
otee.UnityObject = function (unityweb, id, w, h, bg, border, redirectUrl) {
    if (!document.getElementById) { return; }
    this.attributes = new Array();
    this.params = new Object();
    if(unityweb) { this.setAttribute('src', unityweb); }
	if(id) { this.setAttribute('id', id); }
	if(w) { this.setAttribute('width', w); }
	if(h) { this.setAttribute('height', h); }
	if(bg) { this.addParam('backgroundcolor', bg); }
	if(border) { this.addParam('bordercolor', border); }
    this.setAttribute('redirectUrl', '');
	if(redirectUrl) { this.setAttribute('redirectUrl', redirectUrl); }
}
 
otee.UnityObject.prototype = {
		detectUnityWebPlayer: function () {
			var tInstalled = false;
			if (navigator.appVersion.indexOf("MSIE") != -1 && navigator.appVersion.toLowerCase().indexOf("win") != -1) {
				document.write(" \n");
				document.write("<script language='VBscript'> \n");
					document.write("function DetectUnityWebPlayerActiveX \n");
						document.write("on error resume next \n");
						document.write("dim tControl \n");
						document.write("dim res \n");
						document.write("res = 0 \n");
						document.write("set tControl = CreateObject(\"UnityWebPlayer.UnityWebPlayer.1\") \n");
						document.write("if IsObject(tControl) then \n");
							document.write("res = 1 \n");
						document.write("end if \n");
						document.write("DetectUnityWebPlayerActiveX = res\n");
					document.write("end function\n");
				document.write("</script>\n");				
				
				tInstalled = DetectUnityWebPlayerActiveX();
				
			} else {
				if (navigator.mimeTypes && navigator.mimeTypes["application/vnd.unity"] && navigator.mimeTypes["application/vnd.unity"].enabledPlugin) {
					if (navigator.plugins && navigator.plugins["Unity Player"]) {
						tInstalled = true;	
					}
				}
			}
			
			return tInstalled;	
	},

	writeEmbedDOM: function() {
			var uniSrc = '<object classid="clsid:444785F1-DE89-4295-863A-D46C3A781394" codebase="http://webplayer.unity3d.com/download_webplayer-2.x/UnityWebPlayer.cab#version=2,0,0,0" id="'+this.getAttribute('id')+'_object" width="'+this.getAttribute('width')+'" height="'+this.getAttribute('height')+'"><param name="src" value="'+this.getAttribute('src')+'" />'; 
			var params = this.getParams();
			for(var key in params) {
				uniSrc += '<param name="'+ key +'" value="'+ params[key] +'" />';
			}
			uniSrc += '<embed type="application/vnd.unity" pluginspage="http://www.unity3d.com/unity-web-player-2.x" id="'+this.getAttribute('id')+'_embed" width="'+this.getAttribute('width')+'" height="'+this.getAttribute('height')+'" src="'+this.getAttribute('src')+'"';
			
			var params = this.getParams();
			for(var key in params){
				uniSrc += [key] +'="'+ params[key] +'" ';
			}
			
			uniSrc += ' /></object\>';
			return uniSrc;
	},
	
	addParam: function(name, value){
		this.params[name] = value;
	},

	getParams: function(){
		return this.params;
	},

	setAttribute: function(name, value){
		this.attributes[name] = value;
	},

	getAttribute: function(name){
		return this.attributes[name];
	},

	write: function (elementId) {
		if(this.detectUnityWebPlayer()) {
			document.write(this.writeEmbedDOM());
			this.findEar();
			return true;
		} else {
			if(this.getAttribute('altHTML') != "") {
				document.write(this.getAttribute('altHTML'));
			} else if(this.getAttribute('redirectUrl') != "") {
				document.location.replace(this.getAttribute('redirectUrl'));
			}
		}
		return false;
	},

	findEar: function () {
		this.unityEar = "";
		if (navigator.appVersion.indexOf("MSIE") != -1 && navigator.appVersion.toLowerCase().indexOf("win") != -1) {
			this.unityEar = document.getElementById(this.getAttribute('id')+"_object");
		} else if (navigator.appVersion.toLowerCase().indexOf("safari") != -1) {
			this.unityEar = document.getElementById(this.getAttribute('id')+"_object")
		} else {
			this.unityEar = document.getElementById(this.getAttribute('id')+"_embed");
		}
		document.Unity = this.unityEar;
	},
    
	msg: function (unObj, unFunc, unVar) {
		this.unityEar.SendMessage(unObj, unFunc, unVar);
	}    
}

if (!document.getElementById && document.all) { document.getElementById = function(id) { return document.all[id]; }}
var UnityObject = otee.UnityObject;