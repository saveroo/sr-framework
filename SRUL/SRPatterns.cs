// // using System;
// // using System.Collections.Generic;
// // using DevExpress.XtraBars.Ribbon;
// //
// // namespace SRUL;
// //
// // public interface IStatReader
// // {
// //     public void Read(string statName);
// //     public void Write(string value, string type);
// // }
// //
// // public interface EnlistedStatReader : IStatReader
// // {
// //     public void Enlist(string statName);
// //     public void Unenlist(string statName);
// // }
// //
// // public struct SoldierStat
// // {
// //     public string StatID;
// //     public string StatName;
// //     public string StatValue;
// // }
// //
// // public abstract class Soldier : EnlistedStatReader
// // {
// //     public string SoldierAddress;
// //     public string SoldierName;
// //     public string SoldierClass;
// //     public IList<SoldierStat> SoldierStats;
// //     public Soldier ShallowCopy()
// //     {
// //         return (Soldier) this.MemberwiseClone();
// //     }
// //
// //     public Soldier DeepCopy()
// //     {
// //         Soldier clone = (Soldier) this.MemberwiseClone();
// //         clone.SoldierFeature = new Feature(IdInfo.IdNumber);
// //         return clone;
// //     }
// //     
// //     public abstract void CreateSoldier();
// //
// //     public void Read(string statName)
// //     {
// //         throw new NotImplementedException();
// //     }
// //
// //     public void Write(string value, string type)
// //     {
// //         throw new NotImplementedException();
// //     }
// //
// //     public void Enlist(string statName)
// //     {
// //         throw new NotImplementedException();
// //     }
// //
// //     public void Unenlist(string statName)
// //     {
// //         throw new NotImplementedException();
// //     }
// // }
// // public class SoldierFactory: Soldier
// // {
// //     
// // }
//
// public class Originator
// {
//     private string _state;
//
//     public Originator(string state)
//     {
//         this._state = state;
//         Console.WriteLine("Originator: initial state > " + state);
//     }
//         
//     // The Originator's business logic may affect its internal state.
//     // Therefore, the client should backup the state before launching
//     // methods of the business logic via the save() method.
//     public void DoSomething()
//     {
//         Console.WriteLine("Originator: waa");
//         this._state = "test";
//     }
//         
//     public IMemento Save()
//     {
//         return new ConcreteMemento(this._state);
//     }
//         
//     public void Restore(IMemento memento)
//     {
//         if (!(memento is ConcreteMemento))
//         {
//             throw new Exception($"Unknown memento class {memento}");
//         }
//
//         this._state = memento.GetState();
//         Console.Write($@"Originator: My state has changed to: {_state}");
//     }
// }
//
// // Example code of MEMENTO Pattern Implementation
//     // The Memento interface provides a way to retrieve the memento's metadata,
//     // such as creation date or name. However, it doesn't expose the
//     // Originator's state.
//     public interface IMemento
//     {
//         string GetName();
//
//         string GetState();
//
//         DateTime GetDate();
//     }
//     
//     // The Concrete Memento contains the infrastructure for storing the
//     // Originator's state.
//     class ConcreteMemento : IMemento
//     {
//         private string _state;
//
//         private DateTime _date;
//
//         public ConcreteMemento(string state)
//         {
//             this._state = state;
//             this._date = DateTime.Now;
//         }
//
//         // The Originator uses this method when restoring its state.
//         public string GetState()
//         {
//             return this._state;
//         }
//         
//         // The rest of the methods are used by the Caretaker to display
//         // metadata.
//         public string GetName()
//         {
//             return $"{this._date} / ({this._state.Substring(0, 9)})...";
//         }
//
//         public DateTime GetDate()
//         {
//             return this._date;
//         }
//     }
//     
//     // The Caretaker doesn't depend on the Concrete Memento class. Therefore, it
//     // doesn't have access to the originator's state, stored inside the memento.
//     // It works with all mementos via the base Memento interface.
//     class Caretaker
//     {
//         private List<IMemento> _mementos = new List<IMemento>();
//
//         private Originator _originator = null;
//
//         public Caretaker(Originator originator)
//         {
//             this._originator = originator;
//         }
//
//         public void Backup()
//         {
//             Console.WriteLine("\nCaretaker: Saving Originator's state...");
//             this._mementos.Add(this._originator.Save());
//         }
//
//         public void Undo()
//         {
//             if (this._mementos.Count == 0)
//             {
//                 return;
//             }
//
//             var memento = this._mementos.Last();
//             this._mementos.Remove(memento);
//
//             Console.WriteLine("Caretaker: Restoring state to: " + memento.GetName());
//
//             try
//             {
//                 this._originator.Restore(memento);
//             }
//             catch (Exception)
//             {
//                 this.Undo();
//             }
//         }
//
//         public void ShowHistory()
//         {
//             Console.WriteLine("Caretaker: Here's the list of mementos:");
//
//             foreach (var memento in this._mementos)
//             {
//                 Console.WriteLine(memento.GetName());
//             }
//         }
//     }