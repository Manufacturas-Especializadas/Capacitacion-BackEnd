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

            foreach (var attendee in report.Attendees)
            {
                AddAttendee(
                    section,
                    attendee,
                    signatureAssets
                );
            }

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
                Unit.FromPoint(9);

            normal.Font.Color =
                TextColor;

            normal.ParagraphFormat.SpaceAfter =
                Unit.FromPoint(4);
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
                Unit.FromPoint(8);

            var titleCell =
                row.Cells[0];

            var title =
                titleCell.AddParagraph();

            title.Format.Font.Size =
                Unit.FromPoint(20);

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
                CreateInformationTable(section);

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
                "Nómina del líder",
                report.LeaderPayroll
            );

            AddKeyValueCell(
                secondRow.Cells[1],
                "Semana",
                report.WeekNumber?.ToString()
                    ?? "No especificada"
            );

            var thirdRow =
                table.AddRow();

            AddKeyValueCell(
                thirdRow.Cells[0],
                "Fecha de creación",
                report.CreatedAt.ToString(
                    "dd/MM/yyyy HH:mm",
                    CultureInfo.InvariantCulture
                )
            );

            AddKeyValueCell(
                thirdRow.Cells[1],
                "Asistentes registrados",
                report.Attendees.Count.ToString()
            );

            AddSectionSpacing(section);
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
            SignatureAssets signatureAssets
        )
        {
            AddSectionTitle(
                section,
                $"Asistente · {attendee.EmployeeName}"
            );

            var attendeeHeader =
                section.AddParagraph();

            attendeeHeader.Format.SpaceAfter =
                Unit.FromPoint(6);

            var payrollText =
                attendeeHeader.AddFormattedText(
                    $"Nómina: {attendee.EmployeeNumber}",
                    TextFormat.Bold
                );

            payrollText.Color =
                PrimaryColor;

            var information =
                BuildAttendeeInformation(
                    attendee
                );

            AddDynamicInformationTable(
                section,
                information
            );

            AddTopics(
                section,
                attendee
            );

            AddSchedule(
                section,
                attendee
            );

            signatureAssets.Attendees.TryGetValue(
                attendee.Id,
                out var attendeeSignatures
            );

            AddAttendeeSignatures(
                section,
                attendeeSignatures
            );

            var separator =
                section.AddParagraph();

            separator.Format.SpaceBefore =
                Unit.FromPoint(8);

            separator.Format.SpaceAfter =
                Unit.FromPoint(12);

            separator.Format.Borders.Bottom.Width =
                Unit.FromPoint(0.8);

            separator.Format.Borders.Bottom.Color =
                BorderColor;
        }

        private static List<(string Label, string Value)>
            BuildAttendeeInformation(
                TrainingReportAttendeeDetailsDto attendee
            )
        {
            var items =
                new List<(string, string)>
                {
                    (
                        "Línea",
                        attendee.LineName
                    )
                };

            AddOptionalItem(
                items,
                "Turno",
                attendee.Shift
            );

            AddOptionalItem(
                items,
                "Maquinaria",
                attendee.Machinery
            );

            AddOptionalItem(
                items,
                "AST",
                attendee.Ast
            );

            AddOptionalItem(
                items,
                "Cliente",
                attendee.CustomerClient
            );

            AddOptionalItem(
                items,
                "Clasificación de unión",
                attendee.UnionClassification
            );

            AddOptionalItem(
                items,
                "Porcentaje de soldadura",
                attendee.WeldingPercentage
            );

            AddOptionalItem(
                items,
                "Diámetro",
                attendee.Diameter
            );

            return items;
        }

        private static void AddTopics(
            Section section,
            TrainingReportAttendeeDetailsDto attendee
        )
        {
            AddSubsectionTitle(
                section,
                "Temas"
            );

            if (attendee.Topics.Count == 0)
            {
                var empty =
                    section.AddParagraph(
                        "Sin temas registrados."
                    );

                empty.Format.Font.Color =
                    MutedColor;

                return;
            }

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
                LightBackgroundColor;

            AddTableHeader(
                header.Cells[0],
                "Código"
            );

            AddTableHeader(
                header.Cells[1],
                "Tema"
            );

            foreach (
                var topic
                in attendee.Topics
            )
            {
                var row =
                    table.AddRow();

                AddTableValue(
                    row.Cells[0],
                    topic.TopicCode
                );

                AddTableValue(
                    row.Cells[1],
                    topic.TopicName
                );
            }

            AddSmallSpacing(section);
        }

        private static void AddSchedule(
            Section section,
            TrainingReportAttendeeDetailsDto attendee
        )
        {
            AddSubsectionTitle(
                section,
                "Días y horas de capacitación"
            );

            var table =
                section.AddTable();

            table.Borders.Color =
                BorderColor;

            table.Borders.Width =
                Unit.FromPoint(0.5);

            table.AddColumn(
                Unit.FromCentimeter(9)
            );

            table.AddColumn(
                Unit.FromCentimeter(8.9)
            );

            var header =
                table.AddRow();

            header.Shading.Color =
                LightBackgroundColor;

            AddTableHeader(
                header.Cells[0],
                "Día"
            );

            AddTableHeader(
                header.Cells[1],
                "Horas"
            );

            AddDayRow(
                table,
                "Lunes",
                attendee.DayMonday,
                attendee.HoursMonday
            );

            AddDayRow(
                table,
                "Martes",
                attendee.DayTuesday,
                attendee.HoursTuesday
            );

            AddDayRow(
                table,
                "Miércoles",
                attendee.DayWednesday,
                attendee.HoursWednesday
            );

            AddDayRow(
                table,
                "Jueves",
                attendee.DayThursday,
                attendee.HoursThursday
            );

            AddDayRow(
                table,
                "Viernes",
                attendee.DayFriday,
                attendee.HoursFriday
            );

            AddDayRow(
                table,
                "Sábado",
                attendee.DaySaturday,
                attendee.HoursSaturday
            );

            AddDayRow(
                table,
                "Domingo",
                attendee.DaySunday,
                attendee.HoursSunday
            );

            var totalRow =
                table.AddRow();

            totalRow.Shading.Color =
                PrimarySoftColor;

            AddTableHeader(
                totalRow.Cells[0],
                "TOTAL"
            );

            var totalParagraph =
                totalRow.Cells[1]
                    .AddParagraph();

            totalParagraph.Format.Alignment =
                ParagraphAlignment.Center;

            totalParagraph.Format.Font.Bold =
                true;

            totalParagraph.Format.Font.Color =
                PrimaryColor;

            totalParagraph.AddText(
                FormatHours(
                    attendee.TotalHours
                )
            );

            AddSmallSpacing(section);
        }

        private static void AddDayRow(
            Table table,
            string dayName,
            bool selected,
            decimal? hours
        )
        {
            if (!selected)
            {
                return;
            }

            var row =
                table.AddRow();

            AddTableValue(
                row.Cells[0],
                dayName
            );

            var hoursParagraph =
                row.Cells[1]
                    .AddParagraph();

            hoursParagraph.Format.Alignment =
                ParagraphAlignment.Center;

            hoursParagraph.Format.Font.Bold =
                true;

            hoursParagraph.AddText(
                FormatHours(hours)
            );
        }

        private static void AddAttendeeSignatures(
            Section section,
            AttendeeSignatureAssets? assets
        )
        {
            AddSubsectionTitle(
                section,
                "Firmas del asistente"
            );

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

            var row =
                table.AddRow();

            AddSignatureCell(
                row.Cells[0],
                "Firma del asistente",
                assets?.Trainee
            );

            AddSignatureCell(
                row.Cells[1],
                "Firma del supervisor",
                assets?.Supervisor
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

            var table =
                section.AddTable();

            table.AddColumn(
                Unit.FromCentimeter(17.9)
            );

            table.Borders.Color =
                BorderColor;

            table.Borders.Width =
                Unit.FromPoint(0.5);

            var row =
                table.AddRow();

            row.Cells[0].Shading.Color =
                LightBackgroundColor;

            var paragraph =
                row.Cells[0]
                    .AddParagraph();

            paragraph.Format.SpaceBefore =
                Unit.FromPoint(5);

            paragraph.Format.SpaceAfter =
                Unit.FromPoint(5);

            paragraph.AddText(
                string.IsNullOrWhiteSpace(
                    observations
                )
                    ? "Sin observaciones registradas."
                    : observations.Trim()
            );

            AddSectionSpacing(section);
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

        private static void AddDynamicInformationTable(
            Section section,
            List<(string Label, string Value)> items
        )
        {
            var table =
                CreateInformationTable(section);

            for (
                var index = 0;
                index < items.Count;
                index += 2
            )
            {
                var row =
                    table.AddRow();

                var first =
                    items[index];

                AddKeyValueCell(
                    row.Cells[0],
                    first.Label,
                    first.Value
                );

                if (index + 1 < items.Count)
                {
                    var second =
                        items[index + 1];

                    AddKeyValueCell(
                        row.Cells[1],
                        second.Label,
                        second.Value
                    );
                }
            }

            AddSmallSpacing(section);
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
                Unit.FromPoint(6);

            content.Format.SpaceAfter =
                Unit.FromPoint(6);

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
                    Unit.FromCentimeter(1.6);
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
                Unit.FromPoint(10);

            paragraph.Format.SpaceAfter =
                Unit.FromPoint(6);

            paragraph.Format.Font.Size =
                Unit.FromPoint(12);

            paragraph.Format.Font.Bold =
                true;

            paragraph.Format.Font.Color =
                PrimaryColor;

            paragraph.AddText(title);
        }

        private static void AddSubsectionTitle(
            Section section,
            string title
        )
        {
            var paragraph =
                section.AddParagraph();

            paragraph.Format.SpaceBefore =
                Unit.FromPoint(7);

            paragraph.Format.SpaceAfter =
                Unit.FromPoint(4);

            paragraph.Format.Font.Size =
                Unit.FromPoint(9);

            paragraph.Format.Font.Bold =
                true;

            paragraph.Format.Font.Color =
                TextColor;

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

        private static void AddOptionalItem(
            ICollection<(string Label, string Value)> items,
            string label,
            string? value
        )
        {
            if (string.IsNullOrWhiteSpace(
                value
            ))
            {
                return;
            }

            items.Add(
                (
                    label,
                    value.Trim()
                )
            );
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