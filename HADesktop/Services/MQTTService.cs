using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using MQTTnet;

namespace HADesktop.Services
{
    internal class MQTTService : IDisposable
    {
        IMqttClient client;

        AppSettingsService appSettingsService = AppLocator.Current.GetRequiredService<AppSettingsService>();
        SchedulingService schedulingService = AppLocator.Current.GetRequiredService<SchedulingService>();
        ScheduledTask? reconnectTask;
        MqttClientOptions? mqttClientOptions;
        Dictionary<string, MqttApplicationMessage> messagequeue = [];
        SemaphoreSlim queueKey = new(1, 1);
        SemaphoreSlim clientKey = new(1, 1);

        public MQTTService()
        {
            var mqttFactory = new MqttClientFactory();
            client = mqttFactory.CreateMqttClient();
        }

        public async Task StartConnect()
        {
            try
            {
                if (client.IsConnected)
                {
                    await Disconnect();
                }
                mqttClientOptions = new MqttClientOptionsBuilder()
                    .WithTcpServer(appSettingsService.Settings.Server, appSettingsService.Settings.Port)
                    .WithTlsOptions(b =>
                    {
                        b.UseTls(appSettingsService.Settings.TLS);
                        b.WithAllowUntrustedCertificates(appSettingsService.Settings.AllowUntrustedTLS);
                        b.WithCertificateValidationHandler(MqttClientDefaultCertificateValidationHandler.Handle);
                        //b.WithCertificateValidationHandler(e => { return true; });
                    }/*new MqttClientTlsOptions() { UseTls = appSettingsService.Settings.TLS, AllowUntrustedCertificates = appSettingsService.Settings.AllowUntrustedTLS, CertificateValidationHandler }*/)
                    .WithCredentials(appSettingsService.Settings.Username, appSettingsService.Settings.Password)
                    .Build();
                reconnectTask = schedulingService.Schedule(Reconnect, DateTime.Now, TimeSpan.FromSeconds(30));
            }
            catch (Exception ex)
            {
                //TODO
            }
        }

        public async Task ConnectNow(int mstimeout = 5000)
        {
            try
            {
                var cts = new CancellationTokenSource(mstimeout);
                await Reconnect(cts.Token);
            }
            catch (Exception ex)
            {
                //TODO
            }
        }

        async ValueTask Reconnect(CancellationToken token)
        {
            using (await clientKey.LockAsync())
            {
                if (!client.IsConnected)
                {
                    if (mqttClientOptions == null)
                    {
                        return;
                    }
                    try
                    {
                        var response = await client.ConnectAsync(mqttClientOptions, token);
                        if (response.ResultCode != MqttClientConnectResultCode.Success)
                        {
                            //TODO
                        }
                    }
                    catch (Exception ex)
                    {
                        //TODO
                    }
                    if (client.IsConnected)
                    {
                        try
                        {
                            //client.SubscribeAsync
                        }
                        catch (Exception ex)
                        {
                            //todo
                        }
                    }
                }
            }
            if (token.IsCancellationRequested)
            {
                return;
            }
            if (client.IsConnected && messagequeue.Count > 0)
            {
                while (client.IsConnected && messagequeue.Count > 0)
                {
                    MqttApplicationMessage data;
                    using (await queueKey.LockAsync())
                    {
                        if (!messagequeue.Remove(messagequeue.Keys.First(), out data!))
                        {
                            continue;//no clue what happened here
                        }
                    }
                    try
                    {
                        var result = await client.PublishAsync(data, token);
                        if (!result.IsSuccess)
                        {
                            //TODO
                        }
                    }
                    catch (Exception ex)
                    {
                        //TODO
                    }
                    if (!token.IsCancellationRequested)
                    {
                        await Task.Delay(100);
                    }
                }
                if (messagequeue.Count == 0)
                {
                    messagequeue.TrimExcess();
                }
            }
        }

        public async Task Disconnect()
        {
            try
            {
                reconnectTask?.Cancel();
                reconnectTask = null;
                if (client.IsConnected)
                {
                    await client.DisconnectAsync(new MqttClientDisconnectOptions(), CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                //TODO
            }
        }

        public void Dispose()
        {
            client.Dispose();
        }
        public async void Publish(string topic, string? message, bool retain = false)
        {
            await PublishAsync(topic, message, retain).ConfigureAwait(false);
        }

        public async Task PublishAsync(string topic, string? message, bool retain = false)
        {
            try
            {
                var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(message)
                .WithRetainFlag(retain)
                .Build();
                using (await queueKey.LockAsync())
                {
                    if (!client.IsConnected || (messagequeue.Count > 0 && messagequeue.ContainsKey(topic)))
                    {
                        //todo log
                        messagequeue[topic] = applicationMessage;
                        return;
                    }
                }
                var result = await client.PublishAsync(applicationMessage, CancellationToken.None);
                if (!result.IsSuccess)
                {
                    //TODO
                }
            }
            catch (Exception ex)
            {
                //TODO
            }
        }
    }
}
