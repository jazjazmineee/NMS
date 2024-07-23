<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="log-in.aspx.cs" Inherits="NMS.pages.log_in" %>

<!DOCTYPE html>
<html>
    <head>
        <meta charset="utf-8">
        <meta http-equiv="X-UA-Compatible" content="IE=edge">
        <title>The Bellevue Manila</title>
        <link rel="icon" type="image/x-icon" href="../assets/img/bellevue-hotel.png" />
        <meta name="description" content="">
        <meta name="viewport" content="width=device-width, initial-scale=1">
        <link rel="stylesheet" href="../assets/styles/login.css">
        <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/boxicons/2.1.0/css/boxicons.min.css">
        
    </head>
    <body onload="noBack();" onpageshow="if (event.persisted) noBack();" onunload="">
        <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
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
                <img src="../assets/img/logo-tbm.svg" alt="Logo" class="logo">
                <h1>Welcome back!</h1>
                <div class="login-text">
                    <p>Login to your account</p>
                </div>
                <!--  ----- -->  
                <div class="input-group">
                    <div class="input-field">
                        <i class='bx bxs-user'></i>
                        <asp:TextBox ID="uname" runat="server" CssClass="input" placeholder="Username"></asp:TextBox>
                    </div>
                    <div class="input-field">
                        <i class='bx bxs-lock-alt'></i>
                        <asp:TextBox ID="pword" runat="server" CssClass="input" placeholder="Password" TextMode="Password"></asp:TextBox>
                        <i class='bx bxs-hide toggle-password' onclick="togglePasswordVisibility()"></i>
                     <!-- Error label container -->
                    <div class="auto-style1">
                        <asp:Label ID="errorLabel" runat="server" ForeColor="Red" CssClass="auto-style2"></asp:Label>
                    </div>
                  </div>
                  <!-- forget container -->   
                    <p>
                        <asp:LinkButton ID="forgotbtn" runat="server" Text="Forgot Password" OnClick="forgotbtn_Click" />
                    </p>
                </div>

                
                    <div class="btn-field">
                        <asp:Button ID="loginbtn" CssClass="styled-button" runat="server" Text="LOGIN" OnClientClick="return validateFields()" OnClick="loginbtn_Click" />
                    </div>
                </div>

            </div>
        </div>
    </form>
       
         <!-- SCRIPT-->   
        <script type="text/javascript">
            window.history.forward();
            function noBack() {
                window.history.forward();
            }
        </script>
        <script>

            function togglePasswordVisibility() {
                var passwordInput = document.getElementById('<%= pword.ClientID %>');
                var eyeIcon = passwordInput.nextElementSibling;

                if (passwordInput.type === 'password') {
                    passwordInput.type = 'text';
                    eyeIcon.classList.remove('bxs-hide');
                    eyeIcon.classList.add('bxs-show');
                } else {
                    passwordInput.type = 'password';
                    eyeIcon.classList.remove('bxs-show');
                    eyeIcon.classList.add('bxs-hide');
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
                var password = document.getElementById('<%= pword.ClientID %>').value;
                var errorLabel = document.getElementById('<%= errorLabel.ClientID %>');

                if (!password) { // Example condition for error
                    errorLabel.style.display = 'block';
                    errorLabel.innerText = 'Username and Password is required.';
                    return false; // Prevent form submission
                } else {
                    errorLabel.style.display = 'none';
                    return true; // Allow form submission
                }
            }

        </script>
       
    </body>
</html>
