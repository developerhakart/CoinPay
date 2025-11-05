namespace CoinPay.Api.Services.Swap.Exceptions;

/// <summary>
/// Exception thrown when wallet has insufficient balance for swap
/// </summary>
public class InsufficientBalanceException : InvalidOperationException
{
    public decimal Required { get; }
    public decimal Available { get; }
    public decimal Shortfall { get; }

    public InsufficientBalanceException(string message, decimal required, decimal available)
        : base(message)
    {
        Required = required;
        Available = available;
        Shortfall = required - available;
    }

    public InsufficientBalanceException(string message) : base(message)
    {
    }
}
