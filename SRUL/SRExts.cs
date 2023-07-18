using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.Utils.Extensions;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using SmartAssembly.Attributes;
using SRUL.Annotations;
using SRUL.Types;

namespace SRUL
{
    // public static class StringExtensions
    // {
    //     public static LineSplitEnumerator SplitLines(this string str)
    //     {
    //         // LineSplitEnumerator is a struct so there is no allocation here
    //         return new LineSplitEnumerator(str.AsSpan());
    //     }
    //
    //     // Must be a ref struct as it contains a ReadOnlySpan<char>
    //     public ref struct LineSplitEnumerator
    //     {
    //         private ReadOnlySpan<char> _str;
    //
    //         public LineSplitEnumerator(ReadOnlySpan<char> str)
    //         {
    //             _str = str;
    //             Current = default;
    //         }
    //
    //         // Needed to be compatible with the foreach operator
    //         public LineSplitEnumerator GetEnumerator() => this;
    //
    //         public bool MoveNext()
    //         {
    //             var span = _str;
    //             if (span.Length == 0) // Reach the end of the string
    //                 return false;
    //
    //             var index = span.IndexOfAny('+', '.');
    //             if (index == -1) // The string is composed of only one line
    //             {
    //                 _str = ReadOnlySpan<char>.Empty; // The remaining string is an empty string
    //                 Current = new LineSplitEntry(span, ReadOnlySpan<char>.Empty);
    //                 return true;
    //             }
    //
    //             if (index < span.Length - 1 && span[index] == '+')
    //             {
    //                 // Try to consume the '\n' associated to the '\r'
    //                 var next = span[index + 1];
    //                 if (next == '\n')
    //                 {
    //                     Current = new LineSplitEntry(span.Slice(0, index), span.Slice(index, 2));
    //                     _str = span.Slice(index + 2);
    //                     return true;
    //                 }
    //             }
    //
    //             Current = new LineSplitEntry(span.Slice(0, index), span.Slice(index, 1));
    //             _str = span.Slice(index + 1);
    //             return true;
    //         }
    //
    //         public LineSplitEntry Current { get; private set; }
    //     }
    //
    //     public readonly ref struct LineSplitEntry
    //     {
    //         public LineSplitEntry(ReadOnlySpan<char> line, ReadOnlySpan<char> separator)
    //         {
    //             Line = line;
    //             Separator = separator;
    //         }
    //
    //         public ReadOnlySpan<char> Line { get; }
    //         public ReadOnlySpan<char> Separator { get; }
    //
    //         // This method allow to deconstruct the type, so you can write any of the following code
    //         // foreach (var entry in str.SplitLines()) { _ = entry.Line; }
    //         // foreach (var (line, endOfLine) in str.SplitLines()) { _ = line; }
    //         // https://docs.microsoft.com/en-us/dotnet/csharp/deconstruct?WT.mc_id=DT-MVP-5003978#deconstructing-user-defined-types
    //         public void Deconstruct(out ReadOnlySpan<char> line, out ReadOnlySpan<char> separator)
    //         {
    //             line = Line;
    //             separator = Separator;
    //         }
    //
    //         // This method allow to implicitly cast the type into a ReadOnlySpan<char>, so you can write the following code
    //         // foreach (ReadOnlySpan<char> entry in str.SplitLines())
    //         public static implicit operator ReadOnlySpan<char>(LineSplitEntry entry) => entry.Line;
    //     }
    // }
    
    [DoNotObfuscate]
    public static class StringExtensions
    {
        public static LineSplitEnumerator SplitWithSpan(this string str)
        {
            // LineSplitEnumerator is a struct so there is no allocation here
            return new LineSplitEnumerator(str.AsSpan());
        }

        // Must be a ref struct as it contains a ReadOnlySpan<char>
        public ref struct LineSplitEnumerator
        {
            private ReadOnlySpan<char> _str;

            public LineSplitEnumerator(ReadOnlySpan<char> str)
            {
                _str = str;
                Current = default;
            }

            // Needed to be compatible with the foreach operator
            public LineSplitEnumerator GetEnumerator() => this;

            public bool MoveNext()
            {
                var span = _str;
                if (span.Length == 0) // Reach the end of the string
                    return false;

                var index = span.IndexOfAny('+', ',');
                if (index == -1) // The string is composed of only one line
                {
                    _str = ReadOnlySpan<char>.Empty; // The remaining string is an empty string
                    Current = new LineSplitEntry(span, ReadOnlySpan<char>.Empty);
                    return true;
                }

                // if (index < span.Length - 1 && span[index] == '\r')
                // {
                //     // Try to consume the '\n' associated to the '\r'
                //     var next = span[index + 1];
                //     if (next == ',')
                //     {
                //         Current = new LineSplitEntry(span.Slice(0, index), span.Slice(index, 2));
                //         _str = span.Slice(index + 2);
                //         return true;
                //     }
                // }

                Current = new LineSplitEntry(span.Slice(0, index), span.Slice(index, 1));
                _str = span.Slice(index + 1);
                return true;
            }

            public LineSplitEntry Current { get; private set; }
        }

