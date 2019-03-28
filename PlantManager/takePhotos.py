# CamJam EduKit 3 - Robotics
# Wii controller remote control script


import time # Import the Time library
import numpy
import cv2
import threading
import datetime
import os
import base64
import requests
from PIL import Image
from urllib.parse import urlencode
from urllib.request import Request, urlopen
import requests
from HIH6130.io import HIH6130
import time


count = 0
cap = cv2.VideoCapture(0)

def takePhoto():
  threading.Timer(10.0, takePhoto).start()
  now = datetime.datetime.now()
  ret, frame = cap.read()

      
  rht = HIH6130()
  rht.read()

  print("Temp: {0}".format(rht.t))
  print("Humidity: {0}".format(rht.rh))

  r = requests.post(url='http://35.204.36.81/index3.php/sensors/postsensordata?sensorid=1&sensortype=temp&value={0}'.format(rht.t))
    
	
  timestampStr = now.strftime("%d%b%Y(%H%M%S%f)")
  timeStr = str(timestampStr)
  path = os.path.dirname(os.path.abspath(__file__))
  fullPath = path +  '\\images\\frame' + timeStr + '.jpg'
  print(fullPath)
  cv2.imwrite(fullPath, frame)  
  cv2.imshow('img',frame)
  encoded_string = ""
  with open(fullPath, "rb") as image_file:
    imageRead = image_file.read()
    encoded_string = base64.b64encode(imageRead).decode('ascii')
    
    
  headers = {'Content-type': 'application/json', 'Accept': 'text/plain'}
  
  
  r = requests.post('http://35.204.36.81/index3.php/images/postimagedata', json={'data':encoded_string})
  print(r.request.headers['Content-Type'])
  print(r.request.url)
  print(r.request.body)
  
  json_response = r.json()
  print(json_response['data'])
  print(r)

  cv2.waitKey(0)

takePhoto()

cap.release()
cv2.destroyAllWindows()







