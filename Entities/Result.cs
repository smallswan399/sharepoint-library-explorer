namespace Entities
{
    public enum ResultStatus
    {
        Success, Pending, Error, None
    }
    public class Result
    {
        public ResultStatus Status { get; set; }
        public string Message { get; set; }

        public Result()
        {
            Status = ResultStatus.None;
        }
    }
}
