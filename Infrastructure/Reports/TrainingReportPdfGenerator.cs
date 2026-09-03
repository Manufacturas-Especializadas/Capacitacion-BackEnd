using Application.DTOs.TrainingReports;
using Application.Interfaces.Reports;
using Domain.Interfaces;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using System.Globalization;

namespace Infrastructure.Reports
{
    public sealed class TrainingReportPdfGenerator(
        IBlobStorageService blobStorageService
    ) : ITrainingReportPdfGenerator
    {
        private static readonly Color PrimaryColor =
            Color.FromRgb(30, 64, 175);

        private static readonly Color PrimarySoftColor =
            Color.FromRgb(239, 246, 255);

        private static readonly Color BorderColor =
            Color.FromRgb(203, 213, 225);

        private static readonly Color TextColor =
            Color.FromRgb(30, 41, 59);

        private static readonly Color MutedColor =
            Color.FromRgb(100, 116, 139);

        private static readonly Color LightBackgroundColor =
            Color.FromRgb(248, 250, 252);

        public async Task<byte[]> GenerateAsync(
            TrainingReportDetailsDto report,
            CancellationToken cancellationToken = default
        )
        {
            var signatureAssets =
                await LoadSignatureAssetsAsync(
                    report,
                    cancellationToken
                );

            var document =
                CreateDocument(
                    report,
                    signatureAssets
                );

            var renderer =
                new PdfDocumentRenderer
                {
                    Document = document
                };

            renderer.RenderDocument();

            using var stream =
                new MemoryStream();

            renderer.PdfDocument.Save(
                stream,
                false
            );

            return stream.ToArray();
        }

        private Document CreateDocument(
            TrainingReportDetailsDto report,
            SignatureAssets signatureAssets
        )
        {
            var document =
                new Document();

            document.Info.Title =
                $"Reporte de capacitación #{report.Id}";

            document.Info.Subject =
                "Reporte de capacitación";

            ConfigureStyles(document);

            var section =
                document.AddSection();

            ConfigurePage(section);

            AddFooter(
                section,
                report.Id
            );

            AddReportHeader(
                section,
                report
            );

            AddGeneralInformation(
                section,
                report
            );

            if (report.WeldingUnionTypes.Count > 0)
            {
                AddWeldingUnionTypes(
                    section,
                    report
                );
            }

            AddSectionTitle(
    section,
    "Participantes"
);

            for (
                var index = 0;
                index < report.Attendees.Count;
                index++
            )
            {
                AddAttendee(
                    section,
                    report.Attendees[index],
                    index + 1
                );
            }

            AddAttendeeSignaturesTable(
                section,
                report,
                signatureAssets
            );

            AddObservations(
                section,
                report.Observations
            );

            AddClosingSignatures(
                section,
                signatureAssets
            );

            return document;
        }

        private static void ConfigureStyles(
            Document document
        )
        {
            var normal =
                document.Styles["Normal"];

            normal.Font.Name =
                TrainingReportFontResolver.FamilyName;

            normal.Font.Size =
                Unit.FromPoint(8.5);

            normal.Font.Color =
                TextColor;

            normal.ParagraphFormat.SpaceAfter =
                Unit.FromPoint(2);
        }

        private static void ConfigurePage(
            Section section
        )
        {
            section.PageSetup.PageFormat =
                PageFormat.A4;

            section.PageSetup.Orientation =
                Orientation.Portrait;

            section.PageSetup.TopMargin =
                Unit.FromCentimeter(1.3);

            section.PageSetup.BottomMargin =
                Unit.FromCentimeter(1.4);

            section.PageSetup.LeftMargin =
                Unit.FromCentimeter(1.4);

            section.PageSetup.RightMargin =
                Unit.FromCentimeter(1.4);
        }

        private static void AddFooter(
            Section section,
            int reportId
        )
        {
            var footer =
                section.Footers.Primary
                    .AddParagraph();

            footer.Format.Alignment =
                ParagraphAlignment.Center;

            footer.Format.Font.Size =
                Unit.FromPoint(7);

            footer.Format.Font.Color =
                MutedColor;

            footer.AddText(
                $"Reporte #{reportId} · Página "
            );

            footer.AddPageField();
        }

        private static void AddReportHeader(
            Section section,
            TrainingReportDetailsDto report
        )
        {
            var table =
                section.AddTable();

            table.AddColumn(
                Unit.FromCentimeter(13.4)
            );

            table.AddColumn(
                Unit.FromCentimeter(4.5)
            );

            table.Borders.Width = 0;

            var row =
                table.AddRow();

            row.BottomPadding =
                Unit.FromPoint(4);

            var titleCell =
                row.Cells[0];

            var title =
                titleCell.AddParagraph();

            title.Format.Font.Size =
                Unit.FromPoint(16);

            title.Format.Font.Bold = true;

            title.Format.Font.Color =
                PrimaryColor;

            title.AddText(
                "REPORTE DE CAPACITACIÓN"
            );

            var subtitle =
                titleCell.AddParagraph();

            subtitle.Format.Font.Size =
                Unit.FromPoint(9);

            subtitle.Format.Font.Color =
                MutedColor;

            subtitle.AddText(
                "Registro de entrenamiento y evidencia de capacitación"
            );

            var idCell =
                row.Cells[1];

            idCell.Shading.Color =
                PrimarySoftColor;

            idCell.Format.Alignment =
                ParagraphAlignment.Center;

            idCell.VerticalAlignment =
                VerticalAlignment.Center;

            var idLabel =
                idCell.AddParagraph();

            idLabel.Format.Font.Size =
                Unit.FromPoint(7);

            idLabel.Format.Font.Bold = true;

            idLabel.Format.Font.Color =
                MutedColor;

            idLabel.AddText(
                "REPORTE"
            );

            var idValue =
                idCell.AddParagraph();

            idValue.Format.Font.Size =
                Unit.FromPoint(14);

            idValue.Format.Font.Bold = true;

            idValue.Format.Font.Color =
                PrimaryColor;

            idValue.AddText(
                $"#{report.Id}"
            );

            var separator =
                section.AddParagraph();

            separator.Format.SpaceAfter =
                Unit.FromPoint(12);

            separator.Format.Borders.Bottom.Width =
                Unit.FromPoint(1.5);

            separator.Format.Borders.Bottom.Color =
                PrimaryColor;
        }

        private static void AddGeneralInformation(
    Section section,
    TrainingReportDetailsDto report
)
        {
            AddSectionTitle(
                section,
                "Información general"
            );

            var table =
                CreateInformationTable(
                    section
                );

            var firstRow =
                table.AddRow();

            AddKeyValueCell(
                firstRow.Cells[0],
                "Tipo de capacitación",
                report.TrainingType
            );

            AddKeyValueCell(
                firstRow.Cells[1],
                "Líder o instructor",
                report.LeaderName
            );

            var secondRow =
                table.AddRow();

            AddKeyValueCell(
                secondRow.Cells[0],
                "Nómina",
                report.LeaderPayroll
            );

            AddKeyValueCell(
                secondRow.Cells[1],
                "Semana",
                report.WeekNumber?.ToString()
                    ?? "No especificada"
            );

            var metadata =
                section.AddParagraph();

            metadata.Format.Font.Size =
                Unit.FromPoint(7);

            metadata.Format.Font.Color =
                MutedColor;

            metadata.Format.SpaceBefore =
                Unit.FromPoint(2);

            metadata.Format.SpaceAfter =
                Unit.FromPoint(3);

            metadata.AddText(
                $"Creado: {report.CreatedAt.ToString(
                    "dd/MM/yyyy HH:mm",
                    CultureInfo.InvariantCulture
                )}"
            );

