from flask import Flask
from flask import request
app = Flask(__name__)

################## GPIO Definitions

def export(pin):
    try:
        print("Exporting GPIO{}".format(pin))
        export_fs = open("/sys/class/gpio/export", "w")
        export_fs.write(str(pin))
        export_fs.close()
    except OSError:
        print("GPIO{} pin already exported".format(pin))
        return -1

def unexport(pin):
    try:
        print("Unexporting GPIO{}".format(pin))
        export_fs = open("/sys/class/gpio/unexport", "w")
        export_fs.write(str(pin))
        export_fs.close()
    except OSError:
        print("GPIO{} pin not exported".format(pin))
        return -1

def setDir(pin, newDir):
    if newDir not in ["out", "in"]:
        print("{} is not a valid pin direction".format(newDir))
        exit(-1)

    try:
        print("Setting GPIO{} direction to {}".format(pin, newDir))
        dir_fs = open("/sys/class/gpio/gpio{}/direction".format(pin), "w")
        dir_fs.write(newDir)
        dir_fs.close()
    except OSError:
        print("GPIO{} pin already {}".format(pin, newDir))
        return -1

def setPin(pin, val):
    if val not in ["1", "0"]:
        print("{} is not a valid pin value".format(val))
        return -1;

    try:
        val_fs = open("/sys/class/gpio/gpio{}/value".format(pin), "w")
        val_fs.write(val)
        val_fs.close()
    except OSError:
        print("Couldn't change pin state of GPIO{}".format(pin))
        return -1

def readPin(pin):
    try:
        val_fs = open("/sys/class/gpio/gpio{}/value".format(pin), "r")
        val = val_fs.read()
        val_fs.close()
        return val
    except OSError:
        print("Couldn't read pin state of GPIO{}".format(pin))
        return -1

################## Routing

@app.route("/led/<int:pin>", methods=['POST', 'GET'])
def setLed(pin):
    if request.method == 'POST':
        content = request.get_json(True)

        if "state" not in content:
            return "State json field not specified"

        state = content['state']

        if setPin(pin, str(state)) is -1:
            return "Invalid state: {}".format(state)
        return "New state: {}".format(state)

    if request.method == 'GET':
        val = readPin(pin)
        if val is -1:
            return "GPIO{} not available".format(pin)
        return str(val)

@app.route("/export/<int:pin>")
def exp(pin):
    if export(pin) == -1:
        return "Can't export GPIO{}".format(pin)
    return "ok"

@app.route("/unexport/<int:pin>")
def unexp(pin):
    if unexport(pin) == -1:
        return "Can't unexport GPIO{}".format(pin)
    return "ok"

@app.route("/dir/<int:pin>/<dir>")
def dir(pin, dir):
    if setDir(pin, dir) == -1:
        return "Can't set GPIO{} to {}".format(pin, dir)
    return "ok"
    

################## Main loop

if __name__ == "__main__":
    app.run(host='0.0.0.0', port=80, debug=True)
