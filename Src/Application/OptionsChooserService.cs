using System.Text;
using Application.Interfaces;

namespace Application;
public class OptionsChooserService: IOptionsChooserService
{
    private readonly IPlayerInputProvider _playerInputProvider;

    public OptionsChooserService(IPlayerInputProvider playerInputProvider)
    {
        _playerInputProvider = playerInputProvider;
    }

    public T ChoiceFromPlayer<T>(string startMessage , IEnumerable<T> options) where T : IOption
    {
        var optionList = options.ToList();
        if (optionList.Count == 0)
        {
            throw new ArgumentException("Cannot choose from an empty list of options.", nameof(options));
        }

        var messageBuilder = new StringBuilder();
        messageBuilder.AppendLine(startMessage);
        
        var optionLines = optionList.Select((option, index) => $"{index + 1} - {option.Name}");
        messageBuilder.AppendJoin('\n', optionLines).AppendLine();

        var validChoices = Enumerable.Range(1, optionList.Count);
        var choice = _playerInputProvider.GetPlayerInputAsInt(messageBuilder.ToString(), validChoices);
        return optionList[choice - 1];
    }
}
