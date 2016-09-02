<?php

$host = "localhost";
$user="root";
$password="1234?";
$dbname ="ujek_db";

$con = mysqli_connect($host,$user,$password);
//if(!$con)
//{
//	die("Error in Database Connection".mysqli_connect_error());
//}
//else
//{
mysqli_select_db($con,$dbname);
//	echo "Database Connection Success";
//}
?>