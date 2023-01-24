namespace Majorsilence.Vpn.Logic.Payments;

public interface IPaypalSettings
{
    string Username { get; init; }
    string Password { get; init; }
    string Url { get; init; }
}