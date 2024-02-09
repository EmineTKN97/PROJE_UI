
using Newtonsoft.Json;
using PROJE_UI.Models;
using PROJE_UI;
using System.Text;

public class ApiService : IApiService
{
    private readonly HttpClient _client;

    public ApiService(HttpClient client)
    {
        _client = client;
    }

    public async Task<List<Blog>> GetBlogsByCommentAndLikeCount()
    {
        var response = await _client.GetAsync("https://localhost:7185/api/Blogs/GetBlogsByCommentAndLikeCount");

        if (response.IsSuccessStatusCode)
        {
            var apiResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApiResponseModel<List<Blog>>>(apiResponse);

            return result.Data;
        }
        else
        {
            // Hata durumuyla başa çıkın
            throw new Exception("API ile iletişim sırasında bir hata oluştu.");
        }
    }

    public async Task<Blog> GetBlogById(Guid blogId)
    {
        var response = await _client.GetAsync($"https://localhost:7185/api/Blogs/GetById?id={blogId}");

        if (response.IsSuccessStatusCode)
        {
            var apiResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Blog>(apiResponse);

            return result;
        }
        else
        {
            // Hata durumuyla başa çıkın
            throw new Exception("API ile iletişim sırasında bir hata oluştu.");
        }
    }

    public async Task<List<BlogComment>> GetCommentsByBlogId(Guid blogId)
    {
        var response = await _client.GetAsync($"https://localhost:7185/api/BlogComments/GetCommentsByBlogId?BlogId={blogId}");

        if (response.IsSuccessStatusCode)
        {
            var apiResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<BlogComment>>(apiResponse);

            return result;
        }
        else
        {
            // Hata durumuyla başa çıkın
            throw new Exception("API ile iletişim sırasında bir hata oluştu.");
        }
    }

    public async Task<BlogComment> AddComment(BlogComment comment)
    {
        StringContent content = new StringContent(JsonConvert.SerializeObject(comment), Encoding.UTF8, "application/json");

        using (var response = await _client.PostAsync("https://localhost:7185/api/BlogComments/AddComment", content))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<BlogComment>(apiResponse);

                return result;
            }
            else
            {
                // Hata durumuyla başa çıkın
                throw new Exception("Blog Yorumu eklenirken bir hata oluştu.");
            }
        }
    }
}