        public readonly ref struct LineSplitEntry
        {
            public LineSplitEntry(ReadOnlySpan<char> line, ReadOnlySpan<char> separator)
            {
                Line = line;
                Separator = separator;
            }

            public ReadOnlySpan<char> Line { get; }
            public ReadOnlySpan<char> Separator { get; }

            // This method allow to deconstruct the type, so you can write any of the following code
            // foreach (var entry in str.SplitLines()) { _ = entry.Line; }
            // foreach (var (line, endOfLine) in str.SplitLines()) { _ = line; }
            // https://docs.microsoft.com/en-us/dotnet/csharp/deconstruct?WT.mc_id=DT-MVP-5003978#deconstructing-user-defined-types
            public void Deconstruct(out ReadOnlySpan<char> line, out ReadOnlySpan<char> separator)
            {
                line = Line;
                separator = Separator;
            }

            // This method allow to implicitly cast the type into a ReadOnlySpan<char>, so you can write the following code
            // foreach (ReadOnlySpan<char> entry in str.SplitLines())
            public static implicit operator ReadOnlySpan<char>(LineSplitEntry entry) => entry.Line;
        }
    }
    
    public static class NumericExtension
    {
        static public decimal SafeDecimalDivision(this decimal Numerator, decimal Denominator)
        {
            return (Denominator == 0) ? 0 : Numerator / Denominator;
        }

        public static int SafeIntDivision(this int Numerator, int Denominator)
        {
            return (Denominator == 0) ? 0 : Numerator / Denominator;
        }

        public static string SafePercentage(this decimal current, decimal total, string format = "P1") {
            return (total == 0) ? "0" : (current / total).ToString(format);
        }
        
        public static decimal SafePercentage(this decimal current, decimal total)
        {
            return (total == 0) ? 0 : (current / total);
        }
        
        public static Int64 ConvertToInt64(this string val)
        {
            if (val != string.Empty)
                return Convert.ToInt64(Single.Parse(val));
            return 0;
        }

        public static IEnumerable<T> ApplyToAll<T>(this IEnumerable<T> sequence, 
            params Action<T>[] actions)
        {
            foreach (var element in sequence)
            {
                foreach (var action in actions)
                {
                    action(element);
                }
                yield return element;
            }
        }

        public static string ToSemanticRepresentation(this decimal arg, bool noKMB = false)
        {
            decimal result;
            if (decimal.TryParse(arg.ToString(), out result))
            {
                if (result >= 1000000000)
                    return (result / 1000000000).ToString(noKMB ? $"#,##0" : $"#,##0.0b") ;
                if (result >= 1000000)
                    return (result / 1000000).ToString(noKMB ? $"#,##0" : $"#,##0m") ;
                if (result >= 1000)
                    return (result / 1000).ToString(noKMB ? $"#,##0" : $"#,##0k") ;
                if (result >= 1)
                    return result.ToString("#,##0");
            }
            else
                return string.Format("{0:C0}", result.ToString()) + " M";
            return string.Format("{0}", result.ToString("0"));
        }

        public static string ToSemanticRepresentation(this decimal arg, string addition, bool active = false)
        {
            decimal result;
            if (decimal.TryParse(arg.ToString(), out result) && active)
            {
                if (result >= 1000000000)
                    return (result / 1000000000).ToString($"##,###,##0.## B{addition}");
                if (result >= 1000000)
                    return (result / 1000000).ToString($"##,###,##0.## M{addition}");
                if (result >= 1000)
                    return (result / 1000).ToString($"#,##0.## K{addition}");
                if (result >= 1)
                    return result.ToString($"#,##0{addition}");
            }
            else
                return result.ToString($"#,##0{addition}");
            return string.Format(new CultureInfo("en-US"), "{0:C0}", result.ToString());
        }
        public static decimal RatingDivision(this decimal ratingValue)
        {
            return ratingValue / 10;
        }
        
