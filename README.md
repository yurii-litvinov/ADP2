# Automatic Diploma Processor 2

A tool for processing various kinds of quailification works for Software Engineering chair of St. Petersburg State University. 
It collects available information from google sheets and files with reports and reviews, then prepares data for uploading to 
[SE chair site](https://se.math.spbu.ru/theses.html). Advanced version of
https://github.com/yurii-litvinov/adp. It supports bachelor qualification works, 3rd and 2nd course semester works.

## Usage

* Build this project using .NET (>= 6.0) using `dotnet publish -c Release -r win-x64 -p:PublishReadyToRun=true -o release` (if you not on Windows, replace win-x64 by [some other RID](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog)).
* Add `release` folder to your `PATH` variable, for example, `set PATH=C:\Users\yurii\source\repos\ADP2\release;%PATH%`.
* Collect your qualification work files in one folder, make sure that they are named as 
  `<student name>-<report|slides|review|advisor-review|reviewer-review>.pdf`
  For example, `Ololoev-advisor-review.pdf`.
* Run `adp2` in that folder.
* `_config.json` file shall appear, edit it:
  * WorkType: 2 for practice, 3 for bachelor's qualification work, 4 for master's qualification work;
  * Course: course of students in a folder (we assume they are from the same course);
  * Year: year of defence (now we publish only spring works);
  * GoogleSheetId: hash of a Google Sheet with info about works, for example, "1b1fhGFInVDNXAb_Ok14Nl03V-DviKe-GrE2Geuwsw9o". You can get it from URL of an opened sheet in Google Docs.
  * SecretKey is an API access token. Get it from People That Know It.
* Run `adp2` again. Google OAuth may ask you to grant permission to read your Google Sheets. Accept it. Two files shall appear: `_out.json` and `_upload.py`.
* Check `_out.json` manually.
* To actually upload works, run `python _upload.py`.
* See https://se.math.spbu.ru/theses_tmp.html for results of the uploading.


