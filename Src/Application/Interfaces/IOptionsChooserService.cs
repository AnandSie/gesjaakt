namespace Application.Interfaces;

public interface IOptionsChooserService
{
    public T ChoiceFromPlayer<T>(string startMessage , IEnumerable<T> options) where T: IOption ;
}