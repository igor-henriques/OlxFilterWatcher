namespace OlxFilterWatcher.Domain.Validators;

public class OlxFilterDTOValidator : AbstractValidator<OlxFilterDTO>
{
	public OlxFilterDTOValidator()
	{
		RuleFor(x => x.Filter)
			.NotNull()
			.NotEmpty()
			.Must(ValidateFilter)
			.WithMessage("Filtro inválido")
			.Must(VerifyFilterDepth)
			.WithMessage("Filtro muito amplo");

		RuleFor(x => x.Emails)
			.NotNull()
			.NotEmpty()
			.Must(ValidateEmail)
			.WithMessage("E-mail(s) inválido(s)");
	}

	private bool ValidateFilter(string filter)
	{
		if (!filter.Contains("olx.com.br"))
			return false;

		if (!Uri.TryCreate(filter, UriKind.Absolute, out _))
			return false;

		return true;
	}

    private bool ValidateEmail(List<string> emails)
    {
		foreach (var email in emails)
		{
			if (!Regex.Match(email, "[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+.[a-zA-Z]{2,}").Success)
				return false;

			if (email.Length < 10)
				return false;
		}

		return true;
    }

	private bool VerifyFilterDepth(string filter)
	{
		if (filter.Count(c => c.Equals('?')) < 1)
		{
			return false;
		}

		return true;
	}
}
