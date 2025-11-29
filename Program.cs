using AtividadeLabIV.Aspects.Audit;

namespace AtividadeLabIV;

public class Pedido
{
    public Guid Id { get; private set; }
    public string Cliente { get; private set; } = default!;
    public List<string> Produtos { get; private set; } = [];
    public double ValorTotal { get; private set; }

    private Pedido()
    {}

    public static Pedido CreateInstance(
        string cliente,
        List<string> produtos,
        double valorTotal)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            Cliente = cliente,
            Produtos = produtos,
            ValorTotal = valorTotal
        };
    }

    public void Update(string cliente, List<string> produtos, double valorTotal)
    {
        Cliente = cliente;
        Produtos = produtos;
        ValorTotal = valorTotal;
    }

    public override string ToString()
    {
        return $"Pedido(Id={Id}, Cliente={Cliente}, Itens={Produtos?.Count ?? 0}, Valor={ValorTotal})";
    }
}

public class LojaVirtual
{
    private readonly List<Pedido> Pedidos = [];

    [AuditLog]
    public void CadastrarPedido(Pedido pedido)
    {
        Pedidos.Add(pedido);
    }

    [AuditLog]
    public List<Pedido> ListarPedidos() => Pedidos;

    [AuditLog]
    public void AlterarPedido(Guid id, Pedido pedido)
    {
        var pedidoParaAtualizar = Pedidos.FirstOrDefault(x => x.Id == id) 
            ?? throw new ArgumentOutOfRangeException(nameof(id));

        pedidoParaAtualizar.Update(
            pedido.Cliente,
            pedido.Produtos,
            pedido.ValorTotal);
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
        var loja = new LojaVirtual();

        loja.CadastrarPedido(Pedido.CreateInstance(
            "190",
            [
                "sabonete",
            ],
            0.90
        ));

        var pedidos = loja.ListarPedidos();

        foreach (var pedido in pedidos)
        {
            Console.WriteLine(pedido.ToString());
        }
    }
}