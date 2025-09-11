namespace RestaurantManagement.Domain.Entities
{
    public enum PaymentMethod
    {
        Cash,
        CreditCard,
        BankTransfer,
        EWallet,
        Voucher
    }
    
    public class PaymentDetail
    {
        public int Id { get; set; }
        public int PaymentId { get; set; }
        public Payment Payment { get; set; } = null!;
        
        public PaymentMethod Method { get; set; } = PaymentMethod.Cash;
        public decimal Amount { get; set; }
        public string? TransactionCode { get; set; }
        public string? Provider { get; set; }
        public string? ExtraInfo { get; set; }
    }
}
