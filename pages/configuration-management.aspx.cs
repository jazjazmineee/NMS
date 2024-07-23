using ClosedXML.Excel;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NMS.pages
{
    public partial class configuration_management : BasePage
    {
        protected System.Web.UI.WebControls.Repeater PageRepeater;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Populate dropdown lists only on the first load
                PopulateDepartmentDropDown();
                PopulateBuildingDropDown();
                PopulateDepartmentBuildingDropDown();
                PopulateUpdateBuildingDropDown();
                PopulateDropDownLists();
                DropDownList1.SelectedValue = "showall";

                BindGridView(); // Bind the GridView with data
                BindPageSummary1();
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
        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearSearchTextBox();
            DepartmentGridView.Visible = false;
            BuildingGridView.Visible = false;
            BindGridBasedOnDropDown();
        }

        protected void DropDownList2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearSearchTextBox();
            int pageSize = int.Parse(EntriesDropdown.SelectedValue);

            // Hide other GridView controls based on the selected value in DropDownList1
            if (DropDownList1.SelectedValue == "showall" && DropDownList3.SelectedValue == "showall")
            {
                // If none of the options are selected, hide all GridView controls
                GridView1.Visible = true;
                GridView2.Visible = false;
                GridView3.Visible = false;
                GridView4.Visible = false;
                DepartmentGridView.Visible = false;
                BuildingGridView.Visible = false;
                GridView1.PageSize = pageSize;
                BindGridView();
            }
            else if (DropDownList1.SelectedValue == "Digital Device")
            {
                GridView1.Visible = false;
                GridView2.Visible = true;
                GridView3.Visible = false;
                GridView4.Visible = false;
                DepartmentGridView.Visible = false;
                BuildingGridView.Visible = false;
                GridView2.PageSize = pageSize;
                BindDeviceGridView();
            }
            else if (DropDownList1.SelectedValue == "IP Address")
            {
                GridView1.Visible = false;
                GridView2.Visible = false;
                GridView3.Visible = true;
                GridView4.Visible = false;
                DepartmentGridView.Visible = false;
                BuildingGridView.Visible = false;
                GridView3.PageSize = pageSize;
                BindIPGridView();
            }
            else if (DropDownList1.SelectedValue == "Web Address")
            {
                GridView1.Visible = false;
                GridView2.Visible = false;
                GridView3.Visible = false;
                GridView4.Visible = true;
                DepartmentGridView.Visible = false;
                BuildingGridView.Visible = false;
                GridView4.PageSize = pageSize;
                BindWebGridView();
            }
            else if (DropDownList3.SelectedValue == "Departments" && DropDownList1.SelectedValue == "showall")
            {
                GridView1.Visible = false;
                GridView2.Visible = false;
                GridView3.Visible = false;
                GridView4.Visible = false;
                DepartmentGridView.Visible = true;
                BuildingGridView.Visible = false;
                DepartmentGridView.PageSize = pageSize;
                BindDepartmentGridView();
            }
            else if (DropDownList3.SelectedValue == "Buildings" && DropDownList1.SelectedValue == "showall")
            {
                GridView1.Visible = false;
                GridView2.Visible = false;
                GridView3.Visible = false;
                GridView4.Visible = false;
                DepartmentGridView.Visible = false;
                BuildingGridView.Visible = true;
                BuildingGridView.PageSize = pageSize;
                BindBuildingGridView();
            }
        }

        private void ClearSearchTextBox()
        {
            searchtxtbox.Text = string.Empty;
        }
        protected void searchtxtbox_TextChanged(object sender, EventArgs e)
        {
            string searchQuery = searchtxtbox.Text.Trim();
            string gridview = "";

            if (DropDownList3.SelectedValue == "showall")
            {
                if (DropDownList1.SelectedValue == "showall")
                {
                    gridview = "GridView1";
                }
                else if (DropDownList1.SelectedValue == "Digital Device")
                {
                    gridview = "GridView2";
                }
                else if (DropDownList1.SelectedValue == "IP Address")
                {
                    gridview = "GridView3";
                }
                else if (DropDownList1.SelectedValue == "Web Address")
                {
                    gridview = "GridView4";
                }
            }
            else if (DropDownList3.SelectedValue == "Department")
            {
                gridview = "DepartmentGridView";
            }
            else if (DropDownList3.SelectedValue == "Building")
            {
                gridview = "BuildingGridView";
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                SearchGridView(searchQuery, gridview);
            }
            else
            {
                BindGridView();
            }
        }

        private void SearchGridView(string searchQuery, string gridview)
        {
            string connectionString = GetConnectionString();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string sql = "";

                if (gridview == "GridView1")
                {
                    sql = @"
                    SELECT device.DeviceTypeID AS Type, device.DigitalDevName AS Name, device.DigitalDevIP AS Address, 
                           NULL AS BuildingID, device.RefreshInterval, device.IntervalUnit AS IntervalUnit, 
                           NULL AS Description
                    FROM device
                    WHERE device.DigitalDevName LIKE @searchQuery OR device.DigitalDevIP LIKE @searchQuery OR 
                          device.RefreshInterval LIKE @searchQuery OR device.IntervalUnit LIKE @searchQuery
                    UNION ALL
                    SELECT ipaddress.DeviceTypeID AS Type, department.DepartmentName AS Name, ipaddress.IPAddress AS Address, 
                           ipaddress.Description, building.BuildingName AS Building, ipaddress.RefreshInterval, ipaddress.IntervalUnit AS IntervalUnit
                    FROM ipaddress
                    JOIN department ON ipaddress.DepartmentID = department.DepartmentID
                    JOIN building ON ipaddress.BuildingID = building.BuildingID
                    WHERE department.DepartmentName LIKE @searchQuery OR ipaddress.IPAddress LIKE @searchQuery OR 
                          ipaddress.Description LIKE @searchQuery OR building.BuildingName LIKE @searchQuery OR 
                          ipaddress.RefreshInterval LIKE @searchQuery OR ipaddress.IntervalUnit LIKE @searchQuery
                    UNION ALL
                    SELECT webaddress.DeviceTypeID AS Type, webaddress.WebAddress AS Name, webaddress.WebAddress AS Address, 
                           NULL AS BuildingID, webaddress.RefreshInterval, webaddress.IntervalUnit AS IntervalUnit, 
                           NULL AS Description
                    FROM webaddress
                    WHERE webaddress.WebAddress LIKE @searchQuery OR webaddress.RefreshInterval LIKE @searchQuery OR 
                          webaddress.IntervalUnit LIKE @searchQuery";
                }

                else if (gridview == "GridView2")
                {
                    sql = @"
                    SELECT device.DigitalDevID, device.DeviceTypeID AS Type, device.DigitalDevName AS Name, device.DigitalDevIP AS Address, device.RefreshInterval, device.IntervalUnit
                    FROM device
                    WHERE device.DigitalDevName LIKE @searchQuery OR device.DigitalDevIP LIKE @searchQuery OR device.RefreshInterval LIKE @searchQuery OR device.IntervalUnit LIKE @searchQuery";
                }
                else if (gridview == "GridView3")
                {
                    sql = @"
                    SELECT ipaddress.IPAddressID, ipaddress.DeviceTypeID AS Type, department.DepartmentName AS Department, 
                           ipaddress.IPAddress AS IPAddress, building.BuildingName AS Building, 
                           ipaddress.RefreshInterval, ipaddress.IntervalUnit AS IntervalUnit, 
                           ipaddress.Description, ipaddress.DepartmentID
                    FROM ipaddress
                    INNER JOIN department ON ipaddress.DepartmentID = department.DepartmentID
                    INNER JOIN building ON ipaddress.BuildingID = building.BuildingID
                    WHERE department.DepartmentName LIKE @searchQuery OR ipaddress.IPAddress LIKE @searchQuery OR 
                          building.BuildingName LIKE @searchQuery OR ipaddress.RefreshInterval LIKE @searchQuery OR 
                          ipaddress.IntervalUnit LIKE @searchQuery OR ipaddress.Description LIKE @searchQuery";
                }
                else if (gridview == "GridView4")
                {
                    sql = @"
                    SELECT webaddress.WebID, webaddress.DeviceTypeID AS Type, webaddress.WebAddress AS Name, webaddress.WebAddress AS Address, webaddress.RefreshInterval, webaddress.IntervalUnit
                    FROM webaddress
                    WHERE webaddress.WebAddress LIKE @searchQuery OR webaddress.RefreshInterval LIKE @searchQuery OR webaddress.IntervalUnit LIKE @searchQuery";
                }
                else if (gridview == "DepartmentGridView")
                {
                    sql = @"
                    SELECT d.DepartmentID, d.DepartmentName, d.BuildingID, b.BuildingName
                    FROM department d
                    INNER JOIN building b ON d.BuildingID = b.BuildingID
                    WHERE d.DepartmentName LIKE @searchQuery OR b.BuildingName LIKE @searchQuery";
                }
                else if (gridview == "BuildingGridView")
                {
                    sql = "SELECT BuildingID, BuildingName FROM building WHERE BuildingName LIKE @searchQuery";
                }

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@searchQuery", "%" + searchQuery + "%");
                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        if (gridview == "GridView1")
                        {
                            GridView1.DataSource = dt;
                            GridView1.DataBind();
                        }
                        else if (gridview == "GridView2")
                        {
                            GridView2.DataSource = dt;
                            GridView2.DataBind();
                        }
                        else if (gridview == "GridView3")
                        {
                            GridView3.DataSource = dt;
                            GridView3.DataBind();
                        }
                        else if (gridview == "GridView4")
                        {
                            GridView4.DataSource = dt;
                            GridView4.DataBind();
                        }
                        else if (gridview == "DepartmentGridView")
                        {
                            DepartmentGridView.DataSource = dt;
                            DepartmentGridView.DataBind();
                        }
                        else if (gridview == "BuildingGridView")
                        {
                            BuildingGridView.DataSource = dt;
                            BuildingGridView.DataBind();
                        }

                        UpdateResultsLabel(dt.Rows.Count);
                    }
                }
            }
        }
        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                int newPageIndex = e.NewPageIndex;
                DataTable dt = ViewState["GridData"] as DataTable;

                if (dt != null)
                {
                    int totalPages = (int)Math.Ceiling((double)dt.Rows.Count / GridView1.PageSize);

                    // Ensure the new page index is within valid bounds
                    if (newPageIndex >= 0 && newPageIndex < totalPages)
                    {
                        GridView1.PageIndex = newPageIndex;
                        GridView1.DataSource = dt;
                        GridView1.DataBind();
                        BindPageSummary1();
                    }
                    else
                    {
                        // Handle invalid page index, maybe log or ignore
                        Console.WriteLine("Invalid Page Index: " + newPageIndex);
                    }
                }
                else
                {
                    Console.WriteLine("ViewState is null or empty.");
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // Optionally log the error or display a message to the user
                Console.WriteLine("ArgumentOutOfRangeException: " + ex.Message);
                // Do nothing, leave it clickable
            }
        }

        private void BindPageSummary1()
        {
            if (GridView1.Rows.Count == 0) return; // No data, exit method

            int pageSize = GridView1.PageSize;
            int currentPage = GridView1.PageIndex + 1;
            int totalRecords = 0;

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

        protected void GridView2_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                GridView2.PageIndex = e.NewPageIndex;
                BindDeviceGridView();
                BindPageSummary2();
            }
            catch (ArgumentOutOfRangeException)
            {
                // Do nothing, leave it clickable
            }
        }

        private void BindPageSummary2()
        {
            if (GridView2.Rows.Count == 0) return; // No data, exit method

            int pageSize = GridView2.PageSize;
            int currentPage = GridView2.PageIndex + 1;
            int totalRecords = 0;

            // Check if the DataSource is not null and is of type DataTable
            if (GridView2.DataSource is DataTable dataTable)
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

            if (GridView2.BottomPagerRow != null)
            {
                Label PageSummary = GridView2.BottomPagerRow.FindControl("PageSummary") as Label;
                if (PageSummary != null)
                {
                    PageSummary.Text = $"Showing {startRecord} to {endRecord} of {totalRecords} entries";
                }
            }
        }

        protected void GridView3_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                GridView3.PageIndex = e.NewPageIndex;
                BindIPGridView();
                BindPageSummary3();
            }
            catch (ArgumentOutOfRangeException)
            {
                // Do nothing, leave it clickable
            }
        }

        private void BindPageSummary3()
        {
            if (GridView3.Rows.Count == 0) return; // No data, exit method

            int pageSize = GridView3.PageSize;
            int currentPage = GridView3.PageIndex + 1;
            int totalRecords = 0;

            // Check if the DataSource is not null and is of type DataTable
            if (GridView3.DataSource is DataTable dataTable)
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

            if (GridView3.BottomPagerRow != null)
            {
                Label PageSummary = GridView3.BottomPagerRow.FindControl("PageSummary") as Label;
                if (PageSummary != null)
                {
                    PageSummary.Text = $"Showing {startRecord} to {endRecord} of {totalRecords} entries";
                }
            }
        }

        protected void GridView4_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                GridView4.PageIndex = e.NewPageIndex;
                BindWebGridView();
                BindPageSummary4();
            }
            catch (ArgumentOutOfRangeException)
            {
                // Do nothing, leave it clickable
            }
        }

        private void BindPageSummary4()
        {
            int pageSize = GridView4.PageSize;
            int currentPage = GridView4.PageIndex + 1;
            int totalRecords = GridView4.DataSource as DataTable != null ? (GridView4.DataSource as DataTable).Rows.Count : 0;
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            int startRecord = (currentPage - 1) * pageSize + 1;
            int endRecord = currentPage * pageSize;
            if (endRecord > totalRecords)
            {
                endRecord = totalRecords;
            }

            Label PageSummary = GridView4.BottomPagerRow.FindControl("PageSummary") as Label;
            if (PageSummary != null)
            {
                PageSummary.Text = $"Showing {startRecord} to {endRecord} of {totalRecords} entries";
            }
        }

        protected void DepartmentGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                DepartmentGridView.PageIndex = e.NewPageIndex;
                BindDepartmentGridView();
                BindPageSummary5();
            }
            catch (ArgumentOutOfRangeException)
            {
                // Do nothing, leave it clickable
            }
        }

        private void BindPageSummary5()
        {
            if (DepartmentGridView.Rows.Count == 0) return; // No data, exit method

            int pageSize = DepartmentGridView.PageSize;
            int currentPage = DepartmentGridView.PageIndex + 1;
            int totalRecords = 0;

            // Check if the DataSource is not null and is of type DataTable
            if (DepartmentGridView.DataSource is DataTable dataTable)
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

            if (DepartmentGridView.BottomPagerRow != null)
            {
                Label PageSummary = DepartmentGridView.BottomPagerRow.FindControl("PageSummary") as Label;
                if (PageSummary != null)
                {
                    PageSummary.Text = $"Showing {startRecord} to {endRecord} of {totalRecords} entries";
                }
            }
        }
        protected void BuildingGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                BuildingGridView.PageIndex = e.NewPageIndex;
                BindBuildingGridView();
                BindPageSummary6();
            }
            catch (ArgumentOutOfRangeException)
            {
                // Do nothing, leave it clickable
            }
        }

        private void BindPageSummary6()
        {
            if (BuildingGridView.Rows.Count == 0) return; // No data, exit method

            int pageSize = BuildingGridView.PageSize;
            int currentPage = BuildingGridView.PageIndex + 1;
            int totalRecords = 0;

            // Check if the DataSource is not null and is of type DataTable
            if (BuildingGridView.DataSource is DataTable dataTable)
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

            if (BuildingGridView.BottomPagerRow != null)
            {
                Label PageSummary = BuildingGridView.BottomPagerRow.FindControl("PageSummary") as Label;
                if (PageSummary != null)
                {
                    PageSummary.Text = $"Showing {startRecord} to {endRecord} of {totalRecords} entries";
                }
            }
        }
        // GRID 
        protected void BindGridView()
        {
            string connectionString = GetConnectionString();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"
                SELECT device.DeviceTypeID AS Type, device.DigitalDevName AS Name, device.DigitalDevIP AS Address, device.BuildingID, device.RefreshInterval, device.IntervalUnit AS IntervalUnit
                FROM device
                UNION ALL
                SELECT ipaddress.DeviceTypeID AS Type, ipaddress.Description AS Name, ipaddress.IPAddress AS Address, ipaddress.BuildingID, ipaddress.RefreshInterval, ipaddress.IntervalUnit AS IntervalUnit
                FROM ipaddress
                JOIN department ON ipaddress.DepartmentID = department.DepartmentID
                UNION ALL
                SELECT webaddress.DeviceTypeID AS Type, webaddress.WebAddress AS Name, webaddress.WebAddress AS Address, webaddress.BuildingID, webaddress.RefreshInterval, webaddress.IntervalUnit AS IntervalUnit
                FROM webaddress";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // Save the DataTable in ViewState
                        ViewState["GridData"] = dt;

                        GridView1.DataSource = dt;
                        GridView1.DataBind();
                        BindPageSummary1();
                        UpdateResultsLabel(dt.Rows.Count);
                    }
                }
            }
        }

        //BIND 3 DEVICES
        private void BindDeviceGridView()
        {
            string connectionString = GetConnectionString();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT device.DigitalDevID, device.DeviceTypeID AS Type, device.DigitalDevName AS Name, device.DigitalDevIP AS Address, " +
                               "building.BuildingName AS Building, department.DepartmentName AS Department, " +
                               "device.RefreshInterval, device.IntervalUnit " +
                               "FROM device " +
                               "INNER JOIN building ON device.BuildingID = building.BuildingID " +
                               "INNER JOIN department ON device.DepartmentID = department.DepartmentID";
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable table = new DataTable();
                adapter.Fill(table);

                GridView2.DataSource = table;
                GridView2.DataBind();
                BindPageSummary2();
                UpdateResultsLabel(table.Rows.Count); // Assuming you have a method to update the results label
            }
        }

        private void BindIPGridView()
        {
            string connectionString = GetConnectionString();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT ipaddress.IPAddressID, ipaddress.DeviceTypeID AS Type, department.DepartmentName AS Department, " +
                               "ipaddress.IPAddress AS IPAddress, ipaddress.Description, building.BuildingName AS Building, " +
                               "ipaddress.RefreshInterval, ipaddress.IntervalUnit AS IntervalUnit, ipaddress.BuildingID, ipaddress.DepartmentID " +
                               "FROM ipaddress " +
                               "INNER JOIN department ON ipaddress.DepartmentID = department.DepartmentID " +
                               "INNER JOIN building ON ipaddress.BuildingID = building.BuildingID";
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable table = new DataTable();
                adapter.Fill(table);

                GridView3.DataSource = table; // Use GridView3 for IP Addresses
                GridView3.DataKeyNames = new string[] { "IPAddressID" }; // Set DataKeyNames to IPAddressID
                GridView3.DataBind();
                BindPageSummary3();
                UpdateResultsLabel(table.Rows.Count); // Assuming you have a method to update the results label
            }
        }
        private void BindWebGridView()
        {
            string connectionString = GetConnectionString();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT webaddress.WebID, webaddress.DeviceTypeID AS Type, webaddress.WebAddress AS Name, " +
                               "webaddress.WebAddress AS Address, building.BuildingName AS Building, department.DepartmentName AS Department, " +
                               "webaddress.RefreshInterval, webaddress.IntervalUnit " +
                               "FROM webaddress " +
                               "INNER JOIN building ON webaddress.BuildingID = building.BuildingID " +
                               "INNER JOIN department ON webaddress.DepartmentID = department.DepartmentID";
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable table = new DataTable();
                adapter.Fill(table);

                GridView4.DataSource = table;
                GridView4.DataBind();
                BindPageSummary4();
                UpdateResultsLabel(table.Rows.Count);
            }
        }

        protected void DropDownList3_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearSearchTextBox();
            if (!IsPostBack) return;

            string selectedValue = DropDownList3.SelectedValue;

            if (selectedValue == "showall")
            {
                DropDownList1.Visible = true;
                DepartmentGridView.Visible = false;
                BuildingGridView.Visible = false;
                BindGridBasedOnDropDown();
            }
            else if (selectedValue == "Department")
            {
                DepartmentGridView.Visible = true;
                BuildingGridView.Visible = false;
                DropDownList1.Visible = false;
                GridView1.Visible = false;
                GridView2.Visible = false;
                GridView3.Visible = false;
                GridView4.Visible = false;
                BindDepartmentGridView();
            }
            else if (selectedValue == "Building")
            {
                DepartmentGridView.Visible = false;
                BuildingGridView.Visible = true;
                DropDownList1.Visible = false;
                GridView1.Visible = false;
                GridView2.Visible = false;
                GridView3.Visible = false;
                GridView4.Visible = false;
                BindBuildingGridView();
            }
        }

        //2
        private void BindDepartmentGridView()
        {
            string connectionString = GetConnectionString();
            string query = "SELECT Department.DepartmentID, Department.DepartmentName, Building.BuildingName " +
                           "FROM Department " +
                           "INNER JOIN Building ON Department.BuildingID = Building.BuildingID";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, conn))
                {
                    conn.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    DepartmentGridView.DataSource = dt;
                    DepartmentGridView.DataBind();
                    BindPageSummary5();
                    UpdateResultsLabel(dt.Rows.Count);
                }
            }
        }

        private void BindBuildingGridView()
        {
            string connectionString = GetConnectionString();
            string query = "SELECT building.BuildingID, building.BuildingName FROM Building";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, conn))
                {
                    conn.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    BuildingGridView.DataSource = dt;
                    BuildingGridView.DataBind();
                    BindPageSummary6();
                    UpdateResultsLabel(dt.Rows.Count);
                }
            }
        }
        //
        protected void SaveUpdateBuilding(object sender, EventArgs e)
        {
            string buildingName = updateBuilding.Text;
            if (string.IsNullOrWhiteSpace(buildingName))
            {
                ShowError(buildingErrorLabel, "Building name cannot be empty.");
                return;
            }

            string connectionString = GetConnectionString();
            string buildingID = BuildingHiddenField.Value;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand command = new MySqlCommand();
                command.Connection = conn;

                command.CommandText = "UPDATE Building SET BuildingName=@BuildingName WHERE BuildingID=@BuildingID";
                command.Parameters.AddWithValue("@BuildingName", buildingName);
                command.Parameters.AddWithValue("@BuildingID", buildingID);
                command.ExecuteNonQuery();
            }

            BindBuildingGridView();
            PopulateUpdateBuildingDropDown();
            PopulateDepartmentBuildingDropDown();
            PopulateBuildingDropDown();
            PopulateDropDownLists();

            // Refresh the page after a delay (adjust as needed)
            ScriptManager.RegisterStartupScript(this, GetType(), "RefreshPage", "setTimeout(function() { location.reload(); }, 100);", true);
        }

        protected void SaveUpdateDepartment(object sender, EventArgs e)
        {
            string departmentName = updateDept.Text;
            if (string.IsNullOrWhiteSpace(departmentName))
            {
                ShowError(departmentErrorLabel, "Department name cannot be empty.");
                return;
            }

            string connectionString = GetConnectionString();
            string departmentID = DepartmentHiddenField.Value;
            string buildingID = updateBuildingDropDown.SelectedValue;

            if (buildingID != "")
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand command = new MySqlCommand();
                    command.Connection = conn;

                    command.CommandText = "UPDATE Department SET DepartmentName=@DepartmentName, BuildingID=@BuildingID WHERE DepartmentID=@DepartmentID";
                    command.Parameters.AddWithValue("@DepartmentName", departmentName);
                    command.Parameters.AddWithValue("@BuildingID", buildingID);
                    command.Parameters.AddWithValue("@DepartmentID", departmentID);
                    command.ExecuteNonQuery();
                }
            }
            else
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand command = new MySqlCommand();
                    command.Connection = conn;

                    command.CommandText = "UPDATE Department SET DepartmentName=@DepartmentName WHERE DepartmentID=@DepartmentID";
                    command.Parameters.AddWithValue("@DepartmentName", departmentName);
                    command.Parameters.AddWithValue("@DepartmentID", departmentID);
                    command.ExecuteNonQuery();
                }
            }

            BindDepartmentGridView();
            PopulateDropDownLists();
            PopulateDepartmentBuildingDropDown();
            PopulateDepartmentDropDown();

            // Refresh the page after a delay (adjust as needed)
            ScriptManager.RegisterStartupScript(this, GetType(), "RefreshPage", "setTimeout(function() { location.reload(); }, 100);", true);
        }


        private void PopulateUpdateBuildingDropDown()
        {
            string connectionString = GetConnectionString();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // Populate Building DropDownList
                string buildingSql = "SELECT BuildingID, BuildingName FROM Building";
                using (MySqlCommand cmd = new MySqlCommand(buildingSql, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        updateBuildingDropDown.Items.Clear();
                        updateBuildingDropDown.Items.Add(new ListItem("Change", "")); // Default value

                        while (reader.Read())
                        {
                            updateBuildingDropDown.Items.Add(new ListItem(reader["BuildingName"].ToString(), reader["BuildingID"].ToString()));
                        }
                    }
                }
            }
        }

        protected void UpdateBuildingDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void PopulateDepartmentDropDown()
        {
            string connectionString = GetConnectionString();
            string query = "SELECT DepartmentID, DepartmentName FROM Department";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        DepartmentDropDown.DataSource = dt;
                        DepartmentDropDown.DataTextField = "DepartmentName";
                        DepartmentDropDown.DataValueField = "DepartmentID";
                        DepartmentDropDown.DataBind();
                    }
                }
            }

            // Adding a default item
            DepartmentDropDown.Items.Insert(0, new ListItem("--Select Department--", "0"));
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
            BuildingDropDown.Items.Insert(0, new ListItem("--Select Building--", "0"));
        }
        private void PopulateDepartmentBuildingDropDown()
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
                        DepartmentBuildingDropDown.DataSource = dt;
                        DepartmentBuildingDropDown.DataTextField = "BuildingName";
                        DepartmentBuildingDropDown.DataValueField = "BuildingID";
                        DepartmentBuildingDropDown.DataBind();
                    }
                }
            }
        }
        protected void BuildingDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterDepartmentDropDown();
        }

        private void FilterDepartmentDropDown()
        {
            if (!string.IsNullOrEmpty(BuildingDropDown.SelectedValue) && BuildingDropDown.SelectedValue != "0")
            {
                string connectionString = GetConnectionString();
                string query = "SELECT DepartmentID, DepartmentName FROM Department WHERE BuildingID = @BuildingID";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@BuildingID", BuildingDropDown.SelectedValue);
                        using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);
                            DepartmentDropDown.DataSource = dt;
                            DepartmentDropDown.DataTextField = "DepartmentName";
                            DepartmentDropDown.DataValueField = "DepartmentID";
                            DepartmentDropDown.DataBind();
                        }
                    }
                }
            }
            else
            {
                PopulateDepartmentDropDown();
            }
        }

        private bool IsValidIPAddress(string ipAddress)
        {
            // Split the IP address into segments
            string[] segments = ipAddress.Split('.');

            // Check if the IP address has exactly 4 segments
            if (segments.Length != 4)
            {
                return false;
            }

            // Check if each segment is a valid number in the range 0-255
            foreach (string segment in segments)
            {
                if (!int.TryParse(segment, out int value) || value < 0 || value > 255)
                {
                    return false;
                }
            }

            // Check if the IP address ends with a dot, which is invalid
            if (ipAddress.EndsWith("."))
            {
                return false;
            }

            return true;
        }

        private bool IsValidWebAddress(string webAddress)
        {
            // Ensure the web address has a scheme, if not, add "http://" as default for validation
            if (!webAddress.StartsWith("http://") && !webAddress.StartsWith("https://"))
            {
                webAddress = "http://" + webAddress;
            }

            // Validate the web address using Uri.TryCreate and additional pattern matching
            if (Uri.TryCreate(webAddress, UriKind.Absolute, out Uri uriResult))
            {
                string pattern = @"^(https?:\/\/)?([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,6}(:\d{1,5})?(\/.*)?$";
                return Regex.IsMatch(uriResult.ToString(), pattern);
            }

            return false;
        }

        private string RemoveSchemeAndTrailingSlash(string webAddress)
        {
            Uri uri = new Uri(webAddress);
            string result = uri.Host + uri.PathAndQuery;
            return result.TrimEnd('/');
        }
        private void ClearErrorLabel()
        {
            errorLabel.Text = string.Empty;
            errorLabel.Visible = false;
        }
        protected void AddDevice_Click(object sender, EventArgs e)
        {
            ClearErrorLabel();

            string selectedCategory = Request.Form["category"]; // Get the selected radio button value

            if (selectedCategory == "addDevice")
            {
                string deviceType = DeviceTypeDropDown.SelectedValue;
                string intervalText = IntervalTextBox.Text;
                string intervalUnit = IntervalUnitDropDown.SelectedValue;

                // Validate IntervalTextBox
                int interval;
                if (!int.TryParse(intervalText, out interval) || interval <= 0)
                {
                    string errorMessage = "Interval must be a positive number.";
                    ShowError(errorMessage);
                    return;
                }

                // Additional validation for interval based on unit
                if ((intervalUnit == "seconds" || intervalUnit == "minutes") && interval > 60)
                {
                    string errorMessage = "Interval for seconds or minutes should not exceed 60.";
                    ShowError(errorMessage);
                    return;
                }
                else if (intervalUnit == "hours" && interval > 24)
                {
                    string errorMessage = "Interval for hours should not exceed 24.";
                    ShowError(errorMessage);
                    return;
                }

                string departmentID = DepartmentDropDown.SelectedValue;
                string buildingID = BuildingDropDown.SelectedValue;

                if (departmentID == "0" || buildingID == "0" || string.IsNullOrWhiteSpace(intervalText) || string.IsNullOrWhiteSpace(intervalUnit))
                {
                    string errorMessage = "All fields are required.";
                    ShowError(errorMessage);
                    return;
                }

                if (deviceType == "IP Address")
                {
                    string ipAddress = ipTextBox.Text;
                    string description = DescriptionTextbox.Text; // Assuming you have a TextBox for description

                    if (string.IsNullOrWhiteSpace(ipAddress) || string.IsNullOrWhiteSpace(description))
                    {
                        string errorMessage = "All fields are required.";
                        ShowError(errorMessage);
                        return;
                    }

                    AddIPAddress(deviceType, departmentID, ipAddress, buildingID, description, intervalText, intervalUnit);
                }
                else if (deviceType == "Web Address")
                {
                    string webAddress = webAddressTextBox.Text;

                    if (string.IsNullOrWhiteSpace(webAddress))
                    {
                        string errorMessage = "All fields are required.";
                        ShowError(errorMessage);
                        return;
                    }

                    AddWebAddress(deviceType, webAddress, departmentID, buildingID, intervalText, intervalUnit);
                }
                else if (deviceType == "Digital Device")
                {
                    string deviceName = deviceNameTextBox.Text;
                    string deviceIP = deviceIPTextBox.Text;

                    if (string.IsNullOrWhiteSpace(deviceName) || string.IsNullOrWhiteSpace(deviceIP))
                    {
                        string errorMessage = "All fields are required.";
                        ShowError(errorMessage);
                        return;
                    }

                    AddDeviceAddress(deviceType, deviceName, deviceIP, departmentID, buildingID, intervalText, intervalUnit);
                }
            }
            else if (selectedCategory == "department")
            {
                string department = AddDepartment.Text;

                if (string.IsNullOrWhiteSpace(department))
                {
                    string errorMessage = "Department name is required.";
                    ShowError(errorMessage);
                    return;
                }

                AddDepartmentAddress(department);
            }
            else if (selectedCategory == "building")
            {
                string building = AddBuilding.Text;

                if (string.IsNullOrWhiteSpace(building))
                {
                    string errorMessage = "Building name is required.";
                    ShowError(errorMessage);
                    return;
                }

                AddBuildingAddress(building);
            }
        }

        private void ShowError(string errorMessage)
        {
            errorLabel.Text = errorMessage;
            errorLabel.Visible = true;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "showErrorLabel", $"showErrorLabel('{errorMessage}');", true);
        }
        private void AddDepartmentAddress(string department)
        {
            string connectionString = GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string checkQuery = "SELECT COUNT(*) FROM Department WHERE DepartmentName = @DepartmentName";
                MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@DepartmentName", department);
                int existingCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                if (existingCount > 0)
                {
                    string errorMessage = "Department already exists.";
                    ShowError(errorMessage);
                    return;
                }

                // Retrieve the selected building ID from the DepartmentBuildingDropDown
                string buildingID = DepartmentBuildingDropDown.SelectedValue;

                string insertQuery = "INSERT INTO Department (DepartmentName, BuildingID) VALUES (@DepartmentName, @BuildingID)";
                MySqlCommand command = new MySqlCommand(insertQuery, connection);
                command.Parameters.AddWithValue("@DepartmentName", department);
                command.Parameters.AddWithValue("@BuildingID", buildingID);
                command.ExecuteNonQuery();

                ScriptManager.RegisterStartupScript(this, GetType(), "showToast", "showToast('Department added successfully.'); clearTextBoxes();", true);

                // Repopulate the dropdown to reflect the new department
                PopulateDepartmentDropDown();
                PopulateDepartmentBuildingDropDown();

                // Refresh the page after a delay (adjust as needed)
                ScriptManager.RegisterStartupScript(this, GetType(), "RefreshPage", "setTimeout(function() { location.reload(); }, 5000);", true);
            }
        }

        private void AddBuildingAddress(string building)
        {
            string connectionString = GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string checkQuery = "SELECT COUNT(*) FROM Building WHERE BuildingName = @BuildingName";
                MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@BuildingName", building);
                int existingCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                if (existingCount > 0)
                {
                    string errorMessage = "Building already exists.";
                    ShowError(errorMessage);
                    return;
                }

                string insertQuery = "INSERT INTO Building (BuildingName) VALUES (@BuildingName)";
                MySqlCommand command = new MySqlCommand(insertQuery, connection);
                command.Parameters.AddWithValue("@BuildingName", building);
                command.ExecuteNonQuery();

                ScriptManager.RegisterStartupScript(this, GetType(), "showToast", "showToast('Building added successfully.'); clearTextBoxes();", true);

                // Repopulate the dropdown to reflect the new department
                PopulateBuildingDropDown();
                PopulateDepartmentBuildingDropDown();
                PopulateUpdateBuildingDropDown();

                // Refresh the page after a delay (adjust as needed)
                ScriptManager.RegisterStartupScript(this, GetType(), "RefreshPage", "setTimeout(function() { location.reload(); }, 5000);", true);
            }
        }
        private void AddIPAddress(string deviceType, string departmentID, string ipAddress, string buildingID, string description, string interval, string intervalUnit)
        {
            string connectionString = GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                if (!IsValidIPAddress(ipAddress))
                {
                    string errorMessage = "Invalid IP Address.";
                    ShowError(errorMessage);
                    return;
                }

                string checkQuery = "SELECT COUNT(*) FROM IPAddress WHERE IPAddress = @IPAddress";
                MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@IPAddress", ipAddress);
                int existingCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                if (existingCount > 0)
                {
                    string errorMessage = "IP Address already exists.";
                    ShowError(errorMessage);
                    return;
                }

                string getTypeIDQuery = "SELECT DevTypeID FROM DeviceType WHERE DeviceType = @DeviceType";
                MySqlCommand getTypeIDCommand = new MySqlCommand(getTypeIDQuery, connection);
                getTypeIDCommand.Parameters.AddWithValue("@DeviceType", deviceType);
                int deviceTypeID = Convert.ToInt32(getTypeIDCommand.ExecuteScalar());

                // Ensure that BuildingID and DepartmentID are correctly retrieved
                string getBuildingIDQuery = "SELECT BuildingID FROM Building WHERE BuildingID = @BuildingID";
                MySqlCommand getBuildingIDCommand = new MySqlCommand(getBuildingIDQuery, connection);
                getBuildingIDCommand.Parameters.AddWithValue("@BuildingID", buildingID);
                int retrievedBuildingID = Convert.ToInt32(getBuildingIDCommand.ExecuteScalar());

                string getDepartmentIDQuery = "SELECT DepartmentID FROM Department WHERE DepartmentID = @DepartmentID";
                MySqlCommand getDepartmentIDCommand = new MySqlCommand(getDepartmentIDQuery, connection);
                getDepartmentIDCommand.Parameters.AddWithValue("@DepartmentID", departmentID);
                int retrievedDepartmentID = Convert.ToInt32(getDepartmentIDCommand.ExecuteScalar());

                string insertQuery = "INSERT INTO IPAddress (IPAddress, DeviceTypeID, BuildingID, DepartmentID, Description, RefreshInterval, IntervalUnit) " +
                                     "VALUES (@IPAddress, @DeviceTypeID, @BuildingID, @DepartmentID, @Description, @RefreshInterval, @IntervalUnit)";
                MySqlCommand command = new MySqlCommand(insertQuery, connection);
                command.Parameters.AddWithValue("@IPAddress", ipAddress);
                command.Parameters.AddWithValue("@DeviceTypeID", deviceTypeID);
                command.Parameters.AddWithValue("@BuildingID", retrievedBuildingID);
                command.Parameters.AddWithValue("@DepartmentID", retrievedDepartmentID);
                command.Parameters.AddWithValue("@Description", description);
                command.Parameters.AddWithValue("@RefreshInterval", interval);
                command.Parameters.AddWithValue("@IntervalUnit", intervalUnit);
                command.ExecuteNonQuery();

                ScriptManager.RegisterStartupScript(this, GetType(), "showToast", "showToast('IP Address added successfully.'); clearTextBoxes();", true);

                // Refresh the page after a delay (adjust as needed)
                ScriptManager.RegisterStartupScript(this, GetType(), "RefreshPage", "setTimeout(function() { location.reload(); }, 5000);", true);
            }
        }
        private void AddWebAddress(string deviceType, string webAddress, string departmentID, string buildingID, string interval, string intervalUnit)
        {
            string connectionString = GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                bool hasScheme = webAddress.StartsWith("http://") || webAddress.StartsWith("https://");
                if (!hasScheme)
                {
                    webAddress = "http://" + webAddress;
                }

                if (!IsValidWebAddress(webAddress))
                {
                    string errorMessage = "Invalid Web Address.";
                    errorLabel.Text = errorMessage;
                    errorLabel.Visible = true;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showErrorLabel", $"showErrorLabel('{errorMessage}'); clearTextBoxes();", true);
                    return;
                }

                // Strip the scheme and trailing slash before storing it in the database
                string webAddressToStore = RemoveSchemeAndTrailingSlash(webAddress);

                string checkQuery = "SELECT COUNT(*) FROM WebAddress WHERE WebAddress = @WebAddress";
                MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@WebAddress", webAddressToStore);
                int existingCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                if (existingCount > 0)
                {
                    string errorMessage = "Web Address already exists.";
                    errorLabel.Text = errorMessage;
                    errorLabel.Visible = true;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showErrorLabel", $"showErrorLabel('{errorMessage}'); clearTextBoxes();", true);
                    return;
                }

                string getTypeIDQuery = "SELECT DevTypeID FROM DeviceType WHERE DeviceType = @DeviceType";
                MySqlCommand getTypeIDCommand = new MySqlCommand(getTypeIDQuery, connection);
                getTypeIDCommand.Parameters.AddWithValue("@DeviceType", deviceType);
                int deviceTypeID = Convert.ToInt32(getTypeIDCommand.ExecuteScalar());

                string insertQuery = "INSERT INTO WebAddress (WebAddress, DeviceTypeID, DepartmentID, BuildingID, RefreshInterval, IntervalUnit) VALUES (@WebAddress, @DeviceTypeID, @DepartmentID, @BuildingID, @RefreshInterval, @IntervalUnit)";
                MySqlCommand command = new MySqlCommand(insertQuery, connection);
                command.Parameters.AddWithValue("@WebAddress", webAddressToStore);
                command.Parameters.AddWithValue("@DeviceTypeID", deviceTypeID);
                command.Parameters.AddWithValue("@DepartmentID", departmentID);
                command.Parameters.AddWithValue("@BuildingID", buildingID);
                command.Parameters.AddWithValue("@RefreshInterval", interval);
                command.Parameters.AddWithValue("@IntervalUnit", intervalUnit);
                command.ExecuteNonQuery();

                ScriptManager.RegisterStartupScript(this, GetType(), "showToast", "showToast('Web Address added successfully.'); clearTextBoxes();", true);

                // Refresh the page after a delay (adjust as needed)
                ScriptManager.RegisterStartupScript(this, GetType(), "RefreshPage", "setTimeout(function() { location.reload(); }, 5000);", true);
            }
        }
        private void AddDeviceAddress(string deviceType, string deviceName, string deviceIP, string departmentID, string buildingID, string interval, string intervalUnit)
        {
            string connectionString = GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                if (!IsValidIPAddress(deviceIP))
                {
                    string errorMessage = "Invalid IP Address.";
                    errorLabel.Text = errorMessage;
                    errorLabel.Visible = true;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showErrorLabel", $"showErrorLabel('{errorMessage}'); clearTextBoxes();", true);
                    return;
                }

                string checkQuery = "SELECT COUNT(*) FROM Device WHERE DigitalDevIP = @DeviceIP";
                MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@DeviceIP", deviceIP);
                int existingCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                if (existingCount > 0)
                {
                    string errorMessage = "Device IP Address already exists.";
                    errorLabel.Text = errorMessage;
                    errorLabel.Visible = true;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showErrorLabel", $"showErrorLabel('{errorMessage}'); clearTextBoxes();", true);
                    return;
                }

                string getTypeIDQuery = "SELECT DevTypeID FROM DeviceType WHERE DeviceType = @DeviceType";
                MySqlCommand getTypeIDCommand = new MySqlCommand(getTypeIDQuery, connection);
                getTypeIDCommand.Parameters.AddWithValue("@DeviceType", deviceType);
                int deviceTypeID = Convert.ToInt32(getTypeIDCommand.ExecuteScalar());

                string insertQuery = "INSERT INTO Device (DeviceTypeID, DigitalDevName, DigitalDevIP, DepartmentID, BuildingID, RefreshInterval, IntervalUnit) VALUES (@DeviceTypeID, @DigitalDevName, @DigitalDevIP, @DepartmentID, @BuildingID, @RefreshInterval, @IntervalUnit)";
                MySqlCommand command = new MySqlCommand(insertQuery, connection);
                command.Parameters.AddWithValue("@DeviceTypeID", deviceTypeID);
                command.Parameters.AddWithValue("@DigitalDevName", deviceName);
                command.Parameters.AddWithValue("@DigitalDevIP", deviceIP);
                command.Parameters.AddWithValue("@DepartmentID", departmentID);
                command.Parameters.AddWithValue("@BuildingID", buildingID);
                command.Parameters.AddWithValue("@RefreshInterval", interval);
                command.Parameters.AddWithValue("@IntervalUnit", intervalUnit);
                command.ExecuteNonQuery();

                ScriptManager.RegisterStartupScript(this, GetType(), "showToast", "showToast('Device added successfully.'); clearTextBoxes();", true);

                // Refresh the page after a delay (adjust as needed)
                ScriptManager.RegisterStartupScript(this, GetType(), "RefreshPage", "setTimeout(function() { location.reload(); }, 5000);", true);
            }
        }
        private void BindGridBasedOnDropDown()
        {
            string selectedValue = DropDownList1.SelectedValue;

            if (selectedValue == "showall")
            {
                BindGridView();
                GridView2.Visible = false;
                GridView3.Visible = false;
                GridView4.Visible = false;
                GridView1.Visible = true;
            }
            else if (selectedValue == "Digital Device")
            {
                BindDeviceGridView();
                GridView2.Visible = true;
                GridView3.Visible = false;
                GridView4.Visible = false;
                GridView1.Visible = false;
            }
            else if (selectedValue == "IP Address")
            {
                BindIPGridView();
                GridView2.Visible = false;
                GridView3.Visible = true;
                GridView4.Visible = false;
                GridView1.Visible = false;
            }
            else if (selectedValue == "Web Address")
            {
                BindWebGridView();
                GridView2.Visible = false;
                GridView3.Visible = false;
                GridView4.Visible = true;
                GridView1.Visible = false;
            }
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
        private void PopulateDropDownLists()
        {
            string connectionString = GetConnectionString();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // Populate Building DropDownList for IP section
                PopulateBuildingDropDown(conn, EditBuildingDropDown);
                // Populate Department DropDownList for IP section
                PopulateDepartmentDropDown(conn, EditDeptDropDown);

                // Populate Building DropDownList for Device section
                PopulateBuildingDropDown(conn, EditBuildingDropDownDevice);
                // Populate Department DropDownList for Device section
                PopulateDepartmentDropDown(conn, EditDeptDropDownDevice);

                // Populate Building DropDownList for Web section
                PopulateBuildingDropDown(conn, EditBuildingDropDownWeb);
                // Populate Department DropDownList for Web section
                PopulateDepartmentDropDown(conn, EditDeptDropDownWeb);
            }
        }

        private void PopulateBuildingDropDown(MySqlConnection conn, DropDownList dropDown)
        {
            string buildingSql = "SELECT BuildingID, BuildingName FROM Building";
            using (MySqlCommand cmd = new MySqlCommand(buildingSql, conn))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    dropDown.Items.Clear();
                    dropDown.Items.Add(new ListItem("Change", "")); // Default value

                    while (reader.Read())
                    {
                        dropDown.Items.Add(new ListItem(reader["BuildingName"].ToString(), reader["BuildingID"].ToString()));
                    }
                }
            }
        }

        private void PopulateDepartmentDropDown(MySqlConnection conn, DropDownList dropDown)
        {
            string departmentSql = "SELECT DepartmentID, DepartmentName FROM Department";
            using (MySqlCommand cmd = new MySqlCommand(departmentSql, conn))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    dropDown.Items.Clear();
                    dropDown.Items.Add(new ListItem("Change", "")); // Default value

                    while (reader.Read())
                    {
                        dropDown.Items.Add(new ListItem(reader["DepartmentName"].ToString(), reader["DepartmentID"].ToString()));
                    }
                }
            }
        }

        protected void EditBuildingDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterEditDepartmentDropDown(EditBuildingDropDown, EditDeptDropDown);
        }

        protected void EditBuildingDropDownDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterEditDepartmentDropDown(EditBuildingDropDownDevice, EditDeptDropDownDevice);
        }

        protected void EditBuildingDropDownWeb_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterEditDepartmentDropDown(EditBuildingDropDownWeb, EditDeptDropDownWeb);
        }

        private void FilterEditDepartmentDropDown(DropDownList buildingDropDown, DropDownList deptDropDown)
        {
            if (!string.IsNullOrEmpty(buildingDropDown.SelectedValue) && buildingDropDown.SelectedValue != "0")
            {
                string connectionString = GetConnectionString();
                string query = "SELECT DepartmentID, DepartmentName FROM Department WHERE BuildingID = @BuildingID";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@BuildingID", buildingDropDown.SelectedValue);
                        using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);
                            deptDropDown.DataSource = dt;
                            deptDropDown.DataTextField = "DepartmentName";
                            deptDropDown.DataValueField = "DepartmentID";
                            deptDropDown.DataBind();
                        }
                    }
                }
            }
            else
            {
                deptDropDown.Items.Clear();
                deptDropDown.Items.Add(new ListItem("Change", ""));
            }
        }


        // Edit row in GridView
        protected void SaveIpAddress(object sender, EventArgs e)
        {
            // Validate that none of the text fields are null or empty
            if (string.IsNullOrWhiteSpace(EditLocTextbox.Text) ||
                string.IsNullOrWhiteSpace(EditIPInterval.Text))
            {
                // Display an error message
                ShowError(ipErrorLabel, "All fields are required.");
                return;
            }

            if (!TryParseInterval(EditIPInterval.Text, out int refreshInterval))
            {
                ShowError(ipErrorLabel, "Invalid interval value.");
                return;
            }

            string buildingID = EditBuildingDropDown.SelectedValue;
            string departmentID = EditDeptDropDown.SelectedValue;
            string ipAddress = EditLocTextbox.Text;
            string intervalUnit = EditIPUnit.SelectedValue;
            string ipAddressID = hiddenIPAddressID.Value;
            string description = EditDescriptionTextbox.Text; // Retrieve description value

            if ((intervalUnit == "seconds" || intervalUnit == "minutes") && refreshInterval > 60)
            {
                ShowError(ipErrorLabel, "Interval value should not exceed 60 for seconds or minutes.");
                return;
            }
            else if (intervalUnit == "hours" && refreshInterval > 24)
            {
                ShowError(ipErrorLabel, "Interval value should not exceed 24 for hours.");
                return;
            }

            // Update the IP address based on selected values
            if (string.IsNullOrWhiteSpace(buildingID) && string.IsNullOrWhiteSpace(departmentID))
            {
                buildingID = hiddenBuildingID.Value;
                departmentID = hiddenDepartmentID.Value;
                UpdateIpAddress2(ipAddressID, ipAddress, refreshInterval, intervalUnit, description);
            }
            else if (string.IsNullOrWhiteSpace(buildingID))
            {
                buildingID = hiddenBuildingID.Value;
                UpdateIpAddress3(ipAddressID, departmentID, ipAddress, refreshInterval, intervalUnit, description);
            }
            else if (string.IsNullOrWhiteSpace(departmentID))
            {
                departmentID = hiddenDepartmentID.Value;
                UpdateIpAddress4(ipAddressID, buildingID, ipAddress, refreshInterval, intervalUnit, description);
            }
            else
            {
                UpdateIpAddress(ipAddressID, buildingID, departmentID, ipAddress, refreshInterval, intervalUnit, description);
            }


            // Refresh the page after a delay (adjust as needed)
            ScriptManager.RegisterStartupScript(this, GetType(), "RefreshPage", "setTimeout(function() { location.reload(); }, 100);", true);
        }

        protected void SaveDigitalDevice(object sender, EventArgs e)
        {
            // Validate that none of the text fields are null or empty
            if (string.IsNullOrWhiteSpace(EditDeviceName.Text) ||
                string.IsNullOrWhiteSpace(EditDevIPTextbox.Text) ||
                string.IsNullOrWhiteSpace(EditDeviceInterval.Text))
            {
                // Display an error message
                ShowError(digitalDeviceErrorLabel, "All fields are required.");
                return;
            }

            if (!TryParseInterval(EditDeviceInterval.Text, out int refreshInterval))
            {
                ShowError(digitalDeviceErrorLabel, "Invalid interval value.");
                return;
            }

            string deviceID = hiddenDeviceID.Value;
            string deviceName = EditDeviceName.Text;
            string ipAddress = EditDevIPTextbox.Text;
            string intervalUnit = EditDeviceUnit.SelectedValue;
            string buildingID = EditBuildingDropDownDevice.SelectedValue;
            string departmentID = EditDeptDropDownDevice.SelectedValue;

            if ((intervalUnit == "seconds" || intervalUnit == "minutes") && refreshInterval > 60)
            {
                ShowError(digitalDeviceErrorLabel, "Interval value should not exceed 60 for seconds or minutes.");
                return;
            }
            else if (intervalUnit == "hours" && refreshInterval > 24)
            {
                ShowError(digitalDeviceErrorLabel, "Interval value should not exceed 24 for hours.");
                return;
            }

            // Check if "Change" is selected in either dropdown
            if (string.IsNullOrWhiteSpace(buildingID) && string.IsNullOrWhiteSpace(departmentID))
            {
                // If "Change" is selected, keep the existing values from hidden fields
                buildingID = hiddenDeviceBuildingID.Value;
                departmentID = hiddenDeviceDeptID.Value;

                UpdateDigital2(deviceID, ipAddress, refreshInterval, intervalUnit, deviceName);
                BindDeviceGridView();
            }
            else if (string.IsNullOrWhiteSpace(buildingID))
            {
                // If "Change" is selected, keep the existing values from hidden fields
                buildingID = hiddenDeviceBuildingID.Value;

                UpdateDigital3(deviceID, departmentID, ipAddress, refreshInterval, intervalUnit, deviceName);
                BindDeviceGridView();
            }
            else if (string.IsNullOrWhiteSpace(departmentID))
            {
                // If "Change" is selected, keep the existing values from hidden fields
                departmentID = hiddenDeviceDeptID.Value;

                UpdateDigital4(deviceID, buildingID, ipAddress, refreshInterval, intervalUnit, deviceName);
                BindDeviceGridView();
            }
            else
            {
                UpdateDigitalDevice(deviceID, deviceName, ipAddress, refreshInterval, intervalUnit, buildingID, departmentID);

                BindDeviceGridView();
            }

            // Refresh the page after a delay (adjust as needed)
            ScriptManager.RegisterStartupScript(this, GetType(), "RefreshPage", "setTimeout(function() { location.reload(); }, 100);", true);
        }

        protected void SaveWebAddress(object sender, EventArgs e)
        {
            // Validate that none of the text fields are null or empty
            if (string.IsNullOrWhiteSpace(EditWebTextbox.Text) ||
                string.IsNullOrWhiteSpace(EditWebInterval.Text))
            {
                // Display an error message
                ShowError(webAddressErrorLabel, "All fields are required.");
                return;
            }

            if (!TryParseInterval(EditWebInterval.Text, out int refreshInterval))
            {
                ShowError(webAddressErrorLabel, "Invalid interval value.");
                return;
            }

            string webID = hiddenWebID.Value;
            string webAddress = EditWebTextbox.Text;
            string intervalUnit = EditWebUnit.SelectedValue;
            string buildingID = EditBuildingDropDownWeb.SelectedValue;
            string departmentID = EditDeptDropDownWeb.SelectedValue;

            if ((intervalUnit == "seconds" || intervalUnit == "minutes") && refreshInterval > 60)
            {
                ShowError(webAddressErrorLabel, "Interval value should not exceed 60 for seconds or minutes.");
                return;
            }
            else if (intervalUnit == "hours" && refreshInterval > 24)
            {
                ShowError(webAddressErrorLabel, "Interval value should not exceed 24 for hours.");
                return;
            }

            // Check if "Change" is selected in either dropdown
            if (buildingID == "" && departmentID == "")
            {
                // If "Change" is selected, keep the existing values from hidden fields
                buildingID = hiddenDeviceBuildingID.Value;
                departmentID = hiddenDeviceDeptID.Value;

                UpdateWeb2(webID, webAddress, refreshInterval, intervalUnit);
                BindWebGridView();
            }
            else if (buildingID == "")
            {
                // If "Change" is selected, keep the existing values from hidden fields
                buildingID = hiddenDeviceBuildingID.Value;

                UpdateWeb3(webID, departmentID, webAddress, refreshInterval, intervalUnit);
                BindWebGridView();
            }
            else if (departmentID == "")
            {
                // If "Change" is selected, keep the existing values from hidden fields
                departmentID = hiddenDeviceDeptID.Value;

                UpdateWeb4(webID, buildingID, webAddress, refreshInterval, intervalUnit);
                BindWebGridView();
            }
            else
            {
                UpdateWebAddress(webID, webAddress, refreshInterval, intervalUnit, buildingID, departmentID);

                BindWebGridView();
            }

            // Refresh the page after a delay (adjust as needed)
            ScriptManager.RegisterStartupScript(this, GetType(), "RefreshPage", "setTimeout(function() { location.reload(); }, 100);", true);
        }

        private bool TryParseInterval(string intervalText, out int interval)
        {
            return int.TryParse(intervalText, out interval);
        }
        protected void ShowError(Label label, string message)
        {
            label.Text = message;
            label.ForeColor = System.Drawing.Color.Red;
            label.Visible = true;

            // Set a timeout to hide the error label after 5 seconds
            string script = $"setTimeout(function() {{ document.getElementById('{label.ClientID}').style.display = 'none'; }}, 5000);";
            ScriptManager.RegisterStartupScript(this, GetType(), "HideErrorScript", script, true);
        }

        private void ShowToast(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "showToast", $"showToast('{message}');", true);
        }
        private void UpdateIpAddress(string ipAddressID, string buildingID, string departmentID, string ipAddress, int refreshInterval, string intervalUnit, string description)
        {
            string connectionString = GetConnectionString();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE ipaddress SET " +
                                "BuildingID = @buildingID, " +
                                "DepartmentID = @departmentID, " +
                                "IPAddress = @ipAddress, " +
                                "RefreshInterval = @refreshInterval, " +
                                "IntervalUnit = @intervalUnit, " +
                                "Description = @description " +
                                "WHERE IPAddressID = @ipAddressID";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ipAddressID", ipAddressID);
                    command.Parameters.AddWithValue("@buildingID", buildingID);
                    command.Parameters.AddWithValue("@departmentID", departmentID);
                    command.Parameters.AddWithValue("@ipAddress", ipAddress);
                    command.Parameters.AddWithValue("@refreshInterval", refreshInterval);
                    command.Parameters.AddWithValue("@intervalUnit", intervalUnit);
                    command.Parameters.AddWithValue("@description", description);

                    // Ensure the command is executed
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        private void UpdateIpAddress2(string ipAddressID, string ipAddress, int refreshInterval, string intervalUnit, string description)
        {
            string connectionString = GetConnectionString();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE ipaddress SET " +
                               "IPAddress = @ipAddress, " +
                               "Description = @description, " +
                               "RefreshInterval = @refreshInterval, " +
                               "IntervalUnit = @intervalUnit " +
                               "WHERE IPAddressID = @ipAddressID";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ipAddressID", ipAddressID);
                    command.Parameters.AddWithValue("@ipAddress", ipAddress);
                    command.Parameters.AddWithValue("@refreshInterval", refreshInterval);
                    command.Parameters.AddWithValue("@intervalUnit", intervalUnit);
                    command.Parameters.AddWithValue("@description", description);

                    // Ensure the command is executed
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
        private void UpdateIpAddress3(string ipAddressID, string departmentID, string ipAddress, int refreshInterval, string intervalUnit, string description)
        {
            string connectionString = GetConnectionString();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE ipaddress SET " +
                                "DepartmentID = @departmentID, " +
                               "IPAddress = @ipAddress, " +
                               "Description = @description, " +
                               "RefreshInterval = @refreshInterval, " +
                               "IntervalUnit = @intervalUnit " +
                               "WHERE IPAddressID = @ipAddressID";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ipAddressID", ipAddressID);
                    command.Parameters.AddWithValue("@departmentID", departmentID);
                    command.Parameters.AddWithValue("@ipAddress", ipAddress);
                    command.Parameters.AddWithValue("@refreshInterval", refreshInterval);
                    command.Parameters.AddWithValue("@intervalUnit", intervalUnit);
                    command.Parameters.AddWithValue("@description", description);

                    // Ensure the command is executed
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
        private void UpdateIpAddress4(string ipAddressID, string buildingID, string ipAddress, int refreshInterval, string intervalUnit, string description)
        {
            string connectionString = GetConnectionString();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE ipaddress SET " +
                               "BuildingID = @buildingID, " +
                               "IPAddress = @ipAddress, " +
                               "Description = @description, " +
                               "RefreshInterval = @refreshInterval, " +
                               "IntervalUnit = @intervalUnit " +
                               "WHERE IPAddressID = @ipAddressID";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ipAddressID", ipAddressID);
                    command.Parameters.AddWithValue("@buildingID", buildingID);
                    command.Parameters.AddWithValue("@ipAddress", ipAddress);
                    command.Parameters.AddWithValue("@refreshInterval", refreshInterval);
                    command.Parameters.AddWithValue("@intervalUnit", intervalUnit);
                    command.Parameters.AddWithValue("@description", description);

                    // Ensure the command is executed
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
        private void UpdateDigitalDevice(string deviceID, string deviceName, string ipAddress, int refreshInterval, string intervalUnit, string buildingID, string departmentID)
        {
            string connectionString = GetConnectionString();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE Device SET DigitalDevName = @deviceName, DigitalDevIP = @deviceIP, RefreshInterval = @refreshInterval, IntervalUnit = @intervalUnit, BuildingID = @buildingID, DepartmentID = @departmentID WHERE DigitalDevID = @deviceID";

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@deviceID", deviceID);
                command.Parameters.AddWithValue("@deviceName", deviceName);
                command.Parameters.AddWithValue("@deviceIP", ipAddress);
                command.Parameters.AddWithValue("@refreshInterval", refreshInterval);
                command.Parameters.AddWithValue("@intervalUnit", intervalUnit);
                command.Parameters.AddWithValue("@buildingID", buildingID);
                command.Parameters.AddWithValue("@departmentID", departmentID);
                command.ExecuteNonQuery();

                connection.Close();
            }
        }
        private void UpdateDigital2(string deviceID, string ipAddress, int refreshInterval, string intervalUnit, string deviceName)
        {
            string connectionString = GetConnectionString();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE device SET " +
                               "DigitalDevIP = @deviceIP, " +
                               "DigitalDevName = @deviceName, " +
                               "RefreshInterval = @refreshInterval, " +
                               "IntervalUnit = @intervalUnit " +
                               "WHERE DigitalDevID = @deviceID";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@deviceID", deviceID);
                    command.Parameters.AddWithValue("@deviceIP", ipAddress);
                    command.Parameters.AddWithValue("@refreshInterval", refreshInterval);
                    command.Parameters.AddWithValue("@intervalUnit", intervalUnit);
                    command.Parameters.AddWithValue("@deviceName", deviceName);

                    // Ensure the command is executed
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        private void UpdateDigital3(string deviceID, string departmentID, string ipAddress, int refreshInterval, string intervalUnit, string deviceName)
        {
            string connectionString = GetConnectionString();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE device SET " +
                                "DepartmentID = @departmentID, " +
                               "DigitalDevIP = @deviceIP, " +
                               "DigitalDevName = @deviceName, " +
                               "RefreshInterval = @refreshInterval, " +
                               "IntervalUnit = @intervalUnit " +
                               "WHERE DigitalDevID = @deviceID";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@deviceID", deviceID);
                    command.Parameters.AddWithValue("@departmentID", departmentID);
                    command.Parameters.AddWithValue("@deviceIP", ipAddress);
                    command.Parameters.AddWithValue("@refreshInterval", refreshInterval);
                    command.Parameters.AddWithValue("@intervalUnit", intervalUnit);
                    command.Parameters.AddWithValue("@deviceName", deviceName);

                    // Ensure the command is executed
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
        private void UpdateDigital4(string deviceID, string buildingID, string ipAddress, int refreshInterval, string intervalUnit, string deviceName)
        {
            string connectionString = GetConnectionString();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE device SET " +
                               "BuildingID = @buildingID, " +
                               "DigitalDevIP = @deviceIP, " +
                               "DigitalDevName = @deviceName, " +
                               "RefreshInterval = @refreshInterval, " +
                               "IntervalUnit = @intervalUnit " +
                               "WHERE DigitalDevID = @deviceID";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@deviceID", deviceID);
                    command.Parameters.AddWithValue("@buildingID", buildingID);
                    command.Parameters.AddWithValue("@deviceIP", ipAddress);
                    command.Parameters.AddWithValue("@refreshInterval", refreshInterval);
                    command.Parameters.AddWithValue("@intervalUnit", intervalUnit);
                    command.Parameters.AddWithValue("@deviceName", deviceName);

                    // Ensure the command is executed
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
        private void UpdateWebAddress(string webID, string webAddress, int refreshInterval, string intervalUnit, string buildingID, string departmentID)
        {
            string connectionString = GetConnectionString();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE WebAddress SET WebAddress = @webAddress, RefreshInterval = @refreshInterval, IntervalUnit = @intervalUnit, BuildingID = @buildingID, DepartmentID = @departmentID WHERE WebID = @webID";

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@webID", webID);
                command.Parameters.AddWithValue("@webAddress", webAddress);
                command.Parameters.AddWithValue("@refreshInterval", refreshInterval);
                command.Parameters.AddWithValue("@intervalUnit", intervalUnit);
                command.Parameters.AddWithValue("@buildingID", buildingID);
                command.Parameters.AddWithValue("@departmentID", departmentID);
                command.ExecuteNonQuery();

                connection.Close();
            }
        }
        private void UpdateWeb2(string webID, string webAddress, int refreshInterval, string intervalUnit)
        {
            string connectionString = GetConnectionString();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE WebAddress SET " +
                               "WebAddress = @webAddress, " +
                               "RefreshInterval = @refreshInterval, " +
                               "IntervalUnit = @intervalUnit " +
                               "WHERE WebID = @webID";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@webID", webID);
                    command.Parameters.AddWithValue("@webAddress", webAddress);
                    command.Parameters.AddWithValue("@refreshInterval", refreshInterval);
                    command.Parameters.AddWithValue("@intervalUnit", intervalUnit);

                    // Ensure the command is executed
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        private void UpdateWeb3(string webID, string departmentID, string webAddress, int refreshInterval, string intervalUnit)
        {
            string connectionString = GetConnectionString();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE WebAddress SET " +
                               "DepartmentID = @departmentID, " +
                               "WebAddress = @webAddress, " +
                               "RefreshInterval = @refreshInterval, " +
                               "IntervalUnit = @intervalUnit " +
                               "WHERE WebID = @webID";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@webID", webID);
                    command.Parameters.AddWithValue("@departmentID", departmentID);
                    command.Parameters.AddWithValue("@webAddress", webAddress);
                    command.Parameters.AddWithValue("@refreshInterval", refreshInterval);
                    command.Parameters.AddWithValue("@intervalUnit", intervalUnit);

                    // Ensure the command is executed
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
        private void UpdateWeb4(string webID, string buildingID, string webAddress, int refreshInterval, string intervalUnit)
        {
            string connectionString = GetConnectionString();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE WebAddress SET " +
                               "BuildingID = @buildingID, " +
                               "WebAddress = @webAddress, " +
                               "RefreshInterval = @refreshInterval, " +
                               "IntervalUnit = @intervalUnit " +
                               "WHERE WebID = @webID";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@webID", webID);
                    command.Parameters.AddWithValue("@buildingID", buildingID);
                    command.Parameters.AddWithValue("@webAddress", webAddress);
                    command.Parameters.AddWithValue("@refreshInterval", refreshInterval);
                    command.Parameters.AddWithValue("@intervalUnit", intervalUnit);

                    // Ensure the command is executed
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
        protected void GridView2_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < GridView2.Rows.Count)
            {
                string deviceID = GridView2.DataKeys[e.RowIndex].Value.ToString();

                string connectionString = GetConnectionString();
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Device WHERE DigitalDevID = @deviceID";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@deviceID", deviceID);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        BindDeviceGridView();
                        ShowToast("Device has been deleted successfully.");
                    }
                    else
                    {
                        // Handle error or display a message indicating deletion failed
                    }
                }
            }
        }
        protected void GridView3_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < GridView3.Rows.Count)
            {
                string ipAddressID = GridView3.DataKeys[e.RowIndex].Value.ToString();

                string connectionString = GetConnectionString();
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM IPAddress WHERE IPAddressID = @ipAddressID";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ipAddressID", ipAddressID);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        BindIPGridView();
                        ShowToast("IP address has been deleted successfully.");
                    }
                    else
                    {
                        // Optionally handle the case where no row was deleted
                    }
                }
            }
        }

        protected void GridView4_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < GridView4.Rows.Count)
            {
                string WebID = GridView4.DataKeys[e.RowIndex].Value.ToString();

                string connectionString = GetConnectionString();
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM WebAddress WHERE WebID = @WebID";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@WebID", WebID);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        BindWebGridView();
                        ShowToast("Web address has been deleted successfully.");
                    }
                    else
                    {
                        // Optionally handle the case where no row was deleted
                    }
                }
            }
        }

        protected void DepartmentGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < DepartmentGridView.Rows.Count)
            {
                string departmentID = DepartmentGridView.DataKeys[e.RowIndex].Value.ToString();

                string connectionString = GetConnectionString();
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Department WHERE DepartmentID = @DepartmentID";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@DepartmentID", departmentID);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        BindDepartmentGridView();
                        PopulateDropDownLists();
                        PopulateDepartmentBuildingDropDown();
                        PopulateDepartmentDropDown();
                        ShowToast("Department has been deleted successfully.");
                    }
                    else
                    {
                        // Optionally handle the case where no row was deleted
                    }
                }
            }
        }

        protected void BuildingGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < BuildingGridView.Rows.Count)
            {
                string buildingID = BuildingGridView.DataKeys[e.RowIndex].Value.ToString();

                string connectionString = GetConnectionString();
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Building WHERE BuildingID = @BuildingID";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@BuildingID", buildingID);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        BindBuildingGridView();
                        PopulateUpdateBuildingDropDown();
                        PopulateDepartmentBuildingDropDown();
                        PopulateBuildingDropDown();
                        PopulateDropDownLists();
                        ShowToast("Building has been deleted successfully.");
                    }
                    else
                    {
                        // Optionally handle the case where no row was deleted
                    }
                }
            }
        }


        protected void refresh_Click(object sender, EventArgs e)
        {
            Response.Redirect("load.aspx?target=configuration-management.aspx");
        }
    }
}
