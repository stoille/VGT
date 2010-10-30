//keeps track of the variables found in var data
var varMap = new Object();
var varData = new Array();

//last maxTime posted timings
var timeArray = new Array();
var maxTime = 300;

var gameMsg = "1:29:46@Car:Car|suspensionRange,0.1$suspensionDamper,50$suspensionSpringFront,18500$suspensionSpringRear,9250$throttle,0$topSpeed,80.4672$numberOfGears,5$maximumTurn,15$minimumTurn,10$resetTime,5$wheelCount,3$DVolume,0.226$EVolume,0.32$FVolume,0.312$KVolume,0.3955$LVolume,0.244$windVolume,0.32$tunnelVolume,0.8$crashLowVolume,0.8$crashHighVolume,0.5$BackgroundMusicVolume,1$sound$DVolume,0.226$sound$EVolume,0.32$sound$FVolume,0.312$sound$KVolume,0.3955$sound$LVolume,0.244$sound$windVolume,0.32$sound$tunnelVolume,0.8$sound$crashLowVolume,0.8$sound$crashHighVolume,0.5$sound$BackgroundMusicVolume,1$fudgeScale,1$textureSize,256@

Car:Car:Performance,5||Car:Car:topSpeed+Car:Car:numberOfGears+Car:Car:throttle@

Car:Handling,10||Car:Car:maximumTurn+Car:Car:minimumTurn+Car:Car:resetTime+wheelCount";

function UpdatevarMap(msg) {
	var gos = msg.split("@");
	var time = gos[0];
	var oldTime = timeArray.shift();
	timeArray.push(time);
	for (var i = 1; i < gos.length; i++) {
		var goMsg = gos[i];
		var params = goMsg.split("|");
		var goId = params[0];
		var varStr = params[1];
		var exprStr = params[2];
		var go = null;
		//if there are a list of variables, this msg describes a gameMsg update
		if(varStr != "" ){
			//traverse through all variables and create/update values
			var goPathPairs = varStr.split("$");
			for (var j = 1; j < goPathPairs.length; j++) {
				var gopp = goPathPairs[j].split(",");
				var varName = goId + ":" + gopp[0];
				var newVal = gopp[1]; 
				//setup this var if it doesnt exist yet
				if (varMap[varName] == undefined) {
					go = {varName: params[0], values: new Array(), related: new Array() };
					varArray.push( go );
					varMap[varName] = varArray.length - 1;
					go.related.push(goId);
				} else { 
					go = varArray[varMap[varName]];
				}
				//update the value
				go.values.shift();
				go.values.push(newVal);
			}
		}
		else if(exprStr != ""){
			var gopp = params[0].split(",");
			var varName = gopp[0];
			var newVal = gopp[1];
			//setup this var if it doesnt exist yet
			if (varMap[varName] == undefined) {
				go = {varName: varName, values: new Array(), related: new Array() };
				varArray.push( go );
				varMap[varName] = varArray.length - 1;
				var symbs = exprStr.split("+"); //this is limited and temporary
				for(sym in symbs){	
					go.related.push(sym);
				}
			} else {
				go = varArray[varMap[goId]]];
			}
			//update the value
			go.values.shift();
			go.values.push(newVal);
		}
	}
}