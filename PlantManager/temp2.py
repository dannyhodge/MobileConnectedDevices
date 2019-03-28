import requests
from HIH6130.io import HIH6130
import time

while True:
    # Code executed here
    
    rht = HIH6130()
    rht.read()

    print("Temp: {0}".format(rht.t))
    print("Humidity: {0}".format(rht.rh))

#r = requests.get(url='http://35.204.36.81/index2.php/sensors/getsensordata?sensorid=1')

    r = requests.post(url='http://35.204.36.81/index3.php/sensors/postsensordata?sensorid=1&sensortype=temp&value={0}'.format(rht.t))
    time.sleep(1)
