using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGE.API.Services.Reports;
using SGE.Domain.Interfaces;

namespace SGE.API.Controllers
{
    /// <summary>
    /// Controller para geração de relatórios em PDF usando QuestPDF
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RelatoriosPdfController : ApiControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IEstoqueRepository _estoqueRepository;
        private readonly IProdutoRepository _produtoRepository;

        public RelatoriosPdfController(
            IReportService reportService,
            IEstoqueRepository estoqueRepository,
            IProdutoRepository produtoRepository)
        {
            _reportService = reportService;
            _estoqueRepository = estoqueRepository;
            _produtoRepository = produtoRepository;
        }

        /// <summary>
        /// Gera relatório de estoque em PDF
        /// </summary>
        /// <returns>Arquivo PDF com relatório de estoque</returns>
        [HttpGet("estoque/pdf")]
        public async Task<IActionResult> GerarRelatorioEstoquePdf()
        {
            try
            {
                Console.WriteLine("🔄 Iniciando geração de relatório de estoque...");
                
                // Buscar dados do banco com validação
                var itensEstoque = await _estoqueRepository.GetAllAsync();
                Console.WriteLine($"📊 Itens encontrados: {itensEstoque?.Count() ?? 0}");
                
                if (itensEstoque == null)
                {
                    Console.WriteLine("❌ Nenhum item de estoque encontrado");
                    return BadRequest(new { message = "Não foi possível acessar os dados de estoque" });
                }

                if (!itensEstoque.Any())
                {
                    Console.WriteLine("⚠️ Lista de estoque está vazia");
                    // Se não há dados, criar um relatório vazio
                    var itensVazio = new List<RelatorioEstoqueItem>
                    {
                        new RelatorioEstoqueItem
                        {
                            NomeProduto = "Nenhum produto encontrado",
                            CodigoProduto = "N/A",
                            QuantidadeAtual = 0,
                            EstoqueMinimo = 0,
                            PrecoUnitario = 0,
                            CategoriaNome = "N/A"
                        }
                    };
                    
                    var pdfBytesVazio = _reportService.GerarRelatorioEstoque(itensVazio);
                    return File(pdfBytesVazio, "application/pdf", $"RelatorioEstoque_Vazio_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
                }
                
                var relatorioItens = itensEstoque.Select(item => new RelatorioEstoqueItem
                {
                    NomeProduto = item.Produto?.Nome ?? "Produto não encontrado",
                    CodigoProduto = item.Produto?.Codigo ?? "N/A",
                    QuantidadeAtual = item.Quantidade,
                    EstoqueMinimo = item.Produto?.EstoqueMinimo ?? 0,
                    PrecoUnitario = item.Produto?.Preco ?? 0,
                    CategoriaNome = item.Produto?.Categoria?.Nome
                }).ToList();

                // Gerar PDF
                var pdfBytes = _reportService.GerarRelatorioEstoque(relatorioItens);

                var fileName = $"RelatorioEstoque_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                // Log detalhado do erro
                Console.WriteLine($"❌ Erro ao gerar PDF de estoque: {ex}");
                
                return BadRequest(new { 
                    message = "Erro ao gerar relatório de estoque", 
                    error = ex.Message,
                    stackTrace = ex.StackTrace?.Substring(0, Math.Min(500, ex.StackTrace.Length)),
                    timestamp = DateTime.Now
                });
            }
        }

        /// <summary>
        /// Endpoint de teste para verificar se a API está respondendo
        /// </summary>
        [AllowAnonymous]
        [HttpGet("test")]
        public IActionResult TestarConexao()
        {
            return Ok(new { 
                message = "API de Relatórios PDF está funcionando!", 
                timestamp = DateTime.Now,
                questPdfVersion = "OK",
                serverStatus = "Running"
            });
        }

        /// <summary>
        /// Endpoint de debug para verificar dados do estoque
        /// </summary>
        [HttpGet("debug/estoque")]
        public async Task<IActionResult> DebugEstoque()
        {
            try
            {
                var itensEstoque = await _estoqueRepository.GetAllAsync();
                
                return Ok(new {
                    message = "Debug do estoque",
                    totalItens = itensEstoque?.Count() ?? 0,
                    itens = itensEstoque?.Take(3).Select(item => new {
                        id = item.Id,
                        quantidade = item.Quantidade,
                        produtoNome = item.Produto?.Nome ?? "N/A",
                        produtoId = item.ProdutoId
                    }),
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new {
                    message = "Erro ao acessar dados de estoque",
                    error = ex.Message,
                    timestamp = DateTime.Now
                });
            }
        }

        /// <summary>
        /// Endpoint de teste de PDF simples (sem dados do banco)
        /// </summary>
        [HttpGet("test/pdf")]
        public IActionResult TestarPdfSimples()
        {
            try
            {
                // Criar dados fake para teste
                var itensFake = new List<RelatorioEstoqueItem>
                {
                    new RelatorioEstoqueItem
                    {
                        NomeProduto = "Produto Teste 1",
                        CodigoProduto = "TEST001",
                        QuantidadeAtual = 10,
                        EstoqueMinimo = 5,
                        PrecoUnitario = 25.50m,
                        CategoriaNome = "Categoria Teste"
                    },
                    new RelatorioEstoqueItem
                    {
                        NomeProduto = "Produto Teste 2", 
                        CodigoProduto = "TEST002",
                        QuantidadeAtual = 3,
                        EstoqueMinimo = 10,
                        PrecoUnitario = 15.75m,
                        CategoriaNome = "Categoria Teste"
                    }
                };

                // Gerar PDF com dados fake
                var pdfBytes = _reportService.GerarRelatorioEstoque(itensFake);
                var fileName = $"TestePDF_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    message = "Erro ao gerar PDF de teste", 
                    error = ex.Message,
                    stackTrace = ex.StackTrace?.Substring(0, Math.Min(200, ex.StackTrace.Length)),
                    timestamp = DateTime.Now
                });
            }
        }

