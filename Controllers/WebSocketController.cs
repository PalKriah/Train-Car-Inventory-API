using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Train_Car_Inventory_App.Models;
using Train_Car_Inventory_App.Services;

namespace Train_Car_Inventory_App.Controllers
{
    [ApiController]
    [Route("ws")]
    [Route("[controller]")]
    public class WebSocketController : ControllerBase
    {
        private readonly ITrainCarService _trainCarService;

        public WebSocketController(ITrainCarService trainCarService)
        {
            _trainCarService = trainCarService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await Echo(webSocket);
                HttpContext.Response.StatusCode = 202;
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }

        private async Task Echo(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                var data = Encoding.UTF8.GetString(buffer, 0, result.Count);

                if (result.Count == 0)
                {
                    throw new WebSocketException();
                }

                var trainCars = JArray.Parse(data);

                await WebSocketSendData(webSocket, "Processing finished");

                await WebSocketSendData(webSocket, "Starting upload...");

                var trainCarsList = trainCars.Select(x => x.ToObject<TrainCar>()).ToList();

                var payload = new
                {
                    Count = trainCarsList.Count(),
                    TrainCars = new List<object>()
                };

                foreach (var trainCar in trainCarsList)
                {
                    var trainCarId = _trainCarService.Create(trainCar).Id;
                    var newTrainCar = _trainCarService.Get(trainCarId);
                    payload.TrainCars.Add(new
                    {
                        newTrainCar.Id,
                        newTrainCar.SerialNumber,
                        newTrainCar.ManufacturingYear,
                        LocationName = newTrainCar.Location.Name
                    });
                }

                await WebSocketSendData(webSocket, JObject.FromObject(payload).ToString());
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        private static async Task WebSocketSendData(WebSocket webSocket, string data)
        {
            var dataAsBytes = Encoding.UTF8.GetBytes(data);
            await webSocket.SendAsync(new ArraySegment<byte>(dataAsBytes, 0, dataAsBytes.Length),
                WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}