        public static int IfZero(this int thisValue, int showThis) => thisValue == 0 ? showThis : thisValue;
        public static int IfZero(this int thisValue, int ifThisZero, int showThis) => ifThisZero <= 0 ? showThis : thisValue;
    }

    [DoNotObfuscateType]
    public static class SystemExtension
    {
        static Dictionary<string, CancellationTokenSource> _srFreezeTokenSrcs =
            new Dictionary<string, CancellationTokenSource>();

        // public static void ObserveFreeze()
        // {
        //     CancellationTokenSource cts = new CancellationTokenSource();
        //     string realAddress = GetCode(address, file).ToUInt32().ToString("X");
        //
        //     if (SRFreezeTokenSrcs.ContainsKey(realAddress))
        //     {
        //         Debug.WriteLine("Changing SRFreezing Address " + realAddress + " Value " + value);
        //         try
        //         {
        //             SRFreezeTokenSrcs[realAddress].Cancel();
        //             SRFreezeTokenSrcs.Remove(realAddress);
        //         }
        //         catch
        //         {
        //             Debug.WriteLine("ERROR: Avoided a crash. Address " + realAddress + " was not frozen.");
        //         }
        //     }
        //     else
        //         Debug.WriteLine("Adding SRFreezing Address " + realAddress + " Value " + value);
        //
        //     FreezeTokenSrcs.Add(realAddress, cts);
        //
        //     Task.Factory.StartNew(() =>
        //         {
        //             while (!cts.Token.IsCancellationRequested)
        //             {
        //                 WriteMemory(realAddress, type, value, file);
        //                 Thread.Sleep(25);
        //             }
        //         },
        //         cts.Token);
        // }

        public static void FloatBelow(this float value, double compareTo, float threshold)
        {
            if (value < compareTo - threshold)
                throw new Exception("Value is below " + threshold + " from " + compareTo);
        }
        
        public static bool FloatBelow(this float value, double thisValue, bool orTheSame = false)
        {
            if (!orTheSame)
            {
                if (value < thisValue)
                    return true;
            }
            else
            {
                if(value <= thisValue)
                    return true;
            }

            return false;
        }
        
        // Method that return generic T, which check above parameter A and below parameter B
        public static bool FloatBetween(this float value, double aboveThis, double belowThis, bool orTheSame = false)
        {
            if (!orTheSame)
            {
                if (value > aboveThis && value < belowThis)
                    return true;
            }
            else
            {
                if (value >= aboveThis && value <= belowThis)
                    return true;
            }

            return false;
        }

        public static T ReturnThis<T>(this bool condition, T resultToReturn)
        {
            if (condition)
                return resultToReturn;
            return resultToReturn;
        }
        public static void DoThisAction(this bool condition, Action act)
        {
            if(condition) 
                act();
        }
            
        public static void WriteTo(this Feature feat, SRReadWrite readInstance, dynamic value)
        {
            SRLoaderForm._srLoader.rw.SRWrite(feat.name, value);
        }
        
        public static Feature WriteTo(this Dictionary<bool, Feature> feat, SRReadWrite readInstance, dynamic value)
        {
            if (feat.ContainsKey(true))
            {
                SRLoaderForm._srLoader.rw.SRWrite(feat[true].name, value == null ? value : value.ToString());
                return feat[true];
            }

            return feat[false];
        }
        
        public static Feature SafelyWriteTo(this Dictionary<bool, Feature> feat, SRReadWrite readInstance, dynamic? value)
        {
            if (feat.ContainsKey(true))
            {
                SafeWrite(feat[true], readInstance, value == null ? value : value.ToString());
                return feat[true];
            }

            return feat[false];
        }

        public static void ElseWriteTo(this Feature feat, SRReadWrite readInstance, dynamic value)
        {
            SRLoaderForm._srLoader.rw.SRWrite(feat.name, value.ToString());
        }

        public static object[] Use(this Feature feature, SRReadWrite rw)
        {
            return new object[] { rw, feature };
        }
        
        // public static object[] ToWrite(this object[] WriterAndFeatures, dynamic value)
        // {
        //     if (WriterAndFeatures[0] as Feature != null 
        //         && WriterAndFeatures[1] as SRReadWrite != null)
        //         (WriterAndFeatures[0] as Feature).WriteTo((WriterAndFeatures[1] as SRReadWrite), value.ToString());
        //     return WriterAndFeatures;
        // }
        public static object[] AndIf(this object[] features, bool condition)
        {
            return new object[]{features[0], features[1], condition};
        }

