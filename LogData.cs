using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;

namespace Publisher
{
public class LogData
{
    [Name("Log Time")]
    public string LogTime { get; set; }
    public string StepNo { get; set; }
    public string CircuitName { get; set; }
    public string TMP1 { get; set; }
    public string TMP2 { get; set; }
    public string B31 { get; set; }
    public string B32 { get; set; }
    public string B21 { get; set; }
    public string B22 { get; set; }
    public string P101 { get; set; }
    public string RegulatorSP { get; set; }
    public string RegulatorFB { get; set; }
}
