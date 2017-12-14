
var base_URL = 'http://localhost:59824/api/';
	
function sendRequest(callback,URL="",method="GET",body=null) {
    var xhr = new XMLHttpRequest();
    xhr.callback = callback;
    xhr.arguments = Array.prototype.slice.call(arguments, 2);
    xhr.onload = xhrSuccess;
    xhr.onerror = xhrError;
    xhr.open(method, URL, true);
	xhr.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
    xhr.send(body);
}
function xhrSuccess() { 
	console.log(this.statusText+' '+this.responseText); 
    if (this.status==200 || this.status==204){
		this.callback.apply(this, this.arguments); 
	}
}
function xhrError() { 
	console.log(this.statusText+' '+this.responseText); 
}
	
function searchSend(){
	sendRequest(searchResponse,base_URL+'patient?search='+document.getElementById("search").value+'&limit=5','GET');
}
function searchResponse(){
	var res = "";
	var obj = JSON.parse(this.responseText);
	obj.forEach(function(patient) { res += patient.admin.nom+" "+patient.admin.prenom+" <button type='button' onclick=editSend("+patient.id+")>Editer</button><br/>";});
	document.getElementById("result").innerHTML = res;
}

function deleteSend(id){
	sendRequest(deleteResponse,base_URL+'patient/'+id,'DELETE');
}
function deleteResponse(){
	returnToSearch()
}

function updateSend(id){
		
	var form = document.getElementsByTagName("form");
	var inputs = form[0].getElementsByTagName("input");

	var formData = {};
	for(var i=0; i< inputs.length; i++){
		
		if (inputs[i].name.indexOf(".")>-1){
			
			var s = inputs[i].name.split(".");

			if(formData[s[0]]){
				var myObj = formData[s[0]]
			}else{
				var myObj = new Object;
			}
			myObj[s[1]] = inputs[i].value;
			
			formData[s[0]] = myObj;
			
		}else{
			formData[inputs[i].name] = inputs[i].value;
		}
	}

	var formdata = JSON.stringify(formData);
	console.log(formdata)

	sendRequest(updateResponse,base_URL+'patient/'+id,'PUT',formdata);
}
function updateResponse(){
	returnToSearch()
}
		
function createSend(){
		
	var form = document.getElementsByTagName("form");
	var inputs = form[0].getElementsByTagName("input");

	var formData = {};
	for(var i=0; i< inputs.length; i++){
		
		if (inputs[i].name.indexOf(".")>-1){
			
			var s = inputs[i].name.split(".");

			if(formData[s[0]]){
				var myObj = formData[s[0]]
			}else{
				var myObj = new Object;
			}
			myObj[s[1]] = inputs[i].value;
			
			formData[s[0]] = myObj;
			
		}else{
			formData[inputs[i].name] = inputs[i].value;
		}
	}

	var formdata = JSON.stringify(formData);
	console.log(formdata)

	sendRequest(createResponse,base_URL+'patient/','POST',formdata);
}
function createResponse(){
	returnToSearch()
}
		
function editSend(id){

	document.getElementById("search_block").style.display = "none";
	document.getElementById("search").value="";
	document.getElementById("result").innerHTML = "";
	document.getElementById("edit_block").style.display = "block";
	
	sendRequest(editResponse,base_URL+'patient/'+id,'GET');
	
}
function editResponse(){	
	
	var obj = JSON.parse(this.responseText);
			
	form=document.createElement("form");
		
	for (var prop1 in obj) {
		if (typeof(obj[prop1])=='object'){
			for (var prop2 in obj[prop1]) {	
				var input = document.createElement("input");
				input.type = "text";
				input.name = prop1+'.'+prop2;
				input.value = obj[prop1][prop2];
				form.appendChild(input);
			}
		}else{
			var input = document.createElement("input");
			input.type = "text";
			input.name = prop1;
			input.value = obj[prop1];
			input.disabled = true;
			form.appendChild(input);
		}	
	}
	document.getElementById("edit_block").appendChild(form);
	
	updateButton=document.createElement("button");
	updateButton.onclick = function(){updateSend(obj["id"]);};
	updateButton.innerHTML="Update";
	document.getElementById("edit_block").appendChild(updateButton);
	
	deleteButton=document.createElement("button");
	deleteButton.onclick = function(){deleteSend(obj["id"]);};
	deleteButton.innerHTML="Delete";
	document.getElementById("edit_block").appendChild(deleteButton);
	
	returnButton=document.createElement("button");
	returnButton.onclick = function(){returnToSearch();};
	returnButton.innerHTML="Retour";
	document.getElementById("edit_block").appendChild(returnButton);

}