            metadata.AddText(
                $"   ·   Participantes: {report.Attendees.Count}"
            );
        }

        private static void AddWeldingUnionTypes(
            Section section,
            TrainingReportDetailsDto report
        )
        {
            AddSectionTitle(
                section,
                "Tipos de unión de soldadura"
            );

            var table =
                section.AddTable();

            table.Borders.Color =
                BorderColor;

            table.Borders.Width =
                Unit.FromPoint(0.5);

            table.AddColumn(
                Unit.FromCentimeter(4)
            );

            table.AddColumn(
                Unit.FromCentimeter(13.9)
            );

            var header =
                table.AddRow();

            header.Shading.Color =
                PrimarySoftColor;

            AddTableHeader(
                header.Cells[0],
                "Lista"
            );

            AddTableHeader(
                header.Cells[1],
                "Tipo de unión"
            );

            foreach (
                var union
                in report.WeldingUnionTypes
            )
            {
                var row =
                    table.AddRow();

                AddTableValue(
                    row.Cells[0],
                    union.ListNumber.ToString()
                );

                AddTableValue(
                    row.Cells[1],
                    union.UnionName
                );
            }

            AddSectionSpacing(section);
        }

        private static void AddAttendee(
    Section section,
    TrainingReportAttendeeDetailsDto attendee,
    int number
)
        {
            var table =
                section.AddTable();

            table.AddColumn(
                Unit.FromCentimeter(17.9)
            );

            table.Borders.Color =
                BorderColor;

            table.Borders.Width =
                Unit.FromPoint(0.5);

            /*
             * Encabezado compacto del participante.
             */
            var header =
                table.AddRow();

            header.Shading.Color =
                PrimarySoftColor;

            header.TopPadding =
                Unit.FromPoint(3);

            header.BottomPadding =
                Unit.FromPoint(3);

            var headerParagraph =
                header.Cells[0]
                    .AddParagraph();

            headerParagraph.Format.Font.Size =
                Unit.FromPoint(8.5);

            var numberText =
                headerParagraph
                    .AddFormattedText(
                        $"{number:00}  ",
                        TextFormat.Bold
                    );

            numberText.Color =
                PrimaryColor;

            var name =
                headerParagraph
                    .AddFormattedText(
                        attendee.EmployeeName,
                        TextFormat.Bold
                    );

            name.Color =
                TextColor;

            var identityText =
                $"  ·  Nómina {attendee.EmployeeNumber}" +
                $"  ·  Línea {attendee.LineName}";

            headerParagraph.AddText(
                identityText
            );

            var total =
                headerParagraph
                    .AddFormattedText(
                        $"  ·  Total {FormatHoursCompact(attendee.TotalHours)}",
                        TextFormat.Bold
                    );

            total.Color =
                PrimaryColor;

            /*
             * Información del participante.
             */
            var content =
                table.AddRow();

            content.TopPadding =
                Unit.FromPoint(2);

            content.BottomPadding =
                Unit.FromPoint(3);

            var cell =
                content.Cells[0];

            AddCompactLine(
                cell,
                "Temas",
                BuildTopicsText(attendee)
            );

            AddCompactLine(
                cell,
                "Horario",
                BuildScheduleText(attendee)
            );

            var extraInformation =
                BuildAttendeeDetailsText(
                    attendee
                );

            if (
                !string.IsNullOrWhiteSpace(
                    extraInformation
                )
            )
            {
                AddCompactLine(
                    cell,
                    "Datos",
                    extraInformation
                );
            }

            var spacing =
                section.AddParagraph();

            spacing.Format.SpaceAfter =
                Unit.FromPoint(3);
        }

        private static void AddAttendeeSignaturesTable(
    Section section,
    TrainingReportDetailsDto report,
    SignatureAssets signatureAssets
)
        {
            if (report.Attendees.Count == 0)
            {
                return;
            }

            AddSectionTitle(
                section,
                "Firmas de participantes"
            );

            var table =
                section.AddTable();

            table.AddColumn(
                Unit.FromCentimeter(7)
            );

            table.AddColumn(
                Unit.FromCentimeter(5.45)
            );

            table.AddColumn(
                Unit.FromCentimeter(5.45)
            );

            table.Borders.Color =
                BorderColor;

            table.Borders.Width =
                Unit.FromPoint(0.5);

            /*
             * Encabezados.
             */
            var header =
                table.AddRow();

            header.Shading.Color =
                LightBackgroundColor;

            AddTableHeader(
                header.Cells[0],
                "Participante"
            );

            AddTableHeader(
                header.Cells[1],
                "Firma asistente"
            );

            AddTableHeader(
                header.Cells[2],
                "Firma supervisor"
            );

            /*
             * Una sola fila compacta por participante.
             */
            foreach (
                var attendee
                in report.Attendees
            )
            {
                signatureAssets
                    .Attendees
                    .TryGetValue(
                        attendee.Id,
                        out var assets
                    );

                var row =
                    table.AddRow();

                row.TopPadding =
                    Unit.FromPoint(2);

                row.BottomPadding =
                    Unit.FromPoint(2);

                var identity =
                    row.Cells[0]
                        .AddParagraph();

                identity.Format.Font.Size =
                    Unit.FromPoint(7.5);

                identity.AddFormattedText(
                    attendee.EmployeeName,
                    TextFormat.Bold
                );

                identity.AddText(
                    $" · {attendee.EmployeeNumber}"
                );

                AddCompactSignatureImage(
                    row.Cells[1],
                    assets?.Trainee
                );

                AddCompactSignatureImage(
                    row.Cells[2],
                    assets?.Supervisor
                );
            }

            AddSectionSpacing(
                section
            );
        }

        private static void AddCompactSignatureImage(
    Cell cell,
    string? imageSource
)
        {
            var paragraph =
                cell.AddParagraph();

            paragraph.Format.Alignment =
                ParagraphAlignment.Center;

            paragraph.Format.SpaceBefore =
                Unit.FromPoint(1);

            paragraph.Format.SpaceAfter =
                Unit.FromPoint(1);

            if (
                !string.IsNullOrWhiteSpace(
                    imageSource
                )
            )
            {
                var image =
                    paragraph.AddImage(
                        imageSource
                    );

                image.LockAspectRatio =
                    true;

                image.Height =
                    Unit.FromCentimeter(0.85);

                return;
            }

            var missing =
                paragraph.AddFormattedText(
                    "Pendiente"
                );

            missing.Font.Size =
                Unit.FromPoint(7);

            missing.Font.Color =
                MutedColor;
        }

        private static void AddCompactLine(
    Cell cell,
    string label,
    string value
)
        {
            var paragraph =
                cell.AddParagraph();

            paragraph.Format.Font.Size =
                Unit.FromPoint(7.8);

            paragraph.Format.SpaceBefore =
                Unit.FromPoint(1);

            paragraph.Format.SpaceAfter =
                Unit.FromPoint(1);

            var labelText =
                paragraph.AddFormattedText(
                    $"{label}: ",
                    TextFormat.Bold
                );

            labelText.Color =
                MutedColor;

            paragraph.AddText(
                string.IsNullOrWhiteSpace(value)
                    ? "—"
                    : value
            );
        }

        private static string BuildTopicsText(
            TrainingReportAttendeeDetailsDto attendee
        )
        {
            if (attendee.Topics.Count == 0)
            {
                return "Sin temas registrados";
            }

            return string.Join(
                "  ·  ",
                attendee.Topics.Select(topic =>
                {
                    var code =
                        topic.TopicCode?.Trim();

                    var name =
                        topic.TopicName?.Trim();

                    if (
                        string.IsNullOrWhiteSpace(
                            code
                        )
                    )
                    {
                        return name ?? "—";
                    }

                    if (
                        string.IsNullOrWhiteSpace(
                            name
                        )
                    )
                    {
                        return code;
                    }

                    return $"{code} {name}";
                })
            );
        }

        private static string BuildScheduleText(
            TrainingReportAttendeeDetailsDto attendee
        )
        {
            var days =
                new List<string>();

            AddCompactDay(
                days,
                "Lun",
                attendee.DayMonday,
                attendee.HoursMonday
            );

            AddCompactDay(
                days,
                "Mar",
                attendee.DayTuesday,
                attendee.HoursTuesday
            );

            AddCompactDay(
                days,
                "Mié",
                attendee.DayWednesday,
                attendee.HoursWednesday
            );

            AddCompactDay(
                days,
                "Jue",
                attendee.DayThursday,
                attendee.HoursThursday
            );

            AddCompactDay(
                days,
                "Vie",
                attendee.DayFriday,
                attendee.HoursFriday
            );

            AddCompactDay(
                days,
                "Sáb",
                attendee.DaySaturday,
                attendee.HoursSaturday
            );

            AddCompactDay(
                days,
                "Dom",
                attendee.DaySunday,
                attendee.HoursSunday
            );

            if (days.Count == 0)
            {
                return "Sin días seleccionados";
            }

            return string.Join(
                "  ·  ",
                days
            );
        }

        private static void AddCompactDay(
            ICollection<string> days,
            string day,
            bool selected,
            decimal? hours
        )
        {
            if (!selected)
            {
                return;
            }

            days.Add(
                $"{day} {FormatHoursCompact(hours)}"
            );
        }

        private static string BuildAttendeeDetailsText(
            TrainingReportAttendeeDetailsDto attendee
        )
        {
            var values =
                new List<string>();

            AddCompactDetail(
                values,
                "Turno",
                attendee.Shift
            );

            AddCompactDetail(
                values,
                "Maquinaria",
                attendee.Machinery
            );

            AddCompactDetail(
                values,
                "AST",
                attendee.Ast
            );

            AddCompactDetail(
                values,
                "Cliente",
                attendee.CustomerClient
            );

            AddCompactDetail(
                values,
                "Unión",
                attendee.UnionClassification
            );

            AddCompactDetail(
                values,
                "Soldadura",
                attendee.WeldingPercentage
            );

            AddCompactDetail(
                values,
                "Diámetro",
                attendee.Diameter
            );

            return string.Join(
                "  ·  ",
                values
            );
        }

        private static void AddCompactDetail(
            ICollection<string> values,
            string label,
            string? value
        )
        {
            if (
                string.IsNullOrWhiteSpace(value)
            )
            {
                return;
            }

            values.Add(
                $"{label}: {value.Trim()}"
            );
        }


        private static void AddObservations(
    Section section,
    string? observations
)
        {
            AddSectionTitle(
                section,
                "Observaciones"
            );

            var paragraph =
                section.AddParagraph();

            paragraph.Format.Font.Size =
                Unit.FromPoint(8);

            paragraph.Format.SpaceAfter =
                Unit.FromPoint(4);

            paragraph.AddText(
                string.IsNullOrWhiteSpace(
                    observations
                )
                    ? "Sin observaciones registradas."
                    : observations.Trim()
            );
        }

        private static void AddClosingSignatures(
            Section section,
            SignatureAssets assets
        )
        {
            AddSectionTitle(
                section,
                "Firmas de cierre"
            );

            var table =
                section.AddTable();

            table.AddColumn(
                Unit.FromCentimeter(5.9)
            );

            table.AddColumn(
                Unit.FromCentimeter(5.9)
            );

            table.AddColumn(
                Unit.FromCentimeter(5.9)
            );

            table.Borders.Color =
                BorderColor;

            table.Borders.Width =
                Unit.FromPoint(0.5);

            var row =
                table.AddRow();

            AddSignatureCell(
                row.Cells[0],
                "Instructor responsable",
                assets.Instructor
            );

            AddSignatureCell(
                row.Cells[1],
                "Coordinación de capacitación",
                assets.Coordinator
            );

            AddSignatureCell(
                row.Cells[2],
                "Responsable de seguridad",
                assets.Security
            );
        }

        private static Table CreateInformationTable(
            Section section
        )
        {
            var table =
                section.AddTable();

            table.AddColumn(
                Unit.FromCentimeter(8.8)
            );

            table.AddColumn(
                Unit.FromCentimeter(8.8)
            );

            table.Borders.Color =
                BorderColor;

            table.Borders.Width =
                Unit.FromPoint(0.5);

            return table;
        }



        private static void AddKeyValueCell(
            Cell cell,
            string label,
            string value
        )
        {
            cell.Shading.Color =
                LightBackgroundColor;

            var labelParagraph =
                cell.AddParagraph();

            labelParagraph.Format.Font.Size =
                Unit.FromPoint(7);

            labelParagraph.Format.Font.Bold =
                true;

            labelParagraph.Format.Font.Color =
                MutedColor;

            labelParagraph.AddText(
                label.ToUpperInvariant()
            );

            var valueParagraph =
                cell.AddParagraph();

            valueParagraph.Format.Font.Size =
                Unit.FromPoint(9);

            valueParagraph.Format.Font.Bold =
                true;

            valueParagraph.Format.Font.Color =
                TextColor;

            valueParagraph.AddText(
                string.IsNullOrWhiteSpace(value)
                    ? "Sin información"
                    : value
            );
        }

        private static void AddTableHeader(
            Cell cell,
            string value
        )
        {
            var paragraph =
                cell.AddParagraph();

            paragraph.Format.Alignment =
                ParagraphAlignment.Center;

            paragraph.Format.Font.Size =
                Unit.FromPoint(8);

            paragraph.Format.Font.Bold =
                true;

            paragraph.Format.Font.Color =
                TextColor;

            paragraph.AddText(value);
        }

        private static void AddTableValue(
            Cell cell,
            string? value
        )
        {
            var paragraph =
                cell.AddParagraph();

            paragraph.Format.Font.Size =
                Unit.FromPoint(8.5);

            paragraph.AddText(
                string.IsNullOrWhiteSpace(value)
                    ? "—"
                    : value
            );
        }

        private static void AddSignatureCell(
            Cell cell,
            string label,
            string? imageSource
        )
        {
            var title =
                cell.AddParagraph();

            title.Format.Alignment =
                ParagraphAlignment.Center;

            title.Format.Font.Size =
                Unit.FromPoint(7);

            title.Format.Font.Bold =
                true;

            title.Format.Font.Color =
                MutedColor;

            title.AddText(label);

            var content =
                cell.AddParagraph();

            content.Format.Alignment =
                ParagraphAlignment.Center;

            content.Format.SpaceBefore =
                Unit.FromPoint(3);

            content.Format.SpaceAfter =
                Unit.FromPoint(3);

            if (!string.IsNullOrWhiteSpace(
                imageSource
            ))
            {
                var image =
                    content.AddImage(
                        imageSource
                    );

                image.LockAspectRatio = true;

                image.Height =
    Unit.FromCentimeter(1.15);
            }
            else
            {
                var missing =
                    content.AddFormattedText(
                        "Sin firma registrada"
                    );

                missing.Font.Size =
                    Unit.FromPoint(8);

                missing.Font.Color =
                    MutedColor;
            }
        }

        private static void AddSectionTitle(
            Section section,
            string title
        )
        {
            var paragraph =
                section.AddParagraph();

            paragraph.Format.SpaceBefore =
                Unit.FromPoint(6);

            paragraph.Format.SpaceAfter =
                Unit.FromPoint(3);

            paragraph.Format.Font.Size =
                Unit.FromPoint(10.5);

            paragraph.Format.Font.Bold =
                true;

            paragraph.Format.Font.Color =
                PrimaryColor;

            paragraph.AddText(title);
        }



        private static void AddSectionSpacing(
            Section section
        )
        {
            var paragraph =
                section.AddParagraph();

            paragraph.Format.SpaceAfter =
                Unit.FromPoint(4);
        }

        private static void AddSmallSpacing(
            Section section
        )
        {
            var paragraph =
                section.AddParagraph();

            paragraph.Format.SpaceAfter =
                Unit.FromPoint(2);
        }



        private static string FormatHours(
            decimal? value
        )
        {
            if (!value.HasValue)
            {
                return "—";
            }

            var wholeHours =
                decimal.ToInt32(
                    decimal.Truncate(
                        value.Value
                    )
                );

            var minutes =
                decimal.ToInt32(
                    (
                        value.Value
                        - decimal.Truncate(
                            value.Value
                        )
                    ) * 100m
                );

            if (minutes == 0)
            {
                return $"{wholeHours} h";
            }

            if (wholeHours == 0)
            {
                return $"{minutes} min";
            }

            return
                $"{wholeHours} h {minutes} min";
        }

        private static string FormatHoursCompact(
    decimal? value
)
        {
            if (!value.HasValue)
            {
                return "—";
            }

            var wholeHours =
                decimal.ToInt32(
                    decimal.Truncate(
                        value.Value
                    )
                );

            var minutes =
                decimal.ToInt32(
                    (
                        value.Value -
                        decimal.Truncate(
                            value.Value
                        )
                    ) * 100m
                );

            return
                $"{wholeHours}.{minutes:00}";
        }




        private async Task<SignatureAssets>
            LoadSignatureAssetsAsync(
                TrainingReportDetailsDto report,
                CancellationToken cancellationToken
            )
        {
            var assets =
                new SignatureAssets
                {
                    Instructor =
                        await DownloadSignatureAsync(
                            report.InstructorSignatureUrl,
                            cancellationToken
                        ),

                    Coordinator =
                        await DownloadSignatureAsync(
                            report.CoordinatorSignatureUrl,
                            cancellationToken
                        ),

                    Security =
                        await DownloadSignatureAsync(
                            report.SecuritySignatureUrl,
                            cancellationToken
                        )
                };

            foreach (
                var attendee
                in report.Attendees
            )
            {
                assets.Attendees[
                    attendee.Id
                ] =
                    new AttendeeSignatureAssets
                    {
                        Trainee =
                            await DownloadSignatureAsync(
                                attendee.TraineeSignatureUrl,
                                cancellationToken
                            ),

                        Supervisor =
                            await DownloadSignatureAsync(
                                attendee.SupervisorSignatureUrl,
                                cancellationToken
                            )
                    };
            }

            return assets;
        }

        private async Task<string?>
            DownloadSignatureAsync(
                string? fileUrl,
                CancellationToken cancellationToken
            )
        {
            if (string.IsNullOrWhiteSpace(
                fileUrl
            ))
            {
                return null;
            }

            var bytes =
                await blobStorageService
                    .DownloadFileTrainingReportAsync(
                        fileUrl,
                        cancellationToken
                    );

            if (
                bytes is null
                || bytes.Length == 0
            )
            {
                return null;
            }

            return
                "base64:"
                + Convert.ToBase64String(
                    bytes
                );
        }

        private sealed class SignatureAssets
        {
            public string? Instructor
            {
                get;
                set;
            }

            public string? Coordinator
            {
                get;
                set;
            }

            public string? Security
            {
                get;
                set;
            }

            public Dictionary<
                int,
                AttendeeSignatureAssets
            > Attendees
            {
                get;
            } = new();
        }

        private sealed class AttendeeSignatureAssets
        {
            public string? Trainee
            {
                get;
                set;
            }

            public string? Supervisor
            {
                get;
                set;
            }
        }
    }
}
