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
|Syntrax|Railroad diagram (visual illustration of the grammar used for programming languages)|https://kevinpt.github.io/syntrax/|
|Vega|Declarative language for interactive visualization designs|https://vega.github.io/vega/|
|VegaLite|Data visualizations |https://vega.github.io/vega-lite/|
|WaveDrom|Digital Timing Diagram|https://wavedrom.com/|PlantUml also supports timing diagram|
|Mermaid| Multiple diagrams, mostly UML | https://mermaid-js.github.io/mermaid/#/| Generated thanks Javascript references in the header of the final file|
|nomnoml|UML diagrams |https://github.com/skanaar/nomnoml| Generated thanks Javascript references in the header of the final file|
## About PlantUml
PlantUml can provide several kinds of diagrams. If you want to generate charts by PlantUml in MarkSpecs, you have to launch PlantUml FTP servers by running the command   
```
markspecs.exe plantumlserver
```
before.  
The command will launch several instances of the PlantUml servers (number of instances is defined in the application config file, or by the command line interface). Using several instances of PlantUml can save generation time, especially if you have multiple diagrams defined in several markdown files. Numerous instances give room for parallelization, and generation time can be, under some conditions, divided by two or more.

## About Mermaid and nomnoml
Mermaid and nomnoml diagrams are generated thanks Javascript libraries defined in an HTML header file provided to MarkSpecs by the <-h|--header> parameter.


