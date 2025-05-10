// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using SIPSorcery.Media;
using SIPSorcery.Net;
using SIPSorceryMedia.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Realtime
{
    public sealed class RealtimeEndpointWebRTC : OpenAIBaseEndpoint
    {
        private const string OPENAI_DATACHANNEL_NAME = "oai-events";

        public readonly AudioEncoder AudioEncoder;

        public readonly AudioFormat AudioFormat;

        internal RealtimeEndpointWebRTC(OpenAIClient client) : base(client) {
            AudioEncoder = new AudioEncoder(includeOpus: true);
            AudioFormat = AudioEncoder.SupportedFormats.Single(x => x.FormatName == AudioCodecsEnum.OPUS.ToString());
        }

        protected override string Root => "realtime";

        protected override bool? IsWebSocketEndpoint => false;

        private RTCPeerConnection rtcPeerConnection;

        public event Action<IPEndPoint, SDPMediaTypesEnum, RTPPacket> OnRtpPacketReceived;

        public event Action OnPeerConnectionConnected;

        public event Action OnPeerConnectionClosedOrFailed;

        /// <summary>
        /// Creates a new realtime session with the provided <see cref="SessionConfiguration"/> options.
        /// </summary>
        /// <param name="configuration"><see cref="SessionConfiguration"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RealtimeSession"/>.</returns>
        public async Task<RealtimeSessionWebRTC> CreateSessionAsync(SessionConfiguration configuration = null, RTCConfiguration rtcConfiguration = null, CancellationToken cancellationToken = default)
        {
            rtcPeerConnection = await CreatePeerConnection(rtcConfiguration);
            var session = new RealtimeSessionWebRTC(rtcPeerConnection, EnableDebug);
            var sessionCreatedTcs = new TaskCompletionSource<SessionResponse>();

            try
            {
                session.OnEventReceived += OnEventReceived;
                session.OnError += OnError;
                var offerSdp = rtcPeerConnection.createOffer();
                var answerSdp = await SendSdpAsync(configuration?.Model, offerSdp.sdp);
                var setAnswerResult = rtcPeerConnection.setRemoteDescription(
                    new RTCSessionDescriptionInit { sdp = answerSdp, type = RTCSdpType.answer }
                );

                if (setAnswerResult != SetDescriptionResultEnum.OK)
                {
                    sessionCreatedTcs.TrySetException(new Exception("WebRTC SDP negotiation failed"));
                }

                var sessionResponse = await sessionCreatedTcs.Task.WithCancellation(cancellationToken).ConfigureAwait(false);
                session.Configuration = sessionResponse.SessionConfiguration;
                await session.SendAsync(new UpdateSessionRequest(configuration), cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                session.OnError -= OnError;
                session.OnEventReceived -= OnEventReceived;
            }

            return session;

            void OnError(Exception e)
            {
                sessionCreatedTcs.SetException(e);
            }

            void OnEventReceived(IRealtimeEvent @event)
            {
                try
                {
                    switch (@event)
                    {
                        case RealtimeConversationResponse:
                            Console.WriteLine("[conversation.created]");
                            break;
                        case SessionResponse sessionResponse:
                            if (sessionResponse.Type == "session.created")
                            {
                                sessionCreatedTcs.TrySetResult(sessionResponse);
                            }
                            break;
                        case RealtimeEventError realtimeEventError:
                            sessionCreatedTcs.TrySetException(new Exception(realtimeEventError.Error.Message));
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    sessionCreatedTcs.TrySetException(e);
                }
            }
        }

        private async Task<RTCPeerConnection> CreatePeerConnection(RTCConfiguration pcConfig)
        {
            var peerConnection = new RTCPeerConnection(pcConfig);
            MediaStreamTrack audioTrack = new MediaStreamTrack(AudioFormat, MediaStreamStatusEnum.SendRecv);
            peerConnection.addTrack(audioTrack);

            var dataChannel = await peerConnection.createDataChannel(OPENAI_DATACHANNEL_NAME);

            if (EnableDebug)
            {
                peerConnection.onconnectionstatechange += state => Console.WriteLine($"Peer connection connected changed to {state}.");
                peerConnection.OnTimeout += mediaType => Console.WriteLine($"Timeout on media {mediaType}.");
                peerConnection.oniceconnectionstatechange += state => Console.WriteLine($"ICE connection state changed to {state}.");

                peerConnection.onsignalingstatechange += () =>
                {
                    if (peerConnection.signalingState == RTCSignalingState.have_local_offer)
                    {
                        Console.WriteLine($"Local SDP:\n{peerConnection.localDescription.sdp}");
                    }
                    else if (peerConnection.signalingState is RTCSignalingState.have_remote_offer or RTCSignalingState.stable)
                    {
                        Console.WriteLine($"Remote SDP:\n{peerConnection.remoteDescription?.sdp}");
                    }
                };
            }

            peerConnection.OnRtpPacketReceived += (ep, mt, rtp) => OnRtpPacketReceived?.Invoke(ep, mt, rtp);

            peerConnection.onconnectionstatechange += (state) =>
            {
                if (state is RTCPeerConnectionState.closed or
                    RTCPeerConnectionState.failed or
                    RTCPeerConnectionState.disconnected)
                {
                    OnPeerConnectionClosedOrFailed?.Invoke();
                }
            };

            dataChannel.onopen += () => OnPeerConnectionConnected?.Invoke();

            dataChannel.onclose += () => OnPeerConnectionClosedOrFailed?.Invoke(); 

            return peerConnection;
        }

        public void SendAudio(uint durationRtpUnits, byte[] sample)
        {
            if(rtcPeerConnection != null && rtcPeerConnection.connectionState == RTCPeerConnectionState.connected)
            {
                rtcPeerConnection.SendAudio(durationRtpUnits, sample);
            }
        }

        public async Task<string> SendSdpAsync(string model, string offerSdp, CancellationToken cancellationToken = default)
        {
            model = string.IsNullOrWhiteSpace(model) ? Models.Model.GPT4oRealtime : model;
            var queryParameters = new Dictionary<string, string>();

            if (client.OpenAIClientSettings.IsAzureOpenAI)
            {
                queryParameters["deployment"] = model;
            }
            else
            {
                queryParameters["model"] = model;
            }

            var content = new StringContent(offerSdp, Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/sdp");

            var url = GetUrl(queryParameters: queryParameters);
            using var response = await client.Client.PostAsync(GetUrl(queryParameters: queryParameters), content, cancellationToken).ConfigureAwait(false);

            if(!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error sending SDP offer {errorBody}");
            }

            var sdpAnswer = await response.ReadAsStringAsync(EnableDebug, content, cancellationToken).ConfigureAwait(false);
            return sdpAnswer;
        }
    }
}
