# Automatic Diploma Processor 2

[![GitHub Actions CI](https://github.com/yurii-litvinov/ADP2/actions/workflows/ci.yml/badge.svg)](https://github.com/yurii-litvinov/ADP2/actions/workflows/ci.yml)

A tool for processing various kinds of quailification works for Software Engineering chair of St. Petersburg State University. 
It collects available information from google sheets and files with reports and reviews, then prepares data for uploading to 
[SE chair site](https://se.math.spbu.ru/theses.html). Advanced version of
https://github.com/yurii-litvinov/adp. It supports bachelor qualification works, 3rd and 2nd course semester works.

## Usage

* Add NuGet source with DocUtils package:
  `dotnet nuget add source --username <user-name> --password <password> --store-password-in-clear-text --name github "https://nuget.pkg.github.com/yurii-litvinov/index.json"`
  User name and password you can obtain from People That Know It. Password is actually an access token for GitHub NuGet repository.
* Build this project using .NET (>= 6.0) using `dotnet publish -c Release -o release`.
* Add `release` folder to your `PATH` variable, for example, `set PATH=C:\Users\yurii\source\repos\ADP2\release;%PATH%`.
* Collect your qualification work files in one folder, make sure that they are named as 
  `<student name>-<report|slides|review|advisor-review|reviewer-review>.pdf`
  For example, `Ololoev-advisor-review.pdf`.
  If there is more than one student with the same surname, use dot and name to make them unique: `Ololoev.Ivan-advisor-review.pdf`.
* Run `adp2` in that folder.
* `_config.json` file shall appear, edit it:
  * WorkType: 
    * 2 - Bachelor_Report, whatever that means
    * 3 - bachelor's thesis
    * 4 - master's thesis
    * 5 - 2nd course autumn practice
    * 6 - 2nd course spring practice
    * 7 - 3rd course autumn practice
    * 8 - 3rd course spring practice
  * Programme: educational programme of students in a folder (we assume they are from the same program):
    * 1 - Software and Administration of Information Systems (bachelors)
    * 2 - Software engineering (bachelors)
    * 3 - Software and Administration of Information Systems (masters)
    * 4 - Fundamental informatics and information technologies
    * 5 - Information technologies
    * 6 - Group 344 (bachelors) (actually it is SAIS, but whatever)
    * 7 - Software engineering (masters)
  * Year: year of defence (now we publish only spring works, so it is unambiguous);
  * GoogleSheetId: hash of a Google Sheet with info about works, for example, "1b1fhGFInVDNXAb_Ok14Nl03V-DviKe-GrE2Geuwsw9o". 
    You can get it from URL of an opened sheet in Google Docs.
  * SecretKey is an API access token. Get it from People That Know It.
  * A section with metadata descriptios:
    * SheetName - name of a sheet in a spreadsheet. Probably "ÑÏ" or "Ëèñò1".
    * AuthorNameColumn - name of a column where student name and surname is stored. Most probably "ÔÈÎ".
    * AdvisorColumn - name of a column with scientific advisor name.
    * TitleColumn - name of a column with title of the work.
    * SourceUriColumn - name of a column with a link to practical implementation. "-" if not present.
    * CommitterNameColumn - name of a column with commiter name in a repository from SourceUriColumn. Ignored if SourceUriColumn is not present.
    * ResultColumn - name of a column with a final grade or "passed"/"not passed" mark. Probably "Çà÷¸ò".
* Run `adp2` again. Google OAuth may ask you to grant permission to read your Google Sheets. Accept it. Two files shall appear: `_out.json` and `_upload.py`.
* Errors will be reported if present. Note works with missing metainformation and works with missing files --- it is very likely that
  the problem is with transliteration and file naming. Fix those problems and run `adp2` again, until no more errors are reported.
  It can be that some students did not finish their works, so some errors about works with missing files are possible. 
  Check Google Sheets and if this is the case, just ignore errors.
* Check `_out.json` manually.
* To actually upload works, run `python _upload.py`.
* See https://se.math.spbu.ru/theses_tmp.html for results of the uploading.


