<%@ Page Title="" Language="C#" MasterPageFile="~/pages/Menu.Master" AutoEventWireup="true" CodeBehind="dashboard-admin.aspx.cs" Inherits="NMS.pages.dashboard_admin" Async="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <link rel="stylesheet" href="../assets/styles/dashboard-admin.css"/>

    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/boxicons/2.1.0/css/boxicons.min.css"/>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css"/>
    <!--BOOTSTERP -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.2.1/jquery.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.9.3/Chart.min.js"></script>
   
   
    <script src="https://cdn.jsdelivr.net/npm/apexcharts"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.2.1/jquery.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.9.3/Chart.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/chartjs-plugin-annotation/0.5.7/chartjs-plugin-annotation.min.js"></script>  

     <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <div class="container">
     <div class="main-content">
         <div class="titles">
                    <h1><asp:LinkButton ID="refresh" runat="server" Font-Underline="False" ForeColor="Black" OnClick="refresh_Click">Dashboard</asp:LinkButton></h1>
                    <p>Welcome to Network Monitoring System</p>
         </div>
         <asp:HiddenField ID="hfInterval" runat="server" value="30"/>
        <!-- Toast -->
        <div class="toast">
            <div class="toast-content">
                <i class="fas fa-solid fa-exclamation-circle check"></i>
                &nbsp;
                <div class="message">
                    <span class="text text-1">Warning</span>
                    <span class="text text-2">Some IP addresses are having connection problems</span>
                </div>
            </div>
            <i class="fas fa-solid fa-times close" onclick="hideToast()"></i>
            <div class="progress"></div>
        </div>

        <style>
            .toast .text-1, .toast .text-2 {
                white-space: pre-line; /* Ensure new lines are rendered correctly */
            }
        </style>

        <div class="content">
                <!-- CARDS -->
                    <div class="cards-container">
                        <div class="cards">
                            <div class="card">
                             
                                
                               <img src="../assets/img/healthy.png" class="card-icon" alt="Healthcare Icon"/>                           
                                 <asp:HyperLink ID="HyperLinkHealthy" runat="server" NavigateUrl="realtime.aspx?status=healthy" CssClass="card-link">
                                    <div class="card-value">
                                        <asp:FormView ID="FormViewHealthy" runat="server" OnPageIndexChanging="FormViewHealthy_PageIndexChanging">
                                            <ItemTemplate>
                                                <%# Eval("StatusCount") %>
                                            </ItemTemplate>
                                        </asp:FormView>
                                
                                    </div>
                                    <div class="card-status">
                                        <asp:FormView ID="FormViewHealthyStatus" runat="server">
                                            <ItemTemplate>
                                                <span style="color: #464255;"><%# Eval("PingStatus") %></span>
                                            </ItemTemplate>
                                        </asp:FormView>
                                    </div>
                                </asp:HyperLink>       
                               </div>                             
                          
                 <!-- -- -->
                            <div class="card">
                         
                                <asp:Image ID="Image1" runat="server" ImageUrl="~/assets/img/down.png" CssClass="card-icon" AlternateText="Down Icon" />
                                <asp:HyperLink ID="HyperLinkDown" runat="server" NavigateUrl="realtime.aspx?status=down" CssClass="card-link">
                                <div class="card-value">
                                    <asp:FormView ID="FormViewDown" runat="server" OnPageIndexChanging="FormViewDown_PageIndexChanging">
                                        <ItemTemplate>
                                            <%# Eval("StatusCount") %>
                                        </ItemTemplate>
                                    </asp:FormView>
                                </div>
                                <div class="card-status">
                                    <asp:FormView ID="FormViewDownStatus" runat="server">
                                        <ItemTemplate>
                                            <span style="color: #464255;"><%# Eval("PingStatus") %></span>
                                        </ItemTemplate>
                                    </asp:FormView>
                                </div>
                            </asp:HyperLink>
                               </div>                               
                            
                 <!-- -- -->
                            <div class="card">                             
                                  <asp:Image ID="imgProblematicIcon" runat="server" ImageUrl="~/assets/img/problemtic.png" CssClass="card-icon" AlternateText="Problematic Icon" />
                                    <asp:HyperLink ID="HyperLinkProblematic" runat="server" NavigateUrl="realtime.aspx?status=problematic" CssClass="card-link">
                                        <div class="card-value">
                                            <asp:FormView ID="FormViewProblematic" runat="server" OnPageIndexChanging="FormViewProblematic_PageIndexChanging">
                                                <ItemTemplate>
                                                    <%# Eval("StatusCount") %>
                                                </ItemTemplate>
                                            </asp:FormView>
                                        </div>
                                        <div class="card-status">
                                            <asp:FormView ID="FormViewProblematicStatus" runat="server">
                                                <ItemTemplate>
                                                    <span style="color: #464255;"><%# Eval("PingStatus") %></span>
                                                </ItemTemplate>
                                            </asp:FormView>
                                        </div>
                                    </asp:HyperLink> 
                                </div>                                                         
                        </div>
                    </div>
            
                <!-- LINE -->      
                   <div class="line-graph-container">
                    <div class="line-graph">
                       <div class="linecanvas">
                        <div class="line-header">
                            <div class="entries-time">
                                <h1>
                                    <asp:LinkButton ID="overviewbtn" runat="server" Font-Underline="False" ForeColor="Black" OnClick="OverviewBtn_Click">Overview</asp:LinkButton>
                                </h1>
                                <label id="startDateTimeLabel" for="startDateTime" runat="server">Start:</label>
                                <input type="datetime-local" id="startDateTimeInput" name="startDateTime" runat="server" required />&nbsp&nbsp&nbsp
                                <label id="endDateTimeLabel" for="endDateTime" runat="server">End:</label>
                                <input type="datetime-local" id="endDateTimeInput" name="endDateTime" runat="server" required />
                                <span id="startDateTimeDisplay"></span>
                                <asp:Button runat="server" ID="btnApply" Text="Apply" OnClick="btnApply_Click" CssClass="btn-calculate"/>
                            </div>
                            <div class="entries-dropdown">
                                <asp:DropDownList ID="FilterDropdown" runat="server" AutoPostBack="true" OnSelectedIndexChanged="FilterDropdown_SelectedIndexChanged">
                                    <asp:ListItem Text="1 Minute" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="5 Minutes" Value="5"></asp:ListItem>
                                    <asp:ListItem Text="10 Minutes" Value="10"></asp:ListItem>
                                    <asp:ListItem Text="30 Minutes" Value="30"></asp:ListItem>
                                    <asp:ListItem Text="1 Hour" Value="Hour"></asp:ListItem>
                                    <asp:ListItem Text="Today" Value="Day"></asp:ListItem>
                                    <asp:ListItem Text="Week" Value="Week"></asp:ListItem>
                                    <asp:ListItem Text="Month" Value="Month"></asp:ListItem>
                                    <asp:ListItem Text="Year" Value="Year"></asp:ListItem>
                                    <asp:ListItem Text="Custom" Value="Custom"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="line-graph-canvas">
                            <canvas id="lineChartCanvas"></canvas>
                            <input type="hidden" id="lineChartData" runat="server" />
                        </div>
                    </div>

                        <asp:HiddenField ID="healthyChartData" runat="server" />
                        <asp:HiddenField ID="downChartData" runat="server" />
                        <asp:HiddenField ID="probChartData" runat="server" />
                    </div>
                </div>

                <!-- DONUT -->
                    <div class="donut-chart-container">
                        <div class="donut-chart">
                            <div class="donutcanvas">
                             <div class="header-content">
                             <h6>Devices</h6>
                            <div class="chart-legend">
                                <a href="realtime.aspx?tablename=Digital Device&status=healthy" class="legend-item">
                                    <span class="legend-color healthy"></span> Healthy
                                </a>
                                <a href="realtime.aspx?tablename=Digital Device&status=down" class="legend-item">
                                    <span class="legend-color down"></span> Down
                                </a>
                                <a href="realtime.aspx?tablename=Digital Device&status=problematic" class="legend-item">
                                    <span class="legend-color problematic"></span> Problematic
                                </a>
                            </div>
                             </div>
                                <div class="donut-canvas-container">
                                <canvas id="deviceChart"></canvas>
                                </div>

                            </div>                     
                        </div>
                <!-- -- -->
                        <div class="donut-chart">
                            <div class="donutcanvas">
                            <div class="header-content">
                             <h6>IPs</h6>
                                  <div class="chart-legend">
                                 <a href="realtime.aspx?tablename=IP Address&status=healthy" class="legend-item">
                                     <span class="legend-color healthy"></span> Healthy
                                 </a>
                                 <a href="realtime.aspx?tablename=IP Address&status=down" class="legend-item">
                                     <span class="legend-color down"></span> Down
                                 </a>
                                 <a href="realtime.aspx?tablename=IP Address&status=problematic" class="legend-item">
                                     <span class="legend-color problematic"></span> Problematic
                                 </a>
                             </div>
                            </div>
                                 <div class="donut-canvas-container">
                                    <canvas id="ipChart"></canvas>
                                 </div>
                            </div>    
                        </div>
                <!-- -- -->
                        <div class="donut-chart">
                            <div class="donutcanvas">
                            <div class="header-content">
                            <h6>Websites</h6>
                                 <div class="chart-legend">
                                     <a href="realtime.aspx?tablename=Web Address&status=healthy" class="legend-item">
                                         <span class="legend-color healthy"></span> Healthy
                                     </a>
                                     <a href="realtime.aspx?tablename=Web Address&status=down" class="legend-item">
                                         <span class="legend-color down"></span> Down
                                     </a>
                                     <a href="realtime.aspx?tablename=Web Address&status=problematic" class="legend-item">
                                         <span class="legend-color problematic"></span> Problematic
                                     </a>
                                 </div>
                            </div>
                            <div class="donut-canvas-container">
                                <canvas id="webChart"></canvas>
                            </div>
                            </div>    
                        </div>
                    </div>
                      <!-- Hidden fields to hold chart data -->
                    <asp:HiddenField ID="deviceChartData" runat="server" />
                    <asp:HiddenField ID="ipChartData" runat="server" />
                    <asp:HiddenField ID="webChartData" runat="server" />
            </div>
      </div>
