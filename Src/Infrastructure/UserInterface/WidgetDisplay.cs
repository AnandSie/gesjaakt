using Application.Interfaces;

namespace UserInterface;

public class WidgetDisplay : Form, IDisplay
{
    private readonly TextBox _textBox;

    public WidgetDisplay()
    {
        Text = "Simulation Dashboard";
        Width = 400;
        Height = 300;

        _textBox = new TextBox
        {
            Multiline = true,
            Dock = DockStyle.Fill,
            ReadOnly = true,
            Font = new Font("Consolas", 12)
        };

        Controls.Add(_textBox);
    }

    public void UpdateMessage(string message)
    {
        _textBox.Text = message;
    }
}
