namespace Application.Interfaces;

public interface IDisplay
{
    void UpdateMessage(string message);

    // Retires the pinned block. The live standings exist to show progress while a
    // run is going; once it has finished they would sit under the real results
    // saying the same thing twice.
    void Clear();
}
