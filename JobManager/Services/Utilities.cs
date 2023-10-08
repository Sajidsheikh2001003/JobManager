using System.ComponentModel.DataAnnotations;

namespace JobManager.Services
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ValidatePdfFile : ValidationAttribute
    {
        public int FileSize { get; set; } = 3000000;//3MB
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            IFormFile? file = value as IFormFile;
            int allowedFileSize = this.FileSize;
            if (file != null)
            {
                if (file.Length > allowedFileSize)
                {
                    return new ValidationResult($"Invalid File Size. Permissible upload size is {FileSize / 1000000} MB");
                }
                if (file.ContentType != "application/pdf")
                {
                    return new ValidationResult($"Invalid File Type. You can upload only PDF file");
                }
                return ValidationResult.Success;
            }
            //return new ValidationResult($"No file selected to upload");
            return ValidationResult.Success;
        }
    }
}
