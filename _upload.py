import json
import requests
import sys
import io

def add_document(files, work, document_name):
    if document_name in work:
        files.append((document_name, (work[document_name], open(work[document_name], 'rb'), 'application/octet')))

url = "https://se.math.spbu.ru/post_theses"

file_name = '_out.json'

with io.open(file_name, encoding='utf-8') as json_file:
    data = json.load(json_file)

for work in data:
    print("Sending " + work["thesis_info"]["name_ru"])
 
    files = []
    add_document(files, work, "thesis_text")
    add_document(files, work, "reviewer_review")
    add_document(files, work, "presentation")
    add_document(files, work, "supervisor_review")

    files.append(('thesis_info', ('thesis_info', json.dumps(work["thesis_info"]), 'application/json')))

    r = requests.post(url, files=files, allow_redirects=False)
    print(str(r.content, 'utf-8'))