        public static object[] AndIfItsEqualTo(this object[] features, int intToCheck)
        {
            return new object[]{features[0], features[1], features[2], intToCheck};
        }
        
        public static object[] WithInt(this object[] features, int indexCase)
        {
            return new object[] {features[0], features[1], indexCase};
        }
        
        public static object[] WithTheValueOf(this object[] features, bool boolCase)
        {
            if (boolCase)
                return new[] { features[0], features[1], true };
            return new object[]{};
        }
        
        public static object[] AndTheValueOf(this object[] features, bool boolCase)
        {
            // Guard
            if (features.Length < 3) return new object[] { };
            if (features[2].GetType() != typeof(bool)) return new object[]{};
            if ((bool)features[2] &&  boolCase)
                return new[] { features[0], features[1], true };
            return new[] { features[0], features[1], false };
        }

        public static void WriteThis(this object[] featuresWithSize3, dynamic trueValue, dynamic falseValue)
        {
            if (featuresWithSize3.Length < 3) return;
            if(featuresWithSize3[2].GetType() != typeof(bool)) return;
            
            (featuresWithSize3[0] as SRReadWrite)?
                    .SRWrite((featuresWithSize3[1] as Feature)?.name, 
                        ((bool)featuresWithSize3[2]) 
                            ? trueValue 
                            : falseValue);
        }
        public static object[] WriteThis(this object[] features, dynamic value)
        {
            if(features.Length == 3)
            {
                if (((bool)features[2]))
                {
                    (features[0] as SRReadWrite)?.SRWrite((features[1] as Feature)?.name, value.ToString());
                    return features;
                }
                return features;   
            } 
            if(features.Length == 4) 
                if (((int)features[2]) == (int)features[3])
                    {
                        (features[0] as SRReadWrite)?.SRWrite((features[1] as Feature)?.name, value.ToString());
                        return features;
                    }
            return features;   
        }
        

        public static Dictionary<bool, Feature> IfTrue(this Feature feature, bool condition)
        {
            return new Dictionary<bool, Feature> { { condition, feature } };
        }

        public static void FreezeIt(this Feature feat, SRReadWrite readInstance, dynamic value,
            bool allowIncrease = false)
        {
            readInstance.SRFreeze(feat.name, value.ToString(), allowIncrease);
        }

        public static bool CanWrite(this Feature feat, SRReadWrite readInstance)
        {
            var pointer = SRMain.Instance.PointerStore(feat.name);
            if (readInstance.MemoryAvailable(pointer))
                // readInstance.Read(feat.type, pointer);
                return true;
            return false;
        }

        public static void InGridView(this string[]? colNames, GridView view, Action<GridColumn> act)
        {
            foreach (var colName in colNames)
            { 
                if(view.Columns.ColumnByFieldName(colName) != null) 
                    act(view.Columns[colName]);
            }
        }
        
        public static void WithColNames(this GridView view, string[]? colNames, Action<GridColumn> act)
        {
            foreach (var colName in colNames)
            { 
                if(view.Columns.ColumnByFieldName(colName) != null) 
                    act(view.Columns[colName]);
            }
        }
        
        public static Feature ThenWrite(this bool canWriteChain, string featName, string value)
        {
            var pointer = SRMain.Instance.FeatureIndexedStore[featName];
            if (canWriteChain)
                if (SRLoaderForm._srLoader.rw.SRWrite(pointer.name, value))
                    return pointer;
            return pointer;
        }
        
        public static bool SafeWrite(this Feature feat, SRReadWrite rw, string value)
        {
            if (feat.CanWrite(rw))
            {
                feat.WriteTo(rw, (value));
                return true;
            }
            return false;
        }
        
        public static Feature? Read(this Feature feat)
        {
            var pointer = SRMain.Instance.FeatureIndexedStore[feat.name];
            if (pointer.CanWrite(SRLoaderForm._srLoader.rw))
            {
                pointer.value = SRLoaderForm._srLoader.rw.SRRead(pointer.name);
                return pointer;
            } else 
                return null;
        }

        public static string ValueFilter(this string st)
        {
            return string.IsNullOrEmpty(st) ? "0": st;
        }

        public static bool IsEither(this string st, string val1, string val2)
        {
            return st.Equals(val1) || st.Equals(val2);
        }

