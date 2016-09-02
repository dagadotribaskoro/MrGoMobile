<?php
$email = $_POST["email"];
$pwd =  $_POST["password"];
require "init.php";

$query = "select * from user_table where email = '".$email."' and password = '".$pwd."';";
//$query = "select * from user_table";
$result = mysql_query($query,$con);
//echo "query : ".$query;
//echo "count : ".mysql_num_rows($result);
//while($row = mysql_fetch_array($result))
//echo "<br> $row[0] $row[1] $row[2] $row[3] $row[4]";
if(mysql_num_rows($result)>0)
{
	$response = array();
	$code = "login_true";
	$row = mysql_fetch_array($result);
	$name = $row[1];
	$message = "Login Success...Welcome ".$name;
	array_push($response,array("code"=>$code,"message"=>$message));
	echo json_encode(array("server_response"=>$response));
}
else
{
	$response = array();
	$code = "login_false";	
	$message = "Login Failed...Try again...";
	array_push($response,array("code"=>$code,"message"=>$message));
	echo json_encode(array("server_response"=>$response));
}
mysql_close($con);
?>