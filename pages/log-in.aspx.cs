using MySql.Data.MySqlClient;
using System;
using System.Web;
using System.Web.UI;

namespace NMS.pages
{
    public partial class log_in : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ValidationSettings.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;

                // Prevent browser caching
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();
                Response.Cache.SetExpires(DateTime.MinValue);
            }
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
        protected void loginbtn_Click(object sender, EventArgs e)
        {
            string Username = uname.Text.Trim();
            string Password = pword.Text.Trim();

            using (MySqlConnection connection = GetConnection())
            {
                string query = "SELECT * FROM account WHERE BINARY Username = @Username AND BINARY Password = @Password;";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", Username);
                    command.Parameters.AddWithValue("@Password", Password);
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Login successful
                            // Set session variable
                            Session["Username"] = Username;

                            // Redirect to another page
                            Response.Redirect("load.aspx?target=dashboard-admin.aspx");
                        }
                        else
                        {
                            // Login failed
                            // Show an error message
                            errorLabel.Text = "Invalid username or password.";
                            errorLabel.Visible = true;
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "showErrorLabel", "showErrorLabel();", true);
                        }
                    }
                }
            }
        }


        protected void forgotbtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("load.aspx?target=reset-password.aspx");
        }
    }
}
