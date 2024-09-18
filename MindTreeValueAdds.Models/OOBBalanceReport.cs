using System;
using System.Data;
using System.Data.OracleClient;

namespace MindTreeValueAdds.Helppers
{
    public class OOBBalanceReport
    {
        public static bool ValidatePolicyNumber(string PolicyNumber)
        {
            bool result = false;

            if (!string.IsNullOrEmpty(PolicyNumber))
            {
                string[] FullPolicyData = PolicyNumber.Split(' ');
                if (FullPolicyData.Length == 4)
                {
                    if (!string.IsNullOrEmpty(FullPolicyData[1]) && !string.IsNullOrEmpty(FullPolicyData[2]) && !string.IsNullOrEmpty(FullPolicyData[3])) { result = true; }
                }
            }

            return result;
        }

        public static bool CheckOracleDBConnection(string connectionString)
        {
            bool result = false;

            if (!string.IsNullOrEmpty(connectionString))
            {
                OracleConnection oracleConnection = new OracleConnection(connectionString);
                oracleConnection.Open();
                if (oracleConnection.State == ConnectionState.Open) { result = true; }
                oracleConnection.Close();
                oracleConnection.Dispose();
            }

            return result;
        }

        public static DataTable ExicuteSQLScriptAsync(string connectionString, string SQLQuery)
        {
            DataTable results = new DataTable();

            if (!string.IsNullOrEmpty(connectionString) && !string.IsNullOrEmpty(SQLQuery))
            {
                OracleConnection oracleConnection = new OracleConnection(connectionString);

                try
                {
                    oracleConnection.Open();
                    OracleDataAdapter oracleDataAdapter = new OracleDataAdapter(SQLQuery, oracleConnection);
                    oracleDataAdapter.Fill(results);
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                    results.Clear();
                }
                finally
                {
                    if (oracleConnection.State == ConnectionState.Open)
                    {
                        oracleConnection.Close();
                        oracleConnection.Dispose();
                    }
                }
            }

            return results;
        }
    }
}