<%@ Page Title="" Language="C#" MasterPageFile="~/pages/Menu.Master" AutoEventWireup="true" CodeBehind="configuration-management.aspx.cs" Inherits="NMS.pages.configuration_management" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <link rel="stylesheet" href="../assets/styles/config-mng.css"/>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/boxicons/2.1.0/css/boxicons.min.css"/>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css"/>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/boxicons/2.1.0/css/boxicons.min.css"/>
       

     <div class="container">
     <div class="main-content">
                  
         <asp:HiddenField ID="hiddenToastMessage" runat="server" />
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

            <div class="top-container">
                <div class="top">
                     <h1><asp:LinkButton ID="refresh" runat="server" Font-Underline="False" ForeColor="Black" OnClick="refresh_Click">Configuration Management</asp:LinkButton></h1>  
                </div>
                <div class="top-buttons">
                    <div class="button-add">
                        <button type="button" onclick="clearSearchAndOpenModal()">
                            <i class='bx bx-plus'></i>
                            Add
                        </button>
                    </div>
                </div>
            </div>
                <!-- TOAST SUCESS -->
            <div class="toast">
                <div class="toast-content">
                    <i class="fa-solid fa-check check"></i>
                    <div class="message">
                        <span class="text text-1">Success</span>
                        <span class="text text-2">Your changes have been saved</span>
                    </div>
                </div>
                <i class="fa-solid fa-xmark close" onclick="closeToast()"></i>
                <div class="progress"></div>
            </div>


                <!-- Add Device Modal -->
                <div class="popup-overlay"  id="popup">
                    <div class="popup">
                        <!-- Radio buttons for categories -->
                        <div class="radio-inputs">
                            <label class="radio">
                                <input type="radio" name="category" value="addDevice" checked=""/>
                                <span class="name">Address</span>
                            </label>
                            <label class="radio">
                                <input type="radio" name="category" value="department"/>
                                <span class="name">Department</span>
                            </label>
                            <label class="radio">
                                <input type="radio" name="category" value="building"/>
                                <span class="name">Building</span>
                            </label>
                             <div class="underline"></div>

                        </div>

                        <br/>
                        <div id="addDeviceFields">
                            <h2>Add Address</h2>
                            <label for="deviceType">Device:</label>
                            <asp:DropDownList ID="DeviceTypeDropDown" runat="server" onchange="toggleFields()">               
                                <asp:ListItem Text="IP Address" Value="IP Address"></asp:ListItem>
                                <asp:ListItem Text="Web Address" Value="Web Address"></asp:ListItem>
                                <asp:ListItem Text="Digital Device" Value="Digital Device"></asp:ListItem>
                            </asp:DropDownList>
                            <br/>

                            <asp:UpdatePanel ID="BuildingUpdatePanel" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <label for="BuildingType">Building:</label>
                                    <asp:DropDownList ID="BuildingDropDown" runat="server" AutoPostBack="True" OnSelectedIndexChanged="BuildingDropDown_SelectedIndexChanged"></asp:DropDownList>
                                    <br />
                                    <label for="DepartmentDropDown">Department:</label>
                                    <asp:DropDownList ID="DepartmentDropDown" runat="server"></asp:DropDownList>
                                    <br />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="BuildingDropDown" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>

                            <label for="InvervalType">Time Interval:</label>
                            <asp:TextBox ID="IntervalTextBox" runat="server" type="text" name="interval" placeholder="Enter Interval" CssClass="interval-textbox"></asp:TextBox>
                            <asp:DropDownList ID="IntervalUnitDropDown" runat="server" CssClass="interval-dropdown">
                                <asp:ListItem Text="seconds" Value="seconds"></asp:ListItem>
                                <asp:ListItem Text="minutes" Value="minutes"></asp:ListItem>
                                <asp:ListItem Text="hours" Value="hours"></asp:ListItem>
                            </asp:DropDownList>
                            <br/>

                            <div id="ipFields" style="display: none;">
                                <label for="deviceIP">IP Address:</label>
                                <asp:TextBox ID="ipTextBox" runat="server" type="text" name="locationIP" placeholder="Enter IP Address"></asp:TextBox>
                                <br />
                                <label for="description">Description:</label>
                                <asp:TextBox ID="DescriptionTextbox" runat="server" type="text" name="description" placeholder="Enter Description"></asp:TextBox>
                                <br />
                            </div>

                            <div id="websiteFields" style="display: none;">
                                <label for="websiteURL">Website Address:</label>
                                <asp:TextBox ID="webAddressTextBox" runat="server" type="text" name="websiteURL" placeholder="Enter Website URL"></asp:TextBox>
                            </div>
                            <div id="deviceFields" style="display: none;">
                                <label for="deviceName">Device Name:</label>
                                <asp:TextBox ID="deviceNameTextBox" runat="server" type="text" name="deviceName" placeholder="Enter Device Name"></asp:TextBox>
                                <br/>
                                <label for="deviceIP">IP Address:</label>
                                <asp:TextBox ID="deviceIPTextBox" runat="server" type="text" name="deviceIP" placeholder="Enter IP Address"></asp:TextBox>
                            </div>
                        </div>

                        <!-- DEPARTMENT-->
                        <div id="departmentFields" style="display: none;">      
                              <h2>Add Department</h2>
                            <label for="DepartmentBuildingDropDown">Building:</label>
                            <asp:DropDownList ID="DepartmentBuildingDropDown" runat="server"></asp:DropDownList>
                            <br />
                             <label for="department">Department:</label>
                             <asp:TextBox ID="AddDepartment" runat="server" type="text" name="department" placeholder="Enter Department"></asp:TextBox>
                        </div>
                        <!--BUILDING-->
                        <div id="buildingFields" style="display: none;">       
                             <h2>Add Building</h2>
                            <label for="building">Building:</label>
                            <asp:TextBox ID="AddBuilding" runat="server" type="text" name="building" placeholder="Enter Building"></asp:TextBox>
                        </div>

                        <!-- Error label container -->
                        <div class="auto-style1">
                            <asp:Label ID="errorLabel" runat="server" ForeColor="Red" CssClass="auto-style2"></asp:Label>
                        </div>
                        <br/>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <div class="button-group">
                                    <button type="button" class="exit-btn" onclick="closeModal()">Cancel</button>
                                    <asp:Button ID="addDevice" class="check-btn" runat="server" Text="Create" OnClick="AddDevice_Click" />
                                </div>

                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="addDevice" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>               
                    </div>
            </div>

             <div class="main-container">
                <div class="align-container">

                    <div class="uno-row">
                
                        <div class="custom-search-bar">
                            <i class='bx bx-search'></i>
                            <asp:TextBox ID="searchtxtbox" placeholder="Search..." runat="server" OnTextChanged="searchtxtbox_TextChanged" AutoPostBack="true"></asp:TextBox>
                
                         </div>
                                 
                            <div class="select-device">
                                <asp:DropDownList ID="DropDownList3" runat="server" AutoPostBack="true" Width="170px" OnSelectedIndexChanged="DropDownList3_SelectedIndexChanged">
                                    <asp:ListItem Text="All" Value="showall"></asp:ListItem>
                                    <asp:ListItem Text="Departments" Value="Department"></asp:ListItem>
                                    <asp:ListItem Text="Buildings" Value="Building"></asp:ListItem>
                                </asp:DropDownList>
                                &nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="true" Width="170px" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
                                    <asp:ListItem Text="Configure:" Value="showall"></asp:ListItem>
                                    <asp:ListItem Text="Device Address" Value="Digital Device"></asp:ListItem>
                                    <asp:ListItem Text="IP Address" Value="IP Address"></asp:ListItem>
                                    <asp:ListItem Text="Web Address" Value="Web Address"></asp:ListItem>
                                </asp:DropDownList>
                            </div>

                        </div>
                   
                       <div class="dos-row">
                           <div class="entries-dropdown">
                            <label for="EntriesDropdown">Items per page:</label>
                            <asp:DropDownList ID="EntriesDropdown" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DropDownList2_SelectedIndexChanged">
                                <asp:ListItem Text="10" Value="10"></asp:ListItem>
                                <asp:ListItem Text="20" Value="20"></asp:ListItem>
                                <asp:ListItem Text="50" Value="50"></asp:ListItem>
                                <asp:ListItem Text="100" Value="100"></asp:ListItem>
                                <asp:ListItem Text="200" Value="200"></asp:ListItem>
                            </asp:DropDownList>
                            </div>

                           <div class="results-label">
                                <asp:Label ID="resultsLabel" runat="server" Text="Showing 1 to 10 of 20 results"></asp:Label>
                           </div>
                    </div>


                </div>
            </div> 

        <!-- Department GridView -->
        <div class="grid-container">
            <asp:GridView ID="DepartmentGridView" runat="server" EnableViewState="true" AutoGenerateColumns="False" OnRowDeleting="DepartmentGridView_RowDeleting"
                OnPageIndexChanging="DepartmentGridView_PageIndexChanging" PageSize="12" AllowPaging="true" DataKeyNames="DepartmentID"
                CssClass="gridview">
                <Columns>
                    <asp:BoundField DataField="DepartmentID" HeaderText="Department ID" visible="false"/>
                    <asp:BoundField DataField="DepartmentName" HeaderText="Department Name" />
                    <asp:BoundField DataField="BuildingName" HeaderText="Building Name" />
                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <asp:LinkButton ID="EditDepartmentButton" runat="server" CssClass="edit-btn"
                                OnClientClick='<%# "showUpdateDepartmentModal(\"" + Eval("DepartmentID") + "\", \"" + Eval("DepartmentName") + "\", \"" + Eval("BuildingName") + "\"); return false;" %>'>
                                <i class='bx bxs-pencil'></i>
                            </asp:LinkButton>
                            <asp:LinkButton ID="DeleteDepartmentButton" runat="server" CommandName="Delete" CssClass="delete-btn"
                               OnClientClick="return confirm('Are you sure you want to delete this item?\nThis action will permanently delete all related data and cannot be undone.');">
                                <i class='bx bxs-trash'></i>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
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

        <!-- Building GridView -->
        <div class="grid-container">
            <asp:GridView ID="BuildingGridView" runat="server" EnableViewState="true" AutoGenerateColumns="False"
                OnRowDeleting="BuildingGridView_RowDeleting" OnPageIndexChanging="BuildingGridView_PageIndexChanging"
                PageSize="12" AllowPaging="true" DataKeyNames="BuildingID" CssClass="gridview">
                <Columns>
                    <asp:BoundField DataField="BuildingID" HeaderText="Building ID" Visible="false" />
                    <asp:BoundField DataField="BuildingName" HeaderText="Building Name" />
                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <asp:LinkButton ID="EditBuildingButton" runat="server" CssClass="edit-btn"
                                OnClientClick='<%# "showUpdateBuildingModal(\"" + Eval("BuildingID") + "\", \"" + Eval("BuildingName") + "\"); return false;" %>'>
                                <i class='bx bxs-pencil'></i>
                            </asp:LinkButton>
                            <asp:LinkButton ID="DeleteBuildingButton" runat="server" CommandName="Delete" CssClass="delete-btn"
                                OnClientClick="return confirm('Are you sure you want to delete this item?\nThis action will permanently delete all related data and cannot be undone.');">
                                <i class='bx bxs-trash'></i>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
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


        <!-- Modal for Department GridView -->
        <div class="popup-update" id="DepartmentModal" style="display:none">
            <div class="popup">
                <h2>Update Department</h2>
                <asp:UpdatePanel ID="DepartmentUpdatePanel" runat="server">
                    <ContentTemplate>
                        <label for="BuildingType">Building:</label>
                            <asp:TextBox ID="updateBuildingHolder" runat="server" type="text" name="BuildingHolder" placeholder="Building" Enabled="False"  CssClass="interval-textbox"></asp:TextBox>
                            <asp:DropDownList ID="updateBuildingDropDown" runat="server" AutoPostBack="True" OnSelectedIndexChanged="UpdateBuildingDropDown_SelectedIndexChanged" CssClass="interval-dropdown">
                            </asp:DropDownList>
                        <label>Department:</label>
                        <asp:TextBox ID="updateDept" runat="server" placeholder="Enter Department"></asp:TextBox>
                        <br />
                        <div>
                            <asp:Label ID="departmentErrorLabel" runat="server" ForeColor="Red" CssClass="auto-style2"></asp:Label>
                        </div>
                        <br />
                        <asp:HiddenField ID="DepartmentHiddenField" runat="server" />
                        <div class="button-group">
                            <button type="button" class="exit-btn" onclick="closeEditModal('DepartmentModal');">Cancel</button>
                            <asp:Button ID="SaveDepartment" class="check-btn" runat="server" Text="Save" OnClick="SaveUpdateDepartment" />
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="SaveDepartment" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>

            <!-- Modal for Building GridView -->
        <div class="popup-update" id="BuildingModal" style="display:none">
            <div class="popup">
                <h2>Update Building</h2>
                <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                    <ContentTemplate>
                        <label>Building:</label>
                        <asp:TextBox ID="updateBuilding" runat="server" placeholder="Enter Building"></asp:TextBox>
                        <br />
                        <div>
                            <asp:Label ID="buildingErrorLabel" runat="server" ForeColor="Red" CssClass="auto-style2"></asp:Label>
                        </div>
                        <br />
                        <asp:HiddenField ID="BuildingHiddenField" runat="server" />
                        <div class="button-group">
                            <button type="button" class="exit-btn" onclick="closeEditModal('BuildingModal');">Cancel</button>
                            <asp:Button ID="SaveBuilding" class="check-btn" runat="server" Text="Save" OnClick="SaveUpdateBuilding" />
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="SaveBuilding" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>

               <!-- ALL Gridview -->
                <div class="grid-container">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <asp:GridView ID="GridView1" runat="server" EnableViewState="true" AutoGenerateColumns="False"
                                OnPageIndexChanging="GridView1_PageIndexChanging" PageSize="10" AllowPaging="true"
                                CssClass="gridview">
                                <Columns>
                                    <asp:BoundField DataField="Type" HeaderText="Type" Visible="false" SortExpression="Type"/>
                                    <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name"/>
                                    <asp:BoundField DataField="Address" HeaderText="Address" SortExpression="Address"/>
                                    <asp:BoundField DataField="RefreshInterval" HeaderText="Interval" SortExpression="RefreshInterval" />
                                    <asp:BoundField DataField="IntervalUnit" HeaderText="Interval Unit" SortExpression="IntervalUnit"/>
                                </Columns>
                                <%-- No Results --%>
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
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>

            <!-- Digital Device -->
            <div class="grid-container">
                <asp:GridView ID="GridView2" runat="server" DataKeyNames="DigitalDevID" EnableViewState="true" AutoGenerateColumns="False"
                    OnPageIndexChanging="GridView2_PageIndexChanging" OnRowDeleting="GridView2_RowDeleting" PageSize="10" AllowPaging="true" CssClass="gridview">
                    <Columns>
                        <asp:TemplateField HeaderText="Device Name" SortExpression="Name">
                            <ItemTemplate>
                                <asp:Label ID="lblDeviceName" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Device IP" SortExpression="Address">
                            <ItemTemplate>
                                <asp:Label ID="lblDeviceIP" runat="server" Text='<%# Eval("Address") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                         <asp:TemplateField HeaderText="Building" SortExpression="Building">
                             <ItemTemplate>
                                 <asp:Label ID="lblBuilding" runat="server" Text='<%# Eval("Building") %>'></asp:Label>
                             </ItemTemplate>
                         </asp:TemplateField>
                         <asp:TemplateField HeaderText="Department" SortExpression="Department">
                             <ItemTemplate>
                                 <asp:Label ID="lblDepartment" runat="server" Text='<%# Eval("Department") %>'></asp:Label>
                             </ItemTemplate>
                         </asp:TemplateField>
                        <asp:BoundField DataField="RefreshInterval" HeaderText="Interval" />
                        <asp:BoundField DataField="IntervalUnit" HeaderText="Interval Unit" />
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <asp:LinkButton ID="LinkButton5" runat="server" CssClass="edit-btn"
                                    OnClientClick='<%# "showDeviceModal(\"" + Eval("DigitalDevID") + "\", \"" + Eval("Name") + "\", \"" + Eval("Address") + "\", " + Eval("RefreshInterval") + ", \"" + Eval("IntervalUnit") + "\", \"" + Eval("Building") + "\", \"" + Eval("Department") + "\"); return false;" %>'>
                                    <i class='bx bxs-pencil'></i>
                                </asp:LinkButton>
                                <asp:LinkButton ID="LinkButton2" runat="server" CommandName="Delete" CssClass="delete-btn"
                                    OnClientClick="return confirm('Are you sure you want to delete this item?');">
                                    <i class='bx bxs-trash'></i>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
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

             <!-- IP Address -->
             <div class="grid-container">
                 <asp:GridView ID="GridView3" runat="server" DataKeyNames="IPAddressID" EnableViewState="true" AutoGenerateColumns="False"
                     OnPageIndexChanging="GridView3_PageIndexChanging" OnRowDeleting="GridView3_RowDeleting" PageSize="10" AllowPaging="true" CssClass="gridview">
                     <Columns>
                         <asp:TemplateField HeaderText="Building" SortExpression="Building">
                             <ItemTemplate>
                                 <asp:Label ID="lblBuilding" runat="server" Text='<%# Eval("Building") %>'></asp:Label>
                             </ItemTemplate>
                         </asp:TemplateField>
                         <asp:TemplateField HeaderText="Department" SortExpression="Department">
                             <ItemTemplate>
                                 <asp:Label ID="lblDepartment" runat="server" Text='<%# Eval("Department") %>'></asp:Label>
                             </ItemTemplate>
                         </asp:TemplateField>
                         <asp:TemplateField HeaderText="IP Address" SortExpression="IPAddress">
                             <ItemTemplate>
                                 <asp:Label ID="lblIPAddress" runat="server" Text='<%# Eval("IPAddress") %>'></asp:Label>
                             </ItemTemplate>
                         </asp:TemplateField>
                         <asp:BoundField DataField="Description" HeaderText="Description" />
                         <asp:BoundField DataField="RefreshInterval" HeaderText="Interval" />
                         <asp:BoundField DataField="IntervalUnit" HeaderText="Interval Unit" />
                         <asp:TemplateField HeaderText="Actions">
                             <ItemTemplate>
                                 <asp:LinkButton ID="LinkButton3" runat="server" CssClass="edit-btn"
                                     OnClientClick='<%# "showDepartmentModal(\"" + Eval("IPAddressID") + "\", \"" + Eval("Department") + "\", \"" + Eval("IPAddress") + "\", \"" + Eval("Description") + "\", " + Eval("RefreshInterval") + ", \"" + Eval("IntervalUnit") + "\", \"" + Eval("Building") + "\"); return false;" %>'>
                                     <i class='bx bxs-pencil'></i>
                                 </asp:LinkButton>
                                 <asp:LinkButton ID="LinkButton4" runat="server" CommandName="Delete" CssClass="delete-btn"
                                     OnClientClick="return confirm('Are you sure you want to delete this item?');">
                                     <i class='bx bxs-trash'></i>
                                 </asp:LinkButton>
                             </ItemTemplate>
                         </asp:TemplateField>
                     </Columns>
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


            <!-- WebAddress -->
            <div class="grid-container">
                <asp:GridView ID="GridView4" runat="server" DataKeyNames="WebID" EnableViewState="true" AutoGenerateColumns="False"
                    OnPageIndexChanging="GridView4_PageIndexChanging" OnRowDeleting="GridView4_RowDeleting" PageSize="10" AllowPaging="true" CssClass="gridview">
                    <Columns>
                        <asp:TemplateField HeaderText="Web Address" SortExpression="Name">
                            <ItemTemplate>
                                <asp:Label ID="lblWebAddress" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Building" SortExpression="Building">
                            <ItemTemplate>
                                <asp:Label ID="lblBuilding" runat="server" Text='<%# Eval("Building") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Department" SortExpression="Department">
                            <ItemTemplate>
                                <asp:Label ID="lblDepartment" runat="server" Text='<%# Eval("Department") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="RefreshInterval" HeaderText="Interval" />
                        <asp:BoundField DataField="IntervalUnit" HeaderText="Interval Unit" />
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnEdit" runat="server" CssClass="edit-btn"
                                    OnClientClick='<%# "showWebModal(\"" + Eval("WebID") + "\", \"" + Eval("Name") + "\", \"" + Eval("RefreshInterval") + "\", \"" + Eval("IntervalUnit") + "\", \"" + Eval("Building") + "\", \"" + Eval("Department") + "\"); return false;" %>'>
                                    <i class='bx bxs-pencil'></i>
                                </asp:LinkButton>

                                <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete" CssClass="delete-btn"
                                    OnClientClick="return confirm('Are you sure you want to delete this item?');">
                                    <i class='bx bxs-trash'></i>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
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



        <!-- MODALS THREE -->

       <!-- EDIT IP Address Modal -->
        <div class="popup-update" id="ipAddressModal" style="display:none">
            <div class="popup">
                <h2>Update IP Address</h2>
                <!-- IP -->
                <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <label for="BuildingType">Building:</label>
                        <asp:TextBox ID="BuildingHolder" runat="server" type="text" name="BuildingHolder" placeholder="Building" Enabled="False" CssClass="interval-textbox"></asp:TextBox>
                        <asp:DropDownList ID="EditBuildingDropDown" runat="server" AutoPostBack="True" OnSelectedIndexChanged="EditBuildingDropDown_SelectedIndexChanged" CssClass="interval-dropdown">
                        </asp:DropDownList>
                        <br />
                        <label for="deviceLocation">Department:</label>
                        <asp:TextBox ID="DeptHolder" runat="server" type="text" name="DeptHolder" placeholder="Department" Enabled="False" CssClass="interval-textbox"></asp:TextBox>
                        <asp:DropDownList ID="EditDeptDropDown" runat="server" CssClass="interval-dropdown">
                        </asp:DropDownList>
                        <br />
                        <label for="deviceIP">IP Address:</label>
                        <asp:TextBox ID="EditLocTextbox" runat="server" type="text" name="locationIP" placeholder="Enter IP Address"></asp:TextBox>
                        <br />
                        <label for="description">Description:</label>
                        <asp:TextBox ID="EditDescriptionTextbox" runat="server" type="text" name="description" placeholder="Enter Description"></asp:TextBox>
                        <br />
                        <label for="interval">Interval:</label>
                        <asp:TextBox ID="EditIPInterval" runat="server" type="text" name="interval" placeholder="Enter Interval" CssClass="interval-textbox"></asp:TextBox>
                        <asp:DropDownList ID="EditIPUnit" runat="server" CssClass="interval-dropdown">
                            <asp:ListItem Text="seconds" Value="seconds"></asp:ListItem>
                            <asp:ListItem Text="minutes" Value="minutes"></asp:ListItem>
                            <asp:ListItem Text="hours" Value="hours"></asp:ListItem>
                        </asp:DropDownList>
                        <br />
                
                        <!-- Error label container -->
                        <div class="auto-style1">
                            <asp:Label ID="ipErrorLabel" runat="server" ForeColor="Red" CssClass="auto-style2"></asp:Label>
                        </div>
                        <br />
                        <!-- Hidden fields for BuildingID and DepartmentID -->
                        <asp:HiddenField ID="hiddenBuildingID" runat="server" />
                        <asp:HiddenField ID="hiddenDepartmentID" runat="server" />
                        <!-- Hidden field for IPAddressID -->
                        <asp:HiddenField ID="hiddenIPAddressID" runat="server" />
                        <div class="button-group">
                            <button type="button" class="exit-btn" onclick="closeEditModal('ipAddressModal');">Cancel</button>
                            <asp:Button ID="addIpButton" class="check-btn" runat="server" Text="Save" OnClick="SaveIpAddress" />
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="addIpButton" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="EditBuildingDropDown" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>


           <!-- DEVICE -->
            <div class="popup-update" id="digitalDeviceModal" style="display:none">
                <div class="popup">
                    <h2>Update Digital Device</h2>
                    <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <label for="BuildingType">Building:</label>
                            <asp:TextBox ID="DeviceBuildingHolder" runat="server" type="text" name="BuildingHolder" placeholder="Building" Enabled="False" CssClass="interval-textbox"></asp:TextBox>
                            <asp:DropDownList ID="EditBuildingDropDownDevice" runat="server" CssClass="interval-dropdown" AutoPostBack="True" OnSelectedIndexChanged="EditBuildingDropDownDevice_SelectedIndexChanged">
                            </asp:DropDownList>
                            <br />
                            <label for="deviceLocation">Department:</label>
                            <asp:TextBox ID="DeviceDeptHolder" runat="server" type="text" name="DeptHolder" placeholder="Department" Enabled="False" CssClass="interval-textbox"></asp:TextBox>
                            <asp:DropDownList ID="EditDeptDropDownDevice" runat="server" CssClass="interval-dropdown">
                            </asp:DropDownList>
                            <br />
                            <label for="deviceName">Device Name:</label>
                            <asp:TextBox ID="EditDeviceName" runat="server" type="text" name="deviceName" placeholder="Enter Device Name"></asp:TextBox>
                            <br />
                            <label for="deviceIP">IP Address:</label>
                            <asp:TextBox ID="EditDevIPTextbox" runat="server" type="text" name="deviceIP" placeholder="Enter IP Address"></asp:TextBox>
                            <br />
                            <label for="interval">Interval:</label>
                            <asp:TextBox ID="EditDeviceInterval" runat="server" type="text" name="interval" placeholder="Enter Interval" CssClass="interval-textbox"></asp:TextBox>
                            <asp:DropDownList ID="EditDeviceUnit" runat="server" CssClass="interval-dropdown">
                                <asp:ListItem Text="seconds" Value="seconds"></asp:ListItem>
                                <asp:ListItem Text="minutes" Value="minutes"></asp:ListItem>
                                <asp:ListItem Text="hours" Value="hours"></asp:ListItem>
                            </asp:DropDownList>
                            <br />
                            <!-- Error label container -->
                            <div>
                                <asp:Label ID="digitalDeviceErrorLabel" runat="server" ForeColor="Red" CssClass="auto-style2"></asp:Label>
                            </div>
                            <br />
                            <!-- Hidden fields for BuildingID and DepartmentID -->
                            <asp:HiddenField ID="hiddenDeviceBuildingID" runat="server" />
                            <asp:HiddenField ID="hiddenDeviceDeptID" runat="server" />
                            <!-- Hidden field for DeviceID -->
                            <asp:HiddenField ID="hiddenDeviceID" runat="server" />
                            <div class="button-group">
                                <button type="button" class="exit-btn" onclick="closeEditModal('digitalDeviceModal');">Cancel</button>
                                <asp:Button ID="addDigitalDeviceButton" class="check-btn" runat="server" Text="Save" OnClick="SaveDigitalDevice" />
                            </div>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="addDigitalDeviceButton" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="EditBuildingDropDownDevice" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </div>



           <!-- WEB -->
            <div class="popup-update" id="webAddressModal" style="display:none">
            <div class="popup">
                <h2>Update Web Address</h2>
                <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>                       
                        <label for="BuildingType">Building:</label>
                        <asp:TextBox ID="WebBuildingHolder" runat="server" type="text" name="BuildingHolder" placeholder="Building" Enabled="False" CssClass="interval-textbox"></asp:TextBox>
                        <asp:DropDownList ID="EditBuildingDropDownWeb" runat="server" CssClass="interval-dropdown" AutoPostBack="True" OnSelectedIndexChanged="EditBuildingDropDownWeb_SelectedIndexChanged">
                        </asp:DropDownList>
                        <br />
                        <label for="deviceLocation">Department:</label>
                        <asp:TextBox ID="WebDeptHolder" runat="server" type="text" name="DeptHolder" placeholder="Department" Enabled="False" CssClass="interval-textbox"></asp:TextBox>
                        <asp:DropDownList ID="EditDeptDropDownWeb" runat="server" CssClass="interval-dropdown" AutoPostBack="True">
                        </asp:DropDownList>
                        <label for="websiteURL">Website Address:</label>
                        <asp:TextBox ID="EditWebTextbox" runat="server" type="text" name="websiteURL" placeholder="Enter Website URL"></asp:TextBox>
                        <br />
                        <br />
                        <label for="interval">Interval:</label>
                        <asp:TextBox ID="EditWebInterval" runat="server" type="text" name="interval" placeholder="Enter Interval" CssClass="interval-textbox"></asp:TextBox>
                        <asp:DropDownList ID="EditWebUnit" runat="server" CssClass="interval-dropdown">
                            <asp:ListItem Text="seconds" Value="seconds"></asp:ListItem>
                            <asp:ListItem Text="minutes" Value="minutes"></asp:ListItem>
                            <asp:ListItem Text="hours" Value="hours"></asp:ListItem>
                        </asp:DropDownList>
                        <br />
                        <!-- Error label container -->
                        <div>
                            <asp:Label ID="webAddressErrorLabel" runat="server" ForeColor="Red" CssClass="auto-style2"></asp:Label>
                        </div>
                        <br />
                        <!-- Hidden fields for BuildingID and DepartmentID -->
                        <asp:HiddenField ID="hiddenWebBuildingID" runat="server" />
                        <asp:HiddenField ID="hiddenWebDeptID" runat="server" />
                        <!-- Hidden field for WebID -->
                        <asp:HiddenField ID="hiddenWebID" runat="server" />
                        <div class="button-group">
                            <button type="button" class="exit-btn" onclick="closeEditModal('webAddressModal');">Cancel</button>
                            <asp:Button ID="addWebAddressButton" class="check-btn" runat="server" Text="Save" OnClick="SaveWebAddress" />
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="addWebAddressButton" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="EditBuildingDropDownWeb" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>


     </div>
     </div>

