<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="create-newpassword.aspx.cs" Inherits="NMS.pages.create_newpassword" %>

<!DOCTYPE html>
<html>
    <head>
        <meta charset="utf-8">
        <meta http-equiv="X-UA-Compatible" content="IE=edge">
        <title>The Bellevue Manila</title>
        <link rel="icon" type="image/x-icon" href="../assets/img/bellevue-hotel.png" />
        <meta name="description" content="">
        <meta name="viewport" content="width=device-width, initial-scale=1">
        <link rel="stylesheet" href="../assets/styles/change-password.css">
        <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/boxicons/2.1.0/css/boxicons.min.css">
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
                    <asp:LinkButton ID="backButton" CssClass="back-button" runat="server" OnClick="backButton_Click" Font-Underline="False">
                        <i class='bx bx-arrow-back'></i> Back
                    </asp:LinkButton>                  
                    <img src="../assets/img/logo-tbm.svg" alt="Logo" class="logo">
                    <h1>Create new password</h1>
                    <div class="login-text">
                        <p>Your new password must be different</p>
                        <p>from previously used password</p>
                    </div>
                    <asp:TextBox ID="uname" placeholder="Username" runat="server" Enabled="False" Visible="False"></asp:TextBox>
        
                    <div class="input-field">
                        <i class='bx bxs-lock-alt'></i>
                        <asp:TextBox ID="pword" placeholder="Password" TextMode="Password" runat="server"></asp:TextBox>

                    </div>
                    <div class="input-field">
                        <i class='bx bxs-lock-alt'></i>
                        <asp:TextBox ID="confirmpword" runat="server" placeholder="Confirm Password" TextMode="Password"></asp:TextBox>
                    </div>
                    <!-- Error label container -->
                    <div class="auto-style1">
                        <asp:Label ID="errorLabel" runat="server" ForeColor="Red" CssClass="auto-style2"></asp:Label>
                    </div>
                    
                    <div class="btn-field">
                        <asp:Button ID="resetbtn" CssClass="styled-button" runat="server" Text="Reset Password" OnClick="resetbtn_Click" />
                    </div>
                </div>
            </div>
        </div>
        </form>
        <!-- SCRIPT-->   
        <script>
            function togglePasswordVisibility(passwordFieldId, iconId) {
                let passwordField = document.getElementById(passwordFieldId);
                let icon = document.getElementById(iconId);

                if (passwordField.type === "password") {
                    passwordField.type = "text";
                    icon.className = "bx bxs-show";
                } else {
                    passwordField.type = "password";
                    icon.className = "bx bxs-hide";
                }
            }
            function hideErrorLabel() {
                var errorLabel = document.getElementById('<%= errorLabel.ClientID %>');
                if (errorLabel) {
                    errorLabel.style.display = 'none';
                }
            }

            function showErrorLabel(message) {
                var errorLabel = document.getElementById('<%= errorLabel.ClientID %>');
                if (errorLabel) {
                    errorLabel.innerText = message;
                    errorLabel.style.display = 'block';
                    setTimeout(hideErrorLabel, 5000); // Hide after 5 seconds
                }
            }
        </script>
    </body>
</html>
