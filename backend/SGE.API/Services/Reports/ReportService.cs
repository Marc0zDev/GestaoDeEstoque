using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace SGE.API.Services.Reports
{
    public interface IReportService
    {
        byte[] GerarRelatorioEstoque(List<RelatorioEstoqueItem> itens);
        byte[] GerarRelatorioProdutos(List<RelatorioProdutoItem> produtos);
        byte[] GerarRelatorioMovimentacoes(List<RelatorioMovimentacaoItem> movimentacoes);
    }

    public class ReportService : IReportService
    {
        public byte[] GerarRelatorioEstoque(List<RelatorioEstoqueItem> itens)
        {
            // Configurar QuestPDF
            QuestPDF.Settings.License = LicenseType.Community;
            
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Height(80)
                        .Background(Colors.Grey.Lighten3)
                        .Padding(20)
                        .Column(column =>
                        {
                            column.Item().Text("Sistema de Gestão de Estoque")
                                .FontSize(20)
                                .Bold()
                                .FontColor(Colors.Blue.Darken2);

                            column.Item().Text($"Relatório de Estoque - {DateTime.Now:dd/MM/yyyy HH:mm}")
                                .FontSize(12);
                        });

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(20);

                            // Resumo
                            column.Item().Background(Colors.Grey.Lighten4)
                                .Padding(15)
                                .Column(col =>
                                {
                                    col.Item().Text("Resumo do Estoque")
                                        .FontSize(16)
                                        .Bold()
                                        .FontColor(Colors.Blue.Darken1);

                                    col.Item().PaddingTop(10).Row(row =>
                                    {
                                        row.RelativeItem().Text($"Total de Produtos: {itens.Count}");
                                        row.RelativeItem().Text($"Valor Total: {itens.Sum(x => x.ValorTotal):C}");
                                        
                                        var produtosComEstoqueBaixo = itens.Count(x => x.QuantidadeAtual <= x.EstoqueMinimo);
                                        row.RelativeItem().Text($"Estoque Baixo: {produtosComEstoqueBaixo}")
                                            .FontColor(produtosComEstoqueBaixo > 0 ? Colors.Red.Darken1 : Colors.Green.Darken1);
                                    });
                                });

                            // Tabela de Itens
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(1);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("Produto").FontColor(Colors.White).Bold();
                                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("Código").FontColor(Colors.White).Bold();
                                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("Qtd Atual").FontColor(Colors.White).Bold();
                                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("Qtd Mín").FontColor(Colors.White).Bold();
                                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("Valor Unit.").FontColor(Colors.White).Bold();
                                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("Valor Total").FontColor(Colors.White).Bold();
                                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("Status").FontColor(Colors.White).Bold();
                                });

                                foreach (var item in itens)
                                {
                                    var isEstoqueBaixo = item.QuantidadeAtual <= item.EstoqueMinimo;
                                    var backgroundColor = isEstoqueBaixo ? Colors.Red.Lighten4 : Colors.White;

                                    table.Cell().Background(backgroundColor).Padding(8).Text(item.NomeProduto);
                                    table.Cell().Background(backgroundColor).Padding(8).Text(item.CodigoProduto);
                                    table.Cell().Background(backgroundColor).Padding(8).Text(item.QuantidadeAtual.ToString("N2")).AlignRight();
                                    table.Cell().Background(backgroundColor).Padding(8).Text(item.EstoqueMinimo.ToString("N2")).AlignRight();
                                    table.Cell().Background(backgroundColor).Padding(8).Text(item.PrecoUnitario.ToString("C")).AlignRight();
                                    table.Cell().Background(backgroundColor).Padding(8).Text(item.ValorTotal.ToString("C")).AlignRight();
                                    table.Cell().Background(backgroundColor).Padding(8)
                                        .Text(isEstoqueBaixo ? "BAIXO" : "OK")
                                        .FontColor(isEstoqueBaixo ? Colors.Red.Darken2 : Colors.Green.Darken2)
                                        .Bold();
                                }
                            });
                        });

                    page.Footer()
                        .Height(50)
                        .Background(Colors.Grey.Lighten3)
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Página ");
                            x.CurrentPageNumber();
                            x.Span(" de ");
                            x.TotalPages();
                            x.Span($" - Gerado em {DateTime.Now:dd/MM/yyyy HH:mm}");
                        });
                });
            });

            return document.GeneratePdf();
        }

        public byte[] GerarRelatorioProdutos(List<RelatorioProdutoItem> produtos)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Height(100)
                        .Background(Colors.Grey.Lighten3)
                        .Padding(20)
                        .Row(row =>
                        {
                            row.RelativeItem().Column(column =>
                            {
                                column.Item().Text("Sistema de Gestão de Estoque")
                                    .FontSize(20)
                                    .FontWeight(FontWeight.Bold)
                                    .FontColor(Colors.Green.Darken2);

                                column.Item().Text(text =>
                                {
                                    text.Span("Relatório de Produtos - ");
                                    text.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm"))
                                        .FontWeight(FontWeight.Medium);
                                });
                            });
                        });

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(20);

                            // Resumo
                            column.Item().Background(Colors.Grey.Lighten4)
                                .Padding(15)
                                .Column(col =>
                                {
                                    col.Item().Text("Resumo dos Produtos")
                                        .FontSize(16)
                                        .FontWeight(FontWeight.Bold)
                                        .FontColor(Colors.Green.Darken1);

                                    col.Item().PaddingTop(10).Row(row =>
                                    {
                                        row.RelativeItem().Text($"Total: {produtos.Count} produtos")
                                            .FontWeight(FontWeight.Medium);
                                        
                                        row.RelativeItem().Text($"Ativos: {produtos.Count(p => p.Ativo)}")
                                            .FontWeight(FontWeight.Medium);
                                        
                                        row.RelativeItem().Text($"Inativos: {produtos.Count(p => !p.Ativo)}")
                                            .FontWeight(FontWeight.Medium);
                                    });
                                });

                            // Tabela de Produtos
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3); // Nome
                                    columns.RelativeColumn(2); // Código
                                    columns.RelativeColumn(2); // Categoria
                                    columns.RelativeColumn(2); // Preço Compra
                                    columns.RelativeColumn(2); // Preço Venda
                                    columns.RelativeColumn(1); // Status
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Background(Colors.Green.Darken2).Text("Nome").FontColor(Colors.White);
                                    header.Cell().Element(CellStyle).Background(Colors.Green.Darken2).Text("Código").FontColor(Colors.White);
                                    header.Cell().Element(CellStyle).Background(Colors.Green.Darken2).Text("Categoria").FontColor(Colors.White);
                                    header.Cell().Element(CellStyle).Background(Colors.Green.Darken2).Text("Preço Compra").FontColor(Colors.White);
                                    header.Cell().Element(CellStyle).Background(Colors.Green.Darken2).Text("Preço Venda").FontColor(Colors.White);
                                    header.Cell().Element(CellStyle).Background(Colors.Green.Darken2).Text("Status").FontColor(Colors.White);

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container.DefaultTextStyle(x => x.FontWeight(FontWeight.Bold))
                                            .PaddingVertical(5)
                                            .PaddingHorizontal(10)
                                            .AlignCenter()
                                            .AlignMiddle();
                                    }
                                });

                                foreach (var produto in produtos)
                                {
                                    var backgroundColor = produto.Ativo ? Colors.White : Colors.Grey.Lighten4;

                                    table.Cell().Element(container => CellStyle(container, backgroundColor)).Text(produto.Nome);
                                    table.Cell().Element(container => CellStyle(container, backgroundColor)).Text(produto.Codigo);
                                    table.Cell().Element(container => CellStyle(container, backgroundColor)).Text(produto.CategoriaNome ?? "-");
                                    table.Cell().Element(container => CellStyle(container, backgroundColor)).Text(produto.PrecoCompra.ToString("C")).AlignRight();
                                    table.Cell().Element(container => CellStyle(container, backgroundColor)).Text(produto.PrecoVenda.ToString("C")).AlignRight();
                                    table.Cell().Element(container => CellStyle(container, backgroundColor))
                                        .Text(produto.Ativo ? "ATIVO" : "INATIVO")
                                        .FontColor(produto.Ativo ? Colors.Green.Darken2 : Colors.Red.Darken2)
                                        .FontWeight(FontWeight.Bold);

                                    static IContainer CellStyle(IContainer container, string backgroundColor)
                                    {
                                        return container.Background(backgroundColor)
                                            .BorderBottom(1)
                                            .BorderColor(Colors.Grey.Lighten2)
                                            .PaddingVertical(8)
                                            .PaddingHorizontal(10)
                                            .AlignMiddle();
                                    }
                                }
                            });
                        });

                    page.Footer()
                        .Height(50)
                        .Background(Colors.Grey.Lighten3)
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Página ");
                            x.CurrentPageNumber();
                            x.Span(" de ");
                            x.TotalPages();
                            x.Span(" - Gerado em ");
                            x.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                        });
                });
            });

            return document.GeneratePdf();
        }

        public byte[] GerarRelatorioMovimentacoes(List<RelatorioMovimentacaoItem> movimentacoes)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Height(100)
                        .Background(Colors.Grey.Lighten3)
                        .Padding(20)
                        .Row(row =>
                        {
                            row.RelativeItem().Column(column =>
                            {
                                column.Item().Text("Sistema de Gestão de Estoque")
                                    .FontSize(20)
                                    .FontWeight(FontWeight.Bold)
                                    .FontColor(Colors.Orange.Darken2);

                                column.Item().Text(text =>
                                {
                                    text.Span("Relatório de Movimentações - ");
                                    text.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm"))
                                        .FontWeight(FontWeight.Medium);
                                });
                            });
                        });

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(20);

                            // Resumo
                            column.Item().Background(Colors.Grey.Lighten4)
                                .Padding(15)
                                .Column(col =>
                                {
                                    col.Item().Text("Resumo das Movimentações")
                                        .FontSize(16)
                                        .FontWeight(FontWeight.Bold)
                                        .FontColor(Colors.Orange.Darken1);

                                    col.Item().PaddingTop(10).Row(row =>
                                    {
                                        row.RelativeItem().Text($"Total: {movimentacoes.Count} movimentações")
                                            .FontWeight(FontWeight.Medium);
                                        
                                        var entradas = movimentacoes.Count(m => m.TipoMovimento == "Entrada");
                                        var saidas = movimentacoes.Count(m => m.TipoMovimento == "Saída");
                                        
                                        row.RelativeItem().Text($"Entradas: {entradas}")
                                            .FontWeight(FontWeight.Medium)
                                            .FontColor(Colors.Green.Darken1);
                                        
                                        row.RelativeItem().Text($"Saídas: {saidas}")
                                            .FontWeight(FontWeight.Medium)
                                            .FontColor(Colors.Red.Darken1);
                                    });
                                });

                            // Tabela de Movimentações
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2); // Data
                                    columns.RelativeColumn(3); // Produto
                                    columns.RelativeColumn(1); // Tipo
                                    columns.RelativeColumn(1); // Quantidade
                                    columns.RelativeColumn(2); // Observações
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Background(Colors.Orange.Darken2).Text("Data").FontColor(Colors.White);
                                    header.Cell().Element(CellStyle).Background(Colors.Orange.Darken2).Text("Produto").FontColor(Colors.White);
                                    header.Cell().Element(CellStyle).Background(Colors.Orange.Darken2).Text("Tipo").FontColor(Colors.White);
                                    header.Cell().Element(CellStyle).Background(Colors.Orange.Darken2).Text("Quantidade").FontColor(Colors.White);
                                    header.Cell().Element(CellStyle).Background(Colors.Orange.Darken2).Text("Observações").FontColor(Colors.White);

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container.DefaultTextStyle(x => x.FontWeight(FontWeight.Bold))
                                            .PaddingVertical(5)
                                            .PaddingHorizontal(10)
                                            .AlignCenter()
                                            .AlignMiddle();
                                    }
                                });

                                foreach (var movimento in movimentacoes.OrderByDescending(m => m.DataMovimento))
                                {
                                    var isEntrada = movimento.TipoMovimento == "Entrada";

                                    table.Cell().Element(CellStyle).Text(movimento.DataMovimento.ToString("dd/MM/yyyy HH:mm"));
                                    table.Cell().Element(CellStyle).Text(movimento.NomeProduto);
                                    table.Cell().Element(CellStyle)
                                        .Text(movimento.TipoMovimento)
                                        .FontColor(isEntrada ? Colors.Green.Darken2 : Colors.Red.Darken2)
                                        .FontWeight(FontWeight.Bold);
                                    table.Cell().Element(CellStyle).Text(movimento.Quantidade.ToString("N2")).AlignRight();
                                    table.Cell().Element(CellStyle).Text(movimento.Observacoes ?? "-");

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container.Background(Colors.White)
                                            .BorderBottom(1)
                                            .BorderColor(Colors.Grey.Lighten2)
                                            .PaddingVertical(8)
                                            .PaddingHorizontal(10)
                                            .AlignMiddle();
                                    }
                                }
                            });
                        });

                    page.Footer()
                        .Height(50)
                        .Background(Colors.Grey.Lighten3)
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Página ");
                            x.CurrentPageNumber();
                            x.Span(" de ");
                            x.TotalPages();
                            x.Span(" - Gerado em ");
                            x.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                        });
                });
            });

            return document.GeneratePdf();
        }
    }

    // DTOs para os relatórios
    public class RelatorioEstoqueItem
    {
        public string NomeProduto { get; set; } = string.Empty;
        public string CodigoProduto { get; set; } = string.Empty;
        public decimal QuantidadeAtual { get; set; }
        public decimal EstoqueMinimo { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal ValorTotal => QuantidadeAtual * PrecoUnitario;
        public string? CategoriaNome { get; set; }
    }

    public class RelatorioProdutoItem
    {
        public string Nome { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public string? CategoriaNome { get; set; }
        public decimal PrecoCompra { get; set; }
        public decimal PrecoVenda { get; set; }
        public bool Ativo { get; set; }
    }

    public class RelatorioMovimentacaoItem
    {
        public DateTime DataMovimento { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public string TipoMovimento { get; set; } = string.Empty;
        public decimal Quantidade { get; set; }
        public string? Observacoes { get; set; }
    }
}