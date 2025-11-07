using Microsoft.AspNetCore.Mvc;
using SGE.Application.Common.DTOs;
using SGE.Domain.Entities;
using SGE.Domain.Interfaces;
using AutoMapper;

namespace SGE.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosCrudController : ApiControllerBase
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IFornecedorRepository _fornecedorRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProdutosCrudController(
        IProdutoRepository produtoRepository,
        ICategoriaRepository categoriaRepository,
        IFornecedorRepository fornecedorRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _produtoRepository = produtoRepository;
        _categoriaRepository = categoriaRepository;
        _fornecedorRepository = fornecedorRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>
    /// Obtém todos os produtos
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProdutoDto>>> GetAll()
    {
        try
        {
            var produtos = await _produtoRepository.GetAllWithDetailsAsync();
            var produtosDto = _mapper.Map<IEnumerable<ProdutoDto>>(produtos);
            return Ok(produtosDto);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao buscar produtos: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtém um produto por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProdutoDto>> GetById(Guid id)
    {
        try
        {
            var produto = await _produtoRepository.GetByIdWithDetailsAsync(id);
            if (produto == null)
                return NotFound("Produto não encontrado");

            var produtoDto = _mapper.Map<ProdutoDto>(produto);
            return Ok(produtoDto);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao buscar produto: {ex.Message}");
        }
    }

    /// <summary>
    /// Cria um novo produto
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ProdutoDto>> Create(CreateProdutoDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verificar se já existe produto com o mesmo código
            var existingProduto = await _produtoRepository.GetByCodigoAsync(createDto.Codigo);
            if (existingProduto != null)
                return BadRequest("Já existe um produto com este código");

            // Verificar se a categoria existe
            var categoria = await _categoriaRepository.GetByIdAsync(createDto.CategoriaId);
            if (categoria == null)
                return BadRequest("Categoria não encontrada");

            // Verificar se o fornecedor existe (se informado)
            if (createDto.FornecedorId.HasValue)
            {
                var fornecedor = await _fornecedorRepository.GetByIdAsync(createDto.FornecedorId.Value);
                if (fornecedor == null)
                    return BadRequest("Fornecedor não encontrado");
            }

            var produto = _mapper.Map<Produto>(createDto);

            await _produtoRepository.AddAsync(produto);
            await _unitOfWork.CommitAsync();

            // Buscar o produto com detalhes para retornar
            var produtoCompleto = await _produtoRepository.GetByIdWithDetailsAsync(produto.Id);
            var produtoDto = _mapper.Map<ProdutoDto>(produtoCompleto);
            
            return CreatedAtAction(nameof(GetById), new { id = produto.Id }, produtoDto);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao criar produto: {ex.Message}");
        }
    }

    /// <summary>
    /// Atualiza um produto existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ProdutoDto>> Update(Guid id, UpdateProdutoDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var produto = await _produtoRepository.GetByIdAsync(id);
            if (produto == null)
                return NotFound("Produto não encontrado");

            // Verificar se já existe produto com o mesmo código (exceto o atual)
            var existingProduto = await _produtoRepository.GetByCodigoAsync(updateDto.Codigo);
            if (existingProduto != null && existingProduto.Id != id)
                return BadRequest("Já existe um produto com este código");

            // Verificar se a categoria existe
            var categoria = await _categoriaRepository.GetByIdAsync(updateDto.CategoriaId);
            if (categoria == null)
                return BadRequest("Categoria não encontrada");

            // Verificar se o fornecedor existe (se informado)
            if (updateDto.FornecedorId.HasValue)
            {
                var fornecedor = await _fornecedorRepository.GetByIdAsync(updateDto.FornecedorId.Value);
                if (fornecedor == null)
                    return BadRequest("Fornecedor não encontrado");
            }

            _mapper.Map(updateDto, produto);
            produto.MarcarComoAtualizado();

            _produtoRepository.Update(produto);
            await _unitOfWork.CommitAsync();

            // Buscar o produto com detalhes para retornar
            var produtoCompleto = await _produtoRepository.GetByIdWithDetailsAsync(produto.Id);
            var produtoDto = _mapper.Map<ProdutoDto>(produtoCompleto);
            
            return Ok(produtoDto);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao atualizar produto: {ex.Message}");
        }
    }

    /// <summary>
    /// Exclui um produto
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            var produto = await _produtoRepository.GetByIdAsync(id);
            if (produto == null)
                return NotFound("Produto não encontrado");

            // Verificar se existem movimentações de estoque para o produto
            var hasMovimentacoes = await _produtoRepository.HasMovimentacoesAsync(id);
            if (hasMovimentacoes)
                return BadRequest("Não é possível excluir o produto pois existem movimentações de estoque vinculadas a ele");

            _produtoRepository.Delete(produto);
            await _unitOfWork.CommitAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao excluir produto: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtém produtos ativos
    /// </summary>
    [HttpGet("ativos")]
    public async Task<ActionResult<IEnumerable<ProdutoDto>>> GetAtivos()
    {
        try
        {
            var produtos = await _produtoRepository.GetAtivosAsync();
            var produtosDto = _mapper.Map<IEnumerable<ProdutoDto>>(produtos);
            return Ok(produtosDto);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao buscar produtos ativos: {ex.Message}");
        }
    }

    /// <summary>
    /// Busca produtos por categoria
    /// </summary>
    [HttpGet("categoria/{categoriaId}")]
    public async Task<ActionResult<IEnumerable<ProdutoDto>>> GetByCategoria(Guid categoriaId)
    {
        try
        {
            var produtos = await _produtoRepository.GetByCategoriaAsync(categoriaId);
            var produtosDto = _mapper.Map<IEnumerable<ProdutoDto>>(produtos);
            return Ok(produtosDto);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao buscar produtos da categoria: {ex.Message}");
        }
    }

    /// <summary>
    /// Busca produtos por fornecedor
    /// </summary>
    [HttpGet("fornecedor/{fornecedorId}")]
    public async Task<ActionResult<IEnumerable<ProdutoDto>>> GetByFornecedor(Guid fornecedorId)
    {
        try
        {
            var produtos = await _produtoRepository.GetByFornecedorAsync(fornecedorId);
            var produtosDto = _mapper.Map<IEnumerable<ProdutoDto>>(produtos);
            return Ok(produtosDto);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao buscar produtos do fornecedor: {ex.Message}");
        }
    }

    /// <summary>
    /// Busca produtos por termo
    /// </summary>
    [HttpGet("buscar")]
    public async Task<ActionResult<IEnumerable<ProdutoDto>>> Search([FromQuery] string termo)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(termo))
                return BadRequest("Termo de busca é obrigatório");

            var produtos = await _produtoRepository.SearchAsync(termo);
            var produtosDto = _mapper.Map<IEnumerable<ProdutoDto>>(produtos);
            return Ok(produtosDto);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao buscar produtos: {ex.Message}");
        }
    }
}