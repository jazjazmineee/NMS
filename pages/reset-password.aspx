<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="reset-password.aspx.cs" Inherits="NMS.pages.reset_password" %>

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
                        <asp:LinkButton ID="backButton" CssClass="back-button" runat="server" OnClick="backButton_Click" Font-Underline="False">
                            <i class='bx bx-arrow-back'></i> Back
                        </asp:LinkButton>
                        <img src="../assets/img/logo-tbm.svg" alt="Logo" class="logo">
                        <h1>Reset Password</h1>
                        <div class="login-text">
                            <p>Don't worry, we'll assist you in resetting your password</p>
                            <p>Enter the password associated with your account.</p>
                        </div>
                        <div class="input-group">
                            <div class="input-field">
                                <i class='bx bxs-user'></i>
                                <asp:TextBox ID="uname" placeholder="Username" runat="server"></asp:TextBox>
                            </div>
                             <!-- Error label container -->
                            <div class="auto-style1">
                                <asp:Label ID="errorLabel" runat="server" ForeColor="Red" CssClass="auto-style2"></asp:Label>
                            </div>
                        </div>
                        <div class="btn-field">
                            <asp:Button ID="verifybtn" runat="server" CssClass="styled-button" Text="VERIFY" OnClick="verifybtn_Click" ButtonType="Button" />
                        </div>

                    </div>
                </div>
            </div>
        </form>

        <!-- SCRIPT-->   
        <script>
            let toggle = document.getElementById("btnToggle");
            let password = document.getElementById("password");
            let icon = toggle.querySelector("i");

            toggle.onclick = function () {
                if (password.type == "password") {
                    password.type = "text";
                    icon.className = "bx bxs-show";
                } else {
                    password.type = "password";
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

            function validateFields() {
                var username = document.getElementById('<%= uname.ClientID %>').value;
                var errorLabel = document.getElementById('<%= errorLabel.ClientID %>');

                if (!username) { // Example condition for error
                    errorLabel.style.display = 'block';
                    errorLabel.innerText = 'Username is required.';
                    return false; // Prevent form submission
                } else {
                    errorLabel.style.display = 'none';
                    return true; // Allow form submission
                }
            }
        </script>
    </body>
</html>
