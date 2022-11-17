using System;
using System.Reactive;
using System.Reactive.Linq;

namespace SuppaZ80.Models;

public static class ReactiveMixin
{
    public static IObservable<Unit> ToSignal<T>(this IObservable<T> observable)
    {
        return observable.Select(_ => Unit.Default);
    }

    public static IObservable<TRet> To<T, TRet>(this IObservable<T> observable, TRet value)
    {
        return observable.Select(_ => value);
    }
}