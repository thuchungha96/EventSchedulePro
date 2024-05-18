using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EventSchedulePro.Pages
{
    public class AllScheduleModel : PageModel
    {
        private string FileNameExample = "excel";
        public void OnGet()
        {
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
                                var rowData = new List<Object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                    rowData.Add(reader.GetValue(i));
                                exceldata.Add(rowData);
                            }

                        } while (reader.NextResult());
                    }
                    ViewData["ExcelData"] = exceldata;
                }
            }
        }
    }
}
