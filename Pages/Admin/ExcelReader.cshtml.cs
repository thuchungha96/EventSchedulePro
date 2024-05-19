using EventSchedulePro.Data.Class;
using EventSchedulePro.Data.Context;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System.Globalization;
using System.Net.WebSockets;

namespace EventSchedulePro.Pages.Admin
{
    public class ExcelReaderModel : PageModel
    {
        private string FileNameExample = "excel";
        [BindProperty]
        public List<List<object>> OutputDat { get; set; }

        public readonly EventDBContext _context;
        public ExcelReaderModel(EventDBContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            var userName = HttpContext.Session.GetString("AdminUserName");
            if (!string.IsNullOrEmpty(userName))
            {
                return new RedirectToPageResult("/Index");
            }
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            var uploadFol = $"{Directory.GetCurrentDirectory()}\\wwwroot\\Uploads";
            var filepat = Path.Combine(uploadFol, FileNameExample);
            if (System.IO.File.Exists(filepat))
            {
                using (var stream = System.IO.File.Open(filepat, FileMode.Open, FileAccess.Read))
                {
                    var exceldata = new List<List<object>>();
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        do
                        {
                            while (reader.Read())
                            {
                                var rowData = new List<object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                    rowData.Add(reader.GetValue(i));
                                exceldata.Add(rowData);
                            }

                        } while (reader.NextResult());
                    }
                    ViewData["ExcelData"] = exceldata;
                }
            }
            return Page();
        }

        [HttpPost("UpdateFile")]
        public async Task<IActionResult> OnPostUpdateFileAsync(IFormFile file)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    var uploadFol = $"{Directory.GetCurrentDirectory()}\\wwwroot\\Uploads";

                    if (!Directory.Exists(uploadFol))
                    {
                        Directory.CreateDirectory(uploadFol);
                    }
                    var filepat = Path.Combine(uploadFol, FileNameExample);

                    if (System.IO.File.Exists(filepat))
                    {
                        System.IO.File.Delete(filepat);
                    }

                    using (var stream = new FileStream(filepat, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    using (var stream = System.IO.File.Open(filepat, FileMode.Open, FileAccess.Read))
                    {
                        var exceldata = new List<List<object>>();
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            do
                            {
                                while (reader.Read())
                                {
                                    var rowData = new List<object>();
                                    for (int i = 0; i < reader.FieldCount; i++)
                                        rowData.Add(reader.GetValue(i));
                                    exceldata.Add(rowData);
                                }

                            } while (reader.NextResult());
                        }
                        ViewData["ExcelData"] = exceldata;
                        saveFileIntoDatabase(exceldata);
                    }

                }
            }
            catch (Exception ex)
            {
                ViewData["Exceptiona"] = ex.ToString();
            }
            return Page();
        }
        private async void saveFileIntoDatabase(List<List<object>> o)
        {
            for (int i = 1; i < o.Count; i++)
            {
                //Time = o[i][0];
                //Activity = o[i][1];
                //Notes = o[i][2];
                //Student Leader = o[i][3];
                //Group = o[i][4];
                //Staff = o[i][5];
                //Location = o[i][6];
                //Check Group not exited then insert
                string groupFromExcel = o[i][4]?.ToString();
                if (string.IsNullOrEmpty(groupFromExcel)) groupFromExcel = "All";
                {
                    {
                        if (!string.IsNullOrEmpty(groupFromExcel))
                        {
                            var groupList = groupFromExcel.Trim().Split(",");
                            foreach (var item in groupList)
                            {
                                var group = _context.Groups.FirstOrDefault(x => x.Name == item.Trim());
                                if (group == null)
                                {
                                    Group newItem = new Group { Name = item.Trim(), Detail = item.Trim() };
                                    _context.Groups.Add(newItem);
                                    _context.SaveChanges();
                                }
                            }
                        }
                    }
                    //Check Staff not exited then insert
                    {
                        if (!string.IsNullOrEmpty(o[i][5]?.ToString()))
                        {
                            var StaffList = o[i][5].ToString().Trim().Split(",");
                            foreach (var item in StaffList)
                            {
                                var staff = _context.Staffs.FirstOrDefault(x => x.UserName == item.Trim());
                                if (staff == null)
                                {
                                    Staff newStaff = new Staff { UserName = item.Trim(), GroupID = 0, RoleUser = "1", PasswordHash = Base64Encode("1") };
                                    _context.Add(newStaff);
                                    _context.SaveChanges();
                                }
                            }
                        }
                    }
                    //Insert Staff into new Group
                    {
                        if (!string.IsNullOrEmpty(groupFromExcel) && !string.IsNullOrEmpty(o[i][5]?.ToString()))
                        {
                            var groupList = groupFromExcel.Trim().Split(",");
                            var StaffList = o[i][5].ToString().Trim().Split(",");
                            var g = _context.Groups.ToList();
                            var s = _context.Staffs.ToList();
                            string GroupIds = "";
                            string GroupNames = "";
                            string StaffIds = "";
                            string StaffNames = "";
                            foreach (var item in groupList)
                            {
                                var group = g.FirstOrDefault(x => x.Name == item.Trim());
                                if (group != null)
                                {
                                    GroupIds += group.Id + ",";
                                    GroupNames += group.Name + ",";
                                }
                            }

                            foreach (var item in StaffList)
                            {
                                var staff = s.FirstOrDefault(x => x.UserName == item.Trim());
                                if (staff != null)
                                {
                                    StaffIds += staff.Id + ",";
                                    StaffNames += staff.UserName + ",";
                                }
                            }

                            if (!string.IsNullOrEmpty(GroupIds) && !string.IsNullOrEmpty(GroupNames))
                                foreach (var item in StaffList)
                                {
                                    var staff = _context.Staffs.FirstOrDefault(x => x.UserName == item.Trim());
                                    if (staff != null)
                                    {
                                        staff.GroupIds = GroupIds;
                                        staff.GroupNames = GroupNames;
                                        _context.SaveChanges();
                                    }
                                }
                            var time = o[i][0].ToString().Trim().Split("-");
                            string starttime = "";
                            string endtime = "";
                            for (int j = 0; j < time.Length; j++)
                            {
                                var timeRaw = time[j].Trim();
                                try
                                {
                                    DateTime timeParse = DateTime.ParseExact(timeRaw, "h:mmtt", CultureInfo.InvariantCulture);
                                    if (j == 0)
                                        starttime = timeParse.ToString("HH:mm");
                                    else endtime = timeParse.ToString("HH:mm");
                                }
                                catch (Exception) { }
                            }

                            foreach (var item in groupList)
                            {
                                var group = g.FirstOrDefault(x => x.Name == item.Trim());
                                if (group != null)
                                {
                                    Schedule t = new Schedule
                                    {
                                        Name = o[i][1]?.ToString(),
                                        Date = DateTime.Now.ToString("yyyy-MM-dd"),
                                        GroupID = group.Id.ToString(),
                                        Activity = o[i][1]?.ToString(),
                                        Staff = StaffIds,
                                        StudentLeader = o[i][3]?.ToString(),
                                        Note = o[i][2]?.ToString(),
                                        Location = o[i][6]?.ToString(),
                                        FromTime = starttime,
                                        ToTime = endtime,
                                        StaffNames = StaffNames
                                    };
                                    _context.Schedules.Add(t);
                                    _context.SaveChanges();
                                }
                            }

                        }
                    }
                    //Insert Schedule
                }
            }
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }

}