</div>
   
     <script type="text/javascript">
         function showToast(text1, text2) {
             const toast = document.querySelector(".toast");
             const progress = document.querySelector(".progress");
             const text1Elem = document.querySelector(".toast .text-1");
             const text2Elem = document.querySelector(".toast .text-2");

             text1Elem.innerHTML = text1.replace(/\n/g, '<br>'); // Use innerHTML to handle new lines
             text2Elem.innerHTML = text2.replace(/\n/g, '<br>'); // Use innerHTML to handle new lines

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


         function getRefreshInterval() {
             var selectedValue = document.getElementById('<%= FilterDropdown.ClientID %>').value;

        if (selectedValue === "1") {
            return 25 * 1000; // 25 seconds
        } else if (selectedValue === "5") {
            return 120 * 1000; // 2 minutes
        } else if (selectedValue === "10") {
            return 300 * 1000; // 5 minutes
        } else {
            return 300 * 1000; // Default interval for all other values (5 minutes)
        }
    }

    function refreshPage() {
        location.reload();
    }

    window.onload = function () {
        var interval = getRefreshInterval();
        setInterval(refreshPage, interval);

        // Update the interval whenever the dropdown value changes
        document.getElementById('<%= FilterDropdown.ClientID %>').addEventListener('change', function () {
            clearInterval(refreshInterval);
            interval = getRefreshInterval();
            setInterval(refreshPage, interval);
        });
    };
     </script>

     <script>
         $(document).ready(function () {
             function renderChart(chartId, data) {
                 var ctx = document.getElementById(chartId).getContext('2d');
                 var chartData = {
                     labels: [],
                     datasets: [{
                         data: [],
                         backgroundColor: []
                     }]
                 };

                 data.forEach(function (item) {
                     chartData.labels.push(item.PingStatus);
                     chartData.datasets[0].data.push(item.StatusCount);
                     // Adjust colors based on PingStatus
                     if (item.PingStatus === 'Healthy') {
                         chartData.datasets[0].backgroundColor.push("#0AD17C");
                     } else if (item.PingStatus === 'Down') {
                         chartData.datasets[0].backgroundColor.push("#c0392b");
                     } else if (item.PingStatus === 'Problematic') {
                         chartData.datasets[0].backgroundColor.push("#E6A23C");
                     }
                 });

                 new Chart(ctx, {
                     type: 'doughnut',
                     data: chartData,
                     options: {
                         legend: {
                             display: false
                         },
                         title: {
                             display: false
                         },
                         tooltips: {
                             bodyFontFamily: 'Poppins',
                             bodyFontStyle: 'normal',
                             bodyFontColor: '#FFF',
                             bodyFontWeight: 'normal',
                         }
                     }
                 });
             }

             var deviceData = JSON.parse($('#<%=deviceChartData.ClientID%>').val());
            var ipData = JSON.parse($('#<%=ipChartData.ClientID%>').val());
        var webData = JSON.parse($('#<%=webChartData.ClientID%>').val());

            renderChart('deviceChart', deviceData);
            renderChart('ipChart', ipData);
            renderChart('webChart', webData);
        });
     </script>

<script type="text/javascript">
    // Retrieve combined hourly data from server-side code
    var combinedHourlyData = <%= CombinedHourlyData %>;

    function formatTimePeriod(timePeriod, format) {
        var date = new Date(timePeriod);
        var hours = date.getHours();
        var minutes = date.getMinutes();
        var seconds = date.getSeconds();
        var ampm = hours < 12 ? 'AM' : 'PM';

        // Convert 24-hour time to 12-hour time
        hours = hours % 12;
        hours = hours ? hours : 12; // the hour '0' should be '12'
        minutes = minutes < 10 ? '0' + minutes : minutes;
        seconds = seconds < 10 ? '0' + seconds : seconds;

        if (format === "1" || format === "5" || format === "10" || format === "30") {
            return hours + ':' + minutes + ':' + seconds + ' ' + ampm;
        } else if (format === "Hour") {
            return hours + ':' + minutes + ':' + seconds + ' ' + ampm;
        } else if (format === "Day") {
            // Include minutes for the Day format
            return hours + ':' + minutes + ' ' + ampm;
        } else if (format === "Week") {
            return timePeriod; // Assuming timePeriod is already in a readable format (e.g., day names)
        } else if (format === "Month") {
            // Convert numeric month to month name
            var monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun",
                "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
            return monthNames[timePeriod - 1]; // Convert numeric month to abbreviated month name
        } else if (format === "Year") {
            return timePeriod; // Year should already be in the desired format
        } else {
            return timePeriod; // For other formats, return as-is
        }
    }

    // Extract labels and data from the combined data
    function getChartData() {
        var selectedValue = document.getElementById('<%= FilterDropdown.ClientID %>').value;
        var labels = [];
        var healthyData = [];
        var downData = [];
        var problematicData = [];

        if (selectedValue === "1") {
            var now = new Date();
            var currentMinute = now.getMinutes();
            var currentHour = now.getHours();

            for (var i = 0; i < 60; i++) {
                var currentSecond = new Date(now.getFullYear(), now.getMonth(), now.getDate(), currentHour, currentMinute, i);
                var formattedTime = formatTimePeriod(currentSecond, "1");
                labels.push(formattedTime);

                var dataPoint = combinedHourlyData.find(item => {
                    var itemDate = new Date(item.TimePeriod);
                    return itemDate.getHours() === currentHour &&
                        itemDate.getMinutes() === currentMinute &&
                        itemDate.getSeconds() === i;
                });

                if (dataPoint) {
                    healthyData.push(dataPoint.Healthy);
                    downData.push(dataPoint.Down);
                    problematicData.push(dataPoint.Problematic);
                } else {
                    healthyData.push(0);
                    downData.push(0);
                    problematicData.push(0);
                }
            }
        } else if (selectedValue === "5" || selectedValue === "10" || selectedValue === "30") {
            var hours = combinedHourlyData.map(function (item) {
                return formatTimePeriod(item.TimePeriod, selectedValue);
            });
            labels = hours;
            healthyData = combinedHourlyData.map(item => item.Healthy);
            downData = combinedHourlyData.map(item => item.Down);
            problematicData = combinedHourlyData.map(item => item.Problematic);
        } else {
            var hours = combinedHourlyData.map(function (item) {
                return formatTimePeriod(item.TimePeriod, selectedValue);
            });
            labels = hours;
            healthyData = combinedHourlyData.map(item => item.Healthy);
            downData = combinedHourlyData.map(item => item.Down);
            problematicData = combinedHourlyData.map(item => item.Problematic);
        }

        return {
            labels: labels,
            healthyData: healthyData,
            downData: downData,
            problematicData: problematicData
        };
    }


    function updateChart() {
        var data = getChartData();
        console.log(data); // Log data to ensure it's correct

        myChart.data.labels = data.labels;
        myChart.data.datasets[0].data = data.healthyData;
        myChart.data.datasets[1].data = data.downData;
        myChart.data.datasets[2].data = data.problematicData;
        myChart.update();
    }

    $(document).ready(function () {
        updateChart();
    });



    // Extract labels (hours) and data (count of each status) from the combined data
    var selectedValue = '<%= FilterDropdown.SelectedValue %>';
    var hours = combinedHourlyData.map(function (item) {
        return formatTimePeriod(item.TimePeriod, selectedValue);
    });

    var healthyData = combinedHourlyData.map(function (item) {
        return item.Healthy;
    });
    var downData = combinedHourlyData.map(function (item) {
        return item.Down;
    });
    var problematicData = combinedHourlyData.map(function (item) {
        return item.Problematic;
    });


    // Render the line chart
    var ctx = document.getElementById('lineChartCanvas').getContext('2d');
    var myChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: hours,
            datasets: [
                {
                    label: 'Healthy',
                    data: healthyData,
                    backgroundColor: 'rgba(3.92%, 81.96%, 48.63%, 0.2)',
                    borderColor: '#0AD17C',
                    borderWidth: 2,
                    tension: 0.4,
                    fill: true
                },
                {
                    label: 'Down',
                    data: downData,
                    backgroundColor: 'rgba(75.29%, 22.35%, 16.86%, 0.2)',
                    borderColor: '#c0392b',
                    borderWidth: 2,
                    tension: 0.4,
                    fill: true
                },
                {
                    label: 'Problematic',
                    data: problematicData,
                    backgroundColor: 'rgba(230, 162, 60, .2)',
                    borderColor: 'rgba(230, 162, 60, .8)',
                    borderWidth: 2,
                    tension: 0.4,
                    fill: true
                }
            ]
        },
        options: {
            scales: {
                yAxes: [{
                    ticks: {
                        beginAtZero: true,
                        fontFamily: 'Arial', // Custom font
                    },
                    gridLines: {
                        borderDash: [5, 5], // Dashed line for grid
                    }
                }],
                xAxes: [{
                    ticks: {
                        fontFamily: 'Arial', // Custom font
                    },
                    gridLines: {
                        display: false // Remove vertical grid lines
                    }
                }]
            },
            title: {
                display: false,
            },
            legend: {
                labels: {
                    fontFamily: 'Arial', // Custom font
                    usePointStyle: true, // Make legend circles
                    padding: 20 // Margin at the bottom
                },
                position: 'top',
                align: 'end' // Align legend to the top right
            },
            layout: {
                padding: {
                    top: 10, // Adjust this value to move the legend higher
                    bottom: 30 // Adjust this value for the desired bottom margin
                }
            }
        }
    });

    

</script>
</asp:Content>
