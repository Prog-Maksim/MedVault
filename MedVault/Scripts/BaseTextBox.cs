using MedVault.Controls;

namespace MedVault.Scripts;

public interface BaseTextBox
{
    public string GetResult() => String.Empty;

    public DoubleValues GetValues() => new ();
}