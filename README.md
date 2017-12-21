# API de comparaison des risques

Outil de comparaison des risques encourus par des patients sur base des paramètres médicaux,
biométriques et d'assuétudes.

Le premier objectif de cette API est de gérer un ensemble de « fiche patient », cette fiche étant
composée d’un certain nombre de paramètres. Il s’agira donc de pouvoir créer, lire, écrire ou
supprimer un objet « patient » représentant cette fiche d’informations. (Gestion des données
de type CRUD)

Le second objectif est de mettre à disposition un ensemble de paramètres afin de pouvoir
établir des corrélations entre eux. L’ensemble de paramètres est constitué d’une part des
paramètres renseignés dans la fiche patient et d’autre part de paramètres calculés à partir de
ceux-ci.

L’API se charge de la synchronisation entre les fiches patient et des données mises à
disposition ainsi que du calcul des paramètres manquant. ( Un patient souhaitant être
supprimé de la base de données verra ses données également supprimées des statistiques)

En plus de fournir les données brutes, l’API possède des fonctions permettant de facilement
comparer deux paramètres choisis sous forme de graphique. Les données sont optimisées pour
être utilisées avec Google Chart mais peuvent facilement s’adapter à n’importe quel outil
générant des graphiques.

## Divers

.NET Core 2.0

Visual Studio 2017

Template Web API 

Dans le répertoire "ComparaisonRisques" le projet en lui même

Dans le répertoire "ComparaisonRisquesTests" le projet des tests unitaires

## Installation

Via Docker Hub : sepulwarrior/comparaisonrisques

Tags disponibles : prototype-windows, prototype-linux

Documentation sur l'installation disponible dans le répertoire "documentation"

## Client de démo 

Un client de démonstaration développé en HTML/Javascript utilisant Google Charts pour les graphiques

Disponible dans le répertoire "client-demo"

## Autheur

* **Frédérik Liénard**
