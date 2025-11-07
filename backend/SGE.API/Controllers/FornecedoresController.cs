using Microsoft.AspNetCore.Mvc;
using SGE.Application.Common.DTOs;
using SGE.Domain.Entities;
using SGE.Domain.Interfaces;
using AutoMapper;

namespace SGE.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FornecedoresController : ApiControllerBase
{
    private readonly IFornecedorRepository _fornecedorRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public FornecedoresController(
        IFornecedorRepository fornecedorRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _fornecedorRepository = fornecedorRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>
    /// Obtém todos os fornecedores
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FornecedorDto>>> GetAll()
    {
        try
        {
            var fornecedores = await _fornecedorRepository.GetAllAsync();
            var fornecedoresDto = _mapper.Map<IEnumerable<FornecedorDto>>(fornecedores);
            return Ok(fornecedoresDto);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao buscar fornecedores: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtém um fornecedor por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<FornecedorDto>> GetById(Guid id)
    {
        try
        {
            var fornecedor = await _fornecedorRepository.GetByIdAsync(id);
            if (fornecedor == null)
                return NotFound("Fornecedor não encontrado");

            var fornecedorDto = _mapper.Map<FornecedorDto>(fornecedor);
            return Ok(fornecedorDto);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao buscar fornecedor: {ex.Message}");
        }
    }

    /// <summary>
    /// Cria um novo fornecedor
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<FornecedorDto>> Create(CreateFornecedorDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verificar se já existe fornecedor com o mesmo CNPJ
            if (!string.IsNullOrEmpty(createDto.Cnpj))
            {
                var existingFornecedor = await _fornecedorRepository.GetByCnpjAsync(createDto.Cnpj);
                if (existingFornecedor != null)
                    return BadRequest("Já existe um fornecedor com este CNPJ");
            }

            var fornecedor = _mapper.Map<Fornecedor>(createDto);

            await _fornecedorRepository.AddAsync(fornecedor);
            await _unitOfWork.CommitAsync();

            var fornecedorDto = _mapper.Map<FornecedorDto>(fornecedor);
            return CreatedAtAction(nameof(GetById), new { id = fornecedor.Id }, fornecedorDto);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao criar fornecedor: {ex.Message}");
        }
    }

    /// <summary>
    /// Atualiza um fornecedor existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<FornecedorDto>> Update(Guid id, UpdateFornecedorDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var fornecedor = await _fornecedorRepository.GetByIdAsync(id);
            if (fornecedor == null)
                return NotFound("Fornecedor não encontrado");

            // Verificar se já existe fornecedor com o mesmo CNPJ (exceto o atual)
            if (!string.IsNullOrEmpty(updateDto.Cnpj))
            {
                var existingFornecedor = await _fornecedorRepository.GetByCnpjAsync(updateDto.Cnpj);
                if (existingFornecedor != null && existingFornecedor.Id != id)
                    return BadRequest("Já existe um fornecedor com este CNPJ");
            }

            _mapper.Map(updateDto, fornecedor);
            fornecedor.MarcarComoAtualizado();

            _fornecedorRepository.Update(fornecedor);
            await _unitOfWork.CommitAsync();

            var fornecedorDto = _mapper.Map<FornecedorDto>(fornecedor);
            return Ok(fornecedorDto);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao atualizar fornecedor: {ex.Message}");
        }
    }

    /// <summary>
    /// Exclui um fornecedor
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            var fornecedor = await _fornecedorRepository.GetByIdAsync(id);
            if (fornecedor == null)
                return NotFound("Fornecedor não encontrado");

            // Verificar se existem produtos vinculados ao fornecedor
            var hasProducts = await _fornecedorRepository.HasProductsAsync(id);
            if (hasProducts)
                return BadRequest("Não é possível excluir o fornecedor pois existem produtos vinculados a ele");

            _fornecedorRepository.Delete(fornecedor);
            await _unitOfWork.CommitAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao excluir fornecedor: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtém fornecedores ativos
    /// </summary>
    [HttpGet("ativos")]
    public async Task<ActionResult<IEnumerable<FornecedorDto>>> GetAtivos()
    {
        try
        {
            var fornecedores = await _fornecedorRepository.GetAtivosAsync();
            var fornecedoresDto = _mapper.Map<IEnumerable<FornecedorDto>>(fornecedores);
            return Ok(fornecedoresDto);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao buscar fornecedores ativos: {ex.Message}");
        }
    }
}