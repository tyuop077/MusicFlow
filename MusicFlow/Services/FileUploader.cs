using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MusicFlow.Services
{
    public class FileUploader
    {
        private static readonly HttpClient http = new HttpClient();
        private string id;
        //private string secret;
        public FileUploader(string id)
        {
            this.id = id;
            http.BaseAddress = new Uri("https://api.imgur.com/3/upload");
        }
        public void UploadImage(IFormFile image)
        {
            using (var ms = new MemoryStream())
            {
                image.CopyTo(ms);
                byte[] fileBytes = ms.ToArray();
                //string s = Convert.ToBase64String(fileBytes);
                // act on the Base64 data
                MultipartFormDataContent form = new();
                //form.Add(fileBytes, "image", image.FileName);
            }
            
            //form.Add(image.OpenReadStream, "image");
            //fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
        }
    }
}
