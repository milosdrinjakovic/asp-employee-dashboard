using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.SkiaSharp;
using employee_dashboard.Models;  

namespace employee_dashboard.Controllers
{
    public class HomeController : Controller
    {
        private static readonly string ApiUrl = "https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries?code=vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==";

        public async Task<IActionResult> Index()
        {
            var entries = await FetchTimeEntriesAsync();
            
            // Prikaz tabele
            var totalHoursByEmployee = entries
                .GroupBy(e => e.EmployeeName)
                .Select(g => new
                {
                    EmployeeName = g.Key,
                    TotalHours = g.Sum(e => (e.EndTimeUtc - e.StarTimeUtc).TotalHours)
                })
                .OrderByDescending(e => e.TotalHours)
                .ToList();

            var plotModel = new PlotModel { Title = "Time Worked by Employee" };
            var pieSeries = new PieSeries();
            var totalHours = totalHoursByEmployee.Sum(e => e.TotalHours);

            foreach (var entry in totalHoursByEmployee)
            {
                pieSeries.Slices.Add(new PieSlice(entry.EmployeeName, entry.TotalHours));
            }

            plotModel.Series.Add(pieSeries);

            var filePath = "wwwroot/pieChart.png";
            using (var stream = System.IO.File.Create(filePath))
            {
                var exporter = new PngExporter { Width = 600, Height = 400 };
                exporter.Export(plotModel, stream);
            }

            var viewModel = new DashboardViewModel
            {
                Employees = totalHoursByEmployee,
                PieChartPath = "/pieChart.png"
            };

            return View(viewModel);
        }

        private async Task<List<TimeEntry>> FetchTimeEntriesAsync()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync(ApiUrl);
                return JsonConvert.DeserializeObject<List<TimeEntry>>(response);
            }
        }
    }
}