        public static void SetWithValueFromMemory(this Feature feat)
        {
            if (feat.CanWrite(SRLoaderForm._srLoader.rw)) 
                feat.value = SRLoaderForm._srLoader.rw.SRRead(feat.name);
        }

        public static dynamic Read(this Feature? feat, SRReadWrite readInstance)
        {
            return readInstance.SRRead(feat.name);
        }
        
        public static int ReadInt(this Feature feat, SRReadWrite readInstance)
        {
            if (feat == null) return 0;
            return readInstance.ReadInt(SRMain.Instance.PointerStore(feat.name));
        }

        public static Feature SetFromRead(this Feature feat, SRReadWrite readInstance, bool rawValue = false)
        {
            feat.value = readInstance.SRRead(feat.name, rawValue);
            return feat;
            // return Convert.ToInt32(Convert.ToDecimal(value));
        }

        public static void WriteDecimalTo(this Feature feat, SRReadWrite readInstance, decimal value)
        {
            readInstance.SRWrite(feat.name, value.ToString());
            // return Convert.ToInt32(Convert.ToDecimal(value));
        }
        
        private static string[] collectedUnits = new string[200];
        // Concrete 
        public static UIntPtr GetStaticAddress(this string pointerOffset)
        {
            return SRLoaderForm._srLoader.rw.GetCode(pointerOffset);
        }

        public static string GetStaticUnitAddress(this Feature feature)
        {
            var p = feature.GetPointer(SRLoaderForm._srLoader.rw);
            if (SRMemento.Instance.TrackedUnitPointerCollection.TryGetValue(p, out var unitPointer))
                return unitPointer;
            SRMemento.Instance.TrackedUnitPointerCollection.TryAdd(p, p);
            return p;
        }
        
        public static string GetPointerString(this Feature featureName)
        {
            // TODO: Part 1, to make a blog, how i managed to get rid of 8000mb Small object heap DPA analysis
            // var featName = SRMain.Instance.pointerStore(varName);
            var featName = SRMain.Instance.FeaturePointerStore[featureName.name];
            return featName;
        }
        
        public static string GetPointer(this Feature featureName, SRReadWrite readInstance)
        {
            // TODO: Part 1, to make a blog, how i managed to get rid of 8000mb Small object heap DPA analysis
            // var featName = SRMain.Instance.pointerStore(varName);
            var featName = SRMain.Instance.FeaturePointerStore[featureName.name];
            var realAddress = readInstance.GetCode(featName).ToUInt32().ToString("X");
            return realAddress;
        }

        public static string GetPointer(this string varName, SRReadWrite readInstance)
        {
            // TODO: Part 1, to make a blog, how i managed to get rid of 8000mb Small object heap DPA analysis
            // var featName = SRMain.Instance.pointerStore(varName);
            var featName = SRMain.Instance.FeaturePointerStore[varName];
            var realAddress = readInstance.GetCode(featName);
            return realAddress.ToUInt32().ToString("X");
        }
        
        public static UIntPtr GetPointerUIntPtr(this string varName, SRReadWrite readInstance)
        {
            // TODO: Part 1, to make a blog, how i managed to get rid of 8000mb Small object heap DPA analysis
            // var featName = SRMain.Instance.pointerStore(varName);
            if(SRMain.Instance.FeaturePointerStore == null) return UIntPtr.Zero;
            if (!SRMain.Instance.FeaturePointerStore.ContainsKey(varName)) return UIntPtr.Zero;
            var featName = SRMain.Instance.FeaturePointerStore[varName];
            return readInstance.GetCode(featName);
        }

        public static bool IsValid(this UIntPtr ptr)
        {
            if(ptr == UIntPtr.Zero)
                return false;
            if (ptr.ToUInt64() < 0x10000)
                return false;
            return true;
        }

        public static string GetBattalionSizeFromUnitClass(this string unitClass)
        {
            switch (unitClass)
            {
                case "0":
                    return "WarfareBattalionSizeRecon";
                case "1":
                    return "WarfareBattalionSizeTank";
                case "2":
                    return "WarfareBattalionSizeAntiTank";
                case "3":
                    return "WarfareBattalionSizeArtillery";
                case "4":
                    return "5";
                case "5":
                    return "6";
                case "6":
                    return "7";
                case "7":
                    return "8";
                case "8":
                    return "9";
                case "9":
                    return "10";
                case "10":
                    return "11";
                case "11":
                    return "12";
                case "12":
                    return "13";
                case "13":
                    return "14";
                case "14":
                    return "15";
                case "15":
                    return "16";
                case "16":
                    return "17";
                case "17":
                    return "18";
                case "18":
                    return "19";
                case "19":
                    return "20";
                case "20":
                    return "21";
                case "21":
                    return "22";
                case "22":
                    return "23";
                case "23":
                    return "24";
            }

            return "test";
        }
        
