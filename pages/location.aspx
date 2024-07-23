<%@ Page Title="" Language="C#" MasterPageFile="~/pages/Menu.Master" AutoEventWireup="true" CodeBehind="location.aspx.cs" Inherits="NMS.pages.location" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
  <link rel="stylesheet" href="../assets/styles/location.css"/>
  <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/boxicons/2.1.0/css/boxicons.min.css"/>
  <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css"/>
  <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/boxicons/2.1.0/css/boxicons.min.css"/>
    <link href='https://cdn.jsdelivr.net/npm/boxicons@2.1.2/css/boxicons.min.css' rel='stylesheet'>

   <!-- FOR DROPDOWN OF CHECKLIST -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.7/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/js/bootstrap.min.js"></script>


   <!-- Include additional CSS files if necessary -->

   <asp:ScriptManager ID="ScriptManager1" runat="server" />
   <div class="container">
   <div class="main-content">
        <div class="top-container">
                <div class="top">
                     <h1><asp:LinkButton ID="refresh" runat="server" Font-Underline="False" ForeColor="Black" OnClick="refresh_Click">Real-Time</asp:LinkButton></h1>  
                </div>
                <div class="top-buttons">
                   <div class="mydict">
                        <div>
                            <label>
                                <asp:RadioButton ID="devicesbtn" runat="server" AutoPostBack="True" OnCheckedChanged="devicesbtn_CheckedChanged" CssClass="radio-button" />
                                <span>Overview</span>
                            </label>
                            <label>
                                 <asp:RadioButton ID="locationsbtn" runat="server" AutoPostBack="True" Checked="True" CssClass="radio-button" Enabled="False" />
                                  <span style="background-color: #B4890A; color: #FFF; border-color: #B4890A;">Monitor</span>
                            </label>
                        </div>
                    </div>
                </div>

            </div>

            <div class="main-container">
                <div class="align-container">
                    <div class="uno-row">

                        <!-- DROPDOWN WITH CHECKLIST -->
                      <div class="dropdown">
                        <button class="btn btn-secondary dropdown-toggle" type="button" id="dropdownMenuButton" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                            Select Buildings <i class='bx bxs-chevron-down'></i>
                        </button>
                        <div class="dropdown-menu p-3" aria-labelledby="dropdownMenuButton">
                            <asp:CheckBox ID="SelectAllBuildingsCheckBox" runat="server" Text="Select All" AutoPostBack="true" OnCheckedChanged="SelectAllBuildingsCheckBox_CheckedChanged" />
                            <br />
                            <asp:CheckBoxList ID="BuildingCheckBoxList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="BuildingCheckBoxList_SelectedIndexChanged">
                            </asp:CheckBoxList>
                        </div>
                    </div>

                       
                        <div class="select-status">
                            <asp:DropDownList ID="CategoryDropDown" runat="server" AutoPostBack="true" Width="160px" OnSelectedIndexChanged="CategoryDropDown_SelectedIndexChanged">
                                <asp:ListItem Value="Select Category">Select Category</asp:ListItem>  
                                <asp:ListItem Value="ip">IP Address</asp:ListItem>
                                <asp:ListItem Value="device">Device Address</asp:ListItem>
                                <asp:ListItem Value="web">Web Address</asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="select-device">
                            <asp:DropDownList ID="DepartmentDropDown" runat="server" AutoPostBack="true" Width="160px" OnSelectedIndexChanged="DepartmentDropDown_SelectedIndexChanged">
                                    <asp:ListItem>Select Location</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                         
                        <div class="select-status">
                            <asp:DropDownList ID="StatusDropDown" runat="server" AutoPostBack="true" Width="160px" OnSelectedIndexChanged="StatusDropDown_SelectedIndexChanged">
                                <asp:ListItem>Select Status</asp:ListItem>
                                <asp:ListItem Value="HEALTHY">Healthy</asp:ListItem>
                                <asp:ListItem Value="DOWN">Down</asp:ListItem>
                                <asp:ListItem Value="PROBLEMATIC">Problematic</asp:ListItem>
                            </asp:DropDownList>
                            <asp:HiddenField ID="hfInterval" runat="server" />
                        </div>
                        


                    </div>
                </div>

                <div class="grid-container2">
                <asp:GridView ID="AllGridView1" runat="server" CssClass="gridview-all" Style="width: 100%" AutoGenerateColumns="False">
                    <%-- No Results --%>
                    <EmptyDataTemplate>
                        <div class="alert alert-info" id="infoAlert_Department" role="alert">
                            No results found.
                        </div>               
                    </EmptyDataTemplate>
                </asp:GridView>

                <asp:GridView ID="AllGridView2" runat="server" CssClass="gridview-all" Style="width: 100%" AutoGenerateColumns="False">
                    <%-- No Results --%>
                    <EmptyDataTemplate>
                        <div class="alert alert-info" id="infoAlert_Department" role="alert">
                            No results found.
                        </div>               
                    </EmptyDataTemplate>
                </asp:GridView>
                </div>


                <div class="grid-container">
                <asp:GridView ID="IPGridView1" runat="server" CssClass="gridview" Style="width: 100%" AutoGenerateColumns="False">
                       <%-- No Results --%>
                       <EmptyDataTemplate>
                           <div class="alert alert-info" id="infoAlert_Department" role="alert">
                               No results found.
                           </div>               
                    </EmptyDataTemplate>
                </asp:GridView>
    
                <asp:GridView ID="IPGridView2" runat="server" CssClass="gridview" Style="width: 100%" AutoGenerateColumns="False" >
                </asp:GridView>

                <asp:GridView ID="WebGridView" runat="server" CssClass="gridview" Style="width: 100%" AutoGenerateColumns="False">
                       <%-- No Results --%>
                       <EmptyDataTemplate>
                           <div class="alert alert-info" id="infoAlert_Department" role="alert">
                               No results found.
                           </div>               
                    </EmptyDataTemplate>
                </asp:GridView>

                <asp:GridView ID="WebGridView2" runat="server" CssClass="gridview" Style="width: 100%" AutoGenerateColumns="False">
                </asp:GridView>

                <asp:GridView ID="DeviceGridView" runat="server" CssClass="gridview" Style="width: 100%" AutoGenerateColumns="False">
                       <%-- No Results --%>
                       <EmptyDataTemplate>
                           <div class="alert alert-info" id="infoAlert_Department" role="alert">
                               No results found.
                           </div>               
                    </EmptyDataTemplate>
                </asp:GridView>

                <asp:GridView ID="DeviceGridView2" runat="server" CssClass="gridview" Style="width: 100%" AutoGenerateColumns="False">
                </asp:GridView>
            </div>
        </div>

    </div>
