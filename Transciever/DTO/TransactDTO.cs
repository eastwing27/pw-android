using System;

namespace PW.DataTransciever.DTO
{
    public class TransactDTO
    {
        public DateTime TransactTime { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public double Amount { get; set; }
        public double SenderResult { get; set; }
        public double ReceiverResult { get; set; }
    }
}
