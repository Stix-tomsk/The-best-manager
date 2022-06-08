import os
import json
import random as rnd
from hashlib import sha256
from flask import Flask, send_from_directory, render_template, request

access_tokens = []
admin_ip = "192.168.100.10"

app = Flask(
    __name__,
    template_folder=os.path.abspath('template'),
    static_url_path=''
    )

@app.route('/auth/e=<string:email>&p=<string:pswd>')
def auth(email, pswd):
    with open("template/authData.json", "r") as f:
        data = json.load(f)
    
    userId = -1
    
    for key, value in data.items():
        if key == "Email":
            for i in range(len(value)):
                if email == value[i]:
                    userId = i
                    break
            else:
                return "-3,--"
        elif key == "Password":
            if value[userId] != pswd:
                return "-1,--"

    token = sha256((email+str(rnd.randint(100000000, 999999999))).encode('utf-8')).hexdigest()
    access_tokens.append(token)
    print(access_tokens)

    usersList = ["User1", "User2", "User3", "User4"]
    ip = sha256((request.remote_addr).encode('utf-8')).hexdigest()
    with open("template/data.json", "r") as f:
        data = json.load(f)
    for key, value in data.items():
        if key == usersList[userId]:
            for k, v in value.items():
                if k == "ip":
                    v.append(ip)
                    break
            break
    
    with open("template/data.json", "w") as f:
        json.dump(data, f)

    return f"{userId},{token}"

@app.route('/autoAuth')
def autoAuth():
    with open("template/data.json", "r") as f:
        data = json.load(f)
    
    ip = sha256((request.remote_addr).encode('utf-8')).hexdigest()
    result = -1
    keys = []
    values = []
    for key, value in data.items():
        keys.append(key)
        values.append(value)
    for i in range(2, 6):
        for key, value in values[i].items():
            if key == "ip":
                for curIp in value:
                    if curIp == ip:
                        token = sha256((values[0][0]+str(rnd.randint(100000000, 999999999))).encode('utf-8')).hexdigest()
                        access_tokens.append(token)
                        result = f"{i-2},{token}"
                        print(f"logged in user index: {i-2}")
    
    print(access_tokens)
    return f'{result}'

@app.route('/get_data/t=<string:token>')
def postData(token):
    if request.remote_addr == admin_ip:
        return send_from_directory("template","data.json")
    for tk in access_tokens:
        if tk == token:
            return send_from_directory("template","data.json")
    return "Denied."

@app.route('/edit_data/id=<int:userId>&t=<string:type>&v=<int:value>')
def editData(userId, type, value):
    if request.remote_addr == admin_ip:
        with open("template/data.json", "r") as f:
            data = json.load(f)
        
        usersList = ["User1", "User2", "User3", "User4"]
        if type == "score":
            for key, val in data.items():
                if key == "Score":
                    val[userId] += value
                    continue
                if key == usersList[userId]:
                    for k, v in val.items():
                        if k == "score":
                            v.append(value)
                            break
                    break
                    
        elif type == "achievements":
            for key, val in data.items():
                if key == usersList[userId]:
                    for k, v in value.items():
                        if k == "achievements":
                            v += value
                            break
                    break
        with open("template/data.json", "w") as f:
            json.dump(data, f)
   
    return "Denied."

@app.route('/end_session/t=<string:token>')
def removeToken(token):
    for tk in access_tokens:
        if tk == token:
            access_tokens.remove(token)
            print(access_tokens)
            return "Success"
    return "Denied."

@app.route('/', methods=['GET', 'POST'])
def index():
    if request.method == 'GET':
        return "Server is running."
    return "I have nothing to do"

if __name__ == "__main__":
    app.run(host='0.0.0.0', port=80, debug=False)
