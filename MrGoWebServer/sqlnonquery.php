<?php
$query = $_POST["query"];
require "init.php";
$result = mysqli_query($con,$query);
mysqli_close($con);
?>	