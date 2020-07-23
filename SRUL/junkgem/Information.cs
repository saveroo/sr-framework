using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using Memories;

namespace SRUL
{
    using System.Data;
    using System.Windows.Forms;

    public class Information
    {
        public DataTable countryInfoTable = new DataTable();
        public DataRow countryInfoRow;
        public DataColumn countryInfoColumn;
        private Boolean isCreated;
        public ReadWrite rw;

        public Information(ReadWrite rw)
        {
            this.rw = rw;
        }

        public Boolean IsCreated
        {
            //s
            get { return isCreated; }
            set { isCreated = value; }
        }

        public DataTable DelegateTable()
        {
            if (!isCreated)
            {
                // Dispatcher.BeginInvoke();
                countryInfoTable.Columns.AddRange(new DataColumn[]
                {
                    new DataColumn("Editable", System.Type.GetType("System.Boolean")),
                    new DataColumn("Freeze", typeof(System.Boolean)),
                    new DataColumn("Info Name"),
                    new DataColumn("Value", typeof(string)),
                    new DataColumn("Description", typeof(string)),
                    new DataColumn("Delegation", typeof(string))
                });
                // countryInfoTable.Columns[0].ColumnMapping = MappingType.Hidden;
                // countryInfoTable.Columns[5].ColumnMapping = MappingType.Hidden;
                // countryInfoTable.AcceptChanges();
                isCreated = true;
            }

            return this.countryInfoTable;
        }

        public delegate float functionPointer();
//        public functionPointer readPopulation = new ReadWrite().readPopulation;

        public void PopulateRow()
        {
            //            functionPointer readPopulation = rw.readPopulation().ToString;
            AddToRowAsync(true,
                    "Treasury",
                    rw.FinTreasury.ToString(),
                    "Country Treasury",
                    "FinTreasury");
            AddToRowAsync(true,
                "GDP",
                rw.FinGDP.ToString(),
                "Countries GDPc",
                "FinGDP");
            // CountryInformation
            AddToRowAsync(false,
                "Population",
                rw.DomPopulation.ToString(),
                " Will affect consumption",
                "DomPopulation");
            AddToRowAsync(false,
            "Immigration",
            rw.DomImmigration.ToString(),
            " Will Increase Population",
                "DomImmigration");
            AddToRowAsync(false,
            "Emigration",
            rw.DomEmigration.ToString(),
            " Will Decrease Population",
                "DomEmigration");
            AddToRowAsync(false,
            "Birth",
            rw.DomBirth.ToString(),
            " Will Increase Population",
            "DomBirth");
            AddToRowAsync(false,
            "Death",
            rw.DomDeath.ToString(),
            " Will Decrease Population",
            "DomDeath");
            AddToRowAsync(false,
            "Literacy",
            rw.DomLiteracy.ToString(),
            " Will Increase Research Speed",
                "DomLiteracy");
            AddToRowAsync(false,
            "Tourism",
            rw.DomTourism.ToString(),
            " Will Increase Immigration",
            "DomTourism")
                ;
            AddToRowAsync(false,
            "Unemployment",
            rw.DomUnemployment.ToString(),
            " Will Increase Immigration",
                "DomUnemployment");
            AddToRowAsync(true,
            "Inflation",
            rw.FinInflation.ToString(),
            " Affect GDP", "FinInflation");
            AddToRowAsync(false,
            "Credit Rating",
            rw.DomCreditRating.ToString(),
            " ", "DomCreditRating");

            AddToRowAsync(false,
            "Treaty Integrity",
            rw.DomTreatyIntegrity.ToString(),
            " ",
            "DomTreatyIntegrity");
            AddToRowAsync(false,
            "Domestic Approval",
            rw.DomDomApproval.ToString(),
            " ",
            "DomDomApproval");
            AddToRowAsync(false,
            "Military Approval",
            rw.DomMilApproval.ToString(),
            " ",
            "DomMilApproval");
            AddToRowAsync(false,
            "World Subsidy",
            rw.DomUNSubsidy.ToString(),
            " ",
            "DomUNSubsidy");
            AddToRowAsync(false,
            "World Opinion",
            rw.DomUNOpinion.ToString(),
            " ",
            "DomUNOpinion");
            // var t = Task.Run(() =>
            // {
            //     
            // });
            // await t.ConfigureAwait(false);
        }

