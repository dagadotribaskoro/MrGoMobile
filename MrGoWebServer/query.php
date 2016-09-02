<?php
$con=mysqli_connect("localhost","root","1234","ujek_db");
// Check connection
if (mysqli_connect_errno())
  {
  echo "Failed to connect to MySQL: " . mysqli_connect_error();
  }

// Perform queries 
$result=mysqli_query($con,"SELECT * FROM member");

// Numeric array
$row=mysqli_fetch_array($result,MYSQLI_NUM);
printf ("%s (%s)\n",$row[0],$row[1]);

// Associative array
$row=mysqli_fetch_array($result,MYSQLI_ASSOC);
printf ("%s (%s)\n",$row["member_email"],$row["member_name"]);

// Free result set
mysqli_free_result($result);

mysqli_close($con);

mysqli_close($con);
?>