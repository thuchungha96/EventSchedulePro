using EventSchedulePro.Data.Class;
using EventSchedulePro.Data.Context;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.X500;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.Intrinsics.Arm;

namespace EventSchedulePro.Pages.Admin
{
    public class ScheduleModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }


        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            /// 


            public string Id { get; set; }
            public string Group { get; set; }

            [Required]
            public string Name { get; set; }
            public string Activity { get; set; }
            public string Staff { get; set; }
            public string Leader { get; set; }
            public string Note { get; set; }
            public string Location { get; set; }
            public string TimeSchedule { get; set; }
            public string FromTime { get; set; }
            public string ToTime { get; set; }
            public List<String> Stafff { get; set; }
        }
        public readonly EventDBContext _context;
        public ScheduleModel(EventDBContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            var userName = HttpContext.Session.GetString("AdminUserName");
            if (string.IsNullOrEmpty(userName))
            {
                return new RedirectToPageResult("/Admin/Login");
            }
            return Page();
        }
        [HttpPost("Add")]
        public async Task<IActionResult> OnPostAddAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            try
            {
                if (Input.Group.CompareTo("Select group") != 0)
                    if (String.IsNullOrEmpty(Input.Id)) { }
                string stafidList = (Input.Stafff != null && Input.Stafff.Any()) ? string.Join(",", Input.Stafff) : "";
                var staffList = _context.Staffs.Where(x => Input.Stafff.Contains(x.Id.ToString())).ToList();

                string staffname = stafidList.Any() ? string.Join(", ", staffList.Select(x => x.UserName.ToString()).ToArray()) : ""; // !string.IsNullOrEmpty(stafidList) ? getStaffName(stafidList) : "";
                if (String.IsNullOrEmpty(Input.Id))
                {
                    Schedule t = new Schedule
                    {
                        Name = Input.Name,
                        Date = Input.TimeSchedule,// DateTime.Parse(Input.TimeSchedule),
                        GroupID = Input.Group,
                        Activity = Input.Activity,
                        Staff = stafidList,
                        StudentLeader = Input.Leader,
                        Note = Input.Note,
                        Location=Input.Location,
                        FromTime=Input.FromTime,
                        ToTime=Input.ToTime,
                        StaffNames=staffname
                    };
                    _context.Schedules.Add(t);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var schedule = _context.Schedules.FirstOrDefault(x => x.Id == int.Parse(Input.Id));
                    if (schedule != null)
                    {
                        CultureInfo enUS = new CultureInfo("en-US");
                        DateTime dateValue;
                        if (DateTime.TryParseExact(Input.TimeSchedule, "MM/dd/yyyy", enUS,
                                  DateTimeStyles.None, out dateValue))
                        {
                            schedule.Name = Input.Name;
                            schedule.Date = dateValue.ToString("MM/dd/yyyy");
                            schedule.GroupID = Input.Group;
                            schedule.Activity = Input.Activity;
                            schedule.Staff = stafidList;
                            schedule.StudentLeader = Input.Leader;
                            schedule.Note = Input.Note;
                            schedule.Location = Input.Location;
                            schedule.FromTime = Input.FromTime;
                            schedule.ToTime= Input.ToTime;
                            schedule.StaffNames = staffname;
                            await _context.SaveChangesAsync();

                        }
                    }
                }
                /*
                using (MySqlConnection conn = new MySqlConnection("server=104.238.129.230;uid=jinzschedule;pwd=Metmoivl@123.@124!;database=jinzschedule;connect Timeout=30"))
                {
                    conn.Open();
                    string myInsertQuery = "";
                    if (String.IsNullOrEmpty(Input.Id))
                    {
                        myInsertQuery = $"insert into schedule(name,date,groupid,activity,staff,studentLeader,Note,Location,fromtime,totime,StaffNames) values ('{Input.Name}','{DateTime.Parse(Input.TimeSchedule).ToString("yyyy-MM-dd")}',{Input.Group},N'{Input.Activity}',N'{stafidList}',N'{Input.Leader}',N'{Input.Note}',N'{Input.Location}',N'{Input.FromTime}',N'{Input.ToTime}',N'{staffname}')";
                    }
                    else
                    {
                        CultureInfo enUS = new CultureInfo("en-US");
                        DateTime dateValue;
                        if (DateTime.TryParseExact(Input.TimeSchedule, "dd/MM/yyyy", enUS,
                                  DateTimeStyles.None, out dateValue))
                        {
                            myInsertQuery = $"update schedule set name=N'{Input.Name}', date='{dateValue.ToString("yyyy-MM-dd")}', groupid={Input.Group},activity = N'{Input.Activity}', staff =N'{stafidList}', studentLeader=N'{Input.Leader}', Note= N'{Input.Note}', " +
                                $"Location= N'{Input.Location}',fromtime =N'{Input.FromTime}',totime=N'{Input.ToTime}',StaffNames=N'{staffname}' where id = {Input.Id}   ";
                        }
                    }
                    MySqlCommand myCommand = new MySqlCommand(myInsertQuery, conn);
                    myCommand.ExecuteNonQuery();
                } */
            }
            catch (Exception ex)
            {

            }
            return Page();
        }
        [HttpPost("Delete")]
        public async Task<IActionResult> OnPostDeleteAsync(string Id = null)
        {
            if (Id != null)
            {
                try
                {
                    var schedule = _context.Schedules.FirstOrDefault(x => x.Id == int.Parse(Id));
                    if (schedule != null)
                    {
                        _context.Remove(schedule);
                        await _context.SaveChangesAsync();
                    }

                }
                catch (Exception ex) { }
            }
            return Page();
        }
        [HttpPost("Edit")]
        public async Task<IActionResult> OnPostEditAsync(string Id = null)
        {
            if (Id != null)
            {
                var EventSchedule = (from sc in _context.Schedules
                                      join grj in _context.Groups on sc.GroupID ?? "" equals grj.Id.ToString() into grs
                                      from gr in grs.DefaultIfEmpty()
                                      where sc.Id == int.Parse(Id)
                                      select new
                                      {
                                          id = sc.Id,
                                          name = sc.Name,
                                          activity = sc.Activity ?? "",
                                          studentLeader = sc.StudentLeader ?? "",
                                          note = sc.Note ?? "",
                                          date = sc.Date,
                                          location = sc.Location,
                                          groupName = gr != null ? "None" : gr.Name,
                                          groupId = sc.GroupID,
                                          fromTime = sc.FromTime ?? "",
                                          toTime = sc.ToTime ?? "",
                                          StaffNames = sc.StaffNames,
                                          staff = sc.Staff
                                      }).FirstOrDefault();
                if (EventSchedule != null)
                {
                    Input.Id = Id;
                    Input.Name = EventSchedule.name;
                    Input.Group = EventSchedule.groupName;
                    Input.Activity = EventSchedule.activity;
                    Input.Stafff = EventSchedule.staff != null ? new List<string>(EventSchedule.staff.ToString().Split(",")) : new List<string>();
                    Input.Leader = EventSchedule.studentLeader;
                    Input.Note = EventSchedule.note;
                    Input.Location = EventSchedule.location;
                    Input.TimeSchedule = EventSchedule.date;//?.ToString("dd/MM/yyyy");
                    Input.FromTime = EventSchedule.fromTime;
                    Input.ToTime = EventSchedule.toTime;
                }
            }
            return Page();
        }
        private string getStaffName(string listId)
        {
            string username = "";
            using (MySqlConnection conn = new MySqlConnection("server=104.238.129.230;uid=jinzschedule;pwd=Metmoivl@123.@124!;database=jinzschedule;connect Timeout=30"))
            {
                conn.Open();
                string sql = $"SELECT Username FROM user where id in ({listId})";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        username+= @rdr.GetString(0) +", ";

                    }
                }
            }
            return username;
        }
    }
}
