﻿using CardanoSharp.Wallet.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardanoSharp.Wallet.TransactionBuilding
{
    public interface ITokenBundleBuilder: IABuilder<Dictionary<byte[], NativeAsset>>
    {
        ITokenBundleBuilder AddToken(byte[] policyId, byte[] asset, ulong amount);
    }

    public class TokenBundleBuilder: ABuilder<Dictionary<byte[], NativeAsset>>, ITokenBundleBuilder
    {
        private TokenBundleBuilder()
        {
            _model = new Dictionary<byte[], NativeAsset>();
        }

        public static ITokenBundleBuilder Create
        {
            get => new TokenBundleBuilder();
        }

        public ITokenBundleBuilder AddToken(byte[] policyId, byte[] asset, ulong amount)
        {
            var policy = _model.FirstOrDefault(x => x.Key.Equals(policyId));
            if (policy.Key is null)
            {
                policy = new KeyValuePair<byte[], NativeAsset>(policyId, new NativeAsset());
                _model.Add(policy.Key, policy.Value);
            }

            policy.Value.Token.Add(asset, amount);
            return this;
        }
    }
}
