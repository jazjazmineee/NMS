using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NMS.pages
{
    public partial class create_newpassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Retrieve the username from the query parameters
            string username = Request.QueryString["username"];

            // Populate the username TextBox with the retrieved username
            uname.Text = username;
        }

        private string GetConnectionString()
        {
            string server = "localhost";
            string database = "networkmonitoring";
            string username = "root";
            string password = "";
            return $"Server={server};Database={database};Uid={username};Pwd={password};";
        }

        private MySqlConnection GetConnection()
        {
            MySqlConnection connection = new MySqlConnection(GetConnectionString());
            return connection;
        }
        protected void resetbtn_Click(object sender, EventArgs e)
        {
            string username = uname.Text.Trim();
            string newPassword = pword.Text.Trim();
            string confirmPassword = confirmpword.Text.Trim();

            // Validate that new password and confirm password are not empty
            if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                // Show an error message if either new password or confirm password is empty
                errorLabel.Text = "New password and confirm password are required.";
                errorLabel.Visible = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showErrorLabel", "showErrorLabel();", true);
                return;
            }

            if (newPassword != confirmPassword)
            {
                // Passwords do not match, show an error message
                errorLabel.Text = "Passwords do not match";
                errorLabel.Visible = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showErrorLabel", "showErrorLabel();", true);
                return;
            }

            // Check if new password is the same as the old password
            using (MySqlConnection connection = GetConnection())
            {
                string query = "SELECT Password FROM account WHERE Username = @Username;";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    connection.Open();
                    string oldPassword = command.ExecuteScalar()?.ToString();

                    if (newPassword == oldPassword)
                    {
                        // New password is the same as the old password, show an error message
                        errorLabel.Text = "New password must be different from old password.";
                        errorLabel.Visible = true;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "showErrorLabel", "showErrorLabel();", true);
                        return;
                    }
                }
            }

            // Update the password in the database
            using (MySqlConnection connection = GetConnection())
            {
                string query = "UPDATE account SET Password = @NewPassword WHERE Username = @Username;";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@NewPassword", newPassword);
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Password reset successful
                        Response.Redirect("sucess-flash.aspx");
                    }
                    else
                    {
                        // Username not found, show an error message
                        errorLabel.Text = "Username not found.";
                        errorLabel.Visible = true;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "showErrorLabel", "showErrorLabel();", true);
                    }
                }
            }
        }

        protected void backButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("log-in.aspx");
        }
    }
}