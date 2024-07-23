<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="load.aspx.cs" Inherits="NMS.pages.load" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>The Bellevue Manila</title>
    <link rel="icon" type="image/x-icon" href="../assets/img/bellevue-hotel.png" />
    <link rel="stylesheet" href="../assets/styles/load.css">
    <link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Playfair+Display:wght@400;700&family=Montserrat&display=swap">
    <script type="text/javascript">
    function getQueryParam(param) {
        var urlParams = new URLSearchParams(window.location.search);
        return urlParams.get(param);
    }

    function redirectToTarget() {
        var target = getQueryParam('target');
        if (target) {
            setTimeout(function() {
                window.location.href = target;
            }, 2000); // 2-second delay before redirecting
        }
    }

    window.onload = redirectToTarget;
    </script>
</head>
<body>
    <div class="wrapper">
        <header class="header">
            <!-- Your header content here -->
        </header>
        <ul class="loader-list">
            <li>
                <div class="loader-5 center"><span></span></div>
            </li>
            <div class="text">
                <p>THE BELLEVUE MANILA</p>
            </div>
            <div class="always">
                <p>Always at Home in</p>
                <p>World-Class Hospitality</p>
            </div>
        </ul>
    </div>
</body>
</html>
