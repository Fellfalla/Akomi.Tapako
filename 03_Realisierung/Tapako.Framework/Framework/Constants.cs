using System.Text.RegularExpressions;

namespace Tapako.Framework
{
    /// <summary>
    /// Klasse, die alle Konstanten beinhaltet
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Output String bevor der übergebene Fehler ausgegeben wird.
        /// </summary>
        public static string LoggerErrorMessage = "Error: ";

        /// <summary>
        /// Output String bevor der übergebene Fehler ausgegeben wird.
        /// </summary>
        public static string LoggerInfoMessage = "Info: ";

        /// <summary>
        /// Output String bevor der übergebene Fehler ausgegeben wird.
        /// </summary>
        public static string LoggerWarningMessage = "Warning: ";

        /// <summary>
        /// Output String bevor der übergebene Fehler ausgegeben wird.
        /// </summary>
        public static string LoggerDebugMessage = "Debug Message: ";

        ///// <summary>
        ///// Level, ab dem Warnings ausgegeben werden
        ///// </summary>
        //public static int LoggingLevelWarning = 2;

        ///// <summary>
        ///// Level, ab den Errors ausgegeben werden
        ///// </summary>
        //public static int LoggingLevelError = 1;

        ///// <summary>
        ///// Levels, aber dem Infos ausgegeben werden
        ///// </summary>
        //public static int LoggingLevelInfo = 3;

        ///// <summary>
        ///// Das aktuelle LoggingLevel, welches bestimmt, welche Messages ausgegeben werden
        ///// </summary>
        //public static int LoggingLevel = LoggingLevelInfo;


        /// <summary>
        /// Pfad zum Repository, welches alle PLC Searcher Driver enthält
        /// </summary>
        public static string PathPlcSearcherDriverRepository = @"DriverRepository";


        public static string DeviceDriverRepository = @"DriverRepository";

        /// <summary>
        /// Standard Subnetadresse, in der nach Hosts gesucht wird
        /// </summary>
        public static string DefaultSubnet = @"10.0.0.";
        
        /// <summary>
        /// Standardname des MACRepositorys
        /// </summary>
        public static string DefaultMacRepository = @"MacList.txt";

        /// <summary>
        /// Timeout for trying to ping in ms
        /// </summary>
        public static int TimeoutPing = 3500;

        /// <summary>
        /// Regular Expression, welche alle Buchstaben enthält, die im Dateinamen eines Treibers vorkommen dürfen
        /// </summary>
        public static Regex ValidDriverCharactersRegex = new Regex(@"[^a-zA-Z0-9_\.]");

        /// <summary>
        /// Char, durch den alle vinvaliden Zeichen eines Strings ersetzt werden
        /// </summary>
        public static char ReplacingChar = '_';

        /// <summary>
        /// Zeichen, die als SubnetString akzeptiert werden
        /// </summary>
        public static Regex ValidSubnetStringCharactersRegex = new Regex(@"[^\d\.]");
    }
}
