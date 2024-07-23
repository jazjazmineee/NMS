<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="sucess-flash.aspx.cs" Inherits="NMS.pages.sucess_flash" %>

<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <title>The Bellevue Manila</title>
    <link rel="icon" type="image/x-icon" href="../assets/img/bellevue-hotel.png" />
    <meta name="description" content="">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="stylesheet" href="../assets/styles/sucess-flash.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/boxicons/2.1.0/css/boxicons.min.css">
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="filter">
        <div class="bg-container">
            <div class="text-form">
                <div class="welcome">
                    <h1>WELCOME TO THE BELLEVUE MANILA</h1>
                </div>
                <div class="world">
                    <h1>Always at Home in</h1>
                    <h1>World-Class Hospitality</h1>
                </div>
            </div>

            <div class="login-form">
                <!-- WHITE FIELD  -->
                                 
                 <img src="../assets/img/logo-tbm.svg" alt="Logo" class="logo">
                 <!-- BUTTON -->              
                <div class="btn-field">
                    <asp:Button ID="contbtn" CssClass="styled-button" runat="server" Text="CONTINUE"  OnClick="contbtn_Click" />
                </div>

           
            
                <!-- SUCESS MESG -->             
                 <div class="success-checkmark">
                    <div class="check-icon">
                        <span class="icon-line line-tip"></span>
                        <span class="icon-line line-long"></span>
                        <div class="icon-circle"></div>
                        <div class="icon-fix"></div>
                    </div>
                </div>

                <div class="mesage">
                    <h1>Password Reset!</h1>
                    <p class ="your-password">Your Password has been successfully reset,</p>
                    <p class ="click-below">click below to continue your access</p>             
                </div>
            </div>
            
        </div>
    </div>
    </form>
    <!-- SCRIPT-->   
    <script>
        $(document).ready(function () {
            $(".check-icon").hide();
            setTimeout(function () {
                $(".check-icon").show();
            }, 10);
        });
    </script>
</body>
</html>
