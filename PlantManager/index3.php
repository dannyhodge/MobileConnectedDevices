<?php
use \Psr\Http\Message\ServerRequestInterface as Request;
use \Psr\Http\Message\ResponseInterface as Response;
  
require 'vendor/autoload.php';
  
# Database config variables (for your MySQL database)
  
$config['displayErrorDetails'] = true;
$config['db']['host']   = "localhost";
$config['db']['user']   = "root";
$config['db']['pass']   = "P76ahQknRkt3";
$config['db']['dbname'] = "sensor_data";
  
// bind the database settings to the app (your service) configuration
$app = new \Slim\App(["settings" => $config]);
$container = $app->getContainer();
  
# Database container function, you will simply call this as 'db' in your REST methods
  
$container['db'] = function ($c) {
    $db = $c['settings']['db'];
    $pdo = new PDO("mysql:host=" . $db['host'] . ";dbname=" . $db['dbname'],
        $db['user'], $db['pass']);
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
    $pdo->setAttribute(PDO::ATTR_DEFAULT_FETCH_MODE, PDO::FETCH_ASSOC);
    return $pdo;
};




// below is a GET endpoint URI that accepts a parameter called name
$app->get('/hello/{name}', function (Request $request, Response $response) {
    $name = $request->getAttribute('name');
   $data = array('name' => $name, 'age' => 40);
   $newResponse = $response->withJson($data);
    return $newResponse;
});
  
  
  
  # GET endpoint to return all data for selected sensor in the sensor table
$app->get('/sensors/getsensordata', function (Request $request, Response $response) {
  
   // Get the sensorid as passed in the calling URL
   // Actually gets all parameters that are specified in the URL
   $allGetVars = $request->getQueryParams();
   foreach($allGetVars as $key => $param){
   $sensor_id = $allGetVars['sensorid'];
}
     
   // Create database query to select all sensor data for the selected sensor
   // Use our db connection from app configuration
   $db = $this->db;
  
   // SQL SELECT query to select and return data from specified sensorid
   $stmt = $db->prepare('SELECT time_stamp,sensor_id, sensor_type, value from sensors where sensor_id = :sensorid');
   // Parameterise query for security puposes
   $stmt->bindValue(':sensorid', $sensor_id);
   // Run query
   $stmt->execute();
   // Save query today using PDO object
   $sensordata = $stmt->fetchAll(PDO::FETCH_OBJ);
   // Close database connection
   $db = null;
   // return sensor data for given sensorid to calling client as JSON data
   $newResponse = $response->withHeader('Content-type', 'application/json');
   $newResponse = $response->withHeader('Access-Control-Allow-Origin', '*');
   $newResponse = $response->withJson($sensordata);
   return $newResponse;
});
  
  
  //GET IMAGES
  
   $app->get('/images/getimagedata', function (Request $request, Response $response) {
	 
	

  $sth = $this->db->prepare("SELECT image FROM images");
    $sth->execute();
    $apps = $sth->fetchAll();
        $result = array();
        foreach ($apps as $app) {
            $app['image'] = base64_encode($app['image']);
            array_push($result, $app);
        }
    return $this->response->withJson($result);
   });
  
  
  
   // POST IMAGES
    $app->post('/images/postimagedata', function ($request, $response) {
    $input = $request->getParsedBody();
	
	
    $input['data'] = json_encode($input['data']);
	$input['data'] = base64_decode($input['data']);
	
   // $sql = "INSERT INTO images (id,image) VALUES (:id,:image)";
	$randomString = "Hello";
	$sql = "INSERT INTO images (id,image) VALUES (:id,:image)";
	
    $sth = $this->db->prepare($sql);
    $sth->bindParam("id", $input['id']);
    $sth->bindParam("image",$input['data']);
    $sth->execute();
    $input['id'] = $this->db->lastInsertId();
    return $this->response->withJson($input);
});
 



/*   # POST endpoint to post specific sensor data to the sensors database table
	$app->post('/images/postimagedata', function (Request $request, Response $response) { 
    // Get the sensorid, sensortype, and sensor value from parameters in the URL from calling client
    $allGetVars = $request->getQueryParams();
    foreach($allGetVars as $key => $param){
    $id = $allGetVars['id'];
    $image = $allGetVars['image'];
}
   
   return "hello";
   // Create database query to insert row of sensor data
   // Use our db connection from app configuration
   $db = $this->db;
   // // SQL INSERT statement query to save data to sensors table
   $stmt = $db->prepare('INSERT into images (id, image) VALUES (:id, :image)');
   // Parameterise query for security puposes
   $stmt->bindValue(':id', $id);
   $stmt->bindValue(':image', $image);
   $stmt->execute();
   //Get id (auto incremented) of  newly inserted row of sensor data
   $lastid = $db->lastInsertId();
   $db = null;
   // Return id of inserted row to calling client in JSON
   $newResponse = $response->withHeader('Content-type', 'application/json');
   $newResponse = $response->withJson($lastid);
   return $newResponse;
	}); */
	
	
	
  
  # POST endpoint to post specific sensor data to the sensors database table
	$app->post('/sensors/postsensordata', function (Request $request, Response $response) { 
    // Get the sensorid, sensortype, and sensor value from parameters in the URL from calling client
    $allGetVars = $request->getQueryParams();
    foreach($allGetVars as $key => $param){
    $sensor_id = $allGetVars['sensorid'];
    $sensor_type = $allGetVars['sensortype'];
    $sensor_value = $allGetVars['value'];
}

   // Create database query to insert row of sensor data
   // Use our db connection from app configuration
   $db = $this->db;
   // // SQL INSERT statement query to save data to sensors table
   $stmt = $db->prepare('INSERT into sensors (sensor_id, sensor_type, value) VALUES (:sensorid, :sensortype, :sensorvalue)');
   // Parameterise query for security puposes
   $stmt->bindValue(':sensorid', $sensor_id);
   $stmt->bindValue(':sensortype', $sensor_type);
   $stmt->bindValue(':sensorvalue', $sensor_value);
   $stmt->execute();
   //Get id (auto incremented) of  newly inserted row of sensor data
   $lastid = $db->lastInsertId();
   $db = null;
   // Return id of inserted row to calling client in JSON
   $newResponse = $response->withHeader('Content-type', 'application/json');
   $newResponse = $response->withJson($lastid);
   return $newResponse;
	});
	
	
  
  
$app->run();
  
?>