        public static UIntPtr GetPointerUIntPtr(this Feature featureName, SRReadWrite readInstance)
        {
            // TODO: Part 1, to make a blog, how i managed to get rid of 8000mb Small object heap DPA analysis
            // var featName = SRMain.Instance.pointerStore(featureName);
            var featName = SRMain.Instance.FeaturePointerStore?[featureName.name];
            if (featName != null) return readInstance.GetCode(featName);
            return UIntPtr.Zero;
        }
        
        // public static UIntPtr GetPointer(this string varName, SRReadWrite readInstance)
        // {
        //     // TODO: Part 1, to make a blog, how i managed to get rid of 8000mb Small object heap DPA analysis
        //     // var featName = SRMain.Instance.pointerStore(varName);
        //     var featName = SRMain.Instance.FeaturePointerStoreRaw[varName];
        //     var realAddress = readInstance.GetCode(featName.ToUInt32().ToString("X"));
        //     return realAddress;
        // }
        
        public static string GetPointer(this string varName, SRReadWrite readInstance, IList<Feature> source)
        {
            // TODO: Part 1, to make a blog, how i managed to get rid of 8000mb Small object heap DPA analysis
            // var featName = SRMain.Instance.pointerStore(varName);
            var featName = SRMain.Instance.FeaturePointerStore[varName];
            var realAddress = readInstance.GetCode(featName).ToUInt32().ToString("X");
            return realAddress;
        }

        public static decimal ConvertRating(this decimal ratingValue)
        {
            switch (ratingValue)
            {
                case 0:
                    return 0;
                case 1:
                    return 0.25m;
                case 2:
                    return 1;
                case 3:
                    return 5;
                default:
                    return 0;
            }
        }
        
        public static decimal ConvertRatingForDisplay(this decimal ratingValue)
        {
            if (ratingValue >= 0 && ratingValue < 0.25m)
                return 0;
            if (ratingValue >= 0.25m && ratingValue < 1.0m)
                return 1;
            if (ratingValue >= 1m && ratingValue < 2.25m)
                return 2;
            if (ratingValue >= 2.25m)
                return 3;
            return ratingValue;
        }

        public static decimal ConvertRatingForEdit(this decimal ratingValue)
        {
            if (ratingValue == 0)
                return 0;
            if (ratingValue == 1)
                return 0.25m;
            if (ratingValue == 2)
                return 1.0m;
            if (ratingValue == 3)
                return 5.25m;
            return ratingValue;
        }
        public static dynamic FixMeUp<T>(this T fixMe)
        {
            var t = fixMe.GetType();
            var returnClass = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;
            foreach (var pr in t.GetProperties())
            {
                var val = pr.GetValue(fixMe);
                if (val is string && string.IsNullOrWhiteSpace(val.ToString()))
                {
                }
                else if (val == null)
                {
                }
                else
                {
                    returnClass.Add(pr.Name, val);
                }
            }

            return returnClass;
        }
        
        public static SubCategory? GetSubCategory(this string varName)
        {
            if (SRMain.Instance.FeatureIndexedStore.ContainsKey(varName))
                foreach (var subCategory in SRMain.Instance.DataSubCategories)
                {
                    if (subCategory.categoryName.Equals(SRMain.Instance.FeatureIndexedStore[varName].subCategory))
                        return subCategory;
                }
            
            return null;
        }

        public static object GetCellValue(this GridView gv, string featureName)
        {
            return gv.GetRowCellValue(featureName.GetFeature().gridId, "value");
        }

        // private static int _temp;
        public static int AsInt([NotNull]this object obj)
        {
            // _temp = Int32.TryParse(obj);
            return (int)obj;
        }
        
        public static Feature? GetFeature(this string varName)
        {
            if (SRMain.Instance.FeatureIndexedStore.ContainsKey(varName))
                return SRMain.Instance.FeatureIndexedStore[varName];
            return null;
        }
        
