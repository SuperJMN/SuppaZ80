using System;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;

namespace SuppaZ80.ViewModels;

public static class Mixin
{
    public static IObservable<T> WhereSuccess<T>(this IObservable<Result<T>> self)
    {
        return self.Where(a => a.IsSuccess)
            .Select(x => x.Value);
    }

    public static IObservable<bool> IsSuccess<T>(this IObservable<Result<T>> self)
    {
        return self
            .Select(a => a.IsSuccess);
    }

    public static IObservable<string> WhereNotEmpty(this IObservable<string> self)
    {
        return self.Where(s => !string.IsNullOrWhiteSpace(s));
    }

    public static IObservable<bool> SelectNotEmpty(this IObservable<string> self)
    {
        return self.Select(s => !string.IsNullOrWhiteSpace(s));
    }
}