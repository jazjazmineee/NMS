using ClosedXML.Excel;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading.Tasks;


namespace NMS.pages
{
    public partial class reports : BasePage
    {
        protected System.Web.UI.WebControls.Repeater PageRepeater;

        private readonly string[] dateFormats = { "MM/dd/yyyy", "M/d/yyyy", "MM-dd-yyyy", "M-d-yyyy", "yyyy-MM-dd", "ddd MMM dd yyyy" };

        protected void Page_Load(object sender, EventArgs e)
        {
            Server.ScriptTimeout = 300; // Set timeout to 5 minutes (300 seconds)

            if (!IsPostBack)
            {
                // Check for query parameters and set DropDownList1 and DropDownList2 values
                string statusQuery = Request.QueryString["status"];
                string tableNameQuery = Request.QueryString["tablename"];

                if (!string.IsNullOrEmpty(statusQuery))
                {
                    ListItem statusItem = DropDownList2.Items.FindByValue(statusQuery);
                    if (statusItem != null)
                    {
                        DropDownList2.ClearSelection();
                        statusItem.Selected = true;
                    }
                }

                if (!string.IsNullOrEmpty(tableNameQuery))
                {
                    ListItem tableNameItem = DropDownList1.Items.FindByValue(tableNameQuery);
                    if (tableNameItem != null)
                    {
                        DropDownList1.ClearSelection();
                        tableNameItem.Selected = true;
                    }
                }

                PopulateBuildingDropDown();
                BindGridView();
                BindPageSummary();
            }

            // Perform automatic deletion of old data
            AutoDeleteOldData();

            // Check if a delete operation was performed
            if (Session["RowsDeleted"] != null)
            {
                int rowsDeleted = (int)Session["RowsDeleted"];
                ShowDeleteNotification(rowsDeleted);
                Session.Remove("RowsDeleted");
            }
            else if (Session["NoDataToDelete"] != null)
            {
                ShowNoDataToDeleteNotification();
                Session.Remove("NoDataToDelete");
            }
            else if (Session["AutoDeletedRows"] != null)
            {
                int autoDeletedRows = (int)Session["AutoDeletedRows"];
                ShowAutoDeleteNotification(autoDeletedRows);
                Session.Remove("AutoDeletedRows");
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

        private void PopulateBuildingDropDown()
        {
            string connectionString = GetConnectionString();
            string query = "SELECT BuildingID, BuildingName FROM Building";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        BuildingDropDown.DataSource = dt;
                        BuildingDropDown.DataTextField = "BuildingName";
                        BuildingDropDown.DataValueField = "BuildingID";
                        BuildingDropDown.DataBind();
                    }
                }
            }

