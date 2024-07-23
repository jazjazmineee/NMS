<%@ Page Title="" Language="C#" MasterPageFile="~/pages/Menu.Master" AutoEventWireup="true" CodeBehind="realtime.aspx.cs" Inherits="NMS.pages.realtime" ViewStateMode="Enabled"%>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
   
     <link rel="stylesheet" href="../assets/styles/realtime.css"/>
     <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/boxicons/2.1.0/css/boxicons.min.css"/>
     <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css"/>
     <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/boxicons/2.1.0/css/boxicons.min.css"/>

   <div class="container">
   <div class="main-content">

         <asp:ScriptManager ID="ScriptManager1" runat="server" />
         <!-- Include additional CSS files if necessary -->
           <div class="top-container">
            <div class="top">
                <h1><asp:LinkButton ID="refresh" runat="server" Font-Underline="False" ForeColor="Black" OnClick="refresh_Click">Real-Time</asp:LinkButton></h1> 
            </div>
            
            <div class="top-buttons">
               <div class="mydict">
                    <div>
                        <label>
                            <asp:RadioButton ID="devicesbtn" runat="server" AutoPostBack="True" Checked="True" CssClass="radio-button" Enabled="False" />
                            <span style="background-color: #B4890A; color: #FFF; border-color: #B4890A;">Overview</span>
                        </label>
                        <label>
                            <asp:RadioButton ID="locationsbtn" runat="server" OnCheckedChanged="locationsbtn_CheckedChanged" AutoPostBack="True" CssClass="radio-button" />
                            <span>Monitor</span>
                        </label>
                    </div>
                </div>
            </div>

        </div>

        <div class="main-container">
            <div class="align-container">
                <div class="uno-row">
                    <div class="select-device">
                        <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="true" Width="170px" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
                            <asp:ListItem>Show All</asp:ListItem>
                            <asp:ListItem Value="Digital Device">Digital Device</asp:ListItem>
                            <asp:ListItem Value="IP Address">IP Address</asp:ListItem>
                            <asp:ListItem Value="Web Address">Web Address</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="select-status">
                   
                        <asp:DropDownList ID="DropDownList2" runat="server" AutoPostBack="true" Width="160px" OnSelectedIndexChanged="DropDownList2_SelectedIndexChanged">
                            <asp:ListItem>Select Status</asp:ListItem>
                            <asp:ListItem Value="healthy">Healthy</asp:ListItem>
                            <asp:ListItem Value="down">Down</asp:ListItem>
                            <asp:ListItem Value="problematic">Problematic</asp:ListItem>
                        </asp:DropDownList>
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

                <!-- INTERVAL -->
                <div class="dos-row">
                    <div class="entries-dropdown">
                        &nbsp;
                        <asp:TextBox ID="tbInterval" runat="server" placeholder="HH:MM:SS" AutoPostBack="true" OnTextChanged="tbInterval_TextChanged"></asp:TextBox>
                        &nbsp;&nbsp;
                        <asp:Button runat="server" ID="btnCalculate" Text="Apply" CssClass="btn-calculate" OnClick="btnCalculate_Click" />
                        <asp:HiddenField ID="hfInterval" runat="server"/>                                  
                        <div><asp:Label ID="errorLabel" runat="server" ForeColor="Red"></asp:Label></div>
                        <div class="right-align">
                            <asp:Label ID="IntervalLabel" runat="server" Text="Interval" CssClass="label" AssociatedControlID="ToggleButton"></asp:Label>                    
                            <label class="switch">
                                <asp:CheckBox ID="ToggleButton" runat="server" AutoPostBack="true" OnCheckedChanged="ToggleButton_CheckedChanged" />
                                <span class="slider round"></span>
                            </label>
                        </div>
                    </div>
                </div>
            </div>

            <!-- GRIDVIEW -->
            <div class="grid-container">
                 <asp:GridView ID="GridView1" runat="server" CssClass="gridview" Style="width: 100%" AutoGenerateColumns="False" PageSize="15" 
                     OnRowDataBound="GridView1_RowDataBound" OnPageIndexChanging="GridView1_PageIndexChanging" AllowPaging="true">
                <Columns>                
                    <asp:TemplateField HeaderText="Address">
                        <ItemTemplate>
                            <asp:Literal ID="PingStatusIndicator" runat="server"></asp:Literal>
                            <asp:Label ID="AddressLabel" runat="server" Text='<%# Eval("Address") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Address" HeaderText="Address" Visible="false" />
                    <asp:BoundField DataField="Name" HeaderText="Description" />
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="statusLabel" Text='<%# Eval("Status") %>' CssClass='<%# GetStatusClass(Eval("Status")) %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle BackColor="Transparent" />
                    </asp:TemplateField>
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
            </div>

            <div class="grid-container">
                    <asp:GridView ID="GridViewDevice" runat="server" CssClass="gridview" Style="width: 100%" AutoGenerateColumns="False" PageSize="15" 
                        OnRowDataBound="GridView1_RowDataBound" AllowPaging="true" OnPageIndexChanging="GridView1_PageIndexChanging">
                    
                    <Columns>                    
                        <asp:TemplateField HeaderText="Address">
                            <ItemTemplate>
                                <asp:Literal ID="PingStatusIndicator" runat="server"></asp:Literal>
                                <asp:Label ID="AddressLabel" runat="server" Text='<%# Eval("Address") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Name" HeaderText="Description" />
                         <asp:TemplateField HeaderText="Status">
                             <ItemTemplate>
                                 <asp:Label runat="server" ID="statusLabel" Text='<%# Eval("Status") %>' CssClass='<%# GetStatusClass(Eval("Status")) %>'></asp:Label>
                             </ItemTemplate>
                             <ItemStyle BackColor="Transparent" />
                         </asp:TemplateField>
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
            refreshPage();
        };


        document.addEventListener('DOMContentLoaded', function () {
            const toggleButton = document.getElementById('<%= ToggleButton.ClientID %>');
            const intervalLabel = document.getElementById('<%= IntervalLabel.ClientID %>');
            const globalInterval = document.getElementById('<%= hfInterval.ClientID %>').value;

            toggleButton.addEventListener('change', function () {
                if (toggleButton.checked) {
                    intervalLabel.textContent = `Global ${globalInterval}-Seconds Interval`;
                } else {
                    intervalLabel.textContent = "Priority-Based Intervals";
                }
            });
        });


        function showToast(message) {
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
            text2.textContent = message;

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

        function showErrorLabel(message) {
            var errorLabel = document.getElementById('<%= errorLabel.ClientID %>');
             if (errorLabel) {
                 errorLabel.innerText = message;
                 errorLabel.style.display = 'block';
                 setTimeout(hideErrorLabel, 5000); // Hide after 5 seconds
             }
        }

        function hideErrorLabel() {
            var errorLabel = document.getElementById('<%= errorLabel.ClientID %>');
            if (errorLabel) {
                errorLabel.style.display = 'none';
            }
        }

        function clearErrorLabel() {
            var errorLabel = document.getElementById('<%= errorLabel.ClientID %>');
            if (errorLabel) {
                errorLabel.innerText = '';
                errorLabel.style.display = 'none';
            }
        }

    </script>


</asp:Content>
