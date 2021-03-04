using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QRCoder;
using System;
using System.Drawing;
using System.IO;

namespace CD.QRCoder.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QrCodeController : ControllerBase
    {
        private readonly ILogger<QrCodeController> _logger;
        private readonly QRCodeGenerator _qrCodeGenerator;

        public QrCodeController(ILogger<QrCodeController> logger)
        {
            _logger = logger;
            _qrCodeGenerator = new QRCodeGenerator();
        }

        [HttpGet("GenerateQrCode/text")]
        public IActionResult GenerateQrCode(string text)
        {
            _logger.LogInformation($"Started generating code for {text}");

            QRCodeData qrCodeData = _qrCodeGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            var file = File(BitmapToBytes(qrCodeImage), "image/png");
            file.FileDownloadName = $"{text}.png";

            _logger.LogInformation($"Generated qr file for {text} at {DateTime.Now}");

            return file;
        }

        private static Byte[] BitmapToBytes(Bitmap img)
        {
            using MemoryStream stream = new MemoryStream();
            img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            return stream.ToArray();
        }


    }
}
