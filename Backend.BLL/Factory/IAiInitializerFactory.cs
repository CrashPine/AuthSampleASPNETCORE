using Backend.BLL.AI;

namespace Backend.BLL.Factory;

public interface IAiInitializerFactory
{
    AiInitializer CreateLocal();
    AiInitializer CreateCloud();
}