function newSend(){

	document.getElementById("search_block").style.display = "none";
	document.getElementById("search").value="";
	document.getElementById("result").innerHTML = "";
	document.getElementById("edit_block").style.display = "block";
	
	sendRequest(newResponse,base_URL+'patient/?limit=1','GET');
	
}
function newResponse(){	
	
	console.log(this.responseText);
	
	var obj = JSON.parse(this.responseText);
	obj=obj[0];

	form=document.createElement("form");
		
	for (var prop1 in obj) {
		if (typeof(obj[prop1])=='object'){
			for (var prop2 in obj[prop1]) {	
				var input = document.createElement("input");
				input.type = "text";
				input.name = prop1+'.'+prop2;
				input.value = "";
				form.appendChild(input);
			}
		}else{
			var input = document.createElement("input");
			input.type = "text";
			input.name = prop1;
			input.value = 0;
			input.disabled = true;
			form.appendChild(input);
		}	
	}
	
	document.getElementById("edit_block").appendChild(form);

	createButton=document.createElement("button");
	createButton.onclick = function(){createSend();};
	createButton.innerHTML="Creer";
	document.getElementById("edit_block").appendChild(createButton);
	
	returnButton=document.createElement("button");
	returnButton.onclick = function(){returnToSearch();};
	returnButton.innerHTML="Retour";
	document.getElementById("edit_block").appendChild(returnButton);

}
	
function showGraph(){
	document.getElementById("choise_block").style.display = "none";
	document.getElementById("graph_block").style.display = "block";
	loadGraphSend();
}
function showSearch(){
	
	document.getElementById("choise_block").style.display = "none";
	document.getElementById("search_block").style.display = "block";
	
	var inputSearch = document.createElement("input");
	inputSearch.id="search";
	inputSearch.placeholder="Nom de famille";
	inputSearch.onkeyup = function(){searchSend();};
	document.getElementById("search_block").appendChild(inputSearch);

	
	var resultDiv = document.createElement("div");
	resultDiv.id="result";
	document.getElementById("search_block").appendChild(resultDiv);
	
	var createPat = document.createElement("button");
	createPat.onclick = function(){newSend();};
	createPat.innerHTML="Creer";
	document.getElementById("search_block").appendChild(createPat);
	
	var retChoise = document.createElement("button");
	retChoise.onclick = function(){returnToChoise();};
	retChoise.innerHTML="Retour";
	document.getElementById("search_block").appendChild(retChoise);
}
function returnToSearch(){
	document.getElementById("edit_block").innerHTML="";
	document.getElementById("edit_block").style.display = "none";
	document.getElementById("search_block").style.display = "block";
}
function returnToChoise(){
	document.getElementById("search_block").innerHTML="";
	document.getElementById("search_block").style.display = "none";
	
	document.getElementById("graph_block").innerHTML="";
	document.getElementById("graph_block").style.display = "none";
	
	document.getElementById("choise_block").style.display = "block";
}

var abscisse="Age";
var ordonnee="BMI";
google.charts.load('current', {'packages':['corechart']});
	
function loadGraphSend(){
	sendRequest(loadGraphResponse,base_URL+'parametre/graph','GET');
}
function loadGraphResponse(){
	
	var retChoise = document.createElement("button");
	retChoise.onclick = function(){returnToChoise();};
	retChoise.innerHTML="Retour";
	document.getElementById("graph_block").appendChild(retChoise);
		
	var selectAbscisse = document.createElement("select");
	var obj = JSON.parse(this.responseText);
	console.log(obj);
	obj.forEach(function(param) {
		var opt = document.createElement('option');
		opt.value = param;
		opt.innerHTML = param;
		selectAbscisse.appendChild(opt);
	});
	
	var selectOrdonnee = selectAbscisse.cloneNode(true);
	
	selectAbscisse.onchange=function(){ abscisse=this.value;console.log("abscisse="+abscisse);drawChartSend();	 };
	selectOrdonnee.onchange=function(){ ordonnee=this.value;console.log("ordonnee="+ordonnee);drawChartSend();	 };

	document.getElementById("graph_block").appendChild(selectAbscisse);
	document.getElementById("graph_block").appendChild(selectOrdonnee);
	
	var chart_div = document.createElement("div");
	chart_div.id="chart_div";
	chart_div.style.width="70%";
	chart_div.style.height="70%";
	document.getElementById("graph_block").appendChild(chart_div);
	
	drawChartSend();				

}

function drawChartSend() {
	sendRequest(drawChartResponse,base_URL+'parametre/scatter_chart/'+abscisse+'/'+ordonnee,'GET');	
}
function drawChartResponse() {
  	
	var obj = JSON.parse(this.responseText);
	
	var data = google.visualization.arrayToDataTable(obj, true);
	var options = { title: 'Comparaison des risques', hAxis: {title: abscisse},vAxis: {title: ordonnee}};

	//ScatterChart
	//LineChart
	var chart = new google.visualization.ScatterChart(document.getElementById('chart_div'));
	chart.draw(data, options);

}