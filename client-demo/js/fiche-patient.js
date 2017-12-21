
var base_URL = 'http://localhost/api/';
var parms;

// Modifie l'URL de base
function changeBaseURL(){
	base_URL='http://'+document.getElementById("base_url").value+'/api/';
	document.getElementById("url_label").innerHTML=base_URL;
}

// Fct envoyant les requêtes à l'API
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
    if (this.status==200 || this.status==204){
		document.getElementById("info_block").innerHTML="";
		this.callback.apply(this, this.arguments); 
	}else{
		document.getElementById("info_block").innerHTML=this.responseText;
		document.getElementById("info_block").style.backgroundColor="#FF0000";
	}
}
function xhrError() { 
	console.log(this.statusText+' '+this.responseText); 
}
	
// Recherche de patient
function searchSend(){
	sendRequest(searchResponse,base_URL+'patient?search='+document.getElementById("search").value+'&limit=5','GET');
}
function searchResponse(){
	var obj = JSON.parse(this.responseText);
	document.getElementById("result").innerHTML="";
	obj.forEach(function(patient) {
				var div = document.createElement("div");
				div.className += "formu";
				var label = document.createElement("label");
				label.innerHTML = patient.admin.nom+" "+patient.admin.prenom;
				div.appendChild(label);
				var buttonEdit = document.createElement("button");
				buttonEdit.onclick=function(){editSend(patient.id);};
				buttonEdit.innerHTML="Editer";
				div.appendChild(buttonEdit);
				document.getElementById("result").appendChild(div);
		});
}

// Suppression d'un patient
function deleteSend(id){
	sendRequest(deleteResponse,base_URL+'patient/'+id,'DELETE');
}
function deleteResponse(){
	returnToSearch()
	document.getElementById("info_block").innerHTML="Suppression OK";
	document.getElementById("info_block").style.backgroundColor="#00FF00";
}

// Mise à jour d'un patient
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

	sendRequest(updateResponse,base_URL+'patient/'+id,'PUT',formdata);
}
function updateResponse(){
	returnToSearch()
	document.getElementById("info_block").innerHTML="Mise à jour OK";
	document.getElementById("info_block").style.backgroundColor="#00FF00";
}
	
// Création d'un patient	
function createSend(){
		
	var form = document.getElementsByTagName("form");
	var inputs = form[0].getElementsByTagName("input");

	// L'objet patient d'origine est reconstruit sur base du formulaire
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

	sendRequest(createResponse,base_URL+'patient/','POST',formdata);
}
function createResponse(){
	returnToSearch()
	document.getElementById("info_block").innerHTML="Creation OK";
	document.getElementById("info_block").style.backgroundColor="#00FF00";
}

// Crée le formulaire d'édition		
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
		
	// Le formulaire est créé à la volée sur base de la structure de l'objet patient	
	for (var prop1 in obj) {
		if (typeof(obj[prop1])=='object'){
			for (var prop2 in obj[prop1]) {
				var div = document.createElement("div");
				div.className += "formu";
				var label = document.createElement("label");
				label.for = prop1+'.'+prop2;
				label.innerHTML = prop2;
				div.appendChild(label);				
				var input = document.createElement("input");
				input.type = "text";
				input.id = prop1+'.'+prop2;
				input.name = prop1+'.'+prop2;
				input.value = obj[prop1][prop2];
				div.appendChild(input);
				form.appendChild(div);
			}
		}else{
			var div = document.createElement("div");
			div.className += "formu";
			var label = document.createElement("label");
			label.for = prop1;
			label.innerHTML = prop1;
			div.appendChild(label);
			var input = document.createElement("input");
			input.type = "text";
			input.id = prop1;
			input.name = prop1;
			input.value = obj[prop1];
			input.disabled = true;
			div.appendChild(input);
			form.appendChild(div);
		}	
	}
	infoSend()
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

// Mise à jour des titres en utilisant la fonction info de patient
function infoSend(){
	sendRequest(infoResponse,base_URL+'patient/info','GET');
}
function infoResponse(){

	var infos = JSON.parse(this.responseText);
	var form = document.getElementsByTagName("form");
	var inputs = form[0].getElementsByTagName("input");
	var labels = form[0].getElementsByTagName("label");
	
	for(var i=0; i< labels.length; i++){
		for(var j=0; j< infos.length; j++){
			if (labels[i].for.indexOf(".")>-1){
				if(infos[j].parent+"."+infos[j].nom==labels[i].for){
					labels[i].innerHTML=infos[j].titre;
				}
			}else{
				if(infos[j].parent+"."+infos[j].nom==labels[i].for){
					labels[i].innerHTML=infos[j].titre;
				}
			}
		}
	}
	
}