        public static Feature GetFeature(this string varName, SREnum.CategoryName category)
        {
            
            return category switch
            {
                SREnum.CategoryName.Resources when 
                    SRMain.Instance.ResourcesIndexedFeatures.ContainsKey(varName) => 
                    SRMain.Instance.ResourcesIndexedFeatures[varName],
                SREnum.CategoryName.Country when 
                    SRMain.Instance.CountryIndexedFeatures.ContainsKey(varName) => 
                    SRMain.Instance.CountryIndexedFeatures[varName],
                SREnum.CategoryName.Special when 
                    SRMain.Instance.SpecialIndexedFeatures.ContainsKey(varName) => 
                    SRMain.Instance.SpecialIndexedFeatures[varName],
                SREnum.CategoryName.Warfare when 
                    SRMain.Instance.WarfareIndexedFeatures.ContainsKey(varName) => 
                    SRMain.Instance.WarfareIndexedFeatures[varName], 
                _ => SRMain.Instance.FeatureIndexedStore[varName]
            };
        }
        
        public static Feature? GetFeature(this string varName, string category)
        {
            return category.ToLower() switch
            {
                "resource" when SRMain.Instance.ResourcesIndexedFeatures
                        .ContainsKey(varName) => 
                    SRMain.Instance.ResourcesIndexedFeatures[varName],
                "country" when SRMain.Instance.CountryIndexedFeatures
                        .ContainsKey(varName) => 
                    SRMain.Instance.CountryIndexedFeatures[varName],
                "special" when SRMain.Instance.SpecialIndexedFeatures
                        .ContainsKey(varName) => 
                    SRMain.Instance.SpecialIndexedFeatures[varName],
                "warfare" when SRMain.Instance.WarfareIndexedFeatures
                        .ContainsKey(varName) => 
                    SRMain.Instance.WarfareIndexedFeatures[varName], 
                _ => null
            };
        }

        public static Feature GetFeatureByName(this List<Feature> featureCollection, string featureName)
        {
            // feat = new ConcurrentBag<Feature>();
            foreach (var ft in featureCollection)
            {
                if (ft.name.Equals(featureName))
                    return ft;
            }
            throw new Exception();
        }
        // static ConcurrentBag<Feature> feat;
        public static Feature GetFeatureByName(this IList<Feature> featureCollection, string featureName)
        {
            // feat = new ConcurrentBag<Feature>();
            foreach (var ft in featureCollection)
            {
                if (ft.name.Equals(featureName))
                    return ft;
            }
            throw new Exception();
            // Parallel.ForEach(featureCollection, (feature) =>
            // {
            //     var name = featureName;
            //     if (feature.name.Equals(name)) 
            //         feat.Add(feature);
            // });
            // featureCollection.AsParallel().First(s => s.name == featureName).AsOrdered();
            // return feat.First() ?? throw new Exception();
        }

        public static Feature GetFeature(this string varName, IList<Feature> from = null)
        {
            if (from != null)
                return from.SingleOrDefault(s => s.name == varName);
            return GetFeature(varName);
        }
        
        public static Feature GetFeatureFromCache(this string featureName, Dictionary<string, Feature> cache)
        {
            if (cache != null)
                return cache[featureName];
            return SRMain.Instance.FeatureIndexedStore[featureName];
        }

        public static Feature Copy(this Feature feat)
        {
            return feat.ShallowCopy();
        }

        public static string[] GetIncludedFeatures(
            this string[] s, 
            Category category,
            int[] categoryIndexToBeIncluded = null)
        {
            List<string> result = new List<string>();
            foreach (var subCategory in category.subCategories)
            {
                if (categoryIndexToBeIncluded == null)
                {
                    result.AddRange(subCategory.categoryIncludedFeatures);
                }
                else
                {
                    foreach (var idx in categoryIndexToBeIncluded)
                    {
                        if (idx == subCategory.id)
                            result.AddRange(subCategory.categoryIncludedFeatures);
                    }
                }
            }

            s = result.Distinct().ToArray();
            return s;
        }
        
        [DoNotPrune]
        public static string[] GetSetExcludedFeatures(
            this string[] s, 
            Category category,
            string[] ArrayOfIncludedFeatures = null)
        {
            List<string> result = new List<string>();
            try {
                foreach (var features in category.features)
                {
                    if(!ArrayOfIncludedFeatures.Contains(features.name))
                        result.Add(features.name);
                }
                
                s = result.Distinct().ToArray();
                return s;
            }
            catch (Exception e)
            {
                throw new Exception("Mapping Failed!", e);
            }
        }

        public static string[] GetSubCategoryIncludedFeaturesById(this Category categoryName, int subCategoryIndex)
        {
            return (new string[] { }).GetIncludedFeatures(categoryName, new[] { subCategoryIndex });
        }

