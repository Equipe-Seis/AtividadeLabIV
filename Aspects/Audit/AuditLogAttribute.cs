using Metalama.Framework.Aspects;

namespace AtividadeLabIV.Aspects.Audit;

public class AuditLogAttribute : OverrideMethodAspect
{
    public override dynamic? OverrideMethod()
    {
        Console.WriteLine($"[AUDIT] Executando: {meta.Target.Method.Name}");

        foreach (var p in meta.Target.Method.Parameters)
        {
            Console.WriteLine($"  → {p.Name}: {p.Value}");
        }

        var result = meta.Proceed();

        Console.WriteLine($"[AUDIT] Finalizado: {meta.Target.Method.Name}");

        return result;
    }
}
