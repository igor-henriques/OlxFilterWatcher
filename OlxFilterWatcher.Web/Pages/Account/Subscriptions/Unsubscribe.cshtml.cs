using System.Diagnostics.CodeAnalysis;

namespace OlxFilterWatcher.Web.Pages.Account.Subscriptions;

public class UnsubscribeModel : PageModel
{
    private readonly IOlxFilterService olxFilterService;

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [AllowNull]
        [Url]
        public string FilterURL { get; set; }

        public string FilterId { get; set; }
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public UnsubscribeModel(IOlxFilterService olxFilterService)
    {
        Input = new InputModel();
        this.olxFilterService = olxFilterService;
    }


    public async Task<IActionResult> OnGetAsync([FromQuery] string filterId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(filterId))
        {
            return RedirectToPage("../../AccessDenied");
        }
            

        var url = await olxFilterService.GetFilterUrlByIdAsync(filterId, cancellationToken);

        this.Input.FilterURL = url;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return Page();

        var unsubscribed = await olxFilterService.UnsubscribeEmailByUrl(Input.FilterURL, Input.Email, cancellationToken);

        return RedirectToPage("Index");
    }
}
