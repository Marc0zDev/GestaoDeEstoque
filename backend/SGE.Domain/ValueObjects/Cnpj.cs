using System.Text.RegularExpressions;

namespace SGE.Domain.ValueObjects;

public class Cnpj
{
    public string Value { get; private set; }

    private Cnpj(string value)
    {
        Value = value;
    }

    public static Cnpj Create(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            throw new ArgumentException("CNPJ não pode ser vazio", nameof(cnpj));

        cnpj = RemoverMascara(cnpj);

        if (!IsValidCnpj(cnpj))
            throw new ArgumentException("CNPJ inválido", nameof(cnpj));

        return new Cnpj(cnpj);
    }

    private static string RemoverMascara(string cnpj)
    {
        return Regex.Replace(cnpj, @"[^\d]", "");
    }

    private static bool IsValidCnpj(string cnpj)
    {
        if (cnpj.Length != 14) return false;
        if (cnpj.All(c => c == cnpj[0])) return false; // Todos os dígitos iguais

        // Validação dos dígitos verificadores
        var multiplicadores1 = new int[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        var multiplicadores2 = new int[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        var soma = 0;
        for (int i = 0; i < 12; i++)
            soma += int.Parse(cnpj[i].ToString()) * multiplicadores1[i];

        var resto = soma % 11;
        var digito1 = resto < 2 ? 0 : 11 - resto;

        if (int.Parse(cnpj[12].ToString()) != digito1)
            return false;

        soma = 0;
        for (int i = 0; i < 13; i++)
            soma += int.Parse(cnpj[i].ToString()) * multiplicadores2[i];

        resto = soma % 11;
        var digito2 = resto < 2 ? 0 : 11 - resto;

        return int.Parse(cnpj[13].ToString()) == digito2;
    }

    public string FormatarComMascara()
    {
        return $"{Value[..2]}.{Value[2..5]}.{Value[5..8]}/{Value[8..12]}-{Value[12..14]}";
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj)
    {
        if (obj is not Cnpj other) return false;
        return Value == other.Value;
    }

    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator string(Cnpj cnpj) => cnpj.Value;
}