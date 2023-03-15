namespace OlxFilterWatcher.Workers.Utils;

public class HtmlBuilder
{
    public static async Task<string> GetBaseHTML(CancellationToken cancellationToken = default) 
        =>  await File.ReadAllTextAsync("./Resources/EmailTemplate.html", cancellationToken);

    public static string BuildHtml(string html, dynamic olxPostDTO, string url)
    {
        var isRentPost = olxPostDTO.GetPostType().Name.Contains("Rent");
        var isVehiclePost = olxPostDTO.GetPostType().Name.Contains("Vehicle");

        var imagesHtml = ((IEnumerable<string>)olxPostDTO.Images)?.Select((image, index) =>
        {
            if (index > 0)
            {
                return InsertImageToHtml(image);
            }

            return null;
        }).Where(img => img is not null).ToList();

        StringBuilder htmlBuilder = new(html);
        StringBuilder paragraphBuilder = new();

        paragraphBuilder.AppendLine(InsertParagraphToHtml("Postado em", olxPostDTO.TimePosted.ToString("dd/MM/yyyy") + " " + olxPostDTO.TimePosted.ToString("HH:mm")));
        paragraphBuilder.AppendLine(InsertParagraphToHtml("Título", olxPostDTO.Title));
        paragraphBuilder.AppendLine(InsertParagraphToHtml("Preço", olxPostDTO.PostPrice.ToString("c", CultureInfo.GetCultureInfo("pt-BR"))));

        if (isRentPost)
        {
            paragraphBuilder.AppendLine(InsertParagraphToHtml("Condomínio", olxPostDTO.CondominiumTax.ToString("c", CultureInfo.GetCultureInfo("pt-BR"))));
            paragraphBuilder.AppendLine(InsertParagraphToHtml("IPTU", olxPostDTO.IptuTax.ToString("c", CultureInfo.GetCultureInfo("pt-BR"))));
            paragraphBuilder.AppendLine(InsertParagraphToHtml("Quartos", olxPostDTO.RoomCount.ToString()));
            paragraphBuilder.AppendLine(InsertParagraphToHtml("Vagas de Carro", olxPostDTO.CarSpot.ToString()));
        }        
        else if (isVehiclePost)
        {
            paragraphBuilder.AppendLine(InsertParagraphToHtml("Ano do Veículo", olxPostDTO.Year));
            paragraphBuilder.AppendLine(InsertParagraphToHtml("Kilometragem", olxPostDTO.KmCount));

            if (!url.Contains("/motos"))
                paragraphBuilder.AppendLine(InsertParagraphToHtml("Transmissão", olxPostDTO.Transmission));
        }

        paragraphBuilder.AppendLine(InsertParagraphToHtml("CEP", olxPostDTO.ZipCode));

        htmlBuilder.Replace("$PARAGRAPHS", paragraphBuilder.ToString());
        htmlBuilder.Replace("$THUMBIMG", ((List<string>)olxPostDTO?.Images)?.FirstOrDefault() ?? HtmlElements.NotFoundImageUrl);
        htmlBuilder.Replace("$CURRENTFILTER", olxPostDTO.FoundByFilter);
        htmlBuilder.Replace("$URL", olxPostDTO.URL);
        htmlBuilder.Replace("$UNSIGN_URL", "http://localhost"); //inserir link para desinscrever da lista de emails
        
        htmlBuilder.Replace("$IMAGES_PLACEHOLDER", imagesHtml is not null ? string.Join("\n", imagesHtml) : string.Empty);
        htmlBuilder.Replace("$IMAGES_LABEL_PLACEHOLDER", imagesHtml is not null ? InsertImagesDescription() : string.Empty);

        return htmlBuilder.ToString();
    }    

    public static string InsertParagraphToHtml(string information, string content)
    {
        StringBuilder sb = new();

        sb.AppendLine("<p style=\"margin: 0; margin-bottom: 16px;\">");
        sb.AppendLine(string.Format("<b>{0}:</b> {1}", information, content));
        sb.AppendLine("</p>");

        return sb.ToString();
    }
    
    private static string InsertImagesDescription()
    {
        StringBuilder sb = new();

        sb.AppendLine("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"heading_block block-4\" role=\"presentation\" style=\"mso-table-lspace: 0pt; mso-table-rspace: 0pt;\" width=\"100%\">");
        sb.AppendLine("<tr>");
        sb.AppendLine("<td class=\"pad\" style=\"width:100%;text-align:center;padding-top:10px;\">");
        sb.AppendLine("<h1 style=\"margin: 0; color: #555555; font-size: 23px; font-family: Arial, Helvetica Neue, Helvetica, sans-serif; line-height: 120%; text-align: center; direction: ltr; font-weight: 700; letter-spacing: normal; margin-top: 0; margin-bottom: 0;\">");
        sb.AppendLine("<span class=\"tinyMce-placeholder\">Veja algumas imagens</span>");
        sb.AppendLine("</h1>");
        sb.AppendLine("</td>");
        sb.AppendLine("</tr>");
        sb.AppendLine("</table>");

        return sb.ToString();
    }

    private static string InsertImageToHtml(string imageUrl)
    {
        StringBuilder sb = new();

        sb.AppendLine("<table border=\"0\" cellpadding=\"15\" cellspacing=\"0\"=\" \"class=\"image_block block-8\" role=\"presentation\" style=\"mso-table-lspace: 0pt; mso-table-rspace: 0pt;\" width=\"100%\">");
        sb.AppendLine("<tr>");
        sb.AppendLine("<td class=\"pad\">");
        sb.AppendLine("<div align=\"center\" class=\"alignment\" style=\"line-height:10px\">");
        sb.AppendLine($"<a href=\"{imageUrl}\"><img class=\"big\" src=\"{imageUrl}\" style=\"display: block; height: auto; border: 0; width: 375px; max-width: 100%;\" width=\"375\" /></a>");
        sb.AppendLine("</div>");
        sb.AppendLine("</td>");
        sb.AppendLine("</tr>");
        sb.AppendLine("</table>");

        return sb.ToString();
    }
}
