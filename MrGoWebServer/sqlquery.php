<?php
$query = $_POST["query"];
$con=mysqli_connect("localhost","root","1234","ujek_db");
if (mysqli_connect_errno())
  {
  echo "Failed to connect to MySQL: " . mysqli_connect_error();
  }
$result=mysqli_query($con,$query);
while($row = mysqli_fetch_array($result,MYSQLI_NUM))
echo "<BR>$row[0];$row[1];$row[2];$row[3];$row[4];$row[5];$row[6];$row[7];$row[8];$row[9];$row[10];$row[11];$row[12];$row[13];$row[14];$row[15]";
mysqli_close($con);
?>