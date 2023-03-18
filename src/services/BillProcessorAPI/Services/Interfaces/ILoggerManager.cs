using Application.AutofacDI;

namespace BillProcessorAPI.Services.Interfaces
{
    public interface ILoggerManager :IAutoDependencyService
    {
        void LogInfo(string message);
        void LogWarn(string message);
        void LogDebug(string message);
        void LogError(Exception ex);
    }
}
