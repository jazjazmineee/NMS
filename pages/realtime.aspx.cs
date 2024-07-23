using Irony;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Common;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace NMS.pages
{
    public partial class realtime : BasePage
    {
        private bool isUserAction = true; // Flag to track if the change is from user action

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializeDefaultValues();
                SetToggleButtonState();
                SetRefreshInterval();
                SetDropdownSelectedValues();
                BindGridView();
            }

            ClearErrorLabel();
        }

        private void InitializeDefaultValues()
        {
            tbInterval.Text = "00:00:00";

            if (Session["IntervalValue"] != null)
            {
                hfInterval.Value = Session["IntervalValue"].ToString();
                IntervalLabel.Text = $"Global {hfInterval.Value}-Seconds Interval";
            }
            else
            {
                hfInterval.Value = GetIntervalFromDatabase().ToString();  // Retrieve from database
                IntervalLabel.Text = $"Global {hfInterval.Value}-Seconds Interval";
            }
        }

        private void SetToggleButtonState()
        {
            if (Session["ToggleButtonState"] != null)
            {
                ToggleButton.Checked = (bool)Session["ToggleButtonState"];
            }
            else
            {
                // Default state on first load
                ToggleButton.Checked = true;
                Session["ToggleButtonState"] = true;
            }
        }

        private void SetRefreshInterval()
        {
            if (ToggleButton.Checked)
            {
                int globalInterval = GetIntervalFromDatabase();
                hfInterval.Value = globalInterval.ToString();
                IntervalLabel.Text = $"Global {hfInterval.Value}-Seconds Interval";
            }
            else
            {
                IntervalLabel.Text = "Priority-Based Intervals";
                hfInterval.Value = "Off";
            }
        }

        private void SetDropdownSelectedValues()
        {
            // Check for the status query parameter and set the DropDownList2 value
            string statusQuery = Request.QueryString["status"];
            if (!string.IsNullOrEmpty(statusQuery))
            {
                ListItem statusItem = DropDownList2.Items.FindByValue(statusQuery);
                if (statusItem != null)
                {
                    DropDownList2.ClearSelection();
                    statusItem.Selected = true;
                }
            }

            // Check for the tablename query parameter and set the DropDownList1 value
            string tablenameQuery = Request.QueryString["tablename"];
            if (!string.IsNullOrEmpty(tablenameQuery))
            {
                ListItem tablenameItem = DropDownList1.Items.FindByValue(tablenameQuery);
                if (tablenameItem != null)
                {
                    DropDownList1.ClearSelection();
                    tablenameItem.Selected = true;
                }
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

        protected void ToggleButton_CheckedChanged(object sender, EventArgs e)
        {
            ClearErrorLabel();

            // Store the toggle button state in the session
            Session["ToggleButtonState"] = ToggleButton.Checked;

            if (ToggleButton.Checked)
            {
                int globalInterval = GetIntervalFromDatabase();
                IntervalLabel.Text = $"Global {globalInterval}-Seconds Interval";
                hfInterval.Value = globalInterval.ToString();
                Session["IntervalValue"] = globalInterval; // Update session with global interval
            }
            else
            {
                IntervalLabel.Text = "Priority-Based Intervals";
                hfInterval.Value = "Off";
            }

            // Clear the notification session variable when toggling
            Session["NotificationShown"] = null;
        }

        private int GetIntervalFromDatabase()
        {
            int interval = 30; // Default interval
            string connectionString = GetConnectionString();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT IntervalInSeconds FROM Settings WHERE Id = 1", conn);
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    interval = Convert.ToInt32(result);
                }
            }

            return interval;
        }

        protected void btnCalculate_Click(object sender, EventArgs e)
        {
            // Clear any existing error messages
            ClearErrorLabel();

            string durationInput = tbInterval.Text.Trim();
            Regex regex = new Regex(@"^(\d{2}):(\d{2}):(\d{2})$");
            Match match = regex.Match(durationInput);

            if (match.Success)
            {
                int hours = int.Parse(match.Groups[1].Value);
                int minutes = int.Parse(match.Groups[2].Value);
                int seconds = int.Parse(match.Groups[3].Value);

                if (hours == 0 && minutes == 0 && seconds == 0)
                {
                    ShowError("Interval value cannot be set to 0.");
                    return;
                }
                ClearErrorLabel();
                int totalSeconds = (hours * 3600) + (minutes * 60) + seconds;

                hfInterval.Value = totalSeconds.ToString();
                ToggleButton.Checked = true;
                IntervalLabel.Text = $"Global {totalSeconds}-Seconds Interval";

                Session["IntervalValue"] = totalSeconds;

                // Update interval in database
                UpdateIntervalInDatabase(totalSeconds);

                // Call functions to update interval in respective tables
                UpdateDeviceTableInterval(totalSeconds, "seconds");
                UpdateIPAddressTableInterval(totalSeconds, "seconds");
                UpdateWebAddressTableInterval(totalSeconds, "seconds");

                ShowNotification("Interval value has been set successfully.");
                Session["NotificationShown"] = true;
            }
            else
            {
                ShowError("Interval value format should be (HH:MM:SS).");
            }

            isUserAction = false;
            tbInterval.Text = "00:00:00";
            isUserAction = true;
        }

        private void UpdateIntervalInDatabase(int interval)
        {
            string connectionString = GetConnectionString();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("UPDATE Settings SET IntervalInSeconds = @interval WHERE Id = 1", conn);
                cmd.Parameters.AddWithValue("@interval", interval);
                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateDeviceTableInterval(int interval, string intervalUnit)
        {
            string connectionString = GetConnectionString();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("UPDATE Device SET RefreshInterval = @interval, IntervalUnit = @intervalUnit", conn);
                cmd.Parameters.AddWithValue("@interval", interval);
                cmd.Parameters.AddWithValue("@intervalUnit", intervalUnit);
                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateIPAddressTableInterval(int interval, string intervalUnit)
        {
            string connectionString = GetConnectionString();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("UPDATE IPAddress SET RefreshInterval = @interval, IntervalUnit = @intervalUnit", conn);
                cmd.Parameters.AddWithValue("@interval", interval);
                cmd.Parameters.AddWithValue("@intervalUnit", intervalUnit);
                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateWebAddressTableInterval(int interval, string intervalUnit)
        {
            string connectionString = GetConnectionString();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("UPDATE WebAddress SET RefreshInterval = @interval, IntervalUnit = @intervalUnit", conn);
                cmd.Parameters.AddWithValue("@interval", interval);
                cmd.Parameters.AddWithValue("@intervalUnit", intervalUnit);
                cmd.ExecuteNonQuery();
            }
        }

        private void ShowNotification(string message)
        {
            // Check if the notification has already been shown in the current session
            if (Session["NotificationShown"] == null)
            {
                string script = $"showToast('{message}');";
                ScriptManager.RegisterStartupScript(this, GetType(), "showToastScript", script, true);
            }
        }

        private void ShowError(string errorMessage)
        {
            ClearErrorLabel(); // Clear any existing errors first
            errorLabel.Text = errorMessage;
            errorLabel.Visible = true;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "showErrorLabel", $"showErrorLabel('{errorMessage}');", true);
        }

        protected void tbInterval_TextChanged(object sender, EventArgs e)
        {
            ClearErrorLabel();
            if (isUserAction && string.IsNullOrEmpty(tbInterval.Text.Trim()))
            {
                // Only set default if the text has actually changed
                if (tbInterval.Text != "00:00:00")
                {
                    tbInterval.Text = "00:00:00"; // Set default value
                }
            }
        }

        private void ClearErrorLabel()
        {
            errorLabel.Text = string.Empty;
            errorLabel.Visible = false;
        }

        protected void BindGridView()
        {
            string connectionString = GetConnectionString();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string sql = "";
                switch (DropDownList1.SelectedValue)
                {
                    case "Digital Device":
                        sql = @"
                SELECT DigitalDevName AS Name, DigitalDevIP AS Address, PingStatus AS Status 
                FROM device";
                        break;
                    case "IP Address":
                        sql = @"
                SELECT Description AS Name, IPAddress AS Address, PingStatus AS Status 
                FROM ipaddress
                INNER JOIN Department ON ipaddress.DepartmentID = Department.DepartmentID";
                        break;
                    case "Web Address":
                        sql = @"
                SELECT WebAddress AS Name, WebAddress AS Address, PingStatus AS Status 
                FROM webaddress";
                        break;
                    default:
                        sql = @"
                SELECT DigitalDevName AS Name, DigitalDevIP AS Address, PingStatus AS Status 
                FROM device
                UNION ALL
                SELECT Description AS Name, IPAddress AS Address, PingStatus AS Status 
                FROM ipaddress
                INNER JOIN Department ON ipaddress.DepartmentID = Department.DepartmentID
                UNION ALL
                SELECT WebAddress AS Name, WebAddress AS Address, PingStatus AS Status 
                FROM webaddress";
                        break;
                }

                if (DropDownList1.SelectedValue != "Show All" && DropDownList2.SelectedValue != "Select Status")
                {
                    sql += $" WHERE PingStatus = '{DropDownList2.SelectedValue}'";
                }
                else if (DropDownList1.SelectedValue == "Show All" && DropDownList2.SelectedValue != "Select Status")
                {
                    sql = $"SELECT * FROM ({sql}) AS AllData WHERE Status = '{DropDownList2.SelectedValue}'";
                }

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (DropDownList1.SelectedValue != "Show All")
                        {
                            GridView1.Visible = false;
                            GridViewDevice.Visible = true;
                            GridViewDevice.DataSource = dt;
                            GridViewDevice.DataBind();
                            BindPageSummary(GridViewDevice);
                        }
                        else
                        {
                            GridViewDevice.Visible = false;
                            GridView1.Visible = true;
                            GridView1.DataSource = dt;
                            GridView1.DataBind();
                            BindPageSummary(GridView1);
                        }
                    }
                }
            }
        }

        private void BindPageSummary(GridView gridView)
        {
            if (gridView.Rows.Count == 0) return; // No data, exit method

            int pageSize = gridView.PageSize;
            int currentPage = gridView.PageIndex + 1;
            int totalRecords = 0;

            // Check if the DataSource is not null and is of type DataTable
            if (gridView.DataSource is DataTable dataTable)
            {
                totalRecords = dataTable.Rows.Count;
            }

            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            int startRecord = (currentPage - 1) * pageSize + 1;
            int endRecord = currentPage * pageSize;
            if (endRecord > totalRecords)
            {
                endRecord = totalRecords;
            }

            if (gridView.BottomPagerRow != null)
            {
                Label PageSummary = gridView.BottomPagerRow.FindControl("PageSummary") as Label;
                if (PageSummary != null)
                {
                    PageSummary.Text = $"Showing {startRecord} to {endRecord} of {totalRecords} entries";
                }
            }
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGridView();
        }

        protected void DropDownList2_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGridView();
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                GridView gridView = sender as GridView;
                if (gridView != null)
                {
                    gridView.PageIndex = e.NewPageIndex;
                    BindGridView();
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception
            }
        }


        private void BindPageSummary1()
        {
            if (GridView1.Rows.Count == 0) return; // No data, exit method

            int pageSize = GridView1.PageSize;
            int currentPage = GridView1.PageIndex + 1;
            int totalRecords = 0;

            // Check if the DataSource is not null and is of type DataTable
            if (GridView1.DataSource is DataTable dataTable)
            {
                totalRecords = dataTable.Rows.Count;
            }

            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            int startRecord = (currentPage - 1) * pageSize + 1;
            int endRecord = currentPage * pageSize;
            if (endRecord > totalRecords)
            {
                endRecord = totalRecords;
            }

            if (GridView1.BottomPagerRow != null)
            {
                Label PageSummary = GridView1.BottomPagerRow.FindControl("PageSummary") as Label;
                if (PageSummary != null)
                {
                    PageSummary.Text = $"Showing {startRecord} to {endRecord} of {totalRecords} entries";
                }
            }
        }
        protected void locationsbtn_CheckedChanged(object sender, EventArgs e)
        {
            Response.Redirect("load.aspx?target=location.aspx");
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string pingStatus = DataBinder.Eval(e.Row.DataItem, "Status").ToString();
                Literal pingStatusIndicator = (Literal)e.Row.FindControl("PingStatusIndicator");
                pingStatusIndicator.Text = GetStatusIndicator(pingStatus);
            }
        }
        protected string GetStatusIndicator(string status)
        {
            if (status == "Healthy")
            {
                return "<span style='color: #0AD17C; font-size: 24px;'>●</span>";
            }
            else if (status == "Down")
            {
                return "<span style='color: #c0392b; font-size: 24px;'>●</span>";
            }
            else if (status == "Problematic")
            {
                return "<span style='color: #E6A23C; font-size: 24px;'>●</span>";
            }
            return string.Empty; // Return an empty string if status is not HEALTHY, DOWN, or PROBLEMATIC
        }
        protected string GetStatusClass(object status)
        {
            if (status == null)
            {
                return "status-box-default"; // Default CSS class if status is null
            }

            string statusValue = status.ToString().ToLower();

            switch (statusValue)
            {
                case "problematic":
                    return "status-box-pending";
                case "down":
                    return "status-box-rejected";
                case "healthy":
                    return "status-box-healthy"; // Add the healthy case
                default:
                    return "status-box-default";
            }
        }
        protected void refresh_Click(object sender, EventArgs e)
        {
            Response.Redirect("load.aspx?target=realtime.aspx");
        }
    }
}