        public Type GetTheType(string strFullyQualifiedName)
        {
            Type type = Type.GetType(strFullyQualifiedName);
            if (type != null)
                return type;
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = asm.GetType(strFullyQualifiedName);
                if (type != null)
                    return type;
            }
            return null;
        }

        public void UpdateTable()
        {
            if (!isCreated || !rw.Allowed) return;
            // Console.WriteLine("isCreated" + isCreated);
            // Console.WriteLine("rw" + rw.Allowed);
////                functionPointer rP = rw.readPopulation;
////                MethodInfo[] methodInfos = typeof(ReadWrite).GetMethods();
            for (int i = 0; i < countryInfoTable.Rows.Count; i++)
            {
////                    ReadWrite r = new ReadWrite();
////                    MethodInfo mi = this.GetType().GetMethod(countryInfoTable.Rows[i][1].ToString());
////                    countryInfoTable.Rows[i][1] = countryInfoTable.Rows[i][1].GetType().GetMethod(countryInfoTable.Rows[i][1].ToString();
////                    MethodInfo mf = rw.GetType().GetMethod(countryInfoTable.Rows[i][3].ToString());
////                    countryInfoTable.Rows[i][1] = r.GetType().GetMethod(countryInfoTable.Rows[i][3].ToString()).Invoke(r, null);
                // Type gp = GetTheType("SRUL.ReadWrite");
                // object objMyClass = Activator.CreateInstance(gp, rw);
                // // new MethodInvoker(delegate { rw.Load(); });
                // // objMyClass.GetType().GetMethod("Load");
                // MethodInfo minf = objMyClass.GetType().GetMethods().FirstOrDefault(
                //     method => method.Name == countryInfoTable.Rows[i][3].ToString() && method.GetParameters().Count()== 0);
                // countryInfoTable.Rows[i][1] = minf.Invoke(objMyClass, null);
                // MethodInfo mInfo = rw.GetType().GetMethod(countryInfoTable.Rows[i][3].ToString());
                // var mInfo = rw.GetType().GetProperty(countryInfoTable.Rows[i][5].ToString());
                // countryInfoTable.Rows[i][1] = mInfo.Invoke(rw, null);
                // Console.WriteLine(mInfo);
                // if (countryInfoTable.Rows[i][0].Equals("Inflation"))
                // {
                //     countryInfoTable.Rows[i][1] = mInfo.Invoke(rw, null);
                // }
                // else
                // {
                // countryInfoTable.Rows[i][1] = mInfo.Invoke(rw, null);
                // if (mInfo != null) countryInfoTable.Rows[i][3] = mInfo.GetValue(rw, null);
                if(countryInfoTable.Rows[i][2] == "Treasury")
                    countryInfoTable.Rows[i][3] = ReadWrite.FormatNumber(rw.InvokeDelegation(countryInfoTable.Rows[i][5].ToString()));
                else if(countryInfoTable.Rows[i][2] == "GDP")
                    countryInfoTable.Rows[i][3] = ReadWrite.FormatNumber(rw.InvokeDelegation(countryInfoTable.Rows[i][5].ToString()));
                else
                    countryInfoTable.Rows[i][3] = rw.InvokeDelegation(countryInfoTable.Rows[i][5].ToString());
                // }
                // .Invoke(objMyClass, null);
                // countryInfoTable.Rows[i][1] = rw.ReadPopulation().ToString();
                //                if (countryInfoTable.Rows[i][4] == "int")
                //                    countryInfoTable.Rows[i][1] = read
            }
        }

        public void AddToRowAsync(bool colEditable, string colName, string colVal, string colDesc, string colDel)
        {
            this.countryInfoRow = countryInfoTable.NewRow();
            this.countryInfoRow["Editable"] = colEditable;
            this.countryInfoRow["Freeze"] = false;
            this.countryInfoRow["Info Name"] = colName;
            this.countryInfoRow["Value"] = colVal;
            this.countryInfoRow["Description"] = colDesc;
            this.countryInfoRow["Delegation"] = colDel;
            countryInfoTable.Rows.Add(countryInfoRow);
        }
    }
}