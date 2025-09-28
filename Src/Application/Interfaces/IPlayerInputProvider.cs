namespace Domain.Interfaces;

public interface IPlayerInputProvider
{
    string GetPlayerInput(string question);
    int GetPlayerInputAsInt(string question, IEnumerable<int> allowedInts);
    int GetPlayerInputAsIntWithMinMax(string question, int min, int max);
}
