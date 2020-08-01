# Mocodo
Mocodo helps to define relational databases by using the Merise methodology (mostly a French approach).  
From a MCD (Modèle conceptuel de données), it can generate an entity-relation diagram in SVG output and relation diagram (MLD).
An online version is available: http://www.mocodo.net/.  
Help is available on https://rawgit.com/laowantong/mocodo/master/doc/fr_refman.html.  
Mocodo project is available on https://github.com/laowantong/mocodo.  

At the time being, only the image_format="svg" is supported. 

Here below, the result of the exemple from the Readme file.
<pre>
```mocodo {colors=ocean image_format="svg" shapes=copperplate relations="html_verbose" }
DF, 11 Élève, 1N Classe
Classe: Num. classe, Num. salle
Faire Cours, 1N Classe, 1N Prof: Vol. horaire
Catégorie: Code catégorie, Nom catégorie

Élève: Num. élève, Nom élève
Noter, 1N Élève, 0N Prof, 0N Matière, 1N Date: Note
Prof: Num. prof, Nom prof
Relever, 0N Catégorie, 11 Prof

Date: Date
Matière: Libellé matière
Enseigner, 11 Prof, 1N Matière
```
</pre>
```mocodo {colors=ocean image_format="svg" shapes=copperplate relations="html_verbose" }
DF, 11 Élève, 1N Classe
Classe: Num. classe, Num. salle
Faire Cours, 1N Classe, 1N Prof: Vol. horaire
Catégorie: Code catégorie, Nom catégorie

Élève: Num. élève, Nom élève
Noter, 1N Élève, 0N Prof, 0N Matière, 1N Date: Note
Prof: Num. prof, Nom prof
Relever, 0N Catégorie, 11 Prof

Date: Date
Matière: Libellé matière
Enseigner, 11 Prof, 1N Matière
```