<!-- JS -->
    <!-- FOR THREE EDIT MODALS -->
<script>
    function showDeviceModal(deviceID, deviceName, ipAddress, refreshInterval, intervalUnit, buildingID, departmentID) {
        showModal("digitalDeviceModal");

        // Set hidden field value
        document.getElementById('<%= hiddenDeviceID.ClientID %>').value = deviceID;
        document.getElementById('<%= hiddenDeviceBuildingID.ClientID %>').value = buildingID;
        document.getElementById('<%= hiddenDeviceDeptID.ClientID %>').value = departmentID;

        // Set values in the form fields
        document.getElementById('<%= EditDeviceName.ClientID %>').value = deviceName;
        document.getElementById('<%= EditDevIPTextbox.ClientID %>').value = ipAddress;
        document.getElementById('<%= EditDeviceInterval.ClientID %>').value = refreshInterval;

        // Set the dropdown value
        var dropdown = document.getElementById('<%= EditDeviceUnit.ClientID %>');
        for (var i = 0; i < dropdown.options.length; i++) {
            if (dropdown.options[i].value === intervalUnit) {
                dropdown.options[i].selected = true;
                break;
            }
        }

        // Set the BuildingHolder and DeptHolder values to display the current values
        document.getElementById('<%= DeviceBuildingHolder.ClientID %>').value = buildingID;
        document.getElementById('<%= DeviceDeptHolder.ClientID %>').value = departmentID;
    }


    function showDepartmentModal(ipAddressID, departmentID, ipAddress, description, refreshInterval, intervalUnit, buildingID) {
        showModal("ipAddressModal");

        // Set hidden field values
        document.getElementById('<%= hiddenIPAddressID.ClientID %>').value = ipAddressID;
        document.getElementById('<%= hiddenBuildingID.ClientID %>').value = buildingID;
        document.getElementById('<%= hiddenDepartmentID.ClientID %>').value = departmentID;

        // Set IP address and refresh interval values
        document.getElementById('<%= EditLocTextbox.ClientID %>').value = ipAddress;
        document.getElementById('<%= EditDescriptionTextbox.ClientID %>').value = description;
        document.getElementById('<%= EditIPInterval.ClientID %>').value = refreshInterval;

        // Set interval unit dropdown value
        var intervalDropdown = document.getElementById('<%= EditIPUnit.ClientID %>');
        for (var i = 0; i < intervalDropdown.options.length; i++) {
            if (intervalDropdown.options[i].value === intervalUnit) {
                intervalDropdown.options[i].selected = true;
                break;
            }
        }

        // Set the BuildingHolder and DeptHolder values to display the current values
        document.getElementById('<%= BuildingHolder.ClientID %>').value = buildingID;
        document.getElementById('<%= DeptHolder.ClientID %>').value = departmentID;
    }

    function showWebModal(webID, websiteURL, refreshInterval, intervalUnit, buildingID, departmentID) {
        showModal("webAddressModal");

        // Set hidden field value
        document.getElementById('<%= hiddenWebID.ClientID %>').value = webID;
        document.getElementById('<%= hiddenWebBuildingID.ClientID %>').value = buildingID;
        document.getElementById('<%= hiddenWebDeptID.ClientID %>').value = departmentID;

        // Set values in the form fields
        document.getElementById('<%= EditWebTextbox.ClientID %>').value = websiteURL;
        document.getElementById('<%= EditWebInterval.ClientID %>').value = refreshInterval;

        // Set the dropdown value
        var dropdown = document.getElementById('<%= EditWebUnit.ClientID %>');
        for (var i = 0; i < dropdown.options.length; i++) {
            if (dropdown.options[i].value === intervalUnit) {
                dropdown.options[i].selected = true;
                break;
            }
        }

        // Set the BuildingHolder and DeptHolder values to display the current values
        document.getElementById('<%= WebBuildingHolder.ClientID %>').value = buildingID;
        document.getElementById('<%= WebDeptHolder.ClientID %>').value = departmentID;
    }


    function showModal(modalId) {
        // Hide all modals
        var modals = document.getElementsByClassName("popup-update");
        for (var i = 0; i < modals.length; i++) {
            modals[i].style.display = "none";
        }
        // Show the modal with the given ID
        document.getElementById(modalId).style.display = "block";
    }

    function showUpdateDepartmentModal(departmentID, departmentName, buildingID) {
        showModal("DepartmentModal");

        // Set values in the form fields
        document.getElementById('<%= updateDept.ClientID %>').value = departmentName;
        document.getElementById('<%= updateBuildingHolder.ClientID %>').value = buildingID;

        // Set hidden field value
        document.getElementById('<%= DepartmentHiddenField.ClientID %>').value = departmentID;
    }

    function showUpdateBuildingModal(buildingID, buildingName) {
        showModal("BuildingModal");

        // Set values in the form fields
        document.getElementById('<%= updateBuilding.ClientID %>').value = buildingName;

        // Set hidden field value
        document.getElementById('<%= BuildingHiddenField.ClientID %>').value = buildingID;
    }

    function confirmDelete(message, deleteFunction) {
        if (confirm(message)) {
            deleteFunction();
        }
    }

    function closeEditModal(modalId) {
        document.getElementById(modalId).style.display = "none";
        clearErrorLabel(labelId);
    }

    function hideErrorLabel(labelId) {
        var errorLabel = document.getElementById(labelId);
        if (errorLabel) {
            errorLabel.style.display = 'none';
        }
    }

    function showErrorLabel(labelId, message) {
        var errorLabel = document.getElementById(labelId);
        if (errorLabel) {
            errorLabel.innerText = message;
            errorLabel.style.display = 'block';
            setTimeout(function () {
                hideErrorLabel(labelId);
            }, 5000); // Hide after 5 seconds
        }
    }
    function clearErrorLabel(labelId) {
        var errorLabel = document.getElementById(labelId);
        if (errorLabel) {
            errorLabel.innerText = '';
            errorLabel.style.display = 'none';
        }
    }