</div>

<script type="text/javascript">
    function refreshPage() {
        var interval = 30; // Refresh interval in seconds
        setTimeout(function () {
            location.reload();
        }, interval * 1000);
    }

    window.onload = function () {
        // Refresh page at set interval
        refreshPage();

        // Event listener for radio button changes
        var devicesBtn = document.getElementById('<%= devicesbtn.ClientID %>');
        var locationsBtn = document.getElementById('<%= locationsbtn.ClientID %>');

        devicesBtn.addEventListener('change', function () {
            if (devicesBtn.checked) {
                // Additional actions if needed when Devices is selected
            }
        });

        locationsBtn.addEventListener('change', function () {
            if (locationsBtn.checked) {
                // Additional actions if needed when Locations is selected
            }
        });

        // Event listener for dropdown menu
        var dropdownToggle = document.querySelector('.dropdown-toggle');
        var dropdownMenu = document.querySelector('.dropdown-menu');

        dropdownToggle.addEventListener('click', function () {
            dropdownMenu.classList.toggle('show');
        });

        // Close dropdown when clicking outside
        window.addEventListener('click', function (event) {
            if (!dropdownToggle.contains(event.target) && !dropdownMenu.contains(event.target)) {
                dropdownMenu.classList.remove('show');
            }
        });

        // Toggle select all checkboxes
        document.querySelector('.selectall').addEventListener('click', function () {
            var checkboxes = document.querySelectorAll('.justone');
            checkboxes.forEach(function (checkbox) {
                checkbox.checked = this.checked;
            }.bind(this));
            // Trigger grid update on checkbox change
            UpdateGridVisibility();
        });

        document.querySelectorAll('.justone').forEach(function (checkbox) {
            checkbox.addEventListener('click', function () {
                var allChecked = document.querySelectorAll('.justone:checked').length === document.querySelectorAll('.justone').length;
                document.querySelector('.selectall').checked = allChecked;
                // Trigger grid update on checkbox change
                UpdateGridVisibility();
            });
        });
    };
</script>


</asp:Content>