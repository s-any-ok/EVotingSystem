using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Users.Data;

namespace Game.Vote.Classes
{
    public class UserChain
    {
        public UserChain Next { get; set; }
        public UserChain Prev { get; set; }

        private User _user;
        private byte[] _myBulletin;
        private Dictionary<int, string> _usersRandomStrings = new();
        private List<(byte[] msg, byte[], int id)> _bulletins = new();

        public List<(byte[] msg, byte[], int id)> Bulletins => _bulletins;

        public UserChain(User user)
        {
            _user = user;
        }

        public byte[] EncryptRSA(byte[] msg)
        {
            var cur = _user.ApplyPublicKey(msg);
            return Next?.EncryptRSA(cur) ?? cur;
        }
    
        public byte[] EncryptRSAWithStr(byte[] msg, int id)
        {
            var randomStr = StringGenerator.GenerateRandom();
            var randomStrBytes = Encoding.UTF8.GetBytes(randomStr);

            var newBulletin = msg.Concat(randomStrBytes).ToArray();
        
            _usersRandomStrings.Add(id, randomStr);
        
            var cur = _user.ApplySecondPublicKey(newBulletin);

            if (Prev != null)
            {
                Prev.SaveMyBulletin(cur, id);
            }

            return Next?.EncryptRSAWithStr(cur, id) ?? cur;
        }
    
        public (byte[] msg, byte[] signed, int id)[] DencryptRSAAndSignBatch((byte[] msg, byte[] signed, int id)[] bulletins)
        {
            var foundMy = false;
            var signed = new List<(byte[] msg, byte[] signed, int id)>();

            foreach (var b in bulletins)
            {
                if (b.id == _user.Id)
                {
                    foundMy = true;
                }

                if (Next != null && !Next.VerifySign(b.msg, b.signed))
                {
                    throw new Exception("invalid sign");
                }
            
                var decrypted = _user.ApplyPrivateKey(b.msg);
                var signedMsg = Sign(decrypted);
            
                signed.Add((decrypted, signedMsg, b.id));
            }

            if (_myBulletin != null && !foundMy) throw new Exception("where is my bulletin? sign step");

            var signedArr = Shuffle(signed.ToArray());

            return Prev != null ? Prev.DencryptRSAAndSignBatch(signedArr) : signedArr;
        }
    
        private byte[] DencryptRSARemoveStr(byte[] msg, int id)
        {
            var decrypted = _user.ApplySecondPrivateKey(msg);

            var str = _usersRandomStrings[id];
            if (String.IsNullOrEmpty(str))
            {
                throw new Exception("no random string");
            }
        
            var strBytes = Encoding.UTF8.GetBytes(str);
        
            if (decrypted.TakeLast(strBytes.Length).Except(strBytes).Count() != 0)
            {
                throw new Exception("missing bulletins");
            }

            var withoutStrBytes = decrypted.Take(decrypted.Length - strBytes.Length).ToArray();
        
            return withoutStrBytes;
        }

        public (byte[] msg, byte[], int id)[] DencryptRSARemoveStrBatch((byte[] msg, byte[], int id)[] batch)
        {
            List<(byte[] msg, byte[], int id)> newBatches = new();
        
            foreach (var b in batch)
            {
                var decrypted = DencryptRSARemoveStr(b.msg, b.id);

                newBatches.Add((decrypted, null, b.id));
            }

            if (_myBulletin != null && !newBatches.Select(b => b.msg).Contains(_myBulletin))
            {
                throw new Exception("where is my bulletin?");
            }

            var newBatchesArr = Shuffle(newBatches.ToArray());
        
            return Prev != null ? Prev.DencryptRSARemoveStrBatch(newBatchesArr) : newBatchesArr;
        }

        public bool VerifySign(byte[] msg, byte[] signature)
        {
            return _user.CheckIfSigned(signature, msg);
        }

        public void AcceptBulletin(byte[] msg, int id)
        {
            if (_bulletins.Any(b => b.id == id))
            {
                throw new Exception("user already voted");
            }
        
            _bulletins.Add((msg, null, id));
        }
        
        private byte[] Sign(byte[] msg)
        {
            return _user.SignWithPrivateKey(msg);
        }

        private void SaveMyBulletin(byte[] msg, int id)
        {
            if (id != _user.Id)
            {
                return;
            }

            _myBulletin = msg;
        }

        private (byte[] msg, byte[] signed, int id)[] Shuffle((byte[] msg, byte[] signed, int id)[] arr)
        {
            Random random = new Random();
            arr = arr.OrderBy(x => random.Next()).ToArray();
            return arr;
        }
    }
}