</script>

<!-- FOR ADD DEVICE MODAL -->
<script>
    function clearSearchAndOpenModal() {
        document.getElementById('<%= searchtxtbox.ClientID %>').value = '';
        clearErrorLabel();
        openModal();
    }
    function openModal() {
        document.getElementById('popup').classList.add('active');
        toggleFields(); // Call toggleFields function when the modal is opened
    }

    function closeModal() {
        document.getElementById('popup').classList.remove('active');
        clearTextBoxes();
    }

    function toggleFields() {
        var deviceType = document.getElementById('<%= DeviceTypeDropDown.ClientID %>').value;
        var ipFields = document.getElementById('ipFields');
        var websiteFields = document.getElementById('websiteFields');
        var deviceFields = document.getElementById('deviceFields');

        ipFields.style.display = 'none';
        websiteFields.style.display = 'none';
        deviceFields.style.display = 'none';

        if (deviceType === 'IP Address') {
            ipFields.style.display = 'block';
        } else if (deviceType === 'Web Address') {
            websiteFields.style.display = 'block';
        } else if (deviceType === 'Digital Device') {
            deviceFields.style.display = 'block';
        }
    }

    function addDevice() {
        const toast = document.querySelector(".toast");
        const progress = document.querySelector(".progress");
        let timer1, timer2;

        toast.classList.add("active");
        progress.classList.add("active");

        timer1 = setTimeout(() => {
            toast.classList.remove("active");
        }, 5000);

        timer2 = setTimeout(() => {
            progress.classList.remove("active");
        }, 5300);
    }
    document.addEventListener('DOMContentLoaded', function () {
        var toastMessage = document.getElementById('<%= hiddenToastMessage.ClientID %>').value;
        if (toastMessage) {
            showToast(toastMessage);
            document.getElementById('<%= hiddenToastMessage.ClientID %>').value = ''; // Clear the hidden field value
        }
    });

    function showToast(message) {
        const toast = document.querySelector(".toast");
        const progress = document.querySelector(".progress");
        const text1 = document.querySelector(".toast .text-1");
        const text2 = document.querySelector(".toast .text-2");

        text1.textContent = "Success";
        text2.textContent = message;

        toast.classList.add("active");
        progress.classList.add("active");

        setTimeout(() => {
            toast.classList.remove("active");
        }, 5000);

        setTimeout(() => {
            progress.classList.remove("active");
        }, 5300);
    }

    function closeToast() {
        const toast = document.querySelector(".toast");
        const progress = document.querySelector(".progress");
        toast.classList.remove("active");
        setTimeout(() => {
            progress.classList.remove("active");
        }, 300);
    }

    function hideErrorLabel() {
        var errorLabel = document.getElementById('<%= errorLabel.ClientID %>');
        if (errorLabel) {
            errorLabel.style.display = 'none';
        }
    }

    function showErrorLabel(message) {
        var errorLabel = document.getElementById('<%= errorLabel.ClientID %>');
        if (errorLabel) {
            errorLabel.innerText = message;
            errorLabel.style.display = 'block';
            setTimeout(hideErrorLabel, 5000); // Hide after 5 seconds
        }
    }
    function clearErrorLabel() {
        var errorLabel = document.getElementById('<%= errorLabel.ClientID %>');
        if (errorLabel) {
            errorLabel.innerText = '';
            errorLabel.style.display = 'none';
        }
    }
    function clearTextBoxes() {
        document.querySelectorAll('input[type=text]').forEach(input => input.value = '');
    }