        /// <summary>
        /// Gera relatório de produtos em PDF
        /// </summary>
        /// <param name="ativos">Filtrar apenas produtos ativos</param>
        /// <returns>Arquivo PDF com relatório de produtos</returns>
        [HttpGet("produtos/pdf")]
        public async Task<IActionResult> GerarRelatorioProdutosPdf([FromQuery] bool? ativos = null)
        {
            try
            {
                // Buscar dados do banco
                var produtos = ativos.HasValue && ativos.Value 
                    ? await _produtoRepository.GetActiveAsync()
                    : await _produtoRepository.GetAllAsync();

                var relatorioItens = produtos.Select(produto => new RelatorioProdutoItem
                {
                    Nome = produto.Nome,
                    Codigo = produto.Codigo,
                    CategoriaNome = produto.Categoria?.Nome,
                    PrecoCompra = produto.Preco, // Usando apenas 'Preco' como base
                    PrecoVenda = produto.Preco * 1.30m, // Simular margem de 30%
                    Ativo = produto.Ativo
                }).ToList();

                // Gerar PDF
                var pdfBytes = _reportService.GerarRelatorioProdutos(relatorioItens);

                var fileName = $"RelatorioProdutos_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao gerar relatório de produtos", error = ex.Message });
            }
        }

        /// <summary>
        /// Gera relatório de movimentações em PDF
        /// </summary>
        /// <param name="dataInicio">Data início do filtro</param>
        /// <param name="dataFim">Data fim do filtro</param>
        /// <param name="produtoId">ID do produto para filtrar</param>
        /// <returns>Arquivo PDF com relatório de movimentações</returns>
        [HttpGet("movimentacoes/pdf")]
        public async Task<IActionResult> GerarRelatorioMovimentacoesPdf(
            [FromQuery] DateTime? dataInicio = null,
            [FromQuery] DateTime? dataFim = null,
            [FromQuery] Guid? produtoId = null)
        {
            try
            {
                // Definir período padrão se não informado
                var dataFimFinal = dataFim ?? DateTime.Now;
                var dataInicioFinal = dataInicio ?? dataFimFinal.AddDays(-30);

                // Buscar dados do banco
                var movimentacoes = await _estoqueRepository.GetMovimentacoesPorPeriodoAsync(
                    dataInicioFinal, 
                    dataFimFinal, 
                    produtoId);

                var relatorioItens = movimentacoes.Select(mov => new RelatorioMovimentacaoItem
                {
                    DataMovimento = mov.DataMovimento,
                    NomeProduto = mov.EstoqueItem?.Produto?.Nome ?? "Produto não encontrado",
                    TipoMovimento = mov.TipoMovimento switch
                    {
                        SGE.Domain.Enums.TipoMovimento.Entrada => "Entrada",
                        SGE.Domain.Enums.TipoMovimento.Saida => "Saída",
                        SGE.Domain.Enums.TipoMovimento.Ajuste => "Ajuste",
                        SGE.Domain.Enums.TipoMovimento.Transferencia => "Transferência",
                        _ => mov.TipoMovimento.ToString()
                    },
                    Quantidade = mov.Quantidade,
                    Observacoes = mov.Observacoes ?? ""
                }).ToList();

                // Gerar PDF
                var pdfBytes = _reportService.GerarRelatorioMovimentacoes(relatorioItens);

                var fileName = $"RelatorioMovimentacoes_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao gerar relatório de movimentações", error = ex.Message });
            }
        }


    }
}