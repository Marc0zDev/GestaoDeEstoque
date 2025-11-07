using Microsoft.AspNetCore.Mvc;
using SGE.Application.Common.DTOs;
using SGE.Domain.Entities;
using SGE.Domain.Interfaces;
using AutoMapper;

namespace SGE.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ApiControllerBase
{
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CategoriasController(
        ICategoriaRepository categoriaRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _categoriaRepository = categoriaRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>
    /// Obtém todas as categorias
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetAll()
    {
        try
        {
            var categorias = await _categoriaRepository.GetAllAsync();
            var categoriasDto = _mapper.Map<IEnumerable<CategoriaDto>>(categorias);
            return Ok(categoriasDto);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao buscar categorias: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtém uma categoria por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoriaDto>> GetById(Guid id)
    {
        try
        {
            var categoria = await _categoriaRepository.GetByIdAsync(id);
            if (categoria == null)
                return NotFound("Categoria não encontrada");

            var categoriaDto = _mapper.Map<CategoriaDto>(categoria);
            return Ok(categoriaDto);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao buscar categoria: {ex.Message}");
        }
    }

    /// <summary>
    /// Cria uma nova categoria
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CategoriaDto>> Create(CreateCategoriaDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verificar se já existe categoria com o mesmo nome
            var existingCategoria = await _categoriaRepository.GetByNameAsync(createDto.Nome);
            if (existingCategoria != null)
                return BadRequest("Já existe uma categoria com este nome");

            var categoria = _mapper.Map<Categoria>(createDto);

            await _categoriaRepository.AddAsync(categoria);
            await _unitOfWork.CommitAsync();

            var categoriaDto = _mapper.Map<CategoriaDto>(categoria);
            return CreatedAtAction(nameof(GetById), new { id = categoria.Id }, categoriaDto);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao criar categoria: {ex.Message}");
        }
    }

    /// <summary>
    /// Atualiza uma categoria existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<CategoriaDto>> Update(Guid id, UpdateCategoriaDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoria = await _categoriaRepository.GetByIdAsync(id);
            if (categoria == null)
                return NotFound("Categoria não encontrada");

            // Verificar se já existe categoria com o mesmo nome (exceto a atual)
            var existingCategoria = await _categoriaRepository.GetByNameAsync(updateDto.Nome);
            if (existingCategoria != null && existingCategoria.Id != id)
                return BadRequest("Já existe uma categoria com este nome");

            _mapper.Map(updateDto, categoria);
            categoria.MarcarComoAtualizado();

            _categoriaRepository.Update(categoria);
            await _unitOfWork.CommitAsync();

            var categoriaDto = _mapper.Map<CategoriaDto>(categoria);
            return Ok(categoriaDto);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao atualizar categoria: {ex.Message}");
        }
    }

    /// <summary>
    /// Exclui uma categoria
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            var categoria = await _categoriaRepository.GetByIdAsync(id);
            if (categoria == null)
                return NotFound("Categoria não encontrada");

            // Verificar se existem produtos vinculados à categoria
            var hasProducts = await _categoriaRepository.HasProductsAsync(id);
            if (hasProducts)
                return BadRequest("Não é possível excluir a categoria pois existem produtos vinculados a ela");

            _categoriaRepository.Delete(categoria);
            await _unitOfWork.CommitAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao excluir categoria: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtém categorias ativas
    /// </summary>
    [HttpGet("ativas")]
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetAtivas()
    {
        try
        {
            var categorias = await _categoriaRepository.GetAtivasAsync();
            var categoriasDto = _mapper.Map<IEnumerable<CategoriaDto>>(categorias);
            return Ok(categoriasDto);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao buscar categorias ativas: {ex.Message}");
        }
    }
}