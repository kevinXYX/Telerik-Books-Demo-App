using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Linq;
using KendoNET.DynamicLinq;
using System.Linq.Dynamic.Core;

namespace TelerikDemoApp.Controllers
{
    public class BooksController : Controller
    {
        private static string connectionString = "Data Source=34.124.150.194,1433;Initial Catalog=Webcam;User ID=kevin;Password=upwork";

        [HttpPost]
        public DataSourceResult GetBooks([FromBody] DataSourceRequest requestModel)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                var sql = $"exec TestSP";
                var result = conn.Query<Book>(sql).AsQueryable();

                var filteredResults = FilterResults(requestModel.Filter, result);

                requestModel.Filter = null; //set to null after to apply custom filtering

                return filteredResults.AsQueryable().ToDataSourceResult(requestModel);
            }
        }

        private List<Book> FilterResults(KendoNET.DynamicLinq.Filter filterObj, IQueryable<Book> result)
        {
            var filteredResults = result.ToList();

            if (filterObj != null)
            {
                filteredResults = new List<Book>();

                foreach (var filter in filterObj.Filters)
                {
                    switch (filter.Field)
                    {
                        case "note":
                            var noteQuery = result.Where(x => filter.Operator == "eq" ? x.Note.ToLower() == filter.Value.ToString().ToLower() : x.Note.ToLower().Contains(filter.Value.ToString().ToLower())).ToList();
                            noteQuery.ForEach(x => filteredResults.Add(x));
                            break;
                        case "userId":
                            var userIdQuery = result.Where(x => filter.Operator == "eq" ? x.UserId == int.Parse(filter.Value.ToString()) : x.UserId.ToString().ToLower().Contains(filter.Value.ToString().ToLower())).ToList();
                            userIdQuery.ForEach(x => filteredResults.Add(x));
                            break;
                        case "isbn":
                            var isbnQuery = result.Where(x => filter.Operator == "eq" ? x.Isbn.ToLower() == filter.Value.ToString().ToLower() : x.Isbn.ToLower().Contains(filter.Value.ToString().ToLower())).ToList();
                            isbnQuery.ForEach(x => filteredResults.Add(x));
                            break;
                        case "fileName":
                            var fileNameQuery = result.Where(x => filter.Operator == "eq" ? x.Isbn.ToLower() == filter.Value.ToString().ToLower() : x.Isbn.ToLower().Contains(filter.Value.ToString().ToLower())).ToList();
                            fileNameQuery.ForEach(x => filteredResults.Add(x));
                            break;
                        case "fileSize":
                            var fileSizeQuery = result.Where(x => filter.Operator == "eq" ? x.FileSize == int.Parse(filter.Value.ToString()) : x.FileSize.ToString().ToLower().Contains(filter.Value.ToString().ToLower())).ToList();
                            fileSizeQuery.ForEach(x => filteredResults.Add(x));
                            break;
                        case "videoDuration":
                            var videoDurationQuery = result.Where(x => filter.Operator == "eq" ? x.VideoDuration == int.Parse(filter.Value.ToString()) : x.VideoDuration.ToString().ToLower().Contains(filter.Value.ToString().ToLower())).ToList();
                            videoDurationQuery.ForEach(x => filteredResults.Add(x));
                            break;
                        case "uploadDate":
                            var uploadDateQuery = result.Where(x => !string.IsNullOrEmpty(x.UploadDate) ? DateTime.Parse(x.UploadDate).ToString("MM-dd-yyyy").Contains(filter.Value.ToString()) : false).ToList();
                            uploadDateQuery.ForEach(x => filteredResults.Add(x));
                            break;
                        case "created":
                            var createDateQuery = result.Where(x => !string.IsNullOrEmpty(x.Created) ? DateTime.Parse(x.Created).ToString("MM-dd-yyyy").Contains(filter.Value.ToString()) : false).ToList();
                            createDateQuery.ForEach(x => filteredResults.Add(x));
                            break;
                    }
                }
            }

            filteredResults.ForEach(x =>
            {
                x.UploadDate = !string.IsNullOrEmpty(x.UploadDate) ? DateTime.Parse(x.UploadDate).ToString("MM-dd-yyyy") : string.Empty;
                x.Created = !string.IsNullOrEmpty(x.Created) ? DateTime.Parse(x.Created).ToString("MM-dd-yyyy") : string.Empty;
            });

            return filteredResults;
        }
    }

    public class Book
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TypeId { get; set; }
        public string Created { get; set; }
        public string Isbn { get; set; }
        public string Note { get; set; }
        public string FileName { get; set; }    
        public int FileSize { get; set; }
        public int VideoDuration { get; set; }
        public string UploadDate { get; set; }
    }
}
