using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;

namespace NMS.pages
{
    public partial class dashboard_admin : BasePage
    {
        public string CombinedHourlyData { get; set; }

        protected async void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var tasks = new List<Task>
                {
                    BindCharts(),
                    Task.Run(() => BindFormViewData()),
                    Task.Run(() => CheckProblematicIPs())
                };

                await Task.WhenAll(tasks);

                ShowDateTimeInputs(false);
                FilterDropdown.SelectedValue = "Day";
                DateTime startDate = DateTime.Today;
                DateTime endDate = DateTime.Now;
                string groupByClause = "DATE_FORMAT(DateTime, '%Y-%m-%d %H:%i')";
                string orderByClause = groupByClause;

                try
                {
                    List<string> statuses = new List<string> { "Healthy", "Down", "Problematic" };

                    DataTable filteredData = await GetFilteredChartDataAsync(statuses, startDate, endDate, groupByClause, orderByClause);
                    CombinedHourlyData = DataTableToJson(filteredData);

                    ScriptManager.RegisterStartupScript(this, GetType(), "updateChart", "updateChart();", true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in Page_Load: {ex.Message}");
                }
            }
        }

        private string GetConnectionString()
        {
            return "Server=localhost;Database=networkmonitoring;Uid=root;Pwd=;Connection Timeout=60;";
        }
        private async Task CheckProblematicIPs()
        {
            string connectionString = GetConnectionString();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = @"
                    SELECT 
                        (SELECT COUNT(*) FROM device WHERE PingStatus = 'Problematic') +
                        (SELECT COUNT(*) FROM ipaddress WHERE PingStatus = 'Problematic') +
                        (SELECT COUNT(*) FROM webaddress WHERE PingStatus = 'Problematic')
                    AS ProblematicCount,
                        (SELECT COUNT(*) FROM device WHERE PingStatus = 'Down') +
                        (SELECT COUNT(*) FROM ipaddress WHERE PingStatus = 'Down') +
                        (SELECT COUNT(*) FROM webaddress WHERE PingStatus = 'Down')
                    AS DownCount";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        int problematicCount = 0;
                        int downCount = 0;

                        if (await reader.ReadAsync())
                        {
                            problematicCount = reader.GetInt32("ProblematicCount");
                            downCount = reader.GetInt32("DownCount");
                        }

                        int previousProblematicCount = Session["ProblematicCount"] != null ? (int)Session["ProblematicCount"] : 0;
                        int previousDownCount = Session["DownCount"] != null ? (int)Session["DownCount"] : 0;

                        Session["ProblematicCount"] = problematicCount;
                        Session["DownCount"] = downCount;

                        StringBuilder script = new StringBuilder();
                        script.Append("<script type='text/javascript'>");

                        if ((problematicCount > 0 && problematicCount != previousProblematicCount) ||
                            (downCount > 0 && downCount != previousDownCount))
                        {
                            string message = "\n";
                            if (problematicCount > 0)
                            {
                                message += $"{problematicCount} IP address are having connection problems.\n\n";
                            }
                            if (downCount > 0)
                            {
                                message += $"{downCount} IP address are down.";
                            }
                            script.AppendFormat("showToast('Warning', '{0}');", message.Replace("\n", "\\n"));
                        }
                        else
                        {
                            script.Append("hideToast();");
                        }
                        script.Append("</script>");

                        ClientScript.RegisterStartupScript(this.GetType(), "showToastOrHideToast", script.ToString());
                    }
                }
            }
        }

        private void BindFormViewData()
        {
            string connectionString = GetConnectionString();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                SELECT PingStatus, COUNT(*) AS StatusCount
                FROM (
                    SELECT PingStatus FROM device
                    UNION ALL
                    SELECT PingStatus FROM ipaddress
                    UNION ALL
                    SELECT PingStatus FROM webaddress
                ) AS AllStatuses
                GROUP BY PingStatus";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        var statusCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
                        {
                            { "Total Healthy", 0 },
                            { "Total Down", 0 },
                            { "Total Problematic", 0 }
                        };

                        foreach (DataRow row in dataSet.Tables[0].Rows)
                        {
                            string pingStatus = row["PingStatus"].ToString().ToUpper();
                            int count = Convert.ToInt32(row["StatusCount"]);

                            switch (pingStatus)
                            {
                                case "HEALTHY":
                                    statusCounts["Total Healthy"] = count;
                                    break;
                                case "DOWN":
                                    statusCounts["Total Down"] = count;
                                    break;
                                case "PROBLEMATIC":
                                    statusCounts["Total Problematic"] = count;
                                    break;
                            }
                        }

                        DataTable dt = new DataTable();
                        dt.Columns.Add("PingStatus", typeof(string));
                        dt.Columns.Add("StatusCount", typeof(int));

                        foreach (var entry in statusCounts)
                        {
                            DataRow newRow = dt.NewRow();
                            newRow["PingStatus"] = entry.Key;
                            newRow["StatusCount"] = entry.Value;
                            dt.Rows.Add(newRow);
                        }

                        BindFormViewDataSource(dt, "Total Healthy", FormViewHealthy, FormViewHealthyStatus);
                        BindFormViewDataSource(dt, "Total Down", FormViewDown, FormViewDownStatus);
                        BindFormViewDataSource(dt, "Total Problematic", FormViewProblematic, FormViewProblematicStatus);
                    }
                }
            }
        }
        protected void FormViewHealthy_PageIndexChanging(object sender, FormViewPageEventArgs e)
        {
            BindFormViewData();
        }

        protected void FormViewDown_PageIndexChanging(object sender, FormViewPageEventArgs e)
        {
            BindFormViewData();
        }

        protected void FormViewProblematic_PageIndexChanging(object sender, FormViewPageEventArgs e)
        {
            BindFormViewData();
        }

        private void BindFormViewDataSource(DataTable dt, string status, FormView formView, FormView formViewStatus)
        {
            DataTable dtFiltered = dt.Clone();
            foreach (DataRow row in dt.Select($"PingStatus = '{status}'"))
            {
                dtFiltered.ImportRow(row);
            }
            formView.DataSource = dtFiltered;
            formView.DataBind();
            formViewStatus.DataSource = dtFiltered;
            formViewStatus.DataBind();
        }

        protected async void btnApply_Click(object sender, EventArgs e)
        {
            // Parse start and end date times
            DateTime startDateTimeParsed = DateTime.ParseExact(startDateTimeInput.Value, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
            DateTime endDateTimeParsed = DateTime.ParseExact(endDateTimeInput.Value, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);

            List<string> statuses = new List<string> { "Healthy", "Down", "Problematic" };

            // Set the appropriate group by and order by clauses based on the current filter
            string groupByClause = "DATE_FORMAT(DateTime, '%Y-%m-%d %H:%i')";
            string orderByClause = groupByClause; // Adjust this if needed for your specific scenario

            DataTable filteredData = await GetFilteredChartDataAsync(statuses, startDateTimeParsed, endDateTimeParsed, groupByClause, orderByClause);

            CombinedHourlyData = DataTableToJson(filteredData);
            ScriptManager.RegisterStartupScript(this, GetType(), "updateChart", "updateChart();", true);
        }

        private string DataTableToJson(DataTable dt)
        {
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(rows);
        }
        protected async void FilterDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedValue = FilterDropdown.SelectedValue;
            DateTime startDateTime;
            DateTime endDateTime = DateTime.Now;

            string groupByClause, orderByClause;
            int daysToSunday = (int)DateTime.Today.DayOfWeek; // Initialize daysToSunday

            switch (selectedValue)
            {
                case "1":
                    startDateTime = DateTime.Now.AddMinutes(-1);
                    groupByClause = "DATE_FORMAT(DateTime, '%Y-%m-%d %H:%i:%s')"; // Group by date, hour, minute, and second
                    orderByClause = groupByClause;
                    ShowDateTimeInputs(false); // Hide date time inputs
                    break;
                case "5":
                    startDateTime = DateTime.Now.AddMinutes(-5);
                    groupByClause = "DATE_FORMAT(DateTime, '%Y-%m-%d %H:%i:%s')"; // Group by date, hour, minute, and second
                    orderByClause = groupByClause;
                    ShowDateTimeInputs(false); // Hide date time inputs
                    break;
                case "10":
                    startDateTime = DateTime.Now.AddMinutes(-10);
                    groupByClause = "DATE_FORMAT(DateTime, '%Y-%m-%d %H:%i:%s')"; // Group by date, hour, minute, and second
                    orderByClause = groupByClause;
                    ShowDateTimeInputs(false); // Hide date time inputs
                    break;
                case "30":
                    startDateTime = DateTime.Now.AddMinutes(-30);
                    groupByClause = "DATE_FORMAT(DateTime, '%Y-%m-%d %H:%i:%s')"; // Group by date, hour, minute, and second
                    orderByClause = groupByClause;
                    ShowDateTimeInputs(false); // Hide date time inputs
                    break;
                case "Hour":
                    startDateTime = DateTime.Now.AddHours(-1);
                    groupByClause = "DATE_FORMAT(DateTime, '%Y-%m-%d %H:%i:%s')"; // Group by date, hour, minute, and second
                    orderByClause = groupByClause;
                    ShowDateTimeInputs(false); // Hide date time inputs
                    break;
                case "Day":
                    startDateTime = DateTime.Today;
                    groupByClause = "DATE_FORMAT(DateTime, '%Y-%m-%d %H:%i')"; // Group by date, hour, and minute
                    orderByClause = groupByClause;
                    ShowDateTimeInputs(false); // Hide date time inputs
                    break;
                case "Week":
                    // Calculate the start of the current week (Sunday)
                    startDateTime = DateTime.Today.AddDays(-daysToSunday);
                    groupByClause = "DAYNAME(DateTime)"; // Group by day of the week (1 = Sunday, ..., 7 = Saturday)
                    orderByClause = "DAYOFWEEK(DateTime)"; // Order by day of the week
                    ShowDateTimeInputs(false); // Hide date time inputs
                    break;
                case "Month":
                    startDateTime = DateTime.Today.AddMonths(-1);
                    groupByClause = "MONTH(DateTime)"; // Group by month
                    orderByClause = groupByClause;
                    ShowDateTimeInputs(false); // Hide date time inputs
                    break;
                case "Year":
                    startDateTime = DateTime.Today.AddYears(-1);
                    groupByClause = "YEAR(DateTime)"; // Group by year
                    orderByClause = groupByClause;
                    ShowDateTimeInputs(false); // Hide date time inputs
                    break;
                case "Custom":
                    // Calculate the start of the current week (Sunday) for Custom
                    startDateTime = DateTime.Today.AddDays(-daysToSunday);
                    groupByClause = "DAYNAME(DateTime)"; // Group by day of the week (1 = Sunday, ..., 7 = Saturday)
                    orderByClause = "DAYOFWEEK(DateTime)"; // Order by day of the week
                    ShowDateTimeInputs(true); // Show date time inputs
                    break;
                default:
                    startDateTime = DateTime.Today;
                    groupByClause = "DATE_FORMAT(DateTime, '%Y-%m-%d %H:%i')";
                    orderByClause = groupByClause;
                    ShowDateTimeInputs(false); // Hide date time inputs for default
                    break;
            }

            // Reset startDateTime and endDateTime inputs if not "Custom"
            if (selectedValue != "Custom")
            {
                startDateTimeInput.Value = ""; // Clear the startDateTime input
                endDateTimeInput.Value = ""; // Clear the endDateTime input
            }

            List<string> statuses = new List<string> { "Healthy", "Down", "Problematic" };
            DataTable filteredData = await GetFilteredChartDataAsync(statuses, startDateTime, endDateTime, groupByClause, orderByClause);

            CombinedHourlyData = DataTableToJson(filteredData);
            ScriptManager.RegisterStartupScript(this, GetType(), "updateChart", "updateChart();toggleTimeInputs();", true);
        }

        private void ShowDateTimeInputs(bool show)
        {
            startDateTimeLabel.Visible = show;
            startDateTimeInput.Visible = show;
            endDateTimeLabel.Visible = show;
            endDateTimeInput.Visible = show;
            btnApply.Visible = show;
        }
        private async Task<DataTable> GetFilteredChartDataAsync(List<string> statuses, DateTime startDateTime, DateTime endDateTime, string groupByClause, string orderByClause)
        {
            string statusCondition = string.Join(" OR ", statuses.Select(status => $"Status = '{status}'"));
            string query = $@"
            SELECT
                {groupByClause} AS TimePeriod,
                SUM(CASE WHEN Status = 'Healthy' THEN 1 ELSE 0 END) AS Healthy,
                SUM(CASE WHEN Status = 'Down' THEN 1 ELSE 0 END) AS Down,
                SUM(CASE WHEN Status = 'Problematic' THEN 1 ELSE 0 END) AS Problematic
            FROM
                reports
            WHERE
                ({statusCondition}) AND
                DateTime >= @startDateTime AND
                DateTime < @endDateTime
            GROUP BY
                {groupByClause}
            ORDER BY
                {orderByClause}";

            var dataTable = new DataTable();

            try
            {
                using (var connection = new MySqlConnection(GetConnectionString()))
                {
                    await connection.OpenAsync();

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@startDateTime", startDateTime);
                        command.Parameters.AddWithValue("@endDateTime", endDateTime);

                        using (var adapter = new MySqlDataAdapter(command))
                        {
                            await Task.Run(() => adapter.Fill(dataTable));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Enhanced error handling
                Console.WriteLine($"Error in GetFilteredChartDataAsync: {ex.Message}");
            }

            return dataTable;
        }

        private DataTable GetCombinedHourlyChartData(List<string> statuses)
        {
            string connectionString = GetConnectionString();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string statusCondition = string.Join(" OR ", statuses.Select(status => $"Status = '{status}'"));
                string query = $@"
                SELECT
                    HOUR(DateTime) AS Hour,
                    SUM(CASE WHEN Status = 'Healthy' THEN 1 ELSE 0 END) AS Healthy,
                    SUM(CASE WHEN Status = 'Down' THEN 1 ELSE 0 END) AS Down,
                    SUM(CASE WHEN Status = 'Problematic' THEN 1 ELSE 0 END) AS Problematic
                FROM
                    reports
                WHERE
                    ({statusCondition}) AND
                    DateTime >= CURDATE() AND
                    DateTime < CURDATE() + INTERVAL 1 DAY
                GROUP BY
                    HOUR(DateTime)
                ORDER BY
                    Hour";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }
        private async Task<DataTable> GetHourlyChartDataAsync(string status)
        {
            string connectionString = GetConnectionString();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    string query = @"
                    SELECT
                        HOUR(DateTime) AS Hour,
                        COUNT(*) AS Total
                    FROM
                        reports
                    WHERE
                        Status = @status AND
                        DateTime >= @startDate AND
                        DateTime < @endDate
                    GROUP BY
                        HOUR(DateTime)
                    ORDER BY
                        Hour";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@startDate", DateTime.Today);
                    cmd.Parameters.AddWithValue("@endDate", DateTime.Today.AddDays(1));
                    cmd.CommandTimeout = 120; // Set command timeout to 120 seconds

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    await Task.Run(() => adapter.Fill(dt));
                    return dt;
                }
                catch (MySqlException ex)
                {
                    // Log MySQL specific errors
                    Console.WriteLine($"MySQL Error: {ex.Message}");
                    throw;
                }
                catch (IOException ex)
                {
                    // Log I/O errors
                    Console.WriteLine($"I/O Error: {ex.Message}");
                    throw;
                }
                catch (SocketException ex)
                {
                    // Log socket errors
                    Console.WriteLine($"Socket Error: {ex.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    // Log general errors
                    Console.WriteLine($"General Error: {ex.Message}");
                    throw;
                }
            }
        }

        private async Task<string> GetSerializedChartDataAsync(string status)
        {
            DataTable dt = await GetHourlyChartDataAsync(status);
            var data = new List<object>();

            foreach (DataRow row in dt.Rows)
            {
                data.Add(new
                {
                    Hour = Convert.ToInt32(row["Hour"]),
                    Total = Convert.ToInt32(row["Total"])
                });
            }

            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(data);
        }
        private async Task BindCharts()
        {
            // Get device, IP, and web data synchronously
            var deviceData = GetStatusChartData("SELECT PingStatus, COUNT(*) AS StatusCount FROM device GROUP BY PingStatus");
            var ipData = GetStatusChartData("SELECT PingStatus, COUNT(*) AS StatusCount FROM ipaddress GROUP BY PingStatus");
            var webData = GetStatusChartData("SELECT PingStatus, COUNT(*) AS StatusCount FROM webaddress GROUP BY PingStatus");

            // Serialize device, IP, and web data
            deviceChartData.Value = SerializeData(deviceData);
            ipChartData.Value = SerializeData(ipData);
            webChartData.Value = SerializeData(webData);

            // Get serialized data asynchronously
            healthyChartData.Value = await GetSerializedChartDataAsync("Healthy");
            downChartData.Value = await GetSerializedChartDataAsync("Down");
            probChartData.Value = await GetSerializedChartDataAsync("Problematic");
        }

        private DataTable GetStatusChartData(string query)
        {
            string connectionString = GetConnectionString();
            DataTable dt = new DataTable();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    conn.Open();
                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }

        private string SerializeData(DataTable dt)
        {
            var data = new List<object>();

            foreach (DataRow row in dt.Rows)
            {
                data.Add(new
                {
                    PingStatus = row["PingStatus"].ToString(),
                    StatusCount = Convert.ToInt32(row["StatusCount"])
                });
            }

            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(data);
        }
        protected void refresh_Click(object sender, EventArgs e)
        {
            Response.Redirect("load.aspx?target=dashboard-admin.aspx");
        }
        protected void OverviewBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("load.aspx?target=reports.aspx");
        }
    }
}