</script>

    <!-- FOR RADIO BUTTONS -->
<script>
    document.addEventListener('DOMContentLoaded', () => {
        const radioInputs = document.querySelectorAll('.radio-inputs .radio input');

        radioInputs.forEach(input => {
            input.addEventListener('change', () => {
                const category = input.value;
                toggleCategory(category);
                if (category !== 'department') {
                    clearDepartmentFields();
                }
                if (category !== 'addDevice') {
                    clearAddDeviceFields();
                }
                if (category !== 'building') {
                    clearBuildingFields();
                }
            });
        });

        // Initialize the default state
        const defaultCheckedInput = document.querySelector('.radio-inputs .radio input:checked');
        if (defaultCheckedInput) {
            toggleCategory(defaultCheckedInput.value);
        }
    });


    function toggleCategory(category) {
        document.getElementById('addDeviceFields').style.display = category === 'addDevice' ? 'block' : 'none';
        document.getElementById('buildingFields').style.display = category === 'building' ? 'block' : 'none';
        document.getElementById('departmentFields').style.display = category === 'department' ? 'block' : 'none';
    }

    function clearAddDeviceFields() {
        document.querySelectorAll('#addDeviceFields input[type=text]').forEach(input => input.value = '');
        document.getElementById('<%= BuildingDropDown.ClientID %>').selectedIndex = 0;
        document.getElementById('<%= DepartmentDropDown.ClientID %>').selectedIndex = 0;
    }

    function clearDepartmentFields() {
        document.getElementById('<%= DepartmentBuildingDropDown.ClientID %>').selectedIndex = 0;
        document.getElementById('<%= AddDepartment.ClientID %>').value = '';
    }

    function clearBuildingFields() {
        document.getElementById('<%= AddBuilding.ClientID %>').value = '';
    }

        function toggleFields() {
            var deviceType = document.getElementById('<%= DeviceTypeDropDown.ClientID %>').value;
        document.getElementById('ipFields').style.display = deviceType === 'IP Address' ? 'block' : 'none';
        document.getElementById('websiteFields').style.display = deviceType === 'Web Address' ? 'block' : 'none';
        document.getElementById('deviceFields').style.display = deviceType === 'Digital Device' ? 'block' : 'none';
    }

</script>

</asp:Content>