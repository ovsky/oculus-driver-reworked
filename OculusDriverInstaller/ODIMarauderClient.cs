using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Daybreak.Net;

namespace OculusDriverInstaller
{
	public class ODIMarauderClient : MarauderClient
	{
		public ODIMarauderClient(Networker networker, long appId, string accessToken, Dictionary<string, string> substitutions = null, Action<CancellationToken> initialisationAction = null)
			: base(networker, appId, accessToken, substitutions, initialisationAction, () => Task.FromResult(result: true))
		{
		}

		public override async Task Shutdown()
		{
			await Tick();
		}
	}
}
