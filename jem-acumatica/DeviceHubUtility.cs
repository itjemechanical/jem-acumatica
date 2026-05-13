using PX.Common;
using PX.Data;
using PX.Objects.CS;
using PX.SM;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Objects.Common
{
    public static class DeviceHubUtility
    {
        public static void PrintReportToDeviceHub(PXGraph graph, string reportID, Dictionary<string, string> parameters)
        {
            if (!PXAccess.FeatureInstalled<FeaturesSet.deviceHub>())
                return;

            Dictionary<PrintSettings, PXReportRequiredException> jobs = new Dictionary<PrintSettings, PXReportRequiredException>();

            foreach (SMPrinter printer in PXSelect<SMPrinter, Where<SMPrinter.isActive, Equal<True>>>.Select(graph))
            {
                var printSettings = new PrintSettings
                {
                    PrintWithDeviceHub = true,
                    DefinePrinterManually = true,
                    PrinterID = printer.PrinterID,
                    NumberOfCopies = 1
                };

                PXTrace.WriteInformation($"Enviando reporte '{reportID}' a la impresora '{printer.Description}' ({printer.PrinterName})");

                var exception = PXReportRequiredException.CombineReport(null, reportID, parameters);
                jobs.Add(printSettings, exception);
            }

            SMPrintJobMaint.CreatePrintJobGroups(jobs, CancellationToken.None);
        }
    }
}