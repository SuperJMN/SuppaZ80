using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Helpers;

namespace SuppaZ80.ViewModels;

public class ModifiableValue : ReactiveValidationObject
{
    [Reactive] public string Text { get; set; }
}