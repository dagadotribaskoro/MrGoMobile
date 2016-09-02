<?php
$query = $_POST["query"];
require "init.php";
//$query = "select * from user_table";
$result = mysql_query($query,$con);
//echo "query : ".$query;
//echo "count : ".mysql_num_rows($result);
//while($row = mysql_fetch_array($result))
//echo "<br> $row[0] $row[1] $row[2] $row[3] $row[4]";
if(mysql_num_rows($result)>0)
{
	while($row = mysql_fetch_array($result))
	echo "<BR>$row[0];$row[1];$row[2];$row[3];$row[4];$row[5];$row[6];$row[7];$row[8];$row[9];$row[10];$row[11];$row[12];$row[13];$row[14];$row[15]";
	//echo json_encode(array("server_response"=>$response));
}
else
{
	$response = array();
	$code = "login_false";	
	$message = "Login Failed...Try again...";
	array_push($response,array("code"=>$code,"message"=>$message));
	//echo json_encode(array("server_response"=>$response));
}
mysql_close($con);
?>