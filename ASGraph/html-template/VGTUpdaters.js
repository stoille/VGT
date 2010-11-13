//keeps track of the variables found in var data
var varMap = new Object();
var varData = new Array();

//last maxTime posted timings
var timeArray = new Array();
var maxTime = 300;

//the index for the focus range
var fi = {x:200, dx:100};
var selectedGroups = new Object();

var colorArray = [ 	"steelBlue", "#0000FF", "#FF00FF", "#808080",	"#008000", "#00FF00", "#800000","#000080", "#808000", "#800080", "#FF0000",	"#C0C0C0", "#008080"];
//zoom panel stuff
var end = new Date();
var start = new Date(end - (1000*maxTime));
var related = new Array();
var selectedNames = '';
var zoomData;
var zoomIdxDate = start;
var zoomIdx = 0;
/*
var sampleExpr = '<b><font color='+colorArray[0]+'>My:own:sample = </font></b><font color='+colorArray[1]+'>My:own:sample</font>';
varData.push({varName:"My:own:sample", expression: sampleExpr, related:[0], index:0, values:[182904,196530,203944,192492,77393,81243,83653,80634,80015,84246,85383,83490,26730,27663,27360,27685,25642,26938,26407,27043,24198,25500,25935,26271,89108,93122,93594,91355,89745,94387,95463]});

selectedGroups[0] = varData[0];
*/
/* 
//sample format
var gameMsg = "1:29:46@Car:Car|suspensionRange,0.1$suspensionDamper,50$suspensionSpringFront,18500$suspensionSpringRear,9250$throttle,0$topSpeed,80.4672$numberOfGears,5$maximumTurn,15$minimumTurn,10$resetTime,5$wheelCount,3$DVolume,0.226$EVolume,0.32$FVolume,0.312$KVolume,0.3955$LVolume,0.244$windVolume,0.32$tunnelVolume,0.8$crashLowVolume,0.8$crashHighVolume,0.5$BackgroundMusicVolume,1$sound$DVolume,0.226$sound$EVolume,0.32$sound$FVolume,0.312$sound$KVolume,0.3955$sound$LVolume,0.244$sound$windVolume,0.32$sound$tunnelVolume,0.8$sound$crashLowVolume,0.8$sound$crashHighVolume,0.5$sound$BackgroundMusicVolume,1$fudgeScale,1$textureSize,256@

Car:Car:Performance,5|Car:Car:topSpeed+Car:Car:numberOfGears+Car:Car:throttle@

Car:Car:Handling,10|Car:Car:maximumTurn+Car:Car:minimumTurn+Car:Car:resetTime+wheelCount";
*/

function UpdateVarMap(msg) {
	var goMsgs = msg.split("@");
	var time = goMsgs[0];
	var oldTime = timeArray.shift();
	timeArray.push(time);
	for (var i = 1; i < goMsgs.length; i++) {
		var goMsg = goMsgs[i];
		var params = goMsg.split("|");
		var go, varName, newVal, expression;
		//if there are a list of variables, this msg describes a gameMsg update
		if(params[0].split(",").length == 1){
			var varStr = params[1];
			//traverse through all variables and create/update values
			var goPathPairs = varStr.split("$");
			for (var j = 0; j < goPathPairs.length; ++j) {
				var gopp = goPathPairs[j].split(",");
				varName = params[0] + ":" + gopp[0];
				newVal = gopp[1]; 
				if(typeof newVal != "undefined"){
					//setup this var if it doesnt exist yet
					if (varMap[varName] == undefined) {
						expression = '<b><font color='+colorArray[0]+'>'+varName+' = </font></b><font color='+colorArray[1]+'>'+varName+'</font>';
						go = {varName: varName, expression: expression, index: varData.length, values: new Array(), related: new Array() };
						varMap[varName] = go.index;
						go.related.push(go.index);
						varData.push( go );
					} else { go = varData[varMap[varName]]; }
					//update the value
					if(go.values.length > maxTime)
						go.values.shift();
					go.values.push(newVal);
				}
			}
		}
		else {
			var gopp = params[0].split(",");
			varName = gopp[0];
			newVal = gopp[1];
			if(typeof newVal != "undefined"){
				var exprStr = params[1];
				//setup this var if it doesnt exist yet
				if (varMap[varName] == undefined) {
					expression = '<b><font color='+colorArray[0]+'>'+varName+' = </font></b>';
					go = {varName: varName, expression: expression, index: varData.length, values: new Array(), related: new Array() };
					varMap[varName] = go.index;
					varData.push( go );
					var symbs = exprStr.split("+"); //this is limited and temporary
					var c = 0;
					for(sym in symbs){	
						go.related.push(varMap[sym]);
						go.expression += '<font color='+colorArray[c++]+'>'+sym+'</font> +';
					}
					go.expression = go.expression.substr(0,go.expression.length-1);
				} else { go = varData[varMap[varName]]; }
				//update the value
				if(go.values.length > maxTime)
					go.values.shift();
				go.values.push(newVal);
			}
		}
	}
}