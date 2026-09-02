using PdfSharp.Fonts;

namespace Infrastructure.Reports
{
    public sealed class TrainingReportFontResolver : IFontResolver
    {
        public const string FamilyName =
            "TrainingReportSans";

        private const string RegularFace =
            "TrainingReportSans-Regular";

        private const string BoldFace =
            "TrainingReportSans-Bold";

        private const string RegularResource =
            "TrainingReports.Fonts.NotoSans-Regular.ttf";

        private const string BoldResource =
            "TrainingReports.Fonts.NotoSans-Bold.ttf";

        public FontResolverInfo? ResolveTypeface(
            string familyName,
            bool bold,
            bool italic
        )
        {
            /*
             * Este resolver es exclusivo para nuestros PDFs.
             *
             * Aunque MigraDoc solicite otra familia internamente,
             * siempre la resolvemos hacia Noto Sans.
             */

            var faceName =
                bold
                    ? BoldFace
                    : RegularFace;

            return new FontResolverInfo(
                faceName,
                false,
                italic
            );
        }

        public byte[]? GetFont(
            string faceName
        )
        {
            var resourceName =
                faceName switch
                {
                    RegularFace =>
                        RegularResource,

                    BoldFace =>
                        BoldResource,

                    _ =>
                        null
                };

            if (resourceName is null)
            {
                return null;
            }

            var assembly =
                typeof(TrainingReportFontResolver)
                    .Assembly;

            using var stream =
                assembly.GetManifestResourceStream(
                    resourceName
                )
                ?? throw new InvalidOperationException(
                    $"No se encontró la fuente embebida '{resourceName}'."
                );

            using var memoryStream =
                new MemoryStream();

            stream.CopyTo(memoryStream);

            return memoryStream.ToArray();
        }
    }
}