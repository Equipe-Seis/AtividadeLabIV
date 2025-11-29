using Metalama.Framework.Aspects;

namespace AtividadeLabIV.Aspects.Notify;

public class NotificacaoAttribute : OverrideMethodAspect
{
    public override dynamic? OverrideMethod()
    {
        var result = meta.Proceed();

        Console.WriteLine($"[NOTIFY] Operação concluída: {meta.Target.Method.Name}");

        return result;
    }
}
