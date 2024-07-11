namespace Erp.Base
{
    public class ResponseModel<T> where T : class
    {
        public ResponseModel()
        {
            this.Items = new List<T>();
            this.IsOk = true;
        }

        public List<T> Items { get; set; }

        public T Item { get; set; }

        public bool IsOk { get; set; }  

        public string Message { get; set; }
    }
}
