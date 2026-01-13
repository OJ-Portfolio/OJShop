namespace OJCommerce.Enums.Payments
{
    public enum PaymentMethod
    {
        Card = 1,
        BankTransfer = 2,
        MobileMoney = 3,      // MTN, Airtel, etc.
        USSD = 4,             // *737# codes
        QRCode = 5,
        PayPal = 6,
        ApplePay = 7,
        GooglePay = 8,
        BankAccount = 9,       // Direct debit
        PayOnDelivery = 10,
        Installments = 11,      // Buy now, pay later
        Any = 99
    }
}
