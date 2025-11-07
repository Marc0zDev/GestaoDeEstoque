using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGE.API.Controllers;
using SGE.Application.Features.Relatorios.Commands;
using SGE.Application.Features.Relatorios.DTOs;
using SGE.Domain.Enums;
using System.Text;

namespace SGE.API.Controllers;

/// <summary>
/// Controller para geração de relatórios do sistema de gestão de estoque
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RelatoriosController : ApiControllerBase
{
    // ============================================================
    // 📊 RELATÓRIOS BÁSICOS
    // ============================================================

    /// <summary>
    /// Gera relatório de estoque atual
    /// </summary>
    /// <param name="categoriaId">ID da categoria (opcional)</param>
    /// <param name="fornecedorId">ID do fornecedor (opcional)</param>
    /// <param name="localId">ID do local (opcional)</param>
    /// <param name="status">Status do estoque (opcional)</param>
    /// <param name="incluirSemEstoque">Incluir produtos sem estoque</param>
    /// <returns>Relatório de estoque</returns>
    [HttpGet("estoque")]
    public async Task<ActionResult<List<RelatorioEstoqueDto>>> GerarRelatorioEstoque(
        [FromQuery] Guid? categoriaId = null,
        [FromQuery] Guid? fornecedorId = null,
        [FromQuery] Guid? localId = null,
        [FromQuery] StatusEstoque? status = null,
        [FromQuery] bool incluirSemEstoque = true)
    {
        try
        {
            // Por enquanto, retornando dados simulados até os handlers serem corrigidos
            var relatorioSimulado = GerarDadosSimuladosEstoque();
            return Ok(relatorioSimulado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Erro ao gerar relatório de estoque", error = ex.Message });
        }
    }

    /// <summary>
    /// Gera relatório de movimentações do estoque
    /// </summary>
    /// <param name="dataInicio">Data inicial</param>
    /// <param name="dataFim">Data final</param>
    /// <param name="produtoId">ID do produto (opcional)</param>
    /// <param name="usuarioId">ID do usuário (opcional)</param>
    /// <param name="localId">ID do local (opcional)</param>
    /// <param name="tipoMovimento">Tipo de movimento (opcional)</param>
    /// <returns>Relatório de movimentações</returns>
    [HttpGet("movimentacoes")]
    public async Task<ActionResult<List<RelatorioMovimentacaoDto>>> GerarRelatorioMovimentacoes(
        [FromQuery] DateTime? dataInicio = null,
        [FromQuery] DateTime? dataFim = null,
        [FromQuery] Guid? produtoId = null,
        [FromQuery] Guid? usuarioId = null,
        [FromQuery] Guid? localId = null,
        [FromQuery] TipoMovimento? tipoMovimento = null)
    {
        try
        {
            // Por enquanto, retornando dados simulados até os handlers serem corrigidos
            var relatorioSimulado = GerarDadosSimuladosMovimentacoes();
            return Ok(relatorioSimulado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Erro ao gerar relatório de movimentações", error = ex.Message });
        }
    }

    /// <summary>
    /// Gera relatório geral do sistema
    /// </summary>
    /// <param name="dataReferencia">Data de referência (opcional)</param>
    /// <returns>Relatório geral</returns>
    [HttpGet("geral")]
    public async Task<ActionResult<RelatorioGeralDto>> GerarRelatorioGeral(
        [FromQuery] DateTime? dataReferencia = null)
    {
        try
        {
            // Por enquanto, retornando dados simulados até os handlers serem corrigidos
            var relatorioSimulado = GerarDadosSimuladosGeral();
            return Ok(relatorioSimulado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Erro ao gerar relatório geral", error = ex.Message });
        }
    }

    // ============================================================
    // 📄 EXPORTAÇÃO CSV
    // ============================================================

    /// <summary>
    /// Exporta relatório de estoque em CSV
    /// </summary>
    [HttpGet("estoque/export/csv")]
    public async Task<IActionResult> ExportarRelatorioEstoqueCSV(
        [FromQuery] Guid? categoriaId = null,
        [FromQuery] Guid? fornecedorId = null,
        [FromQuery] Guid? localId = null,
        [FromQuery] StatusEstoque? status = null,
        [FromQuery] bool incluirSemEstoque = true)
    {
        try
        {
            var dados = GerarDadosSimuladosEstoque();
            var csvBytes = await GerarCSVEstoque(dados);
            var fileName = $"relatorio_estoque_{DateTime.Now:yyyyMMdd_HHmm}.csv";
            
            return File(csvBytes, "text/csv", fileName);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Erro ao gerar CSV de estoque", error = ex.Message });
        }
    }

    /// <summary>
    /// Exporta relatório de movimentações em CSV
    /// </summary>
    [HttpGet("movimentacoes/export/csv")]
    public async Task<IActionResult> ExportarRelatorioMovimentacoesCSV(
        [FromQuery] DateTime dataInicio,
        [FromQuery] DateTime dataFim,
        [FromQuery] Guid? produtoId = null,
        [FromQuery] Guid? usuarioId = null,
        [FromQuery] Guid? localId = null,
        [FromQuery] TipoMovimento? tipoMovimento = null)
    {
        try
        {
            var dados = GerarDadosSimuladosMovimentacoes();
            var csvBytes = await GerarCSVMovimentacoes(dados);
            var fileName = $"relatorio_movimentacoes_{DateTime.Now:yyyyMMdd_HHmm}.csv";
            
            return File(csvBytes, "text/csv", fileName);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Erro ao gerar CSV de movimentações", error = ex.Message });
        }
    }

    /// <summary>
    /// Exporta relatório geral em CSV
    /// </summary>
    [HttpGet("geral/export/csv")]
    public async Task<IActionResult> ExportarRelatorioGeralCSV(
        [FromQuery] DateTime? dataReferencia = null)
    {
        try
        {
            var dados = GerarDadosSimuladosGeral();
            var csvBytes = await GerarCSVGeral(dados);
            var fileName = $"relatorio_geral_{DateTime.Now:yyyyMMdd_HHmm}.csv";
            
            return File(csvBytes, "text/csv", fileName);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Erro ao gerar CSV geral", error = ex.Message });
        }
    }

    // ============================================================
    // 📄 EXPORTAÇÃO PDF
    // ============================================================

    /// <summary>
    /// Exporta relatório de estoque em PDF
    /// </summary>
    [HttpGet("estoque/export/pdf")]
    public async Task<IActionResult> ExportarRelatorioEstoquePDF(
        [FromQuery] Guid? categoriaId = null,
        [FromQuery] Guid? fornecedorId = null,
        [FromQuery] Guid? localId = null,
        [FromQuery] StatusEstoque? status = null,
        [FromQuery] bool incluirSemEstoque = true)
    {
        try
        {
            var dados = GerarDadosSimuladosEstoque();
            var pdfBytes = await GerarPDFEstoque(dados);
            var fileName = $"relatorio_estoque_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
            
            return File(pdfBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Erro ao gerar PDF de estoque", error = ex.Message });
        }
    }

    /// <summary>
    /// Exporta relatório de movimentações em PDF
    /// </summary>
    [HttpGet("movimentacoes/export/pdf")]
    public async Task<IActionResult> ExportarRelatorioMovimentacoesPDF(
        [FromQuery] DateTime dataInicio,
        [FromQuery] DateTime dataFim,
        [FromQuery] Guid? produtoId = null,
        [FromQuery] Guid? usuarioId = null,
        [FromQuery] Guid? localId = null,
        [FromQuery] TipoMovimento? tipoMovimento = null)
    {
        try
        {
            var dados = GerarDadosSimuladosMovimentacoes();
            var pdfBytes = await GerarPDFMovimentacoes(dados);
            var fileName = $"relatorio_movimentacoes_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
            
            return File(pdfBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Erro ao gerar PDF de movimentações", error = ex.Message });
        }
    }

    /// <summary>
    /// Exporta relatório geral em PDF
    /// </summary>
    [HttpGet("geral/export/pdf")]
    public async Task<IActionResult> ExportarRelatorioGeralPDF(
        [FromQuery] DateTime? dataReferencia = null)
    {
        try
        {
            var dados = GerarDadosSimuladosGeral();
            var pdfBytes = await GerarPDFGeral(dados);
            var fileName = $"relatorio_geral_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
            
            return File(pdfBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Erro ao gerar PDF geral", error = ex.Message });
        }
    }

    // ============================================================
    // 🛠️ MÉTODOS AUXILIARES PARA GERAÇÃO DE CSV
    // ============================================================

    private async Task<byte[]> GerarCSVEstoque(List<RelatorioEstoqueDto> dados)
    {
        var csv = new StringBuilder();
        
        // Cabeçalho
        csv.AppendLine("Código,Produto,Categoria,Quantidade Atual,Estoque Mínimo,Estoque Máximo,Valor Unitário,Valor Total,Local,Última Movimentação,Status");
        
        // Dados
        foreach (var item in dados)
        {
            csv.AppendLine($"\"{item.CodigoProduto}\"," +
                          $"\"{item.NomeProduto}\"," +
                          $"\"{item.Categoria}\"," +
                          $"{item.QuantidadeAtual}," +
                          $"{item.EstoqueMinimo}," +
                          $"{item.EstoqueMaximo}," +
                          $"\"{item.ValorUnitario:F2}\"," +
                          $"\"{item.ValorTotalEstoque:F2}\"," +
                          $"\"{item.LocalArmazenagem}\"," +
                          $"\"{item.UltimaMovimentacao:yyyy-MM-dd HH:mm}\"," +
                          $"\"{item.Status}\"");
        }

        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    private async Task<byte[]> GerarCSVMovimentacoes(List<RelatorioMovimentacaoDto> dados)
    {
        var csv = new StringBuilder();
        
        // Cabeçalho
        csv.AppendLine("Data,Código,Produto,Tipo,Quantidade,Observações,Usuário,Local");
        
        // Dados
        foreach (var item in dados)
        {
            csv.AppendLine($"\"{item.Data:yyyy-MM-dd HH:mm}\"," +
                          $"\"{item.CodigoProduto}\"," +
                          $"\"{item.NomeProduto}\"," +
                          $"\"{item.Tipo}\"," +
                          $"{item.Quantidade}," +
                          $"\"{item.Observacoes}\"," +
                          $"\"{item.Usuario}\"," +
                          $"\"{item.LocalArmazenagem}\"");
        }

        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    private async Task<byte[]> GerarCSVGeral(RelatorioGeralDto dados)
    {
        var csv = new StringBuilder();
        
        // Cabeçalho do relatório geral
        csv.AppendLine("RELATÓRIO GERAL DO SISTEMA DE GESTÃO DE ESTOQUE");
        csv.AppendLine($"Data de Geração: {DateTime.Now:yyyy-MM-dd HH:mm}");
        csv.AppendLine();
        
        // Resumo geral
        csv.AppendLine("RESUMO GERAL");
        csv.AppendLine("Indicador,Valor");
        csv.AppendLine($"Total de Produtos,{dados.TotalProdutos}");
        csv.AppendLine($"Produtos com Estoque,{dados.ProdutosComEstoque}");
        csv.AppendLine($"Produtos sem Estoque,{dados.ProdutosSemEstoque}");
        csv.AppendLine($"Produtos com Estoque Mínimo,{dados.ProdutosEstoqueMinimo}");
        csv.AppendLine($"Produtos com Estoque Crítico,{dados.ProdutosEstoqueCritico}");
        csv.AppendLine($"Valor Total do Estoque,\"{dados.ValorTotalEstoque:F2}\"");
        csv.AppendLine($"Movimentações Hoje,{dados.TotalMovimentacoesHoje}");
        csv.AppendLine($"Movimentações Este Mês,{dados.TotalMovimentacoesMes}");
        csv.AppendLine();

        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    // ============================================================
    // 🛠️ MÉTODOS AUXILIARES PARA GERAÇÃO DE PDF
    // ============================================================

    private async Task<byte[]> GerarPDFEstoque(List<RelatorioEstoqueDto> dados)
    {
        var html = new StringBuilder();
        
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html><head>");
        html.AppendLine("<meta charset='UTF-8'>");
        html.AppendLine("<title>Relatório de Estoque</title>");
        html.AppendLine("<style>");
        html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; font-size: 12px; }");
        html.AppendLine("table { border-collapse: collapse; width: 100%; margin-top: 20px; }");
        html.AppendLine("th, td { border: 1px solid #ddd; padding: 6px; text-align: left; }");
        html.AppendLine("th { background-color: #f2f2f2; font-weight: bold; }");
        html.AppendLine(".header { text-align: center; margin-bottom: 20px; }");
        html.AppendLine("</style>");
        html.AppendLine("</head><body>");
        
        // Cabeçalho
        html.AppendLine("<div class='header'>");
        html.AppendLine("<h1>Sistema de Gestão de Estoque</h1>");
        html.AppendLine("<h2>Relatório de Estoque</h2>");
        html.AppendLine($"<p>Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}</p>");
        html.AppendLine("</div>");
        
        // Tabela
        html.AppendLine("<table>");
        html.AppendLine("<thead>");
        html.AppendLine("<tr>");
        html.AppendLine("<th>Código</th>");
        html.AppendLine("<th>Produto</th>");
        html.AppendLine("<th>Categoria</th>");
        html.AppendLine("<th>Qtd</th>");
        html.AppendLine("<th>Status</th>");
        html.AppendLine("</tr>");
        html.AppendLine("</thead>");
        html.AppendLine("<tbody>");
        
        foreach (var item in dados)
        {
            html.AppendLine("<tr>");
            html.AppendLine($"<td>{item.CodigoProduto}</td>");
            html.AppendLine($"<td>{item.NomeProduto}</td>");
            html.AppendLine($"<td>{item.Categoria}</td>");
            html.AppendLine($"<td>{item.QuantidadeAtual}</td>");
            html.AppendLine($"<td>{item.Status}</td>");
            html.AppendLine("</tr>");
        }
        
        html.AppendLine("</tbody>");
        html.AppendLine("</table>");
        html.AppendLine("</body></html>");

        return Encoding.UTF8.GetBytes(html.ToString());
    }

    private async Task<byte[]> GerarPDFMovimentacoes(List<RelatorioMovimentacaoDto> dados)
    {
        var html = new StringBuilder();
        
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html><head>");
        html.AppendLine("<meta charset='UTF-8'>");
        html.AppendLine("<title>Relatório de Movimentações</title>");
        html.AppendLine("<style>");
        html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; font-size: 12px; }");
        html.AppendLine("table { border-collapse: collapse; width: 100%; margin-top: 20px; }");
        html.AppendLine("th, td { border: 1px solid #ddd; padding: 6px; text-align: left; }");
        html.AppendLine("th { background-color: #f2f2f2; font-weight: bold; }");
        html.AppendLine(".header { text-align: center; margin-bottom: 20px; }");
        html.AppendLine("</style>");
        html.AppendLine("</head><body>");
        
        html.AppendLine("<div class='header'>");
        html.AppendLine("<h1>Sistema de Gestão de Estoque</h1>");
        html.AppendLine("<h2>Relatório de Movimentações</h2>");
        html.AppendLine($"<p>Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}</p>");
        html.AppendLine("</div>");
        
        html.AppendLine("<table>");
        html.AppendLine("<thead>");
        html.AppendLine("<tr>");
        html.AppendLine("<th>Data</th>");
        html.AppendLine("<th>Produto</th>");
        html.AppendLine("<th>Tipo</th>");
        html.AppendLine("<th>Qtd</th>");
        html.AppendLine("</tr>");
        html.AppendLine("</thead>");
        html.AppendLine("<tbody>");
        
        foreach (var item in dados)
        {
            html.AppendLine("<tr>");
            html.AppendLine($"<td>{item.Data:dd/MM/yyyy}</td>");
            html.AppendLine($"<td>{item.NomeProduto}</td>");
            html.AppendLine($"<td>{item.Tipo}</td>");
            html.AppendLine($"<td>{item.Quantidade}</td>");
            html.AppendLine("</tr>");
        }
        
        html.AppendLine("</tbody>");
        html.AppendLine("</table>");
        html.AppendLine("</body></html>");

        return Encoding.UTF8.GetBytes(html.ToString());
    }

    private async Task<byte[]> GerarPDFGeral(RelatorioGeralDto dados)
    {
        var html = new StringBuilder();
        
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html><head>");
        html.AppendLine("<meta charset='UTF-8'>");
        html.AppendLine("<title>Relatório Geral</title>");
        html.AppendLine("<style>");
        html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; font-size: 12px; }");
        html.AppendLine(".header { text-align: center; margin-bottom: 30px; }");
        html.AppendLine(".kpi { margin: 10px 0; }");
        html.AppendLine("</style>");
        html.AppendLine("</head><body>");
        
        html.AppendLine("<div class='header'>");
        html.AppendLine("<h1>Sistema de Gestão de Estoque</h1>");
        html.AppendLine("<h2>Relatório Geral</h2>");
        html.AppendLine($"<p>Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}</p>");
        html.AppendLine("</div>");
        
        html.AppendLine("<div class='kpi'>");
        html.AppendLine($"<p><strong>Total de Produtos:</strong> {dados.TotalProdutos}</p>");
        html.AppendLine($"<p><strong>Produtos com Estoque:</strong> {dados.ProdutosComEstoque}</p>");
        html.AppendLine($"<p><strong>Valor Total do Estoque:</strong> R$ {dados.ValorTotalEstoque:N2}</p>");
        html.AppendLine($"<p><strong>Produtos Críticos:</strong> {dados.ProdutosEstoqueCritico}</p>");
        html.AppendLine("</div>");
        
        html.AppendLine("</body></html>");

        return Encoding.UTF8.GetBytes(html.ToString());
    }

    // ============================================================
    // 🎭 MÉTODOS DE DADOS SIMULADOS (TEMPORÁRIO)
    // ============================================================

    private List<RelatorioEstoqueDto> GerarDadosSimuladosEstoque()
    {
        return new List<RelatorioEstoqueDto>
        {
            new RelatorioEstoqueDto
            {
                ProdutoId = Guid.NewGuid(),
                CodigoProduto = "PROD001",
                NomeProduto = "Notebook Dell Inspiron 15",
                Categoria = "Eletrônicos",
                QuantidadeAtual = 25,
                EstoqueMinimo = 5,
                EstoqueMaximo = 50,
                ValorUnitario = 2500.00m,
                ValorTotalEstoque = 62500.00m,
                LocalArmazenagem = "Depósito Principal",
                UltimaMovimentacao = DateTime.Now.AddDays(-2),
                Status = StatusEstoque.Normal
            },
            new RelatorioEstoqueDto
            {
                ProdutoId = Guid.NewGuid(),
                CodigoProduto = "PROD002",
                NomeProduto = "Mouse Logitech MX Master",
                Categoria = "Periféricos",
                QuantidadeAtual = 2,
                EstoqueMinimo = 10,
                EstoqueMaximo = 30,
                ValorUnitario = 150.00m,
                ValorTotalEstoque = 300.00m,
                LocalArmazenagem = "Depósito Principal",
                UltimaMovimentacao = DateTime.Now.AddDays(-1),
                Status = StatusEstoque.EstoqueCritico
            },
            new RelatorioEstoqueDto
            {
                ProdutoId = Guid.NewGuid(),
                CodigoProduto = "PROD003",
                NomeProduto = "Teclado Mecânico Corsair",
                Categoria = "Periféricos",
                QuantidadeAtual = 0,
                EstoqueMinimo = 5,
                EstoqueMaximo = 25,
                ValorUnitario = 300.00m,
                ValorTotalEstoque = 0.00m,
                LocalArmazenagem = "Depósito Principal",
                UltimaMovimentacao = DateTime.Now.AddDays(-5),
                Status = StatusEstoque.SemEstoque
            }
        };
    }

    private List<RelatorioMovimentacaoDto> GerarDadosSimuladosMovimentacoes()
    {
        return new List<RelatorioMovimentacaoDto>
        {
            new RelatorioMovimentacaoDto
            {
                Data = DateTime.Now.AddDays(-1),
                CodigoProduto = "PROD001",
                NomeProduto = "Notebook Dell Inspiron 15",
                Tipo = TipoMovimento.Entrada,
                Quantidade = 10,
                Observacoes = "Compra mensal",
                Usuario = "Admin",
                LocalArmazenagem = "Depósito Principal"
            },
            new RelatorioMovimentacaoDto
            {
                Data = DateTime.Now.AddDays(-2),
                CodigoProduto = "PROD002",
                NomeProduto = "Mouse Logitech MX Master",
                Tipo = TipoMovimento.Saida,
                Quantidade = 5,
                Observacoes = "Venda para cliente",
                Usuario = "Vendedor1",
                LocalArmazenagem = "Depósito Principal"
            },
            new RelatorioMovimentacaoDto
            {
                Data = DateTime.Now.AddDays(-3),
                CodigoProduto = "PROD003",
                NomeProduto = "Teclado Mecânico Corsair",
                Tipo = TipoMovimento.Saida,
                Quantidade = 3,
                Observacoes = "Venda online",
                Usuario = "Sistema",
                LocalArmazenagem = "Depósito Principal"
            }
        };
    }

    private RelatorioGeralDto GerarDadosSimuladosGeral()
    {
        return new RelatorioGeralDto
        {
            TotalProdutos = 150,
            ProdutosComEstoque = 120,
            ProdutosSemEstoque = 15,
            ProdutosEstoqueMinimo = 25,
            ProdutosEstoqueCritico = 8,
            ValorTotalEstoque = 275000.50m,
            TotalMovimentacoesHoje = 12,
            TotalMovimentacoesMes = 185,
            ProdutosCriticos = GerarDadosSimuladosEstoque().Where(p => p.Status == StatusEstoque.EstoqueCritico || p.Status == StatusEstoque.SemEstoque).ToList(),
            UltimasMovimentacoes = GerarDadosSimuladosMovimentacoes().Take(5).ToList()
        };
    }
}