// Crée le formulaire pour la création d'un patient
function newSend(){

	document.getElementById("search_block").style.display = "none";
	document.getElementById("search").value="";
	document.getElementById("result").innerHTML = "";
	document.getElementById("edit_block").style.display = "block";
	
	sendRequest(newResponse,base_URL+'patient/?limit=1','GET');
	
}
function newResponse(){	
	
	var obj = JSON.parse(this.responseText);
	obj=obj[0];

	// Le formulaire est créé à la volée sur base de la structure de l'objet patient
	form=document.createElement("form");
	for (var prop1 in obj) {
		if (typeof(obj[prop1])=='object'){
			for (var prop2 in obj[prop1]) {	
				var div = document.createElement("div");
				div.className += "formu";
				var label = document.createElement("label");
				label.for = prop1+'.'+prop2;
				label.innerHTML = prop2;
				div.appendChild(label);				
				var input = document.createElement("input");
				input.type = "text";
				input.id = prop1+'.'+prop2;
				input.name = prop1+'.'+prop2;
				input.placeholder = obj[prop1][prop2];
				input.value = "";
				div.appendChild(input);
				form.appendChild(div);
			}
		}else{
			var div = document.createElement("div");
			div.className += "formu";
			var label = document.createElement("label");
			label.for = prop1;
			label.innerHTML = prop1;
			div.appendChild(label);
			var input = document.createElement("input");
			input.type = "text";
			input.id = prop1;
			input.name = prop1;
			input.value = 0;
			input.disabled = true;
			div.appendChild(input);
			form.appendChild(div);
		}	
	}		
	infoSend()
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
	
// Fonctions de navigation
function showGraph(){
	document.getElementById("info_block").innerHTML="";
	document.getElementById("choice_block").style.display = "none";
	document.getElementById("graph_block").style.display = "block";
	loadGraphSend();
}
function showSearch(){
	
	document.getElementById("info_block").innerHTML="";
	document.getElementById("choice_block").style.display = "none";
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
	
	var retchoice = document.createElement("button");
	retchoice.onclick = function(){returnTochoice();};
	retchoice.innerHTML="Retour";
	document.getElementById("search_block").appendChild(retchoice);
}
function returnToSearch(){
	document.getElementById("info_block").innerHTML="";
	document.getElementById("edit_block").innerHTML="";
	document.getElementById("edit_block").style.display = "none";
	document.getElementById("search_block").style.display = "block";
}
function returnTochoice(){
		
	document.getElementById("info_block").innerHTML="";
	document.getElementById("search_block").innerHTML="";
	document.getElementById("search_block").style.display = "none";
	
	document.getElementById("graph_block").innerHTML="";
	document.getElementById("graph_block").style.display = "none";
	
	document.getElementById("choice_block").style.display = "block";
}

var abscisse="age";
var ordonnee="bmi";
var graphtype="scatter_chart";
google.charts.load('current', {'packages':['corechart']});
	
// Crée la page des graphiques sur base de la fonction info de parametre
function loadGraphSend(){
	sendRequest(loadGraphResponse,base_URL+'parametre/info','GET');
}
function loadGraphResponse(){
	
	var retchoice = document.createElement("button");
	retchoice.onclick = function(){returnTochoice();};
	retchoice.innerHTML="Retour";
	document.getElementById("graph_block").appendChild(retchoice);
		
	
	parms = JSON.parse(this.responseText);

	var selectAbscisse = document.createElement("select");
	parms.forEach(function(param) {
		var opt = document.createElement('option');
		opt.value = param.nom;
		opt.innerHTML = param.titre;
		if(abscisse==param.nom){
			opt.selected=true;
		}
		selectAbscisse.appendChild(opt);
	});
	selectAbscisse.onchange=function(){ abscisse=this.value;drawChartSend();};
	document.getElementById("graph_block").appendChild(selectAbscisse);
	
	var selectOrdonnee = document.createElement("select");
	parms.forEach(function(param) {
		var opt = document.createElement('option');
		opt.value = param.nom;
		opt.innerHTML = param.titre;
		if(ordonnee==param.nom){
			opt.selected=true;
		}
		selectOrdonnee.appendChild(opt);
	});
	selectOrdonnee.onchange=function(){ ordonnee=this.value;drawChartSend();};
	document.getElementById("graph_block").appendChild(selectOrdonnee);
	
	var selectGraphType = document.createElement("select");

	var optScatterChart = document.createElement('option');
	optScatterChart.value = "scatter_chart";
	optScatterChart.innerHTML = "ScatterChart";
	selectGraphType.appendChild(optScatterChart);
	
	var optLineChart = document.createElement('option');
	optLineChart.value = "line_chart";
	optLineChart.innerHTML = "LineChart";
	selectGraphType.appendChild(optLineChart);
	
	selectGraphType.onchange=function(){ graphtype=this.value;drawChartSend();	 };
	
	document.getElementById("graph_block").appendChild(selectGraphType);
			
	var chart_div = document.createElement("div");
	chart_div.id="chart_div";
	chart_div.style.width="90%";
	chart_div.style.height="calc( 85% - 160px ) ";
	document.getElementById("graph_block").appendChild(chart_div);
	
	drawChartSend();				

}

// Création du graphique avec Google Chart utilisant les fonctions graphique de paramètre
function drawChartSend() {
	sendRequest(drawChartResponse,base_URL+'parametre/'+graphtype+'/'+abscisse+'/'+ordonnee,'GET');	
}
function drawChartResponse() {
  	
	var obj = JSON.parse(this.responseText);
	
	var abscisseTitre = "";
	var abscisseMin = "";
	var abscisseMax = "";
	var ordonneeTitre = "";
	var ordonneeMin = "";
	var ordonneeMax = "";
	
	parms.forEach(function(param) {

		if(param.nom==abscisse){
					
			abscisseTitre=param.titre+" ( "+param.unite+" ) ";
			abscisseMin=param.min;
			abscisseMax=param.max;
		}
		if(param.nom==ordonnee){
					
			ordonneeTitre=param.titre+" ( "+param.unite+" ) ";
			ordonneeMin=param.min;
			ordonneeMax=param.max;
		}

	});
	
	var data = google.visualization.arrayToDataTable(obj, true);
	var options = { title: 'Comparaison des risques', 		hAxis: {title: abscisseTitre,
															viewWindowMode: 'explicit', viewWindow: { min : abscisseMin, max : abscisseMax }},
															vAxis: {title: ordonneeTitre, 
															viewWindowMode: 'explicit', viewWindow: { min : ordonneeMin, max : ordonneeMax }}};

	if(graphtype=="line_chart"){
		var chart = new google.visualization.LineChart(document.getElementById('chart_div'));
	}else{
		var chart = new google.visualization.ScatterChart(document.getElementById('chart_div'));
	}
	chart.draw(data, options);

}