using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using SDT.Helpers;
using SDT.Models;
using SDT.Services;

namespace SDT.ViewModels
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        private User userModel;
        private PC pcModel;
        private Printer printerModel;

        private UserService userService;
        private PCService pcService;
        private PrinterService printerService;

        public IDialogCoordinator dialogCoordinator;

        public ApplicationViewModel(IDialogCoordinator instance)
        {
            userModel = new User();
            pcModel = new PC();
            printerModel = new Printer();
            dialogCoordinator = instance;

            userService = new UserService(userModel, this, dialogCoordinator);
            pcService = new PCService(pcModel, this, dialogCoordinator);
            printerService = new PrinterService(printerModel, this, dialogCoordinator);

        }

        /// <summary>
        ///  Model Properties
        /// </summary>
        public User UserModel
        {
            get { return userModel; }
            set
            {
                if (Equals(userModel, value)) return;
                userModel = value;
                OnPropertyChanged("UserModel");
            }
        }

        public PC PCModel
        {
            get { return pcModel; }
            set
            {
                if (Equals(pcModel, value)) return;
                pcModel = value;
                OnPropertyChanged("PCModel");
            }
        }

        public Printer PrinterModel
        {
            get { return printerModel; }
            set
            {
                if (Equals(printerModel, value)) return;
                printerModel = value;
                OnPropertyChanged("PrinterModel");
            }
        }


        /// <summary>
        ///   User Commands
        /// </summary>
        private async Task ExecuteCheckUser() 
        {
            await userService.CheckUser();
            OnPropertyChanged("UserModel");
        
        }
        private ICommand _userCheckCommand;
        public ICommand UserCheckCommand 
        { 
            get 
            { 
                return _userCheckCommand ?? (_userCheckCommand = new RelayCommandAsync(ExecuteCheckUser, (c) => true)); 
            } 
        }

        /// <summary>
        ///   Printer Commands
        /// </summary>
        private async Task ExecuteCheckPrinter()
        {
            await printerService.PrinterCheck();
            OnPropertyChanged("PrinterModel");
        }
        private ICommand _printerCheckCommand;
        public ICommand PrinterCheckCommand
        {
            get
            {
                return _printerCheckCommand ?? (_printerCheckCommand = new RelayCommandAsync(ExecuteCheckPrinter, (c) => true));
            }
        }

        /// <summary>
        ///   PC Commands
        /// </summary>
        private async Task ExecuteCheckPc()
        {
            await pcService.CheckPc();
            OnPropertyChanged("PCModel");
        }
        private ICommand _pcCheckCommand;
        public ICommand PcCheckCommand
        {
            get
            {
                return _pcCheckCommand ?? (_pcCheckCommand = new RelayCommandAsync(ExecuteCheckPc, (c) => true));
            }
        }

        private async Task ExecuteRunRCV()
        {
            await pcService.RunRCV();
            OnPropertyChanged("PCModel");
        }
        private ICommand _runRCVCommand;
        public ICommand RunRCVCommand
        {
            get
            {
                return _runRCVCommand ?? (_runRCVCommand = new RelayCommandAsync(ExecuteRunRCV, (c) => true));
            }
        }

        private async Task ExecuteRunSharing()
        {
            await pcService.RunSharing();
            OnPropertyChanged("PCModel");
        }
        private ICommand _runSharingCommand;
        public ICommand RunSharingCommand
        {
            get
            {
                return _runSharingCommand ?? (_runSharingCommand = new RelayCommandAsync(ExecuteRunSharing, (c) => true));
            }
        }

        private async Task ExecuteRunPsExec()
        {
            await pcService.RunPsExec();
            OnPropertyChanged("PCModel");
        }
        private ICommand _runPsExecCommand;
        public ICommand RunPsExecCommand
        {
            get
            {
                return _runPsExecCommand ?? (_runPsExecCommand = new RelayCommandAsync(ExecuteRunPsExec, (c) => true));
            }
        }

        private async Task ExecuteRunPingT()
        {
            await pcService.RunPingT();
            OnPropertyChanged("PCModel");
        }
        private ICommand _runPingTCommand;
        public ICommand RunPingTCommand
        {
            get
            {
                return _runPingTCommand ?? (_runPingTCommand = new RelayCommandAsync(ExecuteRunPingT, (c) => true));
            }
        }

        private async Task ExecuteRunGPUUpdate()
        {
            await pcService.RunGPUUpdate();
            OnPropertyChanged("PCModel");
        }
        private ICommand _runGPUUpdateCommand;
        public ICommand RunGPUUpdateCommand
        {
            get
            {
                return _runGPUUpdateCommand ?? (_runGPUUpdateCommand = new RelayCommandAsync(ExecuteRunGPUUpdate, (c) => true));
            }
        }

        private async Task ExecuteCheckBitLocker()
        {
            await pcService.CheckBitLocker();
            OnPropertyChanged("PCModel");
        }
        private ICommand _checkBitLockerCommand;
        public ICommand CheckBitLockerCommand
        {
            get
            {
                return _checkBitLockerCommand ?? (_checkBitLockerCommand = new RelayCommandAsync(ExecuteCheckBitLocker, (c) => true));
            }
        }

        private async Task ExecuteRunSpoolReset()
        {
            await pcService.RunSpoolReset();
            OnPropertyChanged("PCModel");
        }
        private ICommand _runSpoolResetCommand;
        public ICommand RunSpoolResetCommand
        {
            get
            {
                return _runSpoolResetCommand ?? (_runSpoolResetCommand = new RelayCommandAsync(ExecuteRunSpoolReset, (c) => true));
            }
        }




        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}
