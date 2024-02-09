namespace PROJE_UI
{
    public class ApiResponseModel<T>
    {
            public T Data { get; set; }
            public bool Success { get; set; }
            public string Message { get; set; }
        
    }
}
