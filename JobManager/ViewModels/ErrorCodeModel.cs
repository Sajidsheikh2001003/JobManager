namespace EasyQ.Models
{
    public class ResponseModel
    {
        public ResponseModel(bool isSuccess, int errorCode, string errorMessage)
        {
            IsSuccess = isSuccess;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public int ErrorCode { get; set; }
    }
    public class ResponseModel<T>:ResponseModel
    {
        public ResponseModel(bool isSuccess, int errorCode, string errorMessage)
            : base(isSuccess, errorCode, errorMessage)
        {
           
        }
        
        public T Result { get; set; }
    }


    public class ErrorCodeModel
    {
        public ErrorCodeModel(bool isSuccess, int errorCode, string errorMessage)
        {
            IsSuccess = isSuccess;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public int ErrorCode { get; set; }
    }
}
