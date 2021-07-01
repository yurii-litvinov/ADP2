# Automatic Diploma Processor 2

A tool for processing various kinds of quailification works for Software Engineering chair of St. Petersburg State University. 
It collects available information from google sheets and files with reports and reviews, then prepares data for uploading to 
[SE chair site](https://se.math.spbu.ru/theses.html). Advanced version of
https://github.com/yurii-litvinov/adp. It supports bachelor qualification works, 3rd and 2nd course semester works.

## Usage

* Build this project using .NET (>= 6.0) using `dotnet publish -c Release -r win10-x64 -o release`.
* Add `release` folder to your `PATH` variable, for example, `set PATH=C:\Users\yurii\source\repos\ADP2\release;%PATH%`.
* Collect your qualification work files in one folder, make sure that they are named as 
  `<student name>-<report|slides|review|advisor-review|reviewer-review>.pdf`
  For example, `Ololoev-advisor-review.pdf`.
* Run `adp2.cli` in that folder.

