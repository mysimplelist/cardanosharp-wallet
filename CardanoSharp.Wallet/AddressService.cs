﻿
using System;
using CardanoSharp.Wallet.Common;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Utilities;

namespace CardanoSharp.Wallet
{
    public interface IAddressService
    {
        Address GetAddress(PublicKey payment, PublicKey stake, NetworkType networkType, AddressType addressType);
    }
    public class AddressService : IAddressService
    {
        public Address GetAddress(PublicKey payment, PublicKey stake, NetworkType networkType, AddressType addressType)
        {
            var networkInfo = getNetworkInfo(networkType);
            var paymentEncoded = HashUtility.Blake2b244(payment.Key);
            var stakeEncoded = HashUtility.Blake2b244(stake.Key);

            //get prefix
            var prefix = $"{getPrefixHeader(addressType)}{getPrefixTail(networkType)}";

            //get header
            var header = getAddressHeader(networkInfo, addressType);
            //get body
            byte[] addressArray;
            switch (addressType)
            {
                case AddressType.Base:
                    addressArray = new byte[1 + paymentEncoded.Length + stakeEncoded.Length];
                    addressArray[0] = header;
                    Buffer.BlockCopy(paymentEncoded, 0, addressArray, 1, paymentEncoded.Length);
                    Buffer.BlockCopy(stakeEncoded, 0, addressArray, paymentEncoded.Length + 1, stakeEncoded.Length);
                    break;
                case AddressType.Enterprise:
                case AddressType.Reward:
                    addressArray = new byte[1 + paymentEncoded.Length];
                    addressArray[0] = header;
                    Buffer.BlockCopy(paymentEncoded, 0, addressArray, 1, paymentEncoded.Length);
                    break;
                default:
                    throw new Exception("Unknown address type");
            }

            return new Address(prefix, addressArray);
        }

        private string getPrefixHeader(AddressType addressType) =>
            addressType switch
            {
                AddressType.Reward => "stake",
                AddressType.Base => "addr",
                AddressType.Enterprise => "addr",
                _ => throw new Exception("Unknown address type")
            };

        private string getPrefixTail(NetworkType networkType) =>
            networkType switch
            {
                NetworkType.Testnet => "_test",
                NetworkType.Mainnet => "",
                _ => throw new Exception("Unknown address type")
            };

        private NetworkInfo getNetworkInfo(NetworkType type) =>
            type switch
            {
                NetworkType.Testnet => new NetworkInfo(0b0000, 1097911063),
                NetworkType.Mainnet => new NetworkInfo(0b0001, 764824073),
                _ => throw new Exception("Unknown network type")
            };

        private byte getAddressHeader(NetworkInfo networkInfo, AddressType addressType) =>
            addressType switch
            {
                AddressType.Base => (byte)(networkInfo.NetworkId & 0xF),
                AddressType.Enterprise => (byte)(0b0110_0000 | networkInfo.NetworkId & 0xF),
                AddressType.Reward => (byte)(0b1110_0000 | networkInfo.NetworkId & 0xF),
                _ => throw new Exception("Unknown address type")
            };

    }
}
