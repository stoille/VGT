package
{
	import flash.display.Sprite;
	import flash.net.*;
	import flash.events.NetStatusEvent;
	import flash.external.ExternalInterface;
	
	public class ASGraph extends Sprite
	{
		public function ASGraph()
		{
			init();
		}
			
		private const SERVER_ADDRESS:String = "rtmfp://stratus.adobe.com/";
		private const DEVELOPER_KEY:String = "7a00ffac96b9169c6b53524f-d3aea86aca8c";
		
		private var nc:NetConnection;
		private var myPeerID:String;
		private var farPeerID:String;
		// streams
		private var sendStream:NetStream;
		private var recvStream:NetStream;
		private var graphMsg:String;
		
		private function initConnection():void{	
			nc = new NetConnection();
			nc.addEventListener(NetStatusEvent.NET_STATUS,ncStatus);
			nc.connect(SERVER_ADDRESS+DEVELOPER_KEY);			
		}
		
		private function ncStatus(event:NetStatusEvent):void{
			
			trace(event.info.code);
			
			myPeerID = nc.nearID;
			
			
			if(event.info.code == "NetConnection.Connect.Success")
				initSendStream();
			//else Alert.show(event.info.code,"ncStatus");
			
		}
		
		private function initSendStream():void{
			
			sendStream = new NetStream(nc,NetStream.DIRECT_CONNECTIONS);
			sendStream.addEventListener(NetStatusEvent.NET_STATUS, netStatusHandler);
			sendStream.publish("media");
			
			var sendStreamClient:Object = new Object();
			sendStreamClient.onPeerConnect = function(callerns:NetStream):Boolean{
				
				farPeerID = callerns.farID;
				trace("onPeerConnect "+farPeerID);
				//initRecvStream();
				
				return true;
			}
			
			sendStream.client = sendStreamClient;	
			
			initRecvStream();
		}
		
		private function initRecvStream():void{
			//Alert.show("initRecvStream");
			if(farPeerID)
			{
				recvStream = new NetStream(nc,farPeerID);
				recvStream.addEventListener(NetStatusEvent.NET_STATUS,netStatusHandler);
				recvStream.play("media");
				
				recvStream.client = this;
				
				//Alert.show("Connection with game established.");
			}else trace("far peer id is null");
		}
		
		public function receiveSomeData(graphMsg:String):void{
			//Alert.show("Got the msg!","//Alert");
			ExternalInterface.call("UpdateGraph",graphMsg);
		}			
		
		private function sendSomeData(gameMsg:String):void{
			//	sendStream.send("UpdateGraph",gameMsg);
		}
		
		private function netStatusHandler(event:NetStatusEvent):void{
			trace(event.info.code);
			//Alert.show(event.info.code,"netStatusHandler");
		}
		
		private function init():void{
			
			//this.farPeerID = "f35739bc9316e42cfa4c71a55939181d8aa121480f24255aacce42fb1eda6880";//
			this.farPeerID = getHtmlParameters("farID");
			////Alert.show(this.farPeerID);
			initConnection();
			
		}
		
		public static function getHtmlParameters(querystring:String):String
		{
			// retrieve the querry string
			var uparam:String = ExternalInterface.call("window.location.search.toString");
			if(uparam==null) return null;
			else uparam=uparam.replace("\?","&");
			
			// build the parameter+value pairs array
			var paramArray:Array = uparam.split("&");
			// build the parameters object
			var paramsObject:Object = new Object;
			for(var x:int=0; x<paramArray.length; x++)
			{
				// split the name/value pair on “=”
				var splitArray:Array = paramArray[x].split('=');
				// retrieve name and value for this parameter
				var name:String = splitArray[0];
				var value:String = splitArray[1];
				// adds the parameter to the result object
				if (querystring== name) {return value;}
			}
			// returns the result object
			return "";
		}
		
	}
}