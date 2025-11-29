using AtividadeLabIV.Aspects.Audit;
using AtividadeLabIV.Aspects.Auth;
using AtividadeLabIV.Aspects.Exceptions;
using AtividadeLabIV.Aspects.Notify;

namespace AtividadeLabIV;

public interface IComponenteAutenticado
{
    public Guid? IdSessao { get; set; }

    Guid Autenticar();
}

public class Pedido
{
    public Guid Id { get; private set; }
    public string Cliente { get; private set; } = default!;
    public List<string> Produtos { get; private set; } = [];
    public double ValorTotal { get; private set; }

    private Pedido()
    {}

    [LogAuditoria]
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

public class LojaVirtual : IComponenteAutenticado
{
    private Guid? _idSessao = null;
    private readonly List<Pedido> Pedidos = [];

    public Guid? IdSessao { get => _idSessao; set => _idSessao = value; }

    [LogAuditoria]
    public List<Pedido> ListarPedidos() => Pedidos;

    [Autenticado, LogAuditoria, Notificacao]
    public void CadastrarPedido(Pedido pedido) => Pedidos.Add(pedido);

    [Autenticado, LogAuditoria, HandleException]
    public void RemoverPedido(Guid Id)
    {
        var idx = Pedidos.FindIndex(x => x.Id == Id);

        if (idx > 0)
        {
            Pedidos.RemoveAt(idx);
            return;
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(Id));
        }
    }

    [Autenticado, LogAuditoria, HandleException, Notificacao]
    public void AlterarPedido(Guid id, Pedido pedido)
    {
        var pedidoParaAtualizar = Pedidos.FirstOrDefault(x => x.Id == id) 
            ?? throw new ArgumentOutOfRangeException(nameof(id));

        pedidoParaAtualizar.Update(
            pedido.Cliente,
            pedido.Produtos,
            pedido.ValorTotal);
    }

    public Guid Autenticar()
    {
        var id = Guid.NewGuid();
        IdSessao = id;
        return id;
    }
}

internal class Program
{
    private static void LogPedidos(List<Pedido> pedidos)
    {
        Console.WriteLine("[MAIN] LogPedidos");

        foreach (var pedido in pedidos)
        {
            Console.WriteLine(pedido.ToString());
        }
    }

    private static void Main(string[] args)
    {
        var loja = new LojaVirtual();

        var pedidoTeste = Pedido.CreateInstance(
            "190",
            [
                "sabonete",
            ],
            0.90
        );

        // testando seu autenticar
        loja.CadastrarPedido(pedidoTeste);

        loja.Autenticar();

        // testando autenticado 
        loja.CadastrarPedido(pedidoTeste);

        LogPedidos(loja.ListarPedidos());

        // testando auth 
        loja.AlterarPedido(Guid.NewGuid(), pedidoTeste);

        // testando exception handler 
        loja.AlterarPedido(Guid.NewGuid(), pedidoTeste);

        // caminho feliz
        loja.AlterarPedido(pedidoTeste.Id, pedidoTeste);

        // testando remover com pedido que nao existe 
        loja.RemoverPedido(Guid.NewGuid());

        // testando remover com pedido que existe
        loja.RemoverPedido(pedidoTeste.Id);

        LogPedidos(loja.ListarPedidos());
    }
}