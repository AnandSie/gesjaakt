namespace Domain.Entities.Game.Qwixx;

// QX-010: identifies exactly one of the (up to 8) colored candidate sums a player chose to mark.
public record QwixxMark(QwixxColor Color, int Number);
