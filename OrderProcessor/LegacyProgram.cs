// LEGACY ORDER PROCESSOR (intentionally messy, for refactoring practice)
// ---------------------------------------------------------------------
// Behavior summary (do not change without tests!):
// - Reads order records from a CSV (columns in any order, but must include: id, customer, type, amount, date, region, state).
// - If no file path is provided, it tries to read "orders.csv" from the working directory.
// - If file is missing, it falls back to 5 hard‑coded sample lines.
// - For each order, it applies a discount by "type" (Food=0.9, Electronics=1.1 surcharge, Other=1.0) — yes, Electronics is *more expensive* on purpose.
// - Adds tax by state (NY=8.875%, CA=7.25%, others=5%).
// - Skips negative or zero amounts, but logs a WARN (still includes line in report with amount=0).
// - Dates are accepted in multiple formats (yyyy-MM-dd, M/d/yyyy, or ticks); records with invalid date get today's date.
// - Output: Prints a crude table + a summary of total count and gross/net/revenue.
// - Side effects: writes a file "report.txt" with the same table.
// - Caches previously seen customer names (global cache) and prints a note when a repeat customer appears.
//
// Notes:
// - The code intentionally mixes parsing, calculation, IO, and formatting.
// - Duplication, magic numbers, poor naming, inconsistent casing, mutable statics, long methods, and error handling smells are left in on purpose.
// - Your task is to refactor while preserving behavior. Add tests first.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace OrderProcessor
{
    public class LegacyProgram
    {
        // Global mutable cache (yikes)
        static List<string> cacheOfCustomers = new List<string>();
        static Dictionary<string, double> typemap = new Dictionary<string, double>()
        {
            {"Food", 0.9},
            {"Electronics", 1.1},
            {"Other", 1.0},
            {"food", 0.9},
            {"ELECTRONICS", 1.1},
            {"other", 1.0}
        };

        static double taxNY = 0.08875; // magic numbers
        static double taxCA = 0.0725;
        static double taxDefault = 0.05;

        public static void Main(string[] args)
        {
            string path;
            if (args != null && args.Length > 0)
            {
                path = args[0];
            }
            else
            {
                path = "orders.csv"; // default
            }

            List<string> lines;
            if (File.Exists(path))
            {
                try
                {
                    lines = File.ReadAllLines(path).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERR: couldn't read file: " + ex.Message);
                    lines = GetFallbackLines();
                }
            }
            else
            {
                Console.WriteLine("WARN: file not found; using fallback data");
                lines = GetFallbackLines();
            }

            // Maybe CSV has a header; detect naively
            if (lines.Count > 0 && (lines[0].ToLower().Contains("id") || lines[0].ToLower().Contains("customer")))
            {
                lines = lines.Skip(1).ToList();
            }

            // process
            var reportLines = new List<string>();
            reportLines.Add("ID | Customer | Type | Amount | Date | Region | State | Net | Note");
            reportLines.Add(new string('-', 95));

            int totalCount = 0;
            double gross = 0; // original amount sum
            double revenue = 0; // after discounts & tax

            foreach (var l in lines)
            {
                if (string.IsNullOrWhiteSpace(l))
                {
                    continue; // skip blank
                }

                var parsed = parseLine(l);
                totalCount++;

                // parse amount twice (why not!)
                double amt;
                if (!double.TryParse(parsed.amount, NumberStyles.Any, CultureInfo.InvariantCulture, out amt))
                {
                    double t2;
                    bool ok2 = double.TryParse(parsed.amount, out t2);
                    if (ok2)
                    {
                        amt = t2;
                    }
                    else
                    {
                        Console.WriteLine("WARN: bad amount -> " + parsed.amount + "; will use 0");
                        amt = 0;
                    }
                }

                if (amt <= 0)
                {
                    Console.WriteLine("WARN: non-positive amount for id=" + parsed.id);
                }

                gross += amt;

                string type = parsed.type;
                double discountOrSurcharge = 1.0; // default
                if (typemap.ContainsKey(type))
                {
                    discountOrSurcharge = typemap[type];
                }
                else
                {
                    // try relaxed key
                    var k = (type ?? "").Trim();
                    if (typemap.ContainsKey(k))
                    {
                        discountOrSurcharge = typemap[k];
                    }
                    else
                    {
                        discountOrSurcharge = 1.0; // Other
                    }
                }

                double interim = amt * discountOrSurcharge;

                // TAX (duplicated style)
                double taxRate = 0;
                if (parsed.state == "NY")
                {
                    taxRate = taxNY;
                }
                else if (parsed.state == "CA")
                {
                    taxRate = taxCA;
                }
                else
                {
                    taxRate = taxDefault;
                }

                var taxed = interim + (interim * taxRate);
                revenue += taxed;

                // Date parsing (redundant + forgiving)
                DateTime dt;
                if (!DateTime.TryParseExact(parsed.date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    DateTime tmp;
                    if (!DateTime.TryParse(parsed.date, out tmp))
                    {
                        long ticks;
                        if (long.TryParse(parsed.date, out ticks))
                        {
                            dt = new DateTime(ticks);
                        }
                        else
                        {
                            dt = DateTime.Today;
                        }
                    }
                    else
                    {
                        dt = tmp;
                    }
                }

                string note = "";
                if (cacheOfCustomers.Contains(parsed.customer))
                {
                    note = "repeat-customer";
                }
                else
                {
                    cacheOfCustomers.Add(parsed.customer);
                }

                // build row (stringly)
                string row = parsed.id + " | " + parsed.customer + " | " + parsed.type + " | " + amt.ToString("0.00", CultureInfo.InvariantCulture)
                    + " | " + dt.ToString("yyyy-MM-dd") + " | " + parsed.region + " | " + parsed.state + " | " + taxed.ToString("0.00", CultureInfo.InvariantCulture)
                    + " | " + note;

                reportLines.Add(row);

                // duplicate logging path
                if (taxRate > 0.08)
                {
                    Console.WriteLine("INFO: high tax rate applied for state=" + parsed.state);
                }

                if (parsed.region == "EU" && parsed.state == "")
                {
                    Console.WriteLine("INFO: EU order without state code");
                }
            }

            // summary (inconsistently formatted)
            reportLines.Add(new string('-', 95));
            reportLines.Add("Total Orders: " + totalCount);
            reportLines.Add("Gross: " + gross.ToString("0.00", CultureInfo.InvariantCulture));
            reportLines.Add("Revenue: " + revenue.ToString("0.00", CultureInfo.InvariantCulture));
            double avg = 0;
            if (totalCount > 0) { avg = revenue / totalCount; }
            reportLines.Add("Avg Net/Order: " + avg.ToString("0.00", CultureInfo.InvariantCulture));

            foreach (var r in reportLines) Console.WriteLine(r);

            try
            {
                File.WriteAllLines("report.txt", reportLines);
            }
            catch (Exception e)
            {
                Console.WriteLine("WARN: couldn't write report.txt: " + e.Message);
            }
        }

        // CSV columns: id,customer,type,amount,date,region,state (but order may vary)
        private static (string id, string customer, string type, string amount, string date, string region, string state) parseLine(string l)
        {
            // naive split; no quoting support
            var parts = l.Split(',');
            string id = "", customer = "", type = "", amount = "", date = "", region = "", state = "";

            // try map by guessing headers if present (not really)
            if (parts.Length >= 7)
            {
                id = parts[0];
                customer = parts[1];
                type = parts[2];
                amount = parts[3];
                date = parts[4];
                region = parts[5];
                state = parts[6];
            }
            else
            {
                // try alternative order (duplicated logic)
                if (parts.Length == 6)
                {
                    id = parts[0];
                    customer = parts[1];
                    type = parts[2];
                    amount = parts[3];
                    date = parts[4];
                    region = parts[5];
                    state = "";
                }
                else
                {
                    // shrug; pad
                    var list = parts.ToList();
                    while (list.Count < 7) list.Add("");
                    id = list[0]; customer = list[1]; type = list[2]; amount = list[3]; date = list[4]; region = list[5]; state = list[6];
                }
            }

            return (id, customer, type, amount, date, region, state);
        }

        private static List<string> GetFallbackLines()
        {
            // (id,customer,type,amount,date,region,state)
            return new List<string>
            {
                "1,Ada Lovelace,Food,100.00,2024-07-01,US,NY",
                "2,Grace Hopper,Electronics,250.49,7/4/2024,US,CA",
                "3,Alan Turing,Other,-42,16908480000000000,EU,",
                "4,Katherine Johnson,Food,0.00,2024-10-15,US,TX",
                "5,Grace Hopper,Other,10.25,2024-12-31,US,WA"
            };
        }
    }
}
