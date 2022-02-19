using Core.DataClasses;

namespace BLL.Abstractions.Interfaces;

public interface IStartupService
{
    Task<ExceptionalResult> SetUp();
}