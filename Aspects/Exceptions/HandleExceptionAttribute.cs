using Metalama.Framework.Aspects;

namespace AtividadeLabIV.Aspects.Exceptions;

public class HandleExceptionAttribute : OverrideMethodAspect
{
    public override dynamic? OverrideMethod()
    {
        try
        {
            return meta.Proceed();
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERRO] Ocorreu um problema ao executar a operação.");
            Console.WriteLine($"[LOG] {ex.Message}");
            return default;
        }
    }
}