            // Adding a default item
            BuildingDropDown.Items.Insert(0, new ListItem("Select Building", "0"));
        }
        protected void BindGridView(string date = null, int pageIndex = 0)
        {
            string connectionString = GetConnectionString();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string tableName = DropDownList1.SelectedValue;
                string status = DropDownList2.SelectedValue;
                string searchText = searchtxtbox.Text.Trim();
                string buildingID = BuildingDropDown.SelectedValue;

                List<string> whereConditions = new List<string>();
                string selectedDate = date ?? ViewState["SelectedDate"] as string;

                if (tableName != "Select Category")
                {
                    whereConditions.Add($"reports.TableName = '{tableName}'");
                }

                if (status != "Select Status")
                {
                    whereConditions.Add($"reports.Status = '{status}'");
                }

                if (buildingID != "0")
                {
                    whereConditions.Add($"Building.BuildingID = {buildingID}");
                }

                if (!string.IsNullOrEmpty(selectedDate))
                {
                    whereConditions.Add($"DATE(reports.DateTime) = '{selectedDate}'");
                }

                if (!string.IsNullOrEmpty(searchText))
                {
                    whereConditions.Add($"(reports.TableName LIKE '%{searchText}%' OR reports.Description LIKE '%{searchText}%' OR reports.Address LIKE '%{searchText}%' OR reports.Status LIKE '%{searchText}%' OR Building.BuildingName LIKE '%{searchText}%')");
                }

                string query = @"SELECT reports.ReportID, reports.DateTime, reports.TableName, 
                         reports.Description, reports.Address, reports.Status, Building.BuildingName 
                         FROM reports 
                         LEFT JOIN Building ON reports.BuildingID = Building.BuildingID";

                if (whereConditions.Count > 0)
                {
                    query += " WHERE " + string.Join(" AND ", whereConditions);
                }

                query += " ORDER BY reports.ReportID DESC LIMIT @PageSize OFFSET @Offset";

                int pageSize = GridView1.PageSize;
                int offset = pageIndex * pageSize;

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                cmd.Parameters.AddWithValue("@Offset", offset);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                string countQuery = @"SELECT COUNT(*) 
                              FROM reports 
                              LEFT JOIN Building ON reports.BuildingID = Building.BuildingID";

                if (whereConditions.Count > 0)
                {
                    countQuery += " WHERE " + string.Join(" AND ", whereConditions);
                }

                MySqlCommand countCmd = new MySqlCommand(countQuery, connection);
                int totalRecords = Convert.ToInt32(countCmd.ExecuteScalar());

                GridView1.DataSource = dt;
                GridView1.DataBind();

                ViewState["TotalRecords"] = totalRecords;
                ViewState["TotalPages"] = (int)Math.Ceiling((double)totalRecords / pageSize);

                BindPageSummary();
                UpdateResultsLabel(totalRecords); // Call UpdateResultsLabel here
            }
        }

        protected async void BindGridViewAsync(int pageIndex = 0)
        {
            string connectionString = GetConnectionString();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string tableName = DropDownList1.SelectedValue;
                string status = DropDownList2.SelectedValue;
                string searchText = searchtxtbox.Text.Trim();
                string buildingID = BuildingDropDown.SelectedValue;

                List<string> whereConditions = new List<string>();

                string selectedDate = ViewState["SelectedDate"] as string;

                if (tableName != "Select Category")
                {
                    whereConditions.Add($"reports.TableName = '{tableName}'");
                }

                if (status != "Select Status")
                {
                    whereConditions.Add($"reports.Status = '{status}'");
                }

                if (buildingID != "0")
                {
                    whereConditions.Add($"Building.BuildingID = {buildingID}");
                }

                if (!string.IsNullOrEmpty(selectedDate))
                {
                    whereConditions.Add($"DATE(reports.DateTime) = '{selectedDate}'");
                }

                if (!string.IsNullOrEmpty(searchText))
                {
                    whereConditions.Add($"(reports.TableName LIKE '%{searchText}%' OR reports.Description LIKE '%{searchText}%' OR reports.Address LIKE '%{searchText}%' OR reports.Status LIKE '%{searchText}%' OR Building.BuildingName LIKE '%{searchText}%')");
                }

                string query = @"SELECT reports.ReportID, reports.DateTime, reports.TableName, 
                                 reports.Description, reports.Address, reports.Status, Building.BuildingName 
                                 FROM reports 
                                 LEFT JOIN Building ON reports.BuildingID = Building.BuildingID";

                if (whereConditions.Count > 0)
                {
                    query += " WHERE " + string.Join(" AND ", whereConditions);
                }

                query += " ORDER BY reports.ReportID DESC LIMIT @PageSize OFFSET @Offset";

                int pageSize = GridView1.PageSize;
                int offset = pageIndex * pageSize;

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                cmd.Parameters.AddWithValue("@Offset", offset);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                await Task.Run(() => adapter.Fill(dt));

                string countQuery = @"SELECT COUNT(*) 
                      FROM reports 
                      LEFT JOIN Building ON reports.BuildingID = Building.BuildingID";

                if (whereConditions.Count > 0)
                {
                    countQuery += " WHERE " + string.Join(" AND ", whereConditions);
                }

                MySqlCommand countCmd = new MySqlCommand(countQuery, connection);
                int totalRecords = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

                GridView1.DataSource = dt;
                GridView1.DataBind();

                ViewState["TotalRecords"] = totalRecords;
                ViewState["TotalPages"] = (int)Math.Ceiling((double)totalRecords / pageSize);

                BindPageSummary();
            }
        }
        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            int newPageIndex = e.NewPageIndex;
            int totalPages = (int)ViewState["TotalPages"];

            if (newPageIndex >= 0 && newPageIndex < totalPages)
            {
                GridView1.PageIndex = newPageIndex;
                string selectedDate = ViewState["SelectedDate"] as string;
                BindGridView(selectedDate, newPageIndex);
                BindPageSummary();
            }
        }

        private void BindPageSummary()
        {
            int pageSize = GridView1.PageSize;
            int currentPage = GridView1.PageIndex + 1;
            int totalRecords = (int)ViewState["TotalRecords"];
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
        protected void EntriesDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pageSize = int.Parse(EntriesDropdown.SelectedValue);
            GridView1.PageSize = pageSize;
            BindGridView(); // Rebind data to reflect new page size
            UpdateResultsLabel((int)ViewState["TotalRecords"]); // Update results label
        }

        private void UpdateResultsLabel(int totalResults)
        {
            int pageSize = int.Parse(EntriesDropdown.SelectedValue);
            int currentPage = GridView1.PageIndex + 1;
            int startRow = (currentPage - 1) * pageSize + 1;
            int endRow = startRow + pageSize - 1;
            if (endRow > totalResults)
            {
                endRow = totalResults;
            }
            resultsLabel.Text = $"Showing {startRow} to {endRow} of {totalResults} results";
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
        protected void BuildingDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGridView();
            UpdateResultsLabel((int)ViewState["TotalRecords"]); // Update results label
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGridView();
            UpdateResultsLabel((int)ViewState["TotalRecords"]);
        }

        protected void DropDownList2_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGridView();
            UpdateResultsLabel((int)ViewState["TotalRecords"]);
        }

        protected void searchtxtbox_TextChanged(object sender, EventArgs e)
        {
            BindGridView();
            UpdateResultsLabel((int)ViewState["TotalRecords"]);
        }
        protected void singleDateRange_TextChanged(object sender, EventArgs e)
        {
            string selectedDate = singleDateRange.Text;

            // Ensure date is parsed correctly
            if (DateTime.TryParseExact(selectedDate, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                ViewState["SelectedDate"] = parsedDate.ToString("yyyy-MM-dd");
                BindGridView(parsedDate.ToString("yyyy-MM-dd"));
            }
            else
            {
                // Handle the error or show a notification if the date is invalid
                ViewState["SelectedDate"] = null;
                BindGridView();
            }

            UpdateResultsLabel((int)ViewState["TotalRecords"]);
        }
        protected void downloadbtn_Click(object sender, EventArgs e)
        {
            try
            {
                string connectionString = GetConnectionString();
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string tableName = DropDownList1.SelectedValue;
                    string status = DropDownList2.SelectedValue;
                    string searchText = searchtxtbox.Text.Trim();
                    string buildingID = BuildingDropDown.SelectedValue;
                    string selectedDate = ViewState["SelectedDate"] as string;

                    List<string> whereConditions = new List<string>();

                    if (tableName != "Select Category")
                    {
                        whereConditions.Add($"reports.TableName = '{tableName}'");
                    }

                    if (status != "Select Status")
                    {
                        whereConditions.Add($"reports.Status = '{status}'");
                    }

                    if (buildingID != "0")
                    {
                        whereConditions.Add($"Building.BuildingID = {buildingID}");
                    }

                    if (!string.IsNullOrEmpty(selectedDate))
                    {
                        whereConditions.Add($"DATE(reports.DateTime) = '{selectedDate}'");
                    }

                    if (!string.IsNullOrEmpty(searchText))
                    {
                        whereConditions.Add($"(reports.TableName LIKE '%{searchText}%' OR reports.Description LIKE '%{searchText}%' OR reports.Address LIKE '%{searchText}%' OR reports.Status LIKE '%{searchText}%' OR Building.BuildingName LIKE '%{searchText}%')");
                    }

                    // If no filters are selected, download only today's data
                    if (whereConditions.Count == 0)
                    {
                        string today = DateTime.Now.ToString("yyyy-MM-dd");
                        whereConditions.Add($"DATE(reports.DateTime) = '{today}'");
                    }

                    string query = @"SELECT reports.ReportID, reports.DateTime, reports.TableName, 
                             reports.Description, reports.Address, reports.Status, Building.BuildingName AS BuildingName
                             FROM reports 
                             LEFT JOIN Building ON reports.BuildingID = Building.BuildingID";

                    if (whereConditions.Count > 0)
                    {
                        query += " WHERE " + string.Join(" AND ", whereConditions);
                    }

                    query += " ORDER BY reports.ReportID DESC";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(dt, "Reports");

                        using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
                        {
                            wb.SaveAs(memoryStream);
                            byte[] byteArray = memoryStream.ToArray();

                            Response.Clear();
                            Response.Buffer = true;
                            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            Response.AddHeader("content-disposition", "attachment;filename=Reports.xlsx");
                            Response.BinaryWrite(byteArray);
                            Response.Flush();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception and show an error message
                Response.Write("Error generating report. Please try again later.");
            }
        }


        protected void deletebtn_Click(object sender, EventArgs e)
        {
            int rowsDeleted = DeleteOldData();
            BindGridView();
            if (rowsDeleted > 0)
            {
                Session["RowsDeleted"] = rowsDeleted;
            }
            else
            {
                Session["NoDataToDelete"] = true;
            }

            // Redirect to the same page to refresh
            Response.Redirect(Request.RawUrl);
        }
        private void ShowDeleteNotification(int rowsDeleted)
        {
            string script = $"showToast({rowsDeleted});";
            ScriptManager.RegisterStartupScript(this, GetType(), "showToastScript", script, true);
        }

        private void ShowNoDataToDeleteNotification()
        {
            string script = "showNoDataToDeleteToast();";
            ScriptManager.RegisterStartupScript(this, GetType(), "showNoDataToDeleteToastScript", script, true);
        }
        private int DeleteOldData()
        {
            string connectionString = GetConnectionString();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM reports WHERE DateTime < DATE_SUB(NOW(), INTERVAL 3 MONTH)";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected;
            }
        }

        private void ShowAutoDeleteNotification(int autoDeletedRows)
        {
            string script = $"showAutoDeleteToast({autoDeletedRows});";
            ScriptManager.RegisterStartupScript(this, GetType(), "showAutoDeleteToastScript", script, true);
        }
        private void AutoDeleteOldData()
        {
            string connectionString = GetConnectionString();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM reports WHERE DateTime < DATE_SUB(NOW(), INTERVAL 3 MONTH)";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Session["AutoDeletedRows"] = rowsAffected;
                }
            }
        }

        protected void refresh_Click(object sender, EventArgs e)
        {
            Response.Redirect("load.aspx?target=reports.aspx");
        }
    }
}
