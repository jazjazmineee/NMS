<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="no-connection.aspx.cs" Inherits="NMS.pages.no_connection" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <title>The Bellevue Manila</title>
    <link rel="icon" type="image/x-icon" href="../assets/img/bellevue-hotel.png" />
    <meta name="description" content="">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="stylesheet" href="../assets/styles/no-con.css">
    <link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Playfair+Display:wght@400;700&family=Montserrat&display=swap">
    <script>
        function checkInternetConnection() {
            var img = new Image();
            img.src = "https://www.google.com/images/phd/px.gif?" + new Date().getTime();
            img.onload = function () {
                window.history.back();
            };
        }

        // Check for internet connection every 5 seconds
        setInterval(checkInternetConnection, 5000);
    </script>
</head>
<body>
    <div class="wrapper">
        <div class="wifi-symbol">
            <div class="wifi-circle first"></div>
            <div class="wifi-circle second"></div>
            <div class="wifi-circle third"></div>  
            <div class="wifi-circle fourth"></div>
        </div>

        <div class="text">
            <p>NO INTERNET</p>
        </div>

        <div class="always">
            <p>Something wrong with your connection,</p>
            <p>please check and try again.</p>
        </div>
    </div>
</body>
</html>