        public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> source, int count)
        {
            var enumerator = source.GetEnumerator();
            var queue = new Queue<T>(count + 1);

            while (true)
            {
                if (!enumerator.MoveNext())
                    break;
                queue.Enqueue(enumerator.Current);
                if (queue.Count > count)
                    yield return queue.Dequeue();
            }
        }
        
        public static bool WriteIntoMemory(this Feature feat, SRReadWrite writeInstance)
        {
            var pointer = feat.name.GetPointer(writeInstance);
            return writeInstance.WriteMemory(pointer, feat.type, feat.value);
        }

        public static T SerializeCloneMethod<T>(this T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }

        public static IList<SRUL.Feature> CloneFeatures<T>(this IList<SRUL.Feature> source)
        {
            IList<Feature> newClone = new List<Feature>();
            foreach (var clone in source)
            {
                newClone.Add(clone.ShallowCopy());
            }

            return newClone;
        }

        public static bool IsBetween(this double value, double minimum, double maximum)
        {
            return value > minimum && value < maximum;
        }
        
        public static bool IsBetween(this float value, float minimum, float maximum)
        {
            return value > minimum && value < maximum;
        }
        
        public static bool IsBetween(this decimal value, decimal minimum, decimal maximum)
        {
            return value > minimum && value < maximum;
        }
        
        public static void Do(this bool cond, Action action)
        {
            if (cond)
                action();
        }

        public static double StrToDouble(this string value)
        {
            if(value == null || value == "")
                return -1;
            return Convert.ToDouble(value);
        }
        
        // convert string to int, then convert  to ordinal
        public static string ToOrdinal(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return "0";
            return Convert.ToInt32(value).ConverstToOrdinal();
        }


        public static string ConverstToOrdinal(this int num)
        {
            if (num <= 0)
                return num.ToString();

            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return $"{num.ToString()}th";
            }

            switch (num % 10)
            {
                case 1:
                    return $"{num.ToString()}st";
                case 2:
                    return $"{num.ToString()}nd";
                case 3:
                    return $"{num.ToString()}rd";
                default:
                    return $"{num.ToString()}th";
            }
        }
        
        public static decimal StrToDecimal(this string value)
        {
            return String.IsNullOrEmpty(value) ? 0 : Convert.ToDecimal(value);
        }

        public static int StrToInt(this string value)
        {
            int i;
            if (!int.TryParse(value, out i)) i = 0;
            // return Convert.ToInt32(Convert.ToDecimal(value));
            return i;
        }

        public static object gvGetRowCellValue(this GridView gv, string fieldName, string varName)
        {
            object val = "0";
            for (int i = 0; i < gv.RowCount; i++)
            {
                var name = gv.GetRowCellValue(i, gv.Columns["name"]);
                if (varName != name) continue;
                val = gv.GetRowCellValue(i, fieldName);
                break;
            }

            return val;
        }

        public static void gvSetRowCellValue(this GridView gv, string fieldName, string varName, string value,
            bool setSource = false)
        {
            var c = gv.DataController;
            object getCellValue(int id, string ss) => gv.GetListSourceRowCellValue(id, gv.Columns[ss]);
            for (var i = 0; i < c.ListSourceRowCount; i++)
            {
                if (!string.Equals(varName, getCellValue(i, "name").ToString(),
                        StringComparison.CurrentCultureIgnoreCase)) continue;
                if (setSource)
                    varName.GetFeature(getCellValue(i, "category").ToString())!.value = value;
                    // SRMain.Instance.FeatureByCategoryAndName(
                    //     getCellValue(i, "category").ToString(),
                    //     getCellValue(i, "name").ToString()).value = value;
                else
                    gv.SetRowCellValue(i, gv.Columns[fieldName], value);
                gv.PostEditor();

                break;
                // return true;
            }
            // return false;
        }

        public static bool gvSetFreezeRowCellValue(this GridView gv, string varName, bool value)
        {
            for (int i = 0; i < gv.DataRowCount; i++)
            {
                var name = gv.GetRowCellValue(i, gv.Columns["name"]);
                if (varName != name as string) continue;
                gv.SetRowCellValue(i, "freeze", value);
                return true;
            }

            return false;
        }

        public static void DisposeSequence<T>(this IEnumerable<T> source)
        {
            foreach (IDisposable disposableObject in source.OfExactType<IDisposable>())
            {
                disposableObject.Dispose();
            };
        }
    }
}