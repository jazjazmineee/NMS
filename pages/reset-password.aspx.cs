using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NMS.pages
{
    public partial class reset_password : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        private string GetConnectionString()
        {
            string server = "localhost";
            string database = "networkmonitoring";
            string username = "root";
            string password = "";
            return $"Server={server};Database={database};Uid={username};Pwd={password};";
        }
        protected void verifybtn_Click(object sender, EventArgs e)
        {
            string username = uname.Text;

            // Your connection string
            string connectionString = GetConnectionString();

            // MySQL query to check if the username exists
            string query = "SELECT COUNT(1) FROM account WHERE username = @username";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    connection.Open();

                    // Execute the MySQL command
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int count = reader.GetInt32(0); // Get the value of the first column (COUNT(1))

                            if (count > 0)
                            {
                                // Redirect to ResetPassword.aspx if username exists
                                Response.Redirect("create-newpassword.aspx?username=" + username);
                            }
                            else
                            {
                                // Username does not exist, show an alert
                                errorLabel.Text = "Username not found.";
                                errorLabel.Visible = true;
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "showErrorLabel", "showErrorLabel();", true);

                                uname.Text = "";
                            }
                        }
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