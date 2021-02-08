using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DB;
using Server.Game;
using ServerCore;

namespace Server
{
    public partial class ClientSession : PacketSession
    {
        public void HandleLogin(C_Login loginPacket)
        {
			// TODO : 이런 저런 보안 체크
			if (ServerState != PlayerServerState.ServerStateLogin)
				return;


			// TODO : 문제가 있다!
			// - 동시에 다른 사람이 같은 UniqueID를 보내면?
			// - 악의적으로 여러번 보낸다면?
			// - 쌩뚱맞은 타이밍에 그냥 이 패킷을 보내면?

			using (AppDbContext db = new AppDbContext())
			{
				AccountDb findAccount = db.Accounts
					.Include(a => a.Players)
					.Where(a => a.AccountName == loginPacket.UniqueId).FirstOrDefault();

				if (findAccount != null)
				{
					S_Login loginOk = new S_Login() { LoginOk = 1 };
					Send(loginOk);
				}
				else
				{
					AccountDb newAccount = new AccountDb() { AccountName = loginPacket.UniqueId };
					db.Accounts.Add(newAccount);
					db.SaveChanges(); // TODO : Exception

					S_Login loginOk = new S_Login() { LoginOk = 1 };
					Send(loginOk);
				}
			}
		}

    }
}
