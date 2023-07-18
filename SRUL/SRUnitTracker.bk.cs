// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.Linq;
// using System.Threading;
//
// namespace SRUL
// {
//     
//     // TODO: Not usable atm.
//         public class UnitTracker : IObservable<Unit>
//     {  
//         private IList<IObserver<Unit>> units;
//         CancellationTokenSource cts = new CancellationTokenSource();
//
//         public UnitTracker()
//         {
//             units = new List<IObserver<Unit>>();
//         }
//         public IDisposable Subscribe(IObserver<Unit> observer)
//         {
//             if(!units.Contains(observer))
//                 units.Add(observer);
//             return new Unsubscriber(units, observer);
//         }
//         
//         // public IDisposable AddToObserve(Nullable<Units> unit)
//         // {
//         //     if(!units.Contains(unit))
//         //         units.Add(unit);
//         //     return new Unsubscriber(units, unit);
//         // }
//
//         private class Unsubscriber : IDisposable
//         {
//             private IList<IObserver<Unit>> _units;
//             private IObserver<Unit> _unit;
//             
//             public Unsubscriber(IList<IObserver<Unit>> observers, IObserver<Unit> observer)
//             {
//                 this._units = observers;
//                 this._unit = observer;
//             }
//
//             public void Dispose()
//             {
//                 if (_unit != null && _units.Contains(_unit))
//                 {
//                     _units.Remove(_unit);
//                 }
//             }
//         }
//         // public void TrackUnit(Unit feat)
//         // {
//         //     foreach (var unit in units)
//         //     {
//         //         // if (units.Contains(feat))
//         //         // {
//         //         //     unit.OnError(new FeatureUnknownException());
//         //         // }
//         //         // else
//         //         // {
//         //         //     unit.OnNext(feat);
//         //         // }
//         //         if (!feat.HasValue)
//         //         {
//         //             unit.OnError(new FeatureUnknownException());
//         //         }
//         //         else
//         //         {
//         //             unit.OnNext(feat.Value);
//         //         }
//         //     }
//         // }
//
//         public void EndTransmission()
//         {
//             foreach (var unit in units.ToArray())
//                 if(units.Contains(unit))
//                         unit.OnCompleted();
//             units.Clear();
//         }
//             
//         public class FeatureUnknownException : Exception
//         {
//             internal FeatureUnknownException()
//             { }
//         }
//     }
//     
//     public class UnitReporter : IObserver<Unit>
//     {
//         private IDisposable unsubscriber;
//         private string instName;
//
//         public UnitReporter(string name)
//         {
//             this.instName = name;
//         }
//
//         public string Name
//         {  get{ return this.instName; } }
//
//         public virtual void Subscribe(IObservable<Unit> provider)
//         {
//             if (provider != null)
//                 unsubscriber = provider.Subscribe(this);
//         }
//
//         public virtual void OnCompleted()
//         {
//             Debug.WriteLine("The Location Tracker has completed transmitting data to {0}.", this.Name);
//             this.Unsubscribe();
//         }
//
//         public virtual void OnError(Exception e)
//         {
//             Debug.WriteLine("{0}: The location cannot be determined.", this.Name);
//         }
//
//         public virtual void OnNext(Unit value)
//         {
//             Debug.WriteLine("{2}: The current location is {0}, {1}", value.UnitAddress, value.UnitStats, this.Name);
//         }
//
//         public virtual void Unsubscribe()
//         {
//             unsubscriber.Dispose();
//         }
//     }
// }