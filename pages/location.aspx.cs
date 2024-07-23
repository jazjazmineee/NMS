using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NMS.pages
{
    public partial class location : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateBuildingCheckBoxList();
                PopulateLocationDropdown();

                // Ensure there is at least one building to select
                if (BuildingCheckBoxList.Items.Count > 0)
                {
                    BuildingCheckBoxList.Items[0].Selected = true; // Select the first building by default
                    PopulateLocationDropdown(); // Populate locations based on the first building
                    BindAllGrids("");
                    UpdateGridVisibility();
                }
            }
        }
        protected void PopulateBuildingCheckBoxList()
        {
            using (MySqlConnection con = new MySqlConnection(GetConnectionString()))
            {
                string query = "SELECT BuildingID, BuildingName FROM Building";
                MySqlCommand cmd = new MySqlCommand(query, con);
                con.Open();
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    string buildingID = rdr["BuildingID"].ToString();
                    string buildingName = rdr["BuildingName"].ToString();

                    ListItem item = new ListItem(buildingName, buildingID);
                    BuildingCheckBoxList.Items.Add(item);
                }
            }
        }

        protected void PopulateLocationDropdown()
        {
            DepartmentDropDown.Items.Clear();
            DepartmentDropDown.Items.Add("Select Location");

            List<string> selectedBuildings = BuildingCheckBoxList.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToList();
            if (selectedBuildings.Count > 0)
            {
                using (MySqlConnection con = new MySqlConnection(GetConnectionString()))
                {
                    string query = "SELECT DISTINCT DepartmentName FROM Department WHERE BuildingID IN (" + string.Join(",", selectedBuildings.Select(id => "'" + id + "'")) + ")";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    con.Open();
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        string location = rdr["DepartmentName"].ToString();
                        DepartmentDropDown.Items.Add(location);
                    }
                }
            }
            else
            {
                using (MySqlConnection con = new MySqlConnection(GetConnectionString()))
                {
                    string query = "SELECT DISTINCT DepartmentName FROM Department";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    con.Open();
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        string location = rdr["DepartmentName"].ToString();
                        DepartmentDropDown.Items.Add(location);
                    }
                }
            }
        }

        protected void SelectAllBuildingsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bool selectAll = SelectAllBuildingsCheckBox.Checked;
            foreach (ListItem item in BuildingCheckBoxList.Items)
            {
                item.Selected = selectAll;
            }
            PopulateLocationDropdown(); // Update locations based on selected buildings
            BindAllGrids(""); // Rebind all grids
            UpdateGridVisibility(); // Update grid visibility
        }
      


        private string GetConnectionString()
        {
            string server = "localhost";
            string database = "networkmonitoring";
            string username = "root";
            string password = "";
            return $"Server={server};Database={database};Uid={username};Pwd={password};";
        }
        protected void BuildingDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateLocationDropdown();
            BindAllGrids("");
            ClearGridViews(); // Clear grid views before binding new data
            UpdateGridVisibility();
        }

        protected void BuildingCheckBoxList_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateLocationDropdown();
            BindAllGrids("");
            ClearGridViews(); // Clear grid views before binding new data
            UpdateGridVisibility();
        }

        private string selectedLocation = null;

        protected void DepartmentDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedLocation = DepartmentDropDown.SelectedValue;
            BindAllGrids("");
            ClearGridViews(); // Clear grid views before binding new data
            UpdateGridVisibility();
        }

        protected void StatusDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindAllGrids("");
            ClearGridViews(); // Clear grid views before binding new data
            UpdateGridVisibility();
        }

        protected void CategoryDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearGridViews(); // Clear grid views before binding new data
            string selectedCategory = CategoryDropDown.SelectedValue;

            if (selectedCategory == "Select Category")
            {
                BindAllGrids("");
            }
            else
            {
                UpdateGridViewsBasedOnCategory();
            }
            UpdateGridVisibility();
        }
        private void UpdateGridViewsBasedOnCategory()
        {
            string selectedCategory = CategoryDropDown.SelectedValue;

            if (selectedCategory == "web")
            {
                BindWebGrid("");
                WebGridView.Visible = WebGridView.Rows.Count > 0;
                WebGridView2.Visible = WebGridView2.Rows.Count > 0;
                IPGridView1.Visible = false;
                IPGridView2.Visible = false;
                DeviceGridView.Visible = false;
                DeviceGridView2.Visible = false;
                AllGridView1.Visible = false;
                AllGridView2.Visible = false;
            }
            else if (selectedCategory == "ip")
            {
                BindIPGrid("");
                IPGridView1.Visible = IPGridView1.Rows.Count > 0;
                IPGridView2.Visible = IPGridView2.Rows.Count > 0;
                WebGridView.Visible = false;
                WebGridView2.Visible = false;
                DeviceGridView.Visible = false;
                DeviceGridView2.Visible = false;
                AllGridView1.Visible = false;
                AllGridView2.Visible = false;
            }
            else if (selectedCategory == "device")
            {
                BindDeviceGrid("");
                DeviceGridView.Visible = DeviceGridView.Rows.Count > 0;
                DeviceGridView2.Visible = DeviceGridView2.Rows.Count > 0;
                WebGridView.Visible = false;
                WebGridView2.Visible = false;
                IPGridView1.Visible = false;
                IPGridView2.Visible = false;
                AllGridView1.Visible = false;
                AllGridView2.Visible = false;
            }
            else
            {
                // Show AllGridViews only when "Select Category" is selected
                BindAllGrids("");
                AllGridView1.Visible = AllGridView1.Rows.Count > 0;
                AllGridView2.Visible = AllGridView2.Rows.Count > 0;
                WebGridView.Visible = false;
                WebGridView2.Visible = false;
                IPGridView1.Visible = false;
                IPGridView2.Visible = false;
                DeviceGridView.Visible = false;
                DeviceGridView2.Visible = false;
                DeviceGridView.Columns.Clear();
                IPGridView1.Columns.Clear();
                WebGridView.Columns.Clear();
            }
        }
        private void UpdateGridVisibility()
        {
            // Get selected building IDs from checkboxes
            List<string> selectedBuildingIds = BuildingCheckBoxList.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToList();
            string selectedCategory = CategoryDropDown.SelectedValue;

            bool showAllBuildings = selectedBuildingIds.Count == 0 || selectedBuildingIds.Contains("External");
            bool showAllLocations = DepartmentDropDown.SelectedValue == null || DepartmentDropDown.SelectedValue == "Select Location";

            // Hide all specific grid views initially
            WebGridView.Visible = false;
            WebGridView2.Visible = false;
            IPGridView1.Visible = false;
            IPGridView2.Visible = false;
            DeviceGridView.Visible = false;
            DeviceGridView2.Visible = false;
            AllGridView1.Visible = false;
            AllGridView2.Visible = false;

            // Construct filters based on selected buildings
            string buildingFilter = showAllBuildings ? "" : $" AND Department.BuildingID IN ({string.Join(",", selectedBuildingIds)})";

            // Show all grids if "Select Category" is selected
            if (selectedCategory == "Select Category")
            {
                BindAllGrids(buildingFilter);
                AllGridView1.Visible = AllGridView1.Rows.Count > 0;
                AllGridView2.Visible = AllGridView2.Rows.Count > 0;
            }
            else
            {
                // Determine visibility based on selected category and whether to show all buildings or locations
                if (selectedCategory == "web")
                {
                    BindWebGrid(buildingFilter);
                    WebGridView.Visible = WebGridView.Rows.Count > 0;
                    WebGridView2.Visible = WebGridView2.Rows.Count > 0;
                }
                else if (selectedCategory == "ip")
                {
                    BindIPGrid(buildingFilter);
                    IPGridView1.Visible = IPGridView1.Rows.Count > 0;
                    IPGridView2.Visible = IPGridView2.Rows.Count > 0;
                }
                else if (selectedCategory == "device")
                {
                    BindDeviceGrid(buildingFilter);
                    DeviceGridView.Visible = DeviceGridView.Rows.Count > 0;
                    DeviceGridView2.Visible = DeviceGridView2.Rows.Count > 0;
                }
            }
        }
        private void ClearGridViews()
        {
            WebGridView.Columns.Clear();
            WebGridView.DataSource = null;
            WebGridView.DataBind();

            WebGridView2.Columns.Clear();
            WebGridView2.DataSource = null;
            WebGridView2.DataBind();

            IPGridView1.Columns.Clear();
            IPGridView1.DataSource = null;
            IPGridView1.DataBind();

            IPGridView2.Columns.Clear();
            IPGridView2.DataSource = null;
            IPGridView2.DataBind();

            DeviceGridView.Columns.Clear();
            DeviceGridView.DataSource = null;
            DeviceGridView.DataBind();

            DeviceGridView2.Columns.Clear();
            DeviceGridView2.DataSource = null;
            DeviceGridView2.DataBind();

            AllGridView1.Columns.Clear();
            AllGridView1.DataSource = null;
            AllGridView1.DataBind();

            AllGridView2.Columns.Clear();
            AllGridView2.DataSource = null;
            AllGridView2.DataBind();
        }
        private void BindAllGrids(string buildingFilter)
        {
            using (MySqlConnection con = new MySqlConnection(GetConnectionString()))
            {
                string baseQuery = @"
                SELECT Department.DepartmentName, IPAddress.IPAddress AS Address, IPAddress.Description AS Name, IPAddress.PingStatus AS Status
                FROM IPAddress
                INNER JOIN Department ON IPAddress.DepartmentID = Department.DepartmentID
                WHERE 1=1 {0}

                UNION ALL

                SELECT Department.DepartmentName, Device.DigitalDevIP AS Address, Device.DigitalDevName AS Name, Device.PingStatus AS Status
                FROM Device
                INNER JOIN Department ON Device.DepartmentID = Department.DepartmentID
                WHERE 1=1 {1}

                UNION ALL

                SELECT Department.DepartmentName, WebAddress.WebAddress AS Address, NULL AS Name, WebAddress.PingStatus AS Status
                FROM WebAddress
                INNER JOIN Department ON WebAddress.DepartmentID = Department.DepartmentID
                WHERE 1=1 {2}";

                string ipAddressFilter = buildingFilter;
                string deviceFilter = buildingFilter;
                string webAddressFilter = buildingFilter;

                if (StatusDropDown.SelectedValue != "Select Status")
                {
                    ipAddressFilter += " AND IPAddress.PingStatus = @PingStatus";
                    deviceFilter += " AND Device.PingStatus = @PingStatus";
                    webAddressFilter += " AND WebAddress.PingStatus = @PingStatus";
                }
                if (DepartmentDropDown.SelectedValue != "Select Location")
                {
                    ipAddressFilter += " AND Department.DepartmentName = @DepartmentName";
                    deviceFilter += " AND Department.DepartmentName = @DepartmentName";
                    webAddressFilter += " AND Department.DepartmentName = @DepartmentName";
                }

                string query = string.Format(baseQuery, ipAddressFilter, deviceFilter, webAddressFilter);

                MySqlCommand cmd = new MySqlCommand(query, con);

                if (StatusDropDown.SelectedValue != "Select Status")
                {
                    cmd.Parameters.AddWithValue("@PingStatus", StatusDropDown.SelectedValue);
                }
                if (DepartmentDropDown.SelectedValue != "Select Location")
                {
                    cmd.Parameters.AddWithValue("@DepartmentName", DepartmentDropDown.SelectedValue);
                }

                con.Open();
                MySqlDataReader rdr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(rdr);

                Dictionary<string, List<(string Address, string Name, string Status)>> departmentItems = new Dictionary<string, List<(string Address, string Name, string Status)>>();

                foreach (DataRow row in dt.Rows)
                {
                    string departmentName = row["DepartmentName"].ToString();
                    string address = row["Address"].ToString();
                    string name = row["Name"].ToString();
                    string status = row["Status"].ToString();

                    if (!departmentItems.ContainsKey(departmentName))
                    {
                        departmentItems[departmentName] = new List<(string Address, string Name, string Status)>();
                    }
                    departmentItems[departmentName].Add((address, name, status));
                }

                AllGridView1.Columns.Clear();
                AllGridView2.Columns.Clear();

                if (departmentItems.Count == 0)
                {
                    AllGridView1.DataSource = null;
                    AllGridView1.DataBind();

                    AllGridView2.DataSource = null;
                    AllGridView2.DataBind();

                    return;
                }

                var departmentList = departmentItems.Keys.ToList();
                int halfCount = (departmentList.Count + 1) / 2;

                var firstSet = departmentList.Take(halfCount).ToList();
                var secondSet = departmentList.Skip(halfCount).ToList();

                foreach (string departmentName in firstSet)
                {
                    TemplateField field = new TemplateField();
                    field.HeaderText = departmentName;
                    field.ItemTemplate = new AllStatusTemplate(departmentName);
                    AllGridView1.Columns.Add(field);
                }

                foreach (string departmentName in secondSet)
                {
                    TemplateField field = new TemplateField();
                    field.HeaderText = departmentName;
                    field.ItemTemplate = new AllStatusTemplate(departmentName);
                    AllGridView2.Columns.Add(field);
                }

                DataTable gridTable1 = new DataTable();
                DataTable gridTable2 = new DataTable();
                foreach (string departmentName in firstSet)
                {
                    gridTable1.Columns.Add(departmentName);
                }
                foreach (string departmentName in secondSet)
                {
                    gridTable2.Columns.Add(departmentName);
                }

                int maxRowCount1 = firstSet.Max(dept => departmentItems[dept].Count);
                int maxRowCount2 = secondSet.Count > 0 ? secondSet.Max(dept => departmentItems[dept].Count) : 0;

                for (int i = 0; i < maxRowCount1; i++)
                {
                    DataRow newRow1 = gridTable1.NewRow();
                    foreach (var entry in departmentItems)
                    {
                        string departmentName = entry.Key;
                        List<(string Address, string Name, string Status)> items = entry.Value;

                        if (i < items.Count && firstSet.Contains(departmentName))
                        {
                            var item = items[i];
                            newRow1[departmentName] = $"{item.Address}|{item.Name ?? string.Empty}|{item.Status}";
                        }
                        else if (firstSet.Contains(departmentName))
                        {
                            newRow1[departmentName] = string.Empty;
                        }
                    }
                    gridTable1.Rows.Add(newRow1);
                }

                for (int i = 0; i < maxRowCount2; i++)
                {
                    DataRow newRow2 = gridTable2.NewRow();
                    foreach (var entry in departmentItems)
                    {
                        string departmentName = entry.Key;
                        List<(string Address, string Name, string Status)> items = entry.Value;

                        if (i < items.Count && secondSet.Contains(departmentName))
                        {
                            var item = items[i];
                            newRow2[departmentName] = $"{item.Address}|{item.Name ?? string.Empty}|{item.Status}";
                        }
                        else if (secondSet.Contains(departmentName))
                        {
                            newRow2[departmentName] = string.Empty;
                        }
                    }
                    gridTable2.Rows.Add(newRow2);
                }

                AllGridView1.DataSource = gridTable1;
                AllGridView1.DataBind();

                if (secondSet.Count > 0)
                {
                    AllGridView2.DataSource = gridTable2;
                    AllGridView2.DataBind();
                }
            }

            // Additional logic for visibility and other specific cases
            if (CategoryDropDown.SelectedValue == "IP Address" && buildingFilter == "")
            {
                // Ensure IPGridView1 and IPGridView2 are not visible
                IPGridView1.Visible = false;
                IPGridView2.Visible = false;

                // Ensure AllGridView1 and AllGridView2 are visible
                AllGridView1.Visible = true;
                AllGridView2.Visible = true;
            }
            else
            {
                // Handle other visibility cases if needed
            }
        }
        public class AllStatusTemplate : ITemplate
        {
            private string _location;

            public AllStatusTemplate(string location)
            {
                _location = location;
            }

            public void InstantiateIn(Control container)
            {
                Label lbl = new Label();
                lbl.DataBinding += new EventHandler((sender, e) =>
                {
                    Label l = (Label)sender;
                    GridViewRow row = (GridViewRow)l.NamingContainer;
                    string[] data = DataBinder.Eval(row.DataItem, _location).ToString().Split('|');
                    if (data.Length == 3)
                    {
                        string address = data[0];
                        string name = data[1];
                        string status = data[2];
                        l.Text = GetStatusIndicator(status) + "" + FormatDeviceInfo(address,name,status);
                    }
                });
                container.Controls.Add(lbl);
            }

            protected string GetStatusIndicator(string status)
            {
                string color;
                switch (status.ToLower())
                {
                    case "healthy":
                        color = "#0AD17C";
                        break;
                    case "down":
                        color = "#FF0000";
                        break;
                    case "problematic":
                        color = "#E6A23C";
                        break;
                    default:
                        return string.Empty;
                }

                return $"<span class='status-indicator' style='color: {color};'>●</span>";
            }

            protected string FormatDeviceInfo(string address, string name, string status)
            {
                string cssClass;
                switch (status.ToLower())
                {
                    case "problematic":
                        cssClass = "status-box status-box-pending";
                        break;
                    case "down":
                        cssClass = "status-box status-box-rejected";
                        break;
                    case "healthy":
                        cssClass = "status-box status-box-healthy";
                        break;
                    default:
                        cssClass = "status-box";
                        break;
                }
                string formattedAddress = $"<span class='{cssClass}'>{address}</span>";
                string formattedName = $"<span class='{cssClass}'>{name}</span>";
                return $"<span class='{cssClass}'>{formattedAddress} ({formattedName})</span>";
            }
        }
        private void BindDeviceGrid(string buildingFilter)
        {
            using (MySqlConnection con = new MySqlConnection(GetConnectionString()))
            {
                string query = "SELECT Department.DepartmentName, DigitalDevIP, DigitalDevName, PingStatus FROM Device " +
                               "INNER JOIN Department ON Device.DepartmentID = Department.DepartmentID WHERE 1=1 " + buildingFilter;

                if (StatusDropDown.SelectedValue != "Select Status")
                {
                    query += " AND PingStatus = @PingStatus";
                }
                if (DepartmentDropDown.SelectedValue != "Select Location")
                {
                    query += " AND Department.DepartmentName = @DepartmentName";
                }

                MySqlCommand cmd = new MySqlCommand(query, con);

                if (StatusDropDown.SelectedValue != "Select Status")
                {
                    cmd.Parameters.AddWithValue("@PingStatus", StatusDropDown.SelectedValue);
                }
                if (DepartmentDropDown.SelectedValue != "Select Location")
                {
                    cmd.Parameters.AddWithValue("@DepartmentName", DepartmentDropDown.SelectedValue);
                }

                con.Open();
                MySqlDataReader rdr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(rdr);

                Dictionary<string, List<(string DigitalDevIP, string DigitalDevName, string Status)>> departmentDevices = new Dictionary<string, List<(string DigitalDevIP, string DigitalDevName, string Status)>>();

                foreach (DataRow row in dt.Rows)
                {
                    string departmentName = row["DepartmentName"].ToString();
                    string digitalDevIP = row["DigitalDevIP"].ToString();
                    string digitalDevName = row["DigitalDevName"].ToString();
                    string status = row["PingStatus"].ToString();

                    if (!departmentDevices.ContainsKey(departmentName))
                    {
                        departmentDevices[departmentName] = new List<(string DigitalDevIP, string DigitalDevName, string Status)>();
                    }
                    departmentDevices[departmentName].Add((digitalDevIP, digitalDevName, status));
                }

                DeviceGridView.Columns.Clear();
                DeviceGridView2.Columns.Clear();

                if (departmentDevices.Count == 0)
                {
                    DeviceGridView.DataSource = null;
                    DeviceGridView.DataBind();

                    DeviceGridView2.DataSource = null;
                    DeviceGridView2.DataBind();

                    return;
                }

                var departmentList = departmentDevices.Keys.ToList();
                int halfCount = (departmentList.Count + 1) / 2;

                var firstSet = departmentList.Take(halfCount).ToList();
                var secondSet = departmentList.Skip(halfCount).ToList();

                foreach (string departmentName in firstSet)
                {
                    TemplateField field = new TemplateField();
                    field.HeaderText = departmentName;
                    field.ItemTemplate = new DeviceStatusTemplate(departmentName);
                    DeviceGridView.Columns.Add(field);
                }

                foreach (string departmentName in secondSet)
                {
                    TemplateField field = new TemplateField();
                    field.HeaderText = departmentName;
                    field.ItemTemplate = new DeviceStatusTemplate(departmentName);
                    DeviceGridView2.Columns.Add(field);
                }

                DataTable gridTable1 = new DataTable();
                DataTable gridTable2 = new DataTable();
                foreach (string departmentName in firstSet)
                {
                    gridTable1.Columns.Add(departmentName);
                }
                foreach (string departmentName in secondSet)
                {
                    gridTable2.Columns.Add(departmentName);
                }

                int maxRowCount1 = firstSet.Max(dept => departmentDevices[dept].Count);
                int maxRowCount2 = secondSet.Count > 0 ? secondSet.Max(dept => departmentDevices[dept].Count) : 0;

                for (int i = 0; i < maxRowCount1; i++)
                {
                    DataRow newRow1 = gridTable1.NewRow();
                    foreach (var entry in departmentDevices)
                    {
                        string departmentName = entry.Key;
                        List<(string DigitalDevIP, string DigitalDevName, string Status)> devices = entry.Value;

                        if (i < devices.Count && firstSet.Contains(departmentName))
                        {
                            newRow1[departmentName] = devices[i].DigitalDevIP + "|" + devices[i].DigitalDevName + "|" + devices[i].Status;
                        }
                        else if (firstSet.Contains(departmentName))
                        {
                            newRow1[departmentName] = string.Empty;
                        }
                    }
                    gridTable1.Rows.Add(newRow1);
                }

                for (int i = 0; i < maxRowCount2; i++)
                {
                    DataRow newRow2 = gridTable2.NewRow();
                    foreach (var entry in departmentDevices)
                    {
                        string departmentName = entry.Key;
                        List<(string DigitalDevIP, string DigitalDevName, string Status)> devices = entry.Value;

                        if (i < devices.Count && secondSet.Contains(departmentName))
                        {
                            newRow2[departmentName] = devices[i].DigitalDevIP + "|" + devices[i].DigitalDevName + "|" + devices[i].Status;
                        }
                        else if (secondSet.Contains(departmentName))
                        {
                            newRow2[departmentName] = string.Empty;
                        }
                    }
                    gridTable2.Rows.Add(newRow2);
                }

                DeviceGridView.DataSource = gridTable1;
                DeviceGridView.DataBind();

                if (secondSet.Count > 0)
                {
                    DeviceGridView2.DataSource = gridTable2;
                    DeviceGridView2.DataBind();
                }
            }
        }

        public class DeviceStatusTemplate : ITemplate
        {
            private string _location;

            public DeviceStatusTemplate(string location)
            {
                _location = location;
            }

            public void InstantiateIn(Control container)
            {
                Label lbl = new Label();
                lbl.DataBinding += new EventHandler((sender, e) =>
                {
                    Label l = (Label)sender;
                    GridViewRow row = (GridViewRow)l.NamingContainer;
                    string[] data = DataBinder.Eval(row.DataItem, _location).ToString().Split('|');
                    if (data.Length == 3)
                    {
                        string digitalDevIP = data[0];
                        string digitalDevName = data[1];
                        string status = data[2];
                        l.Text = GetStatusIndicator(status) + "" + FormatDeviceInfo(digitalDevIP,digitalDevName,status);
                    }
                });
                container.Controls.Add(lbl);
            }

            protected string GetStatusIndicator(string status)
            {
                string color;
                switch (status)
                {
                    case "Healthy":
                        color = "#0AD17C";
                        break;
                    case "Down":
                        color = "#FF0000";
                        break;
                    case "Problematic":
                        color = "#E6A23C";
                        break;
                    default:
                        return string.Empty;
                }

                return $"<span class='status-indicator' style='color: {color};'>●</span>";
            }

            protected string FormatDeviceInfo(string ipAddress, string deviceName, string status)
            {
                string cssClass;
                switch (status)
                {
                    case "Problematic":
                        cssClass = "status-box status-box-pending";
                        break;
                    case "Down":
                        cssClass = "status-box status-box-rejected";
                        break;
                    case "Healthy":
                        cssClass = "status-box status-box-healthy";
                        break;
                    default:
                        cssClass = "status-box";
                        break;
                }
                string formattedIPAddress = $"<span class='{cssClass}'>{ipAddress}</span>";
                string formattedDeviceName = $"<span class='{cssClass}'>{deviceName}</span>";
                return $"{formattedIPAddress} <span class='{cssClass}'>({formattedDeviceName})</span>";
            }

            protected string GetLegend(string status)
            {
                string indicator = GetStatusIndicator(status);
                return $"<span class='legend'>{indicator} {status}</span>";
            }
        }
        private void BindWebGrid(string buildingFilter)
        {
            using (MySqlConnection con = new MySqlConnection(GetConnectionString()))
            {
                string query = "SELECT Department.DepartmentName, WebAddress, PingStatus FROM WebAddress " +
                               "INNER JOIN Department ON WebAddress.DepartmentID = Department.DepartmentID " +
                               "WHERE 1=1 " + buildingFilter;

                // Status filter
                if (StatusDropDown.SelectedValue != "Select Status")
                {
                    query += " AND PingStatus = @PingStatus";
                }

                // Department filter
                if (DepartmentDropDown.SelectedValue != "Select Location")
                {
                    query += " AND Department.DepartmentName = @DepartmentName";
                }

                MySqlCommand cmd = new MySqlCommand(query, con);

                // Add Status parameter
                if (StatusDropDown.SelectedValue != "Select Status")
                {
                    cmd.Parameters.AddWithValue("@PingStatus", StatusDropDown.SelectedValue);
                }

                // Add Department parameter
                if (DepartmentDropDown.SelectedValue != "Select Location")
                {
                    cmd.Parameters.AddWithValue("@DepartmentName", DepartmentDropDown.SelectedValue);
                }

                con.Open();
                MySqlDataReader rdr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(rdr);

                Dictionary<string, List<(string WebAddress, string Status)>> departmentWebs = new Dictionary<string, List<(string WebAddress, string Status)>>();

                foreach (DataRow row in dt.Rows)
                {
                    string departmentName = row["DepartmentName"].ToString();
                    string webAddress = row["WebAddress"].ToString();
                    string status = row["PingStatus"].ToString();

                    if (!departmentWebs.ContainsKey(departmentName))
                    {
                        departmentWebs[departmentName] = new List<(string WebAddress, string Status)>();
                    }
                    departmentWebs[departmentName].Add((webAddress, status));
                }

                if (departmentWebs.Count == 0)
                {
                    WebGridView.Columns.Clear();
                    WebGridView.DataSource = null;
                    WebGridView.DataBind();

                    WebGridView2.Columns.Clear();
                    WebGridView2.DataSource = null;
                    WebGridView2.DataBind();
                }
                else
                {
                    var departmentList = departmentWebs.Keys.ToList();
                    int halfCount = (departmentList.Count + 1) / 2; // Split the columns evenly

                    var firstSet = departmentList.Take(halfCount).ToList();
                    var secondSet = departmentList.Skip(halfCount).ToList();

                    WebGridView.Columns.Clear();
                    foreach (string departmentName in firstSet)
                    {
                        TemplateField field = new TemplateField();
                        field.HeaderText = departmentName;
                        field.ItemTemplate = new WebStatusTemplate(departmentName);
                        WebGridView.Columns.Add(field);
                    }

                    WebGridView2.Columns.Clear();
                    foreach (string departmentName in secondSet)
                    {
                        TemplateField field = new TemplateField();
                        field.HeaderText = departmentName;
                        field.ItemTemplate = new WebStatusTemplate(departmentName);
                        WebGridView2.Columns.Add(field);
                    }

                    DataTable gridTable1 = new DataTable();
                    DataTable gridTable2 = new DataTable();
                    foreach (string departmentName in firstSet)
                    {
                        gridTable1.Columns.Add(departmentName);
                    }
                    foreach (string departmentName in secondSet)
                    {
                        gridTable2.Columns.Add(departmentName);
                    }

                    int maxRowCount1 = firstSet.Max(dept => departmentWebs[dept].Count);
                    int maxRowCount2 = secondSet.Count > 0 ? secondSet.Max(dept => departmentWebs[dept].Count) : 0;

                    for (int i = 0; i < maxRowCount1; i++)
                    {
                        DataRow newRow1 = gridTable1.NewRow();
                        foreach (var entry in departmentWebs)
                        {
                            string departmentName = entry.Key;
                            List<(string WebAddress, string Status)> webs = entry.Value;

                            if (i < webs.Count && firstSet.Contains(departmentName))
                            {
                                newRow1[departmentName] = webs[i].WebAddress + "|" + webs[i].Status;
                            }
                            else if (firstSet.Contains(departmentName))
                            {
                                newRow1[departmentName] = string.Empty;
                            }
                        }
                        gridTable1.Rows.Add(newRow1);
                    }

                    for (int i = 0; i < maxRowCount2; i++)
                    {
                        DataRow newRow2 = gridTable2.NewRow();
                        foreach (var entry in departmentWebs)
                        {
                            string departmentName = entry.Key;
                            List<(string WebAddress, string Status)> webs = entry.Value;

                            if (i < webs.Count && secondSet.Contains(departmentName))
                            {
                                newRow2[departmentName] = webs[i].WebAddress + "|" + webs[i].Status;
                            }
                            else if (secondSet.Contains(departmentName))
                            {
                                newRow2[departmentName] = string.Empty;
                            }
                        }
                        gridTable2.Rows.Add(newRow2);
                    }

                    WebGridView.DataSource = gridTable1;
                    WebGridView.DataBind();

                    if (secondSet.Count > 0)
                    {
                        WebGridView2.DataSource = gridTable2;
                        WebGridView2.DataBind();
                    }
                }
            }
        }

        public class WebStatusTemplate : ITemplate
        {
            private string _location;

            public WebStatusTemplate(string location)
            {
                _location = location;
            }

            public void InstantiateIn(Control container)
            {
                Label lbl = new Label();
                lbl.DataBinding += new EventHandler((sender, e) =>
                {
                    Label l = (Label)sender;
                    GridViewRow row = (GridViewRow)l.NamingContainer;
                    string[] data = DataBinder.Eval(row.DataItem, _location).ToString().Split('|');
                    if (data.Length == 2)
                    {
                        string webAddress = data[0];
                        string status = data[1];
                        l.Text = GetStatusIndicator(status) + "" + FormatWebAddress(webAddress,status);
                    }
                });
                container.Controls.Add(lbl);
            }

            protected string GetStatusIndicator(string status)
            {
                string color;
                switch (status)
                {
                    case "Healthy":
                        color = "#0AD17C";
                        break;
                    case "Down":
                        color = "#FF0000";
                        break;
                    case "Problematic":
                        color = "#E6A23C";
                        break;
                    default:
                        return string.Empty;
                }

                return $"<span class='status-indicator' style='color: {color};'>●</span>";
            }

            protected string FormatWebAddress(string webAddress, string status)
            {
                string cssClass;
                switch (status)
                {
                    case "Problematic":
                        cssClass = "status-box status-box-pending";
                        break;
                    case "Down":
                        cssClass = "status-box status-box-rejected";
                        break;
                    case "Healthy":
                        cssClass = "status-box status-box-healthy";
                        break;
                    default:
                        cssClass = "status-box";
                        break;
                }
                return $"<span class='{cssClass}'>{webAddress}</span>";
            }

            protected string GetLegend(string status)
            {
                string indicator = GetStatusIndicator(status);
                return $"<span class='legend'>{indicator} {status}</span>";
            }
        }
        private void BindIPGrid(string buildingFilter)
        {
            using (MySqlConnection con = new MySqlConnection(GetConnectionString()))
            {
                string query = "SELECT Department.DepartmentName, IPAddress, PingStatus, Description FROM IPAddress " +
                               "INNER JOIN Department ON IPAddress.DepartmentID = Department.DepartmentID " +
                               "WHERE 1=1 " + buildingFilter;

                // Status filter
                if (StatusDropDown.SelectedValue != "Select Status")
                {
                    query += " AND PingStatus = @PingStatus";
                }

                // Department filter
                if (DepartmentDropDown.SelectedValue != "Select Location")
                {
                    query += " AND Department.DepartmentName = @DepartmentName";
                }

                MySqlCommand cmd = new MySqlCommand(query, con);

                // Add Status parameter
                if (StatusDropDown.SelectedValue != "Select Status")
                {
                    cmd.Parameters.AddWithValue("@PingStatus", StatusDropDown.SelectedValue);
                }

                // Add Department parameter
                if (DepartmentDropDown.SelectedValue != "Select Location")
                {
                    cmd.Parameters.AddWithValue("@DepartmentName", DepartmentDropDown.SelectedValue);
                }

                con.Open();
                MySqlDataReader rdr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(rdr);

                Dictionary<string, List<(string IPAddress, string Status, string Description)>> departmentIPs = new Dictionary<string, List<(string IPAddress, string Status, string Description)>>();

                foreach (DataRow row in dt.Rows)
                {
                    string departmentName = row["DepartmentName"].ToString();
                    string ipAddress = row["IPAddress"].ToString();
                    string status = row["PingStatus"].ToString();
                    string description = row["Description"].ToString();

                    if (!departmentIPs.ContainsKey(departmentName))
                    {
                        departmentIPs[departmentName] = new List<(string IPAddress, string Status, string Description)>();
                    }
                    departmentIPs[departmentName].Add((ipAddress, status, description));
                }

                if (departmentIPs.Count == 0)
                {
                    IPGridView1.Columns.Clear();
                    IPGridView1.DataSource = null;
                    IPGridView1.DataBind();

                    IPGridView2.Columns.Clear();
                    IPGridView2.DataSource = null;
                    IPGridView2.DataBind();
                }
                else
                {
                    var departmentList = departmentIPs.Keys.ToList();
                    int halfCount = (departmentList.Count + 1) / 2; // Split the columns evenly

                    var firstSet = departmentList.Take(halfCount).ToList();
                    var secondSet = departmentList.Skip(halfCount).ToList();

                    IPGridView1.Columns.Clear();
                    foreach (string departmentName in firstSet)
                    {
                        TemplateField field = new TemplateField();
                        field.HeaderText = departmentName;
                        field.ItemTemplate = new StatusTemplate(departmentName);
                        IPGridView1.Columns.Add(field);
                    }

                    IPGridView2.Columns.Clear();
                    foreach (string departmentName in secondSet)
                    {
                        TemplateField field = new TemplateField();
                        field.HeaderText = departmentName;
                        field.ItemTemplate = new StatusTemplate(departmentName);
                        IPGridView2.Columns.Add(field);
                    }

                    DataTable gridTable1 = new DataTable();
                    DataTable gridTable2 = new DataTable();
                    foreach (string departmentName in firstSet)
                    {
                        gridTable1.Columns.Add(departmentName);
                    }
                    foreach (string departmentName in secondSet)
                    {
                        gridTable2.Columns.Add(departmentName);
                    }

                    int maxRowCount1 = firstSet.Max(dept => departmentIPs[dept].Count);
                    int maxRowCount2 = secondSet.Count > 0 ? secondSet.Max(dept => departmentIPs[dept].Count) : 0;

                    for (int i = 0; i < maxRowCount1; i++)
                    {
                        DataRow newRow1 = gridTable1.NewRow();
                        foreach (var entry in departmentIPs)
                        {
                            string departmentName = entry.Key;
                            List<(string IPAddress, string Status, string Description)> ips = entry.Value;

                            if (i < ips.Count && firstSet.Contains(departmentName))
                            {
                                newRow1[departmentName] = ips[i].IPAddress + "|" + ips[i].Status + "|" + ips[i].Description;
                            }
                            else if (firstSet.Contains(departmentName))
                            {
                                newRow1[departmentName] = string.Empty;
                            }
                        }
                        gridTable1.Rows.Add(newRow1);
                    }

                    for (int i = 0; i < maxRowCount2; i++)
                    {
                        DataRow newRow2 = gridTable2.NewRow();
                        foreach (var entry in departmentIPs)
                        {
                            string departmentName = entry.Key;
                            List<(string IPAddress, string Status, string Description)> ips = entry.Value;

                            if (i < ips.Count && secondSet.Contains(departmentName))
                            {
                                newRow2[departmentName] = ips[i].IPAddress + "|" + ips[i].Status + "|" + ips[i].Description;
                            }
                            else if (secondSet.Contains(departmentName))
                            {
                                newRow2[departmentName] = string.Empty;
                            }
                        }
                        gridTable2.Rows.Add(newRow2);
                    }

                    IPGridView1.DataSource = gridTable1;
                    IPGridView1.DataBind();

                    if (secondSet.Count > 0)
                    {
                        IPGridView2.DataSource = gridTable2;
                        IPGridView2.DataBind();
                    }
                }
            }
        }

        protected void refresh_Click(object sender, EventArgs e)
        {
            Response.Redirect("load.aspx?target=location.aspx");
        }

        protected void devicesbtn_CheckedChanged(object sender, EventArgs e)
        {
            Response.Redirect("load.aspx?target=realtime.aspx");
        }
    }
    public class StatusTemplate : ITemplate
    {
        private string _location;

        public StatusTemplate(string location)
        {
            _location = location;
        }

        public void InstantiateIn(Control container)
        {
            Label lbl = new Label();
            lbl.DataBinding += new EventHandler((sender, e) =>
            {
                Label l = (Label)sender;
                GridViewRow row = (GridViewRow)l.NamingContainer;
                string[] data = DataBinder.Eval(row.DataItem, _location).ToString().Split('|');
                if (data.Length == 3)
                {
                    string ipAddress = data[0];
                    string status = data[1];
                    string desc = data[2];

                    l.Text = GetStatusIndicator(status) + "" + FormatIPAddress(ipAddress,status) + "" + FormatDescription(desc,status);
                }
            });
            container.Controls.Add(lbl);
        }

        protected string GetStatusIndicator(string status)
        {
            string color;
            switch (status.ToLower())
            {
                case "healthy":
                    color = "#0AD17C";
                    break;
                case "down":
                    color = "#FF0000";
                    break;
                case "problematic":
                    color = "#E6A23C";
                    break;
                default:
                    return string.Empty;
            }

            return $"<span class='status-indicator' style='color: {color};'>●</span>";
        }

        protected string FormatIPAddress(string ipAddress, string status)
        {
            string cssClass;
            switch (status.ToLower())
            {
                case "healthy":
                    cssClass = "status-box status-box-healthy";
                    break;
                case "down":
                    cssClass = "status-box status-box-rejected";
                    break;
                case "problematic":
                    cssClass = "status-box status-box-pending";
                    break;
                default:
                    cssClass = "status-box";
                    break;
            }
            return $"<span class='{cssClass}'>{ipAddress}</span>";
        }

        protected string FormatDescription(string desc, string status)
        {
            string cssClass;
            switch (status.ToLower())
            {
                case "healthy":
                    cssClass = "status-box status-box-healthy";
                    break;
                case "down":
                    cssClass = "status-box status-box-rejected";
                    break;
                case "problematic":
                    cssClass = "status-box status-box-pending";
                    break;
                default:
                    cssClass = "status-box";
                    break;
            }
            return $"<span class='{cssClass}'>({desc})</span>";
        }

        protected string GetLegend(string status)
        {
            string indicator = GetStatusIndicator(status);
            return $"<span class='legend'>{indicator} {status}</span>";
        }
    }
}