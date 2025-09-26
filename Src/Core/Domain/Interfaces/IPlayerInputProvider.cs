namespace Domain.Interfaces;

public interface IPlayerInputProvider
{
    string GetPlayerInput(string question);
    // TODO: Another method where you can give an enum
    int GetPlayerInputAsInt(string question, IEnumerable<int> allowedInts);
    int GetPlayerInputAsIntWithMinMax(string question, int min, int max);
    int GetPlayerInputAsInt(IEnumerable<int> allowedInts);
    bool GetPlayerInputForYesNo();
}
