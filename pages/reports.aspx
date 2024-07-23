<%@ Page Title="" Language="C#" MasterPageFile="~/pages/Menu.Master" AutoEventWireup="true" CodeBehind="reports.aspx.cs" Inherits="NMS.pages.reports" Async="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<link rel="stylesheet" href="../assets/styles/report.css"/>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/boxicons/2.1.0/css/boxicons.min.css"/>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css"/>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/boxicons/2.1.0/css/boxicons.min.css"/>
    <link href="https://cdn.jsdelivr.net/npm/pikaday/css/pikaday.css" rel="stylesheet"/>
    <!-- Include additional CSS files if necessary -->

    <!-- Header -->   
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <div class="container">
    <div class="main-content">
                <div class="top-container">
                    <div class="top">
                        <h1><asp:LinkButton ID="refresh" runat="server" Font-Underline="False" ForeColor="Black" OnClick="refresh_Click">Reports</asp:LinkButton></h1>  
                    </div>
                    <div class="top-buttons">
                       <div class="button-add">
                        <asp:LinkButton ID="downloadbtn" runat="server" OnClick="downloadbtn_Click" CssClass="button">
                            <div class="text">Export</div>
                            <span class="icon">
                                <i class="fa-solid fa-file-arrow-down"></i>
                            </span>
                        </asp:LinkButton>               
                    </div>
                        &nbsp;&nbsp;
                              <!-- DELETE BUTTON -->               
                   <div class="button-del">                  
                        <asp:LinkButton ID="deletedatabtn" runat="server" OnClick="deletebtn_Click" OnClientClick="return confirmDelete();" CssClass="delete-button">
                            Erase Entry
                        </asp:LinkButton>
                    </div>

                    </div>
                </div>

    	             <!-- Toast -->
                <div class="toast">
                    <div class="toast-content">
                        <div class="icon check">
                            <i class="fa-solid fa-check"></i>
                        </div>
                        &nbsp;
                        <div class="message">
                            <span class="text text-1">Success</span>
                            <span class="text text-2"></span>
                        </div>
                    </div>
                    <i class="fas fa-solid fa-times close"></i>
                    <div class="progress"></div>
                </div>



                <!-- Main Content -->
                <div class="main-container">
                       <div class="align-container">
                            <div class="uno-row">
                   

                             <div class="custom-search-bar">
                                 <i class='bx bx-search'></i>
                                 <asp:TextBox ID="searchtxtbox" placeholder="Search..." runat="server" OnTextChanged="searchtxtbox_TextChanged" AutoPostBack="true"></asp:TextBox>              
                             </div>

                             <div class="select-controls">
                                <div class="select-date">
                                <asp:TextBox ID="singleDateRange" CssClass="daterange" runat="server" placeholder="Select Date..." AutoPostBack="true" OnTextChanged="singleDateRange_TextChanged"></asp:TextBox>
                                </div>
               
                                    <div class="select-device">
                                         <asp:DropDownList ID="BuildingDropDown" runat="server" AutoPostBack="True" OnSelectedIndexChanged="BuildingDropDown_SelectedIndexChanged"></asp:DropDownList> &nbsp

                                        <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="true" Width="170px" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
                                            <asp:ListItem>Select Category</asp:ListItem>
                                            <asp:ListItem Value="Digital Device">Digital Device</asp:ListItem>
                                            <asp:ListItem Value="IP Address">IP Address</asp:ListItem>
                                            <asp:ListItem Value="Web Address">Web Address</asp:ListItem>
                                        </asp:DropDownList>
                    
                                &nbsp;
                                </div>
                                <div class="select-status">
                                    <asp:DropDownList ID="DropDownList2" runat="server" AutoPostBack="true" Width="160px" OnSelectedIndexChanged="DropDownList2_SelectedIndexChanged">
                                        <asp:ListItem>Select Status</asp:ListItem>
                                        <asp:ListItem Value="healthy">Healthy</asp:ListItem>
                                        <asp:ListItem Value="down">Down</asp:ListItem>
                                        <asp:ListItem Value="problematic">Problematic</asp:ListItem>
                                    </asp:DropDownList>
                                &nbsp;&nbsp;&nbsp;&nbsp;
                                </div>

                            </div>
                       </div>              
                             &nbsp;&nbsp;&nbsp;
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <div class="grid-container">
                                <asp:GridView ID="GridView1" runat="server" EnableViewState="true" AutoGenerateColumns="False" 
                                              CssClass="gridview" Style="width: 100%" PageSize="15" AllowPaging="true" 
                                              OnPageIndexChanging="GridView1_PageIndexChanging">
                                    <Columns>
                                        <asp:BoundField DataField="ReportID" HeaderText="Report ID" SortExpression="ReportID" />
                                        <asp:BoundField DataField="DateTime" HeaderText="Date and Time" SortExpression="DateTime" />
                                        <asp:BoundField DataField="TableName" HeaderText="Table Name" SortExpression="TableName" />
                                        <asp:BoundField DataField="Description" HeaderText="Description" SortExpression="Description" />
                                        <asp:BoundField DataField="Address" HeaderText="Address" SortExpression="Address" />
                                        <asp:BoundField DataField="BuildingName" HeaderText="Building" SortExpression="BuildingName" />
                                        <asp:TemplateField HeaderText="Status">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="statusLabel" Text='<%# Eval("Status") %>' CssClass='<%# GetStatusClass(Eval("Status")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle BackColor="Transparent" />
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        <div class="alert alert-info" id="infoAlert_Department" role="alert">
                                            No results found.
                                        </div>
                                    </EmptyDataTemplate>
                                    <PagerTemplate>
                                        <div class="pager">
                                            <asp:LinkButton ID="FirstPage" CommandName="Page" CommandArgument="First" runat="server">First</asp:LinkButton>
                                            <asp:LinkButton ID="PrevPage" CommandName="Page" CommandArgument="Prev" runat="server"><i class="fa-solid fa-chevron-left"></i></asp:LinkButton>
                                            <asp:Label ID="PageSummary" runat="server" CssClass="summary-pager"></asp:Label>
                                            <asp:LinkButton ID="NextPage" CommandName="Page" CommandArgument="Next" runat="server"><i class="fa-solid fa-chevron-right"></i></asp:LinkButton>
                                            <asp:LinkButton ID="LastPage" CommandName="Page" CommandArgument="Last" runat="server">Last</asp:LinkButton>
                                        </div>
                                    </PagerTemplate>
                                </asp:GridView>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                           <div class="dos-row">
                               <div class="entries-dropdown">
                                <label for="EntriesDropdown">Items per page:</label>
                                <asp:DropDownList ID="EntriesDropdown" runat="server" AutoPostBack="true" OnSelectedIndexChanged="EntriesDropdown_SelectedIndexChanged">
                                    <asp:ListItem Text="15" Value="15"></asp:ListItem>
                                    <asp:ListItem Text="30" Value="30"></asp:ListItem>
                                    <asp:ListItem Text="50" Value="50"></asp:ListItem>
                                    <asp:ListItem Text="100" Value="100"></asp:ListItem>
                                    <asp:ListItem Text="200" Value="200"></asp:ListItem>
                                </asp:DropDownList>
                                </div>

                               <div class="results-label">
                                    <asp:Label ID="resultsLabel" runat="server" Text="Showing 1 to 15 of 20 results"></asp:Label>
                               </div>
                        </div>
                    </div>
                </div>
    </div>
    </div>

    <!-- JavaScript -->
    <script>$(document).ready(function () {
            $('.calendar-icon').click(function () {
                $('#singleDateRange').datepicker({
                    dateFormat: 'dd/mm/yy',
                    showOtherMonths: true,
                    selectOtherMonths: true,
                    showButtonPanel: true,
                    changeMonth: true,
                    changeYear: true,
                    showOn: 'both',
                    buttonText: '<i class="bx bx-calendar"></i>',
                    prevText: '<i class="bx bx-chevron-left"></i>',
                    nextText: '<i class="bx bx-chevron-right"></i>'
                }).datepicker('show');
            });
        });

    </script>

    <script src="https://cdn.jsdelivr.net/npm/pikaday/pikaday.js"></script>

    <script>
        function initializeDatePicker() {
            var datePicker = document.getElementById('<%= singleDateRange.ClientID %>');

        var picker = new Pikaday({
            field: datePicker,
            format: 'YYYY-MM-DD', // Use ISO format for better compatibility
            onSelect: function (date) {
                datePicker.value = picker.toString('YYYY-MM-DD');
                __doPostBack('<%= singleDateRange.ClientID %>', '');
            },
            defaultDate: new Date(),
        });
        }

        // Call the initializeDatePicker function when the DOM is fully loaded
        document.addEventListener('DOMContentLoaded', function () {
            initializeDatePicker();
        });
    </script>

    <script>
        function showToast(rowsDeleted) {
            const toast = document.querySelector(".toast");
            const progress = document.querySelector(".progress");
            const text1 = document.querySelector(".toast .text-1");
            const text2 = document.querySelector(".toast .text-2");
            const iconContainer = document.querySelector(".toast-content .icon");

            // Reset classes and icon for success
            iconContainer.classList.remove("failed");
            iconContainer.classList.add("check");
            iconContainer.innerHTML = '<i class="fa-solid fa-check"></i>';
            iconContainer.style.backgroundColor = "#4caf50";
            text1.textContent = "Success";
            text2.textContent = `${rowsDeleted} row(s) deleted successfully.`;

            toast.classList.add("active");
            progress.classList.add("active");

            setTimeout(() => {
                hideToast();
            }, 10000); // 10 seconds
        }

        function confirmDelete() {
            return confirm('Are you sure you want to delete old data?');
        }

        function showNoDataToDeleteToast() {
            const toast = document.querySelector(".toast");
            const progress = document.querySelector(".progress");
            const text1 = document.querySelector(".toast .text-1");
            const text2 = document.querySelector(".toast .text-2");
            const iconContainer = document.querySelector(".toast-content .icon");

            // Change to failed state
            iconContainer.classList.remove("check");
            iconContainer.classList.add("failed");
            iconContainer.innerHTML = '<i class="fa-solid fa-times"></i>';
            iconContainer.style.backgroundColor = "#c0392b";
            text1.textContent = "Failed";
            text2.textContent = `No data to delete.`;

            // Update progress bar color
            progress.style.backgroundColor = "#c0392b";

            toast.classList.add("active");
            progress.classList.add("active");

            setTimeout(() => {
                hideToast();
            }, 10000); // 10 seconds
        }

        function hideToast() {
            const toast = document.querySelector(".toast");
            const progress = document.querySelector(".progress");

            toast.classList.remove("active");
            progress.classList.remove("active");
        }

        document.addEventListener("DOMContentLoaded", function () {
            document.querySelector(".toast .close").addEventListener("click", () => {
                hideToast();
            });
        });


    </script>

</asp:Content>
