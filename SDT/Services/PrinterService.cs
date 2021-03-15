using MahApps.Metro.Controls.Dialogs;
using Microsoft.VisualBasic.FileIO;
using SDT.Models;
using SDT.ViewModels;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SDT.Services
{
    class PrinterService
    {
        private Printer printerModel;
        private ApplicationViewModel applicationVM;
        private IDialogCoordinator dialogCoordinator;
        private string PrinterName;

        public PrinterService(Printer PrinterModel, ApplicationViewModel ApplicationVM, IDialogCoordinator DialogCoordinator)
        {
            printerModel = PrinterModel;
            applicationVM = ApplicationVM;
            dialogCoordinator = DialogCoordinator;
        }

        public async Task PrinterCheck()
        {
            try
            {
                PrinterName = printerModel.PrinterName;


                if (!File.Exists(@"CSV FILE"))
                {
                    await dialogCoordinator.ShowMessageAsync(applicationVM, "Printer", "Brak dostępu do danych.");
                    return;
                }
                else
                {
                    TextFieldParser _fieldParser = new TextFieldParser(@"CSV FILE");
                    string currentLine;
                    _fieldParser.TextFieldType = FieldType.Delimited;
                    _fieldParser.Delimiters = new string[] { "|" };
                    _fieldParser.TrimWhiteSpace = true;
                    bool printerFound = false;

                    do
                    {
                        currentLine = _fieldParser.ReadLine();
                        if (currentLine != null)
                        {
                            string file = currentLine;

                            string serialNumber = file.Split('|')[0].Trim();
                            string adresIP = file.Split('|')[1].Trim();
                            string model = file.Split('|')[2].Trim();
                            string status = file.Split('|')[3].Trim();
                            string address = file.Split('|')[4].Trim();
                            string lan = file.Split('|')[11].Trim();
                            string guardian = file.Split('|')[12].Trim();
                            string server = file.Split('|')[13].Trim();
                            string queue = file.Split('|')[14].Trim();
                            string share = file.Split('|')[15].Trim();

                            if (adresIP == PrinterName || serialNumber == PrinterName)
                            {
                                printerFound = true;

                                printerModel.PrinterNS= serialNumber;
                                printerModel.PrinterIP = adresIP;
                                printerModel.PrinterModel = model;
                                printerModel.PrinterServer = server;
                                printerModel.PrinterStatus = status;
                                printerModel.PrinterGuardian = guardian;
                                printerModel.PrinterShare = share;
                                printerModel.PrinterLan = lan;
                                printerModel.PrinterAddress = address;
                                printerModel.PrinterQueue = queue;
                            }
                        }
                    } while (currentLine != null);
                    if (printerFound == false)
                    {
                        await dialogCoordinator.ShowMessageAsync(applicationVM, "Printer", "Nie znaleziono drukarki.");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                await dialogCoordinator.ShowMessageAsync(applicationVM, "Printer", ex.ToString());
                return;
            }
        }
    }
}
