# MarkSpecs
MarkSpecs is a Markdown framework to write Software Requirements Specifications.

When you write Software Requirements, you need to provide diagrams, schematics, tables, datagrams, and many other kinds of data and provide a solution to generate a document from multiple Markdown files. MarkSpecs provides an environment where users can express their needs by using one or several ASCII files.  
This way, you can use Git (or any version control tool) for version control; you don't need specific tools to create your schematics nor generate your documentation... What you need is a text editor (could be Notepad, Visual Studio, etc.) and MarkSpecs, that's all!  
At the time being, Markspecs only provides HTML output. I hope to be able to implement PDF generated document soon.  

MarkSpecs is build around [MarkDig](https://github.com/lunet-io/markdig) and takes credit of the extension capabilities to introduce [Mocodo](https://github.com/laowantong/mocodo), [PlantUml](https://plantuml.com/) and many others dependencies. As Markdig is [CommonMark](https://commonmark.org/) compliant, MarkSpecs is also CommonMark compliant.

# Installation from sources
MarkSpecs is a c# project. The sources include all the dependencies, so you should be able to build MarkSpecs without troubles by opening MarkSpecs project in Visual Studio 2019.

# Basic Installation
The package (not yet created) will contain the windows executable and the associated dependencies. You should be able to run MarkSpecs as a standalone application without troubles.  
You will be able to update the dependencies independently of MarkSpecs core.

# Usage
If you have a single MarkDown file, do
```
  markspecs.exe html [path_to_md_file]
```
to generate the HTML file in the same directory than the original markdown file.

If you want to build an HTML file from several Markdown files, replace the markdown file path by the path to the directory where the markdown files are set.  
```
markspecs.exe html [path_to_dir]
```
Markspecs will then retrieve all the markdown files, set them in alphabetic order, and build the HTML output files accordingly.

# Creating diagrams
MarkSpecs introduces Diagrams by specific code blocks (initiated by three quotes `). MarkSpecs only generates SVG diagrams.

## Supported diagrams
|Diagram Type|Description|Sources|Notes
|---|---|---|--|
|Mocodo|Merise Database schematics|https://github.com/laowantong/mocodo|
|PlantUml|Multiple diagrams, mostly UML|https://plantuml.com/ | Request to launch MarkSpecs PlantUml server|
|NwDiag|Network Diagrams|http://blockdiag.com/en/nwdiag/introduction.html#setup| Also included in PlantUml|
|PackDiag|Packet header diagram images|http://blockdiag.com/en/nwdiag/index.html| Provided by NwDiag|
|RackDiag|Rack-structure diagram images|http://blockdiag.com/en/nwdiag/index.html| Provided by NwDiag|
|Railroad-Diagram|Railroad diagram (visual illustration of the grammar used for programming languages)https://github.com/tabatkins/railroad-diagrams|
|Vega|Declarative language for interactive visualization designs|https://vega.github.io/vega/|
|VegaLite|Data visualizations |https://vega.github.io/vega-lite/|
|WaveDrom|Digital Timing Diagram|https://wavedrom.com/|PlantUml also supports timing diagram|
|Mermaid| Multiple diagrams, mostly UML | https://mermaid-js.github.io/mermaid/#/| Generated thanks Javascript references in the header of the final file|
|nomnoml|UML diagrams |https://github.com/skanaar/nomnoml| Generated thanks Javascript references in the header of the final file|
|GoJS|FaultTree generator| https://github.com/NorthwoodsSoftware/GoJS| Not Free Licence. Javascript generated.
|Schemdraw|Electrical Circuit|https://schemdraw.readthedocs.io/en/latest/index.html|
## About PlantUml
PlantUml can provide several kinds of diagrams. If you want to generate charts by PlantUml in MarkSpecs, you have to launch PlantUml FTP servers by running the command   
```
markspecs.exe plantumlserver
```
before.  
The command will launch several instances of the PlantUml servers (number of instances is defined in the application config file, or by the command line interface). Using several instances of PlantUml can save generation time, especially if you have multiple diagrams defined in several markdown files. Numerous instances give room for parallelization, and generation time can be, under some conditions, divided by two or more.

## About Mermaid and nomnoml
Mermaid and nomnoml diagrams are generated thanks Javascript libraries defined in an HTML header file provided to MarkSpecs by the <-h|--header> parameter.

# More information about the modules
## Mocodo
### Aim
### Installation and Configuration
### Syntax
### Licence
## Plantuml syntax
### Aim
### Installation and Configuration
### Syntax
### Licence
## NwDiag syntax
### Aim
### Installation and Configuration
### Syntax
### Licence
## PackDiag syntax
### Aim
### Installation and Configuration
### Syntax
### Licence
## RackDiag syntax
### Aim
### Installation and Configuration
### Syntax
### Licence
## Railroad-Diagram
### Generalities
RailRoad-Diagram is used to generate RailRoad Diagrams
<img src="https://github.com/tabatkins/railroad-diagrams/raw/gh-pages/images/rr-title.svg?sanitize=true" width="2000" style="max-width:100%;">
Railroad diagrams are mostly used to define software language grammar, as you can see on https://www.json.org/json-en.html.But they may be used for other purpose.  
The diagrams are generated in SVG format by a Python script created from scracth by Markspecs. The script will refer to the Railroad-diagram Python library (available https://github.com/tabatkins/railroad-diagrams). The path to the library is set in the configuration file.
### Prerequisite
Python > 3.8 (tested configuration, should be OK with any version > 3.1).
### Installation and Configuration
By default, MarkSpecs is provided with the RailRoad-diagrams library provided in the vendor/railroad directory. 
To update the RailRoad-Diagrams library, your are free to replace the content of the directory by a new version of the library, or you can set the new version in a directory of your choice and update the MarkSpecs configuration file.
### Syntax
Refer to https://github.com/tabatkins/railroad-diagrams to know the syntax.  
In the markdown file, you don't have to provide the Diagram object. You only need to provide the list of diagam items, as in the exemple below:
<pre>
  ```railroad
   Optional('+', 'skip'),
  Choice(0,
    NonTerminal('name-start char'),
    NonTerminal('escape')),
  ZeroOrMore(
    Choice(0,
      NonTerminal('name char'),
      NonTerminal('escape')))
  ```
</pre>
This exemple will lead MarkSpecs to generate the Python Script
```python
import sys
import importlib.util
spec = importlib.util.spec_from_file_location("railroad", "C:/Temp/railroad-diagrams-gh-pages/railroad.py")
module = importlib.util.module_from_spec(spec)
sys.modules[spec.name] = module 
spec.loader.exec_module(module)
        
from railroad import *
d = Diagram(
  Optional('+', 'skip'),
  Choice(0,
    NonTerminal('name-start char'),
    NonTerminal('escape')),
  ZeroOrMore(
    Choice(0,
      NonTerminal('name char'),
      NonTerminal('escape'))))
d.writeSvg(sys.stdout.write)
```
  

### Licence
[CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/)  
This document and all associated files in the github project https://github.com/tabatkins/railroad-diagrams are licensed under CC0. This means you can reuse, remix, or otherwise appropriate RailRoad-Diagrams project for your own use without restriction.
### SVG data generation
The SVG data are generated through a Python script created on the flag by MarkSpecs. One of the difficulties is to refer to the RailRoad-Diagrams library by providing an absolute path (Python management of the libraries, modules,... has some specificities). Maybe a more efficient solution exists...

A typical python file created on the flag, may look:
```python
import sys
import importlib.util
spec = importlib.util.spec_from_file_location("railroad", "C:/Specific_Path/.../railroad/railroad.py")
module = importlib.util.module_from_spec(spec)
sys.modules[spec.name] = module 
spec.loader.exec_module(module)
        
from railroad import *
d = Diagram(
  Optional('+', 'skip'),
  Choice(0,
    NonTerminal('name-start char'),
    NonTerminal('escape')),
  ZeroOrMore(
    Choice(0,
      NonTerminal('name char'),
      NonTerminal('escape'))))
d.writeSvg(sys.stdout.write)
```
